using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Catchy.Analyzers.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AwaitAssertionCodeFix)), Shared]
public class AwaitAssertionCodeFix : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds
        => [Diagnostics.AssertionAwaitAnalyzer.DiagnosticId];

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return;
        }

        var diag = context.Diagnostics[0];
        if (root.FindNode(diag.Location.SourceSpan) is not InvocationExpressionSyntax node)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create("Add await", c => AddAwaitAsync(context.Document, node, c), nameof(AddAwaitAsync)),
            context.Diagnostics);

        context.RegisterCodeFix(
            CodeAction.Create("Prefix with `_ =`", c => AddDiscardAsync(context.Document, node, c), nameof(AddDiscardAsync)),
            context.Diagnostics);
    }

    private static async Task<Document> AddAwaitAsync(Document doc, InvocationExpressionSyntax node, CancellationToken ct)
    {
        var root = await doc.GetSyntaxRootAsync(ct).ConfigureAwait(false);
        if (root is null)
        {
            return doc;
        }

        var awaitExpr = SyntaxFactory.AwaitExpression(node.WithoutTrivia()).WithTriviaFrom(node);
        var newRoot = root.ReplaceNode(node, awaitExpr);
        return doc.WithSyntaxRoot(newRoot);
    }

    private static async Task<Document> AddDiscardAsync(Document doc, InvocationExpressionSyntax node, CancellationToken ct)
    {
        var root = await doc.GetSyntaxRootAsync(ct).ConfigureAwait(false);
        if (root is null || node.Parent is not ExpressionStatementSyntax expressionStatement)
        {
            return doc;
        }

        var newNode = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.IdentifierName("_"),
                    node.WithoutTrivia()))
            .WithTriviaFrom(expressionStatement);

        var newRoot = root.ReplaceNode(expressionStatement, newNode);
        return doc.WithSyntaxRoot(newRoot);
    }
}

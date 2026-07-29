using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Catchy.Analyzers.Diagnostics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class AssertionAwaitAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CATCHY001";
        public const string DetachedAmbientSoftDiagnosticId = "CATCHY002";
        private static readonly LocalizableString Title = "Assertion chain must be awaited or its result consumed";
        private static readonly LocalizableString MessageFormat = "Assertion chain should be awaited or its result used";
        private static readonly LocalizableString Description = "Fluent assertion chains return an object that starts an async pipeline; forgetting to await the chain will not execute the assertions.";
        private static readonly LocalizableString DetachedAmbientSoftTitle = "AmbientSoft soft assertions should not run in detached async contexts";
        private static readonly LocalizableString DetachedAmbientSoftMessage = "AmbientSoft soft assertions can lose context in detached async execution; use an explicit soft asserter instance instead";
        private static readonly LocalizableString DetachedAmbientSoftDescription = "AmbientSoft soft assertions depend on test-scoped ambient context. Detached execution like Task.Run or Parallel.ForEach can break accumulation and flush ownership; use an explicit soft asserter instance for detached work.";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description);

        private static readonly DiagnosticDescriptor DetachedAmbientSoftRule = new(
            DetachedAmbientSoftDiagnosticId,
            DetachedAmbientSoftTitle,
            DetachedAmbientSoftMessage,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: DetachedAmbientSoftDescription);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule, DetachedAmbientSoftRule];

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeInvocation(SyntaxNodeAnalysisContext ctx)
        {
            var invocation = (InvocationExpressionSyntax)ctx.Node;

            // resolve symbol
            if (ctx.SemanticModel.GetSymbolInfo(invocation).Symbol is not IMethodSymbol symbol) return;

            // check attribute AssertionMethod on method or its containing type
            var hasAttr = HasAssertionMethodAttribute(symbol) || HasAssertionMethodAttribute(symbol.ContainingType);
            if (!hasAttr) return;

            // If the method returns a type that is awaitable (has GetAwaiter) then it will be awaited normally.
            // But in our model the chain returns an assertion object; we still want to force awaiting the chain result later.

            if (IsDetachedAmbientSoftAssertion(invocation, ctx.SemanticModel))
            {
                var detachedDiag = Diagnostic.Create(DetachedAmbientSoftRule, invocation.GetLocation());
                ctx.ReportDiagnostic(detachedDiag);
            }

            if (!IsUnconsumedAssertionPattern(invocation))
                return;

            // report diagnostic at invocation location (message does not reference symbols/names)
            var diag = Diagnostic.Create(Rule, invocation.GetLocation());
            ctx.ReportDiagnostic(diag);
        }

        private static bool IsUnconsumedAssertionPattern(InvocationExpressionSyntax invocation)
        {
            ExpressionSyntax current = invocation;
            while (current.Parent is ParenthesizedExpressionSyntax parenthesized)
                current = parenthesized;

            return current.Parent is ExpressionStatementSyntax
                || current.Parent is EqualsValueClauseSyntax
                || current.Parent is AssignmentExpressionSyntax { Parent: ExpressionStatementSyntax };
        }

        private static bool IsDetachedAmbientSoftAssertion(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel)
        {
            return UsesAmbientSoftEntry(invocation, semanticModel)
                && IsInsideDetachedExecution(invocation, semanticModel);
        }

        private static bool UsesAmbientSoftEntry(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel)
        {
            foreach (var nestedInvocation in invocation.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>())
            {
                var symbol = semanticModel.GetSymbolInfo(nestedInvocation).Symbol as IMethodSymbol;
                if (symbol?.Name != "That")
                    continue;

                if (nestedInvocation.Expression is not MemberAccessExpressionSyntax memberAccess)
                    continue;

                if (semanticModel.GetSymbolInfo(memberAccess.Expression).Symbol is not IPropertySymbol receiverSymbol)
                    continue;

                var containingType = receiverSymbol.ContainingType?.ToDisplayString();
                if ((containingType == "Catchy.AmbientSoft" && receiverSymbol.Name == "Verify")
                    || (containingType == "Catchy.AmbientAlias" && receiverSymbol.Name == "Verify"))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsInsideDetachedExecution(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel)
        {
            var anonymousFunction = invocation.Ancestors().OfType<AnonymousFunctionExpressionSyntax>().FirstOrDefault();
            if (anonymousFunction is null)
                return false;

            if (anonymousFunction.Parent is not ArgumentSyntax argument
                || argument.Parent?.Parent is not InvocationExpressionSyntax detachedInvocation)
            {
                return false;
            }

            var symbolInfo = semanticModel.GetSymbolInfo(detachedInvocation);
            var method = symbolInfo.Symbol as IMethodSymbol
                ?? symbolInfo.CandidateSymbols.OfType<IMethodSymbol>().FirstOrDefault();

            if (method is null)
                return false;

            var containingType = method.ContainingType?.ToDisplayString();
            return (containingType == "System.Threading.Tasks.Task" && method.Name == "Run")
                || (containingType == "System.Threading.Tasks.TaskFactory" && method.Name == "StartNew")
                || (containingType == "System.Threading.Tasks.Parallel"
                    && (method.Name == "ForEach" || method.Name == "ForEachAsync" || method.Name == "Invoke"));
        }

        private static bool HasAssertionMethodAttribute(ISymbol sym)
        {
            if (sym is null) return false;
            return sym.GetAttributes()
                .Any(a => a.AttributeClass?.ToDisplayString() == "Catchy.Sdk.AssertionMethodAttribute");
        }
    }
}

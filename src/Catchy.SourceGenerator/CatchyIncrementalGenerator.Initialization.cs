using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Catchy.SourceGenerator
{
    public sealed partial class CatchyIncrementalGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var assertableTypes = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    AssertableAttributeFqn,
                    predicate: static (node, _) => node is TypeDeclarationSyntax,
                    transform: static (ctx, ct) => GetAssertableModel(ctx, ct))
                .Where(static m => m is not null)
                .Select(static (m, _) => m!);

            context.RegisterSourceOutput(assertableTypes,
                static (spc, model) => EmitAssertable(spc, model));

            var assertForClasses = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (node, _) => node is ClassDeclarationSyntax c
                        && c.Modifiers.Any(SyntaxKind.StaticKeyword)
                        && c.Modifiers.Any(SyntaxKind.PartialKeyword),
                    transform: static (ctx, ct) => GetAssertForModel(ctx, ct))
                .Where(static m => m is not null)
                .Select(static (m, _) => m!);

            context.RegisterSourceOutput(assertForClasses,
                static (spc, model) => EmitAssertFor(spc, model));

            var crossTypeRules = context.CompilationProvider
                .Select(static (compilation, _) => DiscoverCrossTypeRules(compilation));

            var inlineRulePairs = assertableTypes
                .SelectMany(static (model, _) => model.CrossTypes
                    .Where(ct => ct.IsResolved && ct.RegisterInGlobalRegistry && ct.FieldMaps.Length > 0)
                    .Select(ct => new RulePairModel(model.FullTypeName, ct.TargetTypeFullName))
                    .ToImmutableArray())
                .Collect();

            var crossAndInlinePairs = crossTypeRules.Combine(inlineRulePairs);

            context.RegisterSourceOutput(crossAndInlinePairs,
                static (spc, pair) => EmitCrossTypeRules(spc, pair.Left, pair.Right));

            var arityMethods = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    GenerateArityOverloadsFqn,
                    predicate: static (node, _) => node is MethodDeclarationSyntax,
                    transform: static (ctx, ct) => GetArityModel(ctx, ct))
                .Where(static m => m is not null)
                .Collect();

            context.RegisterSourceOutput(arityMethods,
                static (spc, models) => EmitArityOverloads(spc, models));

            var methodTemplates = context.CompilationProvider
                .Select(static (compilation, _) => BuildMethodTemplateMetadata(compilation));

            context.RegisterSourceOutput(methodTemplates,
                static (spc, metadata) => EmitMethodTemplates(spc, metadata));

            var assertEntryModels = context.CompilationProvider
                .Select(static (compilation, _) => DiscoverAssertEntryModels(compilation));

            context.RegisterSourceOutput(assertEntryModels,
                static (spc, models) => EmitAssertEntries(spc, models));

            var assertViaModels = context.CompilationProvider
                .Select(static (compilation, _) => DiscoverAssertViaModels(compilation));

            context.RegisterSourceOutput(assertViaModels,
                static (spc, models) => EmitAssertVias(spc, models));
        }
    }
}


using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Catchy.SourceGenerator
{
    /// <summary>Arity-overload forwarding info extracted from [GenerateArityOverloads] on a template method.</summary>
    public sealed record ArityForwardInfo(string Target, int From, int To);

    public sealed record MethodTemplateDiscoveredMethod(
        string MethodName,
        string ContainingTypeName,
        string ContainingNamespace,
        string ContainingTypeKindKeyword,
        string ContainingTypeModifiers,
        string FullyQualifiedMethodName,
        string ReturnTypeName,
        bool ReturnsVoid,
        bool IsExtensionMethod,
        string ReceiverParameterName,
        string ReceiverTypeName,
        string ReceiverTypeOriginalDefinitionName,
        string AdditionalParametersSignature,
        string InvocationArguments,
        string MethodBodySource,
        string TemplateValueTypeName,
        ImmutableArray<string> TemplateTargetTypeNames,
        string DocumentationCommentXml,
        string MethodModifiers,
        string WrapperMethodTypeParametersDeclaration,
        string WrapperMethodTypeConstraints,
        ImmutableArray<ArityForwardInfo> ArityForwards);

    public sealed class MethodTemplateDiscoveryResult
    {
        public ImmutableArray<MethodTemplateDiscoveredMethod> Methods { get; init; } = [];
        public ImmutableArray<GeneratorDiagnostic> Diagnostics { get; init; } = [];
    }

    public sealed class MethodTemplateDiscovery
    {
        public MethodTemplateDiscoveryResult Discover(Compilation compilation)
        {
            var methods = new List<MethodTemplateDiscoveredMethod>();
            var diagnostics = new List<GeneratorDiagnostic>();
            var seenMethods = new HashSet<string>(StringComparer.Ordinal);

            foreach (var assembly in TemplateDiscoveryHelpers.EnumerateAssemblies(compilation))
            {
                // Skip referenced assemblies — their typed-overload methods have already been
                // emitted by the generator during that assembly's own compilation.
                // Processing them here would fail (no syntax references available) and produce
                // duplicate / broken output.
                if (!SymbolEqualityComparer.Default.Equals(assembly, compilation.Assembly))
                {
                    continue;
                }

                foreach (var type in TemplateDiscoveryHelpers.EnumerateAllTypes(assembly.GlobalNamespace))
                {
                    foreach (var method in type.GetMembers().OfType<IMethodSymbol>())
                    {
                        var templateConfig = GetTemplateConfiguration(method);
                        if (templateConfig is null)
                        {
                            continue;
                        }

                        var fullyQualifiedMethodName = $"{type.ToDisplayString()}.{method.Name}";
                        var methodIdentity = $"{assembly.Name}|{fullyQualifiedMethodName}|{method.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)}";
                        if (!seenMethods.Add(methodIdentity))
                        {
                            continue;
                        }

                        var templateValueTypeName = templateConfig.Value.TemplateValueTypeName;
                        if (string.IsNullOrWhiteSpace(templateValueTypeName))
                        {
                            diagnostics.Add(new GeneratorDiagnostic
                            {
                                Code = "ASRT0101",
                                Severity = DiagnosticSeverity.Warning,
                                Message = $"Method {fullyQualifiedMethodName} has [GenerateTypedOverloads] but no template type could be inferred; add TemplateType = typeof(X) to the attribute."
                            });
                            continue;
                        }

                        if (templateConfig.Value.TargetTypeNames.Length == 0)
                        {
                            diagnostics.Add(new GeneratorDiagnostic
                            {
                                Code = "ASRT0102",
                                Severity = DiagnosticSeverity.Warning,
                                Message = $"Method {fullyQualifiedMethodName} has [GenerateTypedOverloads] but no target types were specified."
                            });
                            continue;
                        }

                        methods.Add(new MethodTemplateDiscoveredMethod(
                            MethodName: method.Name,
                            ContainingTypeName: type.ToDisplayString(),
                            ContainingNamespace: type.ContainingNamespace?.ToDisplayString() ?? string.Empty,
                            ContainingTypeKindKeyword: type.IsRecord ? "record" : (type.TypeKind == TypeKind.Struct ? "struct" : "class"),
                            ContainingTypeModifiers: BuildContainingTypeModifiers(type),
                            FullyQualifiedMethodName: fullyQualifiedMethodName,
                            ReturnTypeName: method.ReturnType.ToDisplayString(),
                            ReturnsVoid: method.ReturnsVoid,
                            IsExtensionMethod: method.IsExtensionMethod,
                            ReceiverParameterName: method.Parameters.FirstOrDefault()?.Name ?? "self",
                            ReceiverTypeName: TemplateDiscoveryHelpers.GetReceiverTypeName(method),
                            ReceiverTypeOriginalDefinitionName: TemplateDiscoveryHelpers.GetReceiverOriginalDefinitionName(method),
                            AdditionalParametersSignature: TemplateDiscoveryHelpers.BuildAdditionalParametersSignatureWithAttributes(method),
                            InvocationArguments: TemplateDiscoveryHelpers.BuildInvocationArguments(method),
                            MethodBodySource: TemplateDiscoveryHelpers.ExtractMethodBodySource(method),
                            TemplateValueTypeName: templateValueTypeName,
                            TemplateTargetTypeNames: templateConfig.Value.TargetTypeNames,
                            DocumentationCommentXml: TemplateDiscoveryHelpers.GetDocumentationCommentXmlSafe(method),
                            MethodModifiers: BuildMethodModifiers(method),
                            WrapperMethodTypeParametersDeclaration: TemplateDiscoveryHelpers.BuildWrapperMethodTypeParametersDeclaration(method),
                            WrapperMethodTypeConstraints: TemplateDiscoveryHelpers.BuildWrapperMethodTypeConstraints(method),
                            ArityForwards: GetArityForwards(method)));
                    }
                }
            }

            return new MethodTemplateDiscoveryResult
            {
                Methods = [.. methods
                    .OrderBy(m => m.ContainingTypeName, StringComparer.Ordinal)
                    .ThenBy(m => m.MethodName, StringComparer.Ordinal)
                    .ThenBy(static m => m.ReturnTypeName, StringComparer.Ordinal)],
                Diagnostics = [.. diagnostics
                    .OrderBy(d => d.Code, StringComparer.Ordinal)
                    .ThenBy(d => d.Message, StringComparer.Ordinal)]
            };
        }

        private static (string TemplateValueTypeName, ImmutableArray<string> TargetTypeNames)? GetTemplateConfiguration(IMethodSymbol method)
        {
            // Method-level attribute takes priority.
            var config = TryExtractTypedOverloadsConfig(method.GetAttributes(), method);
            if (config is not null)
                return config;

            // Fall back to class-level attribute.
            var classConfig = TryExtractTypedOverloadsConfig(method.ContainingType.GetAttributes(), method);
            if (classConfig is null)
                return null;

            // Skip already-generated overloads: when the receiver's type argument is one of the
            // target types the method was produced by a previous generation pass, not authored
            // as a template. Processing it again would create duplicate members.
            var receiverTypeFqn = InferTemplateValueTypeFqnName(method);
            if (!string.IsNullOrEmpty(receiverTypeFqn)
                && classConfig.Value.TargetTypeNames.Any(t => string.Equals(t, receiverTypeFqn, StringComparison.Ordinal)))
            {
                return null;
            }

            return classConfig;
        }

        private static (string TemplateValueTypeName, ImmutableArray<string> TargetTypeNames)? TryExtractTypedOverloadsConfig(
            ImmutableArray<AttributeData> attributes,
            IMethodSymbol method)
        {
            foreach (var attribute in attributes)
            {
                if (attribute.AttributeClass?.ToDisplayString() != CatchyIncrementalGenerator.GenerateTypedOverloadsAttributeFqn)
                {
                    continue;
                }

                // Constructor: (params Type[] targetTypes)
                // The single ctor argument is the params array of target types.
                var targetTypeNames = attribute.ConstructorArguments.Length > 0
                    ? [.. attribute.ConstructorArguments[0].Values
                        .Select(v => v.Value as ITypeSymbol)
                        .Where(v => v is not null)
                        .Select(v => v!.ToDisplayString())]
                    : ImmutableArray<string>.Empty;

                // Named property: TemplateType = typeof(X)  (optional override)
                var explicitTemplateValueTypeName = string.Empty;
                foreach (var namedArg in attribute.NamedArguments)
                {
                    if (string.Equals(namedArg.Key, "TemplateType", StringComparison.Ordinal)
                        && namedArg.Value.Value is ITypeSymbol explicitType)
                    {
                        explicitTemplateValueTypeName = explicitType.ToDisplayString();
                        break;
                    }
                }

                var templateValueTypeName = string.IsNullOrWhiteSpace(explicitTemplateValueTypeName)
                    ? InferTemplateValueTypeName(method)
                    : explicitTemplateValueTypeName;

                return (templateValueTypeName, targetTypeNames);
            }

            return null;
        }


        private static ImmutableArray<ArityForwardInfo> GetArityForwards(IMethodSymbol method)
        {
            var result = new List<ArityForwardInfo>();
            foreach (var attribute in method.GetAttributes())
            {
                if (attribute.AttributeClass?.ToDisplayString() != CatchyIncrementalGenerator.GenerateArityOverloadsFqn)
                    continue;

                // Ctor: (string target, int from = 2, int to = 5)
                var args = attribute.ConstructorArguments;
                var target = args.Length > 0 ? args[0].Value as string ?? string.Empty : string.Empty;
                var from = args.Length > 1 && args[1].Value is int f ? f : 2;
                var to   = args.Length > 2 && args[2].Value is int t ? t : 5;
                if (!string.IsNullOrWhiteSpace(target))
                    result.Add(new ArityForwardInfo(target, from, to));
            }
            return [.. result];
        }

        /// <summary>
        /// Infers the template value type name from the first parameter of the template method.
        /// Syntax is preferred over semantics because the method body is extracted as raw source
        /// text — the template type name must match exactly what the author wrote in the body
        /// (e.g. <c>BigInteger</c> from a <c>using System.Numerics;</c> import), NOT the
        /// fully-qualified <c>System.Numerics.BigInteger</c> that <c>ToDisplayString()</c>
        /// returns. Nullable wrappers are unwrapped: <c>int?</c> → <c>int</c>.
        /// </summary>
        private static string InferTemplateValueTypeName(IMethodSymbol method)
        {
            // Syntax-primary: reads the type token exactly as the template author wrote it.
            var syntaxName = TryInferTemplateValueTypeNameFromSyntax(method);
            if (!string.IsNullOrEmpty(syntaxName))
                return syntaxName;

            // Semantic fallback: used only when syntax is unavailable (e.g. metadata-only
            // symbols from referenced assemblies that somehow bypassed the early-out guard).
            var receiverType = method.Parameters.FirstOrDefault()?.Type as INamedTypeSymbol;
            if (receiverType?.TypeArguments.Length == 1)
            {
                var typeArg = receiverType.TypeArguments[0] as INamedTypeSymbol;
                if (typeArg?.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T
                    && typeArg.TypeArguments.Length == 1)
                {
                    return typeArg.TypeArguments[0].ToDisplayString();
                }
                return receiverType.TypeArguments[0].ToDisplayString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns the FQN of the value type argument of the first receiver parameter,
        /// including the nullable suffix when present (e.g. "System.DateTime?").
        /// Used as a secondary substitution key for semantic-origin strings
        /// (ReceiverTypeName, ReturnTypeName) which are produced by ToDisplayString().
        /// </summary>
        private static string InferTemplateValueTypeFqnName(IMethodSymbol method)
        {
            var receiverType = method.Parameters.FirstOrDefault()?.Type as INamedTypeSymbol;
            if (receiverType?.TypeArguments.Length != 1)
                return string.Empty;

            return receiverType.TypeArguments[0].ToDisplayString();
        }

        /// <summary>
        /// Reads the first type-argument of the first parameter from the method's own syntax.
        /// For <c>this ValueAssertions&lt;BigInteger&gt; a</c> returns <c>"BigInteger"</c>.
        /// For <c>this ValueAssertions&lt;int?&gt; a</c> returns <c>"int"</c> (nullable unwrapped).
        /// </summary>
        private static string TryInferTemplateValueTypeNameFromSyntax(IMethodSymbol method)
        {
            try
            {
                var syntax = method.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax()
                    as MethodDeclarationSyntax;
                var firstParam = syntax?.ParameterList.Parameters.FirstOrDefault();
                if (firstParam?.Type is not GenericNameSyntax generic
                    || generic.TypeArgumentList.Arguments.Count == 0)
                    return string.Empty;

                var typeArgSyntax = generic.TypeArgumentList.Arguments[0];

                // Unwrap nullable suffix: int? → int, BigInteger? → BigInteger.
                if (typeArgSyntax is NullableTypeSyntax nullable)
                    return nullable.ElementType.ToString().Trim();

                return typeArgSyntax.ToString().Trim();
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string BuildContainingTypeModifiers(INamedTypeSymbol type)
        {
            var modifiers = new List<string>
            {
                type.DeclaredAccessibility switch
                {
                    Accessibility.Public => "public",
                    Accessibility.Internal => "internal",
                    Accessibility.Private => "private",
                    Accessibility.Protected => "protected",
                    Accessibility.ProtectedAndInternal => "private protected",
                    Accessibility.ProtectedOrInternal => "protected internal",
                    _ => "internal"
                }
            };

            if (type.IsStatic)
            {
                modifiers.Add("static");
            }
            else
            {
                if (type.IsAbstract)
                {
                    modifiers.Add("abstract");
                }

                if (type.IsSealed)
                {
                    modifiers.Add("sealed");
                }
            }

            if (type.IsRecord && !type.IsStatic)
            {
                modifiers.Add(type.TypeKind == TypeKind.Struct ? "record struct" : "record");
                return string.Join(" ", modifiers);
            }

            modifiers.Add("partial");
            return string.Join(" ", modifiers);
        }

        private static string BuildMethodModifiers(IMethodSymbol method)
        {
            var modifiers = new List<string>
            {
                method.DeclaredAccessibility switch
                {
                    Accessibility.Public => "public",
                    Accessibility.Internal => "internal",
                    Accessibility.Private => "private",
                    Accessibility.Protected => "protected",
                    Accessibility.ProtectedAndInternal => "private protected",
                    Accessibility.ProtectedOrInternal => "protected internal",
                    _ => "internal"
                }
            };

            if (method.IsStatic)
            {
                modifiers.Add("static");
            }

            return string.Join(" ", modifiers);
        }
    }
}

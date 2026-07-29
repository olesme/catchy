using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Catchy.SourceGenerator
{
    internal static class TemplateDiscoveryHelpers
    {
        internal static IEnumerable<IAssemblySymbol> EnumerateAssemblies(Compilation compilation)
        {
            yield return compilation.Assembly;

            var referencedAssemblies = compilation.SourceModule.ReferencedAssemblySymbols
                .OrderBy(a => a.Name, StringComparer.Ordinal);

            foreach (var assembly in referencedAssemblies)
            {
                yield return assembly;
            }
        }

        internal static IEnumerable<INamedTypeSymbol> EnumerateAllTypes(INamespaceSymbol @namespace)
        {
            foreach (var member in @namespace.GetTypeMembers())
            {
                foreach (var type in EnumerateTypeAndNested(member))
                {
                    yield return type;
                }
            }

            foreach (var childNamespace in @namespace.GetNamespaceMembers())
            {
                foreach (var type in EnumerateAllTypes(childNamespace))
                {
                    yield return type;
                }
            }
        }

        internal static IEnumerable<INamedTypeSymbol> EnumerateTypeAndNested(INamedTypeSymbol type)
        {
            yield return type;

            foreach (var nested in type.GetTypeMembers())
            {
                foreach (var child in EnumerateTypeAndNested(nested))
                {
                    yield return child;
                }
            }
        }

        internal static string GetDocumentationCommentXmlSafe(IMethodSymbol method)
        {
            try
            {
                return method.GetDocumentationCommentXml(cancellationToken: default) ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        internal static string BuildAdditionalParametersSignature(IMethodSymbol method)
            => string.Join(", ", method.Parameters.Skip(1).Select(BuildParameterDeclaration));

        internal static string BuildAdditionalParametersSignatureWithAttributes(IMethodSymbol method)
        {
            var syntax = method.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as MethodDeclarationSyntax;
            var syntaxParams = syntax?.ParameterList.Parameters.Skip(1).ToArray();

            return string.Join(", ", method.Parameters.Skip(1).Select((p, i) =>
            {
                var semantic = BuildParameterDeclaration(p);
                if (syntaxParams is null || i >= syntaxParams.Length)
                    return semantic;

                var attrs = syntaxParams[i].AttributeLists.ToString();
                return string.IsNullOrWhiteSpace(attrs) ? semantic : $"{attrs} {semantic}";
            }));
        }

        internal static string BuildInvocationArguments(IMethodSymbol method)
            => string.Join(", ", method.Parameters.Skip(1).Select(p => p.Name));

        internal static string BuildWrapperMethodTypeParametersDeclaration(IMethodSymbol method)
        {
            var wrapperTypeParameters = GetWrapperTypeParameters(method);
            if (wrapperTypeParameters.Length == 0)
            {
                return string.Empty;
            }

            return $"<{string.Join(", ", wrapperTypeParameters.Select(tp => tp.Name))}>";
        }

        internal static string BuildWrapperMethodTypeConstraints(IMethodSymbol method)
        {
            var wrapperTypeParameters = GetWrapperTypeParameters(method);
            if (wrapperTypeParameters.Length == 0)
            {
                return string.Empty;
            }

            var wrapperTypeParameterNames = new HashSet<string>(wrapperTypeParameters.Select(tp => tp.Name), StringComparer.Ordinal);
            var clauses = new List<string>();

            foreach (var typeParameter in wrapperTypeParameters)
            {
                var constraints = new List<string>();

                if (typeParameter.HasNotNullConstraint)
                {
                    constraints.Add("notnull");
                }
                else if (typeParameter.HasReferenceTypeConstraint)
                {
                    constraints.Add("class");
                }
                else if (typeParameter.HasValueTypeConstraint)
                {
                    constraints.Add("struct");
                }

                constraints.AddRange(typeParameter.ConstraintTypes
                    .Select(t => t.ToDisplayString())
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Select(c => c.Trim())
                    .Where(c => DoesNotDependOnMissingTypeParameter(c, wrapperTypeParameterNames)));

                if (typeParameter.HasUnmanagedTypeConstraint)
                {
                    constraints.Add("unmanaged");
                }

                if (typeParameter.HasConstructorConstraint)
                {
                    constraints.Add("new()");
                }

                if (constraints.Count > 0)
                {
                    clauses.Add($"where {typeParameter.Name} : {string.Join(", ", constraints)}");
                }
            }

            return clauses.Count == 0 ? string.Empty : string.Join(" ", clauses);
        }

        internal static string GetReceiverTypeName(IMethodSymbol method)
            => method.Parameters.FirstOrDefault()?.Type.ToDisplayString() ?? string.Empty;

        internal static string GetReceiverOriginalDefinitionName(IMethodSymbol method)
            => method.Parameters.FirstOrDefault()?.Type is INamedTypeSymbol named
                ? named.OriginalDefinition.ToDisplayString()
                : method.Parameters.FirstOrDefault()?.Type.ToDisplayString() ?? string.Empty;

        internal static string ExtractMethodBodySource(IMethodSymbol method)
        {
            try
            {
                var syntax = method.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();
                if (syntax is null) return "// Method body could not be extracted";

                var methodDeclaration = syntax as MethodDeclarationSyntax;
                if (methodDeclaration?.Body is not null)
                {
                    return methodDeclaration.Body.ToString();
                }

                if (methodDeclaration?.ExpressionBody is not null)
                {
                    // ArrowExpressionClauseSyntax.ToString() already includes "=> ",
                    // so we return it directly without adding another "=> " prefix.
                    return methodDeclaration.ExpressionBody.ToString();
                }

                return "// No body found";
            }
            catch
            {
                return "// Error extracting method body";
            }
        }

        private static string BuildParameterDeclaration(IParameterSymbol parameter)
        {
            var parts = new List<string>();
            if (parameter.IsParams)
            {
                parts.Add("params");
            }
            else if (parameter.RefKind == RefKind.Ref)
            {
                parts.Add("ref");
            }
            else if (parameter.RefKind == RefKind.Out)
            {
                parts.Add("out");
            }
            else if (parameter.RefKind == RefKind.In)
            {
                parts.Add("in");
            }

            parts.Add(parameter.Type.ToDisplayString());
            parts.Add(parameter.Name);

            if (parameter.HasExplicitDefaultValue)
            {
                parts.Add("=");
                parts.Add(FormatDefaultValue(parameter.ExplicitDefaultValue));
            }

            return string.Join(" ", parts);
        }

        private static string FormatDefaultValue(object? value)
            => value switch
            {
                null => "null",
                string s => $"\"{s.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"",
                char c => $"'{c.ToString().Replace("'", "\\'")}'",
                bool b => b ? "true" : "false",
                _ => value.ToString() ?? "null"
            };

        private static ImmutableArray<ITypeParameterSymbol> GetWrapperTypeParameters(IMethodSymbol method)
        {
            if (method.TypeParameters.Length == 0)
            {
                return [];
            }

            var companionNames = new HashSet<string>(StringComparer.Ordinal);

            return [.. method.TypeParameters
                .Where(typeParameter => method.Parameters
                    .Skip(1)
                    .Any(parameter => UsesTypeParameter(parameter.Type, typeParameter.Name)))
                .Where(typeParameter => !companionNames.Contains(typeParameter.Name))];
        }

        private static bool UsesTypeParameter(ITypeSymbol type, string typeParameterName)
        {
            if (type is ITypeParameterSymbol typeParameter)
            {
                return string.Equals(typeParameter.Name, typeParameterName, StringComparison.Ordinal);
            }

            if (type is IArrayTypeSymbol arrayType)
            {
                return UsesTypeParameter(arrayType.ElementType, typeParameterName);
            }

            if (type is INamedTypeSymbol namedType)
            {
                return namedType.TypeArguments.Any(arg => UsesTypeParameter(arg, typeParameterName));
            }

            return false;
        }

        private static bool DoesNotDependOnMissingTypeParameter(string constraintText, HashSet<string> wrapperTypeParameterNames)
        {
            if (wrapperTypeParameterNames.Count == 0)
            {
                return false;
            }

            return wrapperTypeParameterNames.Any(token =>
                constraintText.Contains(token, StringComparison.Ordinal));
        }

        internal static string SubstituteTypeParams(
            string text,
            Dictionary<string, string> substitutions)
        {
            foreach (var kvp in substitutions)
            {
                text = SubstituteOneTypeParam(text, kvp.Key, kvp.Value);
            }

            return text;
        }

        internal static string RemoveTypeParameterFromDeclaration(string declaration, string typeParameterName)
        {
            if (string.IsNullOrWhiteSpace(declaration))
            {
                return declaration;
            }

            var inner = declaration.Trim();
            if (!inner.StartsWith("<", StringComparison.Ordinal) || !inner.EndsWith(">", StringComparison.Ordinal))
            {
                return declaration;
            }

            var parts = inner.Substring(1, inner.Length - 2)
                .Split(',')
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrWhiteSpace(p) && !string.Equals(p, typeParameterName, StringComparison.Ordinal))
                .ToArray();

            return parts.Length == 0 ? string.Empty : $"<{string.Join(", ", parts)}>";
        }

        internal static string RemoveTypeParameterConstraintClause(string constraints, string typeParameterName)
        {
            if (string.IsNullOrWhiteSpace(constraints))
            {
                return constraints;
            }

            var clauses = constraints
                .Split([" where "], StringSplitOptions.None)
                .Select((part, index) => index == 0 ? part.Trim() : $"where {part.Trim()}")
                .Where(part => !string.IsNullOrWhiteSpace(part))
                .Where(part => !part.StartsWith($"where {typeParameterName} :", StringComparison.Ordinal))
                .ToArray();

            return clauses.Length == 0 ? string.Empty : string.Join(" ", clauses);
        }

        private static string SubstituteOneTypeParam(
            string text,
            string typeParamName,
            string concreteTypeName)
        {
            // Normalize: concrete type without trailing '?' (to avoid double ?? when substituting T?)
            var concreteBase = concreteTypeName.TrimEnd('?');

            // Replace "TypeParamName?" first (nullable form) to avoid double ?
            text = ReplaceWordBoundary(text, typeParamName + "?", concreteBase + "?");

            // Replace remaining standalone "TypeParamName" (non-nullable form)
            text = ReplaceWordBoundary(text, typeParamName, concreteTypeName);

            return text;
        }

        private static string ReplaceWordBoundary(string text, string target, string replacement)
        {
            if (string.IsNullOrEmpty(target))
            {
                return text;
            }

            var sb = new StringBuilder(text.Length);
            var i = 0;
            while (i < text.Length)
            {
                if (i + target.Length <= text.Length &&
                    string.Compare(text, i, target, 0, target.Length, StringComparison.Ordinal) == 0)
                {
                    var validBefore = i == 0 || !IsIdentifierChar(text[i - 1]);
                    var afterIdx = i + target.Length;
                    var validAfter = afterIdx >= text.Length || !IsIdentifierChar(text[afterIdx]);

                    if (validBefore && validAfter)
                    {
                        sb.Append(replacement);
                        i += target.Length;
                        continue;
                    }
                }

                sb.Append(text[i]);
                i++;
            }

            return sb.ToString();
        }

        private static bool IsIdentifierChar(char c)
            => char.IsLetterOrDigit(c) || c == '_';
    }
}

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Catchy.SourceGenerator
{
    public sealed partial class CatchyIncrementalGenerator
    {
        private static AssertableModel? GetAssertableModel(
            GeneratorAttributeSyntaxContext ctx, System.Threading.CancellationToken ct)
        {
            if (ctx.TargetSymbol is not INamedTypeSymbol type) return null;
            ct.ThrowIfCancellationRequested();

            var attr = ctx.Attributes.FirstOrDefault();
            if (attr is null) return null;

            var compilation = ctx.SemanticModel.Compilation;
            int maxArity = 8;
            bool autoMapFields = true;
            bool ignoreExtraFields = true;
            bool registerCrossTypeRules = true;
            string defaultStringComparisonExpression = DefaultStringComparisonExpression;
            string? baseAssertionTypeName = null;
            var crossTypeWith = ImmutableArray<string>.Empty;
            var markerInterfaces = ImmutableArray<string>.Empty;

            foreach (var named in attr.NamedArguments)
            {
                switch (named.Key)
                {
                    case "MaxArity" when named.Value.Value is int a:
                        maxArity = a;
                        break;
                    case "AutoMapFields" when named.Value.Value is bool auto:
                        autoMapFields = auto;
                        break;
                    case "IgnoreExtraFields" when named.Value.Value is bool ignore:
                        ignoreExtraFields = ignore;
                        break;
                    case "RegisterCrossTypeRules" when named.Value.Value is bool register:
                        registerCrossTypeRules = register;
                        break;
                    case "StringComparison":
                        defaultStringComparisonExpression = ParseStringComparisonExpression(named.Value, defaultStringComparisonExpression);
                        break;
                    case "BaseAssertionType" when named.Value.Kind == TypedConstantKind.Type
                                                  && named.Value.Value is INamedTypeSymbol bat:
                        baseAssertionTypeName = bat.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                        break;
                    case "MarkerInterfaces" when named.Value.Kind == TypedConstantKind.Array:
                        markerInterfaces = [.. named.Value.Values
                            .Select(v => v.Value?.ToString())
                            .Where(s => !string.IsNullOrWhiteSpace(s))
                            .Select(s => ResolveCrossType(type, compilation, s!).FullTypeName)
                            .Select(EnsureGlobalQualification)
                            .Distinct(StringComparer.Ordinal)];
                        break;
                }
            }


            if (attr.ConstructorArguments.Length > 0
                && attr.ConstructorArguments[0].Kind == TypedConstantKind.Array)
            {
                crossTypeWith = [.. attr.ConstructorArguments[0].Values
                    .Select(v => v.Value?.ToString() ?? "")
                    .Where(s => s.Length > 0)];
            }

            var fields = ExtractAssertableFields(type, defaultStringComparisonExpression);
            var crossTypes = ExtractCrossTypes(
                type,
                compilation,
                fields,
                crossTypeWith,
                autoMapFields,
                ignoreExtraFields,
                registerCrossTypeRules,
                defaultStringComparisonExpression);

            var ns = type.ContainingNamespace?.IsGlobalNamespace == false
                ? type.ContainingNamespace.ToDisplayString()
                : "";

            return new AssertableModel(
                ns,
                type.Name,
                type.ToDisplayString(),
                maxArity,
                autoMapFields,
                ignoreExtraFields,
                registerCrossTypeRules,
                defaultStringComparisonExpression,
                baseAssertionTypeName,
                markerInterfaces,
                crossTypes,
                fields);
        }

        private static ImmutableArray<AssertableFieldModel> ExtractAssertableFields(
            INamedTypeSymbol type,
            string defaultStringComparisonExpression)
        {
            var fieldsBuilder = ImmutableArray.CreateBuilder<AssertableFieldModel>();

            foreach (var member in type.GetMembers())
            {
                if (member is not IPropertySymbol and not IFieldSymbol)
                {
                    continue;
                }

                var assertMemberAttr = member.GetAttributes()
                    .FirstOrDefault(a => a.AttributeClass?.Name == "AssertMemberAttribute");

                if (assertMemberAttr is null)
                {
                    continue;
                }

                string? mapTo = null;
                bool skip = false;
                string? messageFormat = null;
                bool useStringComparison = false;
                int order = 0;
                var stringComparisonExpression = defaultStringComparisonExpression;
                string? transitionTypeName = null;

                foreach (var named in assertMemberAttr.NamedArguments)
                {
                    switch (named.Key)
                    {
                        case "MapTo" when named.Value.Value is string s:
                            mapTo = s;
                            break;
                        case "Skip" when named.Value.Value is bool b:
                            skip = b;
                            break;
                        case "MessageFormat" when named.Value.Value is string m:
                            messageFormat = m;
                            break;
                        case "UseStringComparison" when named.Value.Value is bool useCmp:
                            useStringComparison = useCmp;
                            break;
                        case "StringComparison":
                            stringComparisonExpression = ParseStringComparisonExpression(named.Value, defaultStringComparisonExpression);
                            break;
                        case "Order" when named.Value.Value is int fieldOrder:
                            order = fieldOrder;
                            break;
                        case "TransitionType" when named.Value.Value is INamedTypeSymbol transitionTypeSymbol:
                            transitionTypeName = transitionTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                            break;
                    }
                }

                var propName = member.Name;
                var propTypeSymbol = member is IPropertySymbol p
                    ? p.Type
                    : ((IFieldSymbol)member).Type;
                var propType = propTypeSymbol.ToDisplayString();
                var isAssertableType = propTypeSymbol.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == AssertableAttributeFqn);

                fieldsBuilder.Add(new AssertableFieldModel(
                    propName,
                    propType,
                    isAssertableType,
                    mapTo,
                    skip,
                    messageFormat,
                    useStringComparison,
                    stringComparisonExpression,
                    order,
                    transitionTypeName));
            }

            return [.. fieldsBuilder
                .OrderBy(f => f.Order)
                .ThenBy(f => f.PropertyName, StringComparer.Ordinal)];
        }

        private static ImmutableArray<CrossTypeModel> ExtractCrossTypes(
            INamedTypeSymbol type,
            Compilation compilation,
            ImmutableArray<AssertableFieldModel> sourceFields,
            ImmutableArray<string> crossTypeWith,
            bool autoMapFields,
            bool ignoreExtraFields,
            bool registerCrossTypeRules,
            string defaultStringComparisonExpression)
        {
            var builder = ImmutableArray.CreateBuilder<CrossTypeModel>();

            foreach (var targetName in crossTypeWith)
            {
                var (Symbol, FullTypeName, IsResolved) = ResolveCrossType(type, compilation, targetName);
                var maps = BuildCrossTypeFieldMaps(sourceFields, Symbol, autoMapFields);
                builder.Add(new CrossTypeModel(
                    targetName,
                    FullTypeName,
                    IsResolved,
                    autoMapFields,
                    ignoreExtraFields,
                    registerCrossTypeRules,
                    defaultStringComparisonExpression,
                    0,
                    maps));
            }

            foreach (var attr in type.GetAttributes().Where(a => a.AttributeClass?.Name == "CrossTypeAttribute"))
            {
                var targetName = attr.ConstructorArguments.FirstOrDefault().Value?.ToString();
                if (string.IsNullOrWhiteSpace(targetName))
                {
                    continue;
                }

                var nonNullTargetName = targetName!;
                var (Symbol, FullTypeName, IsResolved) = ResolveCrossType(type, compilation, nonNullTargetName);
                var model = new CrossTypeModel(
                    nonNullTargetName,
                    FullTypeName,
                    IsResolved,
                    autoMapFields,
                    ignoreExtraFields,
                    registerCrossTypeRules,
                    defaultStringComparisonExpression,
                    0,
                    BuildCrossTypeFieldMaps(sourceFields, Symbol, autoMapFields));

                foreach (var named in attr.NamedArguments)
                {
                    model = named.Key switch
                    {
                        "AutoMapFields" when named.Value.Value is bool auto
                            => model with
                            {
                                AutoMapFields = auto,
                                FieldMaps = BuildCrossTypeFieldMaps(sourceFields, Symbol, auto)
                            },
                        "IgnoreExtraFields" when named.Value.Value is bool ignore
                            => model with { IgnoreExtraFields = ignore },
                        "RegisterInGlobalRegistry" when named.Value.Value is bool register
                            => model with { RegisterInGlobalRegistry = register },
                        "StringComparison"
                            => model with { StringComparisonExpression = ParseStringComparisonExpression(named.Value, defaultStringComparisonExpression) },
                        "Order" when named.Value.Value is int order
                            => model with { Order = order },
                        _ => model
                    };
                }

                builder.Add(model);
            }

            return [.. builder
                .GroupBy(x => (x.TargetTypeFullName, x.Order))
                .Select(g => g.First())
                .OrderBy(x => x.Order)
                .ThenBy(x => x.TargetTypeFullName, StringComparer.Ordinal)];
        }

        private static (INamedTypeSymbol? Symbol, string FullTypeName, bool IsResolved) ResolveCrossType(
            INamedTypeSymbol sourceType,
            Compilation compilation,
            string targetTypeName)
        {
            if (string.IsNullOrWhiteSpace(targetTypeName))
            {
                return (null, targetTypeName, false);
            }

            INamedTypeSymbol? symbol;

            if (targetTypeName.IndexOf(".", StringComparison.Ordinal) >= 0)
            {
                symbol = compilation.GetTypeByMetadataName(targetTypeName);
            }
            else
            {
                var inSameNamespace = sourceType.ContainingNamespace?.IsGlobalNamespace == false
                    ? sourceType.ContainingNamespace.ToDisplayString() + "." + targetTypeName
                    : targetTypeName;

                symbol = compilation.GetTypeByMetadataName(inSameNamespace)
                    ?? compilation.GetTypeByMetadataName(targetTypeName)
                    ?? FindTypeBySimpleName(compilation.Assembly.GlobalNamespace, targetTypeName);
            }

            if (symbol is null)
            {
                return (null, targetTypeName, false);
            }

            return (symbol, symbol.ToDisplayString(), true);
        }

        private static ImmutableArray<CrossTypeFieldMapModel> BuildCrossTypeFieldMaps(
            ImmutableArray<AssertableFieldModel> sourceFields,
            INamedTypeSymbol? targetType,
            bool autoMapFields)
        {
            if (targetType is null)
            {
                return [];
            }

            var targetMembers = new Dictionary<string, string>(StringComparer.Ordinal);

            foreach (var member in targetType.GetMembers())
            {
                switch (member)
                {
                    case IPropertySymbol p:
                        targetMembers[p.Name] = p.Type.ToDisplayString();
                        break;
                    case IFieldSymbol f:
                        targetMembers[f.Name] = f.Type.ToDisplayString();
                        break;
                }
            }

            var builder = ImmutableArray.CreateBuilder<CrossTypeFieldMapModel>();

            foreach (var field in sourceFields.Where(f => !f.Skip))
            {
                var targetName = field.MapTo;

                if (string.IsNullOrWhiteSpace(targetName))
                {
                    if (!autoMapFields)
                    {
                        continue;
                    }

                    targetName = field.PropertyName;
                }

                var nonNullTargetName = targetName!;
                if (!targetMembers.TryGetValue(nonNullTargetName, out var targetTypeName))
                {
                    continue;
                }

                builder.Add(new CrossTypeFieldMapModel(
                    field.PropertyName,
                    field.PropertyType,
                    nonNullTargetName,
                    targetTypeName,
                    field.UseStringComparison,
                    field.StringComparisonExpression,
                    field.MessageFormat,
                    field.Order));
            }

            return [.. builder
                .OrderBy(m => m.Order)
                .ThenBy(m => m.SourceMemberName, StringComparer.Ordinal)];
        }

        private static INamedTypeSymbol? FindTypeBySimpleName(INamespaceSymbol ns, string simpleName)
        {
            foreach (var typeMember in ns.GetTypeMembers())
            {
                if (typeMember.Name == simpleName)
                {
                    return typeMember;
                }
            }

            foreach (var nestedNs in ns.GetNamespaceMembers())
            {
                var found = FindTypeBySimpleName(nestedNs, simpleName);
                if (found is not null)
                {
                    return found;
                }
            }

            return null;
        }

        private static string ParseStringComparisonExpression(TypedConstant constant, string fallback)
        {
            if (constant.IsNull)
            {
                return fallback;
            }

            if (constant.Value is int enumValue)
            {
                return $"global::System.StringComparison.{(StringComparison)enumValue}";
            }

            var text = constant.Value?.ToString();
            if (string.IsNullOrWhiteSpace(text))
            {
                return fallback;
            }

            var nonNullText = text!;
            return nonNullText.IndexOf(".", StringComparison.Ordinal) >= 0
                ? $"global::System.{nonNullText}"
                : $"global::System.StringComparison.{nonNullText}";
        }
    }
}

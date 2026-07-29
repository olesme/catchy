using System.Collections.Immutable;

namespace Catchy.SourceGenerator
{
    public sealed partial class CatchyIncrementalGenerator
    {
        private sealed record AssertableFieldModel(
            string PropertyName,
            string PropertyType,
            bool IsAssertableType,
            string? MapTo,
            bool Skip,
            string? MessageFormat,
            bool UseStringComparison,
            string StringComparisonExpression,
            int Order,
            string? TransitionTypeName);

        private sealed record CrossTypeFieldMapModel(
            string SourceMemberName,
            string SourceMemberType,
            string TargetMemberName,
            string TargetMemberType,
            bool UseStringComparison,
            string StringComparisonExpression,
            string? MessageFormat,
            int Order);

        private sealed record CrossTypeModel(
            string TargetTypeName,
            string TargetTypeFullName,
            bool IsResolved,
            bool AutoMapFields,
            bool IgnoreExtraFields,
            bool RegisterInGlobalRegistry,
            string StringComparisonExpression,
            int Order,
            ImmutableArray<CrossTypeFieldMapModel> FieldMaps);

        private sealed record AssertableModel(
            string Namespace,
            string TypeName,
            string FullTypeName,
            int MaxArity,
            bool AutoMapFields,
            bool IgnoreExtraFields,
            bool RegisterCrossTypeRules,
            string StringComparisonDefault,
            string? BaseAssertionTypeName,
            ImmutableArray<string> MarkerInterfaces,
            ImmutableArray<CrossTypeModel> CrossTypes,
            ImmutableArray<AssertableFieldModel> Fields);

        private sealed record CrossTypeMemberMapModel(
            string SourceMemberName,
            string TargetMemberName,
            bool UseStringComparison,
            string StringComparisonExpression,
            string? MessageFormat,
            int Order);

        private sealed record CrossTypeRuleModel(
            string Namespace,
            string ContainerClassName,
            string SourceTypeFullName,
            string TargetTypeFullName,
            bool AutoMapFields,
            bool IgnoreExtraFields,
            bool RegisterInGlobalRegistry,
            string StringComparisonExpression,
            int Order,
            ImmutableArray<CrossTypeMemberMapModel> MemberMaps);

        private sealed record RulePairModel(
            string SourceTypeFullName,
            string TargetTypeFullName);

        /// <summary>
        /// Model for <c>[AssertEntry]</c> / <c>[assembly: AssertEntry(typeof(T))]</c>.
        /// Generates: <c>Asserter.That&lt;T&gt;(T value) where T : BaseTypeName</c>.
        /// </summary>
        private sealed record AssertEntryModel(
            /// <summary>Namespace for the generated extensions class.</summary>
            string Namespace,
            /// <summary>Fully-qualified base type name used in the generic constraint.</summary>
            string BaseTypeFullName,
            /// <summary>Simple name of the base type, used for naming the generated class.</summary>
            string BaseTypeName);

        /// <summary>
        /// Model for <c>[AssertVia("Prop")]</c> / <c>[assembly: AssertVia(typeof(T), "Prop")]</c>.
        /// Generates: <c>Asserter.That(OwnerType value)</c> that internally extracts
        /// <c>value.PropertyName</c> and returns <c>ValueAssertions&lt;PropertyType&gt;</c>.
        /// </summary>
        private sealed record AssertViaModel(
            /// <summary>Namespace for the generated extensions class.</summary>
            string Namespace,
            /// <summary>Fully-qualified owner type name (type that has the property).</summary>
            string OwnerTypeFullName,
            /// <summary>Simple owner type name, used for naming the generated class.</summary>
            string OwnerTypeName,
            /// <summary>Name of the property to extract.</summary>
            string PropertyName,
            /// <summary>Fully-qualified type returned by the property.</summary>
            string PropertyTypeFullName);
    }
}


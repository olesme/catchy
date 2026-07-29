namespace Catchy
{
    /// <summary>
    /// Marks a field or property for automatic assertion generation.
    /// When applied to a field/property on a class marked with [Assertable],
    /// the source generator will create assertion methods for that field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false)]
    public sealed class AssertMemberAttribute : Attribute
    {
        /// <summary>
        /// Optional target member name/path mapping for cross-type generated rules.
        /// Supports simple nested-path notation like "Address.City".
        /// </summary>
        public string? MapTo { get; set; }

        /// <summary>
        /// If true, this member is skipped in generated cross-type and field-based comparisons.
        /// </summary>
        public bool Skip { get; set; }

        /// <summary>
        /// Custom message format for assertion failure.
        /// Use {0} for the actual value, {1} for the expected value.
        /// </summary>
        public string? MessageFormat { get; set; }

        /// <summary>
        /// Optional string comparison mode for this member.
        /// Applied only for string-to-string generated comparisons.
        /// </summary>
        public StringComparison StringComparison { get; set; } = StringComparison.Ordinal;

        /// <summary>
        /// If true and both compared members are strings, use the configured StringComparison.
        /// </summary>
        public bool UseStringComparison { get; set; }

        /// <summary>
        /// Optional comparison priority for generated matching pipelines.
        /// Lower values are evaluated first.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Overrides the return type of the generated transition extension method for this member.
        /// When set, the transition will return this type instead of the default <c>ValueAssertions&lt;T&gt;</c>.
        /// The type must be constructible as <c>new TTransition(AssertionPipeline, TValue)</c>.
        /// Example: <c>[AssertMember(TransitionType = typeof(MyCustomAssertions))]</c>
        /// </summary>
        public Type? TransitionType { get; set; }

        public AssertMemberAttribute()
        {
        }
    }

    /// <summary>
    /// Marks a class as a source for cross-type assertions.
    /// Types marked with this attribute can be compared with other types using generated assertions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class CrossTypeAttribute(string targetTypeName) : Attribute
    {
        /// <summary>
        /// The fully qualified name or simple name of the type this can be compared with.
        /// </summary>
        public string TargetTypeName { get; } = targetTypeName;

        /// <summary>
        /// If true, members are auto-matched by name and compatible type.
        /// If false, explicit mappings are required.
        /// </summary>
        public bool AutoMapFields { get; set; } = true;

        /// <summary>
        /// If true, extra fields in the target type are ignored.
        /// </summary>
        public bool IgnoreExtraFields { get; set; } = true;

        /// <summary>
        /// Default string comparison mode for this cross-type rule.
        /// </summary>
        public StringComparison StringComparison { get; set; } = StringComparison.Ordinal;

        /// <summary>
        /// Whether generated registration should occur for this rule.
        /// </summary>
        public bool RegisterInGlobalRegistry { get; set; } = true;

        /// <summary>
        /// Optional ordering/group precedence for generated rule registrars.
        /// </summary>
        public int Order { get; set; }
    }
}

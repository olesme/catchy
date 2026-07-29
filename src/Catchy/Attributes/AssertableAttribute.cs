namespace Catchy
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AssertableAttribute : Attribute
    {
        public string[] CrossTypeWith { get; }
        public int MaxArity { get; set; } = 8;

        /// <summary>
        /// Overrides the base type of the generated assertion wrapper class.
        /// Must be an open generic type with a single type parameter, e.g. <c>typeof(StructuralAssertions&lt;&gt;)</c>,
        /// or a concrete non-generic type whose constructor accepts <c>(TValue, AssertionPipeline)</c> or
        /// <c>(AssertionPipeline, TValue)</c>.
        /// When <see langword="null"/> (default), the generated class inherits from
        /// <c>ValueAssertions&lt;TValue&gt;</c>.
        /// </summary>
        public Type? BaseAssertionType { get; set; }

        /// <summary>
        /// Global default for generated cross-type rules declared from this source type.
        /// </summary>
        public bool AutoMapFields { get; set; } = true;

        /// <summary>
        /// When true, members in the target type with no source match are ignored.
        /// </summary>
        public bool IgnoreExtraFields { get; set; } = true;

        /// <summary>
        /// Default string comparison mode for generated cross-type member comparisons.
        /// </summary>
        public StringComparison StringComparison { get; set; } = StringComparison.Ordinal;

        /// <summary>
        /// Enables generation of module-initializer/lazy registration glue for generated cross-type rules.
        /// </summary>
        public bool RegisterCrossTypeRules { get; set; } = true;

        /// <summary>
        /// Additional interfaces/markers implemented by generated assertion type for this [Assertable] class.
        /// Use fully-qualified type names for best generator resolution.
        /// </summary>
        public string[] MarkerInterfaces { get; set; } = [];


        public AssertableAttribute(params string[] crossTypeWith)
        {
            CrossTypeWith = crossTypeWith;
        }

        public AssertableAttribute()
        {
            CrossTypeWith = [];
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class CrossTypeRuleAttribute : Attribute
    {
        public Type SourceType { get; }
        public Type TargetType { get; }

        public bool AutoMapFields { get; set; } = false;
        public bool IgnoreExtraFields { get; set; } = true;
        public StringComparison StringComparison { get; set; } = StringComparison.Ordinal;
        public bool RegisterInGlobalRegistry { get; set; } = true;
        public int Order { get; set; }

        public CrossTypeRuleAttribute(Type sourceType, Type targetType)
        {
            SourceType = sourceType;
            TargetType = targetType;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class CrossTypeMemberMapAttribute : Attribute
    {
        public string SourceMemberName { get; }
        public string TargetMemberName { get; }

        public bool UseStringComparison { get; set; }
        public StringComparison StringComparison { get; set; } = StringComparison.Ordinal;
        public string? MessageFormat { get; set; }
        public int RuleOrder { get; set; }
        public int Order { get; set; }

        public CrossTypeMemberMapAttribute(string sourceMemberName, string targetMemberName)
        {
            SourceMemberName = sourceMemberName;
            TargetMemberName = targetMemberName;
        }
    }

    public enum AssertForGenerationMode
    {
        EntryPoint = 0,
        ExtensionsOnly = 1
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class AssertForAttribute : Attribute
    {
        public Type[] TargetTypes { get; }
        public AssertForGenerationMode Mode { get; set; } = AssertForGenerationMode.EntryPoint;
        public bool GenerateTransitions { get; set; } = true;
        public bool LazyTransitions { get; set; } = true;

        public AssertForAttribute(params Type[] targetTypes)
        {
            TargetTypes = targetTypes ?? [];
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class AssertForAttribute<T> : AssertForAttribute
    {
        public AssertForAttribute()
            : base(typeof(T))
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class AssertionAttribute(string expectationMessage) : Attribute
    {
        public string ExpectationMessage { get; } = expectationMessage;
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false)]
    public sealed class MatchToAttribute(string value) : Attribute
    {
        public string Value { get; } = value;
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false)]
    public sealed class DoNotMatchAttribute : Attribute { }
}

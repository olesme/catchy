namespace Catchy.Sdk
{
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
    public sealed class GenerateQuantifiedEntryPointsAttribute(params string[] typeNames)
        : System.Attribute
    {
        public string[] TypeNames { get; } = typeNames;
        public int MaxArity { get; set; } = 8;

        /// <summary>
        /// Optional #if guard emitted around the generated class and entry points.
        /// Example: "NET5_0_OR_GREATER", "NET6_0_OR_GREATER", "NET7_0_OR_GREATER"
        /// </summary>
        public string? VersionGuard { get; set; }
    }
}

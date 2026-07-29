namespace Catchy
{
    /// <summary>
    /// Instructs the source generator to emit an
    /// <c>Asserter.That(<em>TargetType</em> value)</c> entry point that extracts
    /// the named property and returns <c>ValueAssertions&lt;<em>PropertyType</em>&gt;</c>.
    ///
    /// <para>
    /// Use this when the underlying assertion target (e.g. <c>ILocator</c>, <c>IPage</c>)
    /// is accessible via an internal property on the abstraction class and must not
    /// leak into the test layer. The generated entry point handles the extraction;
    /// all existing assertion extension methods for the property type are immediately
    /// available in IntelliSense.
    /// </para>
    ///
    /// <para><b>On the class itself:</b></para>
    /// <code>
    /// [AssertVia("Locator")]
    /// public abstract class UiElement
    /// {
    ///     internal ILocator Locator { get; }
    /// }
    /// // Generates: Asserter.That(UiElement value) → ValueAssertions&lt;ILocator&gt;
    /// </code>
    ///
    /// <para><b>For types you cannot modify (assembly-level):</b></para>
    /// <code>
    /// [assembly: AssertVia(typeof(ThirdPartyBase), "InternalProp")]
    /// </code>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly,
        AllowMultiple = true, Inherited = false)]
    public sealed class AssertViaAttribute : Attribute
    {
        /// <summary>
        /// Applied directly on the class — target type is inferred from the decorated class.
        /// </summary>
        /// <param name="propertyName">
        /// Name of the property whose value becomes the assertion target.
        /// The property may be <see langword="internal"/>.
        /// </param>
        public AssertViaAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        /// <summary>
        /// Applied at assembly level — explicit target type is required.
        /// </summary>
        /// <param name="targetType">The class that owns the property.</param>
        /// <param name="propertyName">
        /// Name of the property whose value becomes the assertion target.
        /// </param>
        public AssertViaAttribute(Type targetType, string propertyName)
        {
            TargetType = targetType;
            PropertyName = propertyName;
        }

        /// <summary>
        /// The target type that owns <see cref="PropertyName"/>.
        /// <see langword="null"/> when the attribute is placed directly on the class.
        /// </summary>
        public Type? TargetType { get; }

        /// <summary>Name of the property to extract as the assertion target.</summary>
        public string PropertyName { get; } = null!;
    }
}

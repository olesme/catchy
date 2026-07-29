namespace Catchy
{
    /// <summary>
    /// Instructs the source generator to emit a generic
    /// <c>Asserter.That&lt;T&gt;(T value) where T : <em>TargetType</em></c> entry point
    /// that returns <c>ValueAssertions&lt;T&gt;</c>.
    ///
    /// <para>
    /// Because the concrete subtype is preserved as <c>T</c>, extension methods with
    /// interface constraints (e.g. <c>where T : UiElement, IClickable</c>) will appear
    /// in IntelliSense only for element types that satisfy those constraints.
    /// </para>
    ///
    /// <para><b>On the class itself:</b></para>
    /// <code>
    /// [AssertEntry]
    /// public abstract class UiElement { }
    /// // Generates: Asserter.That&lt;T&gt;(T value) where T : UiElement → ValueAssertions&lt;T&gt;
    /// </code>
    ///
    /// <para><b>For types you cannot modify (assembly-level):</b></para>
    /// <code>
    /// [assembly: AssertEntry(typeof(ThirdPartyBase))]
    /// // Generates: Asserter.That&lt;T&gt;(T value) where T : ThirdPartyBase → ValueAssertions&lt;T&gt;
    /// </code>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly,
        AllowMultiple = true, Inherited = false)]
    public sealed class AssertEntryAttribute : Attribute
    {
        /// <summary>
        /// Applied directly on the class — target type is inferred from the decorated class.
        /// </summary>
        public AssertEntryAttribute() { }

        /// <summary>
        /// Applied at assembly level — explicit target type is required.
        /// </summary>
        /// <param name="targetType">
        /// The base type for which to generate
        /// <c>Asserter.That&lt;T&gt;(T) where T : <paramref name="targetType"/></c>.
        /// </param>
        public AssertEntryAttribute(Type targetType)
        {
            TargetType = targetType;
        }

        /// <summary>
        /// The target base type. <see langword="null"/> when the attribute is placed
        /// directly on the class (type is inferred by the generator).
        /// </summary>
        public Type? TargetType { get; }
    }
}

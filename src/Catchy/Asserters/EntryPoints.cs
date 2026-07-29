namespace Catchy
{
    /// <summary>
    /// Hard-assert entry point backed by the shared stateless asserter.
    /// </summary>
    /// <remarks>Use <c>using static Catchy.Stateless;</c>.</remarks>
    public static class Stateless
    {
        /// <summary>
        /// Throws on the first failed assertion.
        /// </summary>
        public static StatelessAsserter Assert => Asserter.DefaultStateless;
    }

    /// <summary>
    /// Hard-assert entry point with <c>Check</c> naming.
    /// </summary>
    /// <remarks>Useful when <c>Assert</c> would conflict with another framework.</remarks>
    public static class StatelessAlias
    {
        /// <summary>
        /// Throws on the first failed assertion.
        /// </summary>
        public static StatelessAsserter Check => Asserter.DefaultStateless;
    }

    /// <summary>
    /// Ambient entry point for shared hard and soft assertions.
    /// </summary>
    /// <remarks>Use <c>Assert.That(...)</c> for hard assertions and <c>Assert.Soft.That(...)</c> for soft assertions.</remarks>
    public static class Ambient
    {
        /// <summary>
        /// Gets the current ambient stateful asserter.
        /// </summary>
        public static StatefulAsserter Assert => AmbientAsserterSource.Current;
    }

    /// <summary>
    /// Ambient entry point with <c>Check</c> naming.
    /// </summary>
    /// <remarks>Use when <c>Assert</c> would conflict with another framework.</remarks>
    public static class AmbientAlias
    {
        /// <summary>
        /// Gets the current ambient stateful asserter.
        /// </summary>
        public static StatefulAsserter Check => AmbientAsserterSource.Current;
    }

    /// <summary>
    /// Ambient entry point for soft assertions only.
    /// </summary>
    public static class AmbientSoft
    {
        /// <summary>
        /// Collects failures in the current ambient soft state.
        /// </summary>
        public static SoftAsserter Verify => AmbientAsserterSource.Current.Soft;
    }
}

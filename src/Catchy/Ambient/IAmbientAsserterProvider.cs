namespace Catchy
{
    /// <summary>
    /// Framework-specific provider for test-scoped ambient asserter storage.
    /// Manages single <see cref="StatefulAsserter"/> instance per test scope.
    /// Soft assertions are accessed via the asserter's <c>.Soft</c> property.
    /// </summary>
    public interface IAmbientAsserterProvider
    {
        /// <summary>
        /// Get or create the stateful asserter for current test scope.
        /// </summary>
        StatefulAsserter GetOrCreateStateful();

        /// <summary>
        /// Try get the stateful asserter if already created for current scope.
        /// </summary>
        StatefulAsserter? TryGetStateful();

        /// <summary>
        /// Clear the asserter for current scope (called on test cleanup).
        /// </summary>
        void ClearStateful();
    }
}

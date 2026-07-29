namespace Catchy
{
    /// <summary>
    /// Throws <see cref="AssertionException"/> immediately on the first failure.
    /// This asserter is stateless—it does not accumulate soft failures internally.
    /// 
    /// Access via:
    /// - <c>Stateless.Assert</c> — static accessor for the global singleton
    /// - <c>Asserter.DefaultStateless</c> — global singleton instance
    /// 
    /// <strong>Thread-Safety & Singleton Usage:</strong>
    /// 
    /// The shared singleton instance (<see cref="Asserter.DefaultStateless"/>) is thread-safe for READ-ONLY operations:
    /// - ✓ Safe: Multiple threads reading configuration and executing read-only assertions
    /// - ✓ Safe: Chaining multiple assertions in a single chain (all read-only)
    /// - ✓ Safe: Passing through read-only verification chains
    /// 
    /// NOT safe for mutations across threads:
    /// - ✗ Unsafe: Modifying AssertionSettings on the singleton from multiple threads
    /// - ✗ Unsafe: Parallel execution of different assertion chains on the singleton
    /// - ✗ Unsafe: Creating state via .With(SoftState) on the singleton from multiple threads
    /// 
    /// <strong>Usage Patterns:</strong>
    /// 
    /// Safe (read-only, single thread):
    /// <code>
    /// // Simple linear verification in same thread
    /// await Stateless.Assert.That(x).Is(1);
    /// await Stateless.Assert.That(y).Is(2);
    /// </code>
    /// 
    /// Unsafe (parallel mutations):
    /// <code>
    /// // WRONG - Parallel execution with different soft states
    /// Parallel.Run(() => Stateless.Assert.That(x).Is(1).With(state1));
    /// Parallel.Run(() => Stateless.Assert.That(y).Is(2).With(state2));
    /// // Better: Use StatefulAsserter instead
    /// </code>
    /// 
    /// <strong>For Stateful Assertions Across Multiple Threads, Use <see cref="StatefulAsserter"/> Instead:</strong>
    /// <code>
    /// var assert = Asserter.NewStateful();
    /// // Each thread gets its own clear context
    /// Parallel.Run(() => assert.That(x).Is(1));  // hard mode
    /// Parallel.Run(() => assert.Soft.That(y).Is(2));  // soft mode, shared state
    /// </code>
    /// </summary>
    public partial class StatelessAsserter : Asserter
    {
        internal StatelessAsserter(AssertionSettings? settings = null) : base(settings) { }
    }
}

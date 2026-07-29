namespace Catchy
{
    /// <summary>
    /// A stateful hard asserter that can have test dependent hooks and can access soft assertions through its Soft property.
    /// 
    /// Use <c>That()</c> for hard assertions (throws immediately on failure).
    /// Use <c>Soft.That()</c> for soft assertions (captures failures in shared state).
    /// 
    /// The <see cref="Soft"/> property returns a separate <see cref="SoftAsserter"/> that shares
    /// the same <see cref="AssertionSettings"/> but has its own independent <see cref="SoftState"/>.
    /// 
    /// Example:
    /// <code>
    /// var assert = Asserter.NewStateful();
    /// 
    /// // Hard assertions - throws on first failure
    /// try { await assert.That(x).Is(1); }
    /// catch (AssertionException) { /* handle */ }
    /// 
    /// // Soft assertions - accumulate failures
    /// await assert.Soft.That(a).Is(2);
    /// await assert.Soft.That(b).Is(3);
    /// 
    /// // Access accumulated failures
    /// if (assert.Soft.HasFailures)
    /// {
    ///     foreach (var error in assert.Soft.Errors)
    ///         Console.WriteLine(error);
    /// }
    /// </code>
    /// 
    /// Thread-safe: The <see cref="Soft"/> property lazily creates and caches a single <see cref="SoftAsserter"/> instance.
    /// </summary>
    public sealed class StatefulAsserter : Asserter
    {
        private SoftAsserter? _softAsserter;

        public StatefulAsserter(AssertionSettings? settings = null) : base(settings) { }
        public StatefulAsserter(Action<AssertionSettings> configure) : base(configure) { }

        /// <summary>
        /// Returns a soft asserter with its own independent SoftState.
        /// Lazily created and cached on first access.
        /// Subsequent calls return the same SoftAsserter instance.
        /// </summary>
        public SoftAsserter Soft
        {
            get
            {
                _softAsserter ??= new SoftAsserter(_settings);
                return _softAsserter;
            }
        }
    }
}

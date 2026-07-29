namespace Catchy
{
    /// <summary>
    /// Accumulates failures instead of throwing immediately.
    /// Flush via <c>await Stateless.Verify.That(softAssert).HasNoErrors()</c>.
    /// </summary>
    public sealed partial class SoftAsserter : Asserter
    {
        public SoftState SoftState { get { _softState ??= new(); return _softState; } }
        public bool HasFailures => SoftState.HasFailures;
        public int ErrorCount => SoftState.ErrorCount;
        public IReadOnlyList<Exception> Errors => SoftState.Errors;

        public SoftAsserter() : this(AssertionSettings.Global, new()) { }
        public SoftAsserter(SoftState softState) : this(AssertionSettings.Global, softState) { }
        public SoftAsserter(AssertionSettings settings) : this(settings, new()) { }
        public SoftAsserter(Action<AssertionSettings> configure) : this(Configured(configure), new()) { }
        internal SoftAsserter(AssertionSettings settings, SoftState softState)
            : base(settings, softState) { _softState = softState; }

        public void Clear() => SoftState.Clear();
        public int Checkpoint() => SoftState.Checkpoint();
        public void Revert(int checkpoint) => SoftState.Revert(checkpoint);
        public void Revert() => SoftState.Revert();

        private static AssertionSettings Configured(Action<AssertionSettings>? configure)
        {
            var s = AssertionSettings.Global.Clone();
            configure?.Invoke(s);
            return s;
        }
    }
}

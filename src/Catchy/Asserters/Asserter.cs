namespace Catchy
{
    namespace Configuration
    {
        public static partial class AsserterExtensions
        {
            public static AssertionSettings Settings(this Asserter a)
            {
                return a._settings;
            }
        }
    }

    public abstract partial class Asserter
    {
        internal readonly AssertionSettings _settings;
        internal readonly bool _isNoOp;
        internal SoftState? _softState = null;
        internal bool _isSoftMode = false;

        protected Asserter(AssertionSettings? settings = null)
            => _settings = settings ?? AssertionSettings.Global;

        protected Asserter(Action<AssertionSettings> configure)
        {
            var s = AssertionSettings.Global.Clone();
            configure(s);
            _settings = s;
        }

        protected Asserter(AssertionSettings settings, SoftState? softState = null, bool isNoOp = false)
        {
            _settings = settings;
            _softState = softState;
            _isNoOp = isNoOp;
        }

        /// <summary>
        /// Global singleton stateless hard asserter.
        /// Thread-safe for read-only verification chains.
        /// See <see cref="StatefulAsserter"/> documentation for thread-safety guidelines.
        /// </summary>
        public static StatelessAsserter DefaultStateless { get; } = new();
        public static StatefulAsserter NewStateful(AssertionSettings? settings = null) => new(settings);
        public static StatefulAsserter NewStateful(Action<AssertionSettings> configure) => new(configure);

        public static SoftAsserter NewSoft() => new(AssertionSettings.Global);
        public static SoftAsserter NewSoft(AssertionSettings settings) => new(settings);
        public static SoftAsserter NewSoft(Action<AssertionSettings> configure) => new(configure);
        public static SoftAsserter NewSoft(IEnumerable<Type> types)
            => Asserter.NewSoft(s => s.CaughtExceptionTypes = [.. types]);
        public static SoftAsserter NewSoft<TException>() where TException : Exception
            => Asserter.NewSoft(s => s.CaughtExceptionTypes = [typeof(TException)]);
        public static SoftAsserter NewSoft(bool allExceptions)
            => Asserter.NewSoft(s => s.CatchAll = allExceptions);
    }
}

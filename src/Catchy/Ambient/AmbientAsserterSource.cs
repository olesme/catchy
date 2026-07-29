namespace Catchy
{
    /// <summary>
    /// Ambient source for test-scoped stateful asserters.
    /// Provides single <see cref="StatefulAsserter"/> instance per test scope.
    /// Soft assertions accessed via <see cref="StatefulAsserter.Soft"/> property.
    /// </summary>
    public static class AmbientAsserterSource
    {
        /// <summary>
        /// Stateful ambient asserter source for current test scope.
        /// Manages the single asserter instance shared between hard and soft assertions.
        /// </summary>
        public static AmbientStatefulSource Stateful { get; } = new();

        /// <summary>
        /// Current ambient stateful asserter instance for the test scope.
        /// Shorthand for <see cref="Stateful"/>.Current.
        /// </summary>
        public static StatefulAsserter Current => Stateful.Current;

        /// <summary>
        /// Clears the current ambient asserter instance.
        /// Shorthand for <see cref="Stateful"/>.Clear().
        /// </summary>
        public static void Clear() => Stateful.Clear();

        /// <summary>
        /// Tries to get the current ambient asserter instance without creating one.
        /// Shorthand for <see cref="Stateful"/>.TryGetCurrent().
        /// </summary>
        public static StatefulAsserter? TryGetCurrent() => Stateful.TryGetCurrent();

        /// <summary>
        /// Generic base for ambient asserter sources with lazy discovery, provider registration, and factory pattern.
        /// </summary>
        public abstract class AmbientSource<T> where T : class
        {
            protected IAmbientAsserterProvider? _provider;
            protected bool _discovered;
            protected readonly object _lock = new();

            public Func<T> Factory { get; set; } = null!;

            internal T Current
            {
                get
                {
                    if (!_discovered) Discover();
                    return GetCurrentImpl();
                }
            }

            public T? TryGetCurrent()
            {
                if (!_discovered) return null;
                return TryGetCurrentImpl();
            }

            public void Register(IAmbientAsserterProvider provider)
            {
                lock (_lock)
                {
                    _provider = provider;
                    _discovered = true;
                }
            }

            public void Clear()
            {
                ClearImpl();
            }

            /// <summary>
            /// Override to implement the get-or-create logic for this asserter type.
            /// Should throw if no provider is available and no fallback exists.
            /// </summary>
            protected abstract T GetCurrentImpl();

            /// <summary>
            /// Override to implement the try-get logic (may return null).
            /// </summary>
            protected abstract T? TryGetCurrentImpl();

            /// <summary>
            /// Override to implement provider and fallback cleanup.
            /// </summary>
            protected abstract void ClearImpl();

            protected virtual void Discover()
            {
                lock (_lock)
                {
                    if (_discovered) return;
                    _discovered = true;
                    foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        Type[] types;
                        try { types = asm.GetExportedTypes(); }
                        catch { continue; }

                        foreach (var type in types)
                        {
                            if (type.IsAbstract || !typeof(IAmbientAsserterProvider).IsAssignableFrom(type))
                                continue;

                            try
                            {
                                _provider = (IAmbientAsserterProvider)Activator.CreateInstance(type)!;
                                return;
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Ambient source for stateful asserters (single instance managing both hard and soft assertions).
        /// </summary>
        public sealed class AmbientStatefulSource : AmbientSource<StatefulAsserter>
        {
            private readonly AsyncLocal<StatefulAsserter?> _fallbackCurrent = new();

            public AmbientStatefulSource()
            {
                Factory = () => Asserter.NewStateful();
            }

            protected override StatefulAsserter GetCurrentImpl()
            {
                return _provider?.GetOrCreateStateful() ?? (_fallbackCurrent.Value ??= Factory());
            }

            protected override StatefulAsserter? TryGetCurrentImpl()
            {
                return _provider?.TryGetStateful() ?? _fallbackCurrent.Value;
            }

            protected override void ClearImpl()
            {
                _provider?.ClearStateful();
                _fallbackCurrent.Value = null;
            }
        }
    }
}

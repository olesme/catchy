using System;
using System.Threading;
using NUnit.Framework.Internal;

namespace Catchy.NUnit
{
    /// <summary>
    /// NUnit provider with native TestContext.Properties storage (preferred) and AsyncLocal fallback.
    /// Manages single StatefulAsserter instance per test scope.
    /// </summary>
    public sealed class NUnitSoftAsserterProvider : IAmbientAsserterProvider
    {
        private const string StatefulAsserterKey = "__Catchy_StatefulAsserter__";

        // Fallback for async contexts not in test scope
        private readonly AsyncLocal<StatefulAsserter?> _fallback = new();

        public StatefulAsserter GetOrCreateStateful()
        {
            // Try NUnit TestContext first (native, auto-cleanup)
            var props = TestExecutionContext.CurrentContext?.CurrentTest?.Properties;
            if (props != null)
            {
                if (props.Get(StatefulAsserterKey) is not StatefulAsserter existing)
                {
                    existing = AmbientAsserterSource.Stateful.Factory();
                    props.Set(StatefulAsserterKey, existing);
                }
                return existing;
            }

            // Fallback to AsyncLocal
            return _fallback.Value ??= AmbientAsserterSource.Stateful.Factory();
        }

        public StatefulAsserter? TryGetStateful()
        {
            // Try NUnit TestContext first
            var props = TestExecutionContext.CurrentContext?.CurrentTest?.Properties;
            if (props?.Get(StatefulAsserterKey) is StatefulAsserter existing)
                return existing;

            // Fallback to AsyncLocal
            return _fallback.Value;
        }

        public void ClearStateful()
        {
            // NUnit recreates test properties per test scope, so only the async fallback needs explicit clearing.
            _fallback.Value = null;
        }

        public static void Register()
        {
            var provider = new NUnitSoftAsserterProvider();
            AmbientAsserterSource.Stateful.Register(provider);
        }
    }
}

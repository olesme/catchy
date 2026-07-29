using System.Threading;

namespace Catchy.MSTest
{
    /// <summary>
    /// MSTest provider with native TestContext.Properties storage (preferred) and AsyncLocal fallback.
    /// Manages single StatefulAsserter instance per test scope.
    /// </summary>
    public sealed class MsTestStatefulAsserterProvider : IAmbientAsserterProvider
    {
        private static readonly AsyncLocal<Microsoft.VisualStudio.TestTools.UnitTesting.TestContext?> CurrentTestContext = new();
        private const string StatefulAsserterKey = "__Catchy_StatefulAsserter__";

        // Fallback for async contexts not in TestContext scope
        private readonly AsyncLocal<StatefulAsserter?> _fallback = new();

        public StatefulAsserter GetOrCreateStateful()
        {
            // Try TestContext first (native MSTest, auto-cleanup)
            var testContext = CurrentTestContext.Value;
            if (testContext != null)
            {
                if (!testContext.Properties.ContainsKey(StatefulAsserterKey))
                {
                    testContext.Properties[StatefulAsserterKey] = AmbientAsserterSource.Stateful.Factory();
                }

                if (testContext.Properties[StatefulAsserterKey] is StatefulAsserter existing)
                {
                    return existing;
                }
            }

            // Fallback to AsyncLocal for async contexts
            return _fallback.Value ??= AmbientAsserterSource.Stateful.Factory();
        }

        public StatefulAsserter? TryGetStateful()
        {
            // Try TestContext first
            var testContext = CurrentTestContext.Value;
            if (testContext?.Properties[StatefulAsserterKey] is StatefulAsserter existing)
            {
                return existing;
            }

            // Fallback to AsyncLocal
            return _fallback.Value;
        }

        public void ClearStateful()
        {
            // Clear both
            var testContext = CurrentTestContext.Value;
            if (testContext?.Properties.ContainsKey(StatefulAsserterKey) == true)
            {
                testContext.Properties.Remove(StatefulAsserterKey);
            }

            _fallback.Value = null;
        }

        /// <summary>
        /// Called in [TestInitialize] to set the current TestContext.
        /// TestContext.Properties is automatically cleaned up between tests by MSTest.
        /// </summary>
        public static void Init(Microsoft.VisualStudio.TestTools.UnitTesting.TestContext testContext)
        {
            CurrentTestContext.Value = testContext;
        }

        public static void Register()
        {
            var provider = new MsTestStatefulAsserterProvider();
            AmbientAsserterSource.Stateful.Register(provider);
        }
    }
}

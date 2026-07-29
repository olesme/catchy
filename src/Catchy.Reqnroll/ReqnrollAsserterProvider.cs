using System.Threading;

namespace Catchy.ReqnrollPlugin
{
    public sealed class ReqnrollAsserterProvider : IAmbientAsserterProvider
    {
        private readonly AsyncLocal<StatefulAsserter?> _stateful = new();

        private static ReqnrollAsserterProvider? _instance;

        public static void EnsureRegistered()
        {
            if (_instance != null) return;
            _instance = new ReqnrollAsserterProvider();
            AmbientAsserterSource.Stateful.Register(_instance);
        }

        public static void SetStateful(StatefulAsserter? stateful) => _instance?._stateful.Value = stateful;

        public StatefulAsserter GetOrCreateStateful() => _stateful.Value ??= AmbientAsserterSource.Stateful.Factory();
        public StatefulAsserter? TryGetStateful() => _stateful.Value;
        public void ClearStateful() => _stateful.Value = null;
    }
}

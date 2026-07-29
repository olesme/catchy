using System.Threading;

namespace Catchy.XUnit
{
    public sealed class XUnitStatefulAsserterProvider : IAmbientAsserterProvider
    {
        private readonly AsyncLocal<StatefulAsserter?> _stateful = new();

        public StatefulAsserter GetOrCreateStateful() => _stateful.Value ??= AmbientAsserterSource.Stateful.Factory();
        public StatefulAsserter? TryGetStateful() => _stateful.Value;
        public void ClearStateful() => _stateful.Value = null;

        public static void Register()
        {
            var provider = new XUnitStatefulAsserterProvider();
            AmbientAsserterSource.Stateful.Register(provider);
        }
    }
}

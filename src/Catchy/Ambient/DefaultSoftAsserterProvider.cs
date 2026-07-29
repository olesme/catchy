namespace Catchy
{
    public sealed class DefaultSoftAsserterProvider : IAmbientAsserterProvider
    {
        private readonly AsyncLocal<StatefulAsserter?> _stateful = new();

        public StatefulAsserter GetOrCreateStateful() => _stateful.Value ??= AmbientAsserterSource.Stateful.Factory();
        public StatefulAsserter? TryGetStateful() => _stateful.Value;
        public void ClearStateful() => _stateful.Value = null;

        public static void Init() => AmbientAsserterSource.Stateful.Register(new DefaultSoftAsserterProvider());
    }
}

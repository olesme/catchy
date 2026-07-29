using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Catchy.TUnit
{
    public sealed class TUnitScopeProvider : IAmbientAsserterProvider
    {
        // Use ThreadLocal instead of AsyncLocal for TUnit
        // because TUnit can have multiple async contexts per test
        private readonly ThreadLocal<StatefulAsserter?> _stateful = new();

        public StatefulAsserter GetOrCreateStateful() => _stateful.Value ??= AmbientAsserterSource.Stateful.Factory();
        public StatefulAsserter? TryGetStateful() => _stateful.Value;
        public void ClearStateful() => _stateful.Value = null;

        [SuppressMessage("Usage", "CA2255:The 'ModuleInitializer' attribute is only intended to be used in application code or advanced source generator scenarios", Justification = "Library-level ambient provider registration must run once at module load for TUnit integration.")]
        [System.Runtime.CompilerServices.ModuleInitializer]
        public static void Register()
        {
            var provider = new TUnitScopeProvider();
            AmbientAsserterSource.Stateful.Register(provider);
        }
    }
}

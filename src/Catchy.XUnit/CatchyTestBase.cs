using System.Threading.Tasks;
using Xunit;

namespace Catchy.XUnit
{
    /// <summary>
    /// Base class providing per-test soft assertion lifecycle.
    /// </summary>
    public abstract class CatchyTestBase : IAsyncLifetime
    {
        static CatchyTestBase()
        {
            XUnitStatefulAsserterProvider.Register();
        }

        public virtual ValueTask InitializeAsync()
        {
            _ = Ambient.Assert.Soft; // ensure created for this test
            return ValueTask.CompletedTask;
        }

        public virtual async ValueTask DisposeAsync()
        {
            var softAsserter = Ambient.Assert.Soft;
            AmbientAsserterSource.Clear();
            await softAsserter.SoftState.FlushIfNeeded();
        }
    }
}

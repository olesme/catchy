using System.Threading.Tasks;
using Xunit;

namespace Catchy.XUnit
{
    public class CatchyFixture : IAsyncLifetime
    {
        public ValueTask InitializeAsync()
        {
            XUnitStatefulAsserterProvider.Register();
            return ValueTask.CompletedTask;
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}

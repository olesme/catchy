using System.Threading.Tasks;
using Catchy;

namespace Catchy.TUnit
{
    /// <summary>
    /// Global TUnit hooks for Catchy ambient assertions.
    /// These hooks are automatically discovered and executed by TUnit for all tests.
    /// </summary>
    public sealed class CatchyHooks
    {
        [BeforeEvery(Test, Order = int.MinValue)]
        public static void Before()
        {
            var soft = Ambient.Assert.Soft;
            soft.Clear();  // Clear errors but keep asserter instance
        }

        [AfterEvery(Test, Order = int.MaxValue)]
        public static async Task After()
        {
            var softAsserter = Ambient.Assert.Soft;
            AmbientAsserterSource.Clear();
            await softAsserter.SoftState.FlushIfNeeded();
        }
    }
}

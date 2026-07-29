using System.Threading.Tasks;
using Reqnroll;

namespace Catchy.ReqnrollPlugin
{
    [Binding]
    public sealed class CatchyHooks(StatefulAsserter assert, ScenarioContext ctx)
    {
        // Set scenario error so following hooks fill see it
        [AfterScenario(Order = int.MinValue)]
        public async Task SetSofError()
        {
            if (assert.Soft.ErrorCount == 0)
                return;
            if (ctx.TestError != null)
                return; // Don't override an existing error, just log the soft assertion failures and let the original error be visible in test results.

            var ex = assert.Soft.SoftState.AggregateException!;
            ctx.InjectError(ex);
        }

        // Throw after all other hooks (trow will fail all following hooks as well)
        [AfterScenario(Order = int.MaxValue)]
        public async Task ThrowOnSofError()
        {
            if (ctx.IsSoftFailInjected())
            {
                await assert.Soft.SoftState.FlushIfNeeded();
            }
        }
    }
}

using System.Threading.Tasks;
using Reqnroll;

namespace Catchy.ReqnrollPlugin
{
    /// <summary>
    /// An example for step definition classes. <see cref="SoftAsserter"/> is resolved from DI
    /// and auto-flushed at scenario end via <c>CatchyHooks</c>.
    /// Add notifiers via <c>assert.SoftState.OnFlush</c>.
    /// </summary>
    [Binding]
    public class CatchyStepsExample(StatefulAsserter assert)
    {
        [When("I got soft fail")]
        public async Task WhenIGotSoftFail()
        {
            await assert.Soft.That(true).IsFalse();
        }

        [Then("the soft fails count should be {int}")]
        public async Task ThenTheSoftFailsCountShouldBe(int count)
        {
            await assert.That().SoftState().Errors().HasCount(count);
        }

        [When("I cleanup soft fails")]
        public void WhenICleanupSoftFails()
        {
            assert.Soft.Clear();
        }

        [When("I flush hard")]
        public async Task WhenIFlushHard()
        {
            await assert.Soft.SoftState.FlushIfNeeded();
        }

        [Then("the test will fail")]
        public void ThenTheTestWillFail()
        {
            // This step is just a placeholder to indicate that the previous flush should cause a test failure.
        }
    }
}

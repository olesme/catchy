using Catchy;
using CatchyCoreTests.Helpers;

namespace CatchyCoreTests.Assertions
{
    public class SoftStateAssertionsTests
    {
        [Fact]
        public async Task HasNoErrors_WithCleanSoftState_Passes()
        {
            var verify = new SoftAsserter();
            await Stateless.Assert.That(verify.SoftState).HasNoErrors();
        }

        [Fact]
        public async Task HasNoErrors_WithFailures_Throws()
        {
            var verify = new SoftAsserter();
            await verify.That(1).Is(2);
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(verify.SoftState).HasNoErrors()
            );

            Assert.Contains("1 soft assertion", message.ToLowerInvariant());
        }

        [Fact]
        public async Task Errors_WithFailures_ExposesCapturedExceptions()
        {
            var verify = new SoftAsserter();
            await verify.That(1).Is(2);
            await verify.That("a").Is("b");

            await Stateless.Assert.That(verify.SoftState).Errors().HasCount(2);
        }

        [Fact]
        public async Task UnlessAlreadyFlushed_WithAlreadyFlushedState_Passes()
        {
            var softState = new SoftState { AlreadyFlushed = true };
            await Stateless.Assert.That(softState).UnlessAlreadyFlushed();
        }
    }
}

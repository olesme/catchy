using Catchy.XUnit;
using Catchy;
using CatchyTestHelpers;

namespace AmbientXUnitTests
{
    public class SoftAssertionsTests: CatchyTestBase
    {
        [Fact]
        public async Task SoftAsserter_HardFlush_throws_aggregate()
        {
            await Ambient.Assert.Soft.That(1).Is(2);
            await Ambient.Assert.Soft.That("a").Is("b");
            try
            {
                await Stateless.Assert.That(Ambient.Assert.Soft).HasNoErrors();
            }
            catch (AggregateAssertionException ex)
            {
                Assert.Equal(2, ex.InnerExceptions.Count);
            }
            // Auto flush will not throw since we already flushed manually
        }

        [Fact]
        public async Task SoftAsserter_no_failures_TryFlush_does_not_throw()
        {
            await Ambient.Assert.Soft.That(42).Is(42);
            var ex = Ambient.Assert.Soft.SoftState.AggregateException;
            Assert.Null(ex);
        }

        [Fact]
        public async Task SoftAsserter_Clear_resets_failures()
        {
            await Ambient.Assert.Soft.That(1).Is(2);
            Ambient.Assert.Soft.Clear();
            Assert.False(Ambient.Assert.Soft.HasFailures);
            Assert.Equal(0, Ambient.Assert.Soft.ErrorCount);
        }

        // Ambient Stateless Asserter tests
        [Fact]
        public async Task Ambient_Hard_throws_on_assertion()
        {
            var msg = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Ambient.Assert.That(1).Is(2)
            );
            Assert.Contains("Assertion failed", msg);
        }

        [Fact]
        public async Task Ambient_Hard_passes_valid_assertion()
        {
            await Ambient.Assert.That(1).Is(1);
            // Should pass
        }
    }
}

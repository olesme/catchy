using Catchy;
using CatchyTestHelpers;

namespace AmbientTUnitTests
{
    public class SoftAssertionsTests
    {
        [Test]
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
                await Assert.That(ex.InnerExceptions.Count).IsEqualTo(2);
            }
        }

        [Test]
        public async Task SoftAsserter_no_failures_TryFlush_does_not_throw()
        {
            await Ambient.Assert.Soft.That(42).Is(42);
            var ex = Ambient.Assert.Soft.SoftState.AggregateException;
            await Assert.That(ex).IsNull();
        }

        [Test]
        public async Task SoftAsserter_Clear_resets_failures()
        {
            await Ambient.Assert.Soft.That(1).Is(2);
            Ambient.Assert.Soft.Clear();
            await Assert.That(Ambient.Assert.Soft.HasFailures).IsFalse();
            await Assert.That(Ambient.Assert.Soft.ErrorCount).IsEqualTo(0);
        }

        // Ambient Stateless Asserter tests
        [Test]
        public async Task Ambient_Hard_throws_on_assertion()
        {
            var msg = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Ambient.Assert.That(1).Is(2)
            );
            await Assert.That(msg).Contains("Assertion failed");
        }

        [Test]
        public async Task Ambient_Hard_passes_valid_assertion()
        {
            await Ambient.Assert.That(1).Is(1);
            // Should pass
        }
    }
}


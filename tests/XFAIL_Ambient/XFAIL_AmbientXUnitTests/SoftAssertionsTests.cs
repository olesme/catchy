using Catchy.XUnit;
using Catchy;

namespace AmbientXUnitTests
{
    public class SoftAssertionsTests: CatchyTestBase
    {
        [Fact]
        public async Task XFAIL_SoftAsserter_collects_failures_without_throwing()
        {
            await Ambient.Assert.Soft.That(1).Is(2);
            await Ambient.Assert.Soft.That("a").Is("b");
            await Ambient.Assert.Soft.That(true).IsTrue();
            Assert.True(Ambient.Assert.Soft.HasFailures);
            Assert.Equal(2, Ambient.Assert.Soft.ErrorCount);
            // Should autofail on cleanup (autoflush)
        }

        [Fact]
        public async Task XFAIL_SoftAsserter_AggregateException()
        {
            await Ambient.Assert.Soft.That(1).Is(2);
            await Ambient.Assert.Soft.That("a").Is("b");
            AggregateAssertionException ex = Ambient.Assert.Soft.SoftState.AggregateException!;
            Assert.Equal(2, ex.InnerExceptions.Count);
            // Should autofail on cleanup (autoflush)
        }

        [Fact]
        public async Task XFAIL_With_SoftAsserter_routes_to_shared_state()
        {
            await Stateless.Assert.That(1).Is(999).With(Ambient.Assert.Soft);
            Assert.True(Ambient.Assert.Soft.HasFailures);
            // Should autofail on cleanup (autoflush)
        }
    }
}

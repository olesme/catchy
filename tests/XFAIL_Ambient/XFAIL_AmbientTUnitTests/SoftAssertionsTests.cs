using Catchy;

namespace AmbientTUnitTests
{
    public class SoftAssertionsTests
    {
        [Test]
        public async Task XFAIL_SoftAsserter_collects_failures_without_throwing()
        {
            await Ambient.Assert.Soft.That(1).Is(2);
            await Ambient.Assert.Soft.That("a").Is("b");
            await Ambient.Assert.Soft.That(true).IsTrue();
            await Assert.That(Ambient.Assert.Soft.HasFailures).IsTrue();
            await Assert.That(Ambient.Assert.Soft.ErrorCount).IsEqualTo(2);
            // Should autofail on cleanup (autoflush)
        }

        [Test]
        public async Task XFAIL_SoftAsserter_AggregateException()
        {
            await Ambient.Assert.Soft.That(1).Is(2);
            await Ambient.Assert.Soft.That("a").Is("b");
            AggregateAssertionException ex = Ambient.Assert.Soft.SoftState.AggregateException!;
            await Assert.That(ex.InnerExceptions.Count).IsEqualTo(2);
            // Should autofail on cleanup (autoflush)
        }

        [Test]
        public async Task XFAIL_With_SoftAsserter_routes_to_shared_state()
        {
            await Stateless.Assert.That(1).Is(999).With(Ambient.Assert.Soft);
            await Assert.That(Ambient.Assert.Soft.HasFailures).IsTrue();
            // Should autofail on cleanup (autoflush)
        }
    }
}


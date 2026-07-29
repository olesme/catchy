using Catchy.NUnit;
using Catchy;

namespace AmbientNUnitTests
{
    [TestFixture]
    public class SoftAssertionTests : AmbientNUnitBase
    {
        [Test]
        public async Task XFAIL_SoftAsserter_collects_failures_without_throwing()
        {
            await Ambient.Assert.Soft.That(1).Is(2);
            await Ambient.Assert.Soft.That("a").Is("b");
            await Ambient.Assert.Soft.That(true).IsTrue();
            Assert.That(Ambient.Assert.Soft.HasFailures, Is.True);
            Assert.That(Ambient.Assert.Soft.ErrorCount, Is.EqualTo(2));
            // Should autofail on cleanup (autoflush)
        }

        [Test]
        public async Task XFAIL_SoftAsserter_AggregateException()
        {
            await Ambient.Assert.Soft.That(1).Is(2);
            await Ambient.Assert.Soft.That("a").Is("b");
            AggregateAssertionException ex = Ambient.Assert.Soft.SoftState.AggregateException!;
            Assert.That(ex.InnerExceptions.Count, Is.EqualTo(2));
            // Should autofail on cleanup (autoflush)
        }

        [Test]
        public async Task XFAIL_With_SoftAsserter_routes_to_shared_state()
        {
            await Stateless.Assert.That(1).Is(999).With(Ambient.Assert.Soft);
            Assert.That(Ambient.Assert.Soft.HasFailures, Is.True);
            // Should autofail on cleanup (autoflush)
        }
    }
}

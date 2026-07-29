using Catchy;
using CatchyCoreTests.Helpers;

namespace CatchyCoreTests.Core
{
    public class ConditionalTests
    {
        [Fact]
        public async Task When_false_skips_failing_assertion()
            // Would fail if not skipped
            => await Stateless.Assert.That(1).Is(999).When(false);

        [Fact]
        public async Task When_true_does_not_skip()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(1).Is(999).When(true));
            Assert.Contains("999", msg);
        }

        [Fact]
        public async Task WhenNot_false_does_not_skip()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(1).Is(999).WhenNot(false));
            Assert.Contains("999", msg);
        }

        [Fact]
        public async Task WhenNot_true_skips()
            => await Stateless.Assert.That(1).Is(999).WhenNot(true);
    }
}

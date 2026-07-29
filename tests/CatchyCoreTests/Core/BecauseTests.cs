using Catchy;
using CatchyCoreTests.Helpers;

namespace CatchyCoreTests.Core
{
    public class BecauseTests
    {
        [Fact]
        public async Task Because_appends_reason_to_error()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(1).Is(2).Because("version must be pinned for deployment"));
            Assert.Contains("version must be pinned for deployment", msg);
        }

        [Fact]
        public async Task Because_does_not_affect_passing_assertion()
            => await Stateless.Assert.That(42).Is(42).Because("just because");

        [Fact]
        public async Task Because_appears_in_chain_links()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(1).Is(99).Because("some reason"));
            Assert.Contains("Because", msg);
            Assert.Contains("some reason", msg);
        }
    }
}

using Catchy;
using CatchyCoreTests.Helpers;

namespace CatchyCoreTests.Assertions.Objects
{
    public class EqualityAssertionTests
    {
        [Fact]
        public async Task Satisfies_passes()
            => await Stateless.Assert.That("hello world").Satisfies(s => s!.Length > 5);

        [Fact]
        public async Task Satisfies_with_description_fails_with_description_in_message()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That("hi").Satisfies(s => s!.Length > 10, "string must be longer than 10"));
            Assert.Contains("string must be longer than 10", msg);
        }

        [Fact]
        public async Task Satisfies_fails()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That("hi").Satisfies(s => s!.Length > 100));
            Assert.Contains("satisfy", msg.ToLower());
        }
    }
}

using Catchy;
using CatchyCoreTests.Helpers;

namespace CatchyCoreTests.Core
{
    public class ChainTruncationTests
    {
        [Fact]
        public async Task Long_expression_is_truncated_in_chain()
        {
            // Create a very long variable expression by using a long local
            var veryLongVariableName_thatExceedsTheMaxLengthForChainLinksTruncation_definitelyOver60Chars = 42;
            var msg = await Catch.FailureOf(async ()
                => await Stateless.Assert.That(veryLongVariableName_thatExceedsTheMaxLengthForChainLinksTruncation_definitelyOver60Chars)
                    .Is(99));

            // Truncation placeholder pattern: {trunc1}
            Assert.Contains("{t", msg);
            Assert.Contains("=>", msg); // truncation legend line
        }

        [Fact]
        public async Task Short_expression_is_not_truncated()
        {
            int x = 42;
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(x).Is(99));
            Assert.DoesNotContain("{t", msg);
        }
    }
}

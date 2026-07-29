using Catchy;
using CatchyCoreTests.Helpers;

namespace CatchyCoreTests.Core
{
    public class ChainingTests
    {
        [Fact]
        public async Task And_connector_all_pass()
            => await Stateless.Assert.That(42).Is(42).And().IsPositive().And().IsEven();

        [Fact]
        public async Task But_connector_all_pass()
            => await Stateless.Assert.That(3).IsPositive().But().IsOdd();

        [Fact]
        public async Task Then_connector_all_pass()
            => await Stateless.Assert.That("hello").IsNotNullOrEmpty().Then().HasLength(5);

        [Fact]
        public async Task Chain_short_circuits_on_first_failure()
        {
            // First op fails: Is(99) → should throw before reaching IsPositive
            var msg = await Catch.FailureOf(async ()
                => await Stateless.Assert.That(42).Is(99).And().IsPositive());
            Assert.Contains("42", msg);
        }

        [Fact]
        public async Task Chain_links_appear_in_error_message()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(1).IsGreaterThan(100));
            // The chain line should be present in the formatted output
            Assert.Contains("Stateless.Assert", msg);
            Assert.Contains("IsGreaterThan", msg);
        }

        [Fact]
        public async Task Multi_op_chain_second_op_fails()
        {
            var msg = await Catch.FailureOf(async ()
                => await Stateless.Assert.That(42).Is(42).And().Is(99));
            Assert.Contains("99", msg);
        }
    }
}

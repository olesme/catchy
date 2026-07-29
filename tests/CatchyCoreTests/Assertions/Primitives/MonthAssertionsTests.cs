using Catchy;
using CatchyCoreTests.Helpers;

namespace CatchyCoreTests.Assertions.Primitives
{
    public class MonthAssertionsTests
    {
        [Fact]
        public async Task Is_WithMatchingEnum_Passes()
        {
            await Stateless.Assert.That(Month.January).Is(Month.January);
        }

        [Fact]
        public async Task Is_WithDifferentEnum_Fails()
        {
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(Month.January).Is(Month.February)
            );

            Assert.Contains("january", message.ToLower());
            Assert.Contains("february", message.ToLower());
        }

        [Fact]
        public async Task Is_WithMatchingInt_Passes()
        {
            await Stateless.Assert.That(Month.March).Is(3);
        }

        [Fact]
        public async Task Is_WithMatchingString_Passes()
        {
            await Stateless.Assert.That(Month.April).Is("April");
        }

        [Fact]
        public async Task Is_WithMatchingAbbreviatedString_Passes()
        {
            await Stateless.Assert.That(Month.September).Is("sep");
        }

        [Fact]
        public async Task Is_WithMatchingNumericString_Passes()
        {
            await Stateless.Assert.That(Month.December).Is("12");
        }

        [Fact]
        public async Task IsNot_WithDifferentString_Passes()
        {
            await Stateless.Assert.That(Month.May).IsNot("June");
        }

        [Fact]
        public async Task IsNot_WithEquivalentNumericString_Fails()
        {
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(Month.May).IsNot("5")
            );

            Assert.Contains("not to be", message.ToLower());
        }

        [Fact]
        public async Task IsBefore_WithEarlierMonth_Passes()
        {
            await Stateless.Assert.That(Month.January).IsBefore(Month.February);
        }

        [Fact]
        public async Task IsBefore_WithLaterInt_Fails()
        {
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(Month.December).IsBefore(1)
            );

            Assert.Contains("before", message.ToLower());
        }

        [Fact]
        public async Task IsAfter_WithLaterMonth_Passes()
        {
            await Stateless.Assert.That(Month.December).IsAfter(Month.November);
        }

        [Fact]
        public async Task IsAfter_WithEarlierInt_Fails()
        {
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(Month.January).IsAfter(12)
            );

            Assert.Contains("after", message.ToLower());
        }

        [Fact]
        public async Task IsJanuary_Passes()
        {
            await Stateless.Assert.That(Month.January).IsJanuary();
        }

        [Fact]
        public async Task IsNotJanuary_Passes()
        {
            await Stateless.Assert.That(Month.February).IsNotJanuary();
        }

        [Fact]
        public async Task IsFebruary_Passes()
        {
            await Stateless.Assert.That(Month.February).IsFebruary();
        }

        [Fact]
        public async Task IsMarch_Passes()
        {
            await Stateless.Assert.That(Month.March).IsMarch();
        }

        [Fact]
        public async Task IsApril_Passes()
        {
            await Stateless.Assert.That(Month.April).IsApril();
        }

        [Fact]
        public async Task IsMay_Passes()
        {
            await Stateless.Assert.That(Month.May).IsMay();
        }

        [Fact]
        public async Task IsJune_Passes()
        {
            await Stateless.Assert.That(Month.June).IsJune();
        }

        [Fact]
        public async Task IsJuly_Passes()
        {
            await Stateless.Assert.That(Month.July).IsJuly();
        }

        [Fact]
        public async Task IsAugust_Passes()
        {
            await Stateless.Assert.That(Month.August).IsAugust();
        }

        [Fact]
        public async Task IsSeptember_Passes()
        {
            await Stateless.Assert.That(Month.September).IsSeptember();
        }

        [Fact]
        public async Task IsOctober_Passes()
        {
            await Stateless.Assert.That(Month.October).IsOctober();
        }

        [Fact]
        public async Task IsNovember_Passes()
        {
            await Stateless.Assert.That(Month.November).IsNovember();
        }

        [Fact]
        public async Task IsDecember_Passes()
        {
            await Stateless.Assert.That(Month.December).IsDecember();
        }

        [Fact]
        public async Task IsNotDecember_WithDecember_Fails()
        {
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(Month.December).IsNotDecember()
            );

            Assert.Contains("december", message.ToLower());
        }
    }
}

using Catchy;
using CatchyCoreTests.Core;
using CatchyTestHelpers;

namespace CatchyCoreTests.Core
{
    public class BetweenExclusivelyTests
    {
        [Fact]
        public async Task DateTime_IsBetween_Exclusively_passes()
        {
            var dt = new DateTime(2024, 6, 15);
            await Stateless.Assert.That(dt).IsBetween(new DateTime(2024, 1, 1), new DateTime(2025, 1, 1))
                .Exclusively();
        }

        [Fact]
        public async Task DateTime_IsBetween_Exclusively_fails_at_endpoint()
        {
            var dt = new DateTime(2024, 1, 1);
            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(dt)
                    .IsBetween(new DateTime(2024, 1, 1), new DateTime(2025, 1, 1))
                    .Exclusively());
            Assert.Contains("to be in range (", message);
        }

        [Fact]
        public async Task TimeSpan_IsBetween_Exclusively_passes()
        {
            var ts = TimeSpan.FromSeconds(5);
            await Stateless.Assert.That(ts).IsBetween(TimeSpan.Zero, TimeSpan.FromSeconds(10)).Exclusively();
        }

        [Fact]
        public async Task Double_IsBetween_Exclusively_passes()
            => await Stateless.Assert.That(3.14).IsBetween(3.0, 4.0).Exclusively();

        [Fact]
        public async Task Decimal_IsBetween_Exclusively_passes()
            => await Stateless.Assert.That(5.5m).IsBetween(1m, 10m).Exclusively();
    }
}

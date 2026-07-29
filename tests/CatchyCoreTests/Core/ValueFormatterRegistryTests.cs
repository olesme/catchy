using Catchy;
using CatchyCoreTests.Core;
using CatchyTestHelpers;

namespace CatchyCoreTests.Core
{
    public class ValueFormatterRegistryTests
    {
        private record Pt(int X, int Y);

        [Fact]
        public async Task Custom_formatter_appears_in_failure_message()
        {
            Catchy.Sdk.ValueFormatterRegistry.Register<Pt>(p => $"Pt({p.X},{p.Y})");

            var msg = await TestHelpers.ShouldFailWithMessageAsync(async () =>
            await Stateless.Assert.That(new Pt(1, 2)).IsNull());
            Assert.Contains("Pt(1,2)", msg);
        }

        [Fact]
        public void IFormattable_used_for_decimal_without_registration()
        {
            var result = Catchy.Sdk.ValueFormatter.Format(3.14m);
            Assert.Contains("3.14", result);
        }
    }
}

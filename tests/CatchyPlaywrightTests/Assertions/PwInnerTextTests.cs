using Catchy;
using CatchyPlaywrightTests.Support;
using CatchyTestHelpers;

namespace CatchyPlaywrightTests.Assertions
{
    public sealed class PwInnerTextTests : PlaywrightTestFixture
    {
        [Test]
        public async Task InnerTextIs()
        {
            var page = EnsurePage();
            await Stateless.Assert.That(page.Locator("h1")).InnerText().Is("todos");
            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
            await Stateless.Assert.That(page.Locator("h1")).InnerText().Is("wrong")
            );
            await Stateless.Assert.That(message).Contains("Expected \"todos\" to equal \"wrong\"");
            await Stateless.Assert.That(message).Contains("InnerText");
        }

        [Test]
        public async Task InnerTextContains()
        {
            var page = EnsurePage();
            await Stateless.Assert.That(page.Locator("h1")).InnerText().Contains("odo");
            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
            await Stateless.Assert.That(page.Locator("h1")).InnerText().Contains("dod")
            );
            await Stateless.Assert.That(message).Contains("Expected \"todos\" to contain \"dod\"");
            await Stateless.Assert.That(message).Contains("InnerText");
        }
    }
}

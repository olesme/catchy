using Catchy;
using CatchyPlaywrightTests.Support;
using CatchyTestHelpers;

namespace CatchyPlaywrightTests.Assertions
{
    public sealed class PwPageAssertionsTests : PlaywrightTestFixture
    {
        [Test]
        public async Task HasTitle_Positive()
        {
            var page = EnsurePage();
            var title = await page.TitleAsync();
            await Stateless.Assert.That(title).Is("Playwright Test Page");
        }

        [Test]
        public async Task HasTitle_Negative_ThrowsAssertionException()
        {
            var page = EnsurePage();
            var title = await page.TitleAsync();

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(title).Is("wrong-title"));

            await Stateless.Assert.That(message).Contains("Expected \"Playwright Test Page\" to equal \"wrong-title\"");
            await Stateless.Assert.That(message).Contains("wrong-title");
        }

        [Test]
        public async Task TitleContains_Positive()
        {
            var page = EnsurePage();
            await Stateless.Assert.That(page).Title().Contains("Playwright");
        }

        [Test]
        public async Task TitleContains_Negative_ThrowsAssertionException()
        {
            var page = EnsurePage();

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(page).Title().Contains("missing-substring"));

            await Stateless.Assert.That(message).Contains("Expected \"Playwright Test Page\" to contain \"missing-substring\"");
            await Stateless.Assert.That(message).Contains("missing-substring");
        }
    }
}

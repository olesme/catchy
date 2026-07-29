using Catchy;
using CatchyPlaywrightTests.Support;
using CatchyTestHelpers;
using Microsoft.Playwright;

namespace CatchyPlaywrightTests.Assertions
{
    public sealed class PwTextModifiersTests : PlaywrightTestFixture
    {
        [Test]
        public async Task HasText_CaseInsensitive()
        {
            var page = EnsurePage();
            await Stateless.Assert.That(page.Locator("h1"))
                .HasText("TODOS")
                .IgnoringCase();
        }

        [Test]
        public async Task ContainsText_CaseInsensitive()
        {
            var page = EnsurePage();
            await Stateless.Assert.That(page.Locator("h1"))
                .ContainsText("TO")
                .IgnoringCase();

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(page.Locator("h1"))
                    .HasText("TO"));

            await Stateless.Assert.That(message).Contains("Locator expected to have text 'TO'");
        }

        [Test]
        public async Task StringComparisonModifiers_Available_On_All_Playwright_TextAware_Surfaces()
        {
            _ = Stateless.Assert.That((ILocator)null!)
                .UsingOrdinal()
                .UsingCurrentCulture()
                .UsingInvariantCulture()
                .IgnoringCase()
                .RespectingCase();

            _ = Stateless.Assert.That((IPage)null!)
                .UsingOrdinal()
                .UsingCurrentCulture()
                .UsingInvariantCulture()
                .IgnoringCase()
                .RespectingCase();

            _ = Stateless.Assert.That((IFrame?)null)
                .UsingOrdinal()
                .UsingCurrentCulture()
                .UsingInvariantCulture()
                .IgnoringCase()
                .RespectingCase();

            _ = Stateless.Assert.That((IAPIResponse)null!)
                .UsingOrdinal()
                .UsingCurrentCulture()
                .UsingInvariantCulture()
                .IgnoringCase()
                .RespectingCase();

            _ = Stateless.Assert.That((IBrowserContext)null!)
                .UsingOrdinal()
                .UsingCurrentCulture()
                .UsingInvariantCulture()
                .IgnoringCase()
                .RespectingCase();

            _ = Stateless.Assert.That((IDownload)null!)
                .UsingOrdinal()
                .UsingCurrentCulture()
                .UsingInvariantCulture()
                .IgnoringCase()
                .RespectingCase();

            await Task.CompletedTask;
        }
    }
}

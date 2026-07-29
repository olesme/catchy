using Catchy;
using CatchyPlaywrightTests.Support;

namespace CatchyPlaywrightTests.Assertions
{
    public sealed class PwStatefulAndSoftTests : PlaywrightTestFixture
    {
        [Test]
        public async Task StatefulAsserter_Chaining_MultipleAssertions()
        {
            var page = EnsurePage();
            var stateful = Asserter.NewStateful();

            var title = await page.TitleAsync();
            await stateful.That(title).Is("Playwright Test Page");
            await stateful.That(page.Locator("#todo-input")).IsVisible();
            await stateful.That(page.Locator("#todo-input")).IsEnabled();
        }

        [Test]
        public async Task SoftAsserter_AccumulatesFailures()
        {
            var page = EnsurePage();
            var verify = Asserter.NewSoft();

            await verify.That(page.Locator("#todo-input")).IsVisible();
            await verify.That(page.Locator("#todo-input")).IsEnabled();
            await verify.That(page.Locator("#todo-input")).IsChecked();

            await Stateless.Assert.That(verify.ErrorCount).Is(1);
        }

        [Test]
        public async Task SoftAsserter_MultipleSoftAssertions_AccumulateAllErrors()
        {
            var page = EnsurePage();
            var verify = Asserter.NewSoft();

            await verify.That(page.Locator("#todo-input")).IsChecked();
            await verify.That(page.Locator("h1")).IsEditable();
            await verify.That(page.Locator("#todo-input")).IsVisible();

            await Stateless.Assert.That(verify.ErrorCount).Is(2);
        }
    }
}

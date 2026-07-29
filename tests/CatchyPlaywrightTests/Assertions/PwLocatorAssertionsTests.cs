using Catchy;
using CatchyPlaywrightTests.Support;
using CatchyTestHelpers;

namespace CatchyPlaywrightTests.Assertions
{
    public sealed class PwLocatorAssertionsTests : PlaywrightTestFixture
    {
        [Test]
        public async Task IsVisible()
        {
            var page = EnsurePage();
            await Stateless.Assert.That(page.Locator("#todo-input")).IsVisible();
            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
            await Stateless.Assert.That(page.Locator("#hidden-area")).IsVisible());
            await Stateless.Assert.That(message).Contains("Locator expected to be visible");
        }

        [Test]
        public async Task IsNotVisible()
        {
            var page = EnsurePage();
            await Stateless.Assert.That(page.Locator("#hidden-area")).IsNotVisible();
            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
            await Stateless.Assert.That(page.Locator("#todo-input")).IsNotVisible());
            await Stateless.Assert.That(message).Contains("Locator expected not to be visible");
        }

        [Test]
        public async Task IsEnabled()
        {
            var page = EnsurePage();
            await Stateless.Assert.That(page.Locator("#todo-input")).IsEnabled();
            await page.EvaluateAsync("""
                () => {
                    document.getElementById('todo-input').setAttribute('disabled', 'disabled');
                }
            """);

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(page.Locator("#todo-input")).IsEnabled());
            await Stateless.Assert.That(message).Contains("Locator expected to be enabled");
        }

        [Test]
        public async Task IsDisabled()
        {
            var page = EnsurePage();
            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(page.Locator("#todo-input")).IsDisabled());
            await Stateless.Assert.That(message).Contains("Locator expected to be disabled");

            await page.EvaluateAsync("""
                () => {
                    document.getElementById('todo-input').setAttribute('disabled', 'disabled');
                }
            """);

            await Stateless.Assert.That(page.Locator("#todo-input")).IsDisabled();
        }

        [Test]
        public async Task IsEditable()
        {
            var page = EnsurePage();
            await Stateless.Assert.That(page.Locator("#todo-input")).IsEditable();

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(page.Locator("h1")).IsEditable());
            await Stateless.Assert.That(message).Contains("Locator expected to be editable");
        }

        [Test]
        public async Task HasText()
        {
            var page = EnsurePage();
            await Stateless.Assert.That(page.Locator("h1")).HasText("todos");

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(page.Locator("h1")).HasText("wrong"));
            await Stateless.Assert.That(message).Contains("Locator expected to have text 'wrong'");
        }

        [Test]
        public async Task ContainsText()
        {
            var page = EnsurePage();
            await Stateless.Assert.That(page.Locator("h1")).ContainsText("tod");

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(page.Locator("h1")).ContainsText("missing"));
            await Stateless.Assert.That(message).Contains("Locator expected to contain text 'missing'");
        }

        [Test]
        public async Task HasCount()
        {
            var page = EnsurePage();
            await Stateless.Assert.That(page.Locator("#todo-list li")).HasCount(1);

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(page.Locator("#todo-list li")).HasCount(2));
            await Stateless.Assert.That(message).Contains("Locator expected to have count '2'");
        }

        [Test]
        public async Task CountGreaterThanOrEqual()
        {
            var page = EnsurePage();
            await Stateless.Assert.That(page.Locator("#todo-list li")).CountGreaterThanOrEqual(1);
            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(page.Locator("#todo-list li")).CountGreaterThanOrEqual(2));
            await Stateless.Assert.That(message).Contains("Expected locator count ≥ 2, but was 1");
        }

        [Test]
        public async Task HasAttributee()
        {
            var page = EnsurePage();
            await Stateless.Assert.That(page.Locator("#todo-input")).HasAttribute("placeholder", "What needs to be done?");

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(page.Locator("#todo-input")).HasAttribute("placeholder", "wrong"));
            await Stateless.Assert.That(message).Contains("Locator expected to have attribute 'placeholder' 'wrong'");
        }

        [Test]
        public async Task HasPlaceholder_Positive()
        {
            var page = EnsurePage();
            await Stateless.Assert.That(page.Locator("#todo-input")).HasPlaceholder("What needs to be done?");
        }

        [Test]
        public async Task InputValueIs()
        {
            var page = EnsurePage();
            await page.Locator("#todo-input").FillAsync("done");
            await Stateless.Assert.That(page.Locator("#todo-input")).InputValueIs("done");

            await page.Locator("#todo-input").FillAsync("done");

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(page.Locator("#todo-input")).InputValueIs("wrong"));

            await Stateless.Assert.That(message).Contains("Expected input value");
        }

        [Test]
        public async Task ContainsClass()
        {
            var page = EnsurePage();
            await Stateless.Assert.That(page.Locator("#todo-list li")).ContainsClass("test-class");

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(page.Locator("#todo-list li")).ContainsClass("missing-class"));
            await Stateless.Assert.That(message).Contains("Locator expected to contain class names 'missing-class'");
        }

        // Property-based API tests
        [Test]
        public async Task Count_HasCount_Property()
        {
            var page = EnsurePage();
            await Stateless.Assert.That(page.Locator("#todo-list li")).Count().Is(1);

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(page.Locator("#todo-list li")).Count().Is(2));
            await Stateless.Assert.That(message).Contains("Expected 2 to be 2, but was 1");
            await Stateless.Assert.That(message).Contains("Stateless.Assert.That(page.Locator(\"#todo-list li\")).Count().Is(2)");
        }

        [Test]
        public async Task Count_GreaterThan_Property()
        {
            var page = EnsurePage();
            await Stateless.Assert.That(page.Locator("#todo-list li")).Count().GreaterThan(0);

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(page.Locator("#todo-list li")).Count().GreaterThan(5));
            await Stateless.Assert.That(message).Contains("Expected 1 to be greater than 5");
            await Stateless.Assert.That(message).Contains("Stateless.Assert.That(page.Locator(\"#todo-list li\")).Count().GreaterThan(5)");
        }

        [Test]
        public async Task Count_GreaterThanOrEqual_Property()
        {
            var page = EnsurePage();
            await Stateless.Assert.That(page.Locator("#todo-list li")).Count().GreaterThanOrEqual(1);

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(page.Locator("#todo-list li")).Count().GreaterThanOrEqual(2));
            await Stateless.Assert.That(message).Contains("Expected 1 to be greater than or equal to 2");
            await Stateless.Assert.That(message).Contains("Stateless.Assert.That(page.Locator(\"#todo-list li\")).Count().GreaterThanOrEqual(2)");
        }

        [Test]
        public async Task Count_LessThan_Property()
        {
            var page = EnsurePage();
            await Stateless.Assert.That(page.Locator("#todo-list li")).Count().LessThan(5);

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(page.Locator("#todo-list li")).Count().LessThan(1));
            await Stateless.Assert.That(message).Contains("Expected 1 to be less than 1");
            await Stateless.Assert.That(message).Contains("Stateless.Assert.That(page.Locator(\"#todo-list li\")).Count().LessThan(1)");
        }

        [Test]
        public async Task Count_DoesNotHaveCount_Property()
        {
            var page = EnsurePage();
            await Stateless.Assert.That(page.Locator("#todo-list li")).Count().IsNot(5);

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(page.Locator("#todo-list li")).Count().IsNot(1));
            await Stateless.Assert.That(message).Contains("Expected value not to be 1, but it was");
            await Stateless.Assert.That(message).Contains("Stateless.Assert.That(page.Locator(\"#todo-list li\")).Count().IsNot(1)");
        }

        [Test]
        public async Task Count_InRange_Property()
        {
            var page = EnsurePage();
            await Stateless.Assert.That(page.Locator("#todo-list li")).Count().IsInRange(0, 5);
            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(page.Locator("#todo-list li")).Count().IsInRange(5, 10));
            await Stateless.Assert.That(message).Contains("Expected 1 to be in [5, 10]");
            await Stateless.Assert.That(message).Contains("Stateless.Assert.That(page.Locator(\"#todo-list li\")).Count().IsInRange(5, 10)");
        }

        [Test]
        public async Task InnerText_Is_Property()
        {
            var page = EnsurePage();
            await Stateless.Assert.That(page.Locator("h1")).InnerText().Is("todos");
            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(page.Locator("h1")).InnerText().Is("wrong"));
            await Stateless.Assert.That(message).Contains("Expected \"todos\" to equal \"wrong\"");
            await Stateless.Assert.That(message).Contains("Stateless.Assert.That(page.Locator(\"h1\")).InnerText().Is(\"wrong\")");
        }

        [Test]
        public async Task InnerText_Contains_Property()
        {
            var page = EnsurePage();
            await Stateless.Assert.That(page.Locator("h1")).InnerText().Contains("tod");
            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(page.Locator("h1")).InnerText().Contains("missing"));
            await Stateless.Assert.That(message).Contains("Expected \"todos\" to contain \"missing\"");
            await Stateless.Assert.That(message).Contains("Stateless.Assert.That(page.Locator(\"h1\")).InnerText().Contains(\"missing\")");
        }

        [Test]
        public async Task InputValue_Is_Property()
        {
            var page = EnsurePage();
            await page.Locator("#todo-input").FillAsync("test value");
            await Stateless.Assert.That(page.Locator("#todo-input")).InputValue().Is("test value");
            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(page.Locator("#todo-input")).InputValue().Is("wrong"));
            await Stateless.Assert.That(message).Contains("Expected \"test value\" to equal \"wrong\"");
            await Stateless.Assert.That(message).Contains("Stateless.Assert.That(page.Locator(\"#todo-input\")).InputValue().Is(\"wrong\")");
        }

        [Test]
        public async Task InputValue_Contains_Property()
        {
            var page = EnsurePage();
            await page.Locator("#todo-input").FillAsync("test value");
            await Stateless.Assert.That(page.Locator("#todo-input")).InputValue().Contains("test");
            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(page.Locator("#todo-input")).InputValue().Contains("missing"));
            await Stateless.Assert.That(message).Contains("Expected \"test value\" to contain \"missing\"");
            await Stateless.Assert.That(message).Contains("Stateless.Assert.That(page.Locator(\"#todo-input\")).InputValue().Contains(\"missing\")");
        }

        [Test]
        public async Task TextContent_Is_Property()
        {
            var page = EnsurePage();
            await Stateless.Assert.That(page.Locator("h1")).TextContent().Is("todos");
            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(page.Locator("h1")).TextContent().Is("wrong"));
            await Stateless.Assert.That(message).Contains("Expected \"todos\" to equal \"wrong\"");
            await Stateless.Assert.That(message).Contains("Stateless.Assert.That(page.Locator(\"h1\")).TextContent().Is(\"wrong\")");
        }

        [Test]
        public async Task TextContent_Contains_Property()
        {
            var page = EnsurePage();
            await Stateless.Assert.That(page.Locator("h1")).TextContent().Contains("tod");
            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(page.Locator("h1")).TextContent().Contains("missing"));
            await Stateless.Assert.That(message).Contains("Expected \"todos\" to contain \"missing\"");
            await Stateless.Assert.That(message).Contains("Stateless.Assert.That(page.Locator(\"h1\")).TextContent().Contains(\"missing\")");
        }

        [Test]
        public async Task InnerHtml_Contains_Property()
        {
            var page = EnsurePage();
            // The h1 element contains text, and InnerHtml will include it
            await Stateless.Assert.That(page.Locator("h1")).InnerHtml().Contains("todos");
            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(page.Locator("h1")).InnerHtml().Contains("missing"));
            await Stateless.Assert.That(message).Contains("Expected \"todos\" to contain \"missing\"");
            await Stateless.Assert.That(message).Contains("Stateless.Assert.That(page.Locator(\"h1\")).InnerHtml().Contains(\"missing\")");
        }

        [Test]
        public async Task Chained_Count_Property()
        {
            var page = EnsurePage();
            // Test that property chains work correctly with configuration
            await Stateless.Assert.That(page.Locator("#todo-list li")).WithTimeout(5000).Count().Is(1);

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(page.Locator("#todo-list li")).WithTimeout(5000).Count().Is(2));
            await Stateless.Assert.That(message).Contains("Expected 2 to be 2, but was 1");
            await Stateless.Assert.That(message).Contains("Stateless.Assert.That(page.Locator(\"#todo-list li\")).WithTimeout(5000).Count().Is(2)");
        }
    }
}

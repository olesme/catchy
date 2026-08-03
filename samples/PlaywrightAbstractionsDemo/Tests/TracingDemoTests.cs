using Catchy;
using PlaywrightAbstractionsDemo.Support;

namespace PlaywrightAbstractionsDemo.Tests;

// Each test saves its trace to traces/<guid>.zip.
// Open with: npx playwright show-trace traces/<file>.zip
public sealed class TracingDemoTests : TracingTestBase
{
    [Test]
    public async Task Passing_assertions_appear_as_checkmarks_in_trace()
    {
        var page = EnsurePage();

        await Ambient.Assert.That(page.Locator("h1")).HasText("Catchy Demo");
        await Ambient.Assert.That(page.Locator("#search")).IsVisible();
        await Ambient.Assert.That(page.Locator("#submit")).IsEnabled();
    }

    [Test]
    public async Task Soft_failures_are_traced_at_accumulation_time()
    {
        var page = EnsurePage();

        // OnAssertion fires immediately when each soft assertion resolves —
        // the browser is still showing the failure state when TraceError is called.
        await Ambient.Assert.Soft.That(page.Locator("#result")).IsVisible();  // fails → ❌ in trace
        await Ambient.Assert.Soft.That(page.Locator("#search")).IsVisible();  // passes → ✔ in trace

        Ambient.Assert.Soft.Clear();
    }

    [Test]
    public async Task Mixed_hard_and_soft_assertions_all_appear_in_trace()
    {
        var page = EnsurePage();

        await Ambient.Assert.That(page.Locator("h1")).IsVisible();
        await Ambient.Assert.Soft.That(page.Locator("#search")).HasAttribute("placeholder", "Search...");
        await Ambient.Assert.Soft.That(page.Locator("#submit")).IsEnabled();
        await Ambient.Assert.That(page.Locator("#result")).IsHidden();
    }
}

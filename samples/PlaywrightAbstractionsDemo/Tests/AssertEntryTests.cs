using Catchy;
using CatchyTestHelpers;
using PlaywrightAbstractionsDemo.Assertions;
using PlaywrightAbstractionsDemo.PageAbstractions;
using static Catchy.StatelessAlias;

namespace PlaywrightAbstractionsDemo.Tests;

public class AssertEntryTests
{
    [Test]
    public async Task Button_IsEnabled_passes_when_enabled()
    {
        var btn = new ButtonElement(new("button#submit", IsEnabled: true));
        await Check.That(btn).IsEnabled();
    }

    [Test]
    public async Task Button_IsEnabled_fails_when_disabled()
    {
        var btn = new ButtonElement(new("button#submit", IsEnabled: false));
        var msg = await TestHelpers.ShouldFailWithMessageAsync(async () => await Check.That(btn).IsEnabled());
        await Check.That(msg).Contains("enabled");
    }

    [Test]
    public async Task Button_IsVisible_passes_when_visible()
    {
        var btn = new ButtonElement(new("button#ok", IsVisible: true));
        await Check.That(btn).IsVisible();
    }

    [Test]
    public async Task Button_IsHidden_passes_when_not_visible()
    {
        var btn = new ButtonElement(new("button#hidden", IsVisible: false));
        await Check.That(btn).IsHidden();
    }

    [Test]
    public async Task Text_HasText_passes_when_text_matches()
    {
        var txt = new TextElement(new("p.intro", Text: "Hello"));
        await Check.That(txt).HasText("Hello");
    }

    [Test]
    public async Task Text_HasText_fails_when_text_differs()
    {
        var txt = new TextElement(new("p.intro", Text: "Hello"));
        var msg = await TestHelpers.ShouldFailWithMessageAsync(async () => await Check.That(txt).HasText("World"));
        await Check.That(msg).Contains("World");
        await Check.That(msg).Contains("Hello");
    }

    [Test]
    public async Task Text_ContainsText_passes_when_substring_present()
    {
        var txt = new TextElement(new("p.intro", Text: "Hello World"));
        await Check.That(txt).ContainsText("World");
    }

    [Test]
    public async Task Checkbox_IsChecked_passes_when_checked()
    {
        var chk = new CheckboxElement(new("input#agree", IsChecked: true));
        await Check.That(chk).IsChecked();
    }

    [Test]
    public async Task Checkbox_IsUnchecked_passes_when_unchecked()
    {
        var chk = new CheckboxElement(new("input#agree", IsChecked: false));
        await Check.That(chk).IsUnchecked();
    }
}

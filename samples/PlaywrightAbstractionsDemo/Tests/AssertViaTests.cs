using CatchyTestHelpers;
using PlaywrightAbstractionsDemo.Assertions;
using PlaywrightAbstractionsDemo.PageAbstractions;
using static Catchy.StatelessAlias;

namespace PlaywrightAbstractionsDemo.Tests;

public class AssertViaTests
{
    [Test]
    public async Task TextField_IsVisible_passes_when_visible()
    {
        var field = new TextFieldWidget(new("input#name", IsVisible: true));
        await Check.That(field).IsVisible();
    }

    [Test]
    public async Task TextField_IsEnabled_passes_when_enabled()
    {
        var field = new TextFieldWidget(new("input#name", IsEnabled: true));
        await Check.That(field).IsEnabled();
    }

    [Test]
    public async Task TextField_HasText_passes_when_value_matches()
    {
        var field = new TextFieldWidget(new("input#name", Text: "John"));
        await Check.That(field).HasText("John");
    }

    [Test]
    public async Task TextField_HasText_fails_when_value_differs()
    {
        var field = new TextFieldWidget(new("input#name", Text: "John"));
        var msg = await TestHelpers.ShouldFailWithMessageAsync(async () => await Check.That(field).HasText("Jane"));
        await TUnit.Assertions.Assert.That(msg).Contains("Jane");
        await TUnit.Assertions.Assert.That(msg).Contains("John");
    }

    [Test]
    public async Task Dropdown_IsVisible_passes_when_visible()
    {
        var drop = new DropdownWidget(new("select#country", IsVisible: true));
        await Check.That(drop).IsVisible();
    }

    [Test]
    public async Task Dropdown_IsEnabled_passes_when_enabled()
    {
        var drop = new DropdownWidget(new("select#country", IsEnabled: true));
        await Check.That(drop).IsEnabled();
    }
}

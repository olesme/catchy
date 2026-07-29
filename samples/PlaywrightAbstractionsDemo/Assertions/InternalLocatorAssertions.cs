// Assertion extensions for the InternalLocator type — used by the AssertVia variant.
// These extensions are available on ValueAssertions<InternalLocator>, which is what
// Asserter.That(UiWidget widget) returns after extracting widget.Locator.

using Catchy;
using Catchy.Sdk;
using PlaywrightAbstractionsDemo.PageAbstractions;

namespace PlaywrightAbstractionsDemo.Assertions;

public static class InternalLocatorAssertions
{
    public static ValueAssertions<InternalLocator> IsVisible(this ValueAssertions<InternalLocator> a)
    {
        a.Link("IsVisible");
        a.Op(_ => CheckOperation.Sync(
            passes:    () => a.GetValue().IsVisible,
            failBody:  () => "Expected locator to be visible.",
            isSkipped: a.IsSkipped()));
        return a;
    }

    public static ValueAssertions<InternalLocator> IsEnabled(this ValueAssertions<InternalLocator> a)
    {
        a.Link("IsEnabled");
        a.Op(_ => CheckOperation.Sync(
            passes:    () => a.GetValue().IsEnabled,
            failBody:  () => "Expected locator to be enabled.",
            isSkipped: a.IsSkipped()));
        return a;
    }

    public static ValueAssertions<InternalLocator> HasText(this ValueAssertions<InternalLocator> a, string expected)
    {
        a.Link("HasText", expected);
        a.Op(_ => CheckOperation.Sync(
            passes:    () => string.Equals(a.GetValue().Text, expected, StringComparison.Ordinal),
            failBody:  () => "Expected text \"" + expected + "\" but found \"" + a.GetValue().Text + "\".",
            isSkipped: a.IsSkipped()));
        return a;
    }

    public static ValueAssertions<InternalLocator> IsChecked(this ValueAssertions<InternalLocator> a)
    {
        a.Link("IsChecked");
        a.Op(_ => CheckOperation.Sync(
            passes:    () => a.GetValue().IsChecked,
            failBody:  () => "Expected locator to be checked.",
            isSkipped: a.IsSkipped()));
        return a;
    }
}

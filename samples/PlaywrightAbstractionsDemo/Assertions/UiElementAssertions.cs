// Custom assertion extensions for the AssertEntry variant.

using Catchy;
using Catchy.Sdk;
using PlaywrightAbstractionsDemo.PageAbstractions;

namespace PlaywrightAbstractionsDemo.Assertions;

public static class UiElementAssertions
{
    public static ValueAssertions<T> IsVisible<T>(this ValueAssertions<T> a)
        where T : UiElement, IHasVisibility
    {
        a.Link("IsVisible");
        a.Op(_ => CheckOperation.Sync(
            passes:    () => a.GetValue().Locator.IsVisible,
            failBody:  () => "Expected element to be visible.",
            isSkipped: a.IsSkipped()));
        return a;
    }

    public static ValueAssertions<T> IsHidden<T>(this ValueAssertions<T> a)
        where T : UiElement, IHasVisibility
    {
        a.Link("IsHidden");
        a.Op(_ => CheckOperation.Sync(
            passes:    () => !a.GetValue().Locator.IsVisible,
            failBody:  () => "Expected element to be hidden.",
            isSkipped: a.IsSkipped()));
        return a;
    }

    public static ValueAssertions<T> IsEnabled<T>(this ValueAssertions<T> a)
        where T : UiElement, IClickable
    {
        a.Link("IsEnabled");
        a.Op(_ => CheckOperation.Sync(
            passes:    () => a.GetValue().Locator.IsEnabled,
            failBody:  () => "Expected element to be enabled.",
            isSkipped: a.IsSkipped()));
        return a;
    }

    public static ValueAssertions<T> IsDisabled<T>(this ValueAssertions<T> a)
        where T : UiElement, IClickable
    {
        a.Link("IsDisabled");
        a.Op(_ => CheckOperation.Sync(
            passes:    () => !a.GetValue().Locator.IsEnabled,
            failBody:  () => "Expected element to be disabled.",
            isSkipped: a.IsSkipped()));
        return a;
    }

    public static ValueAssertions<T> HasText<T>(this ValueAssertions<T> a, string expected)
        where T : UiElement, IHasText
    {
        a.Link("HasText", expected);
        a.Op(_ => CheckOperation.Sync(
            passes:    () => string.Equals(a.GetValue().Locator.Text, expected, StringComparison.Ordinal),
            failBody:  () => "Expected text \"" + expected + "\" but found \"" + a.GetValue().Locator.Text + "\".",
            isSkipped: a.IsSkipped()));
        return a;
    }

    public static ValueAssertions<T> ContainsText<T>(this ValueAssertions<T> a, string substring)
        where T : UiElement, IHasText
    {
        a.Link("ContainsText", substring);
        a.Op(_ => CheckOperation.Sync(
            passes:    () => a.GetValue().Locator.Text.Contains(substring, StringComparison.Ordinal),
            failBody:  () => "Expected text to contain \"" + substring + "\" but found \"" + a.GetValue().Locator.Text + "\".",
            isSkipped: a.IsSkipped()));
        return a;
    }

    public static ValueAssertions<T> IsChecked<T>(this ValueAssertions<T> a)
        where T : UiElement, ICheckable
    {
        a.Link("IsChecked");
        a.Op(_ => CheckOperation.Sync(
            passes:    () => a.GetValue().Locator.IsChecked,
            failBody:  () => "Expected element to be checked.",
            isSkipped: a.IsSkipped()));
        return a;
    }

    public static ValueAssertions<T> IsUnchecked<T>(this ValueAssertions<T> a)
        where T : UiElement, ICheckable
    {
        a.Link("IsUnchecked");
        a.Op(_ => CheckOperation.Sync(
            passes:    () => !a.GetValue().Locator.IsChecked,
            failBody:  () => "Expected element to be unchecked.",
            isSkipped: a.IsSkipped()));
        return a;
    }
}

// Demo: internal element representation that wraps a Playwright-like "locator" concept.
// In real code, InternalLocator would be ILocator from Microsoft.Playwright.
// Here we use a plain record so the demo compiles without an actual browser.

namespace PlaywrightAbstractionsDemo.PageAbstractions;

/// <summary>
/// Simulates the internal Playwright <c>ILocator</c>-like concept.
/// In your real interaction layer this would be <c>Microsoft.Playwright.ILocator</c>.
/// The test layer never sees this type directly.
/// </summary>
public sealed record InternalLocator(
    string Selector,
    bool IsVisible   = true,
    bool IsEnabled   = true,
    bool IsChecked   = false,
    string Text      = "");

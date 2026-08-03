using Microsoft.Playwright;

namespace PlaywrightAbstractionsDemo.Support;

public static class BrowserSession
{
    private static IPlaywright? _playwright;
    private static IBrowser? _browser;

    public static IBrowser Browser =>
        _browser ?? throw new InvalidOperationException("Browser not initialized — session hook did not run.");

    [Before(HookType.TestSession)]
    public static async Task InitAsync()
    {
        Microsoft.Playwright.Program.Main(["install", "chromium"]);
        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new() { Headless = true });
    }

    [After(HookType.TestSession)]
    public static async Task TeardownAsync()
    {
        if (_browser is not null) await _browser.CloseAsync();
        _playwright?.Dispose();
    }
}

using Microsoft.Playwright;

namespace CatchyPlaywrightTests.Support
{
    public static class PlaywrightBrowserSession
    {
        private static IPlaywright? _playwright;
        private static IBrowser? _browser;

        public static IBrowser Browser =>
            _browser ?? throw new InvalidOperationException("Browser not initialized — session hook didn't run.");

        [Before(HookType.TestSession)]
        public static async Task InitAsync()
        {
            await PlaywrightInstaller.EnsureInstalledAsync(); // якщо потрібно

            _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
                Args = ["--disable-dev-shm-usage", "--no-sandbox", "--disable-gpu", "--disable-sync"]
            });
        }

        [After(HookType.TestSession)]
        public static async Task TeardownAsync()
        {
            if (_browser is not null) await _browser.CloseAsync();
            _playwright?.Dispose();
        }
    }
}

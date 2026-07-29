using Microsoft.Playwright;

namespace CatchyPlaywrightTests.Support
{
    public abstract class PlaywrightTestFixture
    {
        protected IBrowserContext? Context { get; private set; }
        protected IPage? Page { get; private set; }
        protected virtual string InitialPageHtml => PlaywrightTestPages.BasicTodoPageHtml;

        [Before(HookType.Test)]
        public async Task SetUpTestAsync()
        {
            Context = await PlaywrightBrowserSession.Browser.NewContextAsync();
            Page = await Context.NewPageAsync();
            await Page.SetContentAsync(InitialPageHtml);
        }

        [After(HookType.Test)]
        public async Task TearDownTestAsync()
        {
            if (Page is not null) { try { await Page.CloseAsync(); } catch { } }
            if (Context is not null) { try { await Context.CloseAsync(); } catch { } }
            Page = null;
            Context = null;
        }

        protected Task UseDynamicPageAsync()
            => EnsurePage().SetContentAsync(PlaywrightTestPages.DynamicBehaviorPageHtml);

        protected IPage EnsurePage()
            => Page ?? throw new InvalidOperationException("Page is not initialized for the current test.");
    }
}

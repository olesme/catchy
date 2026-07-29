using Microsoft.Playwright;

namespace Catchy.Sdk
{
    public static class PwBrowserContextChecks
    {
        public static CheckOperation HasCookie(IBrowserContext ctx, string name, string? domain,
            bool not, Func<StringComparison> cmp, Func<float?> timeoutMsGetter, bool isSkipped)
            => CheckOperation.Async(async () =>
                await PwPolling.PollUntilAsync(async () =>
                {
                    var cookies = await ctx.CookiesAsync().ConfigureAwait(false);
                    var comparison = cmp();
                    bool ok = cookies.Any(c =>
                        string.Equals(c.Name, name, comparison) &&
                        (domain is null || c.Domain.Contains(domain, comparison)));
                    return not ? !ok : ok;
                }, timeoutMsGetter).ConfigureAwait(false),
            () => not ? $"Expected context not to have cookie \"{name}\""
                      : $"Expected context to have cookie \"{name}\"",
            isSkipped);

        public static CheckOperation HasCookieValue(IBrowserContext ctx, string name, string expected,
            bool not, Func<StringComparison> cmp, Func<float?> timeoutMsGetter, bool isSkipped)
        {
            string? actual = null;
            return CheckOperation.Async(async () =>
                await PwPolling.PollUntilAsync(async () =>
                {
                    var cookies = await ctx.CookiesAsync().ConfigureAwait(false);
                    var cookie = cookies.FirstOrDefault(c => c.Name == name);
                    actual = cookie?.Value;
                    bool eq = cookie is not null && string.Equals(actual, expected, cmp());
                    return not ? !eq : eq;
                }, timeoutMsGetter).ConfigureAwait(false),
            () => not ? $"Expected cookie \"{name}\" not to equal \"{expected}\", but was \"{actual}\""
                      : $"Expected cookie \"{name}\" = \"{expected}\", but was \"{actual ?? "not found"}\"",
            isSkipped);
        }

        public static CheckOperation HasCookies(IBrowserContext ctx, bool not,
            Func<float?> timeoutMsGetter, bool isSkipped)
        {
            int count = 0;
            return CheckOperation.Async(async () =>
                await PwPolling.PollUntilAsync(async () =>
                {
                    var cookies = await ctx.CookiesAsync().ConfigureAwait(false);
                    count = cookies.Count;
                    return not ? count == 0 : count > 0;
                }, timeoutMsGetter).ConfigureAwait(false),
            () => not ? $"Expected no cookies, but found {count}"
                      : "Expected context to have cookies, but had none",
            isSkipped);
        }

        public static CheckOperation PageCountIs(IBrowserContext ctx, int count, bool not,
            Func<float?> timeoutMsGetter, bool isSkipped)
            => CheckOperation.Async(async () =>
                await PwPolling.PollUntilAsync(() =>
                    Task.FromResult(not ? ctx.Pages.Count != count : ctx.Pages.Count == count),
                timeoutMsGetter).ConfigureAwait(false),
            () => not ? $"Expected page count not to be {count}, but was {ctx.Pages.Count}"
                      : $"Expected page count = {count}, but was {ctx.Pages.Count}",
            isSkipped);

        public static CheckOperation HasOpenPages(IBrowserContext ctx, bool not,
            Func<float?> timeoutMsGetter, bool isSkipped)
            => CheckOperation.Async(async () =>
                await PwPolling.PollUntilAsync(() =>
                {
                    bool has = ctx.Pages.Count > 0;
                    return Task.FromResult(not ? !has : has);
                }, timeoutMsGetter).ConfigureAwait(false),
            () => not ? $"Expected context to have no open pages, but had {ctx.Pages.Count}"
                      : "Expected context to have open pages, but had none",
            isSkipped);
    }
}

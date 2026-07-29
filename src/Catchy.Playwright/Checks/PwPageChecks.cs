using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using PW = Microsoft.Playwright.Assertions;

namespace Catchy.Sdk
{
    public static class PwPageChecks
    {
        // Mapping

        public static readonly RegexOptions[] RegexOptionsMap =
        [
            RegexOptions.None,
            RegexOptions.IgnoreCase,
            RegexOptions.CultureInvariant,
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase,
            RegexOptions.CultureInvariant,
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase,
        ];

        public static RegexOptions ToRegexOptions(StringComparison c) => RegexOptionsMap[(int)c];

        // Title

        public static CheckOperation HasTitle(IPage page, string expected, bool not,
            Func<StringComparison> cmp, Func<float?> timeoutMs, bool isSkipped)
        {
            string? actual = null;
            return CheckOperation.Async(async () =>
                await PwPolling.PollUntilAsync(async () =>
                {
                    actual = await page.TitleAsync().ConfigureAwait(false);
                    var ok = string.Equals(actual, expected, cmp());
                    return not ? !ok : ok;
                }, timeoutMs).ConfigureAwait(false),
            () => not
                ? $"Expected page title not to equal \"{expected}\", but was \"{actual}\""
                : $"Expected page to have title \"{expected}\", but was \"{actual ?? "null"}\"",
            isSkipped);
        }

        public static CheckOperation HasTitle(IPage page, Regex pattern, bool not,
            Func<float?> timeoutMs, bool isSkipped)
        {
            var o = new PageAssertionsToHaveTitleOptions { Timeout = timeoutMs() };
            return PwLocatorChecks.PwOp(isSkipped,
                () => not ? PW.Expect(page).Not.ToHaveTitleAsync(pattern, o)
                          : PW.Expect(page).ToHaveTitleAsync(pattern, o),
                not ? $"Expected page title not to match /{pattern}/"
                    : $"Expected page title to match /{pattern}/");
        }

        // URL

        public static CheckOperation HasUrl(IPage page, string expected, bool not,
            Func<StringComparison> cmp, Func<float?> timeoutMs, bool isSkipped)
        {
            var regexOpts = ToRegexOptions(cmp());
            if ((regexOpts & RegexOptions.CultureInvariant) != 0)
                return HasUrl(page, new Regex(Regex.Escape(expected), regexOpts), not, timeoutMs, isSkipped);

            var o = new PageAssertionsToHaveURLOptions
            {
                Timeout = timeoutMs(),
                IgnoreCase = (regexOpts & RegexOptions.IgnoreCase) != 0,
            };
            return PwLocatorChecks.PwOp(isSkipped,
                () => not ? PW.Expect(page).Not.ToHaveURLAsync(expected, o)
                          : PW.Expect(page).ToHaveURLAsync(expected, o),
                not ? $"Expected page not to have URL \"{expected}\""
                    : $"Expected page to have URL \"{expected}\"");
        }

        public static CheckOperation HasUrl(IPage page, Regex pattern, bool not,
            Func<float?> timeoutMs, bool isSkipped)
        {
            var o = new PageAssertionsToHaveURLOptions { Timeout = timeoutMs() };
            return PwLocatorChecks.PwOp(isSkipped,
                () => not ? PW.Expect(page).Not.ToHaveURLAsync(pattern, o)
                          : PW.Expect(page).ToHaveURLAsync(pattern, o),
                not ? $"Expected page URL not to match /{pattern}/"
                    : $"Expected page URL to match /{pattern}/");
        }

        // Aria snapshot

        public static CheckOperation MatchesAriaSnapshot(IPage page, string template, bool not,
            Func<float?> timeoutMs, bool isSkipped)
        {
            var o = new LocatorAssertionsToMatchAriaSnapshotOptions { Timeout = timeoutMs() };
            var loc = page.Locator("html");
            return PwLocatorChecks.PwOp(isSkipped,
                () => not ? PW.Expect(loc).Not.ToMatchAriaSnapshotAsync(template, o)
                          : PW.Expect(loc).ToMatchAriaSnapshotAsync(template, o),
                not ? "Expected page not to match aria snapshot"
                    : "Expected page to match aria snapshot");
        }

        // Viewport

        public static CheckOperation HasViewportSize(IPage page, int width, int height,
            bool not, bool isSkipped)
            => CheckOperation.Sync(
                () => { var vp = page.ViewportSize; bool ok = vp is not null && vp.Width == width && vp.Height == height; return not ? !ok : ok; },
                () => not ? $"Expected viewport not to be {width}×{height}"
                          : $"Expected viewport {width}×{height}, but was {page.ViewportSize?.Width}×{page.ViewportSize?.Height}",
                isSkipped);

        // Title contains (polled)

        public static CheckOperation TitleContains(IPage page, string substring, bool not,
            Func<StringComparison> cmp, Func<float?> timeoutMs, bool isSkipped)
        {
            string? actual = null;
            return CheckOperation.Async(async () =>
                await PwPolling.PollUntilAsync(async () =>
                {
                    actual = await page.TitleAsync().ConfigureAwait(false);
                    bool ok = actual.Contains(substring, cmp());
                    return not ? !ok : ok;
                }, timeoutMs).ConfigureAwait(false),
            () => not ? $"Expected page title not to contain \"{substring}\", but was \"{actual}\""
                      : $"Expected page title to contain \"{substring}\", but was \"{actual ?? "null"}\"",
            isSkipped);
        }

        // URL contains (polled)

        public static CheckOperation UrlContains(IPage page, string substring, bool not,
            Func<StringComparison> cmp, Func<float?> timeoutMs, bool isSkipped)
            => CheckOperation.Async(async () =>
                await PwPolling.PollUntilAsync(() =>
                    Task.FromResult(not
                        ? !page.Url.Contains(substring, cmp())
                        : page.Url.Contains(substring, cmp())),
                timeoutMs).ConfigureAwait(false),
            () => not ? $"Expected URL not to contain \"{substring}\", but was \"{page.Url}\""
                      : $"Expected URL to contain \"{substring}\", but was \"{page.Url}\"",
            isSkipped);

        // localStorage (polled)

        public static CheckOperation HasLocalStorageKey(IPage page, string key,
            bool not, Func<float?> timeoutMs, bool isSkipped)
        {
            string? val = null;
            return CheckOperation.Async(async () =>
                await PwPolling.PollUntilAsync(async () =>
                {
                    val = await page.EvaluateAsync<string?>("k => window.localStorage.getItem(k)", key).ConfigureAwait(false);
                    return not ? val is null : val is not null;
                }, timeoutMs).ConfigureAwait(false),
            () => not ? $"Expected localStorage not to have key \"{key}\""
                      : $"Expected localStorage to have key \"{key}\"",
            isSkipped);
        }

        public static CheckOperation HasLocalStorageValue(IPage page, string key, string expectedValue,
            bool not, Func<StringComparison> cmp, Func<float?> timeoutMs, bool isSkipped)
        {
            string? actual = null;
            return CheckOperation.Async(async () =>
                await PwPolling.PollUntilAsync(async () =>
                {
                    actual = await page.EvaluateAsync<string?>("k => window.localStorage.getItem(k)", key).ConfigureAwait(false);
                    bool eq = string.Equals(actual, expectedValue, cmp());
                    return not ? !eq : eq;
                }, timeoutMs).ConfigureAwait(false),
            () => not ? $"Expected localStorage[\"{key}\"] not to equal \"{expectedValue}\", but was \"{actual}\""
                      : $"Expected localStorage[\"{key}\"] = \"{expectedValue}\", but was \"{actual ?? "null"}\"",
            isSkipped);
        }

        public static CheckOperation LocalStorageIsEmpty(IPage page, bool not,
            Func<float?> timeoutMs, bool isSkipped)
        {
            int count = 0;
            return CheckOperation.Async(async () =>
                await PwPolling.PollUntilAsync(async () =>
                {
                    count = await page.EvaluateAsync<int>("() => window.localStorage.length").ConfigureAwait(false);
                    return not ? count > 0 : count == 0;
                }, timeoutMs).ConfigureAwait(false),
            () => not ? "Expected localStorage not to be empty"
                      : $"Expected localStorage to be empty, but had {count} key(s)",
            isSkipped);
        }

        // sessionStorage (polled)

        /// <summary>Checks that a key exists in sessionStorage (value may be anything).</summary>
        public static CheckOperation HasSessionStorageKey(IPage page, string key,
            bool not, Func<float?> timeoutMs, bool isSkipped)
        {
            string? val = null;
            return CheckOperation.Async(async () =>
                await PwPolling.PollUntilAsync(async () =>
                {
                    val = await page.EvaluateAsync<string?>("k => window.sessionStorage.getItem(k)", key).ConfigureAwait(false);
                    return not ? val is null : val is not null;
                }, timeoutMs).ConfigureAwait(false),
            () => not ? $"Expected sessionStorage not to have key \"{key}\""
                      : $"Expected sessionStorage to have key \"{key}\"",
            isSkipped);
        }

        public static CheckOperation HasSessionStorageValue(IPage page, string key, string expectedValue,
            bool not, Func<StringComparison> cmp, Func<float?> timeoutMs, bool isSkipped)
        {
            string? actual = null;
            return CheckOperation.Async(async () =>
                await PwPolling.PollUntilAsync(async () =>
                {
                    actual = await page.EvaluateAsync<string?>("k => window.sessionStorage.getItem(k)", key).ConfigureAwait(false);
                    bool eq = string.Equals(actual, expectedValue, cmp());
                    return not ? !eq : eq;
                }, timeoutMs).ConfigureAwait(false),
            () => not ? $"Expected sessionStorage[\"{key}\"] not to equal \"{expectedValue}\", but was \"{actual}\""
                      : $"Expected sessionStorage[\"{key}\"] = \"{expectedValue}\", but was \"{actual ?? "null"}\"",
            isSkipped);
        }

        // Cookies (polled)

        public static CheckOperation HasCookie(IPage page, string name, string? domain,
            bool not, Func<StringComparison> cmp, Func<float?> timeoutMs, bool isSkipped)
            => CheckOperation.Async(async () =>
                await PwPolling.PollUntilAsync(async () =>
                {
                    var cookies = await page.Context.CookiesAsync().ConfigureAwait(false);
                    var comparison = cmp();
                    bool ok = cookies.Any(c =>
                        string.Equals(c.Name, name, comparison) &&
                        (domain is null || c.Domain.Contains(domain, comparison)));
                    return not ? !ok : ok;
                }, timeoutMs).ConfigureAwait(false),
            () => not ? $"Expected page not to have cookie \"{name}\""
                      : $"Expected page to have cookie \"{name}\"",
            isSkipped);

        /// <summary>Checks that a cookie exists <em>and</em> its value matches.</summary>
        public static CheckOperation HasCookieValue(IPage page, string name, string expectedValue,
            bool not, Func<StringComparison> cmp, Func<float?> timeoutMs, bool isSkipped)
        {
            string? actual = null;
            return CheckOperation.Async(async () =>
                await PwPolling.PollUntilAsync(async () =>
                {
                    var cookies = await page.Context.CookiesAsync().ConfigureAwait(false);
                    var cookie = cookies.FirstOrDefault(c => c.Name == name);
                    actual = cookie?.Value;
                    bool eq = cookie is not null && string.Equals(actual, expectedValue, cmp());
                    return not ? !eq : eq;
                }, timeoutMs).ConfigureAwait(false),
            () => not ? $"Expected cookie \"{name}\" not to equal \"{expectedValue}\", but was \"{actual}\""
                      : $"Expected cookie \"{name}\" = \"{expectedValue}\", but was \"{actual ?? "not found"}\"",
            isSkipped);
        }

        // Frames (polled)

        public static CheckOperation HasFrame(IPage page, string name,
            bool not, Func<float?> timeoutMs, bool isSkipped)
            => CheckOperation.Async(async () =>
                await PwPolling.PollUntilAsync(() =>
                    Task.FromResult(not
                        ? page.Frame(name) is null
                        : page.Frame(name) is not null),
                timeoutMs).ConfigureAwait(false),
            () => not ? $"Expected page not to have frame \"{name}\""
                      : $"Expected page to have frame \"{name}\"",
            isSkipped);

        // Meta tag (polled)

        public static CheckOperation HasMetaTag(IPage page, string name, string content,
            bool not, Func<StringComparison> cmp, Func<float?> timeoutMs, bool isSkipped)
        {
            string? actual = null;
            return CheckOperation.Async(async () =>
                await PwPolling.PollUntilAsync(async () =>
                {
                    actual = await page.EvaluateAsync<string?>(
                        "n => { const el = document.querySelector(`meta[name='${n}']`); return el ? el.getAttribute('content') : null; }",
                        name).ConfigureAwait(false);
                    bool ok = string.Equals(actual, content, cmp());
                    return not ? !ok : ok;
                }, timeoutMs).ConfigureAwait(false),
            () => not ? $"Expected meta[\"{name}\"] content not to be \"{content}\", but was \"{actual}\""
                      : $"Expected meta[\"{name}\"] content = \"{content}\", but was \"{actual ?? "not found"}\"",
            isSkipped);
        }

        // JSON path presence (polled)

        /// <summary>
        /// Checks that a dot-separated JSON path exists in the page-level JS object
        /// returned by <paramref name="jsExpression"/> (defaults to the full response body
        /// if the page exposes it, but more commonly used after an <c>EvaluateAsync</c> call).
        /// For API response JSON path checks use <see cref="PwApiResponseChecks"/>.
        /// </summary>
        public static CheckOperation HasJsonPath(IPage page, string jsExpression, string path,
            bool not, Func<float?> timeoutMs, bool isSkipped)
        {
            bool found = false;
            return CheckOperation.Async(async () =>
                await PwPolling.PollUntilAsync(async () =>
                {
                    try
                    {
                        var json = await page.EvaluateAsync<string>(
                            $"() => JSON.stringify({jsExpression})").ConfigureAwait(false);
                        using var doc = JsonDocument.Parse(json ?? "null");
                        found = TryGetPath(doc.RootElement, path, out _);
                    }
                    catch { found = false; }
                    return not ? !found : found;
                }, timeoutMs).ConfigureAwait(false),
            () => not ? $"Expected JSON path \"{path}\" not to exist"
                      : $"Expected JSON path \"{path}\" to exist",
            isSkipped);
        }

        public static bool TryGetPath(JsonElement root, string path, out JsonElement result)
        {
            result = root;
            foreach (var seg in path.Split('.'))
                if (result.ValueKind != JsonValueKind.Object || !result.TryGetProperty(seg, out result))
                    return false;
            return true;
        }
    }
}

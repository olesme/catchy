using Microsoft.Playwright;

namespace Catchy.Sdk
{
    public static class PwFrameChecks
    {
        public static CheckOperation HasUrl(IFrame frame, string url, bool not,
            Func<StringComparison> cmp, Func<float?> timeoutMsGetter, bool isSkipped)
            => CheckOperation.Async(async () =>
                await PwPolling.PollUntilAsync(() =>
                {
                    bool eq = string.Equals(frame.Url, url, cmp());
                    return Task.FromResult(not ? !eq : eq);
                }, timeoutMsGetter).ConfigureAwait(false),
            () => not ? $"Expected frame URL not to be \"{url}\", but was \"{frame.Url}\""
                      : $"Expected frame URL = \"{url}\", but was \"{frame.Url}\"",
            isSkipped);

        public static CheckOperation UrlContains(IFrame frame, string substring, bool not,
            Func<StringComparison> cmp, Func<float?> timeoutMsGetter, bool isSkipped)
            => CheckOperation.Async(async () =>
                await PwPolling.PollUntilAsync(() =>
                {
                    bool ok = frame.Url.Contains(substring, cmp());
                    return Task.FromResult(not ? !ok : ok);
                }, timeoutMsGetter).ConfigureAwait(false),
            () => not ? $"Expected frame URL not to contain \"{substring}\", but was \"{frame.Url}\""
                      : $"Expected frame URL to contain \"{substring}\", but was \"{frame.Url}\"",
            isSkipped);

        public static CheckOperation HasName(IFrame frame, string name, bool not,
            Func<StringComparison> cmp, Func<float?> timeoutMsGetter, bool isSkipped)
            => CheckOperation.Async(async () =>
                await PwPolling.PollUntilAsync(() =>
                {
                    bool eq = string.Equals(frame.Name, name, cmp());
                    return Task.FromResult(not ? !eq : eq);
                }, timeoutMsGetter).ConfigureAwait(false),
            () => not ? $"Expected frame name not to be \"{name}\", but was \"{frame.Name}\""
                      : $"Expected frame name = \"{name}\", but was \"{frame.Name}\"",
            isSkipped);

        public static CheckOperation IsDetached(IFrame frame, bool not, bool isSkipped)
            => CheckOperation.Sync(
                () => not ? !frame.IsDetached : frame.IsDetached,
                () => not ? "Expected frame not to be detached" : "Expected frame to be detached",
                isSkipped);

        public static CheckOperation HasTitle(IFrame frame, string? expected, bool not,
            Func<StringComparison> cmp, Func<float?> timeoutMsGetter, bool isSkipped)
        {
            string? actual = null;
            return CheckOperation.Async(async () =>
                await PwPolling.PollUntilAsync(async () =>
                {
                    actual = await frame.TitleAsync().ConfigureAwait(false);
                    if (expected is null) return not ? actual is null : actual is not null;
                    bool eq = string.Equals(actual, expected, cmp());
                    return not ? !eq : eq;
                }, timeoutMsGetter).ConfigureAwait(false),
            () => not ? $"Expected frame title not to be \"{expected}\", but was \"{actual}\""
                      : $"Expected frame title = \"{expected}\", but was \"{actual ?? "null"}\"",
            isSkipped);
        }

        public static CheckOperation ChildFrameCountIs(IFrame frame, int count, bool not,
            Func<float?> timeoutMsGetter, bool isSkipped)
            => CheckOperation.Async(async () =>
                await PwPolling.PollUntilAsync(() =>
                {
                    int actual = frame.ChildFrames.Count;
                    return Task.FromResult(not ? actual != count : actual == count);
                }, timeoutMsGetter).ConfigureAwait(false),
            () => not ? $"Expected child frame count not to be {count}, but was {frame.ChildFrames.Count}"
                      : $"Expected child frame count = {count}, but was {frame.ChildFrames.Count}",
            isSkipped);
    }
}

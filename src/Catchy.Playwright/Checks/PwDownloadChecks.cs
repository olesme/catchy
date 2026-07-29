using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace Catchy.Sdk
{
    public static class PwDownloadChecks
    {
        public static CheckOperation HasSuggestedFilename(IDownload download, string filename,
            bool not, Func<StringComparison> cmp, bool isSkipped)
        {
            var actual = download.SuggestedFilename;
            return CheckOperation.Sync(
                () => { bool eq = string.Equals(actual, filename, cmp()); return not ? !eq : eq; },
                () => not ? $"Expected filename not to be \"{filename}\", but was \"{actual}\""
                          : $"Expected filename = \"{filename}\", but was \"{actual}\"",
                isSkipped);
        }

        public static CheckOperation FilenameMatches(IDownload download, Regex pattern, bool not, bool isSkipped)
        {
            var actual = download.SuggestedFilename;
            return CheckOperation.Sync(
                () => { bool ok = pattern.IsMatch(actual); return not ? !ok : ok; },
                () => not ? $"Expected filename not to match /{pattern}/, but was \"{actual}\""
                          : $"Expected filename to match /{pattern}/, but was \"{actual}\"",
                isSkipped);
        }

        public static CheckOperation HasExtension(IDownload download, string extension,
            bool not, Func<StringComparison> cmp, bool isSkipped)
        {
            var actual = Path.GetExtension(download.SuggestedFilename);
            return CheckOperation.Sync(
                () => { bool eq = string.Equals(actual, extension, cmp()); return not ? !eq : eq; },
                () => not ? $"Expected extension not to be \"{extension}\", but was \"{actual}\""
                          : $"Expected extension = \"{extension}\", but was \"{actual}\"",
                isSkipped);
        }

        public static CheckOperation FileSizeGreaterThan(IDownload download, long bytes, bool not, bool isSkipped)
        {
            long actual = 0;
            return CheckOperation.Async(async () =>
            {
                var path = await download.PathAsync().ConfigureAwait(false);
                if (path is null) return not;
                actual = new FileInfo(path).Length;
                return not ? actual <= bytes : actual > bytes;
            },
            () => not ? $"Expected file size not to be > {bytes} bytes, but was {actual}"
                      : $"Expected file size > {bytes} bytes, but was {actual}",
            isSkipped);
        }

        public static CheckOperation FileSizeLessThan(IDownload download, long bytes, bool not, bool isSkipped)
        {
            long actual = 0;
            return CheckOperation.Async(async () =>
            {
                var path = await download.PathAsync().ConfigureAwait(false);
                if (path is null) return not;
                actual = new FileInfo(path).Length;
                return not ? actual >= bytes : actual < bytes;
            },
            () => not ? $"Expected file size not to be < {bytes} bytes, but was {actual}"
                      : $"Expected file size < {bytes} bytes, but was {actual}",
            isSkipped);
        }

        public static CheckOperation FileSizeInRange(IDownload download, long min, long max, bool not, bool isSkipped)
        {
            long actual = 0;
            return CheckOperation.Async(async () =>
            {
                var path = await download.PathAsync().ConfigureAwait(false);
                if (path is null) return not;
                actual = new FileInfo(path).Length;
                bool ok = actual >= min && actual <= max;
                return not ? !ok : ok;
            },
            () => not ? $"Expected file size not to be in [{min}, {max}] bytes, but was {actual}"
                      : $"Expected file size in [{min}, {max}] bytes, but was {actual}",
            isSkipped);
        }

        public static CheckOperation ContentContains(IDownload download, string substring,
            bool not, Func<StringComparison> cmp, bool isSkipped)
            => CheckOperation.Async(async () =>
            {
                var path = await download.PathAsync().ConfigureAwait(false);
                if (path is null) return not;
                var text = await FileHelper.ReadAllTextAsync(path).ConfigureAwait(false);
                bool ok = text.Contains(substring, cmp());
                return not ? !ok : ok;
            },
            () => not ? $"Expected file content not to contain \"{substring}\""
                      : $"Expected file content to contain \"{substring}\"",
            isSkipped);

        public static CheckOperation ContentMatches(IDownload download, Regex pattern, bool not, bool isSkipped)
            => CheckOperation.Async(async () =>
            {
                var path = await download.PathAsync().ConfigureAwait(false);
                if (path is null) return not;
                var text = await FileHelper.ReadAllTextAsync(path).ConfigureAwait(false);
                bool ok = pattern.IsMatch(text);
                return not ? !ok : ok;
            },
            () => not ? $"Expected file content not to match /{pattern}/"
                      : $"Expected file content to match /{pattern}/",
            isSkipped);

        public static CheckOperation IsSuccessful(IDownload download, bool not, bool isSkipped)
        {
            string? failure = null;
            return CheckOperation.Async(async () =>
            {
                failure = await download.FailureAsync().ConfigureAwait(false);
                return not ? failure is not null : failure is null;
            },
            () => not ? "Expected download to fail, but it succeeded"
                      : $"Expected download to succeed, but failed with: {failure}",
            isSkipped);
        }

        public static CheckOperation HasUrl(IDownload download, string url,
            bool not, Func<StringComparison> cmp, bool isSkipped)
            => CheckOperation.Sync(
                () => { bool eq = string.Equals(download.Url, url, cmp()); return not ? !eq : eq; },
                () => not ? $"Expected download URL not to be \"{url}\", but was \"{download.Url}\""
                          : $"Expected download URL = \"{url}\", but was \"{download.Url}\"",
                isSkipped);
    }
}

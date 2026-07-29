using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace Catchy.Sdk
{
    public static class PwApiResponseChecks
    {
        public static CheckOperation IsOk(IAPIResponse response, bool not, bool isSkipped)
            => PwLocatorChecks.PwOp(isSkipped,
                () => not
                    ? Microsoft.Playwright.Assertions.Expect(response).Not.ToBeOKAsync()
                    : Microsoft.Playwright.Assertions.Expect(response).ToBeOKAsync(),
                not ? $"Expected response not to be OK, but was {response.Status}"
                    : $"Expected response to be OK (2xx), but was {response.Status}");

        public static CheckOperation HasStatus(IAPIResponse response, int status, bool not, bool isSkipped)
            => CheckOperation.Sync(
                () => not ? response.Status != status : response.Status == status,
                () => not ? $"Expected response status not to be {status}, but was {response.Status}"
                          : $"Expected response status {status}, but was {response.Status}",
                isSkipped);

        public static CheckOperation HasStatusInRange(IAPIResponse response, int min, int max, bool not, bool isSkipped)
            => CheckOperation.Sync(
                () => { bool ok = response.Status >= min && response.Status <= max; return not ? !ok : ok; },
                () => not ? $"Expected response status not to be in [{min}, {max}], but was {response.Status}"
                          : $"Expected response status in [{min}, {max}], but was {response.Status}",
                isSkipped);

        public static CheckOperation HasHeader(IAPIResponse response, string name, bool not, bool isSkipped)
            => CheckOperation.Sync(
                () => { bool ok = response.Headers.ContainsKey(name.ToLowerInvariant()); return not ? !ok : ok; },
                () => not ? $"Expected response not to have header \"{name}\""
                          : $"Expected response to have header \"{name}\"",
                isSkipped);

        public static CheckOperation HasHeaderValue(IAPIResponse response, string name, string expected, bool not,
            Func<StringComparison> getComparison, bool isSkipped)
        {
            response.Headers.TryGetValue(name.ToLowerInvariant(), out var actual);
            return CheckOperation.Sync(
                () => { bool eq = string.Equals(actual, expected, getComparison()); return not ? !eq : eq; },
                () => not ? $"Expected header \"{name}\" not to equal \"{expected}\", but was \"{actual}\""
                          : $"Expected header \"{name}\" = \"{expected}\", but was \"{actual ?? "null"}\"",
                isSkipped);
        }

        public static CheckOperation HasContentType(IAPIResponse response, string mediaType, bool not, bool isSkipped)
        {
            response.Headers.TryGetValue("content-type", out var raw);
            var actual = raw?.Split(';')[0].Trim();
            return CheckOperation.Sync(
                () => { bool ok = string.Equals(actual, mediaType, StringComparison.OrdinalIgnoreCase); return not ? !ok : ok; },
                () => not ? $"Expected Content-Type not to be \"{mediaType}\", but was \"{actual}\""
                          : $"Expected Content-Type = \"{mediaType}\", but was \"{actual ?? "none"}\"",
                isSkipped);
        }

        public static CheckOperation BodyContains(IAPIResponse response, string substring, bool not,
            Func<StringComparison> getComparison, bool isSkipped)
        {
            string? text = null;
            return CheckOperation.Async(async () =>
            {
                text = await response.TextAsync().ConfigureAwait(false);
                bool ok = text.Contains(substring, getComparison());
                return not ? !ok : ok;
            }, () => not ? $"Expected response body not to contain \"{substring}\""
                         : $"Expected response body to contain \"{substring}\"", isSkipped);
        }

        public static CheckOperation BodyMatches(IAPIResponse response, Regex pattern, bool not, bool isSkipped)
        {
            string? text = null;
            return CheckOperation.Async(async () =>
            {
                text = await response.TextAsync().ConfigureAwait(false);
                bool ok = pattern.IsMatch(text);
                return not ? !ok : ok;
            }, () => not ? $"Expected response body not to match /{pattern}/"
                         : $"Expected response body to match /{pattern}/", isSkipped);
        }

        public static CheckOperation IsValidJson(IAPIResponse response, bool not, bool isSkipped)
            => CheckOperation.Async(async () =>
            {
                var text = await response.TextAsync().ConfigureAwait(false);
                bool ok; try { JsonDocument.Parse(text); ok = true; } catch { ok = false; }
                return not ? !ok : ok;
            }, () => not ? "Expected response not to be valid JSON" : "Expected response to be valid JSON", isSkipped);

        public static CheckOperation HasJsonValue<T>(IAPIResponse response, string path, T expected, bool not, bool isSkipped)
        {
            T? actual = default;
            return CheckOperation.Async(async () =>
            {
                var text = await response.TextAsync().ConfigureAwait(false);
                try
                {
                    using var doc = JsonDocument.Parse(text);
                    if (TryGetPath(doc.RootElement, path, out var el))
                        actual = JsonSerializer.Deserialize<T>(el.GetRawText());
                }
                catch { }
                bool eq = Equals(actual, expected);
                return not ? !eq : eq;
            }, () => not ? $"Expected JSON[\"{path}\"] not to equal {expected}"
                         : $"Expected JSON[\"{path}\"] = {expected}, but was {actual}", isSkipped);
        }

        public static CheckOperation HasJsonArrayLength(IAPIResponse response, string path, int count, bool not, bool isSkipped)
        {
            int actual = -1;
            return CheckOperation.Async(async () =>
            {
                var text = await response.TextAsync().ConfigureAwait(false);
                try
                {
                    using var doc = JsonDocument.Parse(text);
                    if (TryGetPath(doc.RootElement, path, out var el) && el.ValueKind == JsonValueKind.Array)
                        actual = el.GetArrayLength();
                }
                catch { }
                bool ok = actual == count;
                return not ? !ok : ok;
            }, () => not ? $"Expected JSON[\"{path}\"] array length not to be {count}"
                         : $"Expected JSON[\"{path}\"] array length = {count}, but was {actual}", isSkipped);
        }

        private static bool TryGetPath(JsonElement root, string path, out JsonElement result)
        {
            result = root;
            foreach (var seg in path.Split('.'))
                if (result.ValueKind != JsonValueKind.Object || !result.TryGetProperty(seg, out result))
                    return false;
            return true;
        }
    }
}

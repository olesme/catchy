using System.Net;
using System.Net.Http;
using System.Text;
using System.Linq;

namespace Catchy.Sdk
{
    public static class HttpContentChecks
    {
        static string Fmt(object? v, string? expr = null) => ExprFormat.Inline(v, expr);

        public static CheckOperation Is(HttpContent? actual, HttpContent expected, bool isSkipped, string? expr = null)
            => CheckOperation.Async(async () =>
            {
                if (actual is null && expected is null) return true;
                if (actual is null || expected is null) return false;

                // Read bytes once (HttpContent stream is single-use)
                var aBytes = await actual.ReadAsByteArrayAsync().ConfigureAwait(false);
                var eBytes = await expected.ReadAsByteArrayAsync().ConfigureAwait(false);
                if (aBytes is null && eBytes is null) return true;
                if (aBytes is null || eBytes is null) return false;
                if (aBytes.LongLength != eBytes.LongLength) return false;

                // Decode as UTF-8 for text/JSON comparison
                var actualMedia = NormalizeMediaType(actual.Headers?.ContentType?.MediaType);
                var expectedMedia = NormalizeMediaType(expected.Headers?.ContentType?.MediaType);

                var actStr = Encoding.UTF8.GetString(aBytes);
                var expStr = Encoding.UTF8.GetString(eBytes);

                // If media types look like JSON, perform structural comparison
                if (!string.IsNullOrEmpty(actualMedia) && actualMedia!.Contains("json")
                    && !string.IsNullOrEmpty(expectedMedia) && expectedMedia!.Contains("json"))
                {
                    return JsonElementDeepEquals(actStr, expStr);
                }

                // If text content, compare strings
                if (IsLikelyText(actual) && IsLikelyText(expected))
                    return string.Equals(actStr, expStr, StringComparison.Ordinal);

                // Binary comparison by bytes
                for (long i = 0; i < aBytes.LongLength; i++) if (aBytes[i] != eBytes[i]) return false;
                return true;
            },
            () => $"Expected HttpContent to be {Fmt(expected, expr)}, but was {(actual is null ? "null" : actual.ToString())}",
            isSkipped,
            actual);

        public static CheckOperation IsNot(HttpContent? actual, HttpContent unexpected, bool isSkipped, string? expr = null)
            => CheckOperation.Async(async () =>
            {
                if (actual is null && unexpected is null) return false;
                if (actual is null || unexpected is null) return true;

                // Read bytes once (HttpContent stream is single-use)
                var aBytes = await actual.ReadAsByteArrayAsync().ConfigureAwait(false);
                var uBytes = await unexpected.ReadAsByteArrayAsync().ConfigureAwait(false);
                if (aBytes is null && uBytes is null) return false;
                if (aBytes is null || uBytes is null) return true;
                if (aBytes.LongLength != uBytes.LongLength) return true;

                // Decode as UTF-8 for text/JSON comparison
                var actualMedia = NormalizeMediaType(actual.Headers?.ContentType?.MediaType);
                var unexpectedMedia = NormalizeMediaType(unexpected.Headers?.ContentType?.MediaType);

                var actStr = Encoding.UTF8.GetString(aBytes);
                var unStr = Encoding.UTF8.GetString(uBytes);

                // If media types look like JSON, perform structural comparison
                if (!string.IsNullOrEmpty(actualMedia) && actualMedia!.Contains("json")
                    && !string.IsNullOrEmpty(unexpectedMedia) && unexpectedMedia!.Contains("json"))
                {
                    return !JsonElementDeepEquals(actStr, unStr);
                }

                // If text content, compare strings
                if (IsLikelyText(actual) && IsLikelyText(unexpected))
                    return !string.Equals(actStr, unStr, StringComparison.Ordinal);

                // Binary comparison by bytes
                for (long i = 0; i < aBytes.LongLength; i++) if (aBytes[i] != uBytes[i]) return true;
                return false;
            },
            () => $"Expected HttpContent not to be {Fmt(unexpected, expr)}, but was {(actual is null ? "null" : actual.ToString())}",
            isSkipped,
            actual);

        public static CheckOperation HasMediaType(HttpContent? actual, string mediaType, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && string.Equals(NormalizeMediaType(actual.Headers.ContentType?.MediaType), NormalizeMediaType(mediaType), StringComparison.OrdinalIgnoreCase),
                () => $"Expected media type to be {Fmt(mediaType, expr)}, but was '{NormalizeMediaType(actual?.Headers.ContentType?.MediaType) ?? "none"}'",
                isSkipped,
                actual);

        public static CheckOperation DoesNotHaveMediaType(HttpContent? actual, string mediaType, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is null || !string.Equals(NormalizeMediaType(actual.Headers.ContentType?.MediaType), NormalizeMediaType(mediaType), StringComparison.OrdinalIgnoreCase),
                () => $"Expected media type not to be {Fmt(mediaType, expr)}",
                isSkipped,
                actual);

        static string? NormalizeMediaType(string? mediaType)
        {
            if (mediaType is null) return null;
            // strip parameters like ; charset=utf-8
            var idx = mediaType.IndexOf(';');
            return idx >= 0 ? mediaType.Substring(0, idx).Trim().ToLowerInvariant() : mediaType.Trim().ToLowerInvariant();
        }

        public static CheckOperation HasHeader(HttpContent? actual, string headerName, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && actual.Headers.Contains(headerName),
                () => $"Expected content to have header {Fmt(headerName, expr)}",
                isSkipped,
                actual,
                hintsFactory: () => new (string, object?, string?)[] { ("Headers", (object?)string.Join(", ", actual?.Headers.Select(h => h.Key) ?? Array.Empty<string>()), null) });

        public static CheckOperation DoesNotHaveHeader(HttpContent? actual, string headerName, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is null || !actual.Headers.Contains(headerName),
                () => $"Expected content not to have header {Fmt(headerName, expr)}",
                isSkipped,
                actual);

        public static CheckOperation HasHeaderValue(HttpContent? actual, string headerName, string expectedValue, bool isSkipped, string? nameExpr = null, string? valueExpr = null)
        {
            return CheckOperation.Sync(() =>
            {
                if (actual is null) return false;
                if (actual.Headers.TryGetValues(headerName, out var vals))
                    return vals.Any(v => v.Contains(expectedValue, StringComparison.OrdinalIgnoreCase));
                return false;
            },
            () => $"Expected content header {Fmt(headerName, nameExpr)} to contain {Fmt(expectedValue, valueExpr)}",
            isSkipped,
            actual);
        }

        public static CheckOperation HasStringContent(HttpContent? actual, string expected, bool isSkipped, string? expr = null)
            => CheckOperation.Async(async () =>
            {
                if (actual is null) return false;
                var s = await actual.ReadAsStringAsync().ConfigureAwait(false);
                return string.Equals(s, expected, StringComparison.Ordinal);
            },
            () => $"Expected content string to be {Fmt(expected, expr)}",
            isSkipped,
            actual);

        public static CheckOperation IsNotStringContent(HttpContent? actual, string unexpected, bool isSkipped, string? expr = null)
            => CheckOperation.Async(async () =>
            {
                if (actual is null) return true;
                var s = await actual.ReadAsStringAsync().ConfigureAwait(false);
                return !string.Equals(s, unexpected, StringComparison.Ordinal);
            },
            () => $"Expected content string not to be {Fmt(unexpected, expr)}",
            isSkipped,
            actual);

        public static CheckOperation ContainsStringContent(HttpContent? actual, string expected, bool isSkipped, string? expr = null)
            => CheckOperation.Async(async () =>
            {
                if (actual is null) return false;
                var s = await actual.ReadAsStringAsync().ConfigureAwait(false);
                return s?.Contains(expected, StringComparison.OrdinalIgnoreCase) == true;
            },
            () => $"Expected content to contain {Fmt(expected, expr)}",
            isSkipped,
            actual);

        static bool IsLikelyText(HttpContent content)
        {
            var mt = content.Headers?.ContentType?.MediaType;
            if (string.IsNullOrEmpty(mt)) return true; // assume text when unknown
            var normalized = mt!.ToLowerInvariant();
            return normalized.StartsWith("text/") || normalized.Contains("json") || normalized.Contains("xml") || normalized.Contains("html");
        }

        static bool JsonElementDeepEquals(string? a, string? b)
        {
            try
            {
                using var da = System.Text.Json.JsonDocument.Parse(a ?? string.Empty);
                using var db = System.Text.Json.JsonDocument.Parse(b ?? string.Empty);
                return JsonElementDeepEquals(da.RootElement, db.RootElement);
            }
            catch
            {
                return string.Equals(a, b, StringComparison.Ordinal);
            }
        }

        static bool JsonElementDeepEquals(System.Text.Json.JsonElement a, System.Text.Json.JsonElement b)
        {
            if (a.ValueKind != b.ValueKind) return false;
            switch (a.ValueKind)
            {
                case System.Text.Json.JsonValueKind.Object:
                    {
                        var aProps = a.EnumerateObject().OrderBy(p => p.Name, StringComparer.Ordinal).ToArray();
                        var bProps = b.EnumerateObject().OrderBy(p => p.Name, StringComparer.Ordinal).ToArray();
                        if (aProps.Length != bProps.Length) return false;
                        for (int i = 0; i < aProps.Length; i++)
                        {
                            if (aProps[i].Name != bProps[i].Name) return false;
                            if (!JsonElementDeepEquals(aProps[i].Value, bProps[i].Value)) return false;
                        }
                        return true;
                    }
                case System.Text.Json.JsonValueKind.Array:
                    {
                        var ae = a.EnumerateArray().ToArray();
                        var be = b.EnumerateArray().ToArray();
                        if (ae.Length != be.Length) return false;
                        for (int i = 0; i < ae.Length; i++) if (!JsonElementDeepEquals(ae[i], be[i])) return false;
                        return true;
                    }
                case System.Text.Json.JsonValueKind.String:
                    return a.GetString() == b.GetString();
                case System.Text.Json.JsonValueKind.Number:
                    return a.GetRawText() == b.GetRawText();
                case System.Text.Json.JsonValueKind.True:
                case System.Text.Json.JsonValueKind.False:
                    return a.GetBoolean() == b.GetBoolean();
                case System.Text.Json.JsonValueKind.Null:
                    return true;
                default:
                    return a.GetRawText() == b.GetRawText();
            }
        }

        public static CheckOperation HasLength(HttpContent? actual, long expected, bool isSkipped, string? expr = null)
            => CheckOperation.Async(async () =>
            {
                if (actual is null) return false;
                var bytes = await actual.ReadAsByteArrayAsync().ConfigureAwait(false);
                return bytes.LongLength == expected;
            },
            () => $"Expected content length to be {Fmt(expected, expr)}",
            isSkipped,
            actual);

        public static CheckOperation IsEmpty(HttpContent? actual, bool isSkipped)
            => CheckOperation.Async(async () =>
            {
                if (actual is null) return true;
                var s = await actual.ReadAsStringAsync().ConfigureAwait(false);
                return string.IsNullOrEmpty(s);
            },
            () => "Expected content to be empty",
            isSkipped,
            actual);
    }
}

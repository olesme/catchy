using System.Net;
using System.Net.Http;
using System.Text;
using System.Linq;

namespace Catchy.Sdk
{
    public static class HttpRequestMessageChecks
    {
        static string Fmt(object? v, string? expr = null) => ExprFormat.Inline(v, expr);

        public static CheckOperation Is(HttpRequestMessage? actual, HttpRequestMessage expected, bool isSkipped, string? expr = null)
            => CheckOperation.Async(async () =>
            {
                if (actual is null && expected is null) return true;
                if (actual is null || expected is null) return false;

                // Compare method and request URI first
                if (!string.Equals(actual.Method?.Method, expected.Method?.Method, StringComparison.OrdinalIgnoreCase)) return false;
                if (!Equals(actual.RequestUri, expected.RequestUri)) return false;

                // Headers (simple subset): compare request headers and content headers
                if (!HeadersEqual(actual.Headers, expected.Headers)) return false;
                if (!HeadersEqual(actual.Content?.Headers, expected.Content?.Headers)) return false;

                // Read content bytes once (HttpContent streams are single-use)
                var aBytes = actual.Content is null ? null : await actual.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                var eBytes = expected.Content is null ? null : await expected.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                if (aBytes is null && eBytes is null) return true;
                if (aBytes is null || eBytes is null) return false;
                if (aBytes.LongLength != eBytes.LongLength) return false;

                // Decode as UTF-8 for text/JSON comparison
                var actualMedia = NormalizeMediaType(actual.Content?.Headers.ContentType?.MediaType);
                var expectedMedia = NormalizeMediaType(expected.Content?.Headers.ContentType?.MediaType);

                var actStr = Encoding.UTF8.GetString(aBytes);
                var expStr = Encoding.UTF8.GetString(eBytes);

                // If media types look like JSON, perform structural comparison
                if (!string.IsNullOrEmpty(actualMedia) && actualMedia!.Contains("json")
                    && !string.IsNullOrEmpty(expectedMedia) && expectedMedia!.Contains("json"))
                {
                    return JsonElementDeepEquals(actStr, expStr);
                }

                // If text content, compare strings
                var actualIsText = IsLikelyText(actual);
                var expectedIsText = IsLikelyText(expected);
                if (actualIsText && expectedIsText)
                {
                    return string.Equals(actStr, expStr, StringComparison.Ordinal);
                }

                if (actualIsText != expectedIsText) return false;

                // Binary comparison by bytes
                for (long i = 0; i < aBytes.LongLength; i++) if (aBytes[i] != eBytes[i]) return false;
                return true;
            },
            () => $"Expected HttpRequestMessage to be {Fmt(expected, expr)}, but was {(actual is null ? "null" : actual.ToString())}",
            isSkipped,
            actual);

        public static CheckOperation IsNot(HttpRequestMessage? actual, HttpRequestMessage? unexpected, bool isSkipped, string? expr = null)
            => CheckOperation.Async(async () =>
            {
                if (actual is null && unexpected is null) return false;
                if (actual is null || unexpected is null) return true;

                // Read content bytes once (HttpContent streams are single-use)
                var aBytes = actual.Content is null ? null : await actual.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                var uBytes = unexpected.Content is null ? null : await unexpected.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                if (aBytes is null && uBytes is null) return false;
                if (aBytes is null || uBytes is null) return true;
                if (aBytes.LongLength != uBytes.LongLength) return true;

                // Decode as UTF-8 for text/JSON comparison
                var actualMedia = NormalizeMediaType(actual.Content?.Headers.ContentType?.MediaType);
                var unexpectedMedia = NormalizeMediaType(unexpected.Content?.Headers.ContentType?.MediaType);

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
            () => $"Expected HttpRequestMessage not to be {Fmt(unexpected, expr)}, but was {(actual is null ? "null" : actual.ToString())}",
            isSkipped,
            actual);

        public static CheckOperation HasMediaType(HttpRequestMessage? actual, string mediaType, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && string.Equals(NormalizeMediaType(actual.Content?.Headers.ContentType?.MediaType), NormalizeMediaType(mediaType), StringComparison.OrdinalIgnoreCase),
                () => $"Expected media type to be {Fmt(mediaType, expr)}, but was '{NormalizeMediaType(actual?.Content?.Headers.ContentType?.MediaType) ?? "none"}'",
                isSkipped,
                actual);

        public static CheckOperation DoesNotHaveMediaType(HttpRequestMessage? actual, string mediaType, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is null || !string.Equals(NormalizeMediaType(actual.Content?.Headers.ContentType?.MediaType), NormalizeMediaType(mediaType), StringComparison.OrdinalIgnoreCase),
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

        public static CheckOperation HasHeader(HttpRequestMessage? actual, string headerName, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && actual.Headers.Contains(headerName),
                () => $"Expected content to have header {Fmt(headerName, expr)}",
                isSkipped,
                actual,
                hintsFactory: () => new (string, object?, string?)[] { ("Headers", (object?)string.Join(", ", actual?.Headers.Select(h => h.Key) ?? Array.Empty<string>()), null) });

        public static CheckOperation DoesNotHaveHeader(HttpRequestMessage? actual, string headerName, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is null || !actual.Headers.Contains(headerName),
                () => $"Expected content not to have header {Fmt(headerName, expr)}",
                isSkipped,
                actual);

        public static CheckOperation HasHeaderValue(HttpRequestMessage? actual, string headerName, string expectedValue, bool isSkipped, string? nameExpr = null, string? valueExpr = null)
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

        public static CheckOperation HasStringContent(HttpRequestMessage? actual, string expected, bool isSkipped, string? expr = null)
            => CheckOperation.Async(async () =>
            {
                if (actual is null) return false;
                var s = actual.Content is null ? null : await actual.Content.ReadAsStringAsync().ConfigureAwait(false);
                return string.Equals(s, expected, StringComparison.Ordinal);
            },
            () => $"Expected content string to be {Fmt(expected, expr)}",
            isSkipped,
            actual);

        public static CheckOperation IsNotStringContent(HttpRequestMessage? actual, string unexpected, bool isSkipped, string? expr = null)
            => CheckOperation.Async(async () =>
            {
                if (actual is null) return true;
                var s = actual.Content is null ? null : await actual.Content.ReadAsStringAsync().ConfigureAwait(false);
                return !string.Equals(s, unexpected, StringComparison.Ordinal);
            },
            () => $"Expected content string not to be {Fmt(unexpected, expr)}",
            isSkipped,
            actual);

        public static CheckOperation ContainsStringContent(HttpRequestMessage? actual, string expected, bool isSkipped, string? expr = null)
            => CheckOperation.Async(async () =>
            {
                if (actual is null) return false;
                var s = actual.Content is null ? null : await actual.Content.ReadAsStringAsync().ConfigureAwait(false);
                return s?.Contains(expected, StringComparison.OrdinalIgnoreCase) == true;
            },
            () => $"Expected content to contain {Fmt(expected, expr)}",
            isSkipped,
            actual);

        static bool IsLikelyText(HttpRequestMessage content)
        {
            var mt = content.Content?.Headers.ContentType?.MediaType;
            if (string.IsNullOrEmpty(mt)) return true; // assume text when unknown
            var normalized = mt!.ToLowerInvariant();
            return normalized.StartsWith("text/") || normalized.Contains("json") || normalized.Contains("xml") || normalized.Contains("html");
        }

        static bool HeadersEqual(System.Net.Http.Headers.HttpHeaders? a, System.Net.Http.Headers.HttpHeaders? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            var aKeys = a.Select(h => h.Key).OrderBy(k => k, StringComparer.OrdinalIgnoreCase).ToArray();
            var bKeys = b.Select(h => h.Key).OrderBy(k => k, StringComparer.OrdinalIgnoreCase).ToArray();
            if (aKeys.Length != bKeys.Length) return false;
            for (int i = 0; i < aKeys.Length; i++) if (!string.Equals(aKeys[i], bKeys[i], StringComparison.OrdinalIgnoreCase)) return false;
            foreach (var k in aKeys)
            {
                if (!a.TryGetValues(k, out var av)) return false;
                if (!b.TryGetValues(k, out var bv)) return false;
                var avs = av.OrderBy(x => x, StringComparer.Ordinal).ToArray();
                var bvs = bv.OrderBy(x => x, StringComparer.Ordinal).ToArray();
                if (avs.Length != bvs.Length) return false;
                for (int i = 0; i < avs.Length; i++) if (avs[i] != bvs[i]) return false;
            }
            return true;
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

        public static CheckOperation HasLength(HttpRequestMessage? actual, long expected, bool isSkipped, string? expr = null)
            => CheckOperation.Async(async () =>
            {
                if (actual?.Content is null) return false;
                var bytes = await actual.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                return bytes.LongLength == expected;
            },
            () => $"Expected content length to be {Fmt(expected, expr)}",
            isSkipped,
            actual);

        public static CheckOperation IsEmpty(HttpRequestMessage? actual, bool isSkipped)
            => CheckOperation.Async(async () =>
            {
                if (actual is null) return true;
                var s = actual.Content is null ? null : await actual.Content.ReadAsStringAsync().ConfigureAwait(false);
                return string.IsNullOrEmpty(s);
            },
            () => "Expected content to be empty",
            isSkipped,
            actual);
    }
}

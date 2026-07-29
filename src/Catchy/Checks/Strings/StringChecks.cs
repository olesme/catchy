using System.Text.RegularExpressions;

namespace Catchy.Sdk
{
    public static class StringChecks
    {
        static string Fmt(object? v, string? expr = null) => ExprFormat.Inline(v, expr);

        public static CheckOperation EqualTo(string? actual, string? expected,
            Func<StringComparison> getComparison, bool isSkipped, string? expectedExpr = null)
            => CheckOperation.Sync(
                () => string.Equals(actual, expected, getComparison()),
                () => { var diff = StringDiff.Build(expected, actual); return $"Expected {Fmt(actual)} to equal {Fmt(expected, expectedExpr)} ({getComparison()}){diff}"; },
                isSkipped);

        public static CheckOperation EqualTo(string? actual, string? expected,
            StringComparison comparison, bool isSkipped, string? expectedExpr = null)
            => CheckOperation.Sync(
                () => string.Equals(actual, expected, comparison),
                () => { var diff = StringDiff.Build(expected, actual); return $"Expected {Fmt(actual)} to equal {Fmt(expected, expectedExpr)} ({comparison}){diff}"; },
                isSkipped);

        public static CheckOperation NotEqualTo(string? actual, string? expected, StringComparison comparison,
            bool isSkipped, string? expectedExpr = null)
            => CheckOperation.Sync(
                () => !string.Equals(actual, expected, comparison),
                () => $"Expected {Fmt(actual)} not to equal {Fmt(expected, expectedExpr)}",
                isSkipped);

        public static CheckOperation Contains(string? actual, string substring, StringComparison comparison,
            bool isSkipped, string? substringExpr = null)
            => CheckOperation.Sync(
                () => actual?.IndexOf(substring, comparison) >= 0,
                () => $"Expected {Fmt(actual)} to contain {Fmt(substring, substringExpr)}",
                isSkipped);

        public static CheckOperation NotContains(string? actual, string substring, StringComparison comparison,
            bool isSkipped, string? substringExpr = null)
            => CheckOperation.Sync(
                () => actual is null || actual.IndexOf(substring, comparison) < 0,
                () => $"Expected {Fmt(actual)} not to contain {Fmt(substring, substringExpr)}",
                isSkipped);

        public static CheckOperation StartsWith(string? actual, string prefix, StringComparison comparison,
            bool isSkipped, string? prefixExpr = null)
            => CheckOperation.Sync(
                () => actual?.StartsWith(prefix, comparison) == true,
                () => $"Expected {Fmt(actual)} to start with {Fmt(prefix, prefixExpr)}",
                isSkipped);

        public static CheckOperation NotStartsWith(string? actual, string prefix, StringComparison comparison,
            bool isSkipped, string? prefixExpr = null)
            => CheckOperation.Sync(
                () => actual is null || !actual.StartsWith(prefix, comparison),
                () => $"Expected {Fmt(actual)} not to start with {Fmt(prefix, prefixExpr)}",
                isSkipped);

        public static CheckOperation EndsWith(string? actual, string suffix, StringComparison comparison,
            bool isSkipped, string? suffixExpr = null)
            => CheckOperation.Sync(
                () => actual?.EndsWith(suffix, comparison) == true,
                () => $"Expected {Fmt(actual)} to end with {Fmt(suffix, suffixExpr)}",
                isSkipped);

        public static CheckOperation NotEndsWith(string? actual, string suffix, StringComparison comparison,
            bool isSkipped, string? suffixExpr = null)
            => CheckOperation.Sync(
                () => actual is null || !actual.EndsWith(suffix, comparison),
                () => $"Expected {Fmt(actual)} not to end with {Fmt(suffix, suffixExpr)}",
                isSkipped);

        public static CheckOperation IsNullOrEmpty(string? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => string.IsNullOrEmpty(actual),
                () => $"Expected string to be null or empty, but was {Fmt(actual)}",
                isSkipped);

        public static CheckOperation IsNotNullOrEmpty(string? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => !string.IsNullOrEmpty(actual),
                () => $"Expected non-null non-empty string, but was {Fmt(actual)}",
                isSkipped);

        public static CheckOperation IsNullOrWhiteSpace(string? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => string.IsNullOrWhiteSpace(actual),
                () => $"Expected string to be null or whitespace",
                isSkipped);

        public static CheckOperation IsNotNullOrWhiteSpace(string? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => !string.IsNullOrWhiteSpace(actual),
                () => $"Expected non-null non-whitespace string, but was {Fmt(actual)}",
                isSkipped);

        public static CheckOperation HasLength(string? actual, int expected, bool isSkipped, string? expectedExpr = null)
            => CheckOperation.Sync(
                () => actual?.Length == expected,
                () => $"Expected string length {expected}, but was {actual?.Length.ToString() ?? "null"}",
                isSkipped);

        public static CheckOperation HasLengthGreaterThan(string? actual, int length, bool isSkipped, string? lengthExpr = null)
            => CheckOperation.Sync(
                () => actual?.Length > length,
                () => $"Expected string length > {length}, but was {actual?.Length.ToString() ?? "null"}",
                isSkipped);

        public static CheckOperation HasLengthLessThan(string? actual, int length, bool isSkipped, string? lengthExpr = null)
            => CheckOperation.Sync(
                () => actual is not null && actual.Length < length,
                () => $"Expected string length < {length}, but was {actual?.Length.ToString() ?? "null"}",
                isSkipped);

        public static CheckOperation DoesMatch(string? actual, Regex regex, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && regex.IsMatch(actual),
                () => $"Expected {Fmt(actual)} to match /{regex}/",
                isSkipped);

        public static CheckOperation DoesNotMatch(string? actual, Regex regex, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || !regex.IsMatch(actual),
                () => $"Expected {Fmt(actual)} not to match /{regex}/",
                isSkipped);

        public static CheckOperation IsUpperCase(string? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual == actual.ToUpperInvariant(),
                () => $"Expected {Fmt(actual)} to be upper-case",
                isSkipped);

        public static CheckOperation IsLowerCase(string? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual == actual.ToLowerInvariant(),
                () => $"Expected {Fmt(actual)} to be lower-case",
                isSkipped);

        public static CheckOperation IsTrimmed(string? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual == actual.Trim(),
                () => "Expected string to be trimmed",
                isSkipped);

        public static CheckOperation IsGuid(string? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && Guid.TryParse(actual, out _),
                () => $"Expected {Fmt(actual)} to be a valid GUID",
                isSkipped);

        public static CheckOperation IsOneOf(string? actual, IEnumerable<string> values, Func<StringComparison> getComparison,
            bool isSkipped)
        {
            var list = values is IReadOnlyList<string> r ? r : [.. values];
            return CheckOperation.Sync(
                () => actual is not null && list.Any(v => string.Equals(actual, v, getComparison())),
                () => $"Expected {Fmt(actual)} to be one of [{string.Join(", ", list)}]",
                isSkipped);
        }

        public static CheckOperation ContainsAll(string? actual, IEnumerable<string> substrings, StringComparison comparison, bool isSkipped)
        {
            var list = substrings as IReadOnlyList<string> ?? substrings.ToList();
            return CheckOperation.Sync(
                () => actual is not null && list.All(s => actual.IndexOf(s, comparison) >= 0),
                () =>
                {
                    if (actual is null) return "Expected a string, but was null";
                    var missing = list.Where(s => actual.IndexOf(s, comparison) < 0).ToList();
                    return $"Expected string to contain all of [{string.Join(", ", list.Select(s => $"\"{s}\""))}], missing: [{string.Join(", ", missing.Select(s => $"\"{s}\""))}]";
                },
                isSkipped);
        }

        public static CheckOperation ContainsAny(string? actual, IEnumerable<string> substrings, StringComparison comparison, bool isSkipped)
        {
            var list = substrings as IReadOnlyList<string> ?? substrings.ToList();
            return CheckOperation.Sync(
                () => actual is not null && list.Any(s => actual.IndexOf(s, comparison) >= 0),
                () => actual is null
                    ? "Expected a string, but was null"
                    : $"Expected string to contain at least one of [{string.Join(", ", list.Select(s => $"\"{s}\""))}]",
                isSkipped);
        }

        public static CheckOperation IsAlpha(string? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is { Length: > 0 } s && s.All(char.IsLetter),
                () => actual is null ? "Expected a string, but was null" : $"Expected {Fmt(actual)} to contain only letters",
                isSkipped);

        public static CheckOperation IsNumeric(string? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is { Length: > 0 } s && s.All(c => c >= '0' && c <= '9'),
                () => actual is null ? "Expected a string, but was null" : $"Expected {Fmt(actual)} to contain only digits",
                isSkipped);

        public static CheckOperation IsAlphanumeric(string? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is { Length: > 0 } s && s.All(c => char.IsLetter(c) || c >= '0' && c <= '9'),
                () => actual is null ? "Expected a string, but was null" : $"Expected {Fmt(actual)} to contain only letters or digits",
                isSkipped);

        public static CheckOperation IsBase64(string? actual, bool isSkipped)
            => CheckOperation.Sync(
                () =>
                {
                    if (actual is not { } s) return false;
                    try { Convert.FromBase64String(s); return true; }
                    catch { return false; }
                },
                () => actual is null ? "Expected a string, but was null" : $"Expected {Fmt(actual)} to be valid Base64",
                isSkipped);

        public static CheckOperation IsValidXml(string? actual, bool isSkipped)
            => CheckOperation.Sync(
                () =>
                {
                    if (actual is not { } s) return false;
                    try
                    {
                        var settings = new System.Xml.XmlReaderSettings { DtdProcessing = System.Xml.DtdProcessing.Ignore };
                        using var reader = System.Xml.XmlReader.Create(new System.IO.StringReader(s), settings);
                        while (reader.Read()) { }
                        return true;
                    }
                    catch { return false; }
                },
                () => actual is null ? "Expected a string, but was null" : $"Expected {Fmt(actual)} to be valid XML",
                isSkipped);

        public static CheckOperation HasNoLeadingWhiteSpace(string? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is { } s && (s.Length == 0 || !char.IsWhiteSpace(s[0])),
                () => actual is null ? "Expected a string, but was null" : $"Expected {Fmt(actual)} to have no leading whitespace",
                isSkipped);

        public static CheckOperation HasNoTrailingWhiteSpace(string? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is { } s && (s.Length == 0 || !char.IsWhiteSpace(s[s.Length - 1])),
                () => actual is null ? "Expected a string, but was null" : $"Expected {Fmt(actual)} to have no trailing whitespace",
                isSkipped);

        public static CheckOperation HasLineCount(string? actual, int expected, bool isSkipped, string? expectedExpr = null)
            => CheckOperation.Sync(
                () => actual is { } s && s.Split('\n').Length == expected,
                () =>
                {
                    if (actual is null) return "Expected a string, but was null";
                    var lineCount = actual.Split('\n').Length;
                    return $"Expected string to have {Fmt(expected, expectedExpr)} line(s), but had {lineCount}";
                },
                isSkipped);

        public static CheckOperation ContainsLine(string? actual, string line, StringComparison comparison, bool isSkipped, string? lineExpr = null)
            => CheckOperation.Sync(
                () => actual?.Split('\n').Any(l => string.Equals(l.TrimEnd('\r'), line.TrimEnd('\r'), comparison)) == true,
                () => actual is null ? "Expected a string, but was null" : $"Expected string to contain line {Fmt(line, lineExpr)}",
                isSkipped);

        public static CheckOperation IsValidJson(string? actual, bool isSkipped)
            => CheckOperation.Sync(
                () =>
                {
                    if (actual is not { } s) return false;
                    try { System.Text.Json.JsonDocument.Parse(s); return true; }
                    catch { return false; }
                },
                () => actual is null ? "Expected a string, but was null" : $"Expected {Fmt(actual)} to be valid JSON",
                isSkipped);

        public static CheckOperation IsJsonEquivalentTo(string? actual, string expectedJson, bool isSkipped, string? expectedExpr = null)
            => CheckOperation.Sync(
                () =>
                {
                    if (actual is not { } s) return false;
                    try
                    {
                        using var actualJson = System.Text.Json.JsonDocument.Parse(s);
                        using var expected = System.Text.Json.JsonDocument.Parse(expectedJson);
                        return StringComparisonHelper.JsonElementsEqual(actualJson.RootElement, expected.RootElement);
                    }
                    catch { return false; }
                },
                () => actual is null ? "Expected a string, but was null" : $"Expected JSON to be structurally equivalent to {Fmt(expectedJson, expectedExpr)}",
                isSkipped);

        public static CheckOperation IsJsonSerializable<T>(string? actual, bool isSkipped)
            => CheckOperation.Sync(
                () =>
                {
                    if (actual is not { } s) return false;
                    try { System.Text.Json.JsonSerializer.Deserialize<T>(s); return true; }
                    catch { return false; }
                },
                () => actual is null ? "Expected a string, but was null" : $"Expected string to be deserializable as {TypeHelper.FriendlyName(typeof(T))}",
                isSkipped);
    }

    public static class StringComparisonHelper
    {
        public const StringComparison UseDefault = (StringComparison)int.MaxValue;

        public static StringComparison Resolve(StringComparison requested, AssertionSettings settings)
            => requested == UseDefault ? settings.DefaultStringComparison : requested;

        public static bool JsonElementsEqual(System.Text.Json.JsonElement a, System.Text.Json.JsonElement b)
        {
            if (a.ValueKind != b.ValueKind) return false;
            return a.ValueKind switch
            {
                System.Text.Json.JsonValueKind.Object =>
                    a.EnumerateObject().OrderBy(p => p.Name)
                     .SequenceEqual(b.EnumerateObject().OrderBy(p => p.Name),
                         JsonPropertyComparer.Instance),
                System.Text.Json.JsonValueKind.Array =>
                    a.EnumerateArray().SequenceEqual(b.EnumerateArray(), JsonElementComparer.Instance),
                _ => a.ToString() == b.ToString(),
            };
        }
    }

    public sealed class JsonElementComparer : IEqualityComparer<System.Text.Json.JsonElement>
    {
        public static readonly JsonElementComparer Instance = new();
        public bool Equals(System.Text.Json.JsonElement x, System.Text.Json.JsonElement y) => StringComparisonHelper.JsonElementsEqual(x, y);
        public int GetHashCode(System.Text.Json.JsonElement obj) => (obj.ToString() ?? string.Empty).GetHashCode();
    }

    public sealed class JsonPropertyComparer : IEqualityComparer<System.Text.Json.JsonProperty>
    {
        public static readonly JsonPropertyComparer Instance = new();
        public bool Equals(System.Text.Json.JsonProperty x, System.Text.Json.JsonProperty y)
            => x.Name == y.Name && StringComparisonHelper.JsonElementsEqual(x.Value, y.Value);
        public int GetHashCode(System.Text.Json.JsonProperty obj) => obj.Name.GetHashCode();
    }
}

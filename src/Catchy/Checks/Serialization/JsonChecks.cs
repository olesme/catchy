using System.Text.Json;

namespace Catchy.Sdk
{
    public static class JsonChecks
    {
        public static CheckOperation Exists(JsonElement? element, string path, bool isSkipped)
            => CheckOperation.Sync(
                () => element is not null,
                () => $"Expected JSON path '{path}' to exist",
                isSkipped);

        public static CheckOperation DoesNotExist(JsonElement? element, string path, bool isSkipped)
            => CheckOperation.Sync(
                () => element is null,
                () => $"Expected JSON path '{path}' not to exist, but it does",
                isSkipped);

        public static CheckOperation HasValue(JsonElement? element, string path, object? expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () =>
                {
                    if (element is not { } el) return false;
                    if (expected is null) return el.ValueKind == JsonValueKind.Null;
                    var expectedJson = JsonSerializer.Serialize(expected);
                    try
                    {
                        using var doc = JsonDocument.Parse(expectedJson);
                        return JsonElementsEqual(el, doc.RootElement);
                    }
                    catch { return el.ToString() == expectedJson; }
                },
                () => element is null
                    ? $"Expected JSON path '{path}' to exist, but was not found"
                    : $"Expected JSON path '{path}' to equal {ExprFormat.Inline(expected, expr)}, but was {element.Value}",
                isSkipped);

        public static CheckOperation DoesNotHaveValue(JsonElement? element, string path, object? unexpected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () =>
                {
                    if (element is not { } el) return true;
                    if (unexpected is null) return el.ValueKind != JsonValueKind.Null;
                    var unexpectedJson = JsonSerializer.Serialize(unexpected);
                    try
                    {
                        using var doc = JsonDocument.Parse(unexpectedJson);
                        return !JsonElementsEqual(el, doc.RootElement);
                    }
                    catch { return el.ToString() != unexpectedJson; }
                },
                () => $"Expected JSON path '{path}' not to equal {ExprFormat.Inline(unexpected, expr)}",
                isSkipped);

        public static CheckOperation IsNull(JsonElement? element, string path, bool isSkipped)
            => CheckOperation.Sync(
                () => element?.ValueKind == JsonValueKind.Null,
                () => element is null
                    ? $"Expected JSON path '{path}' to exist, but was not found"
                    : $"Expected JSON path '{path}' to be null, but was {element.Value.ValueKind}",
                isSkipped);

        public static CheckOperation IsNotNull(JsonElement? element, string path, bool isSkipped)
            => CheckOperation.Sync(
                () => element is not null && element.Value.ValueKind != JsonValueKind.Null,
                () => element is null
                    ? $"Expected JSON path '{path}' to exist"
                    : $"Expected JSON path '{path}' not to be null",
                isSkipped);

        public static CheckOperation IsString(JsonElement? element, string path, bool isSkipped)
            => CheckOperation.Sync(
                () => element?.ValueKind == JsonValueKind.String,
                () => element is null
                    ? $"Expected JSON path '{path}' to exist"
                    : $"Expected JSON path '{path}' to be a string, but was {element.Value.ValueKind}",
                isSkipped);

        public static CheckOperation IsNumber(JsonElement? element, string path, bool isSkipped)
            => CheckOperation.Sync(
                () => element?.ValueKind == JsonValueKind.Number,
                () => element is null
                    ? $"Expected JSON path '{path}' to exist"
                    : $"Expected JSON path '{path}' to be a number, but was {element.Value.ValueKind}",
                isSkipped);

        public static CheckOperation IsBoolean(JsonElement? element, string path, bool isSkipped)
            => CheckOperation.Sync(
                () => element?.ValueKind is JsonValueKind.True or JsonValueKind.False,
                () => element is null
                    ? $"Expected JSON path '{path}' to exist"
                    : $"Expected JSON path '{path}' to be a boolean, but was {element.Value.ValueKind}",
                isSkipped);

        public static CheckOperation IsArray(JsonElement? element, string path, bool isSkipped)
            => CheckOperation.Sync(
                () => element?.ValueKind == JsonValueKind.Array,
                () => element is null
                    ? $"Expected JSON path '{path}' to exist"
                    : $"Expected JSON path '{path}' to be an array, but was {element.Value.ValueKind}",
                isSkipped);

        public static CheckOperation IsObject(JsonElement? element, string path, bool isSkipped)
            => CheckOperation.Sync(
                () => element?.ValueKind == JsonValueKind.Object,
                () => element is null
                    ? $"Expected JSON path '{path}' to exist"
                    : $"Expected JSON path '{path}' to be an object, but was {element.Value.ValueKind}",
                isSkipped);

        public static CheckOperation HasArrayLength(JsonElement? element, string path, int expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => element?.ValueKind == JsonValueKind.Array && element.Value.GetArrayLength() == expected,
                () => element is null
                    ? $"Expected JSON path '{path}' to exist"
                    : element.Value.ValueKind != JsonValueKind.Array
                        ? $"Expected JSON path '{path}' to be an array"
                        : $"Expected JSON array at '{path}' to have length {ExprFormat.Inline(expected, expr)}, but was {element.Value.GetArrayLength()}",
                isSkipped);

        public static CheckOperation IsTrue(JsonElement? element, string path, bool isSkipped)
            => CheckOperation.Sync(
                () => element?.ValueKind == JsonValueKind.True,
                () => element is null
                    ? $"Expected JSON path '{path}' to exist"
                    : $"Expected JSON path '{path}' to be true, but was {element.Value}",
                isSkipped);

        public static CheckOperation IsFalse(JsonElement? element, string path, bool isSkipped)
            => CheckOperation.Sync(
                () => element?.ValueKind == JsonValueKind.False,
                () => element is null
                    ? $"Expected JSON path '{path}' to exist"
                    : $"Expected JSON path '{path}' to be false, but was {element.Value}",
                isSkipped);

        internal static bool JsonElementsEqual(JsonElement a, JsonElement b)
        {
            if (a.ValueKind != b.ValueKind) return false;
            return a.ValueKind switch
            {
                JsonValueKind.Object => a.EnumerateObject().OrderBy(p => p.Name)
                    .SequenceEqual(b.EnumerateObject().OrderBy(p => p.Name), JsonPropertyComparer.Instance),
                JsonValueKind.Array => a.EnumerateArray().SequenceEqual(b.EnumerateArray(), JsonElementEqComparer.Instance),
                _ => a.ToString() == b.ToString(),
            };
        }

        private sealed class JsonElementEqComparer : IEqualityComparer<JsonElement>
        {
            public static readonly JsonElementEqComparer Instance = new();
            public bool Equals(JsonElement x, JsonElement y) => JsonElementsEqual(x, y);
            public int GetHashCode(JsonElement obj) => obj.ToString()?.GetHashCode() ?? 0;
        }

        private sealed class JsonPropertyComparer : IEqualityComparer<JsonProperty>
        {
            public static readonly JsonPropertyComparer Instance = new();
            public bool Equals(JsonProperty x, JsonProperty y) => x.Name == y.Name && JsonElementsEqual(x.Value, y.Value);
            public int GetHashCode(JsonProperty obj) => obj.Name.GetHashCode();
        }
    }

    public static class JsonPathNavigator
    {
        public static JsonElement? TryEvaluate(string json, string path)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                return Navigate(doc.RootElement.Clone(), NormalizePath(path));
            }
            catch { return null; }
        }

        private static string NormalizePath(string path)
        {
            var p = path.TrimStart();
            if (p == "$") return "";
            if (p.StartsWith("$.")) return p.Substring(2);
            if (p.StartsWith("$[")) return p.Substring(1);
            if (p.StartsWith("$")) return p.Substring(1);
            return p;
        }

        private static JsonElement? Navigate(JsonElement root, string path)
        {
            if (string.IsNullOrEmpty(path)) return root;
            var current = root;
            foreach (var segment in SplitSegments(path))
            {
                var indexMatch = System.Text.RegularExpressions.Regex.Match(segment, @"^(.*?)\[(\d+)\]$");
                if (indexMatch.Success)
                {
                    var prop = indexMatch.Groups[1].Value;
                    var idx = int.Parse(indexMatch.Groups[2].Value);
                    if (!string.IsNullOrEmpty(prop))
                    {
                        if (current.ValueKind != JsonValueKind.Object) return null;
                        if (!current.TryGetProperty(prop, out current)) return null;
                    }
                    if (current.ValueKind != JsonValueKind.Array) return null;
                    var arr = current.EnumerateArray().ToArray();
                    if (idx < 0 || idx >= arr.Length) return null;
                    current = arr[idx];
                }
                else
                {
                    if (current.ValueKind != JsonValueKind.Object) return null;
                    if (!current.TryGetProperty(segment, out current)) return null;
                }
            }
            return current;
        }

        private static IEnumerable<string> SplitSegments(string path)
        {
            int depth = 0, start = 0;
            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] == '[') depth++;
                else if (path[i] == ']') depth--;
                else if (path[i] == '.' && depth == 0)
                {
                    if (i > start) yield return path.Substring(start, i - start);
                    start = i + 1;
                }
            }
            if (start < path.Length) yield return path.Substring(start);
        }
    }
}

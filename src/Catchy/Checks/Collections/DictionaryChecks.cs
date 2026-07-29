namespace Catchy.Sdk
{
    public static class DictionaryChecks
    {
        static string Fmt(object? v, string? e = null) => ExprFormat.Inline(v, e);
        static string FmtItems<T>(IEnumerable<T> items, int max = 10)
        {
            var parts = new List<string>(); int count = 0;
            foreach (var x in items) { if (count++ >= max) { parts.Add("…"); break; } parts.Add(ValueFormatter.Format(x)); }
            return string.Join(", ", parts);
        }
        static EqualityComparer<T> Eq<T>() => EqualityComparer<T>.Default;

        public static CheckOperation IsEmpty<K, V>(IEnumerable<KeyValuePair<K, V>>? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || !actual.Any(),
                () => actual is null ? "Expected a dictionary, but was null"
                    : $"Expected empty dictionary, but had {actual.Count()} entries",
                isSkipped);

        public static CheckOperation IsNotEmpty<K, V>(IEnumerable<KeyValuePair<K, V>>? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.Any(),
                () => actual is null ? "Expected a dictionary, but was null"
                    : "Expected non-empty dictionary, but was empty",
                isSkipped);

        public static CheckOperation HasCount<K, V>(IEnumerable<KeyValuePair<K, V>>? actual, int expected, bool isSkipped)
            => CheckOperation.Sync(
                () => actual?.Count() == expected,
                () => actual is null ? "Expected a dictionary, but was null"
                    : $"Expected {expected} entries, but had {actual.Count()}",
                isSkipped);

        public static CheckOperation ContainsKey<K, V>(IEnumerable<KeyValuePair<K, V>>? actual, K key,
            string? keyExpr, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.Any(kv => Eq<K>().Equals(kv.Key, key)),
                () => actual is null ? "Expected a dictionary, but was null"
                    : $"Expected dictionary to contain key {Fmt(key, keyExpr)}",
                isSkipped);

        public static CheckOperation DoesNotContainKey<K, V>(IEnumerable<KeyValuePair<K, V>>? actual, K key,
            string? keyExpr, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || !actual.Any(kv => Eq<K>().Equals(kv.Key, key)),
                () => $"Expected dictionary not to contain key {Fmt(key, keyExpr)}",
                isSkipped);

        public static CheckOperation ContainsAllKeys<K, V>(IEnumerable<KeyValuePair<K, V>>? actual,
            IEnumerable<K> keys, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && keys.All(k => actual.Any(kv => Eq<K>().Equals(kv.Key, k))),
                () =>
                {
                    if (actual is null) return "Expected a dictionary, but was null";
                    var missing = keys.Where(k => !actual.Any(kv => Eq<K>().Equals(kv.Key, k))).ToList();
                    return $"Expected dictionary to contain all keys [{FmtItems(keys)}], missing: [{FmtItems(missing)}]";
                },
                isSkipped);

        public static CheckOperation ContainsAnyKey<K, V>(IEnumerable<KeyValuePair<K, V>>? actual,
            IEnumerable<K> keys, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && keys.Any(k => actual.Any(kv => Eq<K>().Equals(kv.Key, k))),
                () => actual is null ? "Expected a dictionary, but was null"
                    : $"Expected dictionary to contain any of keys [{FmtItems(keys)}]",
                isSkipped);

        public static CheckOperation ContainsNoneOfKeys<K, V>(IEnumerable<KeyValuePair<K, V>>? actual,
            IEnumerable<K> keys, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && !keys.Any(k => actual.Any(kv => Eq<K>().Equals(kv.Key, k))),
                () =>
                {
                    if (actual is null) return "Expected a dictionary, but was null";
                    var found = keys.Where(k => actual.Any(kv => Eq<K>().Equals(kv.Key, k))).ToList();
                    return $"Expected dictionary to contain none of keys [{FmtItems(keys)}], but found: [{FmtItems(found)}]";
                },
                isSkipped);

        public static CheckOperation ContainsValue<K, V>(IEnumerable<KeyValuePair<K, V>>? actual, V value,
            string? valueExpr, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.Any(kv => Eq<V>().Equals(kv.Value, value)),
                () => actual is null ? "Expected a dictionary, but was null"
                    : $"Expected dictionary to contain value {Fmt(value, valueExpr)}",
                isSkipped);

        public static CheckOperation DoesNotContainValue<K, V>(IEnumerable<KeyValuePair<K, V>>? actual, V value,
            string? valueExpr, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || !actual.Any(kv => Eq<V>().Equals(kv.Value, value)),
                () => $"Expected dictionary not to contain value {Fmt(value, valueExpr)}",
                isSkipped);

        public static CheckOperation ContainsAllValues<K, V>(IEnumerable<KeyValuePair<K, V>>? actual,
            IEnumerable<V> values, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && values.All(v => actual.Any(kv => Eq<V>().Equals(kv.Value, v))),
                () =>
                {
                    if (actual is null) return "Expected a dictionary, but was null";
                    var missing = values.Where(v => !actual.Any(kv => Eq<V>().Equals(kv.Value, v))).ToList();
                    return $"Expected dictionary to contain all values [{FmtItems(values)}], missing: [{FmtItems(missing)}]";
                },
                isSkipped);

        public static CheckOperation AllValuesSatisfy<K, V>(IEnumerable<KeyValuePair<K, V>>? actual,
            Func<V, bool> predicate, string? predicateExpr, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.All(kv => predicate(kv.Value)),
                () => actual is null ? "Expected a dictionary, but was null"
                    : $"Expected all values to satisfy: {predicateExpr ?? "<predicate>"}",
                isSkipped);

        public static CheckOperation AnyValueSatisfies<K, V>(IEnumerable<KeyValuePair<K, V>>? actual,
            Func<V, bool> predicate, string? predicateExpr, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.Any(kv => predicate(kv.Value)),
                () => actual is null ? "Expected a dictionary, but was null"
                    : $"Expected at least one value to satisfy: {predicateExpr ?? "<predicate>"}",
                isSkipped);

        public static CheckOperation ContainsEntry<K, V>(IEnumerable<KeyValuePair<K, V>>? actual,
            K key, V value, string? keyExpr, string? valueExpr, bool isSkipped)
            => CheckOperation.Sync(
                () =>
                {
                    if (actual is null) return false;
                    foreach (var kv in actual)
                        if (Eq<K>().Equals(kv.Key, key))
                            return Eq<V>().Equals(kv.Value, value);
                    return false;
                },
                () =>
                {
                    if (actual is null) return "Expected a dictionary, but was null";
                    var entry = actual.FirstOrDefault(kv => Eq<K>().Equals(kv.Key, key));
                    if (!actual.Any(kv => Eq<K>().Equals(kv.Key, key)))
                        return $"Expected key {Fmt(key, keyExpr)}, but was not found";
                    return $"Expected value {Fmt(value, valueExpr)} at key {Fmt(key, keyExpr)}, but was {Fmt(entry.Value)}";
                },
                isSkipped);

        public static CheckOperation HasKeyWithValue<K, V>(IEnumerable<KeyValuePair<K, V>>? actual,
            K key, Func<V, bool> valuePredicate, string? keyExpr, string? predicateExpr, bool isSkipped)
            => CheckOperation.Sync(
                () =>
                {
                    if (actual is null) return false;
                    foreach (var kv in actual)
                        if (Eq<K>().Equals(kv.Key, key))
                            return valuePredicate(kv.Value);
                    return false;
                },
                () =>
                {
                    if (actual is null) return "Expected a dictionary, but was null";
                    if (!actual.Any(kv => Eq<K>().Equals(kv.Key, key)))
                        return $"Expected key {Fmt(key, keyExpr)}, but was not found";
                    var entry = actual.First(kv => Eq<K>().Equals(kv.Key, key));
                    return $"Expected value at key {Fmt(key, keyExpr)} to satisfy: {predicateExpr ?? "<predicate>"}, " +
                           $"but was {Fmt(entry.Value)}";
                },
                isSkipped);

        public static CheckOperation KeysAreEquivalentTo<K, V>(IEnumerable<KeyValuePair<K, V>>? actual,
            IEnumerable<K> expectedKeys, bool isSkipped)
            => CheckOperation.Sync(
                () =>
                {
                    if (actual is null || actual.Count() != expectedKeys.Count()) return false;
                    var actualKeys = actual.Select(kv => kv.Key).ToList();
                    var remaining = new List<K>(expectedKeys);
                    foreach (var k in actualKeys)
                    {
                        int idx = remaining.FindIndex(e => Eq<K>().Equals(k, e));
                        if (idx < 0) return false;
                        remaining.RemoveAt(idx);
                    }
                    return remaining.Count == 0;
                },
                () =>
                {
                    if (actual is null) return "Expected a dictionary, but was null";
                    var actualKeys = actual.Select(kv => kv.Key).ToList();
                    return $"Expected keys to be equivalent to [{FmtItems(expectedKeys)}], but was [{FmtItems(actualKeys)}]";
                },
                isSkipped);

        public static CheckOperation ValuesAreEquivalentTo<K, V>(IEnumerable<KeyValuePair<K, V>>? actual,
            IEnumerable<V> expectedValues, bool isSkipped)
            => CheckOperation.Sync(
                () =>
                {
                    if (actual is null || actual.Count() != expectedValues.Count()) return false;
                    var actualValues = actual.Select(kv => kv.Value).ToList();
                    var remaining = new List<V>(expectedValues);
                    foreach (var v in actualValues)
                    {
                        int idx = remaining.FindIndex(e => Eq<V>().Equals(v, e));
                        if (idx < 0) return false;
                        remaining.RemoveAt(idx);
                    }
                    return remaining.Count == 0;
                },
                () =>
                {
                    if (actual is null) return "Expected a dictionary, but was null";
                    var actualValues = actual.Select(kv => kv.Value).ToList();
                    return $"Expected values to be equivalent to [{FmtItems(expectedValues)}], but was [{FmtItems(actualValues)}]";
                },
                isSkipped);

        public static CheckOperation HasDistinctValues<K, V>(IEnumerable<KeyValuePair<K, V>>? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.Select(kv => kv.Value).Distinct().Count() == actual.Count(),
                () => actual is null ? "Expected a dictionary, but was null"
                    : "Expected dictionary to have distinct values, but found duplicates",
                isSkipped);
    }
}

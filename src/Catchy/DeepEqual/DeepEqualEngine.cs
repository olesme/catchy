using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Catchy.Sdk
{
    internal static class DeepEqualEngine
    {
        private const string ReflectionWarning =
      "Deep equality uses reflection for unknown types. " +
      "Decorate your types with [Assertable] to generate AOT-safe comparators.";

        [RequiresUnreferencedCode(ReflectionWarning)]
        public static bool AreEqualObjects(object? a, object? b, EqualsOptions opts,
            DeepEqualRuleContainer? localRules = null)
        {
            var visited = new HashSet<(int, int)>();
            return AreEqual(a, b, opts, visited, 0, localRules);
        }

        [RequiresUnreferencedCode(ReflectionWarning)]
        public static IEnumerable<string> GetDiffs(object? a, object? b, EqualsOptions opts,
            DeepEqualRuleContainer? localRules = null)
        {
            if (a is null || b is null) yield break;

            var typeA = a.GetType();
            var typeB = b.GetType();
            bool crossType = typeA != typeB;

            var propsA = ReflectionMappingCache.GetReadableProperties(typeA);

            foreach (var propA in propsA)
            {
                if (opts.ShouldExclude(propA.Name)) continue;

                PropertyInfo? propB = null;
                if (crossType)
                {
                    propB = ReflectionMappingCache.TryGetProperty(typeB, propA.Name);
                    if (propB is null)
                    {
                        if (!opts.IgnoreExtraProperties)
                            yield return $"  {propA.Name}: present on actual, missing on expected";
                        continue;
                    }
                }

                var valA = propA.GetValue(a);
                var valB = crossType ? propB!.GetValue(b) : propA.GetValue(b);

                if (opts.IgnoreNullProperties && valA is null && valB is null) continue;

                if (!AreEqualObjects(valA, valB, opts, localRules))
                    yield return $"  {propA.Name}: {ValueFormatter.Format(valA)} != {ValueFormatter.Format(valB)}";
            }

            if (!crossType || opts.IgnoreExtraProperties) yield break;

            var propNamesA = new HashSet<string>(propsA.Select(p => p.Name), StringComparer.Ordinal);
            foreach (var propB2 in ReflectionMappingCache.GetReadableProperties(typeB))
            {
                if (!propNamesA.Contains(propB2.Name))
                    yield return $"  {propB2.Name}: missing on actual, present on expected";
            }
        }

        internal static bool AreEqualSkippingRegistry(object? a, object? b, EqualsOptions opts)
        {
            var visited = new HashSet<(int, int)>();
            return AreEqual(a, b, opts, visited, 0, instanceRules: null, skipRegistry: true);
        }

        [RequiresUnreferencedCode(ReflectionWarning)]
        internal static bool AreEqual(object? a, object? b, EqualsOptions opts,
            HashSet<(int, int)> visited, int depth, DeepEqualRuleContainer? instanceRules = null,
            bool skipRegistry = false)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            if (depth > 20) return Equals(a, b);

            if (instanceRules?.TryCompare(a, b, out var ir) == true) return ir;

            if (!skipRegistry && DeepEqualRuleRegistry.TryCompare(a, b, out var rr)) return rr;

            var typeA = a.GetType();
            var typeB = b.GetType();

            if (typeA == typeof(string) && typeB == typeof(string))
                return string.Equals((string)a, (string)b, opts.StringComparison);

            if (IsNumeric(typeA) && IsNumeric(typeB))
            {
                if (opts.FloatTolerance.HasValue && (typeA == typeof(double) || typeA == typeof(float)))
                    return Math.Abs(Convert.ToDouble(a) - Convert.ToDouble(b)) <= opts.FloatTolerance.Value;
                return Convert.ToDecimal(a) == Convert.ToDecimal(b);
            }

            if (a is IEnumerable ea && b is IEnumerable eb)
                return AreEnumerablesEqual(ea, eb, opts, visited, depth, instanceRules);

            int hA = System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(a);
            int hB = System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(b);
            if (!visited.Add((hA, hB)))
                return opts.IgnoreCyclicReferences;

            return ArePropertiesEqual(a, b, typeA, typeB, opts, visited, depth, instanceRules);
        }

        private static bool ArePropertiesEqual(object a, object b, Type typeA, Type typeB,
            EqualsOptions opts, HashSet<(int, int)> visited, int depth, DeepEqualRuleContainer? localRules)
        {
            var propsA = ReflectionMappingCache.GetReadableProperties(typeA);
            bool crossType = typeA != typeB;

            foreach (var propA in propsA)
            {
                if (opts.ShouldExclude(propA.Name)) continue;

                PropertyInfo? propB = null;
                if (crossType)
                {
                    propB = ReflectionMappingCache.TryGetProperty(typeB, propA.Name);
                    if (propB is null)
                    {
                        if (opts.IgnoreExtraProperties) continue;
                        return false;
                    }
                }

                var valA = propA.GetValue(a);
                var valB = crossType ? propB!.GetValue(b) : propA.GetValue(b);

                if (opts.IgnoreNullProperties && valA is null && valB is null) continue;
                if (!AreEqual(valA, valB, opts, visited, depth + 1, localRules)) return false;
            }
            return true;
        }

        private static bool AreEnumerablesEqual(IEnumerable ea, IEnumerable eb, EqualsOptions opts,
            HashSet<(int, int)> visited, int depth, DeepEqualRuleContainer? localRules)
        {
            var listA = ea.Cast<object?>().ToList();
            var listB = eb.Cast<object?>().ToList();
            if (listA.Count != listB.Count) return false;

            if (opts.IgnoreCollectionOrder)
            {
                var remB = new List<object?>(listB);
                foreach (var itemA in listA)
                {
                    int idx = remB.FindIndex(itemB => AreEqual(itemA, itemB, opts, visited, depth + 1, localRules));
                    if (idx < 0) return false;
                    remB.RemoveAt(idx);
                }
                return remB.Count == 0;
            }

            for (int i = 0; i < listA.Count; i++)
                if (!AreEqual(listA[i], listB[i], opts, visited, depth + 1, localRules)) return false;
            return true;
        }

        private static bool IsNumeric(Type t) =>
            t == typeof(byte) || t == typeof(sbyte) || t == typeof(short) || t == typeof(ushort)
            || t == typeof(int) || t == typeof(uint) || t == typeof(long) || t == typeof(ulong)
            || t == typeof(float) || t == typeof(double) || t == typeof(decimal);
    }

    internal static class ReflectionMappingCache
    {
        private static readonly Dictionary<Type, PropertyInfo[]> _arrayCache = [];
        private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> _dictCache = [];
        private static readonly object _lock = new();

        public static PropertyInfo[] GetReadableProperties(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
            Type type)
        {
            lock (_lock)
            {
                if (_arrayCache.TryGetValue(type, out var cached)) return cached;
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
                    .ToArray();
                _arrayCache[type] = props;
                return props;
            }
        }

        public static PropertyInfo? TryGetProperty(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] 
            Type type, string name)
        {
            lock (_lock)
            {
                if (!_dictCache.TryGetValue(type, out var dict))
                {
                    dict = GetReadableProperties(type)
                        .ToDictionary(p => p.Name, StringComparer.Ordinal);
                    _dictCache[type] = dict;
                }
                dict.TryGetValue(name, out var prop);
                return prop;
            }
        }
    }
}

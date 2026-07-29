using System.Diagnostics;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    public static partial class DictionaryAssertionsExtensions
    {
        /// <summary>Asserts that the dictionary is empty.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> IsEmpty<K, V>(
            this ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> a)
        { a.Link("IsEmpty"); a.Op(a => DictionaryChecks.IsEmpty<K, V>(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the dictionary is not empty.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> IsNotEmpty<K, V>(
            this ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> a)
        { a.Link("IsNotEmpty"); a.Op(a => DictionaryChecks.IsNotEmpty<K, V>(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the dictionary count equals <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> HasCountOf<K, V>(
            this ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> a, int expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("HasCountOf", expr); a.Op(a => DictionaryChecks.HasCount<K, V>(a.GetValue(), expected, a.IsSkipped())); return a; }

        /// <summary>Asserts that the dictionary contains key <paramref name="key"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> ContainsKey<K, V>(
            this ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> a, K key,
            [CallerArgumentExpression(nameof(key))] string? expr = null)
        { a.Link("ContainsKey", expr); a.Op(a => DictionaryChecks.ContainsKey<K, V>(a.GetValue(), key, expr, a.IsSkipped())); return a; }

        /// <summary>Asserts that the dictionary does not contain key <paramref name="key"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> DoesNotContainKey<K, V>(
            this ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> a, K key,
            [CallerArgumentExpression(nameof(key))] string? expr = null)
        { a.Link("DoesNotContainKey", expr); a.Op(a => DictionaryChecks.DoesNotContainKey<K, V>(a.GetValue(), key, expr, a.IsSkipped())); return a; }

        /// <summary>Asserts that the dictionary contains all <paramref name="keys"/>.</summary>
        [GenerateArityOverloads(target: nameof(keys))]
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> ContainsAllKeys<K, V>(
            this ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> a, IEnumerable<K> keys,
            [CallerArgumentExpression(nameof(keys))] string? expr = null)
        {
            var list = keys is IEnumerable<K> r ? r : keys.ToList();
            a.Link("ContainsAllKeys", expr);
            a.Op(a => DictionaryChecks.ContainsAllKeys<K, V>(a.GetValue(), list, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the dictionary contains at least one of <paramref name="keys"/>.</summary>
        [GenerateArityOverloads(target: nameof(keys))]
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> ContainsAnyKey<K, V>(
            this ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> a, IEnumerable<K> keys,
            [CallerArgumentExpression(nameof(keys))] string? expr = null)
        {
            var list = keys is IEnumerable<K> r ? r : keys.ToList();
            a.Link("ContainsAnyKey", expr);
            a.Op(a => DictionaryChecks.ContainsAnyKey<K, V>(a.GetValue(), list, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the dictionary contains none of <paramref name="keys"/>.</summary>
        [GenerateArityOverloads(target: nameof(keys))]
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> ContainsNoneOfKeys<K, V>(
            this ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> a, IEnumerable<K> keys,
            [CallerArgumentExpression(nameof(keys))] string? expr = null)
        {
            var list = keys is IEnumerable<K> r ? r : keys.ToList();
            a.Link("ContainsNoneOfKeys", expr);
            a.Op(a => DictionaryChecks.ContainsNoneOfKeys<K, V>(a.GetValue(), list, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the dictionary contains value <paramref name="value"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> ContainsValue<K, V>(
            this ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> a, V value,
            [CallerArgumentExpression(nameof(value))] string? expr = null)
        { a.Link("ContainsValue", expr); a.Op(a => DictionaryChecks.ContainsValue<K, V>(a.GetValue(), value, expr, a.IsSkipped())); return a; }

        /// <summary>Asserts that the dictionary does not contain value <paramref name="value"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> DoesNotContainValue<K, V>(
            this ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> a, V value,
            [CallerArgumentExpression(nameof(value))] string? expr = null)
        { a.Link("DoesNotContainValue", expr); a.Op(a => DictionaryChecks.DoesNotContainValue<K, V>(a.GetValue(), value, expr, a.IsSkipped())); return a; }

        /// <summary>Asserts that the dictionary contains all <paramref name="values"/>.</summary>
        [GenerateArityOverloads(target: nameof(values))]
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> ContainsAllValues<K, V>(
            this ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> a, IEnumerable<V> values,
            [CallerArgumentExpression(nameof(values))] string? expr = null)
        {
            var list = values is IEnumerable<V> r ? r : values.ToList();
            a.Link("ContainsAllValues", expr);
            a.Op(a => DictionaryChecks.ContainsAllValues<K, V>(a.GetValue(), list, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that all dictionary values satisfy <paramref name="predicate"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> AllValuesSatisfy<K, V>(
            this ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> a, Func<V, bool> predicate,
            [CallerArgumentExpression(nameof(predicate))] string? expr = null)
        { a.Link("AllValuesSatisfy", expr); a.Op(a => DictionaryChecks.AllValuesSatisfy<K, V>(a.GetValue(), predicate, expr, a.IsSkipped())); return a; }

        /// <summary>Asserts that at least one dictionary value satisfies <paramref name="predicate"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> AnyValueSatisfies<K, V>(
            this ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> a, Func<V, bool> predicate,
            [CallerArgumentExpression(nameof(predicate))] string? expr = null)
        { a.Link("AnyValueSatisfies", expr); a.Op(a => DictionaryChecks.AnyValueSatisfies<K, V>(a.GetValue(), predicate, expr, a.IsSkipped())); return a; }

        /// <summary>Asserts that the dictionary contains key <paramref name="key"/> with value <paramref name="expectedValue"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> ContainsEntry<K, V>(
            this ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> a, K key, V expectedValue,
            [CallerArgumentExpression(nameof(key))] string? keyExpr = null,
            [CallerArgumentExpression(nameof(expectedValue))] string? valExpr = null)
        { a.Link("ContainsEntry", keyExpr, valExpr); a.Op(a => DictionaryChecks.ContainsEntry<K, V>(a.GetValue(), key, expectedValue, keyExpr, valExpr, a.IsSkipped())); return a; }

        /// <summary>Asserts that key <paramref name="key"/> has a value satisfying <paramref name="valuePredicate"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> HasKeyWithValue<K, V>(
            this ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> a, K key, Func<V, bool> valuePredicate,
            [CallerArgumentExpression(nameof(key))] string? keyExpr = null,
            [CallerArgumentExpression(nameof(valuePredicate))] string? predicateExpr = null)
        { a.Link("HasKeyWithValue", keyExpr, predicateExpr); a.Op(a => DictionaryChecks.HasKeyWithValue<K, V>(a.GetValue(), key, valuePredicate, keyExpr, predicateExpr, a.IsSkipped())); return a; }

        /// <summary>Asserts that dictionary keys are equivalent to <paramref name="expectedKeys"/>.</summary>
        [GenerateArityOverloads(target: nameof(expectedKeys))]
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> KeysAreEquivalentTo<K, V>(
            this ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> a, IEnumerable<K> expectedKeys,
            [CallerArgumentExpression(nameof(expectedKeys))] string? expr = null)
        {
            var list = expectedKeys is IEnumerable<K> r ? r : expectedKeys.ToList();
            a.Link("KeysAreEquivalentTo", expr);
            a.Op(a => DictionaryChecks.KeysAreEquivalentTo<K, V>(a.GetValue(), list, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that dictionary values are equivalent to <paramref name="expectedValues"/>.</summary>
        [GenerateArityOverloads(target: nameof(expectedValues))]
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> ValuesAreEquivalentTo<K, V>(
            this ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> a, IEnumerable<V> expectedValues,
            [CallerArgumentExpression(nameof(expectedValues))] string? expr = null)
        {
            var list = expectedValues is IEnumerable<V> r ? r : expectedValues.ToList();
            a.Link("ValuesAreEquivalentTo", expr);
            a.Op(a => DictionaryChecks.ValuesAreEquivalentTo<K, V>(a.GetValue(), list, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that dictionary values are distinct.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> HasDistinctValues<K, V>(
            this ValueAssertions<IEnumerable<KeyValuePair<K, V>>?> a)
        { a.Link("HasDistinctValues"); a.Op(a => DictionaryChecks.HasDistinctValues<K, V>(a.GetValue(), a.IsSkipped())); return a; }
    }
}


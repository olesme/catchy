using System.Diagnostics;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    public static partial class AsserterExtensions
    {
        /// <summary>Starts assertions for a <see cref="Memory{T}"/> value.</summary>
        public static ValueAssertions<Memory<T>> That<T>(this Asserter a, Memory<T> value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
            where T : IEquatable<T>
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "That", genericType: typeof(T), valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<Memory<T>>(p, value);
        }

        /// <summary>Starts assertions for a <see cref="ReadOnlyMemory{T}"/> value.</summary>
        public static ValueAssertions<ReadOnlyMemory<T>> That<T>(this Asserter a, ReadOnlyMemory<T> value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
            where T : IEquatable<T>
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "That", genericType: typeof(T), valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<ReadOnlyMemory<T>>(p, value);
        }

        // Span-based entry points are explicit instead of `That(...)` because collection expressions
        // like `[1, 2, 3]` otherwise bind to span/memory overloads and bypass the IEnumerable-based
        // collection assertion surface and generated arity overloads.
        /// <summary>Starts assertions for a <see cref="Span{T}"/> value via explicit span entrypoint.</summary>
        public static ValueAssertions<ReadOnlyMemory<T>> ThatSpan<T>(this Asserter a, Span<T> value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
            where T : IEquatable<T>
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "ThatSpan", genericType: typeof(T), valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<ReadOnlyMemory<T>>(p, value.ToArray());
        }

        // ReadOnlySpan uses the same explicit entry-point rule as Span to avoid conflicting with
        // the normal collection `That(...)` overload family.
        /// <summary>Starts assertions for a <see cref="ReadOnlySpan{T}"/> value via explicit span entrypoint.</summary>
        public static ValueAssertions<ReadOnlyMemory<T>> ThatSpan<T>(this Asserter a, ReadOnlySpan<T> value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
            where T : IEquatable<T>
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "ThatSpan", genericType: typeof(T), valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<ReadOnlyMemory<T>>(p, value.ToArray());
        }
    }

    public static class MemoryAssertionsExtensions
    {
        private static ReadOnlyMemory<T> AsReadOnlyMemory<T>(Memory<T> value) where T : IEquatable<T> => value;
        private static ReadOnlyMemory<T> AsReadOnlyMemory<T>(ReadOnlyMemory<T> value) where T : IEquatable<T> => value;

        [DebuggerHidden, StackTraceHidden]
        private static TAssertions HasLengthCore<TAssertions, T>(TAssertions a, ReadOnlyMemory<T> actual, int expected, string? expr)
            where TAssertions : ValueAssertions
            where T : IEquatable<T>
        { a.Link("HasLength", expr); a.Op(MemoryChecks.HasLength(actual, expected, a.IsSkipped(), expr)); return a; }
        [DebuggerHidden, StackTraceHidden]
        private static TAssertions HasLengthGreaterThanCore<TAssertions, T>(TAssertions a, ReadOnlyMemory<T> actual, int expected, string? expr)
            where TAssertions : ValueAssertions
            where T : IEquatable<T>
        { a.Link("HasLengthGreaterThan", expr); a.Op(MemoryChecks.HasLengthGreaterThan(actual, expected, a.IsSkipped(), expr)); return a; }
        [DebuggerHidden, StackTraceHidden]
        private static TAssertions HasLengthLessThanCore<TAssertions, T>(TAssertions a, ReadOnlyMemory<T> actual, int expected, string? expr)
            where TAssertions : ValueAssertions
            where T : IEquatable<T>
        { a.Link("HasLengthLessThan", expr); a.Op(MemoryChecks.HasLengthLessThan(actual, expected, a.IsSkipped(), expr)); return a; }
        [DebuggerHidden, StackTraceHidden]
        private static TAssertions IsEmptyCore<TAssertions, T>(TAssertions a, ReadOnlyMemory<T> actual)
            where TAssertions : ValueAssertions
            where T : IEquatable<T>
        { a.Link("IsEmpty"); a.Op(MemoryChecks.IsEmpty(actual, a.IsSkipped())); return a; }
        [DebuggerHidden, StackTraceHidden]
        private static TAssertions IsNotEmptyCore<TAssertions, T>(TAssertions a, ReadOnlyMemory<T> actual)
            where TAssertions : ValueAssertions
            where T : IEquatable<T>
        { a.Link("IsNotEmpty"); a.Op(MemoryChecks.IsNotEmpty(actual, a.IsSkipped())); return a; }
        [DebuggerHidden, StackTraceHidden]
        private static TAssertions SequenceEqualsCore<TAssertions, T>(TAssertions a, ReadOnlyMemory<T> actual, ReadOnlyMemory<T> expected, string? expr)
            where TAssertions : ValueAssertions
            where T : IEquatable<T>
        { a.Link("SequenceEquals", expr); a.Op(MemoryChecks.SequenceEquals(actual, expected, a.IsSkipped(), expr)); return a; }
        [DebuggerHidden, StackTraceHidden]
        private static TAssertions DoesNotSequenceEqualCore<TAssertions, T>(TAssertions a, ReadOnlyMemory<T> actual, ReadOnlyMemory<T> expected, string? expr)
            where TAssertions : ValueAssertions
            where T : IEquatable<T>
        { a.Link("DoesNotSequenceEqual", expr); a.Op(MemoryChecks.DoesNotSequenceEqual(actual, expected, a.IsSkipped(), expr)); return a; }
        [DebuggerHidden, StackTraceHidden]
        private static TAssertions ContainsCore<TAssertions, T>(TAssertions a, ReadOnlyMemory<T> actual, T item, string? expr)
            where TAssertions : ValueAssertions
            where T : IEquatable<T>
        { a.Link("Contains", expr); a.Op(MemoryChecks.Contains(actual, item, a.IsSkipped(), expr)); return a; }
        [DebuggerHidden, StackTraceHidden]
        private static TAssertions DoesNotContainCore<TAssertions, T>(TAssertions a, ReadOnlyMemory<T> actual, T item, string? expr)
            where TAssertions : ValueAssertions
            where T : IEquatable<T>
        { a.Link("DoesNotContain", expr); a.Op(MemoryChecks.DoesNotContain(actual, item, a.IsSkipped(), expr)); return a; }
        [DebuggerHidden, StackTraceHidden]
        private static TAssertions StartsWithCore<TAssertions, T>(TAssertions a, ReadOnlyMemory<T> actual, ReadOnlyMemory<T> prefix, string? expr)
            where TAssertions : ValueAssertions
            where T : IEquatable<T>
        { a.Link("StartsWith", expr); a.Op(MemoryChecks.StartsWith(actual, prefix, a.IsSkipped(), expr)); return a; }
        [DebuggerHidden, StackTraceHidden]
        private static TAssertions DoesNotStartWithCore<TAssertions, T>(TAssertions a, ReadOnlyMemory<T> actual, ReadOnlyMemory<T> prefix, string? expr)
            where TAssertions : ValueAssertions
            where T : IEquatable<T>
        { a.Link("DoesNotStartWith", expr); a.Op(MemoryChecks.DoesNotStartWith(actual, prefix, a.IsSkipped(), expr)); return a; }
        [DebuggerHidden, StackTraceHidden]
        private static TAssertions EndsWithCore<TAssertions, T>(TAssertions a, ReadOnlyMemory<T> actual, ReadOnlyMemory<T> suffix, string? expr)
            where TAssertions : ValueAssertions
            where T : IEquatable<T>
        { a.Link("EndsWith", expr); a.Op(MemoryChecks.EndsWith(actual, suffix, a.IsSkipped(), expr)); return a; }
        [DebuggerHidden, StackTraceHidden]
        private static TAssertions DoesNotEndWithCore<TAssertions, T>(TAssertions a, ReadOnlyMemory<T> actual, ReadOnlyMemory<T> suffix, string? expr)
            where TAssertions : ValueAssertions
            where T : IEquatable<T>
        { a.Link("DoesNotEndWith", expr); a.Op(MemoryChecks.DoesNotEndWith(actual, suffix, a.IsSkipped(), expr)); return a; }
        [DebuggerHidden, StackTraceHidden]
        private static TAssertions SliceEqualsCore<TAssertions, T>(TAssertions a, ReadOnlyMemory<T> actual, int start, int length, ReadOnlyMemory<T> expected,
            string? startExpr, string? lengthExpr, string? expectedExpr)
            where TAssertions : ValueAssertions
            where T : IEquatable<T>
        { a.Link("SliceEquals", startExpr, lengthExpr, expectedExpr); a.Op(MemoryChecks.SliceEquals(actual, start, length, expected, a.IsSkipped(), startExpr, lengthExpr, expectedExpr)); return a; }

        /// <summary>Asserts that the memory length equals <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Memory<T>> HasLength<T>(this ValueAssertions<Memory<T>> a, int expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null) where T : IEquatable<T>
            => HasLengthCore(a, AsReadOnlyMemory(a.GetValue()), expected, expr);

        /// <summary>Asserts that the memory length equals <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ReadOnlyMemory<T>> HasLength<T>(this ValueAssertions<ReadOnlyMemory<T>> a, int expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null) where T : IEquatable<T>
            => HasLengthCore(a, AsReadOnlyMemory(a.GetValue()), expected, expr);

        /// <summary>Asserts that the memory length is greater than <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Memory<T>> HasLengthGreaterThan<T>(this ValueAssertions<Memory<T>> a, int expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null) where T : IEquatable<T>
            => HasLengthGreaterThanCore(a, AsReadOnlyMemory(a.GetValue()), expected, expr);

        /// <summary>Asserts that the memory length is greater than <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ReadOnlyMemory<T>> HasLengthGreaterThan<T>(this ValueAssertions<ReadOnlyMemory<T>> a, int expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null) where T : IEquatable<T>
            => HasLengthGreaterThanCore(a, AsReadOnlyMemory(a.GetValue()), expected, expr);

        /// <summary>Asserts that the memory length is less than <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Memory<T>> HasLengthLessThan<T>(this ValueAssertions<Memory<T>> a, int expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null) where T : IEquatable<T>
            => HasLengthLessThanCore(a, AsReadOnlyMemory(a.GetValue()), expected, expr);

        /// <summary>Asserts that the memory length is less than <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ReadOnlyMemory<T>> HasLengthLessThan<T>(this ValueAssertions<ReadOnlyMemory<T>> a, int expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null) where T : IEquatable<T>
            => HasLengthLessThanCore(a, AsReadOnlyMemory(a.GetValue()), expected, expr);

        /// <summary>Asserts that the memory is empty.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Memory<T>> IsEmpty<T>(this ValueAssertions<Memory<T>> a) where T : IEquatable<T>
            => IsEmptyCore(a, AsReadOnlyMemory(a.GetValue()));

        /// <summary>Asserts that the memory is empty.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ReadOnlyMemory<T>> IsEmpty<T>(this ValueAssertions<ReadOnlyMemory<T>> a) where T : IEquatable<T>
            => IsEmptyCore(a, AsReadOnlyMemory(a.GetValue()));

        /// <summary>Asserts that the memory is not empty.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Memory<T>> IsNotEmpty<T>(this ValueAssertions<Memory<T>> a) where T : IEquatable<T>
            => IsNotEmptyCore(a, AsReadOnlyMemory(a.GetValue()));

        /// <summary>Asserts that the memory is not empty.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ReadOnlyMemory<T>> IsNotEmpty<T>(this ValueAssertions<ReadOnlyMemory<T>> a) where T : IEquatable<T>
            => IsNotEmptyCore(a, AsReadOnlyMemory(a.GetValue()));

        /// <summary>Asserts that the memory sequence equals <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Memory<T>> SequenceEquals<T>(this ValueAssertions<Memory<T>> a, ReadOnlyMemory<T> expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null) where T : IEquatable<T>
            => SequenceEqualsCore(a, AsReadOnlyMemory(a.GetValue()), expected, expr);

        /// <summary>Asserts that the memory sequence equals <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ReadOnlyMemory<T>> SequenceEquals<T>(this ValueAssertions<ReadOnlyMemory<T>> a, ReadOnlyMemory<T> expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null) where T : IEquatable<T>
            => SequenceEqualsCore(a, AsReadOnlyMemory(a.GetValue()), expected, expr);

        /// <summary>Asserts that the memory sequence equals <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Memory<T>> SequenceEquals<T>(this ValueAssertions<Memory<T>> a, T[] expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null) where T : IEquatable<T>
            => a.SequenceEquals(expected.AsMemory(), expr);

        /// <summary>Asserts that the memory sequence equals <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ReadOnlyMemory<T>> SequenceEquals<T>(this ValueAssertions<ReadOnlyMemory<T>> a, T[] expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null) where T : IEquatable<T>
            => a.SequenceEquals(expected.AsMemory(), expr);

        /// <summary>Asserts that the memory sequence does not equal <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Memory<T>> DoesNotSequenceEqual<T>(this ValueAssertions<Memory<T>> a, ReadOnlyMemory<T> expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null) where T : IEquatable<T>
            => DoesNotSequenceEqualCore(a, AsReadOnlyMemory(a.GetValue()), expected, expr);

        /// <summary>Asserts that the memory sequence does not equal <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ReadOnlyMemory<T>> DoesNotSequenceEqual<T>(this ValueAssertions<ReadOnlyMemory<T>> a, ReadOnlyMemory<T> expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null) where T : IEquatable<T>
            => DoesNotSequenceEqualCore(a, AsReadOnlyMemory(a.GetValue()), expected, expr);

        /// <summary>Asserts that the memory contains <paramref name="item"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Memory<T>> Contains<T>(this ValueAssertions<Memory<T>> a, T item,
            [CallerArgumentExpression(nameof(item))] string? expr = null) where T : IEquatable<T>
            => ContainsCore(a, AsReadOnlyMemory(a.GetValue()), item, expr);

        /// <summary>Asserts that the memory contains <paramref name="item"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ReadOnlyMemory<T>> Contains<T>(this ValueAssertions<ReadOnlyMemory<T>> a, T item,
            [CallerArgumentExpression(nameof(item))] string? expr = null) where T : IEquatable<T>
            => ContainsCore(a, AsReadOnlyMemory(a.GetValue()), item, expr);

        /// <summary>Asserts that the memory does not contain <paramref name="item"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Memory<T>> DoesNotContain<T>(this ValueAssertions<Memory<T>> a, T item,
            [CallerArgumentExpression(nameof(item))] string? expr = null) where T : IEquatable<T>
            => DoesNotContainCore(a, AsReadOnlyMemory(a.GetValue()), item, expr);

        /// <summary>Asserts that the memory does not contain <paramref name="item"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ReadOnlyMemory<T>> DoesNotContain<T>(this ValueAssertions<ReadOnlyMemory<T>> a, T item,
            [CallerArgumentExpression(nameof(item))] string? expr = null) where T : IEquatable<T>
            => DoesNotContainCore(a, AsReadOnlyMemory(a.GetValue()), item, expr);

        /// <summary>Asserts that the memory starts with <paramref name="prefix"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Memory<T>> StartsWith<T>(this ValueAssertions<Memory<T>> a, ReadOnlyMemory<T> prefix,
            [CallerArgumentExpression(nameof(prefix))] string? expr = null) where T : IEquatable<T>
            => StartsWithCore(a, AsReadOnlyMemory(a.GetValue()), prefix, expr);

        /// <summary>Asserts that the memory starts with <paramref name="prefix"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ReadOnlyMemory<T>> StartsWith<T>(this ValueAssertions<ReadOnlyMemory<T>> a, ReadOnlyMemory<T> prefix,
            [CallerArgumentExpression(nameof(prefix))] string? expr = null) where T : IEquatable<T>
            => StartsWithCore(a, AsReadOnlyMemory(a.GetValue()), prefix, expr);

        /// <summary>Asserts that the memory starts with <paramref name="prefix"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Memory<T>> StartsWith<T>(this ValueAssertions<Memory<T>> a, T[] prefix,
            [CallerArgumentExpression(nameof(prefix))] string? expr = null) where T : IEquatable<T>
            => a.StartsWith(prefix.AsMemory(), expr);

        /// <summary>Asserts that the memory starts with <paramref name="prefix"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ReadOnlyMemory<T>> StartsWith<T>(this ValueAssertions<ReadOnlyMemory<T>> a, T[] prefix,
            [CallerArgumentExpression(nameof(prefix))] string? expr = null) where T : IEquatable<T>
            => a.StartsWith(prefix.AsMemory(), expr);

        /// <summary>Asserts that the memory does not start with <paramref name="prefix"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Memory<T>> DoesNotStartWith<T>(this ValueAssertions<Memory<T>> a, ReadOnlyMemory<T> prefix,
            [CallerArgumentExpression(nameof(prefix))] string? expr = null) where T : IEquatable<T>
            => DoesNotStartWithCore(a, AsReadOnlyMemory(a.GetValue()), prefix, expr);

        /// <summary>Asserts that the memory does not start with <paramref name="prefix"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ReadOnlyMemory<T>> DoesNotStartWith<T>(this ValueAssertions<ReadOnlyMemory<T>> a, ReadOnlyMemory<T> prefix,
            [CallerArgumentExpression(nameof(prefix))] string? expr = null) where T : IEquatable<T>
            => DoesNotStartWithCore(a, AsReadOnlyMemory(a.GetValue()), prefix, expr);

        /// <summary>Asserts that the memory ends with <paramref name="suffix"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Memory<T>> EndsWith<T>(this ValueAssertions<Memory<T>> a, ReadOnlyMemory<T> suffix,
            [CallerArgumentExpression(nameof(suffix))] string? expr = null) where T : IEquatable<T>
            => EndsWithCore(a, AsReadOnlyMemory(a.GetValue()), suffix, expr);

        /// <summary>Asserts that the memory ends with <paramref name="suffix"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ReadOnlyMemory<T>> EndsWith<T>(this ValueAssertions<ReadOnlyMemory<T>> a, ReadOnlyMemory<T> suffix,
            [CallerArgumentExpression(nameof(suffix))] string? expr = null) where T : IEquatable<T>
            => EndsWithCore(a, AsReadOnlyMemory(a.GetValue()), suffix, expr);

        /// <summary>Asserts that the memory ends with <paramref name="suffix"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Memory<T>> EndsWith<T>(this ValueAssertions<Memory<T>> a, T[] suffix,
            [CallerArgumentExpression(nameof(suffix))] string? expr = null) where T : IEquatable<T>
            => a.EndsWith(suffix.AsMemory(), expr);

        /// <summary>Asserts that the memory ends with <paramref name="suffix"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ReadOnlyMemory<T>> EndsWith<T>(this ValueAssertions<ReadOnlyMemory<T>> a, T[] suffix,
            [CallerArgumentExpression(nameof(suffix))] string? expr = null) where T : IEquatable<T>
            => a.EndsWith(suffix.AsMemory(), expr);

        /// <summary>Asserts that the memory does not end with <paramref name="suffix"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Memory<T>> DoesNotEndWith<T>(this ValueAssertions<Memory<T>> a, ReadOnlyMemory<T> suffix,
            [CallerArgumentExpression(nameof(suffix))] string? expr = null) where T : IEquatable<T>
            => DoesNotEndWithCore(a, AsReadOnlyMemory(a.GetValue()), suffix, expr);

        /// <summary>Asserts that the memory does not end with <paramref name="suffix"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ReadOnlyMemory<T>> DoesNotEndWith<T>(this ValueAssertions<ReadOnlyMemory<T>> a, ReadOnlyMemory<T> suffix,
            [CallerArgumentExpression(nameof(suffix))] string? expr = null) where T : IEquatable<T>
            => DoesNotEndWithCore(a, AsReadOnlyMemory(a.GetValue()), suffix, expr);

        /// <summary>Asserts that a slice of the memory equals <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Memory<T>> SliceEquals<T>(this ValueAssertions<Memory<T>> a, int start, int length, ReadOnlyMemory<T> expected,
            [CallerArgumentExpression(nameof(start))] string? startExpr = null,
            [CallerArgumentExpression(nameof(length))] string? lengthExpr = null,
            [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null) where T : IEquatable<T>
            => SliceEqualsCore(a, AsReadOnlyMemory(a.GetValue()), start, length, expected, startExpr, lengthExpr, expectedExpr);

        /// <summary>Asserts that a slice of the memory equals <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ReadOnlyMemory<T>> SliceEquals<T>(this ValueAssertions<ReadOnlyMemory<T>> a, int start, int length, ReadOnlyMemory<T> expected,
            [CallerArgumentExpression(nameof(start))] string? startExpr = null,
            [CallerArgumentExpression(nameof(length))] string? lengthExpr = null,
            [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null) where T : IEquatable<T>
            => SliceEqualsCore(a, AsReadOnlyMemory(a.GetValue()), start, length, expected, startExpr, lengthExpr, expectedExpr);

        /// <summary>Asserts that a slice of the memory equals <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Memory<T>> SliceEquals<T>(this ValueAssertions<Memory<T>> a, int start, int length, T[] expected,
            [CallerArgumentExpression(nameof(start))] string? startExpr = null,
            [CallerArgumentExpression(nameof(length))] string? lengthExpr = null,
            [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null) where T : IEquatable<T>
            => a.SliceEquals(start, length, expected.AsMemory(), startExpr, lengthExpr, expectedExpr);

        /// <summary>Asserts that a slice of the memory equals <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ReadOnlyMemory<T>> SliceEquals<T>(this ValueAssertions<ReadOnlyMemory<T>> a, int start, int length, T[] expected,
            [CallerArgumentExpression(nameof(start))] string? startExpr = null,
            [CallerArgumentExpression(nameof(length))] string? lengthExpr = null,
            [CallerArgumentExpression(nameof(expected))] string? expectedExpr = null) where T : IEquatable<T>
            => a.SliceEquals(start, length, expected.AsMemory(), startExpr, lengthExpr, expectedExpr);
    }
}

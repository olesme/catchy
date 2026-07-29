namespace Catchy.Sdk
{
    public static class MemoryChecks
    {
        static string Fmt(object? v, string? expr = null) => ExprFormat.Inline(v, expr);

        public static CheckOperation HasLength<T>(ReadOnlyMemory<T> actual, int expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual.Length == expected,
                () => $"Expected memory to have length {Fmt(expected, expr)}, but was {actual.Length}",
                isSkipped);

        public static CheckOperation HasLengthGreaterThan<T>(ReadOnlyMemory<T> actual, int expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual.Length > expected,
                () => $"Expected memory to have length > {Fmt(expected, expr)}, but was {actual.Length}",
                isSkipped);

        public static CheckOperation HasLengthLessThan<T>(ReadOnlyMemory<T> actual, int expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual.Length < expected,
                () => $"Expected memory to have length < {Fmt(expected, expr)}, but was {actual.Length}",
                isSkipped);

        public static CheckOperation IsEmpty<T>(ReadOnlyMemory<T> actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.IsEmpty,
                () => $"Expected memory to be empty, but had length {actual.Length}",
                isSkipped);

        public static CheckOperation IsNotEmpty<T>(ReadOnlyMemory<T> actual, bool isSkipped)
            => CheckOperation.Sync(
                () => !actual.IsEmpty,
                () => "Expected memory to be non-empty",
                isSkipped);

        public static CheckOperation SequenceEquals<T>(ReadOnlyMemory<T> actual, ReadOnlyMemory<T> expected, bool isSkipped, string? expr = null)
            where T : IEquatable<T>
            => CheckOperation.Sync(
                () => actual.Span.SequenceEqual(expected.Span),
                () => $"Expected memory to equal {expr ?? "expected"} (actual={actual.Length}, expected={expected.Length})",
                isSkipped);

        public static CheckOperation DoesNotSequenceEqual<T>(ReadOnlyMemory<T> actual, ReadOnlyMemory<T> expected, bool isSkipped, string? expr = null)
            where T : IEquatable<T>
            => CheckOperation.Sync(
                () => !actual.Span.SequenceEqual(expected.Span),
                () => $"Expected memory not to equal {expr ?? "expected"}",
                isSkipped);

        public static CheckOperation Contains<T>(ReadOnlyMemory<T> actual, T item, bool isSkipped, string? expr = null)
            where T : IEquatable<T>
            => CheckOperation.Sync(
                () => MemoryExtensions.IndexOf(actual.Span, item) >= 0,
                () => $"Expected memory to contain {Fmt(item, expr)}",
                isSkipped);

        public static CheckOperation DoesNotContain<T>(ReadOnlyMemory<T> actual, T item, bool isSkipped, string? expr = null)
            where T : IEquatable<T>
            => CheckOperation.Sync(
                () => MemoryExtensions.IndexOf(actual.Span, item) < 0,
                () => $"Expected memory not to contain {Fmt(item, expr)}",
                isSkipped);

        public static CheckOperation StartsWith<T>(ReadOnlyMemory<T> actual, ReadOnlyMemory<T> prefix, bool isSkipped, string? expr = null)
            where T : IEquatable<T>
            => CheckOperation.Sync(
                () => actual.Span.StartsWith(prefix.Span),
                () => $"Expected memory to start with {expr ?? "prefix"}",
                isSkipped);

        public static CheckOperation DoesNotStartWith<T>(ReadOnlyMemory<T> actual, ReadOnlyMemory<T> prefix, bool isSkipped, string? expr = null)
            where T : IEquatable<T>
            => CheckOperation.Sync(
                () => !actual.Span.StartsWith(prefix.Span),
                () => $"Expected memory not to start with {expr ?? "prefix"}",
                isSkipped);

        public static CheckOperation EndsWith<T>(ReadOnlyMemory<T> actual, ReadOnlyMemory<T> suffix, bool isSkipped, string? expr = null)
            where T : IEquatable<T>
            => CheckOperation.Sync(
                () => actual.Span.EndsWith(suffix.Span),
                () => $"Expected memory to end with {expr ?? "suffix"}",
                isSkipped);

        public static CheckOperation DoesNotEndWith<T>(ReadOnlyMemory<T> actual, ReadOnlyMemory<T> suffix, bool isSkipped, string? expr = null)
            where T : IEquatable<T>
            => CheckOperation.Sync(
                () => !actual.Span.EndsWith(suffix.Span),
                () => $"Expected memory not to end with {expr ?? "suffix"}",
                isSkipped);

        public static CheckOperation SliceEquals<T>(ReadOnlyMemory<T> actual, int start, int length, ReadOnlyMemory<T> expected, bool isSkipped,
            string? startExpr = null, string? lengthExpr = null, string? expectedExpr = null)
            where T : IEquatable<T>
            => CheckOperation.Sync(
                () => start + length <= actual.Length && actual.Slice(start, length).Span.SequenceEqual(expected.Span),
                () => $"Expected memory[{Fmt(start, startExpr)}..{start + length}] to equal {expectedExpr ?? "expected"}",
                isSkipped);
    }
}

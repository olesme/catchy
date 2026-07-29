using System.IO;

namespace Catchy.Sdk
{
    public static class StreamChecks
    {
        public static CheckOperation IsReadable(Stream? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.CanRead,
                () => $"Expected stream to be readable, but CanRead={actual?.CanRead.ToString() ?? "null"}",
                isSkipped);

        public static CheckOperation IsNotReadable(Stream? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && !actual.CanRead,
                () => $"Expected stream not to be readable",
                isSkipped);

        public static CheckOperation IsWritable(Stream? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.CanWrite,
                () => $"Expected stream to be writable, but CanWrite={actual?.CanWrite.ToString() ?? "null"}",
                isSkipped);

        public static CheckOperation IsNotWritable(Stream? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && !actual.CanWrite,
                () => $"Expected stream not to be writable",
                isSkipped);

        public static CheckOperation IsSeekable(Stream? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.CanSeek,
                () => $"Expected stream to be seekable, but CanSeek={actual?.CanSeek.ToString() ?? "null"}",
                isSkipped);

        public static CheckOperation IsNotSeekable(Stream? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && !actual.CanSeek,
                () => $"Expected stream not to be seekable",
                isSkipped);

        public static CheckOperation IsEmpty(Stream? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.CanSeek && actual.Length == 0,
                () => $"Expected stream to be empty, but had {actual?.Length ?? 0} bytes",
                isSkipped);

        public static CheckOperation IsNotEmpty(Stream? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.CanSeek && actual.Length > 0,
                () => "Expected stream to be non-empty",
                isSkipped);

        public static CheckOperation HasLength(Stream? actual, long expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && actual.CanSeek && actual.Length == expected,
                () => $"Expected stream to have length {ExprFormat.Inline(expected, expr)}, but was {actual?.Length.ToString() ?? "null"}",
                isSkipped);

        public static CheckOperation HasLengthGreaterThan(Stream? actual, long expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && actual.CanSeek && actual.Length > expected,
                () => $"Expected stream to have length > {ExprFormat.Inline(expected, expr)}, but was {actual?.Length.ToString() ?? "null"}",
                isSkipped);

        public static CheckOperation HasLengthLessThan(Stream? actual, long expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && actual.CanSeek && actual.Length < expected,
                () => $"Expected stream to have length < {ExprFormat.Inline(expected, expr)}, but was {actual?.Length.ToString() ?? "null"}",
                isSkipped);

        public static CheckOperation HasLengthInRange(Stream? actual, long min, long max, bool isSkipped, string? minExpr = null, string? maxExpr = null)
            => CheckOperation.Sync(
                () => actual is not null && actual.CanSeek && actual.Length >= min && actual.Length <= max,
                () => $"Expected stream to have length in [{ExprFormat.Inline(min, minExpr)}, {ExprFormat.Inline(max, maxExpr)}], but was {actual?.Length.ToString() ?? "null"}",
                isSkipped);

        public static CheckOperation IsAtStart(Stream? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.CanSeek && actual.Position == 0,
                () => $"Expected stream to be at position 0, but was at {actual?.Position.ToString() ?? "null"}",
                isSkipped);

        public static CheckOperation IsNotAtStart(Stream? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.CanSeek && actual.Position != 0,
                () => $"Expected stream not to be at start",
                isSkipped);

        public static CheckOperation IsAtEnd(Stream? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.CanSeek && actual.Position == actual.Length,
                () => $"Expected stream to be at end (position={actual?.Position}, length={actual?.Length})",
                isSkipped);

        public static CheckOperation IsNotAtEnd(Stream? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.CanSeek && actual.Position != actual.Length,
                () => $"Expected stream not to be at end",
                isSkipped);

        public static CheckOperation HasPosition(Stream? actual, long expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && actual.CanSeek && actual.Position == expected,
                () => $"Expected stream to be at position {ExprFormat.Inline(expected, expr)}, but was at {actual?.Position.ToString() ?? "null"}",
                isSkipped);

        public static CheckOperation CanTimeout(Stream? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.CanTimeout,
                () => "Expected stream to support timeouts",
                isSkipped);

        public static CheckOperation CannotTimeout(Stream? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && !actual.CanTimeout,
                () => "Expected stream not to support timeouts",
                isSkipped);

        public static CheckOperation CanRead(Stream? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.CanRead,
                () => "Expected stream to be readable",
                isSkipped);

        public static CheckOperation CanWrite(Stream? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.CanWrite,
                () => "Expected stream to be writable",
                isSkipped);

        public static CheckOperation CanSeek(Stream? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.CanSeek,
                () => "Expected stream to be seekable",
                isSkipped);

        public static CheckOperation IsClosed(Stream? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && !actual.CanRead && !actual.CanWrite,
                () => "Expected stream to be closed",
                isSkipped);
    }
}

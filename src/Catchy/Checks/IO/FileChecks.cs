using System;
using System.IO;

namespace Catchy.Sdk
{
    public static class FileChecks
    {
        public static CheckOperation Exists(string? path, bool isSkipped)
            => CheckOperation.Sync(
                () => path is not null && File.Exists(path),
                () => $"Expected file '{path ?? "null"}' to exist",
                isSkipped);

        public static CheckOperation DoesNotExist(string? path, bool isSkipped)
            => CheckOperation.Sync(
                () => path is null || !File.Exists(path),
                () => $"Expected file '{path}' not to exist",
                isSkipped);

        public static CheckOperation HasName(string? path, string expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => path is not null && string.Equals(Path.GetFileName(path), expected, StringComparison.OrdinalIgnoreCase),
                () => $"Expected file name to be {ExprFormat.Inline(expected, expr)}, but was '{(path is null ? "null" : Path.GetFileName(path))}'",
                isSkipped);

        public static CheckOperation HasExtension(string? path, string expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => path is not null && string.Equals(Path.GetExtension(path), expected, StringComparison.OrdinalIgnoreCase),
                () => $"Expected file extension to be {ExprFormat.Inline(expected, expr)}, but was '{(path is null ? "null" : Path.GetExtension(path))}'",
                isSkipped);

        public static CheckOperation HasSize(string? path, long expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => path is not null && File.Exists(path) && new FileInfo(path).Length == expected,
                () => $"Expected file size to be {ExprFormat.Inline(expected, expr)} bytes, but was {(path is not null && File.Exists(path) ? new FileInfo(path).Length.ToString() : "N/A")}",
                isSkipped);

        public static CheckOperation HasSizeGreaterThan(string? path, long expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => path is not null && File.Exists(path) && new FileInfo(path).Length > expected,
                () => $"Expected file size to be > {ExprFormat.Inline(expected, expr)} bytes",
                isSkipped);

        public static CheckOperation HasSizeLessThan(string? path, long expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => path is not null && File.Exists(path) && new FileInfo(path).Length < expected,
                () => $"Expected file size to be < {ExprFormat.Inline(expected, expr)} bytes",
                isSkipped);

        public static CheckOperation HasSizeInRange(string? path, long min, long max, bool isSkipped, string? minExpr = null, string? maxExpr = null)
        {
            return CheckOperation.Sync(() =>
            {
                if (path is null || !File.Exists(path)) return false;
                var len = new FileInfo(path).Length;
                return len >= min && len <= max;
            },
            () => $"Expected file size to be in [{ExprFormat.Inline(min, minExpr)}, {ExprFormat.Inline(max, maxExpr)}] bytes",
            isSkipped);
        }

        public static CheckOperation IsInDirectory(string? path, string directoryPath, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => path is not null && string.Equals(Path.GetFullPath(Path.GetDirectoryName(path) ?? ""), Path.GetFullPath(directoryPath), StringComparison.OrdinalIgnoreCase),
                () => $"Expected file to be in directory {ExprFormat.Inline(directoryPath, expr)}",
                isSkipped);

        public static CheckOperation IsNotInDirectory(string? path, string directoryPath, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => path is null || !string.Equals(Path.GetFullPath(Path.GetDirectoryName(path) ?? ""), Path.GetFullPath(directoryPath), StringComparison.OrdinalIgnoreCase),
                () => $"Expected file not to be in directory {ExprFormat.Inline(directoryPath, expr)}",
                isSkipped);

        public static CheckOperation IsReadonly(string? path, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => path is not null && File.Exists(path) && new FileInfo(path).IsReadOnly,
                () => $"Expected file to be readonly {ExprFormat.Inline(expr)}",
                isSkipped);
    }
}

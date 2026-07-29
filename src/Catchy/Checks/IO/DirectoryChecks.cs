using System;
using System.IO;
using System.Linq;

namespace Catchy.Sdk
{
    public static class DirectoryChecks
    {
        public static CheckOperation Exists(string? path, bool isSkipped)
            => CheckOperation.Sync(
                () => path is not null && Directory.Exists(path),
                () => $"Expected directory '{path ?? "null"}' to exist",
                isSkipped);

        public static CheckOperation DoesNotExist(string? path, bool isSkipped)
            => CheckOperation.Sync(
                () => path is null || !Directory.Exists(path),
                () => $"Expected directory '{path}' not to exist",
                isSkipped);

        public static CheckOperation IsEmpty(string? path, bool isSkipped)
            => CheckOperation.Sync(
                () => path is not null && Directory.Exists(path) && !Directory.EnumerateFileSystemEntries(path).Any(),
                () => $"Expected directory '{path}' to be empty",
                isSkipped);

        public static CheckOperation IsNotEmpty(string? path, bool isSkipped)
            => CheckOperation.Sync(
                () => path is not null && Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any(),
                () => $"Expected directory '{path}' to be non-empty",
                isSkipped);

        public static CheckOperation HasFile(string? path, string fileName, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => path is not null && Directory.Exists(path) && File.Exists(Path.Combine(path, fileName)),
                () => $"Expected directory to contain file {ExprFormat.Inline(fileName, expr)}",
                isSkipped);

        public static CheckOperation DoesNotHaveFile(string? path, string fileName, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => path is null || !Directory.Exists(path) || !File.Exists(Path.Combine(path, fileName)),
                () => $"Expected directory not to contain file {ExprFormat.Inline(fileName, expr)}",
                isSkipped);

        public static CheckOperation HasSubdirectory(string? path, string name, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => path is not null && Directory.Exists(path) && Directory.Exists(Path.Combine(path, name)),
                () => $"Expected directory to contain subdirectory {ExprFormat.Inline(name, expr)}",
                isSkipped);

        public static CheckOperation DoesNotHaveSubdirectory(string? path, string name, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => path is null || !Directory.Exists(path) || !Directory.Exists(Path.Combine(path, name)),
                () => $"Expected directory not to contain subdirectory {ExprFormat.Inline(name, expr)}",
                isSkipped);

        public static CheckOperation HasFileCount(string? path, int expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => path is not null && Directory.Exists(path) && Directory.GetFiles(path).Length == expected,
                () => $"Expected directory to have {ExprFormat.Inline(expected, expr)} file(s), but had {(path is not null && Directory.Exists(path) ? Directory.GetFiles(path).Length.ToString() : "N/A")}",
                isSkipped);

        public static CheckOperation HasFileCountGreaterThan(string? path, int expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => path is not null && Directory.Exists(path) && Directory.GetFiles(path).Length > expected,
                () => $"Expected directory to have > {ExprFormat.Inline(expected, expr)} file(s)",
                isSkipped);

        public static CheckOperation HasFileCountLessThan(string? path, int expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => path is not null && Directory.Exists(path) && Directory.GetFiles(path).Length < expected,
                () => $"Expected directory to have < {ExprFormat.Inline(expected, expr)} file(s)",
                isSkipped);

        public static CheckOperation HasName(string? path, string expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => path is not null && string.Equals(new DirectoryInfo(path).Name, expected, StringComparison.OrdinalIgnoreCase),
                () => $"Expected directory name to be {ExprFormat.Inline(expected, expr)}, but was '{(path is null ? "null" : new DirectoryInfo(path).Name)}'",
                isSkipped);

        public static CheckOperation HasFileMatching(string? path, string searchPattern, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => path is not null && Directory.Exists(path) && Directory.GetFiles(path, searchPattern).Length > 0,
                () => $"Expected directory to contain file matching {ExprFormat.Inline(searchPattern, expr)}",
                isSkipped);

        public static CheckOperation DoesNotHaveFileMatching(string? path, string searchPattern, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => path is null || !Directory.Exists(path) || Directory.GetFiles(path, searchPattern).Length == 0,
                () => $"Expected directory not to contain file matching {ExprFormat.Inline(searchPattern, expr)}",
                isSkipped);

        public static CheckOperation IsInDirectory(string? path, string parentPath, bool isSkipped, string? expr = null)
        {
            return CheckOperation.Sync(() =>
            {
                if (path is null) return false;
                var info = new DirectoryInfo(path);
                var parent = info.Parent?.FullName ?? "";
                return string.Equals(Path.GetFullPath(parent), Path.GetFullPath(parentPath), StringComparison.OrdinalIgnoreCase);
            },
            () => $"Expected directory to be inside {ExprFormat.Inline(parentPath, expr)}",
            isSkipped);
        }

        public static CheckOperation IsNotInDirectory(string? path, string parentPath, bool isSkipped, string? expr = null)
        {
            return CheckOperation.Sync(() =>
            {
                if (path is null) return true;
                var info = new DirectoryInfo(path);
                var parent = info.Parent?.FullName ?? "";
                return !string.Equals(Path.GetFullPath(parent), Path.GetFullPath(parentPath), StringComparison.OrdinalIgnoreCase);
            },
            () => $"Expected directory not to be inside {ExprFormat.Inline(parentPath, expr)}",
            isSkipped);
        }
    }
}

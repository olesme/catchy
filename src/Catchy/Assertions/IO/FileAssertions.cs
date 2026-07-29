using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    public static partial class AsserterExtensions
    {
        /// <summary>Starts file assertions from a file path.</summary>
        public static ValueAssertions<FileInfo?> ThatFile(this Asserter a, string? path,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(path))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "ThatFile", valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<FileInfo?>(p, path is null ? null : new FileInfo(path));
        }

        /// <summary>Starts file assertions from a <see cref="FileInfo"/> instance.</summary>
        public static ValueAssertions<FileInfo?> ThatFile(this Asserter a, FileInfo? fileInfo,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(fileInfo))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "ThatFile", valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<FileInfo?>(p, fileInfo);
        }

        /// <summary>Starts assertions for a <see cref="FileInfo"/> value.</summary>
        public static ValueAssertions<FileInfo?> That(this Asserter a, FileInfo? fileInfo,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(fileInfo))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<FileInfo?>(p, fileInfo);
        }
    }

    /// <summary>Provides fluent assertions and projections for <see cref="FileInfo"/> values.</summary>
    public static class FileAssertExtensions
    {
        /// <summary>Asserts that the file exists on the file system.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<FileInfo?> Exists(this ValueAssertions<FileInfo?> a)
        { a.Link("Exists"); a.Op(a => FileChecks.Exists(a.GetValue()?.FullName, a.IsSkipped())); return a; }

        /// <summary>Asserts that the file does not exist on the file system.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<FileInfo?> DoesNotExist(this ValueAssertions<FileInfo?> a)
        { a.Link("DoesNotExist"); a.Op(a => FileChecks.DoesNotExist(a.GetValue()?.FullName, a.IsSkipped())); return a; }

        /// <summary>Asserts that the file has the given name.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<FileInfo?> HasName(this ValueAssertions<FileInfo?> a, string expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("HasName", expr); a.Op(a => FileChecks.HasName(a.GetValue()?.FullName, expected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the file has the given extension.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<FileInfo?> HasExtension(this ValueAssertions<FileInfo?> a, string expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("HasExtension", expr); a.Op(a => FileChecks.HasExtension(a.GetValue()?.FullName, expected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the file has exactly the given size in bytes.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<FileInfo?> HasSize(this ValueAssertions<FileInfo?> a, long expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("HasSize", expr); a.Op(a => FileChecks.HasSize(a.GetValue()?.FullName, expected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the file size is greater than <paramref name="expected"/> bytes.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<FileInfo?> HasSizeGreaterThan(this ValueAssertions<FileInfo?> a, long expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("HasSizeGreaterThan", expr); a.Op(a => FileChecks.HasSizeGreaterThan(a.GetValue()?.FullName, expected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the file size is less than <paramref name="expected"/> bytes.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<FileInfo?> HasSizeLessThan(this ValueAssertions<FileInfo?> a, long expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("HasSizeLessThan", expr); a.Op(a => FileChecks.HasSizeLessThan(a.GetValue()?.FullName, expected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the file size is between <paramref name="min"/> and <paramref name="max"/> bytes (inclusive).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<FileInfo?> HasSizeInRange(this ValueAssertions<FileInfo?> a, long min, long max,
            [CallerArgumentExpression(nameof(min))] string? minExpr = null,
            [CallerArgumentExpression(nameof(max))] string? maxExpr = null)
        { a.Link("HasSizeInRange", minExpr, maxExpr); a.Op(a => FileChecks.HasSizeInRange(a.GetValue()?.FullName, min, max, a.IsSkipped(), minExpr, maxExpr)); return a; }

        /// <summary>Asserts that the file is located in the specified directory.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<FileInfo?> IsInDirectory(this ValueAssertions<FileInfo?> a, string directoryPath,
            [CallerArgumentExpression(nameof(directoryPath))] string? expr = null)
        { a.Link("IsInDirectory", expr); a.Op(a => FileChecks.IsInDirectory(a.GetValue()?.FullName, directoryPath, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the file is not located in the specified directory.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<FileInfo?> IsNotInDirectory(this ValueAssertions<FileInfo?> a, string directoryPath,
            [CallerArgumentExpression(nameof(directoryPath))] string? expr = null)
        { a.Link("IsNotInDirectory", expr); a.Op(a => FileChecks.IsNotInDirectory(a.GetValue()?.FullName, directoryPath, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the file has the read-only attribute set.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<FileInfo?> IsReadonly(this ValueAssertions<FileInfo?> a,
            [CallerArgumentExpression(nameof(a))] string? expr = null)
        { a.Link("IsReadonly", expr); a.Op(a => FileChecks.IsReadonly(a.GetValue()?.FullName, a.IsSkipped(), expr)); return a; }

        /// <summary>Projects the file extension.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<string> Extension(this ValueAssertions<FileInfo?> a)
        {
            a.Link("Extension");
            var ext = a.GetValue() is null ? null : Path.GetExtension(a.GetValue()!.FullName);
            return new ValueAssertions<string>(a.GetPipeline(), ext!);
        }

        /// <summary>Projects the file name.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<string> Name(this ValueAssertions<FileInfo?> a)
        {
            a.Link("Name");
            var name = a.GetValue() is null ? null : Path.GetFileName(a.GetValue()!.FullName);
            return new ValueAssertions<string>(a.GetPipeline(), name!);
        }

        /// <summary>Projects the file length in bytes.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<long?> Length(this ValueAssertions<FileInfo?> a)
        {
            a.Link("Length");
            var length = a.GetValue() is null ? (long?)null : new FileInfo(a.GetValue()!.FullName).Length;
            return new ValueAssertions<long?>(a.GetPipeline(), length);
        }

        /// <summary>Projects the file creation time.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<DateTime?> CreationTime(this ValueAssertions<FileInfo?> a)
        {
            a.Link("CreationTime");
            var creationTime = a.GetValue() is null ? (DateTime?)null : new FileInfo(a.GetValue()!.FullName).CreationTime;
            return new ValueAssertions<DateTime?>(a.GetPipeline(), creationTime);
        }

        /// <summary>Projects the file last write time.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<DateTime?> LastWriteTime(this ValueAssertions<FileInfo?> a)
        {
            a.Link("LastWriteTime");
            var lastWriteTime = a.GetValue() is null ? (DateTime?)null : new FileInfo(a.GetValue()!.FullName).LastWriteTime;
            return new ValueAssertions<DateTime?>(a.GetPipeline(), lastWriteTime);
        }
    }
}


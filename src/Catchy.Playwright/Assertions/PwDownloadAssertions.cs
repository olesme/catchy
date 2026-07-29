using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Catchy.Sdk;
using Microsoft.Playwright;

namespace Catchy
{
    namespace Sdk
    {
        internal static class FileHelper
        {
            internal static async Task<string> ReadAllTextAsync(string filePath)
            {
#if NETSTANDARD2_0
                using var sourceStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
                using var reader = new StreamReader(sourceStream);
                return await reader.ReadToEndAsync();
#else
                return await File.ReadAllTextAsync(filePath).ConfigureAwait(false);
#endif
            }
        }
    }

    public static partial class PwAsserterExtensions
    {
        public static ValueAssertions<IDownload> That(this Asserter a, IDownload value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "That",
                valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<IDownload>(p, value);
        }
    }

    public static class PwDownloadAssertionsExtensions
    {
        private static Func<StringComparison> GetEffectiveComparison(this ValueAssertions<IDownload> assertions) => () => assertions.GetPipeline().Settings.DefaultStringComparison;

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IDownload> HasSuggestedFilename(this ValueAssertions<IDownload> assertions, string filename, [CallerArgumentExpression(nameof(filename))] string? expr = null)
        { assertions.Link("HasSuggestedFilename", expr); assertions.Op(a => PwDownloadChecks.HasSuggestedFilename(assertions.GetValue(), filename, false, assertions.GetEffectiveComparison(), assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IDownload> DoesNotHaveSuggestedFilename(this ValueAssertions<IDownload> assertions, string filename, [CallerArgumentExpression(nameof(filename))] string? expr = null)
        { assertions.Link("DoesNotHaveSuggestedFilename", expr); assertions.Op(a => PwDownloadChecks.HasSuggestedFilename(assertions.GetValue(), filename, true, assertions.GetEffectiveComparison(), assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IDownload> FilenameMatches(this ValueAssertions<IDownload> assertions, Regex pattern)
        { assertions.Link("FilenameMatches", pattern.ToString()); assertions.Op(a => PwDownloadChecks.FilenameMatches(assertions.GetValue(), pattern, false, assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IDownload> FilenameDoesNotMatch(this ValueAssertions<IDownload> assertions, Regex pattern)
        { assertions.Link("FilenameDoesNotMatch", pattern.ToString()); assertions.Op(a => PwDownloadChecks.FilenameMatches(assertions.GetValue(), pattern, true, assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IDownload> HasExtension(this ValueAssertions<IDownload> assertions, string extension, [CallerArgumentExpression(nameof(extension))] string? expr = null)
        { assertions.Link("HasExtension", expr); assertions.Op(a => PwDownloadChecks.HasExtension(assertions.GetValue(), extension, false, assertions.GetEffectiveComparison(), assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IDownload> DoesNotHaveExtension(this ValueAssertions<IDownload> assertions, string extension, [CallerArgumentExpression(nameof(extension))] string? expr = null)
        { assertions.Link("DoesNotHaveExtension", expr); assertions.Op(a => PwDownloadChecks.HasExtension(assertions.GetValue(), extension, true, assertions.GetEffectiveComparison(), assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IDownload> FileSizeGreaterThan(this ValueAssertions<IDownload> assertions, long bytes, [CallerArgumentExpression(nameof(bytes))] string? expr = null)
        { assertions.Link("FileSizeGreaterThan", expr); assertions.Op(a => PwDownloadChecks.FileSizeGreaterThan(assertions.GetValue(), bytes, false, assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IDownload> FileSizeNotGreaterThan(this ValueAssertions<IDownload> assertions, long bytes, [CallerArgumentExpression(nameof(bytes))] string? expr = null)
        { assertions.Link("FileSizeNotGreaterThan", expr); assertions.Op(a => PwDownloadChecks.FileSizeGreaterThan(assertions.GetValue(), bytes, true, assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IDownload> FileSizeLessThan(this ValueAssertions<IDownload> assertions, long bytes, [CallerArgumentExpression(nameof(bytes))] string? expr = null)
        { assertions.Link("FileSizeLessThan", expr); assertions.Op(a => PwDownloadChecks.FileSizeLessThan(assertions.GetValue(), bytes, false, assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IDownload> FileSizeNotLessThan(this ValueAssertions<IDownload> assertions, long bytes, [CallerArgumentExpression(nameof(bytes))] string? expr = null)
        { assertions.Link("FileSizeNotLessThan", expr); assertions.Op(a => PwDownloadChecks.FileSizeLessThan(assertions.GetValue(), bytes, true, assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IDownload> FileSizeInRange(this ValueAssertions<IDownload> assertions, long min, long max,
            [CallerArgumentExpression(nameof(min))] string? minExpr = null,
            [CallerArgumentExpression(nameof(max))] string? maxExpr = null)
        { assertions.Link("FileSizeInRange", minExpr, maxExpr); assertions.Op(a => PwDownloadChecks.FileSizeInRange(assertions.GetValue(), min, max, false, assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IDownload> ContentContains(this ValueAssertions<IDownload> assertions, string substring, [CallerArgumentExpression(nameof(substring))] string? expr = null)
        { assertions.Link("ContentContains", expr); assertions.Op(a => PwDownloadChecks.ContentContains(assertions.GetValue(), substring, false, assertions.GetEffectiveComparison(), assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IDownload> ContentDoesNotContain(this ValueAssertions<IDownload> assertions, string substring, [CallerArgumentExpression(nameof(substring))] string? expr = null)
        { assertions.Link("ContentDoesNotContain", expr); assertions.Op(a => PwDownloadChecks.ContentContains(assertions.GetValue(), substring, true, assertions.GetEffectiveComparison(), assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IDownload> ContentMatches(this ValueAssertions<IDownload> assertions, Regex pattern)
        { assertions.Link("ContentMatches", pattern.ToString()); assertions.Op(a => PwDownloadChecks.ContentMatches(assertions.GetValue(), pattern, false, assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IDownload> ContentDoesNotMatch(this ValueAssertions<IDownload> assertions, Regex pattern)
        { assertions.Link("ContentDoesNotMatch", pattern.ToString()); assertions.Op(a => PwDownloadChecks.ContentMatches(assertions.GetValue(), pattern, true, assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IDownload> IsSuccessful(this ValueAssertions<IDownload> assertions)
        { assertions.Link("IsSuccessful"); assertions.Op(a => PwDownloadChecks.IsSuccessful(assertions.GetValue(), false, assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IDownload> IsNotSuccessful(this ValueAssertions<IDownload> assertions)
        { assertions.Link("IsNotSuccessful"); assertions.Op(a => PwDownloadChecks.IsSuccessful(assertions.GetValue(), true, assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IDownload> HasUrl(this ValueAssertions<IDownload> assertions, string url, [CallerArgumentExpression(nameof(url))] string? expr = null)
        { assertions.Link("HasUrl", expr); assertions.Op(a => PwDownloadChecks.HasUrl(assertions.GetValue(), url, false, assertions.GetEffectiveComparison(), assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IDownload> DoesNotHaveUrl(this ValueAssertions<IDownload> assertions, string url, [CallerArgumentExpression(nameof(url))] string? expr = null)
        { assertions.Link("DoesNotHaveUrl", expr); assertions.Op(a => PwDownloadChecks.HasUrl(assertions.GetValue(), url, true, assertions.GetEffectiveComparison(), assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IDownload> FileSizeNotInRange(this ValueAssertions<IDownload> assertions, long min, long max,
            [CallerArgumentExpression(nameof(min))] string? minExpr = null,
            [CallerArgumentExpression(nameof(max))] string? maxExpr = null)
        { assertions.Link("FileSizeNotInRange", minExpr, maxExpr); assertions.Op(a => PwDownloadChecks.FileSizeInRange(assertions.GetValue(), min, max, true, assertions.IsSkipped())); return assertions; }
    }
}



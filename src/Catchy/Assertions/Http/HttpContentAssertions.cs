using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    /// <summary>Entry point for HTTP content assertions.</summary>
    public static partial class AsserterExtensions
    {
        /// <summary>Starts assertions for <see cref="HttpContent"/> value.</summary>
        public static ValueAssertions<HttpContent?> That(this Asserter a, HttpContent? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<HttpContent?>(p, value);
        }
    }

    /// <summary>Provides fluent assertions for HTTP content, headers, and media types.</summary>
    public static class HttpContentAssertionsExtensions
    {
        /// <summary>Asserts that the content is equivalent to <paramref name="expected"/> (deep structural comparison of headers and payload).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpContent?> IsEquivalentTo(this ValueAssertions<HttpContent?> a, HttpContent expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("IsEquivalentTo", expr); a.Op(a => HttpContentChecks.Is(a.GetValue(), expected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the content equals the given string.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpContent?> Is(this ValueAssertions<HttpContent?> a, string expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
            => a.HasString(expected, expr);

        /// <summary>Asserts that the content is not equivalent to <paramref name="unexpected"/> (deep structural comparison).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpContent?> IsNotEquivalentTo(this ValueAssertions<HttpContent?> a, HttpContent unexpected,
            [CallerArgumentExpression(nameof(unexpected))] string? expr = null)
        { a.Link("IsNotEquivalentTo", expr); a.Op(a => HttpContentChecks.IsNot(a.GetValue(), unexpected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the content does not equal the given string.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpContent?> IsNot(this ValueAssertions<HttpContent?> a, string unexpected,
            [CallerArgumentExpression(nameof(unexpected))] string? expr = null)
            => a.Op(a => HttpContentChecks.IsNotStringContent(a.GetValue(), unexpected, a.IsSkipped(), expr));

        /// <summary>Asserts that the content has the specified media type.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpContent?> HasMediaType(this ValueAssertions<HttpContent?> a, string mediaType,
            [CallerArgumentExpression(nameof(mediaType))] string? expr = null)
        { a.Link("HasMediaType", expr); a.Op(a => HttpContentChecks.HasMediaType(a.GetValue(), mediaType, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the content does not have the specified media type.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpContent?> DoesNotHaveMediaType(this ValueAssertions<HttpContent?> a, string mediaType,
            [CallerArgumentExpression(nameof(mediaType))] string? expr = null)
        { a.Link("DoesNotHaveMediaType", expr); a.Op(a => HttpContentChecks.DoesNotHaveMediaType(a.GetValue(), mediaType, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the content has the specified header.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpContent?> HasHeader(this ValueAssertions<HttpContent?> a, string headerName,
            [CallerArgumentExpression(nameof(headerName))] string? expr = null)
        { a.Link("HasHeader", expr); a.Op(a => HttpContentChecks.HasHeader(a.GetValue(), headerName, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the content does not have the specified header.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpContent?> DoesNotHaveHeader(this ValueAssertions<HttpContent?> a, string headerName,
            [CallerArgumentExpression(nameof(headerName))] string? expr = null)
        { a.Link("DoesNotHaveHeader", expr); a.Op(a => HttpContentChecks.DoesNotHaveHeader(a.GetValue(), headerName, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the content has a header with the specified name and value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpContent?> HasHeaderValue(this ValueAssertions<HttpContent?> a, string headerName, string expectedValue,
            [CallerArgumentExpression(nameof(headerName))] string? nameExpr = null,
            [CallerArgumentExpression(nameof(expectedValue))] string? valueExpr = null)
        { a.Link("HasHeaderValue", nameExpr, valueExpr); a.Op(a => HttpContentChecks.HasHeaderValue(a.GetValue(), headerName, expectedValue, a.IsSkipped(), nameExpr, valueExpr)); return a; }

        /// <summary>Asserts that the content string equals the given string.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpContent?> HasString(this ValueAssertions<HttpContent?> a, string expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("HasString", expr); a.Op(a => HttpContentChecks.HasStringContent(a.GetValue(), expected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the content string contains the given substring.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpContent?> ContainsString(this ValueAssertions<HttpContent?> a, string expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("ContainsString", expr); a.Op(a => HttpContentChecks.ContainsStringContent(a.GetValue(), expected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the content has exactly the given length in bytes.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpContent?> HasLength(this ValueAssertions<HttpContent?> a, long expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("HasLength", expr); a.Op(a => HttpContentChecks.HasLength(a.GetValue(), expected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the content is empty.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpContent?> IsEmpty(this ValueAssertions<HttpContent?> a)
        { a.Link("IsEmpty"); a.Op(a => HttpContentChecks.IsEmpty(a.GetValue(), a.IsSkipped())); return a; }
    }
}


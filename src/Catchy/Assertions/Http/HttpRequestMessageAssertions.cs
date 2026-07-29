using System.Diagnostics;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    /// <summary>Entry point for HTTP request message assertions.</summary>
    public static partial class AsserterExtensions
    {
        /// <summary>Starts assertions for an <see cref="HttpRequestMessage"/> value.</summary>
        public static ValueAssertions<HttpRequestMessage?> That(this Asserter a, HttpRequestMessage? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<HttpRequestMessage?>(p, value);
        }
    }

    /// <summary>Provides fluent assertions for HTTP request messages, headers, and content.</summary>
    public static class HttpRequestMessageAssertionsExtensions
    {
        /// <summary>Asserts that the request is equivalent to <paramref name="expected"/> (deep structural comparison of method, URI, headers, and content).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpRequestMessage?> IsEquivalentTo(this ValueAssertions<HttpRequestMessage?> a, HttpRequestMessage expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("IsEquivalentTo", expr); a.Op(a => HttpRequestMessageChecks.Is(a.GetValue(), expected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the request content equals the given string.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpRequestMessage?> Is(this ValueAssertions<HttpRequestMessage?> a, string expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
            => a.HasString(expected, expr);

        /// <summary>Asserts that the request is not equivalent to <paramref name="unexpected"/> (deep structural comparison).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpRequestMessage?> IsNotEquivalentTo(this ValueAssertions<HttpRequestMessage?> a, HttpRequestMessage unexpected,
            [CallerArgumentExpression(nameof(unexpected))] string? expr = null)
        { a.Link("IsNotEquivalentTo", expr); a.Op(a => HttpRequestMessageChecks.IsNot(a.GetValue(), unexpected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the request content does not equal the given string.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpRequestMessage?> IsNot(this ValueAssertions<HttpRequestMessage?> a, string unexpected,
            [CallerArgumentExpression(nameof(unexpected))] string? expr = null)
            => a.Op(a => HttpRequestMessageChecks.IsNotStringContent(a.GetValue(), unexpected, a.IsSkipped(), expr));

        /// <summary>Asserts that the request has the specified media type.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpRequestMessage?> HasMediaType(this ValueAssertions<HttpRequestMessage?> a, string mediaType,
            [CallerArgumentExpression(nameof(mediaType))] string? expr = null)
        { a.Link("HasMediaType", expr); a.Op(a => HttpRequestMessageChecks.HasMediaType(a.GetValue(), mediaType, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the request does not have the specified media type.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpRequestMessage?> DoesNotHaveMediaType(this ValueAssertions<HttpRequestMessage?> a, string mediaType,
            [CallerArgumentExpression(nameof(mediaType))] string? expr = null)
        { a.Link("DoesNotHaveMediaType", expr); a.Op(a => HttpRequestMessageChecks.DoesNotHaveMediaType(a.GetValue(), mediaType, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the request has the specified header.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpRequestMessage?> HasHeader(this ValueAssertions<HttpRequestMessage?> a, string headerName,
            [CallerArgumentExpression(nameof(headerName))] string? expr = null)
        { a.Link("HasHeader", expr); a.Op(a => HttpRequestMessageChecks.HasHeader(a.GetValue(), headerName, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the request does not have the specified header.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpRequestMessage?> DoesNotHaveHeader(this ValueAssertions<HttpRequestMessage?> a, string headerName,
            [CallerArgumentExpression(nameof(headerName))] string? expr = null)
        { a.Link("DoesNotHaveHeader", expr); a.Op(a => HttpRequestMessageChecks.DoesNotHaveHeader(a.GetValue(), headerName, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the request has a header with the specified name and value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpRequestMessage?> HasHeaderValue(this ValueAssertions<HttpRequestMessage?> a, string headerName, string expectedValue,
            [CallerArgumentExpression(nameof(headerName))] string? nameExpr = null,
            [CallerArgumentExpression(nameof(expectedValue))] string? valueExpr = null)
        { a.Link("HasHeaderValue", nameExpr, valueExpr); a.Op(a => HttpRequestMessageChecks.HasHeaderValue(a.GetValue(), headerName, expectedValue, a.IsSkipped(), nameExpr, valueExpr)); return a; }

        /// <summary>Asserts that the request content equals the given string.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpRequestMessage?> HasString(this ValueAssertions<HttpRequestMessage?> a, string expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("HasString", expr); a.Op(a => HttpRequestMessageChecks.HasStringContent(a.GetValue(), expected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the request content contains the given string.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpRequestMessage?> ContainsString(this ValueAssertions<HttpRequestMessage?> a, string expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("ContainsString", expr); a.Op(a => HttpRequestMessageChecks.ContainsStringContent(a.GetValue(), expected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the request content has exactly the given length in bytes.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpRequestMessage?> HasLength(this ValueAssertions<HttpRequestMessage?> a, long expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("HasLength", expr); a.Op(a => HttpRequestMessageChecks.HasLength(a.GetValue(), expected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the request has no content.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpRequestMessage?> IsEmpty(this ValueAssertions<HttpRequestMessage?> a)
        { a.Link("IsEmpty"); a.Op(a => HttpRequestMessageChecks.IsEmpty(a.GetValue(), a.IsSkipped())); return a; }
    }
}


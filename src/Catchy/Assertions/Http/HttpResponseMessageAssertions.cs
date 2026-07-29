using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    /// <summary>Entry point for HTTP response message assertions.</summary>
    public static partial class AsserterExtensions
    {
        /// <summary>Starts assertions for an <see cref="HttpResponseMessage"/> value.</summary>
        public static ValueAssertions<HttpResponseMessage?> That(this Asserter a, HttpResponseMessage? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<HttpResponseMessage?>(p, value);
        }
    }

    /// <summary>Provides fluent assertions for HTTP response messages, status codes, headers, and content.</summary>
    public static class HttpResponseMessageAssertExtensions
    {
        /// <summary>Asserts that the response has the specified HTTP status code.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpResponseMessage?> HasStatusCode(this ValueAssertions<HttpResponseMessage?> a, HttpStatusCode expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("HasStatusCode", expr); a.Op(a => HttpResponseChecks.HasStatusCode(a.GetValue(), expected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the response has the specified HTTP status code (as int).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpResponseMessage?> HasStatusCode(this ValueAssertions<HttpResponseMessage?> a, int expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
            => a.HasStatusCode((HttpStatusCode)expected, expr);

        /// <summary>Asserts that the response does not have the specified HTTP status code.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpResponseMessage?> DoesNotHaveStatusCode(this ValueAssertions<HttpResponseMessage?> a, HttpStatusCode unexpected,
            [CallerArgumentExpression(nameof(unexpected))] string? expr = null)
        { a.Link("DoesNotHaveStatusCode", expr); a.Op(a => HttpResponseChecks.DoesNotHaveStatusCode(a.GetValue(), unexpected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the response indicates success (status code 200-299).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpResponseMessage?> IsSuccessfull(this ValueAssertions<HttpResponseMessage?> a)
        { a.Link("IsSuccessfull"); a.Op(a => HttpResponseChecks.IsSuccessful(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the response does not indicate success.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpResponseMessage?> IsNotSuccessfull(this ValueAssertions<HttpResponseMessage?> a)
        { a.Link("IsNotSuccessfull"); a.Op(a => HttpResponseChecks.IsNotSuccessful(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the response indicates a redirect (status code 300-399).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpResponseMessage?> IsRedirection(this ValueAssertions<HttpResponseMessage?> a)
        { a.Link("IsRedirection"); a.Op(a => HttpResponseChecks.IsRedirection(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the response does not indicate a redirect.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpResponseMessage?> IsNotRedirection(this ValueAssertions<HttpResponseMessage?> a)
        { a.Link("IsNotRedirection"); a.Op(a => HttpResponseChecks.IsNotRedirection(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the response indicates a client error (status code 400-499).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpResponseMessage?> IsClientError(this ValueAssertions<HttpResponseMessage?> a)
        { a.Link("IsClientError"); a.Op(a => HttpResponseChecks.IsClientError(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the response does not indicate a client error.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpResponseMessage?> IsNotClientError(this ValueAssertions<HttpResponseMessage?> a)
        { a.Link("IsNotClientError"); a.Op(a => HttpResponseChecks.IsNotClientError(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the response indicates a server error (status code 500-599).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpResponseMessage?> IsServerError(this ValueAssertions<HttpResponseMessage?> a)
        { a.Link("IsServerError"); a.Op(a => HttpResponseChecks.IsServerError(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the response does not indicate a server error.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpResponseMessage?> IsNotServerError(this ValueAssertions<HttpResponseMessage?> a)
        { a.Link("IsNotServerError"); a.Op(a => HttpResponseChecks.IsNotServerError(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the response has the specified header.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpResponseMessage?> HasHeader(this ValueAssertions<HttpResponseMessage?> a, string headerName,
            [CallerArgumentExpression(nameof(headerName))] string? expr = null)
        { a.Link("HasHeader", expr); a.Op(a => HttpResponseChecks.HasHeader(a.GetValue(), headerName, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the response does not have the specified header.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpResponseMessage?> DoesNotHaveHeader(this ValueAssertions<HttpResponseMessage?> a, string headerName,
            [CallerArgumentExpression(nameof(headerName))] string? expr = null)
        { a.Link("DoesNotHaveHeader", expr); a.Op(a => HttpResponseChecks.DoesNotHaveHeader(a.GetValue(), headerName, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the response has a header with the specified name and value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpResponseMessage?> HasHeaderValue(this ValueAssertions<HttpResponseMessage?> a, string headerName, string expectedValue,
            [CallerArgumentExpression(nameof(headerName))] string? nameExpr = null,
            [CallerArgumentExpression(nameof(expectedValue))] string? valueExpr = null)
        { a.Link("HasHeaderValue", nameExpr, valueExpr); a.Op(a => HttpResponseChecks.HasHeaderValue(a.GetValue(), headerName, expectedValue, a.IsSkipped(), nameExpr, valueExpr)); return a; }

        /// <summary>Asserts that the response has the specified Content-Type media type.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpResponseMessage?> HasContentType(this ValueAssertions<HttpResponseMessage?> a, string mediaType,
            [CallerArgumentExpression(nameof(mediaType))] string? expr = null)
        { a.Link("HasContentType", expr); a.Op(a => HttpResponseChecks.HasContentType(a.GetValue(), mediaType, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the response does not have the specified Content-Type media type.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpResponseMessage?> DoesNotHaveContentType(this ValueAssertions<HttpResponseMessage?> a, string mediaType,
            [CallerArgumentExpression(nameof(mediaType))] string? expr = null)
        { a.Link("DoesNotHaveContentType", expr); a.Op(a => HttpResponseChecks.DoesNotHaveContentType(a.GetValue(), mediaType, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the response has the specified reason phrase.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpResponseMessage?> HasReasonPhrase(this ValueAssertions<HttpResponseMessage?> a, string expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("HasReasonPhrase", expr); a.Op(a => HttpResponseChecks.HasReasonPhrase(a.GetValue(), expected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the response has the specified HTTP version.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpResponseMessage?> HasVersion(this ValueAssertions<HttpResponseMessage?> a, Version expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("HasVersion", expr); a.Op(a => HttpResponseChecks.HasVersion(a.GetValue(), expected, a.IsSkipped(), expr)); return a; }

        /// <summary>Projects the HTTP status code.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<HttpStatusCode?> StatusCode(this ValueAssertions<HttpResponseMessage?> a)
        {
            a.Link("StatusCode");
            return new ValueAssertions<HttpStatusCode?>(a.GetPipeline(), a.GetValue()?.StatusCode);
        }

        /// <summary>Projects the response headers.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<IEnumerable<string>?> Headers(this ValueAssertions<HttpResponseMessage?> a)
        {
            a.Link("Headers");
            return new ValueAssertions<IEnumerable<string>?>(a.GetPipeline(), a.GetValue()?.Headers.Select(h => h.Key));
        }

        /// <summary>Projects the response reason phrase.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<string> ReasonPhrase(this ValueAssertions<HttpResponseMessage?> a)
        {
            a.Link("ReasonPhrase");
            return new ValueAssertions<string>(a.GetPipeline(), a.GetValue()?.ReasonPhrase!);
        }

        /// <summary>Projects the response content.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<HttpContent?> Content(this ValueAssertions<HttpResponseMessage?> a)
        {
            a.Link("Content");
            return new ValueAssertions<HttpContent?>(a.GetPipeline(), a.GetValue()?.Content);
        }

        /// <summary>Projects the original HTTP request message.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<HttpRequestMessage?> RequestMessage(this ValueAssertions<HttpResponseMessage?> a)
        {
            a.Link("RequestMessage");
            return new ValueAssertions<HttpRequestMessage?>(a.GetPipeline(), a.GetValue()?.RequestMessage);
        }

        /// <summary>Asserts that the status code is a server error (5xx).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpResponseMessage?> IsServerErrorStatusCode(this ValueAssertions<HttpResponseMessage?> a)
        { a.Link("IsServerErrorStatusCode"); a.Op(a => HttpResponseChecks.IsServerError(a.GetValue(), a.IsSkipped())); return a; }
    }
}


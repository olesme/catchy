using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    public static partial class AsserterExtensions
    {
        /// <summary>Starts assertions for an HTTP status code.</summary>
        public static ValueAssertions<HttpStatusCode?> That(this Asserter a, HttpStatusCode? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<HttpStatusCode?>(p, value);
        }
    }

    public static class HttpStatusCodeAssertExtensions
    {
        /// <summary>Asserts that the status code equals the expected value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpStatusCode?> Is(this ValueAssertions<HttpStatusCode?> a, HttpStatusCode expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("Is", expr); a.Op(a => HttpStatusCodeChecks.Is(a.GetValue(), expected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the status code equals the expected numeric value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpStatusCode?> Is(this ValueAssertions<HttpStatusCode?> a, int expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
            => a.Is((HttpStatusCode)expected, expr);

        /// <summary>Asserts that the status code does not equal the unexpected value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpStatusCode?> IsNot(this ValueAssertions<HttpStatusCode?> a, HttpStatusCode unexpected,
            [CallerArgumentExpression(nameof(unexpected))] string? expr = null)
        { a.Link("IsNot", expr); a.Op(a => HttpStatusCodeChecks.IsNot(a.GetValue(), unexpected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the status code indicates a 2xx success response.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpStatusCode?> IsSuccess(this ValueAssertions<HttpStatusCode?> a)
        { a.Link("IsSuccess"); a.Op(a => HttpStatusCodeChecks.IsSuccess(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the status code does not indicate a 2xx success response.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpStatusCode?> IsNotSuccess(this ValueAssertions<HttpStatusCode?> a)
        { a.Link("IsNotSuccess"); a.Op(a => HttpStatusCodeChecks.IsNotSuccess(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the status code indicates a 3xx redirection response.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpStatusCode?> IsRedirection(this ValueAssertions<HttpStatusCode?> a)
        { a.Link("IsRedirection"); a.Op(a => HttpStatusCodeChecks.IsRedirection(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the status code does not indicate a 3xx redirection response.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpStatusCode?> IsNotRedirection(this ValueAssertions<HttpStatusCode?> a)
        { a.Link("IsNotRedirection"); a.Op(a => HttpStatusCodeChecks.IsNotRedirection(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the status code indicates a 4xx client error response.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpStatusCode?> IsClientError(this ValueAssertions<HttpStatusCode?> a)
        { a.Link("IsClientError"); a.Op(a => HttpStatusCodeChecks.IsClientError(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the status code does not indicate a 4xx client error response.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpStatusCode?> IsNotClientError(this ValueAssertions<HttpStatusCode?> a)
        { a.Link("IsNotClientError"); a.Op(a => HttpStatusCodeChecks.IsNotClientError(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the status code indicates a 5xx server error response.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpStatusCode?> IsServerError(this ValueAssertions<HttpStatusCode?> a)
        { a.Link("IsServerError"); a.Op(a => HttpStatusCodeChecks.IsServerError(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the status code does not indicate a 5xx server error response.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<HttpStatusCode?> IsNotServerError(this ValueAssertions<HttpStatusCode?> a)
        { a.Link("IsNotServerError"); a.Op(a => HttpStatusCodeChecks.IsNotServerError(a.GetValue(), a.IsSkipped())); return a; }

    }
}


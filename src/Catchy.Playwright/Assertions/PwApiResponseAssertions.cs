using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Catchy.Sdk;
using Microsoft.Playwright;

namespace Catchy
{
    namespace Sdk
    {
        public static class PwApiResponseAssertionsAccessors
        {
        }
    }

    public static partial class PwAsserterExtensions
    {
        public static ValueAssertions<IAPIResponse> That(this Asserter a, IAPIResponse value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "That",
                valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<IAPIResponse>(p, value);
        }
    }

    public static class PwApiResponseAssertionsExtensions
    {
        private static Func<StringComparison> GetEffectiveComparison(this ValueAssertions<IAPIResponse> assertions)
            => () => assertions.GetPipeline().Settings.DefaultStringComparison;

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> IsOk(this ValueAssertions<IAPIResponse> assertions)
        { assertions.Link("IsOk"); assertions.Op(a => PwApiResponseChecks.IsOk(a.GetValue(), false, a.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> IsNotOk(this ValueAssertions<IAPIResponse> assertions)
        { assertions.Link("IsNotOk"); assertions.Op(a => PwApiResponseChecks.IsOk(a.GetValue(), true, a.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> HasStatus(this ValueAssertions<IAPIResponse> assertions, int status, [CallerArgumentExpression(nameof(status))] string? expr = null)
        { assertions.Link("HasStatus", expr); assertions.Op(a => PwApiResponseChecks.HasStatus(a.GetValue(), status, false, a.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> DoesNotHaveStatus(this ValueAssertions<IAPIResponse> assertions, int status, [CallerArgumentExpression(nameof(status))] string? expr = null)
        { assertions.Link("DoesNotHaveStatus", expr); assertions.Op(a => PwApiResponseChecks.HasStatus(a.GetValue(), status, true, a.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> HasStatusInRange(this ValueAssertions<IAPIResponse> assertions, int min, int max,
            [CallerArgumentExpression(nameof(min))] string? minExpr = null,
            [CallerArgumentExpression(nameof(max))] string? maxExpr = null)
        { assertions.Link("HasStatusInRange", minExpr, maxExpr); assertions.Op(a => PwApiResponseChecks.HasStatusInRange(a.GetValue(), min, max, false, a.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> DoesNotHaveStatusInRange(this ValueAssertions<IAPIResponse> assertions, int min, int max,
            [CallerArgumentExpression(nameof(min))] string? minExpr = null,
            [CallerArgumentExpression(nameof(max))] string? maxExpr = null)
        { assertions.Link("DoesNotHaveStatusInRange", minExpr, maxExpr); assertions.Op(a => PwApiResponseChecks.HasStatusInRange(a.GetValue(), min, max, true, a.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod] public static ValueAssertions<IAPIResponse> IsCreated(this ValueAssertions<IAPIResponse> assertions) { assertions.Link("IsCreated"); assertions.Op(a => PwApiResponseChecks.HasStatus(a.GetValue(), 201, false, a.IsSkipped())); return assertions; }
        [DebuggerHidden, StackTraceHidden, AssertionMethod] public static ValueAssertions<IAPIResponse> IsNotCreated(this ValueAssertions<IAPIResponse> assertions) { assertions.Link("IsNotCreated"); assertions.Op(a => PwApiResponseChecks.HasStatus(a.GetValue(), 201, true, a.IsSkipped())); return assertions; }
        [DebuggerHidden, StackTraceHidden, AssertionMethod] public static ValueAssertions<IAPIResponse> IsNoContent(this ValueAssertions<IAPIResponse> assertions) { assertions.Link("IsNoContent"); assertions.Op(a => PwApiResponseChecks.HasStatus(a.GetValue(), 204, false, a.IsSkipped())); return assertions; }
        [DebuggerHidden, StackTraceHidden, AssertionMethod] public static ValueAssertions<IAPIResponse> IsNotNoContent(this ValueAssertions<IAPIResponse> assertions) { assertions.Link("IsNotNoContent"); assertions.Op(a => PwApiResponseChecks.HasStatus(a.GetValue(), 204, true, a.IsSkipped())); return assertions; }
        [DebuggerHidden, StackTraceHidden, AssertionMethod] public static ValueAssertions<IAPIResponse> IsBadRequest(this ValueAssertions<IAPIResponse> assertions) { assertions.Link("IsBadRequest"); assertions.Op(a => PwApiResponseChecks.HasStatus(a.GetValue(), 400, false, a.IsSkipped())); return assertions; }
        [DebuggerHidden, StackTraceHidden, AssertionMethod] public static ValueAssertions<IAPIResponse> IsNotBadRequest(this ValueAssertions<IAPIResponse> assertions) { assertions.Link("IsNotBadRequest"); assertions.Op(a => PwApiResponseChecks.HasStatus(a.GetValue(), 400, true, a.IsSkipped())); return assertions; }
        [DebuggerHidden, StackTraceHidden, AssertionMethod] public static ValueAssertions<IAPIResponse> IsUnauthorized(this ValueAssertions<IAPIResponse> assertions) { assertions.Link("IsUnauthorized"); assertions.Op(a => PwApiResponseChecks.HasStatus(a.GetValue(), 401, false, a.IsSkipped())); return assertions; }
        [DebuggerHidden, StackTraceHidden, AssertionMethod] public static ValueAssertions<IAPIResponse> IsNotUnauthorized(this ValueAssertions<IAPIResponse> assertions) { assertions.Link("IsNotUnauthorized"); assertions.Op(a => PwApiResponseChecks.HasStatus(a.GetValue(), 401, true, a.IsSkipped())); return assertions; }
        [DebuggerHidden, StackTraceHidden, AssertionMethod] public static ValueAssertions<IAPIResponse> IsForbidden(this ValueAssertions<IAPIResponse> assertions) { assertions.Link("IsForbidden"); assertions.Op(a => PwApiResponseChecks.HasStatus(a.GetValue(), 403, false, a.IsSkipped())); return assertions; }
        [DebuggerHidden, StackTraceHidden, AssertionMethod] public static ValueAssertions<IAPIResponse> IsNotForbidden(this ValueAssertions<IAPIResponse> assertions) { assertions.Link("IsNotForbidden"); assertions.Op(a => PwApiResponseChecks.HasStatus(a.GetValue(), 403, true, a.IsSkipped())); return assertions; }
        [DebuggerHidden, StackTraceHidden, AssertionMethod] public static ValueAssertions<IAPIResponse> IsNotFound(this ValueAssertions<IAPIResponse> assertions) { assertions.Link("IsNotFound"); assertions.Op(a => PwApiResponseChecks.HasStatus(a.GetValue(), 404, false, a.IsSkipped())); return assertions; }
        [DebuggerHidden, StackTraceHidden, AssertionMethod] public static ValueAssertions<IAPIResponse> IsNotNotFound(this ValueAssertions<IAPIResponse> assertions) { assertions.Link("IsNotNotFound"); assertions.Op(a => PwApiResponseChecks.HasStatus(a.GetValue(), 404, true, a.IsSkipped())); return assertions; }
        [DebuggerHidden, StackTraceHidden, AssertionMethod] public static ValueAssertions<IAPIResponse> IsConflict(this ValueAssertions<IAPIResponse> assertions) { assertions.Link("IsConflict"); assertions.Op(a => PwApiResponseChecks.HasStatus(a.GetValue(), 409, false, a.IsSkipped())); return assertions; }
        [DebuggerHidden, StackTraceHidden, AssertionMethod] public static ValueAssertions<IAPIResponse> IsNotConflict(this ValueAssertions<IAPIResponse> assertions) { assertions.Link("IsNotConflict"); assertions.Op(a => PwApiResponseChecks.HasStatus(a.GetValue(), 409, true, a.IsSkipped())); return assertions; }
        [DebuggerHidden, StackTraceHidden, AssertionMethod] public static ValueAssertions<IAPIResponse> IsUnprocessable(this ValueAssertions<IAPIResponse> assertions) { assertions.Link("IsUnprocessable"); assertions.Op(a => PwApiResponseChecks.HasStatus(a.GetValue(), 422, false, a.IsSkipped())); return assertions; }
        [DebuggerHidden, StackTraceHidden, AssertionMethod] public static ValueAssertions<IAPIResponse> IsNotUnprocessable(this ValueAssertions<IAPIResponse> assertions) { assertions.Link("IsNotUnprocessable"); assertions.Op(a => PwApiResponseChecks.HasStatus(a.GetValue(), 422, true, a.IsSkipped())); return assertions; }
        [DebuggerHidden, StackTraceHidden, AssertionMethod] public static ValueAssertions<IAPIResponse> IsTooManyRequests(this ValueAssertions<IAPIResponse> assertions) { assertions.Link("IsTooManyRequests"); assertions.Op(a => PwApiResponseChecks.HasStatus(a.GetValue(), 429, false, a.IsSkipped())); return assertions; }
        [DebuggerHidden, StackTraceHidden, AssertionMethod] public static ValueAssertions<IAPIResponse> IsNotTooManyRequests(this ValueAssertions<IAPIResponse> assertions) { assertions.Link("IsNotTooManyRequests"); assertions.Op(a => PwApiResponseChecks.HasStatus(a.GetValue(), 429, true, a.IsSkipped())); return assertions; }
        [DebuggerHidden, StackTraceHidden, AssertionMethod] public static ValueAssertions<IAPIResponse> IsServerError(this ValueAssertions<IAPIResponse> assertions) { assertions.Link("IsServerError"); assertions.Op(a => PwApiResponseChecks.HasStatusInRange(a.GetValue(), 500, 599, false, a.IsSkipped())); return assertions; }
        [DebuggerHidden, StackTraceHidden, AssertionMethod] public static ValueAssertions<IAPIResponse> IsNotServerError(this ValueAssertions<IAPIResponse> assertions) { assertions.Link("IsNotServerError"); assertions.Op(a => PwApiResponseChecks.HasStatusInRange(a.GetValue(), 500, 599, true, a.IsSkipped())); return assertions; }
        [DebuggerHidden, StackTraceHidden, AssertionMethod] public static ValueAssertions<IAPIResponse> IsSuccessful(this ValueAssertions<IAPIResponse> assertions) { assertions.Link("IsSuccess"); assertions.Op(a => PwApiResponseChecks.HasStatusInRange(a.GetValue(), 200, 299, false, a.IsSkipped())); return assertions; }
        [DebuggerHidden, StackTraceHidden, AssertionMethod] public static ValueAssertions<IAPIResponse> IsNotSuccessful(this ValueAssertions<IAPIResponse> assertions) { assertions.Link("IsNotSuccessful"); assertions.Op(a => PwApiResponseChecks.HasStatusInRange(a.GetValue(), 200, 299, true, a.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> HasHeader(this ValueAssertions<IAPIResponse> assertions, string name, [CallerArgumentExpression(nameof(name))] string? expr = null)
        { assertions.Link("HasHeader", expr); assertions.Op(a => PwApiResponseChecks.HasHeader(a.GetValue(), name, false, a.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> DoesNotHaveHeader(this ValueAssertions<IAPIResponse> assertions, string name, [CallerArgumentExpression(nameof(name))] string? expr = null)
        { assertions.Link("DoesNotHaveHeader", expr); assertions.Op(a => PwApiResponseChecks.HasHeader(a.GetValue(), name, true, a.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> HasHeaderValue(this ValueAssertions<IAPIResponse> assertions, string name, string expected,
            [CallerArgumentExpression(nameof(name))] string? nameExpr = null,
            [CallerArgumentExpression(nameof(expected))] string? valExpr = null)
        { assertions.Link("HasHeaderValue", nameExpr, valExpr); assertions.Op(a => PwApiResponseChecks.HasHeaderValue(a.GetValue(), name, expected, false, a.GetEffectiveComparison(), a.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> DoesNotHaveHeaderValue(this ValueAssertions<IAPIResponse> assertions, string name, string unexpected,
            [CallerArgumentExpression(nameof(name))] string? nameExpr = null,
            [CallerArgumentExpression(nameof(unexpected))] string? valExpr = null)
        { assertions.Link("DoesNotHaveHeaderValue", nameExpr, valExpr); assertions.Op(a => PwApiResponseChecks.HasHeaderValue(a.GetValue(), name, unexpected, true, a.GetEffectiveComparison(), a.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> HasContentType(this ValueAssertions<IAPIResponse> assertions, string mediaType, [CallerArgumentExpression(nameof(mediaType))] string? expr = null)
        { assertions.Link("HasContentType", expr); assertions.Op(a => PwApiResponseChecks.HasContentType(a.GetValue(), mediaType, false, a.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> DoesNotHaveContentType(this ValueAssertions<IAPIResponse> assertions, string mediaType, [CallerArgumentExpression(nameof(mediaType))] string? expr = null)
        { assertions.Link("DoesNotHaveContentType", expr); assertions.Op(a => PwApiResponseChecks.HasContentType(a.GetValue(), mediaType, true, a.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> HasContentTypeJson(this ValueAssertions<IAPIResponse> assertions)
        { assertions.Link("HasContentTypeJson"); assertions.Op(a => PwApiResponseChecks.HasContentType(a.GetValue(), "application/json", false, a.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> DoesNotHaveContentTypeJson(this ValueAssertions<IAPIResponse> assertions)
        { assertions.Link("DoesNotHaveContentTypeJson"); assertions.Op(a => PwApiResponseChecks.HasContentType(a.GetValue(), "application/json", true, a.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> BodyContains(this ValueAssertions<IAPIResponse> assertions, string substring, [CallerArgumentExpression(nameof(substring))] string? expr = null)
        { assertions.Link("BodyContains", expr); assertions.Op(a => PwApiResponseChecks.BodyContains(a.GetValue(), substring, false, a.GetEffectiveComparison(), a.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> BodyDoesNotContain(this ValueAssertions<IAPIResponse> assertions, string substring, [CallerArgumentExpression(nameof(substring))] string? expr = null)
        { assertions.Link("BodyDoesNotContain", expr); assertions.Op(a => PwApiResponseChecks.BodyContains(a.GetValue(), substring, true, a.GetEffectiveComparison(), a.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> BodyMatches(this ValueAssertions<IAPIResponse> assertions, Regex pattern)
        { assertions.Link("BodyMatches", pattern.ToString()); assertions.Op(a => PwApiResponseChecks.BodyMatches(a.GetValue(), pattern, false, a.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> BodyDoesNotMatch(this ValueAssertions<IAPIResponse> assertions, Regex pattern)
        { assertions.Link("BodyDoesNotMatch", pattern.ToString()); assertions.Op(a => PwApiResponseChecks.BodyMatches(a.GetValue(), pattern, true, a.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> IsValidJson(this ValueAssertions<IAPIResponse> assertions)
        { assertions.Link("IsValidJson"); assertions.Op(a => PwApiResponseChecks.IsValidJson(a.GetValue(), false, a.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> IsNotValidJson(this ValueAssertions<IAPIResponse> assertions)
        { assertions.Link("IsNotValidJson"); assertions.Op(a => PwApiResponseChecks.IsValidJson(a.GetValue(), true, a.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> HasJsonValue<T>(this ValueAssertions<IAPIResponse> assertions, string path, T expected,
            [CallerArgumentExpression(nameof(path))] string? pathExpr = null,
            [CallerArgumentExpression(nameof(expected))] string? valExpr = null)
        { assertions.Link("HasJsonValue", pathExpr, valExpr); assertions.Op(a => PwApiResponseChecks.HasJsonValue(a.GetValue(), path, expected, false, a.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> DoesNotHaveJsonValue<T>(this ValueAssertions<IAPIResponse> assertions, string path, T unexpected,
            [CallerArgumentExpression(nameof(path))] string? pathExpr = null,
            [CallerArgumentExpression(nameof(unexpected))] string? valExpr = null)
        { assertions.Link("DoesNotHaveJsonValue", pathExpr, valExpr); assertions.Op(a => PwApiResponseChecks.HasJsonValue(a.GetValue(), path, unexpected, true, a.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> HasJsonArrayLength(this ValueAssertions<IAPIResponse> assertions, string path, int count,
            [CallerArgumentExpression(nameof(path))] string? pathExpr = null,
            [CallerArgumentExpression(nameof(count))] string? countExpr = null)
        { assertions.Link("HasJsonArrayLength", pathExpr, countExpr); assertions.Op(a => PwApiResponseChecks.HasJsonArrayLength(a.GetValue(), path, count, false, a.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> DoesNotHaveJsonArrayLength(this ValueAssertions<IAPIResponse> assertions, string path, int count,
            [CallerArgumentExpression(nameof(path))] string? pathExpr = null,
            [CallerArgumentExpression(nameof(count))] string? countExpr = null)
        { assertions.Link("DoesNotHaveJsonArrayLength", pathExpr, countExpr); assertions.Op(a => PwApiResponseChecks.HasJsonArrayLength(a.GetValue(), path, count, true, a.IsSkipped())); return assertions; }
    }
}




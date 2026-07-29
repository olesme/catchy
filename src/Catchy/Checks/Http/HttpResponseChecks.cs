using System;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Catchy.Sdk
{
    public static class HttpResponseChecks
    {
        static string Fmt(object? v, string? expr = null) => ExprFormat.Inline(v, expr);

        public static CheckOperation HasStatusCode(HttpResponseMessage? actual, HttpStatusCode expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && actual.StatusCode == expected,
                () => $"Expected status code to be {(int)expected} ({expected}), but was {(actual is null ? "null" : $"{(int)actual.StatusCode} ({actual.StatusCode})")}",
                isSkipped);

        public static CheckOperation DoesNotHaveStatusCode(HttpResponseMessage? actual, HttpStatusCode unexpected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is null || actual.StatusCode != unexpected,
                () => $"Expected status code not to be {(int)unexpected} ({unexpected})",
                isSkipped);

        public static CheckOperation IsSuccessful(HttpResponseMessage? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && (int)actual.StatusCode >= 200 && (int)actual.StatusCode <= 299,
                () => $"Expected response to be successful (2xx), but was {(actual is null ? "null" : $"{(int)actual.StatusCode} ({actual.StatusCode})")}",
                isSkipped);

        public static CheckOperation IsNotSuccessful(HttpResponseMessage? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || (int)actual.StatusCode < 200 || (int)actual.StatusCode > 299,
                () => $"Expected response not to be successful (2xx)",
                isSkipped);

        public static CheckOperation IsRedirection(HttpResponseMessage? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && (int)actual.StatusCode >= 300 && (int)actual.StatusCode <= 399,
                () => $"Expected response to be a redirection (3xx), but was {(actual is null ? "null" : $"{(int)actual.StatusCode} ({actual.StatusCode})")}",
                isSkipped);

        public static CheckOperation IsNotRedirection(HttpResponseMessage? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || (int)actual.StatusCode < 300 || (int)actual.StatusCode > 399,
                () => $"Expected response not to be a redirection (3xx)",
                isSkipped);

        public static CheckOperation IsClientError(HttpResponseMessage? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && (int)actual.StatusCode >= 400 && (int)actual.StatusCode <= 499,
                () => $"Expected response to be a client error (4xx), but was {(actual is null ? "null" : $"{(int)actual.StatusCode} ({actual.StatusCode})")}",
                isSkipped);

        public static CheckOperation IsNotClientError(HttpResponseMessage? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || (int)actual.StatusCode < 400 || (int)actual.StatusCode > 499,
                () => $"Expected response not to be a client error (4xx)",
                isSkipped);

        public static CheckOperation IsServerError(HttpResponseMessage? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && (int)actual.StatusCode >= 500 && (int)actual.StatusCode <= 599,
                () => $"Expected response to be a server error (5xx), but was {(actual is null ? "null" : $"{(int)actual.StatusCode} ({actual.StatusCode})")}",
                isSkipped);

        public static CheckOperation IsNotServerError(HttpResponseMessage? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || (int)actual.StatusCode < 500 || (int)actual.StatusCode > 599,
                () => $"Expected response not to be a server error (5xx)",
                isSkipped);

        public static CheckOperation HasHeader(HttpResponseMessage? actual, string headerName, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && (actual.Headers.Contains(headerName) || actual.Content?.Headers.Contains(headerName) == true),
                () => $"Expected response to have header {Fmt(headerName, expr)}",
                isSkipped);

        public static CheckOperation DoesNotHaveHeader(HttpResponseMessage? actual, string headerName, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is null || (!actual.Headers.Contains(headerName) && actual.Content?.Headers.Contains(headerName) != true),
                () => $"Expected response not to have header {Fmt(headerName, expr)}",
                isSkipped);

        public static CheckOperation HasHeaderValue(HttpResponseMessage? actual, string headerName, string expectedValue, bool isSkipped, string? nameExpr = null, string? valueExpr = null)
        {
            return CheckOperation.Sync(() =>
            {
                if (actual is null) return false;
                System.Collections.Generic.IEnumerable<string>? values = null;
                if (actual.Headers.TryGetValues(headerName, out var h)) values = h;
                else if (actual.Content?.Headers.TryGetValues(headerName, out var ch) == true) values = ch;
                return values?.Any(v => v.Contains(expectedValue, StringComparison.OrdinalIgnoreCase)) == true;
            },
            () => $"Expected header {Fmt(headerName, nameExpr)} to contain {Fmt(expectedValue, valueExpr)}",
            isSkipped);
        }

        public static CheckOperation HasContentType(HttpResponseMessage? actual, string mediaType, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && string.Equals(actual.Content?.Headers.ContentType?.MediaType, mediaType, StringComparison.OrdinalIgnoreCase),
                () => $"Expected Content-Type to be {Fmt(mediaType, expr)}, but was '{actual?.Content?.Headers.ContentType?.MediaType ?? "none"}'",
                isSkipped);

        public static CheckOperation DoesNotHaveContentType(HttpResponseMessage? actual, string mediaType, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is null || !string.Equals(actual.Content?.Headers.ContentType?.MediaType, mediaType, StringComparison.OrdinalIgnoreCase),
                () => $"Expected Content-Type not to be {Fmt(mediaType, expr)}",
                isSkipped);

        public static CheckOperation HasReasonPhrase(HttpResponseMessage? actual, string expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && string.Equals(actual.ReasonPhrase, expected, StringComparison.OrdinalIgnoreCase),
                () => $"Expected reason phrase to be {Fmt(expected, expr)}, but was '{actual?.ReasonPhrase ?? "null"}'",
                isSkipped);

        public static CheckOperation HasVersion(HttpResponseMessage? actual, Version expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && actual.Version == expected,
                () => $"Expected HTTP version to be {Fmt(expected, expr)}, but was {actual?.Version.ToString() ?? "null"}",
                isSkipped);
    }
}

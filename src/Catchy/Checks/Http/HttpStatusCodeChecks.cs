using System.Net;

namespace Catchy.Sdk
{
    public static class HttpStatusCodeChecks
    {
        public static CheckOperation Is(HttpStatusCode? actual, HttpStatusCode expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && actual == expected,
                () => $"Expected status code to be {(int)expected} ({expected}), but was {(actual is null ? "null" : $"{(int)actual} ({actual})")}",
                isSkipped);

        public static CheckOperation IsNot(HttpStatusCode? actual, HttpStatusCode unexpected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is null || actual != unexpected,
                () => $"Expected status code not to be {(int)unexpected} ({unexpected})",
                isSkipped);

        public static CheckOperation IsSuccess(HttpStatusCode? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && (int)actual >= 200 && (int)actual <= 299,
                () => $"Expected status code to be success (2xx), but was {(actual is null ? "null" : $"{(int)actual} ({actual})")}",
                isSkipped);

        public static CheckOperation IsNotSuccess(HttpStatusCode? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || (int)actual < 200 || (int)actual > 299,
                () => $"Expected status code not to be success (2xx)",
                isSkipped);

        public static CheckOperation IsRedirection(HttpStatusCode? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && (int)actual >= 300 && (int)actual <= 399,
                () => $"Expected status code to be redirection (3xx), but was {(actual is null ? "null" : $"{(int)actual} ({actual})")}",
                isSkipped);

        public static CheckOperation IsNotRedirection(HttpStatusCode? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || (int)actual < 300 || (int)actual > 399,
                () => $"Expected status code not to be redirection (3xx)",
                isSkipped);

        public static CheckOperation IsClientError(HttpStatusCode? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && (int)actual >= 400 && (int)actual <= 499,
                () => $"Expected status code to be client error (4xx), but was {(actual is null ? "null" : $"{(int)actual} ({actual})")}",
                isSkipped);

        public static CheckOperation IsNotClientError(HttpStatusCode? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || (int)actual < 400 || (int)actual > 499,
                () => $"Expected status code not to be client error (4xx)",
                isSkipped);

        public static CheckOperation IsServerError(HttpStatusCode? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && (int)actual >= 500 && (int)actual <= 599,
                () => $"Expected status code to be server error (5xx), but was {(actual is null ? "null" : $"{(int)actual} ({actual})")}",
                isSkipped);

        public static CheckOperation IsNotServerError(HttpStatusCode? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || (int)actual < 500 || (int)actual > 599,
                () => $"Expected status code not to be server error (5xx)",
                isSkipped);
    }
}

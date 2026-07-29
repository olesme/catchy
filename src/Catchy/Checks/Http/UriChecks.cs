using System;
using System.Collections.Generic;

namespace Catchy.Sdk
{
    public static class UriChecks
    {
        static string Fmt(object? v, string? expr = null) => ExprFormat.Inline(v, expr);

        public static CheckOperation IsAbsolute(Uri? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.IsAbsoluteUri,
                () => $"Expected URI to be absolute: {actual?.ToString() ?? "null"}",
                isSkipped);

        public static CheckOperation IsRelative(Uri? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && !actual.IsAbsoluteUri,
                () => $"Expected URI to be relative: {actual?.ToString() ?? "null"}",
                isSkipped);

        public static CheckOperation HasScheme(Uri? actual, string scheme, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && actual.IsAbsoluteUri && string.Equals(actual.Scheme, scheme, StringComparison.OrdinalIgnoreCase),
                () => $"Expected URI scheme to be {Fmt(scheme, expr)}, but was '{actual?.Scheme ?? "N/A"}'",
                isSkipped);

        public static CheckOperation DoesNotHaveScheme(Uri? actual, string scheme, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is null || !actual.IsAbsoluteUri || !string.Equals(actual.Scheme, scheme, StringComparison.OrdinalIgnoreCase),
                () => $"Expected URI scheme not to be {Fmt(scheme, expr)}",
                isSkipped);

        public static CheckOperation HasHost(Uri? actual, string host, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && actual.IsAbsoluteUri && string.Equals(actual.Host, host, StringComparison.OrdinalIgnoreCase),
                () => $"Expected URI host to be {Fmt(host, expr)}, but was '{actual?.Host ?? "N/A"}'",
                isSkipped);

        public static CheckOperation DoesNotHaveHost(Uri? actual, string host, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is null || !actual.IsAbsoluteUri || !string.Equals(actual.Host, host, StringComparison.OrdinalIgnoreCase),
                () => $"Expected URI host not to be {Fmt(host, expr)}",
                isSkipped);

        public static CheckOperation HasPort(Uri? actual, int port, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && actual.IsAbsoluteUri && actual.Port == port,
                () => $"Expected URI port to be {Fmt(port, expr)}, but was {actual?.Port.ToString() ?? "N/A"}",
                isSkipped);

        public static CheckOperation DoesNotHavePort(Uri? actual, int port, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is null || !actual.IsAbsoluteUri || actual.Port != port,
                () => $"Expected URI port not to be {Fmt(port, expr)}",
                isSkipped);

        public static CheckOperation HasPath(Uri? actual, string path, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && actual.IsAbsoluteUri && string.Equals(actual.AbsolutePath, path, StringComparison.Ordinal),
                () => $"Expected URI path to be {Fmt(path, expr)}, but was '{actual?.AbsolutePath ?? "N/A"}'",
                isSkipped);

        public static CheckOperation DoesNotHavePath(Uri? actual, string path, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is null || !actual.IsAbsoluteUri || !string.Equals(actual.AbsolutePath, path, StringComparison.Ordinal),
                () => $"Expected URI path not to be {Fmt(path, expr)}",
                isSkipped);

        public static CheckOperation HasQueryParameter(Uri? actual, string paramName, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && actual.IsAbsoluteUri && ParseQueryString(actual.Query).ContainsKey(paramName),
                () => $"Expected URI to have query parameter {Fmt(paramName, expr)}",
                isSkipped);

        public static CheckOperation DoesNotHaveQueryParameter(Uri? actual, string paramName, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is null || !actual.IsAbsoluteUri || !ParseQueryString(actual.Query).ContainsKey(paramName),
                () => $"Expected URI not to have query parameter {Fmt(paramName, expr)}",
                isSkipped);

        public static CheckOperation HasQueryParameterValue(Uri? actual, string paramName, string expectedValue, bool isSkipped, string? nameExpr = null, string? valueExpr = null)
        {
            return CheckOperation.Sync(() =>
            {
                if (actual is null || !actual.IsAbsoluteUri) return false;
                var query = ParseQueryString(actual.Query);
                return query.TryGetValue(paramName, out var val) && string.Equals(val, expectedValue, StringComparison.Ordinal);
            },
            () => $"Expected URI query parameter {Fmt(paramName, nameExpr)} to be {Fmt(expectedValue, valueExpr)}",
            isSkipped);
        }

        public static CheckOperation HasFragment(Uri? actual, string fragment, bool isSkipped, string? expr = null)
        {
            return CheckOperation.Sync(() =>
            {
                if (actual is null || !actual.IsAbsoluteUri) return false;
                var actualFragment = actual.Fragment.TrimStart('#');
                var expected = fragment.TrimStart('#');
                return string.Equals(actualFragment, expected, StringComparison.Ordinal);
            },
            () => $"Expected URI fragment to be {Fmt(fragment, expr)}, but was '{actual?.Fragment.TrimStart('#') ?? "N/A"}'",
            isSkipped);
        }

        public static CheckOperation DoesNotHaveFragment(Uri? actual, string fragment, bool isSkipped, string? expr = null)
        {
            return CheckOperation.Sync(() =>
            {
                if (actual is null || !actual.IsAbsoluteUri) return true;
                var actualFragment = actual.Fragment.TrimStart('#');
                var expected = fragment.TrimStart('#');
                return !string.Equals(actualFragment, expected, StringComparison.Ordinal);
            },
            () => $"Expected URI fragment not to be {Fmt(fragment, expr)}",
            isSkipped);
        }

        private static Dictionary<string, string?> ParseQueryString(string query)
        {
            var result = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            var q = query.TrimStart('?');
            if (string.IsNullOrEmpty(q)) return result;
            foreach (var pair in q.Split('&'))
            {
                if (string.IsNullOrEmpty(pair)) continue;
                var idx = pair.IndexOf('=');
                if (idx < 0) result[Uri.UnescapeDataString(pair)] = null;
                else
                {
                    var key = Uri.UnescapeDataString(pair.Substring(0, idx));
                    var val = Uri.UnescapeDataString(pair.Substring(idx + 1));
                    result[key] = val;
                }
            }
            return result;
        }
    }
}

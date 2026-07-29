namespace Catchy.Sdk
{
    public static class ByteArrayChecks
    {
        static string Fmt(object? v, string? expr = null) => ExprFormat.Inline(v, expr);

        public static CheckOperation HasLength(byte[]? actual, int expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && actual.Length == expected,
                () => $"Expected byte[] to have length {Fmt(expected, expr)}, but was {actual?.Length.ToString() ?? "null"}",
                isSkipped);

        public static CheckOperation HasLengthGreaterThan(byte[]? actual, int expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && actual.Length > expected,
                () => $"Expected byte[] to have length > {Fmt(expected, expr)}, but was {actual?.Length.ToString() ?? "null"}",
                isSkipped);

        public static CheckOperation HasLengthLessThan(byte[]? actual, int expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && actual.Length < expected,
                () => $"Expected byte[] to have length < {Fmt(expected, expr)}, but was {actual?.Length.ToString() ?? "null"}",
                isSkipped);

        public static CheckOperation HasLengthInRange(byte[]? actual, int min, int max, bool isSkipped, string? minExpr = null, string? maxExpr = null)
            => CheckOperation.Sync(
                () => actual is not null && actual.Length >= min && actual.Length <= max,
                () => $"Expected byte[] to have length in [{Fmt(min, minExpr)}, {Fmt(max, maxExpr)}], but was {actual?.Length.ToString() ?? "null"}",
                isSkipped);

        public static CheckOperation IsEmpty(byte[]? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.Length == 0,
                () => $"Expected byte[] to be empty, but had {actual?.Length ?? 0} bytes",
                isSkipped);

        public static CheckOperation IsNotEmpty(byte[]? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.Length > 0,
                () => "Expected byte[] to be non-empty",
                isSkipped);

        public static CheckOperation SequenceEquals(byte[]? actual, byte[] expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && actual.AsSpan().SequenceEqual(expected),
                () => $"Expected byte[] to equal {expr ?? "expected"} (actual={actual?.Length}, expected={expected.Length})",
                isSkipped);

        public static CheckOperation DoesNotSequenceEqual(byte[]? actual, byte[] unexpected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is null || !actual.AsSpan().SequenceEqual(unexpected),
                () => $"Expected byte[] not to equal {expr ?? "expected"}",
                isSkipped);

        public static CheckOperation Contains(byte[]? actual, byte value, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && Array.IndexOf(actual, value) >= 0,
                () => $"Expected byte[] to contain {Fmt(value, expr)} (0x{value:X2})",
                isSkipped);

        public static CheckOperation DoesNotContain(byte[]? actual, byte value, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is null || Array.IndexOf(actual, value) < 0,
                () => $"Expected byte[] not to contain {Fmt(value, expr)} (0x{value:X2})",
                isSkipped);

        public static CheckOperation ContainsSequence(byte[]? actual, byte[] sequence, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && ContainsSubsequence(actual, sequence),
                () => $"Expected byte[] to contain {expr ?? "sequence"}",
                isSkipped);

        public static CheckOperation DoesNotContainSequence(byte[]? actual, byte[] sequence, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is null || !ContainsSubsequence(actual, sequence),
                () => $"Expected byte[] not to contain {expr ?? "sequence"}",
                isSkipped);

        public static CheckOperation StartsWith(byte[]? actual, byte[] prefix, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && actual.Length >= prefix.Length && actual.AsSpan(0, prefix.Length).SequenceEqual(prefix),
                () => $"Expected byte[] to start with {expr ?? "prefix"}",
                isSkipped);

        public static CheckOperation DoesNotStartWith(byte[]? actual, byte[] prefix, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is null || actual.Length < prefix.Length || !actual.AsSpan(0, prefix.Length).SequenceEqual(prefix),
                () => $"Expected byte[] not to start with {expr ?? "prefix"}",
                isSkipped);

        public static CheckOperation EndsWith(byte[]? actual, byte[] suffix, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && actual.Length >= suffix.Length && actual.AsSpan(actual.Length - suffix.Length).SequenceEqual(suffix),
                () => $"Expected byte[] to end with {expr ?? "suffix"}",
                isSkipped);

        public static CheckOperation DoesNotEndWith(byte[]? actual, byte[] suffix, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is null || actual.Length < suffix.Length || !actual.AsSpan(actual.Length - suffix.Length).SequenceEqual(suffix),
                () => $"Expected byte[] not to end with {expr ?? "suffix"}",
                isSkipped);

        public static CheckOperation IsHex(byte[]? actual, string hexString, bool isSkipped, string? expr = null)
        {
            var normalized = hexString.Replace("-", "").Replace(" ", "").ToUpperInvariant();
            return CheckOperation.Sync(
                () => actual is not null && BitConverter.ToString(actual).Replace("-", "").Equals(normalized, StringComparison.OrdinalIgnoreCase),
                () => $"Expected byte[] to equal hex {Fmt(hexString, expr)}, but was \"{(actual is null ? "null" : BitConverter.ToString(actual).Replace("-", ""))}\"",
                isSkipped);
        }

        public static CheckOperation IsNotHex(byte[]? actual, string hexString, bool isSkipped, string? expr = null)
        {
            var normalized = hexString.Replace("-", "").Replace(" ", "").ToUpperInvariant();
            return CheckOperation.Sync(
                () => actual is null || !BitConverter.ToString(actual).Replace("-", "").Equals(normalized, StringComparison.OrdinalIgnoreCase),
                () => $"Expected byte[] not to equal hex {Fmt(hexString, expr)}",
                isSkipped);
        }

        private static bool ContainsSubsequence(byte[] source, byte[] pattern)
        {
            if (pattern.Length == 0) return true;
            if (pattern.Length > source.Length) return false;
            for (int i = 0; i <= source.Length - pattern.Length; i++)
                if (source.AsSpan(i, pattern.Length).SequenceEqual(pattern))
                    return true;
            return false;
        }
    }
}

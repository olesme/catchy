namespace Catchy.Sdk
{
    public static class EnumChecks
    {
        static string Fmt(object? v, string? e = null) => ExprFormat.Inline(v, e);

        public static CheckOperation EqualTo<T>(T? actual, T expected, bool isSkipped, string? expectedExpr = null)
            where T : struct, Enum
            => CheckOperation.Sync(
                () => actual.HasValue && EqualityComparer<T>.Default.Equals(actual.Value, expected),
                () => $"Expected {Fmt(actual)} to equal {Fmt(expected, expectedExpr)}",
                isSkipped);

        public static CheckOperation NotEqualTo<T>(T? actual, T expected, bool isSkipped, string? expectedExpr = null)
            where T : struct, Enum
            => CheckOperation.Sync(
                () => actual.HasValue && !EqualityComparer<T>.Default.Equals(actual.Value, expected),
                () => $"Expected {Fmt(actual)} not to equal {Fmt(expected, expectedExpr)}",
                isSkipped);

        public static CheckOperation IsOneOf<T>(T? actual, T[] values, bool isSkipped, string? valuesExpr = null)
            where T : struct, Enum
            => CheckOperation.Sync(
                () => actual.HasValue && values.Any(v => EqualityComparer<T>.Default.Equals(actual.Value, v)),
                () => $"Expected {Fmt(actual)} to be one of [{string.Join(", ", values)}]",
                isSkipped);

        public static CheckOperation IsDefined<T>(T? actual, bool isSkipped)
            where T : struct, Enum
            => CheckOperation.Sync(
                () => actual.HasValue && Enum.IsDefined(typeof(T), actual.Value),
                () => $"Expected {Fmt(actual)} to be a defined value of {typeof(T).Name}",
                isSkipped);

        public static CheckOperation HasFlag<T>(T? actual, T flag, bool isSkipped, string? flagExpr = null)
            where T : struct, Enum
        {
            ulong flagVal = Convert.ToUInt64(flag);
            return CheckOperation.Sync(
                () => actual.HasValue && (Convert.ToUInt64(actual.Value) & flagVal) == flagVal,
                () => $"Expected {Fmt(actual)} to have flag {Fmt(flag, flagExpr)}",
                isSkipped);
        }

        public static CheckOperation HasNotFlag<T>(T? actual, T flag, bool isSkipped, string? flagExpr = null)
            where T : struct, Enum
        {
            ulong flagVal = Convert.ToUInt64(flag);
            return CheckOperation.Sync(
                () => !actual.HasValue || (Convert.ToUInt64(actual.Value) & flagVal) == 0,
                () => $"Expected {Fmt(actual)} not to have flag {Fmt(flag, flagExpr)}",
                isSkipped);
        }
    }
}

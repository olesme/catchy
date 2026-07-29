using System.Numerics;

namespace Catchy.Sdk
{
    public static class NumericChecks
    {
        static string Fmt(object? v, string? e = null) => ExprFormat.Inline(v, e);

        public static CheckOperation EqualTo<T>(T? actual, T expected, bool isSkipped, string? expectedExpr = null)
            where T : struct, IComparable<T>
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.CompareTo(expected) == 0,
                () => $"Expected {Fmt(actual)} to equal {Fmt(expected, expectedExpr)}",
                isSkipped);

        public static CheckOperation NotEqualTo<T>(T? actual, T expected, bool isSkipped, string? expectedExpr = null)
            where T : struct, IComparable<T>
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.CompareTo(expected) != 0,
                () => $"Expected {Fmt(actual)} not to equal {Fmt(expected, expectedExpr)}",
                isSkipped);

        public static CheckOperation GreaterThan<T>(T? actual, T threshold, bool isSkipped, string? thresholdExpr = null)
            where T : struct, IComparable<T>
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.CompareTo(threshold) > 0,
                () => $"Expected {Fmt(actual)} to be greater than {Fmt(threshold, thresholdExpr)}",
                isSkipped);

        public static CheckOperation GreaterThanOrEqualTo<T>(T? actual, T threshold, bool isSkipped, string? thresholdExpr = null)
            where T : struct, IComparable<T>
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.CompareTo(threshold) >= 0,
                () => $"Expected {Fmt(actual)} to be >= {Fmt(threshold, thresholdExpr)}",
                isSkipped);

        public static CheckOperation LessThan<T>(T? actual, T threshold, bool isSkipped, string? thresholdExpr = null)
            where T : struct, IComparable<T>
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.CompareTo(threshold) < 0,
                () => $"Expected {Fmt(actual)} to be less than {Fmt(threshold, thresholdExpr)}",
                isSkipped);

        public static CheckOperation LessThanOrEqualTo<T>(T? actual, T threshold, bool isSkipped, string? thresholdExpr = null)
            where T : struct, IComparable<T>
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.CompareTo(threshold) <= 0,
                () => $"Expected {Fmt(actual)} to be <= {Fmt(threshold, thresholdExpr)}",
                isSkipped);

        public static CheckOperation InRange<T>(T? actual, T min, T max, bool isSkipped, string? minExpr = null, string? maxExpr = null)
            where T : struct, IComparable<T>
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.CompareTo(min) >= 0 && actual.Value.CompareTo(max) <= 0,
                () => $"Expected {Fmt(actual)} to be in [{Fmt(min, minExpr)}, {Fmt(max, maxExpr)}]",
                isSkipped);

        public static CheckOperation IsPositive<T>(T? actual, bool isSkipped)
            where T : struct, IComparable<T>
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.CompareTo(default) > 0,
                () => $"Expected {Fmt(actual)} to be positive",
                isSkipped);

        public static CheckOperation IsNegative<T>(T? actual, bool isSkipped)
            where T : struct, IComparable<T>
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.CompareTo(default) < 0,
                () => $"Expected {Fmt(actual)} to be negative",
                isSkipped);

        public static CheckOperation IsZero<T>(T? actual, bool isSkipped)
            where T : struct, IComparable<T>
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.CompareTo(default) == 0,
                () => $"Expected {Fmt(actual)} to be zero",
                isSkipped);

        public static CheckOperation GreaterThan<T>(Func<T?> actualProvider, T threshold, bool isSkipped, string? thresholdExpr = null)
            where T : struct, IComparable<T>
            => CheckOperation.Sync(
                () =>
                {
                    var actual = actualProvider();
                    return actual.HasValue && actual.Value.CompareTo(threshold) > 0;
                },
                () => $"Expected {Fmt(actualProvider())} to be greater than {Fmt(threshold, thresholdExpr)}",
                isSkipped);

        public static CheckOperation GreaterThanOrEqualTo<T>(Func<T?> actualProvider, T threshold, bool isSkipped, string? thresholdExpr = null)
            where T : struct, IComparable<T>
            => CheckOperation.Sync(
                () =>
                {
                    var actual = actualProvider();
                    return actual.HasValue && actual.Value.CompareTo(threshold) >= 0;
                },
                () => $"Expected {Fmt(actualProvider())} to be >= {Fmt(threshold, thresholdExpr)}",
                isSkipped);

        public static CheckOperation LessThan<T>(Func<T?> actualProvider, T threshold, bool isSkipped, string? thresholdExpr = null)
            where T : struct, IComparable<T>
            => CheckOperation.Sync(
                () =>
                {
                    var actual = actualProvider();
                    return actual.HasValue && actual.Value.CompareTo(threshold) < 0;
                },
                () => $"Expected {Fmt(actualProvider())} to be less than {Fmt(threshold, thresholdExpr)}",
                isSkipped);

        public static CheckOperation LessThanOrEqualTo<T>(Func<T?> actualProvider, T threshold, bool isSkipped, string? thresholdExpr = null)
            where T : struct, IComparable<T>
            => CheckOperation.Sync(
                () =>
                {
                    var actual = actualProvider();
                    return actual.HasValue && actual.Value.CompareTo(threshold) <= 0;
                },
                () => $"Expected {Fmt(actualProvider())} to be <= {Fmt(threshold, thresholdExpr)}",
                isSkipped);

        public static CheckOperation InRange<T>(Func<T?> actualProvider, T min, T max, bool isSkipped, string? minExpr = null, string? maxExpr = null)
            where T : struct, IComparable<T>
            => CheckOperation.Sync(
                () =>
                {
                    var actual = actualProvider();
                    return actual.HasValue
                        && actual.Value.CompareTo(min) >= 0
                        && actual.Value.CompareTo(max) <= 0;
                },
                () => $"Expected {Fmt(actualProvider())} to be in [{Fmt(min, minExpr)}, {Fmt(max, maxExpr)}]",
                isSkipped);

        // ── Reference-type IComparable support (string, DateTime, TimeSpan, Guid, Version, etc.) ──
        // These methods support any type implementing IComparable<T>, not just structs.
        // Null values are treated as a failure unless explicitly testing for null.

        public static CheckOperation GreaterThan<T>(T? actual, T threshold, bool isSkipped, string? thresholdExpr = null)
            where T : class, IComparable<T>
            => CheckOperation.Sync(
                () => actual is not null && actual.CompareTo(threshold) > 0,
                () => $"Expected {Fmt(actual)} to be greater than {Fmt(threshold, thresholdExpr)}",
                isSkipped);

        public static CheckOperation GreaterThanOrEqualTo<T>(T? actual, T threshold, bool isSkipped, string? thresholdExpr = null)
            where T : class, IComparable<T>
            => CheckOperation.Sync(
                () => actual is not null && actual.CompareTo(threshold) >= 0,
                () => $"Expected {Fmt(actual)} to be >= {Fmt(threshold, thresholdExpr)}",
                isSkipped);

        public static CheckOperation LessThan<T>(T? actual, T threshold, bool isSkipped, string? thresholdExpr = null)
            where T : class, IComparable<T>
            => CheckOperation.Sync(
                () => actual is not null && actual.CompareTo(threshold) < 0,
                () => $"Expected {Fmt(actual)} to be less than {Fmt(threshold, thresholdExpr)}",
                isSkipped);

        public static CheckOperation LessThanOrEqualTo<T>(T? actual, T threshold, bool isSkipped, string? thresholdExpr = null)
            where T : class, IComparable<T>
            => CheckOperation.Sync(
                () => actual is not null && actual.CompareTo(threshold) <= 0,
                () => $"Expected {Fmt(actual)} to be <= {Fmt(threshold, thresholdExpr)}",
                isSkipped);

        public static CheckOperation InRange<T>(T? actual, T min, T max, bool isSkipped, string? minExpr = null, string? maxExpr = null)
            where T : class, IComparable<T>
            => CheckOperation.Sync(
                () => actual is not null && actual.CompareTo(min) >= 0 && actual.CompareTo(max) <= 0,
                () => $"Expected {Fmt(actual)} to be in [{Fmt(min, minExpr)}, {Fmt(max, maxExpr)}]",
                isSkipped);

        public static CheckOperation GreaterThan<T>(Func<T?> actualProvider, T threshold, bool isSkipped, string? thresholdExpr = null)
            where T : class, IComparable<T>
            => CheckOperation.Sync(
                () => actualProvider() is not null && actualProvider()!.CompareTo(threshold) > 0,
                () => $"Expected {Fmt(actualProvider())} to be greater than {Fmt(threshold, thresholdExpr)}",
                isSkipped);

        public static CheckOperation GreaterThanOrEqualTo<T>(Func<T?> actualProvider, T threshold, bool isSkipped, string? thresholdExpr = null)
            where T : class, IComparable<T>
            => CheckOperation.Sync(
                () => actualProvider() is not null && actualProvider()!.CompareTo(threshold) >= 0,
                () => $"Expected {Fmt(actualProvider())} to be >= {Fmt(threshold, thresholdExpr)}",
                isSkipped);

        public static CheckOperation LessThan<T>(Func<T?> actualProvider, T threshold, bool isSkipped, string? thresholdExpr = null)
            where T : class, IComparable<T>
            => CheckOperation.Sync(
                () => actualProvider() is not null && actualProvider()!.CompareTo(threshold) < 0,
                () => $"Expected {Fmt(actualProvider())} to be less than {Fmt(threshold, thresholdExpr)}",
                isSkipped);

        public static CheckOperation LessThanOrEqualTo<T>(Func<T?> actualProvider, T threshold, bool isSkipped, string? thresholdExpr = null)
            where T : class, IComparable<T>
            => CheckOperation.Sync(
                () => actualProvider() is not null && actualProvider()!.CompareTo(threshold) <= 0,
                () => $"Expected {Fmt(actualProvider())} to be <= {Fmt(threshold, thresholdExpr)}",
                isSkipped);

        public static CheckOperation InRange<T>(Func<T?> actualProvider, T min, T max, bool isSkipped, string? minExpr = null, string? maxExpr = null)
            where T : class, IComparable<T>
            => CheckOperation.Sync(
                () => actualProvider() is not null
                    && actualProvider()!.CompareTo(min) >= 0
                    && actualProvider()!.CompareTo(max) <= 0,
                () => $"Expected {Fmt(actualProvider())} to be in [{Fmt(min, minExpr)}, {Fmt(max, maxExpr)}]",
                isSkipped);

        public static CheckOperation IsPositive<T>(Func<T?> actualProvider, bool isSkipped)
            where T : struct, IComparable<T>
            => CheckOperation.Sync(
                () =>
                {
                    var actual = actualProvider();
                    return actual.HasValue && actual.Value.CompareTo(default) > 0;
                },
                () => $"Expected {Fmt(actualProvider())} to be positive",
                isSkipped);

        public static CheckOperation IsNegative<T>(Func<T?> actualProvider, bool isSkipped)
            where T : struct, IComparable<T>
            => CheckOperation.Sync(
                () =>
                {
                    var actual = actualProvider();
                    return actual.HasValue && actual.Value.CompareTo(default) < 0;
                },
                () => $"Expected {Fmt(actualProvider())} to be negative",
                isSkipped);

        public static CheckOperation IsZero<T>(Func<T?> actualProvider, bool isSkipped)
            where T : struct, IComparable<T>
            => CheckOperation.Sync(
                () =>
                {
                    var actual = actualProvider();
                    return actual.HasValue && actual.Value.CompareTo(default) == 0;
                },
                () => $"Expected {Fmt(actualProvider())} to be zero",
                isSkipped);

        public static CheckOperation IsBetween<T>(Func<T?> actualProvider, T min, T max, BetweenOptions opts, bool isSkipped, string? minExpr = null, string? maxExpr = null)
            where T : struct, IComparable<T>
            => CheckOperation.Sync(
                () =>
                {
                    var actual = actualProvider();
                    return actual.HasValue && (opts.Exclusive
                        ? actual.Value.CompareTo(min) > 0 && actual.Value.CompareTo(max) < 0
                        : actual.Value.CompareTo(min) >= 0 && actual.Value.CompareTo(max) <= 0);
                },
                () =>
                {
                    var range = opts.Exclusive
                        ? $"({Fmt(min, minExpr)}, {Fmt(max, maxExpr)})"
                        : $"[{Fmt(min, minExpr)}, {Fmt(max, maxExpr)}]";
                    return $"Expected {Fmt(actualProvider())} to be in range {range}";
                },
                isSkipped);

        public static CheckOperation IsBetween<T>(Func<T?> actualProvider, T min, T max, BetweenOptions opts, bool isSkipped, string? minExpr = null, string? maxExpr = null)
            where T : class, IComparable<T>
            => CheckOperation.Sync(
                () => actualProvider() is not null && (opts.Exclusive
                    ? actualProvider()!.CompareTo(min) > 0 && actualProvider()!.CompareTo(max) < 0
                    : actualProvider()!.CompareTo(min) >= 0 && actualProvider()!.CompareTo(max) <= 0),
                () =>
                {
                    var range = opts.Exclusive
                        ? $"({Fmt(min, minExpr)}, {Fmt(max, maxExpr)})"
                        : $"[{Fmt(min, minExpr)}, {Fmt(max, maxExpr)}]";
                    return $"Expected {Fmt(actualProvider())} to be in range {range}";
                },
                isSkipped);

        public static CheckOperation IsMultipleOf(ulong actual, ulong divisor, bool isSkipped, string? divisorExpr = null)
            => CheckOperation.Sync(
                () => divisor != 0 && actual % divisor == 0,
                () => $"Expected {actual} to be a multiple of {Fmt(divisor, divisorExpr)}",
                isSkipped);

        public static CheckOperation IsNotMultipleOf(ulong actual, ulong divisor, bool isSkipped, string? divisorExpr = null)
            => CheckOperation.Sync(
                () => divisor == 0 || actual % divisor != 0,
                () => divisor == 0 ? "Divisor cannot be zero"
                    : $"Expected {actual} not to be divisible by {Fmt(divisor, divisorExpr)}, but it is",
                isSkipped);

        public static CheckOperation IsMultipleOf(nuint actual, nuint divisor, bool isSkipped, string? divisorExpr = null)
            => CheckOperation.Sync(
                () => divisor != 0 && actual % divisor == 0,
                () => $"Expected {actual} to be a multiple of {Fmt(divisor, divisorExpr)}",
                isSkipped);

        public static CheckOperation IsNotMultipleOf(nuint actual, nuint divisor, bool isSkipped, string? divisorExpr = null)
            => CheckOperation.Sync(
                () => divisor == 0 || actual % divisor != 0,
                () => divisor == 0 ? "Divisor cannot be zero"
                    : $"Expected {actual} not to be divisible by {Fmt(divisor, divisorExpr)}, but it is",
                isSkipped);

        public static CheckOperation IsMultipleOf(nint actual, nint divisor, bool isSkipped, string? divisorExpr = null)
            => CheckOperation.Sync(
                () => divisor != 0 && actual % divisor == 0,
                () => $"Expected {actual} to be divisible by {Fmt(divisor, divisorExpr)}",
                isSkipped);

        public static CheckOperation IsNotMultipleOf(nint actual, nint divisor, bool isSkipped, string? divisorExpr = null)
            => CheckOperation.Sync(
                () => divisor == 0 || actual % divisor != 0,
                () => divisor == 0 ? "Divisor cannot be zero"
                    : $"Expected {actual} not to be divisible by {Fmt(divisor, divisorExpr)}, but it is",
                isSkipped);

        public static CheckOperation IsEven(ulong actual, bool isSkipped)
            => CheckOperation.Sync(() => actual % 2UL == 0, () => $"Expected {actual} to be even", isSkipped);

        public static CheckOperation IsOdd(ulong actual, bool isSkipped)
            => CheckOperation.Sync(() => actual % 2UL != 0, () => $"Expected {actual} to be odd", isSkipped);

        public static CheckOperation IsEven(nuint actual, bool isSkipped)
            => CheckOperation.Sync(() => actual % 2 == 0, () => $"Expected {actual} to be even", isSkipped);

        public static CheckOperation IsOdd(nuint actual, bool isSkipped)
            => CheckOperation.Sync(() => actual % 2 != 0, () => $"Expected {actual} to be odd", isSkipped);

        public static CheckOperation IsEven(BigInteger actual, bool isSkipped)
            => CheckOperation.Sync(() => actual % 2 == 0, () => $"Expected {actual} to be even", isSkipped);

        public static CheckOperation IsOdd(BigInteger actual, bool isSkipped)
            => CheckOperation.Sync(() => actual % 2 != 0, () => $"Expected {actual} to be odd", isSkipped);

        public static CheckOperation IsMultipleOf(BigInteger actual, BigInteger divisor, bool isSkipped, string? divisorExpr = null)
            => CheckOperation.Sync(
                () => divisor != 0 && actual % divisor == 0,
                () => $"Expected {actual} to be a multiple of {Fmt(divisor, divisorExpr)}",
                isSkipped);

        public static CheckOperation IsEven(BigInteger? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is { } v && v % 2 == 0,
                () => actual is null ? "Expected a value, but was null" : $"Expected {actual} to be even",
                isSkipped);

        public static CheckOperation IsOdd(BigInteger? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is { } v && v % 2 != 0,
                () => actual is null ? "Expected a value, but was null" : $"Expected {actual} to be odd",
                isSkipped);

        public static CheckOperation IsMultipleOf(BigInteger? actual, BigInteger divisor, bool isSkipped, string? divisorExpr = null)
            => CheckOperation.Sync(
                () => actual is { } v && divisor != 0 && v % divisor == 0,
                () => actual is null ? "Expected a value, but was null" : $"Expected {actual} to be a multiple of {Fmt(divisor, divisorExpr)}",
                isSkipped);

#if NET7_0_OR_GREATER
        public static CheckOperation IsEven(Int128 actual, bool isSkipped)
            => CheckOperation.Sync(() => actual % 2 == 0, () => $"Expected {actual} to be even", isSkipped);

        public static CheckOperation IsOdd(Int128 actual, bool isSkipped)
            => CheckOperation.Sync(() => actual % 2 != 0, () => $"Expected {actual} to be odd", isSkipped);

        public static CheckOperation IsMultipleOf(Int128 actual, Int128 divisor, bool isSkipped, string? divisorExpr = null)
            => CheckOperation.Sync(
                () => divisor != 0 && actual % divisor == 0,
                () => $"Expected {actual} to be a multiple of {Fmt(divisor, divisorExpr)}",
                isSkipped);

        public static CheckOperation IsEven(Int128? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is { } v && v % 2 == 0,
                () => actual is null ? "Expected a value, but was null" : $"Expected {actual} to be even",
                isSkipped);

        public static CheckOperation IsOdd(Int128? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is { } v && v % 2 != 0,
                () => actual is null ? "Expected a value, but was null" : $"Expected {actual} to be odd",
                isSkipped);

        public static CheckOperation IsMultipleOf(Int128? actual, Int128 divisor, bool isSkipped, string? divisorExpr = null)
            => CheckOperation.Sync(
                () => actual is { } v && divisor != 0 && v % divisor == 0,
                () => actual is null ? "Expected a value, but was null" : $"Expected {actual} to be a multiple of {Fmt(divisor, divisorExpr)}",
                isSkipped);

        public static CheckOperation IsEven(UInt128 actual, bool isSkipped)
            => CheckOperation.Sync(() => actual % 2 == 0, () => $"Expected {actual} to be even", isSkipped);

        public static CheckOperation IsOdd(UInt128 actual, bool isSkipped)
            => CheckOperation.Sync(() => actual % 2 != 0, () => $"Expected {actual} to be odd", isSkipped);
#endif

        public static CheckOperation IsBetween<T>(
            T? actual, T min, T max,
            BetweenOptions opts,
            bool isSkipped,
            string? minExpr = null,
            string? maxExpr = null)
            where T : struct, IComparable<T>
        {
            return CheckOperation.Sync(
                () =>
                {
                    if (!actual.HasValue) return false;
                    return opts.Exclusive
                        ? actual.Value.CompareTo(min) > 0 && actual.Value.CompareTo(max) < 0
                        : actual.Value.CompareTo(min) >= 0 && actual.Value.CompareTo(max) <= 0;
                },
                () =>
                {
                    if (!actual.HasValue) return "Expected a value, but was null";
                    var range = opts.Exclusive
                        ? $"({ExprFormat.Inline(min, minExpr)}, {ExprFormat.Inline(max, maxExpr)})"
                        : $"[{ExprFormat.Inline(min, minExpr)}, {ExprFormat.Inline(max, maxExpr)}]";
                    return $"Expected {actual.Value} to be in range {range}";
                },
                isSkipped);
        }

        public static CheckOperation IsBetweenNonNullable<T>(
            T actual, T min, T max,
            BetweenOptions opts,
            bool isSkipped,
            string? minExpr = null,
            string? maxExpr = null)
            where T : struct, IComparable<T>
            => IsBetween<T>(actual, min, max, opts, isSkipped, minExpr, maxExpr);
    }
}

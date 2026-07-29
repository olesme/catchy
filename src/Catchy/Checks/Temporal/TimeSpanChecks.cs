namespace Catchy.Sdk
{
    public static class TimeSpanChecks
    {
        static string Fmt(object? v) => ValueFormatter.Format(v);

        public static CheckOperation EqualTo(TimeSpan? actual, TimeSpan expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value == expected,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected {Fmt(actual.Value)} to equal {Fmt(expected)}",
                isSkipped);

        public static CheckOperation NotEqualTo(TimeSpan? actual, TimeSpan expected, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value != expected,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected TimeSpan not to equal {Fmt(expected)}",
                isSkipped);

        public static CheckOperation GreaterThan(TimeSpan? actual, TimeSpan expected, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value > expected,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected {Fmt(actual.Value)} to be greater than {Fmt(expected)}",
                isSkipped);

        public static CheckOperation GreaterThanOrEqualTo(TimeSpan? actual, TimeSpan expected, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value >= expected,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected {Fmt(actual.Value)} to be at least {Fmt(expected)}",
                isSkipped);

        public static CheckOperation LessThan(TimeSpan? actual, TimeSpan expected, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value < expected,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected {Fmt(actual.Value)} to be less than {Fmt(expected)}",
                isSkipped);

        public static CheckOperation LessThanOrEqualTo(TimeSpan? actual, TimeSpan expected, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value <= expected,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected {Fmt(actual.Value)} to be at most {Fmt(expected)}",
                isSkipped);

        public static CheckOperation IsBetween(TimeSpan? actual, TimeSpan min, TimeSpan max, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value >= min && actual.Value <= max,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected {Fmt(actual.Value)} to be between {Fmt(min)} and {Fmt(max)}",
                isSkipped);

        public static CheckOperation IsBetween(TimeSpan? actual, TimeSpan min, TimeSpan max, BetweenOptions opts, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && (opts.Exclusive
                    ? actual.Value > min && actual.Value < max
                    : actual.Value >= min && actual.Value <= max),
                () => actual is null ? "Expected a value, but was null"
                    : opts.Exclusive
                        ? $"Expected {Fmt(actual.Value)} to be exclusively between ({Fmt(min)}, {Fmt(max)})"
                        : $"Expected {Fmt(actual.Value)} to be between [{Fmt(min)}, {Fmt(max)}]",
                isSkipped);

        public static CheckOperation IsCloseTo(TimeSpan? actual, TimeSpan expected, TimeSpan within, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && (actual.Value - expected).Duration() <= within,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected {Fmt(actual.Value)} to be within {within} of {Fmt(expected)}, but difference was {(actual.Value - expected).Duration()}",
                isSkipped);

        public static CheckOperation IsPositive(TimeSpan? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value > TimeSpan.Zero,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected {Fmt(actual.Value)} to be positive",
                isSkipped);

        public static CheckOperation IsNegative(TimeSpan? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value < TimeSpan.Zero,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected {Fmt(actual.Value)} to be negative",
                isSkipped);

        public static CheckOperation IsZero(TimeSpan? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value == TimeSpan.Zero,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected {Fmt(actual.Value)} to be zero",
                isSkipped);

        public static CheckOperation HasDays(TimeSpan? actual, int days, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.Days == days,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected days component to be {days}, but was {actual.Value.Days}",
                isSkipped);

        public static CheckOperation HasHours(TimeSpan? actual, int hours, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.Hours == hours,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected hours component to be {hours}, but was {actual.Value.Hours}",
                isSkipped);

        public static CheckOperation HasMinutes(TimeSpan? actual, int minutes, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.Minutes == minutes,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected minutes component to be {minutes}, but was {actual.Value.Minutes}",
                isSkipped);

        public static CheckOperation HasSeconds(TimeSpan? actual, int seconds, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.Seconds == seconds,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected seconds component to be {seconds}, but was {actual.Value.Seconds}",
                isSkipped);
    }
}

namespace Catchy.Sdk
{
    public static class DateTimeChecks
    {
        static string Fmt(object? v) => ValueFormatter.Format(v);

        // DateTime

        public static CheckOperation EqualTo(DateTime? actual, DateTime expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value == expected,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected {Fmt(actual.Value)} to equal {Fmt(expected)}",
                isSkipped);

        public static CheckOperation IsToday(DateTime? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.Date == DateTime.Today,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected {Fmt(actual.Value)} to be today",
                isSkipped);

        public static CheckOperation NotEqualTo(DateTime? actual, DateTime expected, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value != expected,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected DateTime not to equal {Fmt(expected)}",
                isSkipped);

        public static CheckOperation IsBefore(DateTime? actual, DateTime expected, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value < expected,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected {Fmt(actual.Value)} to be before {Fmt(expected)}",
                isSkipped);

        public static CheckOperation IsAfter(DateTime? actual, DateTime expected, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value > expected,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected {Fmt(actual.Value)} to be after {Fmt(expected)}",
                isSkipped);

        public static CheckOperation IsBetween(DateTime? actual, DateTime min, DateTime max, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value >= min && actual.Value <= max,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected {Fmt(actual.Value)} to be between {Fmt(min)} and {Fmt(max)}",
                isSkipped);

        public static CheckOperation IsBetween(DateTime? actual, DateTime min, DateTime max, BetweenOptions opts, bool isSkipped)
    => CheckOperation.Sync(
        () => {
            if (!actual.HasValue) return false;
            return opts.Exclusive
                ? actual.Value > min && actual.Value < max
                : actual.Value >= min && actual.Value <= max;
        },
        () => actual is null ? "Expected a value, but was null"
            : opts.Exclusive
                ? $"Expected {Fmt(actual.Value)} to be exclusively between ({Fmt(min)}, {Fmt(max)})"
                : $"Expected {Fmt(actual.Value)} to be between [{Fmt(min)}, {Fmt(max)}]",
        isSkipped);

        public static CheckOperation IsBetween(DateTimeOffset? actual, DateTimeOffset min, DateTimeOffset max, BetweenOptions opts, bool isSkipped)
            => CheckOperation.Sync(
                () => {
                    if (!actual.HasValue) return false;
                    return opts.Exclusive
                        ? actual.Value > min && actual.Value < max
                        : actual.Value >= min && actual.Value <= max;
                },
                () => actual is null ? "Expected a value, but was null"
                    : opts.Exclusive
                        ? $"Expected {Fmt(actual.Value)} to be exclusively between ({Fmt(min)}, {Fmt(max)})"
                        : $"Expected {Fmt(actual.Value)} to be between [{Fmt(min)}, {Fmt(max)}]",
                isSkipped);

        public static CheckOperation IsCloseTo(DateTime? actual, DateTime expected, TimeSpan within, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && (actual.Value - expected).Duration() <= within,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected {Fmt(actual.Value)} to be within {within} of {Fmt(expected)}, but difference was {(actual.Value - expected).Duration()}",
                isSkipped);

        public static CheckOperation IsAtLeast(DateTime? actual, DateTime expected, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value >= expected,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected {Fmt(actual.Value)} to be at least {Fmt(expected)}",
                isSkipped);

        public static CheckOperation IsAtMost(DateTime? actual, DateTime expected, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value <= expected,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected {Fmt(actual.Value)} to be at most {Fmt(expected)}",
                isSkipped);

        public static CheckOperation HasYear(DateTime? actual, int year, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.Year == year,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected year to be {year}, but was {actual.Value.Year}",
                isSkipped);

        public static CheckOperation HasMonth(DateTime? actual, int month, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.Month == month,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected month to be {month}, but was {actual.Value.Month}",
                isSkipped);

        public static CheckOperation HasDay(DateTime? actual, int day, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.Day == day,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected day to be {day}, but was {actual.Value.Day}",
                isSkipped);

        public static CheckOperation HasHour(DateTime? actual, int hour, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.Hour == hour,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected hour to be {hour}, but was {actual.Value.Hour}",
                isSkipped);

        public static CheckOperation HasMinute(DateTime? actual, int minute, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.Minute == minute,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected minute to be {minute}, but was {actual.Value.Minute}",
                isSkipped);

        public static CheckOperation HasSecond(DateTime? actual, int second, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.Second == second,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected second to be {second}, but was {actual.Value.Second}",
                isSkipped);

        public static CheckOperation IsUtc(DateTime? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.Kind == DateTimeKind.Utc,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected DateTime to have UTC kind, but was {actual.Value.Kind}",
                isSkipped);

        public static CheckOperation IsLocal(DateTime? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.Kind == DateTimeKind.Local,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected DateTime to have Local kind, but was {actual.Value.Kind}",
                isSkipped);

        public static CheckOperation IsInThePast(DateTime? actual, bool isSkipped)
            => CheckOperation.Sync(
                () =>
                {
                    if (!actual.HasValue) return false;
                    var now = actual.Value.Kind == DateTimeKind.Utc ? DateTime.UtcNow : DateTime.Now;
                    return actual.Value < now;
                },
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected {Fmt(actual.Value)} to be in the past",
                isSkipped);

        public static CheckOperation IsInTheFuture(DateTime? actual, bool isSkipped)
            => CheckOperation.Sync(
                () =>
                {
                    if (!actual.HasValue) return false;
                    var now = actual.Value.Kind == DateTimeKind.Utc ? DateTime.UtcNow : DateTime.Now;
                    return actual.Value > now;
                },
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected {Fmt(actual.Value)} to be in the future",
                isSkipped);

        public static CheckOperation IsWithinTheLast(DateTime? actual, TimeSpan duration, bool isSkipped)
            => CheckOperation.Sync(
                () =>
                {
                    if (!actual.HasValue) return false;
                    var now = actual.Value.Kind == DateTimeKind.Utc ? DateTime.UtcNow : DateTime.Now;
                    return actual.Value >= now - duration && actual.Value <= now;
                },
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected {Fmt(actual.Value)} to be within the last {duration}",
                isSkipped);

        public static CheckOperation IsWithinTheNext(DateTime? actual, TimeSpan duration, bool isSkipped)
            => CheckOperation.Sync(
                () =>
                {
                    if (!actual.HasValue) return false;
                    var now = actual.Value.Kind == DateTimeKind.Utc ? DateTime.UtcNow : DateTime.Now;
                    return actual.Value >= now && actual.Value <= now + duration;
                },
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected {Fmt(actual.Value)} to be within the next {duration}",
                isSkipped);

        // DateTimeOffset

        public static CheckOperation EqualTo(DateTimeOffset? actual, DateTimeOffset expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value == expected,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected {Fmt(actual.Value)} to equal {Fmt(expected)}",
                isSkipped);

        public static CheckOperation NotEqualTo(DateTimeOffset? actual, DateTimeOffset expected, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value != expected,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected DateTimeOffset not to equal {Fmt(expected)}",
                isSkipped);

        public static CheckOperation IsBefore(DateTimeOffset? actual, DateTimeOffset expected, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value < expected,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected {Fmt(actual.Value)} to be before {Fmt(expected)}",
                isSkipped);

        public static CheckOperation IsAfter(DateTimeOffset? actual, DateTimeOffset expected, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value > expected,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected {Fmt(actual.Value)} to be after {Fmt(expected)}",
                isSkipped);

        public static CheckOperation IsBetween(DateTimeOffset? actual, DateTimeOffset min, DateTimeOffset max, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value >= min && actual.Value <= max,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected {Fmt(actual.Value)} to be between {Fmt(min)} and {Fmt(max)}",
                isSkipped);

        public static CheckOperation IsCloseTo(DateTimeOffset? actual, DateTimeOffset expected, TimeSpan within, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && (actual.Value - expected).Duration() <= within,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected {Fmt(actual.Value)} to be within {within} of {Fmt(expected)}, but difference was {(actual.Value - expected).Duration()}",
                isSkipped);

        public static CheckOperation HasOffset(DateTimeOffset? actual, TimeSpan expectedOffset, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.Offset == expectedOffset,
                () => actual is null ? "Expected a value, but was null"
                    : $"Expected offset to be {expectedOffset}, but was {actual.Value.Offset}",
                isSkipped);

        public static CheckOperation IsInThePast(DateTimeOffset? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value < DateTimeOffset.UtcNow,
                () => actual is null ? "Expected a value, but was null" : $"Expected {Fmt(actual.Value)} to be in the past",
                isSkipped);

        public static CheckOperation IsInTheFuture(DateTimeOffset? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value > DateTimeOffset.UtcNow,
                () => actual is null ? "Expected a value, but was null" : $"Expected {Fmt(actual.Value)} to be in the future",
                isSkipped);

        public static CheckOperation IsWithinTheLast(DateTimeOffset? actual, TimeSpan duration, bool isSkipped)
            => CheckOperation.Sync(
                () => { var now = DateTimeOffset.UtcNow; return actual.HasValue && actual.Value >= now - duration && actual.Value <= now; },
                () => actual is null ? "Expected a value, but was null" : $"Expected {Fmt(actual.Value)} to be within the last {duration}",
                isSkipped);

        public static CheckOperation IsWithinTheNext(DateTimeOffset? actual, TimeSpan duration, bool isSkipped)
            => CheckOperation.Sync(
                () => { var now = DateTimeOffset.UtcNow; return actual >= now && actual <= now + duration; },
                () => $"Expected {Fmt(actual)} to be within the next {duration}",
                isSkipped);

        #if NET6_0_OR_GREATER
        public static CheckOperation IsBetween(DateOnly? actual, DateOnly min, DateOnly max, BetweenOptions opts, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && (opts.Exclusive ? actual.Value > min && actual.Value < max : actual.Value >= min && actual.Value <= max),
                () => actual is null ? "Expected a value, but was null"
                    : opts.Exclusive
                        ? $"Expected {actual.Value:yyyy-MM-dd} to be exclusively between ({min:yyyy-MM-dd}, {max:yyyy-MM-dd})"
                        : $"Expected {actual.Value:yyyy-MM-dd} to be between [{min:yyyy-MM-dd}, {max:yyyy-MM-dd}]",
                isSkipped);

        public static CheckOperation IsBetween(TimeOnly? actual, TimeOnly min, TimeOnly max, BetweenOptions opts, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && (opts.Exclusive ? actual.Value > min && actual.Value < max : actual.Value >= min && actual.Value <= max),
                () => actual is null ? "Expected a value, but was null"
                    : opts.Exclusive
                        ? $"Expected {actual.Value:HH:mm:ss} to be exclusively between ({min:HH:mm:ss}, {max:HH:mm:ss})"
                        : $"Expected {actual.Value:HH:mm:ss} to be between [{min:HH:mm:ss}, {max:HH:mm:ss}]",
                isSkipped);

        public static CheckOperation IsBetweenTemporalValue<T>(T actual, T min, T max, BetweenOptions opts, bool isSkipped)
        {
            if (typeof(T) == typeof(DateOnly) || typeof(T) == typeof(DateOnly?))
            {
                var minDate = AsDateOnly(min);
                var maxDate = AsDateOnly(max);
                if (!minDate.HasValue || !maxDate.HasValue)
                {
                    return CheckOperation.Sync(() => false, () => "Expected non-null range bounds", isSkipped);
                }

                return IsBetween(AsDateOnly(actual), minDate.Value, maxDate.Value, opts, isSkipped);
            }

            if (typeof(T) == typeof(TimeOnly) || typeof(T) == typeof(TimeOnly?))
            {
                var minTime = AsTimeOnly(min);
                var maxTime = AsTimeOnly(max);
                if (!minTime.HasValue || !maxTime.HasValue)
                {
                    return CheckOperation.Sync(() => false, () => "Expected non-null range bounds", isSkipped);
                }

                return IsBetween(AsTimeOnly(actual), minTime.Value, maxTime.Value, opts, isSkipped);
            }

            return CheckOperation.Sync(
                () => false,
                () => $"Unsupported temporal type '{typeof(T)}' for IsBetween",
                isSkipped);
        }

        private static DateOnly? AsDateOnly<T>(T value)
            => value switch
            {
                null => null,
                DateOnly d => d,
                _ => throw new InvalidOperationException($"Expected DateOnly/DateOnly? but got '{typeof(T)}'.")
            };

        private static TimeOnly? AsTimeOnly<T>(T value)
            => value switch
            {
                null => null,
                TimeOnly t => t,
                _ => throw new InvalidOperationException($"Expected TimeOnly/TimeOnly? but got '{typeof(T)}'.")
            };

        // DateOnly nullable checks
        public static CheckOperation EqualTo(DateOnly? actual, DateOnly expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value == expected,
                () => actual is null ? "Expected a value, but was null" : $"Expected {actual.Value:yyyy-MM-dd} to equal {expected:yyyy-MM-dd}",
                isSkipped);

        public static CheckOperation IsBefore(DateOnly? actual, DateOnly expected, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value < expected,
                () => actual is null ? "Expected a value, but was null" : $"Expected {actual.Value:yyyy-MM-dd} to be before {expected:yyyy-MM-dd}",
                isSkipped);

        public static CheckOperation IsAfter(DateOnly? actual, DateOnly expected, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value > expected,
                () => actual is null ? "Expected a value, but was null" : $"Expected {actual.Value:yyyy-MM-dd} to be after {expected:yyyy-MM-dd}",
                isSkipped);

        public static CheckOperation HasYear(DateOnly? actual, int year, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.Year == year,
                () => actual is null ? "Expected a value, but was null" : $"Expected year to be {year}, but was {actual.Value.Year}",
                isSkipped);

        public static CheckOperation HasMonth(DateOnly? actual, int month, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.Month == month,
                () => actual is null ? "Expected a value, but was null" : $"Expected month to be {month}, but was {actual.Value.Month}",
                isSkipped);

        public static CheckOperation HasDay(DateOnly? actual, int day, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.Day == day,
                () => actual is null ? "Expected a value, but was null" : $"Expected day to be {day}, but was {actual.Value.Day}",
                isSkipped);

        public static CheckOperation IsInThePast(DateOnly? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value < DateOnly.FromDateTime(DateTime.Today),
                () => actual is null ? "Expected a value, but was null" : $"Expected {actual.Value:yyyy-MM-dd} to be in the past",
                isSkipped);

        public static CheckOperation IsInTheFuture(DateOnly? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value > DateOnly.FromDateTime(DateTime.Today),
                () => actual is null ? "Expected a value, but was null" : $"Expected {actual.Value:yyyy-MM-dd} to be in the future",
                isSkipped);

        public static CheckOperation IsNull(DateOnly? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => !actual.HasValue,
                () => actual.HasValue
                    ? $"Expected null, but was {actual.Value:yyyy-MM-dd}"
                    : "Expected null, but was null",
                isSkipped);

        public static CheckOperation IsNotNull(DateOnly? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue,
                () => "Expected a value, but was null",
                isSkipped);

        // TimeOnly nullable checks
        public static CheckOperation EqualTo(TimeOnly? actual, TimeOnly expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value == expected,
                () => actual is null ? "Expected a value, but was null" : $"Expected {actual.Value:HH:mm:ss} to equal {expected:HH:mm:ss}",
                isSkipped);

        public static CheckOperation IsBefore(TimeOnly? actual, TimeOnly expected, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value < expected,
                () => actual is null ? "Expected a value, but was null" : $"Expected {actual.Value:HH:mm:ss} to be before {expected:HH:mm:ss}",
                isSkipped);

        public static CheckOperation IsAfter(TimeOnly? actual, TimeOnly expected, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value > expected,
                () => actual is null ? "Expected a value, but was null" : $"Expected {actual.Value:HH:mm:ss} to be after {expected:HH:mm:ss}",
                isSkipped);

        public static CheckOperation HasHour(TimeOnly? actual, int hour, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.Hour == hour,
                () => actual is null ? "Expected a value, but was null" : $"Expected hour to be {hour}, but was {actual.Value.Hour}",
                isSkipped);

        public static CheckOperation HasMinute(TimeOnly? actual, int minute, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.Minute == minute,
                () => actual is null ? "Expected a value, but was null" : $"Expected minute to be {minute}, but was {actual.Value.Minute}",
                isSkipped);

        public static CheckOperation HasSecond(TimeOnly? actual, int second, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue && actual.Value.Second == second,
                () => actual is null ? "Expected a value, but was null" : $"Expected second to be {second}, but was {actual.Value.Second}",
                isSkipped);

        public static CheckOperation IsNull(TimeOnly? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => !actual.HasValue,
                () => actual.HasValue
                    ? $"Expected null, but was {actual.Value:HH:mm:ss}"
                    : "Expected null, but was null",
                isSkipped);

        public static CheckOperation IsNotNull(TimeOnly? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual.HasValue,
                () => "Expected a value, but was null",
                isSkipped);
#endif
    }
}

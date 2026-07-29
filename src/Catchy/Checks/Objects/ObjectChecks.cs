namespace Catchy.Sdk
{
    public static class ObjectChecks
    {
        static string Fmt(object? v, string? e = null) => ExprFormat.Inline(v, e);

        public static CheckOperation Is<T>(T? actual, T? expected, string? expectedExpr, bool isSkipped)
            => CheckOperation.Sync(
                () => Equals(actual, expected),
                () => $"Expected {Fmt(actual)} to equal {Fmt(expected, expectedExpr)}",
                isSkipped);

        public static CheckOperation IsNot<T>(T? actual, T? unexpected, string? unexpectedExpr, bool isSkipped)
            => CheckOperation.Sync(
                () => !Equals(actual, unexpected),
                () => $"Expected {Fmt(actual)} not to equal {Fmt(unexpected, unexpectedExpr)}, but it did",
                isSkipped);

        public static CheckOperation IsNull<T>(T? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null,
                () => $"Expected null, but was {Fmt(actual)}",
                isSkipped);

        public static CheckOperation IsNotNull<T>(T? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null,
                () => "Expected a value, but was null",
                isSkipped);

        public static CheckOperation IsDefault<T>(T? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || EqualityComparer<T>.Default.Equals(actual, default!),
                () => $"Expected default({typeof(T).Name}), but was {Fmt(actual)}",
                isSkipped);

        public static CheckOperation IsNotDefault<T>(T? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && !EqualityComparer<T>.Default.Equals(actual, default!),
                () => $"Expected non-default {typeof(T).Name}, but was default",
                isSkipped);

        public static CheckOperation Satisfies<T>(T? actual, Func<T?, bool> predicate,
            string? predicateExpr, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && predicate(actual),
                () => actual is null
                    ? "Expected a value, but was null"
                    : $"Expected value to satisfy: {predicateExpr ?? "<predicate>"}",
                isSkipped);

        public static CheckOperation DoesNotSatisfy<T>(T? actual, Func<T?, bool> predicate,
            string? predicateExpr, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || !predicate(actual),
                () => $"Expected value not to satisfy: {predicateExpr ?? "<predicate>"}, but it did",
                isSkipped);

        public static CheckOperation IsInstanceOf(object? actual, Type expectedType, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && expectedType.IsInstanceOfType(actual),
                () => $"Expected instance of {TypeHelper.FriendlyName(expectedType)}, but was {actual?.GetType().Name ?? "null"}",
                isSkipped);

        public static CheckOperation IsNotInstanceOf(object? actual, Type expectedType, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || !expectedType.IsInstanceOfType(actual),
                () => $"Expected not to be instance of {TypeHelper.FriendlyName(expectedType)}, but was {actual?.GetType().Name}",
                isSkipped);

        public static CheckOperation IsExactTypeOf(object? actual, Type expectedType, bool isSkipped)
            => CheckOperation.Sync(
                () => actual?.GetType() == expectedType,
                () => $"Expected exact type {TypeHelper.FriendlyName(expectedType)}, but was {actual?.GetType().Name ?? "null"}",
                isSkipped);

        public static CheckOperation IsNotExactTypeOf(object? actual, Type expectedType, bool isSkipped)
            => CheckOperation.Sync(
                () => actual?.GetType() != expectedType,
                () => $"Expected type other than {TypeHelper.FriendlyName(expectedType)}, but was exactly that",
                isSkipped);

        public static CheckOperation IsSameReferenceAs<T>(T? actual, T? other, bool isSkipped)
            => typeof(T).IsValueType
                ? CheckOperation.Sync(() => false,
                    () => $"IsSameReferenceAs is not applicable to value type {typeof(T).Name}", isSkipped)
                : CheckOperation.Sync(
                    () => ReferenceEquals(actual, other),
                    () => "Expected same reference, but objects were different instances",
                    isSkipped);

        public static CheckOperation IsNotSameReferenceAs<T>(T? actual, T? other, bool isSkipped)
            => typeof(T).IsValueType
                ? CheckOperation.Sync(() => false,
                    () => $"IsNotSameReferenceAs is not applicable to value type {typeof(T).Name}", isSkipped)
                : CheckOperation.Sync(
                    () => !ReferenceEquals(actual, other),
                    () => "Expected different references, but objects were the same instance",
                    isSkipped);

        public static CheckOperation IsDeepCloneOf<T>(T? actual, T? expected,
            EqualsOptions? opts, string? expectedExpr, bool isSkipped)
        {
            var effectiveOpts = opts ?? new EqualsOptions();
            return CheckOperation.Sync(
                () =>
                {
                    if (actual is null || expected is null) return false;
                    if (ReferenceEquals(actual, expected)) return false; // must NOT be same ref
                    return DeepEqualEngine.AreEqualObjects(actual, expected, effectiveOpts);
                },
                () =>
                {
                    if (actual is null) return "Expected a value, but was null";
                    if (expected is null) return "Expected value is null, cannot be a clone";
                    if (ReferenceEquals(actual, expected))
                        return $"Expected a deep clone (different reference), but was the same instance";
                    var diffs = DeepEqualEngine.GetDiffs(actual, expected, effectiveOpts).ToList();
                    return diffs.Count > 0
                        ? $"Expected deep clone of {Fmt(expected, expectedExpr)}, but differ:\n{string.Join("\n", diffs)}"
                        : $"Expected deep clone of {Fmt(expected, expectedExpr)}";
                },
                isSkipped);
        }

        public static CheckOperation IsEquivalentTo<T>(T? actual, object? expected,
            EqualsOptions? opts, DeepEqualRuleContainer? localRules, string? expectedExpr, bool isSkipped)
        {
            var effectiveOpts = opts ?? new EqualsOptions();
            return CheckOperation.Sync(
                () => DeepEqualEngine.AreEqualObjects(actual, expected, effectiveOpts, localRules),
                () =>
                {
                    if (actual is null) return "Expected a value, but was null";
                    if (expected is null) return $"Expected equivalent to null, but was {ValueFormatter.Format(actual)}";
                    var diffs = DeepEqualEngine.GetDiffs(actual, expected, effectiveOpts, localRules).ToList();
                    return diffs.Count > 0
                        ? $"Expected {ValueFormatter.Format(actual)} to be equivalent to {Fmt(expected, expectedExpr)}, but differ:\n{string.Join("\n", diffs)}"
                        : $"Expected {Fmt(actual)} to be equivalent to {Fmt(expected, expectedExpr)}";
                },
                isSkipped);
        }

        public static CheckOperation IsEquivalentTo<T>(
            Func<T?> getActual,
            object? expected,
            Func<EqualsOptions> getOptions,
            Func<DeepEqualRuleContainer?> getLocalRules,
            string? expectedExpr,
            bool isSkipped)
        {
            return CheckOperation.Sync(
                () => DeepEqualEngine.AreEqualObjects(getActual(), expected, getOptions(), getLocalRules()),
                () =>
                {
                    var actual = getActual();
                    if (actual is null) return "Expected a value, but was null";
                    if (expected is null) return $"Expected equivalent to null, but was {ValueFormatter.Format(actual)}";
                    var opts = getOptions();
                    var localRules = getLocalRules();
                    var diffs = DeepEqualEngine.GetDiffs(actual, expected, opts, localRules).ToList();
                    return diffs.Count > 0
                        ? $"Expected {ValueFormatter.Format(actual)} to be equivalent to {Fmt(expected, expectedExpr)}, but differ:\n{string.Join("\n", diffs)}"
                        : $"Expected {Fmt(actual)} to be equivalent to {Fmt(expected, expectedExpr)}";
                },
                isSkipped);
        }

        public static CheckOperation IsNotEquivalentTo<T>(T? actual, object? expected,
            EqualsOptions? opts, DeepEqualRuleContainer? localRules, string? expectedExpr, bool isSkipped)
        {
            var effectiveOpts = opts ?? new EqualsOptions();
            return CheckOperation.Sync(
                () => !DeepEqualEngine.AreEqualObjects(actual, expected, effectiveOpts, localRules),
                () => $"Expected {Fmt(actual)} not to be equivalent to {Fmt(expected, expectedExpr)}, but it was",
                isSkipped);
        }

        public static CheckOperation IsOneOf<T>(T? actual, IReadOnlyList<T> values, bool isSkipped)
            => CheckOperation.Sync(
                () => values.Any(v => Equals(actual, v)),
                () => $"Expected {Fmt(actual)} to be one of [{string.Join(", ", values.Select(v => ValueFormatter.Format(v)))}]",
                isSkipped);

        public static CheckOperation IsNotOneOf<T>(T? actual, IReadOnlyList<T> values, bool isSkipped)
            => CheckOperation.Sync(
                () => !values.Any(v => Equals(actual, v)),
                () => $"Expected {Fmt(actual)} not to be one of [{string.Join(", ", values.Select(v => ValueFormatter.Format(v)))}], but it was",
                isSkipped);
    }
}

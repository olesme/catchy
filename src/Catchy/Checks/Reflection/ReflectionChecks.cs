using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Catchy.Sdk
{
    public static class ReflectionChecks
    {
        private const string ReflectionWarning =
            "String-based member access uses reflection. Use Expression<Func<T, TValue>> for AOT safety.";

        [RequiresUnreferencedCode(ReflectionWarning)]
        public static CheckOperation HasMember<T>(T? actual, string memberName, bool isSkipped)
            => CheckOperation.Sync(
                () =>
                {
                    if (actual is null) return false;
                    var type = actual.GetType();
                    return type.GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) is not null
                        || type.GetField(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) is not null;
                },
                () => actual is null
                    ? "Expected a value, but was null"
                    : $"Expected object to have member '{memberName}', but it was not found",
                isSkipped);

        [RequiresUnreferencedCode(ReflectionWarning)]
        public static CheckOperation DoesNotHaveMember<T>(T? actual, string memberName, bool isSkipped)
            => CheckOperation.Sync(
                () =>
                {
                    if (actual is null) return true;
                    var type = actual.GetType();
                    return type.GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) is null
                        && type.GetField(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) is null;
                },
                () => $"Expected object not to have member '{memberName}', but it exists",
                isSkipped);

        [RequiresUnreferencedCode(ReflectionWarning)]
        public static CheckOperation MemberEquals<T>(T? actual, string memberName, object? expected,
            string? expectedExpr, bool isSkipped)
        {
            if (actual is null)
                return CheckOperation.Sync(() => false, () => $"Expected a value to check '{memberName}', but was null", isSkipped);

            object? memberValue;
            try { memberValue = ReflectionMemberCache.GetValue(actual.GetType(), memberName, actual); }
            catch (Exception ex)
            { return CheckOperation.Sync(() => false, () => $"Failed to access '{memberName}': {ex.Message}", isSkipped); }

            return CheckOperation.Sync(
                () => Equals(memberValue, expected),
                () => $"Expected {memberName} = {ExprFormat.Inline(expected, expectedExpr)}, but was {ValueFormatter.Format(memberValue)}",
                isSkipped);
        }

        [RequiresUnreferencedCode(ReflectionWarning)]
        public static CheckOperation MemberNotEquals<T>(T? actual, string memberName, object? unexpected,
            string? unexpectedExpr, bool isSkipped)
        {
            if (actual is null)
                return CheckOperation.Sync(() => false, () => $"Expected a value to check '{memberName}', but was null", isSkipped);

            object? memberValue;
            try { memberValue = ReflectionMemberCache.GetValue(actual.GetType(), memberName, actual); }
            catch (Exception ex)
            { return CheckOperation.Sync(() => false, () => $"Failed to access '{memberName}': {ex.Message}", isSkipped); }

            return CheckOperation.Sync(
                () => !Equals(memberValue, unexpected),
                () => $"Expected {memberName} != {ExprFormat.Inline(unexpected, unexpectedExpr)}, but was {ValueFormatter.Format(memberValue)}",
                isSkipped);
        }

        [RequiresUnreferencedCode(ReflectionWarning)]
        public static CheckOperation MemberSatisfies<T>(T? actual, string memberName,
            Func<object?, bool> predicate, string? predicateExpr, bool isSkipped)
        {
            if (actual is null)
                return CheckOperation.Sync(() => false, () => $"Expected a value to check '{memberName}', but was null", isSkipped);

            object? memberValue;
            try { memberValue = ReflectionMemberCache.GetValue(actual.GetType(), memberName, actual); }
            catch (Exception ex)
            { return CheckOperation.Sync(() => false, () => $"Failed to access '{memberName}': {ex.Message}", isSkipped); }

            return CheckOperation.Sync(
                () => predicate(memberValue),
                () => $"Expected {memberName} to satisfy {predicateExpr ?? "<predicate>"}, but was {ValueFormatter.Format(memberValue)}",
                isSkipped);
        }

        [RequiresUnreferencedCode(ReflectionWarning)]
        public static CheckOperation MemberIsNull<T>(T? actual, string memberName, bool isSkipped)
        {
            if (actual is null)
                return CheckOperation.Sync(() => false, () => $"Expected a value to check '{memberName}', but was null", isSkipped);

            object? memberValue;
            try { memberValue = ReflectionMemberCache.GetValue(actual.GetType(), memberName, actual); }
            catch (Exception ex)
            { return CheckOperation.Sync(() => false, () => $"Failed to access '{memberName}': {ex.Message}", isSkipped); }

            return CheckOperation.Sync(
                () => memberValue is null,
                () => $"Expected {memberName} to be null, but was {ValueFormatter.Format(memberValue)}",
                isSkipped);
        }

        [RequiresUnreferencedCode(ReflectionWarning)]
        public static CheckOperation MemberIsNotNull<T>(T? actual, string memberName, bool isSkipped)
        {
            if (actual is null)
                return CheckOperation.Sync(() => false, () => $"Expected a value to check '{memberName}', but was null", isSkipped);

            object? memberValue;
            try { memberValue = ReflectionMemberCache.GetValue(actual.GetType(), memberName, actual); }
            catch (Exception ex)
            { return CheckOperation.Sync(() => false, () => $"Failed to access '{memberName}': {ex.Message}", isSkipped); }

            return CheckOperation.Sync(
                () => memberValue is not null,
                () => $"Expected {memberName} to be non-null, but was null",
                isSkipped);
        }

        public static CheckOperation MemberEquals<T, TValue>(T? actual, string name, Func<T, TValue> getter,
            TValue expected, string? expectedExpr, bool isSkipped)
        {
            if (actual is null)
                return CheckOperation.Sync(() => false, () => $"Expected a value to check '{name}', but was null", isSkipped);

            TValue memberValue;
            try { memberValue = getter(actual); }
            catch (Exception ex)
            { return CheckOperation.Sync(() => false, () => $"Failed to access '{name}': {ex.Message}", isSkipped); }

            return CheckOperation.Sync(
                () => EqualityComparer<TValue>.Default.Equals(memberValue, expected),
                () => $"Expected {name} = {ExprFormat.Inline(expected, expectedExpr)}, but was {ValueFormatter.Format(memberValue)}",
                isSkipped);
        }

        public static CheckOperation MemberNotEquals<T, TValue>(T? actual, string name, Func<T, TValue> getter,
            TValue unexpected, string? unexpectedExpr, bool isSkipped)
        {
            if (actual is null)
                return CheckOperation.Sync(() => false, () => $"Expected a value to check '{name}', but was null", isSkipped);

            TValue memberValue;
            try { memberValue = getter(actual); }
            catch (Exception ex)
            { return CheckOperation.Sync(() => false, () => $"Failed to access '{name}': {ex.Message}", isSkipped); }

            return CheckOperation.Sync(
                () => !EqualityComparer<TValue>.Default.Equals(memberValue, unexpected),
                () => $"Expected {name} != {ExprFormat.Inline(unexpected, unexpectedExpr)}, but was {ValueFormatter.Format(memberValue)}",
                isSkipped);
        }

        public static CheckOperation MemberSatisfies<T, TValue>(T? actual, string name, Func<T, TValue> getter,
            Func<TValue, bool> predicate, string? predicateExpr, bool isSkipped)
        {
            if (actual is null)
                return CheckOperation.Sync(() => false, () => $"Expected a value to check '{name}', but was null", isSkipped);

            TValue memberValue;
            try { memberValue = getter(actual); }
            catch (Exception ex)
            { return CheckOperation.Sync(() => false, () => $"Failed to access '{name}': {ex.Message}", isSkipped); }

            return CheckOperation.Sync(
                () => predicate(memberValue),
                () => $"Expected {name} to satisfy {predicateExpr ?? "<predicate>"}, but was {ValueFormatter.Format(memberValue)}",
                isSkipped);
        }

        public static CheckOperation MemberIsNull<T, TValue>(T? actual, string name, Func<T, TValue> getter, bool isSkipped)
        {
            if (actual is null)
                return CheckOperation.Sync(() => false, () => $"Expected a value to check '{name}', but was null", isSkipped);

            TValue memberValue;
            try { memberValue = getter(actual); }
            catch (Exception ex)
            { return CheckOperation.Sync(() => false, () => $"Failed to access '{name}': {ex.Message}", isSkipped); }

            return CheckOperation.Sync(
                () => memberValue is null,
                () => $"Expected {name} to be null, but was {ValueFormatter.Format(memberValue)}",
                isSkipped);
        }

        public static CheckOperation MemberIsNotNull<T, TValue>(T? actual, string name, Func<T, TValue> getter, bool isSkipped)
        {
            if (actual is null)
                return CheckOperation.Sync(() => false, () => $"Expected a value to check '{name}', but was null", isSkipped);

            TValue memberValue;
            try { memberValue = getter(actual); }
            catch (Exception ex)
            { return CheckOperation.Sync(() => false, () => $"Failed to access '{name}': {ex.Message}", isSkipped); }

            return CheckOperation.Sync(
                () => memberValue is not null,
                () => $"Expected {name} to be non-null, but was null",
                isSkipped);
        }

        public static CheckOperation MemberIsDefault<T, TValue>(T? actual, string name, Func<T, TValue> getter, bool isSkipped)
        {
            if (actual is null)
                return CheckOperation.Sync(() => false, () => $"Expected a value to check '{name}', but was null", isSkipped);

            TValue memberValue;
            try { memberValue = getter(actual); }
            catch (Exception ex)
            { return CheckOperation.Sync(() => false, () => $"Failed to access '{name}': {ex.Message}", isSkipped); }

            return CheckOperation.Sync(
                () => EqualityComparer<TValue>.Default.Equals(memberValue, default!),
                () => $"Expected {name} to be default, but was {ValueFormatter.Format(memberValue)}",
                isSkipped);
        }

        public static CheckOperation MemberIsNotDefault<T, TValue>(T? actual, string name, Func<T, TValue> getter, bool isSkipped)
        {
            if (actual is null)
                return CheckOperation.Sync(() => false, () => $"Expected a value to check '{name}', but was null", isSkipped);

            TValue memberValue;
            try { memberValue = getter(actual); }
            catch (Exception ex)
            { return CheckOperation.Sync(() => false, () => $"Failed to access '{name}': {ex.Message}", isSkipped); }

            return CheckOperation.Sync(
                () => !EqualityComparer<TValue>.Default.Equals(memberValue, default!),
                () => $"Expected {name} to be non-default",
                isSkipped);
        }
    }
}

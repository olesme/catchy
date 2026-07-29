using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    public abstract class MemberAccessor<TOwner, TValue>(string memberName, Func<Func<TOwner, TValue>> getterFactory)
    {
        private readonly Lazy<Func<TOwner, TValue>> _getter = new(getterFactory);
        public string MemberName { get; } = memberName;

        public TValue Get(TOwner owner) => _getter.Value(owner);
    }

    public sealed class FieldAccessor<TOwner, TValue> : MemberAccessor<TOwner, TValue>
    {
        private FieldAccessor(string name) : base(name, () => BuildGetter(name)) { }

        public static FieldAccessor<TOwner, TValue> For(string fieldName) => new(fieldName);

        private static Func<TOwner, TValue> BuildGetter(string name)
        {
            var field = typeof(TOwner).GetField(name,
                BindingFlags.Instance | BindingFlags.Static |
                BindingFlags.Public | BindingFlags.NonPublic)
                ?? throw new InvalidOperationException($"Field '{name}' not found on {typeof(TOwner).Name}");
            var param = Expression.Parameter(typeof(TOwner), "o");
            var access = Expression.Field(field.IsStatic ? null : param, field);
            return Expression.Lambda<Func<TOwner, TValue>>(Expression.Convert(access, typeof(TValue)), param).Compile();
        }
    }

    public sealed class PropertyAccessor<TOwner, TValue> : MemberAccessor<TOwner, TValue>
    {
        private PropertyAccessor(string name) : base(name, () => BuildGetter(name)) { }

        public static PropertyAccessor<TOwner, TValue> For(string propertyName) => new(propertyName);

        private static Func<TOwner, TValue> BuildGetter(string name)
        {
            var prop = typeof(TOwner).GetProperty(name,
                BindingFlags.Instance | BindingFlags.Static |
                BindingFlags.Public | BindingFlags.NonPublic)
                ?? throw new InvalidOperationException($"Property '{name}' not found on {typeof(TOwner).Name}");
            var get = prop.GetGetMethod(nonPublic: true)
                ?? throw new InvalidOperationException($"Property '{name}' on {typeof(TOwner).Name} has no getter");
            var param = Expression.Parameter(typeof(TOwner), "o");
            var call = get.IsStatic ? (Expression)Expression.Call(get) : Expression.Call(param, get);
            return Expression.Lambda<Func<TOwner, TValue>>(Expression.Convert(call, typeof(TValue)), param).Compile();
        }
    }

    public static partial class ReflectionAssertionsExtensions
    {
        private const string StringMemberWarning =
       "String-based member access uses reflection. Use Expression<Func<T, TValue>> for AOT safety.";

        /// <summary>Asserts that a member named <paramref name="memberName"/> exists.</summary>
        [RequiresUnreferencedCode(StringMemberWarning)]
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<T> HasMember<T>(this StructuralAssertions<T> a, string memberName,
            [CallerArgumentExpression(nameof(memberName))] string? expr = null)
        { a.Link("HasMember", expr); a.Op(a => ReflectionChecks.HasMember(a.GetValue(), memberName, a.IsSkipped())); return a; }

        /// <summary>Asserts that a member named <paramref name="memberName"/> does not exist.</summary>
        [RequiresUnreferencedCode(StringMemberWarning)]
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<T> DoesNotHaveMember<T>(this StructuralAssertions<T> a, string memberName,
            [CallerArgumentExpression(nameof(memberName))] string? expr = null)
        { a.Link("DoesNotHaveMember", expr); a.Op(a => ReflectionChecks.DoesNotHaveMember(a.GetValue(), memberName, a.IsSkipped())); return a; }

        /// <summary>Asserts that member <paramref name="memberName"/> equals <paramref name="expected"/>.</summary>
        [RequiresUnreferencedCode(StringMemberWarning)]
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<T> HasMember<T>(this StructuralAssertions<T> a, string memberName, object? expected,
            [CallerArgumentExpression(nameof(memberName))] string? memberNameExpr = null,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("HasMember", memberNameExpr, expr); a.Op(a => ReflectionChecks.MemberEquals(a.GetValue(), memberName, expected, expr, a.IsSkipped())); return a; }

        /// <summary>Asserts that member <paramref name="memberName"/> does not equal <paramref name="unexpected"/>.</summary>
        [RequiresUnreferencedCode(StringMemberWarning)]
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<T> DoesNotHaveMember<T>(this StructuralAssertions<T> a, string memberName, object? unexpected,
            [CallerArgumentExpression(nameof(memberName))] string? memberNameExpr = null,
            [CallerArgumentExpression(nameof(unexpected))] string? expr = null)
        { a.Link("DoesNotHaveMember", memberNameExpr, expr); a.Op(a => ReflectionChecks.MemberNotEquals(a.GetValue(), memberName, unexpected, expr, a.IsSkipped())); return a; }

        /// <summary>Asserts that member <paramref name="memberName"/> satisfies <paramref name="predicate"/>.</summary>
        [RequiresUnreferencedCode(StringMemberWarning)]
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<T> MemberSatisfies<T>(this StructuralAssertions<T> a, string memberName, Func<object?, bool> predicate,
            [CallerArgumentExpression(nameof(memberName))] string? memberNameExpr = null,
            [CallerArgumentExpression(nameof(predicate))] string? expr = null)
        { a.Link("MemberSatisfies", memberNameExpr, expr); a.Op(a => ReflectionChecks.MemberSatisfies(a.GetValue(), memberName, predicate, expr, a.IsSkipped())); return a; }

        /// <summary>Asserts that member <paramref name="memberName"/> is null.</summary>
        [RequiresUnreferencedCode(StringMemberWarning)]
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<T> MemberIsNull<T>(this StructuralAssertions<T> a, string memberName,
            [CallerArgumentExpression(nameof(memberName))] string? expr = null)
        { a.Link("MemberIsNull", expr); a.Op(a => ReflectionChecks.MemberIsNull(a.GetValue(), memberName, a.IsSkipped())); return a; }

        /// <summary>Asserts that member <paramref name="memberName"/> is not null.</summary>
        [RequiresUnreferencedCode(StringMemberWarning)]
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<T> MemberIsNotNull<T>(this StructuralAssertions<T> a, string memberName,
            [CallerArgumentExpression(nameof(memberName))] string? expr = null)
        { a.Link("MemberIsNotNull", expr); a.Op(a => ReflectionChecks.MemberIsNotNull(a.GetValue(), memberName, a.IsSkipped())); return a; }

        /// <summary>Asserts that expression member <paramref name="member"/> equals <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<T> HasMember<T, TValue>(this StructuralAssertions<T> a,
            Expression<Func<T, TValue>> member, TValue expected,
            [CallerArgumentExpression(nameof(member))] string? memberExpr = null,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        {
            var (name, getter) = ReflectionOps.Resolve(member);
            a.Link("HasMember", memberExpr, expr);
            a.Op(a => ReflectionChecks.MemberEquals(a.GetValue(), name, getter, expected, expr, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that expression member <paramref name="member"/> does not equal <paramref name="unexpected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<T> DoesNotHaveMember<T, TValue>(this StructuralAssertions<T> a,
            Expression<Func<T, TValue>> member, TValue unexpected,
            [CallerArgumentExpression(nameof(member))] string? memberExpr = null,
            [CallerArgumentExpression(nameof(unexpected))] string? expr = null)
        {
            var (name, getter) = ReflectionOps.Resolve(member);
            a.Link("DoesNotHaveMember", memberExpr, expr);
            a.Op(a => ReflectionChecks.MemberNotEquals(a.GetValue(), name, getter, unexpected, expr, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that expression member <paramref name="member"/> satisfies <paramref name="predicate"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<T> MemberSatisfies<T, TValue>(this StructuralAssertions<T> a,
            Expression<Func<T, TValue>> member, Func<TValue, bool> predicate,
            [CallerArgumentExpression(nameof(member))] string? memberExpr = null,
            [CallerArgumentExpression(nameof(predicate))] string? expr = null)
        {
            var (name, getter) = ReflectionOps.Resolve(member);
            a.Link("MemberSatisfies", memberExpr, expr);
            a.Op(a => ReflectionChecks.MemberSatisfies(a.GetValue(), name, getter, predicate, expr, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that expression member <paramref name="member"/> is null.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<T> MemberIsNull<T, TValue>(this StructuralAssertions<T> a,
            Expression<Func<T, TValue>> member,
            [CallerArgumentExpression(nameof(member))] string? memberExpr = null)
        {
            var (name, getter) = ReflectionOps.Resolve(member);
            a.Link("MemberIsNull", memberExpr);
            a.Op(a => ReflectionChecks.MemberIsNull(a.GetValue(), name, getter, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that expression member <paramref name="member"/> is not null.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<T> MemberIsNotNull<T, TValue>(this StructuralAssertions<T> a,
            Expression<Func<T, TValue>> member,
            [CallerArgumentExpression(nameof(member))] string? memberExpr = null)
        {
            var (name, getter) = ReflectionOps.Resolve(member);
            a.Link("MemberIsNotNull", memberExpr);
            a.Op(a => ReflectionChecks.MemberIsNotNull(a.GetValue(), name, getter, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that expression member <paramref name="member"/> equals its default value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<T> MemberIsDefault<T, TValue>(this StructuralAssertions<T> a,
            Expression<Func<T, TValue>> member,
            [CallerArgumentExpression(nameof(member))] string? memberExpr = null)
        {
            var (name, getter) = ReflectionOps.Resolve(member);
            a.Link("MemberIsDefault", memberExpr);
            a.Op(a => ReflectionChecks.MemberIsDefault(a.GetValue(), name, getter, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that expression member <paramref name="member"/> does not equal its default value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<T> MemberIsNotDefault<T, TValue>(this StructuralAssertions<T> a,
            Expression<Func<T, TValue>> member,
            [CallerArgumentExpression(nameof(member))] string? memberExpr = null)
        {
            var (name, getter) = ReflectionOps.Resolve(member);
            a.Link("MemberIsNotDefault", memberExpr);
            a.Op(a => ReflectionChecks.MemberIsNotDefault(a.GetValue(), name, getter, a.IsSkipped()));
            return a;
        }
    }

    namespace Sdk
    {
        public static class ReflectionOps
        {
            public static CheckOperation StringOp<T>(
                StructuralAssertions<T> a, string memberName,
                Func<object?, bool> check, Func<object?, string> failMsg)
            {
                if (a._isNull)
                    return CheckOperation.Sync(() => false,
                        () => $"Expected a value to check '{memberName}', but was null", a.IsSkipped());
                object? actual;
                try { actual = ReflectionMemberCache.GetValue(typeof(T), memberName, (object?)a._value); }
                catch (Exception ex)
                { return CheckOperation.Sync(() => false, () => $"Failed to access '{memberName}': {ex.Message}", a.IsSkipped()); }
                return CheckOperation.Sync(() => check(actual), () => failMsg(actual), a.IsSkipped());
            }

            public static CheckOperation ExprOp<T, TValue>(
                StructuralAssertions<T> a, string name, Func<T, TValue> getter,
                Func<TValue, bool> check, Func<TValue, string> failMsg)
            {
                if (a._isNull)
                    return CheckOperation.Sync(() => false,
                        () => $"Expected a value to check '{name}', but was null", a.IsSkipped());
                TValue actual;
                try { actual = getter(a._value); }
                catch (Exception ex)
                { return CheckOperation.Sync(() => false, () => $"Failed to access '{name}': {ex.Message}", a.IsSkipped()); }
                return CheckOperation.Sync(() => check(actual), () => failMsg(actual), a.IsSkipped());
            }

            public static CheckOperation AccessorOp<T, TValue>(
                StructuralAssertions<T> a, MemberAccessor<T, TValue> accessor,
                Func<TValue, bool> check, Func<TValue, string> failMsg)
            {
                if (a._isNull)
                    return CheckOperation.Sync(() => false,
                        () => $"Expected a value to check '{accessor.MemberName}', but was null", a.IsSkipped());
                TValue actual;
                try { actual = accessor.Get(a._value); }
                catch (Exception ex)
                { return CheckOperation.Sync(() => false, () => $"Failed to access '{accessor.MemberName}': {ex.Message}", a.IsSkipped()); }
                return CheckOperation.Sync(() => check(actual), () => failMsg(actual), a.IsSkipped());
            }

            public static (string name, Func<T, TValue> getter) Resolve<T, TValue>(
                System.Linq.Expressions.Expression<Func<T, TValue>> expr)
            {
                var body = expr.Body is System.Linq.Expressions.UnaryExpression
                { NodeType: System.Linq.Expressions.ExpressionType.Convert } u
                    ? u.Operand : expr.Body;
                if (body is System.Linq.Expressions.MemberExpression me)
                    return (me.Member.Name, expr.Compile());
                throw new ArgumentException(
                    $"Expression must be a simple member access (x => x.Member), got: {expr}");
            }

            public static T DefaultOf<T>() => default!;
        }

        public static class ReflectionMemberCache
        {
            private static readonly Dictionary<(Type, string), Func<object, object?>> _cache = [];
            private static readonly object _lock = new();
            private static readonly BindingFlags _flags =
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

            public static object? GetValue(
                        [DynamicallyAccessedMembers(
                            DynamicallyAccessedMemberTypes.PublicProperties   |
                            DynamicallyAccessedMemberTypes.NonPublicProperties |
                            DynamicallyAccessedMemberTypes.PublicFields        |
                            DynamicallyAccessedMemberTypes.NonPublicFields)]
                Type type, string memberName, object? instance)
            {
                if (instance is null) return null;
                var getter = GetOrBuildGetter(type, memberName);
                return getter(instance);
            }

            public static Func<object, object?> GetOrBuildGetter(
                        [DynamicallyAccessedMembers(
                            DynamicallyAccessedMemberTypes.PublicProperties   |
                            DynamicallyAccessedMemberTypes.NonPublicProperties |
                            DynamicallyAccessedMemberTypes.PublicFields        |
                            DynamicallyAccessedMemberTypes.NonPublicFields)]
                Type type, string name)
            {
                lock (_lock)
                {
                    if (_cache.TryGetValue((type, name), out var cached)) return cached;
                }
                var getter = BuildGetter(type, name);
                lock (_lock) { _cache[(type, name)] = getter; }
                return getter;
            }

            private static Func<object, object?> BuildGetter(
                        [DynamicallyAccessedMembers(
                            DynamicallyAccessedMemberTypes.PublicProperties   |
                            DynamicallyAccessedMemberTypes.NonPublicProperties |
                            DynamicallyAccessedMemberTypes.PublicFields        |
                            DynamicallyAccessedMemberTypes.NonPublicFields)]
                Type type, string name)
            {
                var param = Expression.Parameter(typeof(object), "o");
                var cast = Expression.Convert(param, type);

                var prop = type.GetProperty(name, _flags);
                if (prop?.GetGetMethod(nonPublic: true) is { } get)
                {
                    Expression access = get.IsStatic
                        ? Expression.Call(get)
                        : Expression.Call(cast, get);
                    var body = Expression.Convert(access, typeof(object));
                    return Expression.Lambda<Func<object, object?>>(body, param).Compile();
                }

                var field = type.GetField(name, _flags);
                if (field is not null)
                {
                    Expression access = field.IsStatic
                        ? Expression.Field(null, field)
                        : Expression.Field(cast, field);
                    var body = Expression.Convert(access, typeof(object));
                    return Expression.Lambda<Func<object, object?>>(body, param).Compile();
                }

                throw new InvalidOperationException(
                    $"No property or field '{name}' on {type.Name}");
            }
        }
    }
}


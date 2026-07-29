using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    [GenerateQuantifiedEntryPoints]
    public class StructuralAssertions<T> : ValueAssertions<StructuralAssertions<T>, T>
    {
        internal bool _isNull => _value is null;

        public StructuralAssertions([AllowNull] T value, AssertionPipeline pipeline)
            : base(pipeline, value!) { }

        /// <summary>
        /// Pipeline-first constructor matching the generator convention <c>(AssertionPipeline, TValue)</c>.
        /// </summary>
        public StructuralAssertions(AssertionPipeline pipeline, [AllowNull] T value)
            : base(pipeline, value!) { }
    }

    public static partial class AsserterExtensions
    {
        /// <summary>Starts assertions for an object value, cast to <typeparamref name="T"/>. Falls back to <see langword="default"/> when the cast fails.</summary>
        public static StructuralAssertions<T> That<T>(
            this Asserter a, object? value, __._ _ = default,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(
                        asserterExpr: aExpr,
                        methodName: "That",
                        genericType: typeof(T),
                        valueExpr: vExpr,
                        file: file, line: line, member: member);
            var typed = value is T t ? t : default;
            return new StructuralAssertions<T>(typed, p);
        }

        /// <summary>Starts assertions for an untyped object value.</summary>
        public static StructuralAssertions<object> That(
            this Asserter a, object? value, __._ _ = default,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(
                        asserterExpr: aExpr,
                        methodName: "That",
                        valueExpr: vExpr,
                        file: file, line: line, member: member);
            return new StructuralAssertions<object>(value, p);
        }
    }

    public static partial class StructuralAssertExtensions
    {

        /// <summary>Asserts that the value is an instance of <typeparamref name="TTarget"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<T> IsInstanceOf<T, TTarget>(this StructuralAssertions<T> a)
        { a.Link("IsInstanceOf", typeof(TTarget)); a.Op(a => ObjectChecks.IsInstanceOf(a.GetValue(), typeof(TTarget), a.IsSkipped())); return a; }

        /// <summary>Asserts that the value is an instance of <paramref name="targetType"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<T> IsInstanceOf<T>(this StructuralAssertions<T> a, Type targetType)
        { a.Link("IsInstanceOf", targetType); a.Op(a => ObjectChecks.IsInstanceOf(a.GetValue(), targetType, a.IsSkipped())); return a; }

        /// <summary>Asserts that the value is not an instance of <typeparamref name="TTarget"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<T> IsNotInstanceOf<T, TTarget>(this StructuralAssertions<T> a)
        { a.Link("IsNotInstanceOf", typeof(TTarget)); a.Op(a => ObjectChecks.IsNotInstanceOf(a.GetValue(), typeof(TTarget), a.IsSkipped())); return a; }

        /// <summary>Asserts that the value is not an instance of <paramref name="targetType"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<T> IsNotInstanceOf<T>(this StructuralAssertions<T> a, Type targetType)
        { a.Link("IsNotInstanceOf", targetType); a.Op(a => ObjectChecks.IsNotInstanceOf(a.GetValue(), targetType, a.IsSkipped())); return a; }

        /// <summary>Asserts that the value's runtime type is exactly <typeparamref name="TTarget"/> (not a derived type).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<T> IsExactTypeOf<T, TTarget>(this StructuralAssertions<T> a)
        { a.Link("IsExactTypeOf", typeof(TTarget)); a.Op(a => ObjectChecks.IsExactTypeOf(a.GetValue(), typeof(TTarget), a.IsSkipped())); return a; }

        /// <summary>Asserts that the value's runtime type is not exactly <typeparamref name="TTarget"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<T> IsNotExactTypeOf<T, TTarget>(this StructuralAssertions<T> a)
        { a.Link("IsNotExactTypeOf", typeof(TTarget)); a.Op(a => ObjectChecks.IsNotExactTypeOf(a.GetValue(), typeof(TTarget), a.IsSkipped())); return a; }

        /// <summary>Asserts that the value refers to the same object instance as <paramref name="other"/> (reference equality).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<T> IsSameReferenceAs<T>(this StructuralAssertions<T> a, T? other,
            [CallerArgumentExpression(nameof(other))] string? expr = null)
        { a.Link("IsSameReferenceAs", expr); a.Op(a => ObjectChecks.IsSameReferenceAs(a.GetValue(), other, a.IsSkipped())); return a; }

        /// <summary>Asserts that the value does not refer to the same object instance as <paramref name="other"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<T> IsNotSameReferenceAs<T>(this StructuralAssertions<T> a, T? other,
            [CallerArgumentExpression(nameof(other))] string? expr = null)
        { a.Link("IsNotSameReferenceAs", expr); a.Op(a => ObjectChecks.IsNotSameReferenceAs(a.GetValue(), other, a.IsSkipped())); return a; }

        /// <summary>Asserts that the value is structurally equal to <paramref name="unexpected"/> but is a different object instance (deep clone check).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<T> IsDeepCloneOf<T>(this StructuralAssertions<T> a, T? unexpected,
            Action<EqualsOptions>? configure = null,
            [CallerArgumentExpression(nameof(unexpected))] string? expr = null)
        {
            var opts = new EqualsOptions();
            configure?.Invoke(opts);
            a.Link("IsDeepCloneOf", expr);
            a.Op(a => ObjectChecks.IsDeepCloneOf(a.GetValue(), unexpected, opts, expr, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the value is deeply equivalent to <paramref name="unexpected"/> using the current deep-equality rules.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<T> IsEquivalentTo<T>(this StructuralAssertions<T> a, object? unexpected,
            [CallerArgumentExpression(nameof(unexpected))] string? expr = null)
        {
            a.Link("IsEquivalentTo", expr);
            a.Op(a => ObjectChecks.IsEquivalentTo(
                getActual: a.GetValue,
                expected: unexpected,
                getOptions: () => a.GetPipeline().Settings.EqualsOptions ?? new EqualsOptions(),
                getLocalRules: () => a.GetPipeline().GetEffectiveDeepEqualRuleContainer(),
                expectedExpr: expr,
                isSkipped: a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the value is not deeply equivalent to <paramref name="unexpected"/> using the current deep-equality rules.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<T> IsNotEquivalentTo<T>(this StructuralAssertions<T> a, object? unexpected,
            [CallerArgumentExpression(nameof(unexpected))] string? expr = null)
        {
            a.Link("IsNotEquivalentTo", expr);
            a.Op(a => ObjectChecks.IsNotEquivalentTo(
                actual: a.GetValue(),
                expected: unexpected,
                opts: a.GetPipeline().Settings.EqualsOptions,
                localRules: a.GetPipeline().GetEffectiveDeepEqualRuleContainer(),
                expectedExpr: expr,
                isSkipped: a.IsSkipped()));
            return a;
        }

        /// <summary>Projects the current value via <paramref name="project"/> and continues the assertion chain on the projected value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<TValue> ThatHas<T, TValue>(this StructuralAssertions<T> a, Func<T, TValue> project,
            [CallerArgumentExpression(nameof(project))] string? expr = null)
        {
            a.Link("ThatHas", expr);
            var projected = !a.GetIsNull() ? project(a.GetValue()) : default;
            return new StructuralAssertions<TValue>(projected, a.GetPipeline());
        }

        /// <summary>Alias for ThatHas — projects to a property/field for further assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<TValue> Property<T, TValue>(this StructuralAssertions<T> a, Func<T, TValue> selector,
            [CallerArgumentExpression(nameof(selector))] string? expr = null)
            => a.ThatHas(selector, expr);
    }

    namespace Sdk
    {
        public static class StructuralAssertionsAccessors
        {
            public static object? GetValueAsObject<T>(this StructuralAssertions<T> a) => a._isNull ? null : (object?)a.GetValue();
            public static IDeepEqualRule? GetDeepEqualRule<T>(this StructuralAssertions<T> a)
                => a.GetPipeline().GetEffectiveDeepEqualRule();
            public static bool GetIsNull<T>(this StructuralAssertions<T> a) => a._isNull;
        }
    }
}



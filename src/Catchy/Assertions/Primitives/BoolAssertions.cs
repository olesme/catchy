using System.Diagnostics;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    public static partial class AsserterExtensions
    {
        /// <summary>Starts assertions for a <c>bool</c> value (treated as nullable bool? for uniform null handling).</summary>
        public static ValueAssertions<bool?> That(this Asserter a, bool value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<bool?>(p, value);
        }

        /// <summary>Starts assertions for a nullable <c>bool?</c> value.</summary>
        public static ValueAssertions<bool?> That(this Asserter a, bool? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<bool?>(p, value);
        }
    }

    public static class BoolAssertExtensions
    {
        /// <summary>Asserts that the value equals <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<bool?> Is(this ValueAssertions<bool?> a, bool? expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        {
            a.Link("Is", expr);
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() == expected,
                () => $"Expected {(expected.HasValue ? expected.Value.ToString() : "null")}, but was {(a.GetValue().HasValue ? a.GetValue()!.Value.ToString() : "null")}",
                a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the value is <c>true</c>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<bool?> IsTrue(this ValueAssertions<bool?> a)
        {
            a.Link("IsTrue");
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() == true,
                () => $"Expected true, but was {(a.GetValue().HasValue ? "false" : "null")}",
                a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the value is <c>false</c>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<bool?> IsFalse(this ValueAssertions<bool?> a)
        {
            a.Link("IsFalse");
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() == false,
                () => $"Expected false, but was {(a.GetValue().HasValue ? "true" : "null")}",
                a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the value is <c>true</c> when <paramref name="condition"/> is <c>true</c> (i.e. <c>condition ⇒ value</c>).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<bool?> ImpliedBy(this ValueAssertions<bool?> a, bool condition,
            [CallerArgumentExpression(nameof(condition))] string? expr = null)
        {
            a.Link("ImpliedBy", expr);
            a.Op(a => CheckOperation.Sync(
                () => !condition || a.GetValue() == true,
                () => $"Expected value to be true when {ExprFormat.Inline(condition, expr)} is true, but was {a.GetValue()?.ToString() ?? "null"}",
                a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that when the value is <c>true</c>, <paramref name="consequence"/> is also <c>true</c> (i.e. <c>value ⇒ consequence</c>).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<bool?> Implies(this ValueAssertions<bool?> a, bool consequence,
            [CallerArgumentExpression(nameof(consequence))] string? expr = null)
        {
            a.Link("Implies", expr);
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() != true || consequence,
                () => $"Expected true to imply {ExprFormat.Inline(consequence, expr)}, but consequence was false",
                a.IsSkipped()));
            return a;
        }
    }
}


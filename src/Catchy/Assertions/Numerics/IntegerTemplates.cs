using System.Diagnostics;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    // Non-nullable integer templates.
    // Template type: int (inferred from ValueAssertions<int>).
    // Targets: long, short, byte, sbyte, uint, ushort, nint.
    // Each method body uses native-type arithmetic — no casting to long.
    // The generator substitutes 'int' in the receiver type, return type, and divisor
    // parameter type so each overload is fully type-native with no implicit widening casts.
    [GenerateTypedOverloads(typeof(long), typeof(short), typeof(byte), typeof(sbyte), typeof(uint), typeof(ushort), typeof(nint))]
    public static partial class IntegerTemplates
    {
        /// <summary>Asserts that the value is even.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<int> IsEven(this ValueAssertions<int> a)
        { a.Link("IsEven"); a.Op(a => CheckOperation.Sync(() => a.GetValue() % 2 == 0, () => $"Expected {a.GetValue()} to be even", a.IsSkipped())); return a; }

        /// <summary>Asserts that the value is odd.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<int> IsOdd(this ValueAssertions<int> a)
        { a.Link("IsOdd"); a.Op(a => CheckOperation.Sync(() => a.GetValue() % 2 != 0, () => $"Expected {a.GetValue()} to be odd", a.IsSkipped())); return a; }

        /// <summary>Asserts that the value is a multiple of <paramref name="divisor"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<int> IsMultipleOf(this ValueAssertions<int> a, int divisor,
            [CallerArgumentExpression(nameof(divisor))] string? expr = null)
        { a.Link("IsMultipleOf", expr); a.Op(a => CheckOperation.Sync(() => divisor != 0 && a.GetValue() % divisor == 0, () => divisor == 0 ? "Divisor cannot be zero" : $"Expected {a.GetValue()} to be a multiple of {ExprFormat.Inline(divisor, expr)}", a.IsSkipped())); return a; }

        /// <summary>Asserts that the value is divisible by <paramref name="divisor"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<int> IsDivisibleBy(this ValueAssertions<int> a, int divisor,
            [CallerArgumentExpression(nameof(divisor))] string? expr = null)
            => IsMultipleOf(a, divisor, expr);

        /// <summary>Asserts that the value is not divisible by <paramref name="divisor"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<int> IsNotDivisibleBy(this ValueAssertions<int> a, int divisor,
            [CallerArgumentExpression(nameof(divisor))] string? expr = null)
        { a.Link("IsNotDivisibleBy", expr); a.Op(a => CheckOperation.Sync(() => divisor != 0 && a.GetValue() % divisor != 0, () => divisor == 0 ? "Divisor cannot be zero" : $"Expected {a.GetValue()} not to be divisible by {ExprFormat.Inline(divisor, expr)}, but it is", a.IsSkipped())); return a; }
    }

    // Nullable integer templates.
    // Template type: int (inferred by unwrapping Nullable<int> from ValueAssertions<int?>).
    // Targets: long?, short?, byte?, sbyte?, uint?, ushort?, nint?.
    [GenerateTypedOverloads(typeof(long?), typeof(short?), typeof(byte?), typeof(sbyte?), typeof(uint?), typeof(ushort?), typeof(nint?))]
    public static partial class NullableIntegerTemplates
    {
        /// <summary>Asserts that the nullable value is present and even.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<int?> IsEven(this ValueAssertions<int?> a)
        { a.Link("IsEven"); a.Op(a => CheckOperation.Sync(() => a.GetValue() is { } v && v % 2 == 0, () => { var nv = a.GetValue(); return !nv.HasValue ? "Expected a value, but was null" : $"Expected {nv.Value} to be even"; }, a.IsSkipped())); return a; }

        /// <summary>Asserts that the nullable value is present and odd.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<int?> IsOdd(this ValueAssertions<int?> a)
        { a.Link("IsOdd"); a.Op(a => CheckOperation.Sync(() => a.GetValue() is { } v && v % 2 != 0, () => { var nv = a.GetValue(); return !nv.HasValue ? "Expected a value, but was null" : $"Expected {nv.Value} to be odd"; }, a.IsSkipped())); return a; }

        /// <summary>Asserts that the nullable value is present and a multiple of <paramref name="divisor"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<int?> IsMultipleOf(this ValueAssertions<int?> a, int divisor,
            [CallerArgumentExpression(nameof(divisor))] string? expr = null)
        { a.Link("IsMultipleOf", expr); a.Op(a => CheckOperation.Sync(() => a.GetValue() is { } v && divisor != 0 && v % divisor == 0, () => { var nv = a.GetValue(); return !nv.HasValue ? "Expected a value, but was null" : divisor == 0 ? "Divisor cannot be zero" : $"Expected {nv.Value} to be a multiple of {ExprFormat.Inline(divisor, expr)}"; }, a.IsSkipped())); return a; }

        /// <summary>Asserts that the nullable value is present and divisible by <paramref name="divisor"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<int?> IsDivisibleBy(this ValueAssertions<int?> a, int divisor,
            [CallerArgumentExpression(nameof(divisor))] string? expr = null)
            => IsMultipleOf(a, divisor, expr);

        /// <summary>Asserts that the nullable value is present and not divisible by <paramref name="divisor"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<int?> IsNotDivisibleBy(this ValueAssertions<int?> a, int divisor,
            [CallerArgumentExpression(nameof(divisor))] string? expr = null)
        { a.Link("IsNotDivisibleBy", expr); a.Op(a => CheckOperation.Sync(() => a.GetValue() is { } v && divisor != 0 && v % divisor != 0, () => { var nv = a.GetValue(); return !nv.HasValue ? "Expected a value, but was null" : divisor == 0 ? "Divisor cannot be zero" : $"Expected {nv.Value} not to be divisible by {ExprFormat.Inline(divisor, expr)}, but it is"; }, a.IsSkipped())); return a; }
    }
}

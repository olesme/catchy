using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    public static partial class AsserterExtensions
    {
        internal static ValueAssertions<T> MkNum<T>(this Asserter a, T value, string? aExpr, string? vExpr, string? file, int line, string? member)
               => new(a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member), value);

        internal static ValueAssertions<T?> MkNumN<T>(this Asserter a, T? value, string? aExpr, string? vExpr, string? file, int line, string? member)
            where T : struct
            => new(a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member), value);


        // ── floating-point ──────────────────────────────────────────────────────
        /// <summary>Starts assertions for a <c>double</c> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<double> That(this Asserter a, double value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null, [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0, [CallerMemberName] string? member = null)
            => a.MkNum(value, aExpr, vExpr, file, line, member);

        /// <summary>Starts assertions for a nullable <c>double?</c> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<double?> That(this Asserter a, double? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null, [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0, [CallerMemberName] string? member = null)
            => a.MkNumN(value, aExpr, vExpr, file, line, member);

        /// <summary>Starts assertions for a <c>float</c> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<float> That(this Asserter a, float value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null, [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0, [CallerMemberName] string? member = null)
            => a.MkNum(value, aExpr, vExpr, file, line, member);

        /// <summary>Starts assertions for a nullable <c>float?</c> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<float?> That(this Asserter a, float? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null, [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0, [CallerMemberName] string? member = null)
            => a.MkNumN(value, aExpr, vExpr, file, line, member);

        /// <summary>Starts assertions for a <c>decimal</c> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<decimal> That(this Asserter a, decimal value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null, [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0, [CallerMemberName] string? member = null)
            => a.MkNum(value, aExpr, vExpr, file, line, member);

        /// <summary>Starts assertions for a nullable <c>decimal?</c> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<decimal?> That(this Asserter a, decimal? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null, [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0, [CallerMemberName] string? member = null)
            => a.MkNumN(value, aExpr, vExpr, file, line, member);

        // ── integers ────────────────────────────────────────────────────────────
        /// <summary>Starts assertions for an <c>int</c> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<int> That(this Asserter a, int value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null, [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0, [CallerMemberName] string? member = null)
            => a.MkNum(value, aExpr, vExpr, file, line, member);

        /// <summary>Starts assertions for a nullable <c>int?</c> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<int?> That(this Asserter a, int? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null, [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0, [CallerMemberName] string? member = null)
            => a.MkNumN(value, aExpr, vExpr, file, line, member);

        /// <summary>Starts assertions for a <c>uint</c> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<uint> That(this Asserter a, uint value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null, [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0, [CallerMemberName] string? member = null)
            => a.MkNum(value, aExpr, vExpr, file, line, member);

        /// <summary>Starts assertions for a nullable <c>uint?</c> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<uint?> That(this Asserter a, uint? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null, [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0, [CallerMemberName] string? member = null)
            => a.MkNumN(value, aExpr, vExpr, file, line, member);

        /// <summary>Starts assertions for a <c>long</c> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<long> That(this Asserter a, long value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null, [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0, [CallerMemberName] string? member = null)
            => a.MkNum(value, aExpr, vExpr, file, line, member);

        /// <summary>Starts assertions for a nullable <c>long?</c> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<long?> That(this Asserter a, long? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null, [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0, [CallerMemberName] string? member = null)
            => a.MkNumN(value, aExpr, vExpr, file, line, member);

        /// <summary>Starts assertions for a <c>ulong</c> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ulong> That(this Asserter a, ulong value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null, [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0, [CallerMemberName] string? member = null)
            => a.MkNum(value, aExpr, vExpr, file, line, member);

        /// <summary>Starts assertions for a nullable <c>ulong?</c> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ulong?> That(this Asserter a, ulong? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null, [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0, [CallerMemberName] string? member = null)
            => a.MkNumN(value, aExpr, vExpr, file, line, member);

        /// <summary>Starts assertions for a <c>short</c> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<short> That(this Asserter a, short value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null, [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0, [CallerMemberName] string? member = null)
            => a.MkNum(value, aExpr, vExpr, file, line, member);

        /// <summary>Starts assertions for a nullable <c>short?</c> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<short?> That(this Asserter a, short? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null, [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0, [CallerMemberName] string? member = null)
            => a.MkNumN(value, aExpr, vExpr, file, line, member);

        /// <summary>Starts assertions for a <c>ushort</c> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ushort> That(this Asserter a, ushort value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null, [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0, [CallerMemberName] string? member = null)
            => a.MkNum(value, aExpr, vExpr, file, line, member);

        /// <summary>Starts assertions for a nullable <c>ushort?</c> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ushort?> That(this Asserter a, ushort? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null, [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0, [CallerMemberName] string? member = null)
            => a.MkNumN(value, aExpr, vExpr, file, line, member);

        /// <summary>Starts assertions for a <c>byte</c> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<byte> That(this Asserter a, byte value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null, [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0, [CallerMemberName] string? member = null)
            => a.MkNum(value, aExpr, vExpr, file, line, member);

        /// <summary>Starts assertions for a nullable <c>byte?</c> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<byte?> That(this Asserter a, byte? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null, [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0, [CallerMemberName] string? member = null)
            => a.MkNumN(value, aExpr, vExpr, file, line, member);

        /// <summary>Starts assertions for an <c>sbyte</c> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<sbyte> That(this Asserter a, sbyte value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null, [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0, [CallerMemberName] string? member = null)
            => a.MkNum(value, aExpr, vExpr, file, line, member);

        /// <summary>Starts assertions for a nullable <c>sbyte?</c> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<sbyte?> That(this Asserter a, sbyte? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null, [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0, [CallerMemberName] string? member = null)
            => a.MkNumN(value, aExpr, vExpr, file, line, member);

        /// <summary>Starts assertions for a native-sized signed integer <c>nint</c> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<nint> That(this Asserter a, nint value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null, [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0, [CallerMemberName] string? member = null)
            => a.MkNum(value, aExpr, vExpr, file, line, member);

        /// <summary>Starts assertions for a nullable native-sized signed integer <c>nint?</c> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<nint?> That(this Asserter a, nint? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null, [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0, [CallerMemberName] string? member = null)
            => a.MkNumN(value, aExpr, vExpr, file, line, member);

        /// <summary>Starts assertions for a native-sized unsigned integer <c>nuint</c> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<nuint> That(this Asserter a, nuint value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null, [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0, [CallerMemberName] string? member = null)
            => a.MkNum(value, aExpr, vExpr, file, line, member);

        /// <summary>Starts assertions for a nullable native-sized unsigned integer <c>nuint?</c> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<nuint?> That(this Asserter a, nuint? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null, [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0, [CallerMemberName] string? member = null)
            => a.MkNumN(value, aExpr, vExpr, file, line, member);
    }

    [GenerateTypedOverloads(typeof(float), typeof(decimal))]
    public static partial class NumericAssertionsExtensions
    {
        /// <summary>Asserts that the value is within <paramref name="tolerance"/> of <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<double> IsCloseTo(this ValueAssertions<double> a, double expected, double tolerance,
            [CallerArgumentExpression(nameof(expected))] string? exprE = null,
            [CallerArgumentExpression(nameof(tolerance))] string? exprT = null)
        { a.Link("IsCloseTo", exprE, exprT); a.Op(a => CheckOperation.Sync(() => Math.Abs(a.GetValue() - expected) <= tolerance, () => $"Expected {ExprFormat.Inline(a.GetValue())} to be within {ExprFormat.Inline(tolerance, exprT)} of {ExprFormat.Inline(expected, exprE)}, but difference was {Math.Abs(a.GetValue() - expected)}", a.IsSkipped())); return a; }

        /// <summary>Asserts that the value is approximately equal to <paramref name="expected"/> within <paramref name="tolerance"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<double> IsApproximately(this ValueAssertions<double> a, double expected, double tolerance,
            [CallerArgumentExpression(nameof(expected))] string? exprE = null,
            [CallerArgumentExpression(nameof(tolerance))] string? exprT = null)
        { a.Link("IsApproximately", exprE, exprT); a.Op(a => CheckOperation.Sync(() => Math.Abs(a.GetValue() - expected) <= tolerance, () => $"Expected {ExprFormat.Inline(a.GetValue())} to be approximately {ExprFormat.Inline(expected, exprE)} within {ExprFormat.Inline(tolerance, exprT)}, but difference was {Math.Abs(a.GetValue() - expected)}", a.IsSkipped())); return a; }
    }

    [GenerateTypedOverloads(typeof(float), typeof(decimal))]
    public static partial class NullableNumericAssertionsExtensions
    {
        /// <summary>Asserts that the nullable value is present and within <paramref name="tolerance"/> of <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<double?> IsCloseTo(this ValueAssertions<double?> a, double expected, double tolerance,
            [CallerArgumentExpression(nameof(expected))] string? exprE = null,
            [CallerArgumentExpression(nameof(tolerance))] string? exprT = null)
        { a.Link("IsCloseTo", exprE, exprT); a.Op(a => CheckOperation.Sync(() => a.GetValue() is { } v && Math.Abs(v - expected) <= tolerance, () => { var nv = a.GetValue(); return !nv.HasValue ? "Expected a value, but was null" : $"Expected {ExprFormat.Inline(nv.Value)} to be within {ExprFormat.Inline(tolerance, exprT)} of {ExprFormat.Inline(expected, exprE)}, but difference was {Math.Abs(nv.Value - expected)}"; }, a.IsSkipped())); return a; }

        /// <summary>Asserts that the nullable value is present and approximately equal to <paramref name="expected"/> within <paramref name="tolerance"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<double?> IsApproximately(this ValueAssertions<double?> a, double expected, double tolerance,
            [CallerArgumentExpression(nameof(expected))] string? exprE = null,
            [CallerArgumentExpression(nameof(tolerance))] string? exprT = null)
        { a.Link("IsApproximately", exprE, exprT); a.Op(a => CheckOperation.Sync(() => a.GetValue() is { } v && Math.Abs(v - expected) <= tolerance, () => { var nv = a.GetValue(); return !nv.HasValue ? "Expected a value, but was null" : $"Expected {ExprFormat.Inline(nv.Value)} to be approximately {ExprFormat.Inline(expected, exprE)} within {ExprFormat.Inline(tolerance, exprT)}, but difference was {Math.Abs(nv.Value - expected)}"; }, a.IsSkipped())); return a; }
    }

    // ── Non-templatable integer types ─────────────────────────────────────────
    // ulong, nuint, BigInteger, Int128, UInt128 use type-specific divisor types
    // or arithmetic that cannot be safely substituted from int templates.
    // All other integer types (int, long, short, byte, sbyte, uint, ushort, nint
    // and their nullable counterparts) are generated from templates in
    // IntegerTemplates.cs.

    public static class IntegerAssertionsExtensions
    {
        // ulong — unsigned, overflow-safe path separate from signed int template
        /// <summary>Asserts that the value is even.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ulong> IsEven(this ValueAssertions<ulong> a)
        { a.Link("IsEven"); a.Op(a => CheckOperation.Sync(() => a.GetValue() % 2UL == 0, () => $"Expected {a.GetValue()} to be even", a.IsSkipped())); return a; }

        /// <summary>Asserts that the value is odd.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ulong> IsOdd(this ValueAssertions<ulong> a)
        { a.Link("IsOdd"); a.Op(a => CheckOperation.Sync(() => a.GetValue() % 2UL != 0, () => $"Expected {a.GetValue()} to be odd", a.IsSkipped())); return a; }

        /// <summary>Asserts that the value is a multiple of <paramref name="divisor"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ulong> IsMultipleOf(this ValueAssertions<ulong> a, ulong divisor,
            [CallerArgumentExpression(nameof(divisor))] string? expr = null)
        { a.Link("IsMultipleOf", expr); a.Op(a => CheckOperation.Sync(() => divisor != 0 && a.GetValue() % divisor == 0, () => divisor == 0 ? "Divisor cannot be zero" : $"Expected {a.GetValue()} to be a multiple of {ExprFormat.Inline(divisor, expr)}", a.IsSkipped())); return a; }

        /// <summary>Asserts that the value is divisible by <paramref name="divisor"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ulong> IsDivisibleBy(this ValueAssertions<ulong> a, ulong divisor,
            [CallerArgumentExpression(nameof(divisor))] string? expr = null)
            => IsMultipleOf(a, divisor, expr);

        /// <summary>Asserts that the value is not divisible by <paramref name="divisor"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ulong> IsNotDivisibleBy(this ValueAssertions<ulong> a, ulong divisor,
            [CallerArgumentExpression(nameof(divisor))] string? expr = null)
        { a.Link("IsNotDivisibleBy", expr); a.Op(a => CheckOperation.Sync(() => divisor != 0 && a.GetValue() % divisor != 0, () => divisor == 0 ? "Divisor cannot be zero" : $"Expected {a.GetValue()} not to be divisible by {ExprFormat.Inline(divisor, expr)}, but it is", a.IsSkipped())); return a; }

        // nuint — platform-native unsigned type, same overflow concern as ulong
        /// <summary>Asserts that the value is even.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<nuint> IsEven(this ValueAssertions<nuint> a)
        { a.Link("IsEven"); a.Op(a => CheckOperation.Sync(() => a.GetValue() % 2 == 0, () => $"Expected {a.GetValue()} to be even", a.IsSkipped())); return a; }

        /// <summary>Asserts that the value is odd.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<nuint> IsOdd(this ValueAssertions<nuint> a)
        { a.Link("IsOdd"); a.Op(a => CheckOperation.Sync(() => a.GetValue() % 2 != 0, () => $"Expected {a.GetValue()} to be odd", a.IsSkipped())); return a; }

        /// <summary>Asserts that the value is a multiple of <paramref name="divisor"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<nuint> IsMultipleOf(this ValueAssertions<nuint> a, nuint divisor,
            [CallerArgumentExpression(nameof(divisor))] string? expr = null)
        { a.Link("IsMultipleOf", expr); a.Op(a => CheckOperation.Sync(() => divisor != 0 && a.GetValue() % divisor == 0, () => divisor == 0 ? "Divisor cannot be zero" : $"Expected {a.GetValue()} to be a multiple of {ExprFormat.Inline(divisor, expr)}", a.IsSkipped())); return a; }

        /// <summary>Asserts that the value is divisible by <paramref name="divisor"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<nuint> IsDivisibleBy(this ValueAssertions<nuint> a, nuint divisor,
            [CallerArgumentExpression(nameof(divisor))] string? expr = null)
            => IsMultipleOf(a, divisor, expr);

        /// <summary>Asserts that the value is not divisible by <paramref name="divisor"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<nuint> IsNotDivisibleBy(this ValueAssertions<nuint> a, nuint divisor,
            [CallerArgumentExpression(nameof(divisor))] string? expr = null)
        { a.Link("IsNotDivisibleBy", expr); a.Op(a => CheckOperation.Sync(() => divisor != 0 && a.GetValue() % divisor != 0, () => divisor == 0 ? "Divisor cannot be zero" : $"Expected {a.GetValue()} not to be divisible by {ExprFormat.Inline(divisor, expr)}, but it is", a.IsSkipped())); return a; }

        // BigInteger — arbitrary precision, non-nullable
        /// <summary>Asserts that the value is even.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<BigInteger> IsEven(this ValueAssertions<BigInteger> a)
        { a.Link("IsEven"); a.Op(a => CheckOperation.Sync(() => a.GetValue() % 2 == 0, () => $"Expected {a.GetValue()} to be even", a.IsSkipped())); return a; }

        /// <summary>Asserts that the value is odd.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<BigInteger> IsOdd(this ValueAssertions<BigInteger> a)
        { a.Link("IsOdd"); a.Op(a => CheckOperation.Sync(() => a.GetValue() % 2 != 0, () => $"Expected {a.GetValue()} to be odd", a.IsSkipped())); return a; }

        /// <summary>Asserts that the value is a multiple of <paramref name="divisor"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<BigInteger> IsMultipleOf(this ValueAssertions<BigInteger> a, BigInteger divisor,
            [CallerArgumentExpression(nameof(divisor))] string? expr = null)
        { a.Link("IsMultipleOf", expr); a.Op(a => CheckOperation.Sync(() => divisor != 0 && a.GetValue() % divisor == 0, () => divisor == 0 ? "Divisor cannot be zero" : $"Expected {a.GetValue()} to be a multiple of {ExprFormat.Inline(divisor, expr)}", a.IsSkipped())); return a; }

        // BigInteger? — nullable
        /// <summary>Asserts that the nullable value is present and even.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<BigInteger?> IsEven(this ValueAssertions<BigInteger?> a)
        { a.Link("IsEven"); a.Op(a => CheckOperation.Sync(() => a.GetValue() is { } v && v % 2 == 0, () => { var nv = a.GetValue(); return !nv.HasValue ? "Expected a value, but was null" : $"Expected {nv.Value} to be even"; }, a.IsSkipped())); return a; }

        /// <summary>Asserts that the nullable value is present and odd.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<BigInteger?> IsOdd(this ValueAssertions<BigInteger?> a)
        { a.Link("IsOdd"); a.Op(a => CheckOperation.Sync(() => a.GetValue() is { } v && v % 2 != 0, () => { var nv = a.GetValue(); return !nv.HasValue ? "Expected a value, but was null" : $"Expected {nv.Value} to be odd"; }, a.IsSkipped())); return a; }

        /// <summary>Asserts that the nullable value is present and a multiple of <paramref name="divisor"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<BigInteger?> IsMultipleOf(this ValueAssertions<BigInteger?> a, BigInteger divisor,
            [CallerArgumentExpression(nameof(divisor))] string? expr = null)
        { a.Link("IsMultipleOf", expr); a.Op(a => CheckOperation.Sync(() => a.GetValue() is { } v && divisor != 0 && v % divisor == 0, () => { var nv = a.GetValue(); return !nv.HasValue ? "Expected a value, but was null" : divisor == 0 ? "Divisor cannot be zero" : $"Expected {nv.Value} to be a multiple of {ExprFormat.Inline(divisor, expr)}"; }, a.IsSkipped())); return a; }

#if NET7_0_OR_GREATER
        // Int128 — 128-bit signed integer, non-nullable
        /// <summary>Asserts that the value is even.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Int128> IsEven(this ValueAssertions<Int128> a)
        { a.Link("IsEven"); a.Op(a => CheckOperation.Sync(() => a.GetValue() % 2 == 0, () => $"Expected {a.GetValue()} to be even", a.IsSkipped())); return a; }

        /// <summary>Asserts that the value is odd.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Int128> IsOdd(this ValueAssertions<Int128> a)
        { a.Link("IsOdd"); a.Op(a => CheckOperation.Sync(() => a.GetValue() % 2 != 0, () => $"Expected {a.GetValue()} to be odd", a.IsSkipped())); return a; }

        /// <summary>Asserts that the value is a multiple of <paramref name="divisor"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Int128> IsMultipleOf(this ValueAssertions<Int128> a, Int128 divisor,
            [CallerArgumentExpression(nameof(divisor))] string? expr = null)
        { a.Link("IsMultipleOf", expr); a.Op(a => CheckOperation.Sync(() => divisor != 0 && a.GetValue() % divisor == 0, () => divisor == 0 ? "Divisor cannot be zero" : $"Expected {a.GetValue()} to be a multiple of {ExprFormat.Inline(divisor, expr)}", a.IsSkipped())); return a; }

        // Int128? — nullable
        /// <summary>Asserts that the nullable value is present and even.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Int128?> IsEven(this ValueAssertions<Int128?> a)
        { a.Link("IsEven"); a.Op(a => CheckOperation.Sync(() => a.GetValue() is { } v && v % 2 == 0, () => { var nv = a.GetValue(); return !nv.HasValue ? "Expected a value, but was null" : $"Expected {nv.Value} to be even"; }, a.IsSkipped())); return a; }

        /// <summary>Asserts that the nullable value is present and odd.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Int128?> IsOdd(this ValueAssertions<Int128?> a)
        { a.Link("IsOdd"); a.Op(a => CheckOperation.Sync(() => a.GetValue() is { } v && v % 2 != 0, () => { var nv = a.GetValue(); return !nv.HasValue ? "Expected a value, but was null" : $"Expected {nv.Value} to be odd"; }, a.IsSkipped())); return a; }

        /// <summary>Asserts that the nullable value is present and a multiple of <paramref name="divisor"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Int128?> IsMultipleOf(this ValueAssertions<Int128?> a, Int128 divisor,
            [CallerArgumentExpression(nameof(divisor))] string? expr = null)
        { a.Link("IsMultipleOf", expr); a.Op(a => CheckOperation.Sync(() => a.GetValue() is { } v && divisor != 0 && v % divisor == 0, () => { var nv = a.GetValue(); return !nv.HasValue ? "Expected a value, but was null" : divisor == 0 ? "Divisor cannot be zero" : $"Expected {nv.Value} to be a multiple of {ExprFormat.Inline(divisor, expr)}"; }, a.IsSkipped())); return a; }

        // UInt128 — 128-bit unsigned integer
        /// <summary>Asserts that the value is even.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<UInt128> IsEven(this ValueAssertions<UInt128> a)
        { a.Link("IsEven"); a.Op(a => CheckOperation.Sync(() => a.GetValue() % 2 == 0, () => $"Expected {a.GetValue()} to be even", a.IsSkipped())); return a; }

        /// <summary>Asserts that the value is odd.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<UInt128> IsOdd(this ValueAssertions<UInt128> a)
        { a.Link("IsOdd"); a.Op(a => CheckOperation.Sync(() => a.GetValue() % 2 != 0, () => $"Expected {a.GetValue()} to be odd", a.IsSkipped())); return a; }
#endif
    }
}


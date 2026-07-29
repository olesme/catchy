using System.Diagnostics;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    public static partial class ComparableAssertionsExtensions
    {
        // ── Comparison methods for struct types implementing IComparable<T> ──
        // Numeric types: int, uint, long, ulong, short, ushort, byte, sbyte, nint, nuint, Int128, UInt128, float, double, decimal
        // Temporal types: DateTime, DateTimeOffset, TimeSpan, DateOnly, TimeOnly  
        // These methods work for any struct type that implements IComparable<T>.
        // Numeric-specific methods (IsPositive, IsNegative, IsZero, IsEven, IsOdd) are in NumericAssertionsExtensions.

        /// <summary>Asserts that the value is greater than the given threshold.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T> IsGreaterThan<T>(this ValueAssertions<T> a, T threshold,
            [CallerArgumentExpression(nameof(threshold))] string? expr = null)
            where T : struct, IComparable<T>
        {
            a.Link("IsGreaterThan", expr);
            a.Op(a => NumericChecks.GreaterThan(() => (T?)(object?)a.GetValue(), threshold, a.IsSkipped(), expr));
            return a;
        }

        /// <summary>Asserts that the value is at least (greater than or equal to) the given threshold.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T> IsAtLeast<T>(this ValueAssertions<T> a, T threshold,
            [CallerArgumentExpression(nameof(threshold))] string? expr = null)
            where T : struct, IComparable<T>
        {
            a.Link("IsAtLeast", expr);
            a.Op(a => NumericChecks.GreaterThanOrEqualTo(() => (T?)(object?)a.GetValue(), threshold, a.IsSkipped(), expr));
            return a;
        }

        /// <summary>Asserts that the value is greater than or equal to the given threshold.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T> IsGreaterThanOrEqualTo<T>(this ValueAssertions<T> a, T threshold,
            [CallerArgumentExpression(nameof(threshold))] string? expr = null)
            where T : struct, IComparable<T>
        {
            a.Link("IsGreaterThanOrEqualTo", expr);
            a.Op(a => NumericChecks.GreaterThanOrEqualTo(() => (T?)(object?)a.GetValue(), threshold, a.IsSkipped(), expr));
            return a;
        }

        /// <summary>Asserts that the value is at most (less than or equal to) the given threshold.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T> IsAtMost<T>(this ValueAssertions<T> a, T threshold,
            [CallerArgumentExpression(nameof(threshold))] string? expr = null)
            where T : struct, IComparable<T>
        {
            a.Link("IsAtMost", expr);
            a.Op(a => NumericChecks.LessThanOrEqualTo(() => (T?)(object?)a.GetValue(), threshold, a.IsSkipped(), expr));
            return a;
        }

        /// <summary>Asserts that the value is less than or equal to the given threshold.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T> IsLessThanOrEqualTo<T>(this ValueAssertions<T> a, T threshold,
            [CallerArgumentExpression(nameof(threshold))] string? expr = null)
            where T : struct, IComparable<T>
        {
            a.Link("IsLessThanOrEqualTo", expr);
            a.Op(a => NumericChecks.LessThanOrEqualTo(() => (T?)(object?)a.GetValue(), threshold, a.IsSkipped(), expr));
            return a;
        }

        /// <summary>Asserts that the value is less than the given threshold.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T> IsLessThan<T>(this ValueAssertions<T> a, T threshold,
            [CallerArgumentExpression(nameof(threshold))] string? expr = null)
            where T : struct, IComparable<T>
        {
            a.Link("IsLessThan", expr);
            a.Op(a => NumericChecks.LessThan(() => (T?)(object?)a.GetValue(), threshold, a.IsSkipped(), expr));
            return a;
        }

        /// <summary>Asserts that the value is in the inclusive range [min, max].</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T> IsInRange<T>(this ValueAssertions<T> a, T min, T max,
            [CallerArgumentExpression(nameof(min))] string? minExpr = null,
            [CallerArgumentExpression(nameof(max))] string? maxExpr = null)
            where T : struct, IComparable<T>
        {
            a.Link("IsInRange", minExpr, maxExpr);
            a.Op(a => NumericChecks.InRange(() => (T?)(object?)a.GetValue(), min, max, a.IsSkipped(), minExpr, maxExpr));
            return a;
        }

        /// <summary>Asserts that the value is in the range determined by BetweenMode (inclusive or exclusive).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T> IsBetween<T>(this ValueAssertions<T> a, T min, T max,
            [CallerArgumentExpression(nameof(min))] string? minExpr = null,
            [CallerArgumentExpression(nameof(max))] string? maxExpr = null)
            where T : struct, IComparable<T>
        {
            var opts = new BetweenOptions();
            ((IAssertions)a).GetPipeline().Slots.Set(WellKnownSlots.BetweenMode, opts);
            a.Link("IsBetween", minExpr, maxExpr);
            a.Op(a => NumericChecks.IsBetween(() => (T?)(object?)a.GetValue(), min, max, opts, a.IsSkipped(), minExpr, maxExpr));
            return a;
        }

        // ── Nullable variants for struct IComparable<T> ──

        /// <summary>Asserts that the nullable value is present and greater than the given threshold.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T?> IsGreaterThan<T>(this ValueAssertions<T?> a, T threshold,
            [CallerArgumentExpression(nameof(threshold))] string? expr = null)
            where T : struct, IComparable<T>
        {
            a.Link("IsGreaterThan", expr);
            a.Op(a => NumericChecks.GreaterThan(() => a.GetValue(), threshold, a.IsSkipped(), expr));
            return a;
        }

        /// <summary>Asserts that the nullable value is present and at least (greater than or equal to) the given threshold.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T?> IsAtLeast<T>(this ValueAssertions<T?> a, T threshold,
            [CallerArgumentExpression(nameof(threshold))] string? expr = null)
            where T : struct, IComparable<T>
        {
            a.Link("IsAtLeast", expr);
            a.Op(a => NumericChecks.GreaterThanOrEqualTo(() => a.GetValue(), threshold, a.IsSkipped(), expr));
            return a;
        }

        /// <summary>Asserts that the nullable value is present and greater than or equal to the given threshold.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T?> IsGreaterThanOrEqualTo<T>(this ValueAssertions<T?> a, T threshold,
            [CallerArgumentExpression(nameof(threshold))] string? expr = null)
            where T : struct, IComparable<T>
        {
            a.Link("IsGreaterThanOrEqualTo", expr);
            a.Op(a => NumericChecks.GreaterThanOrEqualTo(() => a.GetValue(), threshold, a.IsSkipped(), expr));
            return a;
        }

        /// <summary>Asserts that the nullable value is present and at most (less than or equal to) the given threshold.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T?> IsAtMost<T>(this ValueAssertions<T?> a, T threshold,
            [CallerArgumentExpression(nameof(threshold))] string? expr = null)
            where T : struct, IComparable<T>
        {
            a.Link("IsAtMost", expr);
            a.Op(a => NumericChecks.LessThanOrEqualTo(() => a.GetValue(), threshold, a.IsSkipped(), expr));
            return a;
        }

        /// <summary>Asserts that the nullable value is present and less than or equal to the given threshold.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T?> IsLessThanOrEqualTo<T>(this ValueAssertions<T?> a, T threshold,
            [CallerArgumentExpression(nameof(threshold))] string? expr = null)
            where T : struct, IComparable<T>
        {
            a.Link("IsLessThanOrEqualTo", expr);
            a.Op(a => NumericChecks.LessThanOrEqualTo(() => a.GetValue(), threshold, a.IsSkipped(), expr));
            return a;
        }

        /// <summary>Asserts that the nullable value is present and less than the given threshold.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T?> IsLessThan<T>(this ValueAssertions<T?> a, T threshold,
            [CallerArgumentExpression(nameof(threshold))] string? expr = null)
            where T : struct, IComparable<T>
        {
            a.Link("IsLessThan", expr);
            a.Op(a => NumericChecks.LessThan(() => a.GetValue(), threshold, a.IsSkipped(), expr));
            return a;
        }

        /// <summary>Asserts that the nullable value is present and in the inclusive range [min, max].</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T?> IsInRange<T>(this ValueAssertions<T?> a, T min, T max,
            [CallerArgumentExpression(nameof(min))] string? minExpr = null,
            [CallerArgumentExpression(nameof(max))] string? maxExpr = null)
            where T : struct, IComparable<T>
        {
            a.Link("IsInRange", minExpr, maxExpr);
            a.Op(a => NumericChecks.InRange(() => a.GetValue(), min, max, a.IsSkipped(), minExpr, maxExpr));
            return a;
        }

        /// <summary>Asserts that the nullable value is present and in the range determined by BetweenMode (inclusive or exclusive).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T?> IsBetween<T>(this ValueAssertions<T?> a, T min, T max,
            [CallerArgumentExpression(nameof(min))] string? minExpr = null,
            [CallerArgumentExpression(nameof(max))] string? maxExpr = null)
            where T : struct, IComparable<T>
        {
            var opts = new BetweenOptions();
            ((IAssertions)a).GetPipeline().Slots.Set(WellKnownSlots.BetweenMode, opts);
            a.Link("IsBetween", minExpr, maxExpr);
            a.Op(a => NumericChecks.IsBetween(() => a.GetValue(), min, max, opts, a.IsSkipped(), minExpr, maxExpr));
            return a;
        }

        /// <summary>
        /// Marks a preceding <c>IsBetween</c> assertion to use exclusive bounds (<c>min &lt; value &lt; max</c>).
        /// </summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T> Exclusively<T>(this ValueAssertions<T> a)
            where T : struct, IComparable<T>
        {
            if (((IAssertions)a).GetPipeline().Slots.TryGet(WellKnownSlots.BetweenMode, out var opts))
                opts.Exclusive = true;
            a.Link("Exclusively");
            return a;
        }

        /// <summary>
        /// Marks a preceding <c>IsBetween</c> assertion to use exclusive bounds (<c>min &lt; value &lt; max</c>).
        /// </summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T?> Exclusively<T>(this ValueAssertions<T?> a)
            where T : struct, IComparable<T>
        {
            if (((IAssertions)a).GetPipeline().Slots.TryGet(WellKnownSlots.BetweenMode, out var opts))
                opts.Exclusive = true;
            a.Link("Exclusively");
            return a;
        }

        /// <summary>Asserts that the value is greater than its type's default (i.e. positive). Numeric-only.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T> IsPositive<T>(this ValueAssertions<T> a)
            where T : struct, IComparable<T>
        {
            a.Link("IsPositive");
            a.Op(a => NumericChecks.IsPositive<T>(() => a.GetValue(), a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the value is less than its type's default (i.e. negative). Numeric-only.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T> IsNegative<T>(this ValueAssertions<T> a)
            where T : struct, IComparable<T>
        {
            a.Link("IsNegative");
            a.Op(a => NumericChecks.IsNegative<T>(() => a.GetValue(), a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the value equals its type's default (i.e. zero). Numeric-only.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T> IsZero<T>(this ValueAssertions<T> a)
            where T : struct, IComparable<T>
        {
            a.Link("IsZero");
            a.Op(a => NumericChecks.IsZero<T>(() => a.GetValue(), a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the nullable value is present and greater than its type's default (i.e. positive). Numeric-only.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T?> IsPositive<T>(this ValueAssertions<T?> a)
            where T : struct, IComparable<T>
        {
            a.Link("IsPositive");
            a.Op(a => NumericChecks.IsPositive<T>(() => a.GetValue(), a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the nullable value is present and less than its type's default (i.e. negative). Numeric-only.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T?> IsNegative<T>(this ValueAssertions<T?> a)
            where T : struct, IComparable<T>
        {
            a.Link("IsNegative");
            a.Op(a => NumericChecks.IsNegative<T>(() => a.GetValue(), a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the nullable value is present and equals its type's default (i.e. zero). Numeric-only.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T?> IsZero<T>(this ValueAssertions<T?> a)
            where T : struct, IComparable<T>
        {
            a.Link("IsZero");
            a.Op(a => NumericChecks.IsZero<T>(() => a.GetValue(), a.IsSkipped()));
            return a;
        }
    }

    // ── Reference-type comparable assertions (string, Version, Guid, etc.) ──
    // Extension class for class types implementing IComparable<T>.
    // These assertions work for reference types that implement comparison semantics.
    public static partial class ReferenceComparableAssertionsExtensions
    {
        /// <summary>Asserts that the value is greater than the given threshold. Returns false if value is null.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T> IsGreaterThan<T>(this ValueAssertions<T> a, T threshold,
            [CallerArgumentExpression(nameof(threshold))] string? expr = null)
            where T : class, IComparable<T>
        {
            a.Link("IsGreaterThan", expr);
            a.Op(a => NumericChecks.GreaterThan(() => a.GetValue(), threshold, a.IsSkipped(), expr));
            return a;
        }

        /// <summary>Asserts that the value is at least (greater than or equal to) the given threshold. Returns false if value is null.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T> IsAtLeast<T>(this ValueAssertions<T> a, T threshold,
            [CallerArgumentExpression(nameof(threshold))] string? expr = null)
            where T : class, IComparable<T>
        {
            a.Link("IsAtLeast", expr);
            a.Op(a => NumericChecks.GreaterThanOrEqualTo(() => a.GetValue(), threshold, a.IsSkipped(), expr));
            return a;
        }

        /// <summary>Asserts that the value is greater than or equal to the given threshold. Returns false if value is null.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T> IsGreaterThanOrEqualTo<T>(this ValueAssertions<T> a, T threshold,
            [CallerArgumentExpression(nameof(threshold))] string? expr = null)
            where T : class, IComparable<T>
        {
            a.Link("IsGreaterThanOrEqualTo", expr);
            a.Op(a => NumericChecks.GreaterThanOrEqualTo(() => a.GetValue(), threshold, a.IsSkipped(), expr));
            return a;
        }

        /// <summary>Asserts that the value is at most (less than or equal to) the given threshold. Returns false if value is null.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T> IsAtMost<T>(this ValueAssertions<T> a, T threshold,
            [CallerArgumentExpression(nameof(threshold))] string? expr = null)
            where T : class, IComparable<T>
        {
            a.Link("IsAtMost", expr);
            a.Op(a => NumericChecks.LessThanOrEqualTo(() => a.GetValue(), threshold, a.IsSkipped(), expr));
            return a;
        }

        /// <summary>Asserts that the value is less than or equal to the given threshold. Returns false if value is null.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T> IsLessThanOrEqualTo<T>(this ValueAssertions<T> a, T threshold,
            [CallerArgumentExpression(nameof(threshold))] string? expr = null)
            where T : class, IComparable<T>
        {
            a.Link("IsLessThanOrEqualTo", expr);
            a.Op(a => NumericChecks.LessThanOrEqualTo(() => a.GetValue(), threshold, a.IsSkipped(), expr));
            return a;
        }

        /// <summary>Asserts that the value is less than the given threshold. Returns false if value is null.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T> IsLessThan<T>(this ValueAssertions<T> a, T threshold,
            [CallerArgumentExpression(nameof(threshold))] string? expr = null)
            where T : class, IComparable<T>
        {
            a.Link("IsLessThan", expr);
            a.Op(a => NumericChecks.LessThan(() => a.GetValue(), threshold, a.IsSkipped(), expr));
            return a;
        }

        /// <summary>Asserts that the value is in the inclusive range [min, max]. Returns false if value is null.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T> IsInRange<T>(this ValueAssertions<T> a, T min, T max,
            [CallerArgumentExpression(nameof(min))] string? minExpr = null,
            [CallerArgumentExpression(nameof(max))] string? maxExpr = null)
            where T : class, IComparable<T>
        {
            a.Link("IsInRange", minExpr, maxExpr);
            a.Op(a => NumericChecks.InRange(() => a.GetValue(), min, max, a.IsSkipped(), minExpr, maxExpr));
            return a;
        }

        /// <summary>Asserts that the value is in the range determined by BetweenMode (inclusive or exclusive). Returns false if value is null.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T> IsBetween<T>(this ValueAssertions<T> a, T min, T max,
            [CallerArgumentExpression(nameof(min))] string? minExpr = null,
            [CallerArgumentExpression(nameof(max))] string? maxExpr = null)
            where T : class, IComparable<T>
        {
            var opts = new BetweenOptions();
            ((IAssertions)a).GetPipeline().Slots.Set(WellKnownSlots.BetweenMode, opts);
            a.Link("IsBetween", minExpr, maxExpr);
            a.Op(a => NumericChecks.IsBetween(() => a.GetValue(), min, max, opts, a.IsSkipped(), minExpr, maxExpr));
            return a;
        }
    }
}

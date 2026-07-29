using System.Diagnostics;
using Catchy.Sdk;

namespace Catchy
{
    [GenerateTypedOverloads(typeof(float))]
    public static partial class FloatingPointTemplates
    {
        /// <summary>Asserts that the value is <see cref="double.NaN"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<double> IsNaN(this ValueAssertions<double> a)
        { a.Link("IsNaN"); a.Op(a => CheckOperation.Sync(() => double.IsNaN(a.GetValue()), () => $"Expected {a.GetValue()} to be NaN", a.IsSkipped())); return a; }

        /// <summary>Asserts that the nullable value is present and <see cref="double.NaN"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<double?> IsNaN(this ValueAssertions<double?> a)
        { a.Link("IsNaN"); a.Op(a => CheckOperation.Sync(() => a.GetValue() is { } v && double.IsNaN(v), () => { var nv = a.GetValue(); return !nv.HasValue ? "Expected a value, but was null" : $"Expected {nv.Value} to be NaN"; }, a.IsSkipped())); return a; }

        /// <summary>Asserts that the value is not <see cref="double.NaN"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<double> IsNotNaN(this ValueAssertions<double> a)
        { a.Link("IsNotNaN"); a.Op(a => CheckOperation.Sync(() => !double.IsNaN(a.GetValue()), () => $"Expected {a.GetValue()} not to be NaN, but it was", a.IsSkipped())); return a; }

        /// <summary>Asserts that the value is positive or negative infinity.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<double> IsInfinity(this ValueAssertions<double> a)
        { a.Link("IsInfinity"); a.Op(a => CheckOperation.Sync(() => double.IsInfinity(a.GetValue()), () => $"Expected {a.GetValue()} to be infinity", a.IsSkipped())); return a; }

        /// <summary>Asserts that the nullable value is present and positive or negative infinity.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<double?> IsInfinity(this ValueAssertions<double?> a)
        { a.Link("IsInfinity"); a.Op(a => CheckOperation.Sync(() => a.GetValue() is { } v && double.IsInfinity(v), () => { var nv = a.GetValue(); return !nv.HasValue ? "Expected a value, but was null" : $"Expected {nv.Value} to be infinity"; }, a.IsSkipped())); return a; }

        /// <summary>Asserts that the value is not infinity.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<double> IsNotInfinity(this ValueAssertions<double> a)
        { a.Link("IsNotInfinity"); a.Op(a => CheckOperation.Sync(() => !double.IsInfinity(a.GetValue()), () => $"Expected {a.GetValue()} not to be infinity, but it was", a.IsSkipped())); return a; }

        /// <summary>Asserts that the value is positive infinity.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<double> IsPositiveInfinity(this ValueAssertions<double> a)
        { a.Link("IsPositiveInfinity"); a.Op(a => CheckOperation.Sync(() => double.IsPositiveInfinity(a.GetValue()), () => $"Expected {a.GetValue()} to be positive infinity", a.IsSkipped())); return a; }

        /// <summary>Asserts that the value is negative infinity.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<double> IsNegativeInfinity(this ValueAssertions<double> a)
        { a.Link("IsNegativeInfinity"); a.Op(a => CheckOperation.Sync(() => double.IsNegativeInfinity(a.GetValue()), () => $"Expected {a.GetValue()} to be negative infinity", a.IsSkipped())); return a; }

#if NET5_0_OR_GREATER
        /// <summary>Asserts that the value is finite (not infinity and not NaN).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<double> IsFinite(this ValueAssertions<double> a)
        { a.Link("IsFinite"); a.Op(a => CheckOperation.Sync(() => double.IsFinite(a.GetValue()), () => $"Expected {a.GetValue()} to be finite", a.IsSkipped())); return a; }

        /// <summary>Asserts that the nullable value is present and finite (not infinity and not NaN).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<double?> IsFinite(this ValueAssertions<double?> a)
        { a.Link("IsFinite"); a.Op(a => CheckOperation.Sync(() => a.GetValue() is { } v && double.IsFinite(v), () => { var nv = a.GetValue(); return !nv.HasValue ? "Expected a value, but was null" : $"Expected {nv.Value} to be finite"; }, a.IsSkipped())); return a; }
#endif
    }
}


using System.Diagnostics;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    /// <summary>Provides fluent temporal assertions for date and time values.</summary>
    public static partial class TemporalComparisonTemplates
    {
        /// <summary>Asserts that the value is before <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        [GenerateTypedOverloads(typeof(DateTimeOffset?), TemplateType = typeof(DateTime?))]
        public static ValueAssertions<DateTime?> IsBefore(this ValueAssertions<DateTime?> a,
            DateTime expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        {
            a.Link("IsBefore", expr);
            a.Op(a => DateTimeChecks.IsBefore(a.GetValue(), expected, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the value is after <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        [GenerateTypedOverloads(typeof(DateTimeOffset?), TemplateType = typeof(DateTime?))]
        public static ValueAssertions<DateTime?> IsAfter(this ValueAssertions<DateTime?> a,
            DateTime expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        {
            a.Link("IsAfter", expr);
            a.Op(a => DateTimeChecks.IsAfter(a.GetValue(), expected, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the value has the specified year component.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<DateTime?> HasYear(this ValueAssertions<DateTime?> a,
            int year,
            [CallerArgumentExpression(nameof(year))] string? expr = null)
        {
            a.Link("HasYear", expr);
            a.Op(a => DateTimeChecks.HasYear(a.GetValue(), year, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the value has the specified month component.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<DateTime?> HasMonth(this ValueAssertions<DateTime?> a,
            int month,
            [CallerArgumentExpression(nameof(month))] string? expr = null)
        {
            a.Link("HasMonth", expr);
            a.Op(a => DateTimeChecks.HasMonth(a.GetValue(), month, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the value has the specified day component.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<DateTime?> HasDay(this ValueAssertions<DateTime?> a,
            int day,
            [CallerArgumentExpression(nameof(day))] string? expr = null)
        {
            a.Link("HasDay", expr);
            a.Op(a => DateTimeChecks.HasDay(a.GetValue(), day, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the value has the specified hour component.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<DateTime?> HasHour(this ValueAssertions<DateTime?> a,
            int hour,
            [CallerArgumentExpression(nameof(hour))] string? expr = null)
        {
            a.Link("HasHour", expr);
            a.Op(a => DateTimeChecks.HasHour(a.GetValue(), hour, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the value has the specified minute component.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<DateTime?> HasMinute(this ValueAssertions<DateTime?> a,
            int minute,
            [CallerArgumentExpression(nameof(minute))] string? expr = null)
        {
            a.Link("HasMinute", expr);
            a.Op(a => DateTimeChecks.HasMinute(a.GetValue(), minute, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the value has the specified second component.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<DateTime?> HasSecond(this ValueAssertions<DateTime?> a,
            int second,
            [CallerArgumentExpression(nameof(second))] string? expr = null)
        {
            a.Link("HasSecond", expr);
            a.Op(a => DateTimeChecks.HasSecond(a.GetValue(), second, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the value represents UTC time.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<DateTime?> IsUtc(this ValueAssertions<DateTime?> a)
        {
            a.Link("IsUtc");
            a.Op(a => DateTimeChecks.IsUtc(a.GetValue(), a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the value represents local time.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<DateTime?> IsLocal(this ValueAssertions<DateTime?> a)
        {
            a.Link("IsLocal");
            a.Op(a => DateTimeChecks.IsLocal(a.GetValue(), a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the value falls on the current local date.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<DateTime?> IsToday(this ValueAssertions<DateTime?> a)
        {
            a.Link("IsToday");
            a.Op(a => DateTimeChecks.IsToday(a.GetValue(), a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the value is within <paramref name="within"/> from <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        [GenerateTypedOverloads(typeof(DateTimeOffset?), TemplateType = typeof(DateTime?))]
        public static ValueAssertions<DateTime?> IsCloseTo(this ValueAssertions<DateTime?> a,
            DateTime expected,
            TimeSpan within,
            [CallerArgumentExpression(nameof(expected))] string? expr = null,
            [CallerArgumentExpression(nameof(within))] string? withinExpr = null)
        {
            a.Link("IsCloseTo", expr, withinExpr);
            a.Op(a => DateTimeChecks.IsCloseTo(a.GetValue(), expected, within, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the value is earlier than the current moment.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        [GenerateTypedOverloads(typeof(DateTimeOffset?), TemplateType = typeof(DateTime?))]
        public static ValueAssertions<DateTime?> IsInThePast(this ValueAssertions<DateTime?> a)
        {
            a.Link("IsInThePast");
            a.Op(a => DateTimeChecks.IsInThePast(a.GetValue(), a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the value is later than the current moment.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        [GenerateTypedOverloads(typeof(DateTimeOffset?), TemplateType = typeof(DateTime?))]
        public static ValueAssertions<DateTime?> IsInTheFuture(this ValueAssertions<DateTime?> a)
        {
            a.Link("IsInTheFuture");
            a.Op(a => DateTimeChecks.IsInTheFuture(a.GetValue(), a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the value is within the last <paramref name="duration"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        [GenerateTypedOverloads(typeof(DateTimeOffset?), TemplateType = typeof(DateTime?))]
        public static ValueAssertions<DateTime?> IsWithinTheLast(this ValueAssertions<DateTime?> a,
            TimeSpan duration,
            [CallerArgumentExpression(nameof(duration))] string? expr = null)
        {
            a.Link("IsWithinTheLast", expr);
            a.Op(a => DateTimeChecks.IsWithinTheLast(a.GetValue(), duration, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the value is within the next <paramref name="duration"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        [GenerateTypedOverloads(typeof(DateTimeOffset?), TemplateType = typeof(DateTime?))]
        public static ValueAssertions<DateTime?> IsWithinTheNext(this ValueAssertions<DateTime?> a,
            TimeSpan duration,
            [CallerArgumentExpression(nameof(duration))] string? expr = null)
        {
            a.Link("IsWithinTheNext", expr);
            a.Op(a => DateTimeChecks.IsWithinTheNext(a.GetValue(), duration, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the value has the specified UTC offset.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<DateTimeOffset?> HasOffset(this ValueAssertions<DateTimeOffset?> a,
            TimeSpan expectedOffset,
            [CallerArgumentExpression(nameof(expectedOffset))] string? expr = null)
        {
            a.Link("HasOffset", expr);
            a.Op(a => DateTimeChecks.HasOffset(a.GetValue(), expectedOffset, a.IsSkipped()));
            return a;
        }
    }
}

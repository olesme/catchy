using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    public enum Month
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }

    public static partial class AsserterExtensions
    {
        public static ValueAssertions<Month?> That(this Asserter a, Month value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<Month?>(p, value);
        }

        public static ValueAssertions<Month?> That(this Asserter a, Month? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<Month?>(p, value);
        }
    }

    public static class MonthAssertExtensions
    {
        /// <summary>Asserts that the month equals <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> Is(this ValueAssertions<Month?> a, Month expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("Is", expr); a.Op(a => CheckOperation.Sync(() => a.GetValue() == expected, () => $"Expected month to be {expected}", a.IsSkipped())); return a; }

        /// <summary>Asserts that the month numeric value equals <paramref name="expected"/> (1..12).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> Is(this ValueAssertions<Month?> a, int expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("Is", expr); a.Op(a => CheckOperation.Sync(() => (int?)a.GetValue() == expected, () => $"Expected month to be {expected}", a.IsSkipped())); return a; }

        /// <summary>Asserts that the month equals <paramref name="expected"/> parsed invariantly (name, abbreviation, or numeric string).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> Is(this ValueAssertions<Month?> a, string expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        {
            a.Link("Is", expr);
            a.Op(a => CheckOperation.Sync(() => a.GetValue() == expected.ToMonth(), () => $"Expected month to be {expected}", a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the month does not equal <paramref name="unexpected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsNot(this ValueAssertions<Month?> a, Month unexpected,
            [CallerArgumentExpression(nameof(unexpected))] string? expr = null)
        { a.Link("IsNot", expr); a.Op(a => CheckOperation.Sync(() => a.GetValue() != unexpected, () => $"Expected month not to be {unexpected}", a.IsSkipped())); return a; }

        /// <summary>Asserts that the month numeric value does not equal <paramref name="unexpected"/> (1..12).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsNot(this ValueAssertions<Month?> a, int unexpected,
            [CallerArgumentExpression(nameof(unexpected))] string? expr = null)
        { a.Link("IsNot", expr); a.Op(a => CheckOperation.Sync(() => (int?)a.GetValue() != unexpected, () => $"Expected month not to be {unexpected}", a.IsSkipped())); return a; }

        /// <summary>Asserts that the month does not equal <paramref name="unexpected"/> parsed invariantly (name, abbreviation, or numeric string).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsNot(this ValueAssertions<Month?> a, string unexpected,
            [CallerArgumentExpression(nameof(unexpected))] string? expr = null)
        {
            a.Link("IsNot", expr);
            a.Op(a => CheckOperation.Sync(() => a.GetValue() != unexpected.ToMonth(), () => $"Expected month not to be {unexpected}", a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the month is before <paramref name="other"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsBefore(this ValueAssertions<Month?> a, Month other,
            [CallerArgumentExpression(nameof(other))] string? expr = null)
        {
            a.Link("IsBefore", expr);
            a.Op(a => CheckOperation.Sync(() => a.GetValue() < other, () => $"Expected month to be before {other}", a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the month numeric value is before <paramref name="other"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsBefore(this ValueAssertions<Month?> a, int other,
            [CallerArgumentExpression(nameof(other))] string? expr = null)
        {
            a.Link("IsBefore", expr);
            a.Op(a => CheckOperation.Sync(() => (int?)a.GetValue() < other, () => $"Expected month to be before {other}", a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the month is before <paramref name="other"/> parsed invariantly.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsBefore(this ValueAssertions<Month?> a, string other,
            [CallerArgumentExpression(nameof(other))] string? expr = null)
        {
            a.Link("IsBefore", expr);
            a.Op(a => CheckOperation.Sync(() => a.GetValue() < other.ToMonth(), () => $"Expected month to be before {other}", a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the month is after <paramref name="other"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsAfter(this ValueAssertions<Month?> a, Month other,
            [CallerArgumentExpression(nameof(other))] string? expr = null)
        {
            a.Link("IsAfter", expr);
            a.Op(a => CheckOperation.Sync(() => a.GetValue() > other, () => $"Expected month to be after {other}", a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the month numeric value is after <paramref name="other"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsAfter(this ValueAssertions<Month?> a, int other,
            [CallerArgumentExpression(nameof(other))] string? expr = null)
        {
            a.Link("IsAfter", expr);
            a.Op(a => CheckOperation.Sync(() => (int?)a.GetValue() > other, () => $"Expected month to be after {other}", a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the month is January.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsJanuary(this ValueAssertions<Month?> a)
        { a.Link("IsJanuary"); a.Op(a => CheckOperation.Sync(() => a.GetValue() == Month.January, () => "Expected month to be January", a.IsSkipped())); return a; }

        /// <summary>Asserts that the month is not January.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsNotJanuary(this ValueAssertions<Month?> a)
        { a.Link("IsNotJanuary"); a.Op(a => CheckOperation.Sync(() => a.GetValue() != Month.January, () => "Expected month not to be January", a.IsSkipped())); return a; }

        /// <summary>Asserts that the month is February.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsFebruary(this ValueAssertions<Month?> a)
        { a.Link("IsFebruary"); a.Op(a => CheckOperation.Sync(() => a.GetValue() == Month.February, () => "Expected month to be February", a.IsSkipped())); return a; }

        /// <summary>Asserts that the month is not February.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsNotFebruary(this ValueAssertions<Month?> a)
        { a.Link("IsNotFebruary"); a.Op(a => CheckOperation.Sync(() => a.GetValue() != Month.February, () => "Expected month not to be February", a.IsSkipped())); return a; }

        /// <summary>Asserts that the month is March.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsMarch(this ValueAssertions<Month?> a)
        { a.Link("IsMarch"); a.Op(a => CheckOperation.Sync(() => a.GetValue() == Month.March, () => "Expected month to be March", a.IsSkipped())); return a; }

        /// <summary>Asserts that the month is not March.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsNotMarch(this ValueAssertions<Month?> a)
        { a.Link("IsNotMarch"); a.Op(a => CheckOperation.Sync(() => a.GetValue() != Month.March, () => "Expected month not to be March", a.IsSkipped())); return a; }

        /// <summary>Asserts that the month is April.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsApril(this ValueAssertions<Month?> a)
        { a.Link("IsApril"); a.Op(a => CheckOperation.Sync(() => a.GetValue() == Month.April, () => "Expected month to be April", a.IsSkipped())); return a; }

        /// <summary>Asserts that the month is not April.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsNotApril(this ValueAssertions<Month?> a)
        { a.Link("IsNotApril"); a.Op(a => CheckOperation.Sync(() => a.GetValue() != Month.April, () => "Expected month not to be April", a.IsSkipped())); return a; }

        /// <summary>Asserts that the month is May.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsMay(this ValueAssertions<Month?> a)
        { a.Link("IsMay"); a.Op(a => CheckOperation.Sync(() => a.GetValue() == Month.May, () => "Expected month to be May", a.IsSkipped())); return a; }

        /// <summary>Asserts that the month is not May.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsNotMay(this ValueAssertions<Month?> a)
        { a.Link("IsNotMay"); a.Op(a => CheckOperation.Sync(() => a.GetValue() != Month.May, () => "Expected month not to be May", a.IsSkipped())); return a; }

        /// <summary>Asserts that the month is June.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsJune(this ValueAssertions<Month?> a)
        { a.Link("IsJune"); a.Op(a => CheckOperation.Sync(() => a.GetValue() == Month.June, () => "Expected month to be June", a.IsSkipped())); return a; }

        /// <summary>Asserts that the month is not June.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsNotJune(this ValueAssertions<Month?> a)
        { a.Link("IsNotJune"); a.Op(a => CheckOperation.Sync(() => a.GetValue() != Month.June, () => "Expected month not to be June", a.IsSkipped())); return a; }

        /// <summary>Asserts that the month is July.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsJuly(this ValueAssertions<Month?> a)
        { a.Link("IsJuly"); a.Op(a => CheckOperation.Sync(() => a.GetValue() == Month.July, () => "Expected month to be July", a.IsSkipped())); return a; }

        /// <summary>Asserts that the month is not July.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsNotJuly(this ValueAssertions<Month?> a)
        { a.Link("IsNotJuly"); a.Op(a => CheckOperation.Sync(() => a.GetValue() != Month.July, () => "Expected month not to be July", a.IsSkipped())); return a; }

        /// <summary>Asserts that the month is August.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsAugust(this ValueAssertions<Month?> a)
        { a.Link("IsAugust"); a.Op(a => CheckOperation.Sync(() => a.GetValue() == Month.August, () => "Expected month to be August", a.IsSkipped())); return a; }

        /// <summary>Asserts that the month is not August.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsNotAugust(this ValueAssertions<Month?> a)
        { a.Link("IsNotAugust"); a.Op(a => CheckOperation.Sync(() => a.GetValue() != Month.August, () => "Expected month not to be August", a.IsSkipped())); return a; }

        /// <summary>Asserts that the month is September.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsSeptember(this ValueAssertions<Month?> a)
        { a.Link("IsSeptember"); a.Op(a => CheckOperation.Sync(() => a.GetValue() == Month.September, () => "Expected month to be September", a.IsSkipped())); return a; }

        /// <summary>Asserts that the month is not September.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsNotSeptember(this ValueAssertions<Month?> a)
        { a.Link("IsNotSeptember"); a.Op(a => CheckOperation.Sync(() => a.GetValue() != Month.September, () => "Expected month not to be September", a.IsSkipped())); return a; }

        /// <summary>Asserts that the month is October.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsOctober(this ValueAssertions<Month?> a)
        { a.Link("IsOctober"); a.Op(a => CheckOperation.Sync(() => a.GetValue() == Month.October, () => "Expected month to be October", a.IsSkipped())); return a; }

        /// <summary>Asserts that the month is not October.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsNotOctober(this ValueAssertions<Month?> a)
        { a.Link("IsNotOctober"); a.Op(a => CheckOperation.Sync(() => a.GetValue() != Month.October, () => "Expected month not to be October", a.IsSkipped())); return a; }

        /// <summary>Asserts that the month is November.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsNovember(this ValueAssertions<Month?> a)
        { a.Link("IsNovember"); a.Op(a => CheckOperation.Sync(() => a.GetValue() == Month.November, () => "Expected month to be November", a.IsSkipped())); return a; }

        /// <summary>Asserts that the month is not November.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsNotNovember(this ValueAssertions<Month?> a)
        { a.Link("IsNotNovember"); a.Op(a => CheckOperation.Sync(() => a.GetValue() != Month.November, () => "Expected month not to be November", a.IsSkipped())); return a; }

        /// <summary>Asserts that the month is December.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsDecember(this ValueAssertions<Month?> a)
        { a.Link("IsDecember"); a.Op(a => CheckOperation.Sync(() => a.GetValue() == Month.December, () => "Expected month to be December", a.IsSkipped())); return a; }

        /// <summary>Asserts that the month is not December.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Month?> IsNotDecember(this ValueAssertions<Month?> a)
        { a.Link("IsNotDecember"); a.Op(a => CheckOperation.Sync(() => a.GetValue() != Month.December, () => "Expected month not to be December", a.IsSkipped())); return a; }
    }

    namespace Sdk
    {
        public static partial class PrimitivesAccessors
        {
            /// <summary>Parses a month from invariant text (full name/abbreviation) or numeric string.</summary>
            public static Month ToMonth(this string? value)
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Month name is null or empty.", nameof(value));

                var normalized = value?.Trim().Normalize().ToUpperInvariant();

                if (int.TryParse(normalized, NumberStyles.Integer, CultureInfo.InvariantCulture, out var monthNumber))
                {
                    if (monthNumber is >= 1 and <= 12)
                        return (Month)monthNumber;

                    throw new ArgumentException($"Invalid month number: {value}", nameof(value));
                }

                return normalized switch
                {
                    "JANUARY" or "JAN" => Month.January,
                    "FEBRUARY" or "FEB" => Month.February,
                    "MARCH" or "MAR" => Month.March,
                    "APRIL" or "APR" => Month.April,
                    "MAY" => Month.May,
                    "JUNE" or "JUN" => Month.June,
                    "JULY" or "JUL" => Month.July,
                    "AUGUST" or "AUG" => Month.August,
                    "SEPTEMBER" or "SEPT" or "SEP" => Month.September,
                    "OCTOBER" or "OCT" => Month.October,
                    "NOVEMBER" or "NOV" => Month.November,
                    "DECEMBER" or "DEC" => Month.December,
                    _ => throw new ArgumentException($"Invalid month name: {value}", nameof(value))
                };
            }
        }
    }
}


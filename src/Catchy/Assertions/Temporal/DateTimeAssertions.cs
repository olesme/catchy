using System.Diagnostics;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    public static partial class AsserterExtensions
    {
        /// <summary>Starts assertions for a <see cref="DateTime"/> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        [GenerateTypedOverloads(typeof(DateTimeOffset), TemplateType = typeof(DateTime))]
        public static ValueAssertions<DateTime?> That(this Asserter a, DateTime value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
            => new(a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member), value);

        /// <summary>Starts assertions for a nullable <see cref="DateTime"/> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        [GenerateTypedOverloads(typeof(DateTimeOffset?), TemplateType = typeof(DateTime?))]
        public static ValueAssertions<DateTime?> That(this Asserter a, DateTime? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
            => new(a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member), value);

#if NET6_0_OR_GREATER
        /// <summary>Starts assertions for a <see cref="DateOnly"/> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<DateOnly> That(this Asserter a, DateOnly value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
            => new(a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member), value);

        /// <summary>Starts assertions for a nullable <see cref="DateOnly"/> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<DateOnly?> That(this Asserter a, DateOnly? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
            => new(a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member), value);

        /// <summary>Starts assertions for a <see cref="TimeOnly"/> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<TimeOnly> That(this Asserter a, TimeOnly value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
            => new(a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member), value);

        /// <summary>Starts assertions for a nullable <see cref="TimeOnly"/> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<TimeOnly?> That(this Asserter a, TimeOnly? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
            => new(a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member), value);
#endif

        /// <summary>Starts assertions for a <see cref="TimeSpan"/> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<TimeSpan> That(this Asserter a, TimeSpan value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
            => new(a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member), value);

        /// <summary>Starts assertions for a nullable <see cref="TimeSpan"/> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<TimeSpan?> That(this Asserter a, TimeSpan? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
            => new(a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member), value);
    }

    /// <summary>Provides component projections for <see cref="DateTime"/> assertion chains.</summary>
    public static class DateTimeAssertExtensions
    {
        /// <summary>Projects the year component.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<int?> Year<TSelf>(this TSelf a) where TSelf : ValueAssertions<DateTime?>
        { a.Link("Year"); return new(a.GetPipeline(), a.GetValue()?.Year); }

        /// <summary>Projects the month component.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<Month?> Month<TSelf>(this TSelf a) where TSelf : ValueAssertions<DateTime?>
        { a.Link("Month"); return new(a.GetPipeline(), (Month?)a.GetValue()?.Month); }

        /// <summary>Projects the day-of-week component.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<DayOfWeek?> DayOfWeek<TSelf>(this TSelf a) where TSelf : ValueAssertions<DateTime?>
        { a.Link("DayOfWeek"); return new(a.GetPipeline(), a.GetValue()?.DayOfWeek); }

        /// <summary>Projects the day component.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<int?> Day<TSelf>(this TSelf a) where TSelf : ValueAssertions<DateTime?>
        { a.Link("Day"); return new(a.GetPipeline(), a.GetValue()?.Day); }

        /// <summary>Projects the hour component.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<int?> Hour<TSelf>(this TSelf a) where TSelf : ValueAssertions<DateTime?>
        { a.Link("Hour"); return new(a.GetPipeline(), a.GetValue()?.Hour); }

        /// <summary>Projects the minute component.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<int?> Minute<TSelf>(this TSelf a) where TSelf : ValueAssertions<DateTime?>
        { a.Link("Minute"); return new(a.GetPipeline(), a.GetValue()?.Minute); }

        /// <summary>Projects the second component.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<int?> Second<TSelf>(this TSelf a) where TSelf : ValueAssertions<DateTime?>
        { a.Link("Second"); return new(a.GetPipeline(), a.GetValue()?.Second); }

        /// <summary>Projects the millisecond component.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<int?> Millisecond<TSelf>(this TSelf a) where TSelf : ValueAssertions<DateTime?>
        { a.Link("Millisecond"); return new(a.GetPipeline(), a.GetValue()?.Millisecond); }
    }

#if NET6_0_OR_GREATER
    /// <summary>Provides component projections for <see cref="DateOnly"/> assertion chains.</summary>
    public static class DateOnlyAssertExtensions
    {
        /// <summary>Projects the year component.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<int> Year(this ValueAssertions<DateOnly> a)
        { a.Link("Year"); return new(a.GetPipeline(), a.GetValue().Year); }

        /// <summary>Projects the month component.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<int> Month(this ValueAssertions<DateOnly> a)
        { a.Link("Month"); return new(a.GetPipeline(), a.GetValue().Month); }

        /// <summary>Projects the day component.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<int> Day(this ValueAssertions<DateOnly> a)
        { a.Link("Day"); return new(a.GetPipeline(), a.GetValue().Day); }

        /// <summary>Projects the day number component.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<int> DayNumber(this ValueAssertions<DateOnly> a)
        { a.Link("DayNumber"); return new(a.GetPipeline(), a.GetValue().DayNumber); }

        /// <summary>Projects the day-of-week component.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<DayOfWeek> DayOfWeek(this ValueAssertions<DateOnly> a)
        { a.Link("DayOfWeek"); return new(a.GetPipeline(), a.GetValue().DayOfWeek); }
    }

    /// <summary>Provides component projections for <see cref="TimeOnly"/> assertion chains.</summary>
    public static class TimeOnlyAssertExtensions
    {
        /// <summary>Projects the hour component.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<int> Hour(this ValueAssertions<TimeOnly> a)
        { a.Link("Hour"); return new(a.GetPipeline(), a.GetValue().Hour); }

        /// <summary>Projects the minute component.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<int> Minute(this ValueAssertions<TimeOnly> a)
        { a.Link("Minute"); return new(a.GetPipeline(), a.GetValue().Minute); }

        /// <summary>Projects the second component.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<int> Second(this ValueAssertions<TimeOnly> a)
        { a.Link("Second"); return new(a.GetPipeline(), a.GetValue().Second); }

        /// <summary>Projects the millisecond component.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<int> Millisecond(this ValueAssertions<TimeOnly> a)
        { a.Link("Millisecond"); return new(a.GetPipeline(), a.GetValue().Millisecond); }
    }
#endif

    /// <summary>Provides total-value projections for <see cref="TimeSpan"/> assertion chains.</summary>
    public static class TimeSpanAssertExtensions
    {
        /// <summary>Projects total seconds.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<double> TotalSeconds(this ValueAssertions<TimeSpan> a)
        { a.Link("TotalSeconds"); return new(a.GetPipeline(), a.GetValue().TotalSeconds); }

        /// <summary>Projects total minutes.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<double> TotalMinutes(this ValueAssertions<TimeSpan> a)
        { a.Link("TotalMinutes"); return new(a.GetPipeline(), a.GetValue().TotalMinutes); }

        /// <summary>Projects total hours.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<double> TotalHours(this ValueAssertions<TimeSpan> a)
        { a.Link("TotalHours"); return new(a.GetPipeline(), a.GetValue().TotalHours); }

        /// <summary>Projects total milliseconds.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<double> TotalMilliseconds(this ValueAssertions<TimeSpan> a)
        { a.Link("TotalMilliseconds"); return new(a.GetPipeline(), a.GetValue().TotalMilliseconds); }

        /// <summary>Projects total days.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<double> TotalDays(this ValueAssertions<TimeSpan> a)
        { a.Link("TotalDays"); return new(a.GetPipeline(), a.GetValue().TotalDays); }
    }
}

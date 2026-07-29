using System.Diagnostics;
using System.Runtime.CompilerServices;
using Catchy.Sdk;
using Microsoft.Playwright;

namespace Catchy
{
    public static partial class PwAsserterExtensions
    {
        public static ValueAssertions<IFrame?> That(this Asserter a, IFrame? value, __._ _ = default,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "That",
                valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<IFrame?>(p, value);
        }
    }

    public static class PwFrameAssertionsExtensions
    {
        private static SlotContainer Slots(this ValueAssertions<IFrame?> assertions) => assertions.GetPipeline().Slots;
        private static Func<float?> TimeoutMsGetter(this ValueAssertions<IFrame?> assertions) => () => assertions.Slots().TryGet(PwSlots.TimeoutMs, out float? value) ? value : null;
        private static Func<StringComparison> GetEffectiveComparison(this ValueAssertions<IFrame?> assertions) => () => assertions.GetPipeline().Settings.DefaultStringComparison;

        public static ValueAssertions<IFrame?> WithTimeout(this ValueAssertions<IFrame?> assertions, float milliseconds,
            [CallerArgumentExpression(nameof(milliseconds))] string? expr = null)
        { assertions.Slots().Set(PwSlots.TimeoutMs, milliseconds); assertions.Link("WithTimeout", expr); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IFrame?> HasUrl(this ValueAssertions<IFrame?> assertions, string url,
            [CallerArgumentExpression(nameof(url))] string? expr = null)
        { assertions.Link("HasUrl", expr); assertions.Op(a => PwFrameChecks.HasUrl(assertions.GetValue()!, url, false, assertions.GetEffectiveComparison(), assertions.TimeoutMsGetter(), assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IFrame?> DoesNotHaveUrl(this ValueAssertions<IFrame?> assertions, string url,
            [CallerArgumentExpression(nameof(url))] string? expr = null)
        { assertions.Link("DoesNotHaveUrl", expr); assertions.Op(a => PwFrameChecks.HasUrl(assertions.GetValue()!, url, true, assertions.GetEffectiveComparison(), assertions.TimeoutMsGetter(), assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IFrame?> UrlContains(this ValueAssertions<IFrame?> assertions, string substring,
            [CallerArgumentExpression(nameof(substring))] string? expr = null)
        { assertions.Link("UrlContains", expr); assertions.Op(a => PwFrameChecks.UrlContains(assertions.GetValue()!, substring, false, assertions.GetEffectiveComparison(), assertions.TimeoutMsGetter(), assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IFrame?> UrlDoesNotContain(this ValueAssertions<IFrame?> assertions, string substring,
            [CallerArgumentExpression(nameof(substring))] string? expr = null)
        { assertions.Link("UrlDoesNotContain", expr); assertions.Op(a => PwFrameChecks.UrlContains(assertions.GetValue()!, substring, true, assertions.GetEffectiveComparison(), assertions.TimeoutMsGetter(), assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IFrame?> HasName(this ValueAssertions<IFrame?> assertions, string name,
            [CallerArgumentExpression(nameof(name))] string? expr = null)
        { assertions.Link("HasName", expr); assertions.Op(a => PwFrameChecks.HasName(assertions.GetValue()!, name, false, assertions.GetEffectiveComparison(), assertions.TimeoutMsGetter(), assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IFrame?> DoesNotHaveName(this ValueAssertions<IFrame?> assertions, string name,
            [CallerArgumentExpression(nameof(name))] string? expr = null)
        { assertions.Link("DoesNotHaveName", expr); assertions.Op(a => PwFrameChecks.HasName(assertions.GetValue()!, name, true, assertions.GetEffectiveComparison(), assertions.TimeoutMsGetter(), assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IFrame?> IsDetached(this ValueAssertions<IFrame?> assertions)
        { assertions.Link("IsDetached"); assertions.Op(a => PwFrameChecks.IsDetached(assertions.GetValue()!, false, assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IFrame?> IsNotDetached(this ValueAssertions<IFrame?> assertions)
        { assertions.Link("IsNotDetached"); assertions.Op(a => PwFrameChecks.IsDetached(assertions.GetValue()!, true, assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IFrame?> HasTitle(this ValueAssertions<IFrame?> assertions, string? expected = null,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { assertions.Link("HasTitle", expr); assertions.Op(a => PwFrameChecks.HasTitle(assertions.GetValue()!, expected, false, assertions.GetEffectiveComparison(), assertions.TimeoutMsGetter(), assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IFrame?> DoesNotHaveTitle(this ValueAssertions<IFrame?> assertions, string? unexpected = null,
            [CallerArgumentExpression(nameof(unexpected))] string? expr = null)
        { assertions.Link("DoesNotHaveTitle", expr); assertions.Op(a => PwFrameChecks.HasTitle(assertions.GetValue()!, unexpected, true, assertions.GetEffectiveComparison(), assertions.TimeoutMsGetter(), assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IFrame?> ChildFrameCountIs(this ValueAssertions<IFrame?> assertions, int count,
            [CallerArgumentExpression(nameof(count))] string? expr = null)
        { assertions.Link("ChildFrameCountIs", expr); assertions.Op(a => PwFrameChecks.ChildFrameCountIs(assertions.GetValue()!, count, false, assertions.TimeoutMsGetter(), assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IFrame?> ChildFrameCountIsNot(this ValueAssertions<IFrame?> assertions, int count,
            [CallerArgumentExpression(nameof(count))] string? expr = null)
        { assertions.Link("ChildFrameCountIsNot", expr); assertions.Op(a => PwFrameChecks.ChildFrameCountIs(assertions.GetValue()!, count, true, assertions.TimeoutMsGetter(), assertions.IsSkipped())); return assertions; }
    }
}


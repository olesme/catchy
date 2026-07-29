using System.Diagnostics;
using System.Runtime.CompilerServices;
using Catchy.Sdk;
using Microsoft.Playwright;

namespace Catchy
{
    public static partial class PwAsserterExtensions
    {
        public static ValueAssertions<IBrowserContext> That(this Asserter a, IBrowserContext value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "That",
                valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<IBrowserContext>(p, value);
        }
    }

    public static class PwBrowserContextAssertionsExtensions
    {
        private static SlotContainer Slots(this ValueAssertions<IBrowserContext> assertions) => assertions.GetPipeline().Slots;
        private static Func<float?> TimeoutMsGetter(this ValueAssertions<IBrowserContext> assertions) => () => assertions.Slots().TryGet(PwSlots.TimeoutMs, out float? value) ? value : null;
        private static Func<StringComparison> GetEffectiveComparison(this ValueAssertions<IBrowserContext> assertions) => () => assertions.GetPipeline().Settings.DefaultStringComparison;

        public static ValueAssertions<IBrowserContext> WithTimeout(this ValueAssertions<IBrowserContext> assertions, float milliseconds,
            [CallerArgumentExpression(nameof(milliseconds))] string? expr = null)
        { assertions.Slots().Set(PwSlots.TimeoutMs, milliseconds); assertions.Link("WithTimeout", expr); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IBrowserContext> HasCookie(this ValueAssertions<IBrowserContext> assertions, string name, string? domain = null,
            [CallerArgumentExpression(nameof(name))] string? expr = null)
        { assertions.Link("HasCookie", expr); assertions.Op(a => PwBrowserContextChecks.HasCookie(assertions.GetValue(), name, domain, false, assertions.GetEffectiveComparison(), assertions.TimeoutMsGetter(), assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IBrowserContext> DoesNotHaveCookie(this ValueAssertions<IBrowserContext> assertions, string name, string? domain = null,
            [CallerArgumentExpression(nameof(name))] string? expr = null)
        { assertions.Link("DoesNotHaveCookie", expr); assertions.Op(a => PwBrowserContextChecks.HasCookie(assertions.GetValue(), name, domain, true, assertions.GetEffectiveComparison(), assertions.TimeoutMsGetter(), assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IBrowserContext> HasCookieValue(this ValueAssertions<IBrowserContext> assertions, string name, string expected,
            [CallerArgumentExpression(nameof(name))] string? nameExpr = null,
            [CallerArgumentExpression(nameof(expected))] string? valExpr = null)
        { assertions.Link("HasCookieValue", nameExpr, valExpr); assertions.Op(a => PwBrowserContextChecks.HasCookieValue(assertions.GetValue(), name, expected, false, assertions.GetEffectiveComparison(), assertions.TimeoutMsGetter(), assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IBrowserContext> DoesNotHaveCookieValue(this ValueAssertions<IBrowserContext> assertions, string name, string unexpected,
            [CallerArgumentExpression(nameof(name))] string? nameExpr = null,
            [CallerArgumentExpression(nameof(unexpected))] string? valExpr = null)
        { assertions.Link("DoesNotHaveCookieValue", nameExpr, valExpr); assertions.Op(a => PwBrowserContextChecks.HasCookieValue(assertions.GetValue(), name, unexpected, true, assertions.GetEffectiveComparison(), assertions.TimeoutMsGetter(), assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IBrowserContext> HasCookies(this ValueAssertions<IBrowserContext> assertions)
        { assertions.Link("HasCookies"); assertions.Op(a => PwBrowserContextChecks.HasCookies(assertions.GetValue(), false, assertions.TimeoutMsGetter(), assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IBrowserContext> HasNoCookies(this ValueAssertions<IBrowserContext> assertions)
        { assertions.Link("HasNoCookies"); assertions.Op(a => PwBrowserContextChecks.HasCookies(assertions.GetValue(), true, assertions.TimeoutMsGetter(), assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IBrowserContext> PageCountIs(this ValueAssertions<IBrowserContext> assertions, int count,
            [CallerArgumentExpression(nameof(count))] string? expr = null)
        { assertions.Link("PageCountIs", expr); assertions.Op(a => PwBrowserContextChecks.PageCountIs(assertions.GetValue(), count, false, assertions.TimeoutMsGetter(), assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IBrowserContext> PageCountIsNot(this ValueAssertions<IBrowserContext> assertions, int count,
            [CallerArgumentExpression(nameof(count))] string? expr = null)
        { assertions.Link("PageCountIsNot", expr); assertions.Op(a => PwBrowserContextChecks.PageCountIs(assertions.GetValue(), count, true, assertions.TimeoutMsGetter(), assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IBrowserContext> HasNoOpenPages(this ValueAssertions<IBrowserContext> assertions)
        { assertions.Link("HasNoOpenPages"); assertions.Op(a => PwBrowserContextChecks.PageCountIs(assertions.GetValue(), 0, false, assertions.TimeoutMsGetter(), assertions.IsSkipped())); return assertions; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IBrowserContext> HasOpenPages(this ValueAssertions<IBrowserContext> assertions)
        { assertions.Link("HasOpenPages"); assertions.Op(a => PwBrowserContextChecks.HasOpenPages(assertions.GetValue(), false, assertions.TimeoutMsGetter(), assertions.IsSkipped())); return assertions; }
    }
}


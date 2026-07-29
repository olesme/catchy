using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Catchy.Sdk;
using Microsoft.Playwright;

namespace Catchy
{
    public static partial class PwAsserterExtensions
    {
        /// <summary>Starts assertions for a Playwright <see cref="IPage"/> value.</summary>
        public static ValueAssertions<IPage> That(this Asserter a, IPage value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "That",
                valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<IPage>(p, value);
        }
    }

    public static class PwPageAssertionsExtensions
    {
        private static SlotContainer Slots(this ValueAssertions<IPage> a) => a.GetPipeline().Slots;
        private static Func<float?> TimeoutMsGetter(this ValueAssertions<IPage> a) => () => a.Slots().TryGet(PwSlots.TimeoutMs, out float? value) ? value : null;
        private static Func<StringComparison> GetEffectiveComparison(this ValueAssertions<IPage> a) => () => a.GetPipeline().Settings.DefaultStringComparison;
        private static IPage GetPage(this ValueAssertions<IPage> a) => a.GetValue();

        /// <summary>Sets timeout in milliseconds for subsequent page assertions.</summary>
        public static ValueAssertions<IPage> WithTimeout(this ValueAssertions<IPage> assertions, float milliseconds,
            [CallerArgumentExpression(nameof(milliseconds))] string? expr = null)
        {
            assertions.Slots().Set(PwSlots.TimeoutMs, milliseconds);
            assertions.Link("WithTimeout", expr);
            return assertions;
        }

        /// <summary>Projects the current page title.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> Title(this ValueAssertions<IPage> assertions)
        {
            assertions.Link("Title");
            return new ValueAssertions<string?>(assertions.GetPipeline(), assertions.GetPage().TitleAsync().GetAwaiter().GetResult());
        }

        /// <summary>Asserts that viewport size equals <paramref name="width"/> x <paramref name="height"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> HasViewportSize(this ValueAssertions<IPage> assertions, int width, int height,
            [CallerArgumentExpression(nameof(width))] string? wExpr = null,
            [CallerArgumentExpression(nameof(height))] string? hExpr = null)
        {
            assertions.Link("HasViewportSize", wExpr, hExpr);
            assertions.Op(a => PwPageChecks.HasViewportSize(assertions.GetPage(), width, height, false, assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that page accessibility tree matches <paramref name="template"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> MatchesAriaSnapshot(this ValueAssertions<IPage> assertions, string template,
            [CallerArgumentExpression(nameof(template))] string? expr = null)
        {
            assertions.Link("MatchesAriaSnapshot", expr);
            assertions.Op(a => PwPageChecks.MatchesAriaSnapshot(assertions.GetPage(), template, false, assertions.TimeoutMsGetter(), assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that page accessibility tree does not match <paramref name="template"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> DoesNotMatchAriaSnapshot(this ValueAssertions<IPage> assertions, string template,
            [CallerArgumentExpression(nameof(template))] string? expr = null)
        {
            assertions.Link("DoesNotMatchAriaSnapshot", expr);
            assertions.Op(a => PwPageChecks.MatchesAriaSnapshot(assertions.GetPage(), template, true, assertions.TimeoutMsGetter(), assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that viewport size is not <paramref name="width"/> x <paramref name="height"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> DoesNotHaveViewportSize(this ValueAssertions<IPage> assertions, int width, int height,
            [CallerArgumentExpression(nameof(width))] string? wExpr = null,
            [CallerArgumentExpression(nameof(height))] string? hExpr = null)
        {
            assertions.Link("DoesNotHaveViewportSize", wExpr, hExpr);
            assertions.Op(a => PwPageChecks.HasViewportSize(assertions.GetPage(), width, height, true, assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that local storage contains key <paramref name="key"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> HasLocalStorageKey(this ValueAssertions<IPage> assertions, string key,
            [CallerArgumentExpression(nameof(key))] string? expr = null)
        {
            assertions.Link("HasLocalStorageKey", expr);
            assertions.Op(a => PwPageChecks.HasLocalStorageKey(assertions.GetPage(), key, false, assertions.TimeoutMsGetter(), assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that local storage does not contain key <paramref name="key"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> DoesNotHaveLocalStorageKey(this ValueAssertions<IPage> assertions, string key,
            [CallerArgumentExpression(nameof(key))] string? expr = null)
        {
            assertions.Link("DoesNotHaveLocalStorageKey", expr);
            assertions.Op(a => PwPageChecks.HasLocalStorageKey(assertions.GetPage(), key, true, assertions.TimeoutMsGetter(), assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that local storage key <paramref name="key"/> equals <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> HasLocalStorageValue(this ValueAssertions<IPage> assertions, string key, string expected,
            [CallerArgumentExpression(nameof(key))] string? keyExpr = null,
            [CallerArgumentExpression(nameof(expected))] string? valExpr = null)
        {
            assertions.Link("HasLocalStorageValue", keyExpr, valExpr);
            assertions.Op(a => PwPageChecks.HasLocalStorageValue(assertions.GetPage(), key, expected, false, assertions.GetEffectiveComparison(), assertions.TimeoutMsGetter(), assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that local storage key <paramref name="key"/> does not equal <paramref name="unexpected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> DoesNotHaveLocalStorageValue(this ValueAssertions<IPage> assertions, string key, string unexpected,
            [CallerArgumentExpression(nameof(key))] string? keyExpr = null,
            [CallerArgumentExpression(nameof(unexpected))] string? valExpr = null)
        {
            assertions.Link("DoesNotHaveLocalStorageValue", keyExpr, valExpr);
            assertions.Op(a => PwPageChecks.HasLocalStorageValue(assertions.GetPage(), key, unexpected, true, assertions.GetEffectiveComparison(), assertions.TimeoutMsGetter(), assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that local storage is empty.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> LocalStorageIsEmpty(this ValueAssertions<IPage> assertions)
        {
            assertions.Link("LocalStorageIsEmpty");
            assertions.Op(a => PwPageChecks.LocalStorageIsEmpty(assertions.GetPage(), false, assertions.TimeoutMsGetter(), assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that local storage is not empty.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> LocalStorageIsNotEmpty(this ValueAssertions<IPage> assertions)
        {
            assertions.Link("LocalStorageIsNotEmpty");
            assertions.Op(a => PwPageChecks.LocalStorageIsEmpty(assertions.GetPage(), true, assertions.TimeoutMsGetter(), assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that session storage contains key <paramref name="key"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> HasSessionStorageKey(this ValueAssertions<IPage> assertions, string key,
            [CallerArgumentExpression(nameof(key))] string? expr = null)
        {
            assertions.Link("HasSessionStorageKey", expr);
            assertions.Op(a => PwPageChecks.HasSessionStorageKey(assertions.GetPage(), key, false, assertions.TimeoutMsGetter(), assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that session storage does not contain key <paramref name="key"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> DoesNotHaveSessionStorageKey(this ValueAssertions<IPage> assertions, string key,
            [CallerArgumentExpression(nameof(key))] string? expr = null)
        {
            assertions.Link("DoesNotHaveSessionStorageKey", expr);
            assertions.Op(a => PwPageChecks.HasSessionStorageKey(assertions.GetPage(), key, true, assertions.TimeoutMsGetter(), assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that session storage key <paramref name="key"/> equals <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> HasSessionStorageValue(this ValueAssertions<IPage> assertions, string key, string expected,
            [CallerArgumentExpression(nameof(key))] string? keyExpr = null,
            [CallerArgumentExpression(nameof(expected))] string? valExpr = null)
        {
            assertions.Link("HasSessionStorageValue", keyExpr, valExpr);
            assertions.Op(a => PwPageChecks.HasSessionStorageValue(assertions.GetPage(), key, expected, false, assertions.GetEffectiveComparison(), assertions.TimeoutMsGetter(), assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that session storage key <paramref name="key"/> does not equal <paramref name="unexpected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> DoesNotHaveSessionStorageValue(this ValueAssertions<IPage> assertions, string key, string unexpected,
            [CallerArgumentExpression(nameof(key))] string? keyExpr = null,
            [CallerArgumentExpression(nameof(unexpected))] string? valExpr = null)
        {
            assertions.Link("DoesNotHaveSessionStorageValue", keyExpr, valExpr);
            assertions.Op(a => PwPageChecks.HasSessionStorageValue(assertions.GetPage(), key, unexpected, true, assertions.GetEffectiveComparison(), assertions.TimeoutMsGetter(), assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that the page has a cookie named <paramref name="name"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> HasCookie(this ValueAssertions<IPage> assertions, string name, string? domain = null,
            [CallerArgumentExpression(nameof(name))] string? expr = null)
        {
            assertions.Link("HasCookie", expr);
            assertions.Op(a => PwPageChecks.HasCookie(assertions.GetPage(), name, domain, false, assertions.GetEffectiveComparison(), assertions.TimeoutMsGetter(), assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that the page does not have a cookie named <paramref name="name"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> DoesNotHaveCookie(this ValueAssertions<IPage> assertions, string name, string? domain = null,
            [CallerArgumentExpression(nameof(name))] string? expr = null)
        {
            assertions.Link("DoesNotHaveCookie", expr);
            assertions.Op(a => PwPageChecks.HasCookie(assertions.GetPage(), name, domain, true, assertions.GetEffectiveComparison(), assertions.TimeoutMsGetter(), assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that cookie <paramref name="name"/> equals <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> HasCookieValue(this ValueAssertions<IPage> assertions, string name, string expected,
            [CallerArgumentExpression(nameof(name))] string? nameExpr = null,
            [CallerArgumentExpression(nameof(expected))] string? valExpr = null)
        {
            assertions.Link("HasCookieValue", nameExpr, valExpr);
            assertions.Op(a => PwPageChecks.HasCookieValue(assertions.GetPage(), name, expected, false, assertions.GetEffectiveComparison(), assertions.TimeoutMsGetter(), assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that cookie <paramref name="name"/> does not equal <paramref name="unexpected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> DoesNotHaveCookieValue(this ValueAssertions<IPage> assertions, string name, string unexpected,
            [CallerArgumentExpression(nameof(name))] string? nameExpr = null,
            [CallerArgumentExpression(nameof(unexpected))] string? valExpr = null)
        {
            assertions.Link("DoesNotHaveCookieValue", nameExpr, valExpr);
            assertions.Op(a => PwPageChecks.HasCookieValue(assertions.GetPage(), name, unexpected, true, assertions.GetEffectiveComparison(), assertions.TimeoutMsGetter(), assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that the page has a frame named <paramref name="name"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> HasFrame(this ValueAssertions<IPage> assertions, string name,
            [CallerArgumentExpression(nameof(name))] string? expr = null)
        {
            assertions.Link("HasFrame", expr);
            assertions.Op(a => PwPageChecks.HasFrame(assertions.GetPage(), name, false, assertions.TimeoutMsGetter(), assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that the page does not have a frame named <paramref name="name"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> DoesNotHaveFrame(this ValueAssertions<IPage> assertions, string name,
            [CallerArgumentExpression(nameof(name))] string? expr = null)
        {
            assertions.Link("DoesNotHaveFrame", expr);
            assertions.Op(a => PwPageChecks.HasFrame(assertions.GetPage(), name, true, assertions.TimeoutMsGetter(), assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that the page has meta tag <paramref name="name"/> with content <paramref name="content"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> HasMetaTag(this ValueAssertions<IPage> assertions, string name, string content,
            [CallerArgumentExpression(nameof(name))] string? nameExpr = null,
            [CallerArgumentExpression(nameof(content))] string? contentExpr = null)
        {
            assertions.Link("HasMetaTag", nameExpr, contentExpr);
            assertions.Op(a => PwPageChecks.HasMetaTag(assertions.GetPage(), name, content, false, assertions.GetEffectiveComparison(), assertions.TimeoutMsGetter(), assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that the page does not have meta tag <paramref name="name"/> with content <paramref name="content"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> DoesNotHaveMetaTag(this ValueAssertions<IPage> assertions, string name, string content,
            [CallerArgumentExpression(nameof(name))] string? nameExpr = null,
            [CallerArgumentExpression(nameof(content))] string? contentExpr = null)
        {
            assertions.Link("DoesNotHaveMetaTag", nameExpr, contentExpr);
            assertions.Op(a => PwPageChecks.HasMetaTag(assertions.GetPage(), name, content, true, assertions.GetEffectiveComparison(), assertions.TimeoutMsGetter(), assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that JSON from <paramref name="jsExpression"/> contains path <paramref name="path"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> HasJsonPath(this ValueAssertions<IPage> assertions, string jsExpression, string path,
            [CallerArgumentExpression(nameof(jsExpression))] string? jsExpr = null,
            [CallerArgumentExpression(nameof(path))] string? pathExpr = null)
        {
            assertions.Link("HasJsonPath", jsExpr, pathExpr);
            assertions.Op(a => PwPageChecks.HasJsonPath(assertions.GetPage(), jsExpression, path, false, assertions.TimeoutMsGetter(), assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that JSON from <paramref name="jsExpression"/> does not contain path <paramref name="path"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> DoesNotHaveJsonPath(this ValueAssertions<IPage> assertions, string jsExpression, string path,
            [CallerArgumentExpression(nameof(jsExpression))] string? jsExpr = null,
            [CallerArgumentExpression(nameof(path))] string? pathExpr = null)
        {
            assertions.Link("DoesNotHaveJsonPath", jsExpr, pathExpr);
            assertions.Op(a => PwPageChecks.HasJsonPath(assertions.GetPage(), jsExpression, path, true, assertions.TimeoutMsGetter(), assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that page load time is less than <paramref name="ms"/> milliseconds.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> LoadTimeLessThan(this ValueAssertions<IPage> assertions, double ms,
            [CallerArgumentExpression(nameof(ms))] string? expr = null)
        {
            assertions.Link("LoadTimeLessThan", expr);
            assertions.Op(a => PwPerformanceChecks.LoadTimeLessThan(assertions.GetPage(), ms, false, assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that page load time is not less than <paramref name="ms"/> milliseconds.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> LoadTimeNotLessThan(this ValueAssertions<IPage> assertions, double ms,
            [CallerArgumentExpression(nameof(ms))] string? expr = null)
        {
            assertions.Link("LoadTimeNotLessThan", expr);
            assertions.Op(a => PwPerformanceChecks.LoadTimeLessThan(assertions.GetPage(), ms, true, assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that DOMContentLoaded time is less than <paramref name="ms"/> milliseconds.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> DomContentLoadedLessThan(this ValueAssertions<IPage> assertions, double ms,
            [CallerArgumentExpression(nameof(ms))] string? expr = null)
        {
            assertions.Link("DomContentLoadedLessThan", expr);
            assertions.Op(a => PwPerformanceChecks.DomContentLoadedLessThan(assertions.GetPage(), ms, false, assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that DOMContentLoaded time is not less than <paramref name="ms"/> milliseconds.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> DomContentLoadedNotLessThan(this ValueAssertions<IPage> assertions, double ms,
            [CallerArgumentExpression(nameof(ms))] string? expr = null)
        {
            assertions.Link("DomContentLoadedNotLessThan", expr);
            assertions.Op(a => PwPerformanceChecks.DomContentLoadedLessThan(assertions.GetPage(), ms, true, assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that time to first byte is less than <paramref name="ms"/> milliseconds.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> TimeToFirstByteLessThan(this ValueAssertions<IPage> assertions, double ms,
            [CallerArgumentExpression(nameof(ms))] string? expr = null)
        {
            assertions.Link("TimeToFirstByteLessThan", expr);
            assertions.Op(a => PwPerformanceChecks.TimeToFirstByteLessThan(assertions.GetPage(), ms, false, assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that largest contentful paint is less than <paramref name="ms"/> milliseconds.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> LargestContentfulPaintLessThan(this ValueAssertions<IPage> assertions, double ms,
            [CallerArgumentExpression(nameof(ms))] string? expr = null)
        {
            assertions.Link("LargestContentfulPaintLessThan", expr);
            assertions.Op(a => PwPerformanceChecks.LargestContentfulPaintLessThan(assertions.GetPage(), ms, false, assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that first contentful paint is less than <paramref name="ms"/> milliseconds.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> FirstContentfulPaintLessThan(this ValueAssertions<IPage> assertions, double ms,
            [CallerArgumentExpression(nameof(ms))] string? expr = null)
        {
            assertions.Link("FirstContentfulPaintLessThan", expr);
            assertions.Op(a => PwPerformanceChecks.FirstContentfulPaintLessThan(assertions.GetPage(), ms, false, assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that cumulative layout shift is below <paramref name="threshold"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> CumulativeLayoutShiftBelow(this ValueAssertions<IPage> assertions, double threshold,
            [CallerArgumentExpression(nameof(threshold))] string? expr = null)
        {
            assertions.Link("CumulativeLayoutShiftBelow", expr);
            assertions.Op(a => PwPerformanceChecks.CumulativeLayoutShiftBelow(assertions.GetPage(), threshold, false, assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that total blocking time is less than <paramref name="ms"/> milliseconds.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> TotalBlockingTimeLessThan(this ValueAssertions<IPage> assertions, double ms,
            [CallerArgumentExpression(nameof(ms))] string? expr = null)
        {
            assertions.Link("TotalBlockingTimeLessThan", expr);
            assertions.Op(a => PwPerformanceChecks.TotalBlockingTimeLessThan(assertions.GetPage(), ms, false, assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that resource count for <paramref name="resourceType"/> is less than <paramref name="count"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> ResourceCountLessThan(this ValueAssertions<IPage> assertions, string resourceType, int count,
            [CallerArgumentExpression(nameof(resourceType))] string? typeExpr = null,
            [CallerArgumentExpression(nameof(count))] string? countExpr = null)
        {
            assertions.Link("ResourceCountLessThan", typeExpr, countExpr);
            assertions.Op(a => PwPerformanceChecks.ResourceCountLessThan(assertions.GetPage(), resourceType, count, false, assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that resource count for <paramref name="resourceType"/> is not less than <paramref name="count"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> ResourceCountNotLessThan(this ValueAssertions<IPage> assertions, string resourceType, int count,
            [CallerArgumentExpression(nameof(resourceType))] string? typeExpr = null,
            [CallerArgumentExpression(nameof(count))] string? countExpr = null)
        {
            assertions.Link("ResourceCountNotLessThan", typeExpr, countExpr);
            assertions.Op(a => PwPerformanceChecks.ResourceCountLessThan(assertions.GetPage(), resourceType, count, true, assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Runs lightweight accessibility checks on the current page.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> AccessibilityLightweight(this ValueAssertions<IPage> assertions)
        {
            assertions.Link("AccessibilityLightweight");
            assertions.Op(a => PwAccessibilityChecks.Lightweight(assertions.GetPage(), assertions.IsSkipped(), assertions.TimeoutMsGetter()));
            return assertions;
        }

        /// <summary>Runs Axe accessibility checks using <paramref name="axeScriptContent"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> AccessibilityWithAxe(this ValueAssertions<IPage> assertions, string axeScriptContent)
        {
            assertions.Link("AccessibilityWithAxe");
            assertions.Op(a => PwAccessibilityChecks.Axe(assertions.GetPage(), axeScriptContent, assertions.IsSkipped(), assertions.TimeoutMsGetter()));
            return assertions;
        }
    }
}



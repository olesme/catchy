using System;
using System.Diagnostics;
using Catchy.Sdk;
using Microsoft.Playwright;

namespace Catchy
{
    public static class PwStringComparisonModifierExtensions
    {
        private static TValueAssertions Configure<TValueAssertions>(
            TValueAssertions assertions,
            string link,
            Func<StringComparison, StringComparison> transform)
            where TValueAssertions : ValueAssertions
        {
            var p = assertions.GetPipeline();
            p.Settings = p.Settings.Clone(s => s.DefaultStringComparison = transform(s.DefaultStringComparison));
            assertions.Link(link);
            return assertions;
        }

        /// <summary>Uses ordinal string comparison for locator assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ILocator> UsingOrdinal(this ValueAssertions<ILocator> a) => Configure(a, "UsingOrdinal", _ => StringComparison.Ordinal);
        /// <summary>Uses ordinal string comparison for page assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> UsingOrdinal(this ValueAssertions<IPage> a) => Configure(a, "UsingOrdinal", _ => StringComparison.Ordinal);
        /// <summary>Uses ordinal string comparison for frame assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IFrame?> UsingOrdinal(this ValueAssertions<IFrame?> a) => Configure(a, "UsingOrdinal", _ => StringComparison.Ordinal);
        /// <summary>Uses ordinal string comparison for API response assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> UsingOrdinal(this ValueAssertions<IAPIResponse> a) => Configure(a, "UsingOrdinal", _ => StringComparison.Ordinal);
        /// <summary>Uses ordinal string comparison for browser context assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IBrowserContext> UsingOrdinal(this ValueAssertions<IBrowserContext> a) => Configure(a, "UsingOrdinal", _ => StringComparison.Ordinal);
        /// <summary>Uses ordinal string comparison for download assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IDownload> UsingOrdinal(this ValueAssertions<IDownload> a) => Configure(a, "UsingOrdinal", _ => StringComparison.Ordinal);

        /// <summary>Uses current-culture string comparison for locator assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ILocator> UsingCurrentCulture(this ValueAssertions<ILocator> a) => Configure(a, "UsingCurrentCulture", _ => StringComparison.CurrentCulture);
        /// <summary>Uses current-culture string comparison for page assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> UsingCurrentCulture(this ValueAssertions<IPage> a) => Configure(a, "UsingCurrentCulture", _ => StringComparison.CurrentCulture);
        /// <summary>Uses current-culture string comparison for frame assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IFrame?> UsingCurrentCulture(this ValueAssertions<IFrame?> a) => Configure(a, "UsingCurrentCulture", _ => StringComparison.CurrentCulture);
        /// <summary>Uses current-culture string comparison for API response assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> UsingCurrentCulture(this ValueAssertions<IAPIResponse> a) => Configure(a, "UsingCurrentCulture", _ => StringComparison.CurrentCulture);
        /// <summary>Uses current-culture string comparison for browser context assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IBrowserContext> UsingCurrentCulture(this ValueAssertions<IBrowserContext> a) => Configure(a, "UsingCurrentCulture", _ => StringComparison.CurrentCulture);
        /// <summary>Uses current-culture string comparison for download assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IDownload> UsingCurrentCulture(this ValueAssertions<IDownload> a) => Configure(a, "UsingCurrentCulture", _ => StringComparison.CurrentCulture);

        /// <summary>Uses invariant-culture string comparison for locator assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ILocator> UsingInvariantCulture(this ValueAssertions<ILocator> a) => Configure(a, "UsingInvariantCulture", _ => StringComparison.InvariantCulture);
        /// <summary>Uses invariant-culture string comparison for page assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> UsingInvariantCulture(this ValueAssertions<IPage> a) => Configure(a, "UsingInvariantCulture", _ => StringComparison.InvariantCulture);
        /// <summary>Uses invariant-culture string comparison for frame assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IFrame?> UsingInvariantCulture(this ValueAssertions<IFrame?> a) => Configure(a, "UsingInvariantCulture", _ => StringComparison.InvariantCulture);
        /// <summary>Uses invariant-culture string comparison for API response assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> UsingInvariantCulture(this ValueAssertions<IAPIResponse> a) => Configure(a, "UsingInvariantCulture", _ => StringComparison.InvariantCulture);
        /// <summary>Uses invariant-culture string comparison for browser context assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IBrowserContext> UsingInvariantCulture(this ValueAssertions<IBrowserContext> a) => Configure(a, "UsingInvariantCulture", _ => StringComparison.InvariantCulture);
        /// <summary>Uses invariant-culture string comparison for download assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IDownload> UsingInvariantCulture(this ValueAssertions<IDownload> a) => Configure(a, "UsingInvariantCulture", _ => StringComparison.InvariantCulture);

        /// <summary>Configures case-insensitive comparison for locator assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ILocator> IgnoringCase(this ValueAssertions<ILocator> a) => Configure(a, "IgnoringCase", c => c.AddIgnoreCase());
        /// <summary>Configures case-insensitive comparison for page assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> IgnoringCase(this ValueAssertions<IPage> a) => Configure(a, "IgnoringCase", c => c.AddIgnoreCase());
        /// <summary>Configures case-insensitive comparison for frame assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IFrame?> IgnoringCase(this ValueAssertions<IFrame?> a) => Configure(a, "IgnoringCase", c => c.AddIgnoreCase());
        /// <summary>Configures case-insensitive comparison for API response assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> IgnoringCase(this ValueAssertions<IAPIResponse> a) => Configure(a, "IgnoringCase", c => c.AddIgnoreCase());
        /// <summary>Configures case-insensitive comparison for browser context assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IBrowserContext> IgnoringCase(this ValueAssertions<IBrowserContext> a) => Configure(a, "IgnoringCase", c => c.AddIgnoreCase());
        /// <summary>Configures case-insensitive comparison for download assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IDownload> IgnoringCase(this ValueAssertions<IDownload> a) => Configure(a, "IgnoringCase", c => c.AddIgnoreCase());

        /// <summary>Configures case-sensitive comparison for locator assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ILocator> RespectingCase(this ValueAssertions<ILocator> a) => Configure(a, "RespectingCase", c => c.RemoveIgnoreCase());
        /// <summary>Configures case-sensitive comparison for page assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> RespectingCase(this ValueAssertions<IPage> a) => Configure(a, "RespectingCase", c => c.RemoveIgnoreCase());
        /// <summary>Configures case-sensitive comparison for frame assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IFrame?> RespectingCase(this ValueAssertions<IFrame?> a) => Configure(a, "RespectingCase", c => c.RemoveIgnoreCase());
        /// <summary>Configures case-sensitive comparison for API response assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IAPIResponse> RespectingCase(this ValueAssertions<IAPIResponse> a) => Configure(a, "RespectingCase", c => c.RemoveIgnoreCase());
        /// <summary>Configures case-sensitive comparison for browser context assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IBrowserContext> RespectingCase(this ValueAssertions<IBrowserContext> a) => Configure(a, "RespectingCase", c => c.RemoveIgnoreCase());
        /// <summary>Configures case-sensitive comparison for download assertions.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IDownload> RespectingCase(this ValueAssertions<IDownload> a) => Configure(a, "RespectingCase", c => c.RemoveIgnoreCase());
    }
}



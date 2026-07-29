using System.Diagnostics;
using System.Text.Json;
using Catchy.Sdk;

namespace Catchy
{
    /// <summary>Provides string comparison mode modifiers for assertion chains.</summary>
    [GenerateTypedOverloads(typeof(JsonDocument), TemplateType = typeof(string))]
    public static partial class StringComparisonModifierTemplates
    {
        /// <summary>Uses ordinal string comparison for subsequent string assertions in the chain.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> UsingOrdinal(this ValueAssertions<string?> a)
        {
            var p = a.GetPipeline();
            p.Settings = p.Settings.Clone(s => s.DefaultStringComparison = StringComparison.Ordinal);
            a.Link("UsingOrdinal");
            return a;
        }

        /// <summary>Uses current-culture string comparison for subsequent string assertions in the chain.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> UsingCurrentCulture(this ValueAssertions<string?> a)
        {
            var p = a.GetPipeline();
            p.Settings = p.Settings.Clone(s => s.DefaultStringComparison = StringComparison.CurrentCulture);
            a.Link("UsingCurrentCulture");
            return a;
        }

        /// <summary>Uses invariant-culture string comparison for subsequent string assertions in the chain.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> UsingInvariantCulture(this ValueAssertions<string?> a)
        {
            var p = a.GetPipeline();
            p.Settings = p.Settings.Clone(s => s.DefaultStringComparison = StringComparison.InvariantCulture);
            a.Link("UsingInvariantCulture");
            return a;
        }

        /// <summary>Enables case-insensitive comparison for subsequent string assertions in the chain.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> IgnoringCase(this ValueAssertions<string?> a)
        {
            var p = a.GetPipeline();
            p.Settings = p.Settings.Clone(s => s.DefaultStringComparison = s.DefaultStringComparison.AddIgnoreCase());
            a.Link("IgnoringCase");
            return a;
        }

        /// <summary>Enables case-sensitive comparison for subsequent string assertions in the chain.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> RespectingCase(this ValueAssertions<string?> a)
        {
            var p = a.GetPipeline();
            p.Settings = p.Settings.Clone(s => s.DefaultStringComparison = s.DefaultStringComparison.RemoveIgnoreCase());
            a.Link("RespectingCase");
            return a;
        }
    }
}

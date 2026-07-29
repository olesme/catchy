using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Catchy.Sdk;

namespace Catchy
{
    /// <summary>Entry points for JSON document assertions.</summary>
    public static partial class AsserterExtensions
    {
        /// <summary>Starts assertions for a <see cref="JsonDocument"/> value.</summary>
        public static ValueAssertions<JsonDocument?> That(this Asserter a, JsonDocument? value, __._ _ = default,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(
                        asserterExpr: aExpr,
                        methodName: "That",
                        valueExpr: vExpr,
                        file: file, line: line, member: member);
            return new ValueAssertions<JsonDocument?>(p, value);
        }
    }

    /// <summary>Provides fluent assertions and projections for <see cref="JsonDocument"/> values.</summary>
    public static partial class JsonDocumentAssertionsExtensions
    {
        /// <summary>Asserts JSON textual equality against <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<JsonDocument?> Is(this ValueAssertions<JsonDocument?> a, string? expected,
            StringComparison? comparison = null,
            [CallerArgumentExpression(nameof(expected))] string? expr = null,
            [CallerArgumentExpression(nameof(comparison))] string? comparisonExpr = null)
        {
            a.Link("Is", expr, comparisonExpr);
            a.Op(a => JsonDocumentChecks.Is(a.GetValue(), expected, () => comparison ?? a.GetPipeline().Settings.DefaultStringComparison, a.IsSkipped(), expr));
            return a;
        }

        /// <summary>Asserts deep JSON equivalence to <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<JsonDocument?> IsEquivalentTo(this ValueAssertions<JsonDocument?> a, object? expected,
            EqualsOptions? opts = null, DeepEqualRuleContainer? localRules = null,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        {
            a.Link("IsEquivalentTo", expr);
            a.Op(a => JsonDocumentChecks.IsEquivalentTo(
                a.GetValue(),
                expected,
                opts ?? a.GetPipeline().Settings.EqualsOptions,
                localRules ?? a.GetPipeline().GetEffectiveDeepEqualRuleContainer(),
                expr,
                a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that JSON is not deeply equivalent to <paramref name="unexpected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<JsonDocument?> IsNotEquivalentTo(this ValueAssertions<JsonDocument?> a, object? unexpected,
            EqualsOptions? opts = null, DeepEqualRuleContainer? localRules = null,
            [CallerArgumentExpression(nameof(unexpected))] string? expr = null)
        {
            a.Link("IsNotEquivalentTo", expr);
            a.Op(a => JsonDocumentChecks.IsNotEquivalentTo(
                a.GetValue(),
                unexpected,
                opts ?? a.GetPipeline().Settings.EqualsOptions,
                localRules ?? a.GetPipeline().GetEffectiveDeepEqualRuleContainer(),
                expr,
                a.IsSkipped()));
            return a;
        }

        /// <summary>Projects the root element of the JSON document.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<JsonElement?> RootElement(this ValueAssertions<JsonDocument?> a)
        {
            a.Link("RootElement");
            var doc = a.GetValue();
            return new ValueAssertions<JsonElement?>(a.GetPipeline(), doc is null ? (JsonElement?)null : doc.RootElement.Clone());
        }

    }
}


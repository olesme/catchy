using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Catchy.Sdk;

namespace Catchy
{
    /// <summary>Entry points for JSON value-kind assertions.</summary>
    public static partial class AsserterExtensions
    {
        /// <summary>Starts assertions for a <see cref="JsonValueKind"/> value.</summary>
        public static ValueAssertions<JsonValueKind?> That(this Asserter a, JsonValueKind? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<JsonValueKind?>(p, value);
        }
    }

    /// <summary>Provides fluent assertions for <see cref="JsonValueKind"/> values.</summary>
    public static class JsonValueKindAssertExtensions
    {
        /// <summary>Asserts that the value kind equals <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<JsonValueKind?> Is(this ValueAssertions<JsonValueKind?> a, JsonValueKind expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("Is", expr); a.Op(a => CheckOperation.Sync(() => a.GetValue() == expected, () => $"Expected JsonValueKind to be {expected}", a.IsSkipped())); return a; }

        /// <summary>Asserts that the value kind does not equal <paramref name="unexpected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<JsonValueKind?> IsNot(this ValueAssertions<JsonValueKind?> a, JsonValueKind unexpected,
            [CallerArgumentExpression(nameof(unexpected))] string? expr = null)
        { a.Link("IsNot", expr); a.Op(a => CheckOperation.Sync(() => a.GetValue() != unexpected, () => $"Expected JsonValueKind not to be {unexpected}", a.IsSkipped())); return a; }
    }
}



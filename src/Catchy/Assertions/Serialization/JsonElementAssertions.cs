using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Catchy.Sdk;

namespace Catchy
{
    /// <summary>Entry points for JSON element assertions.</summary>
    public static partial class AsserterExtensions
    {
        /// <summary>Starts assertions for a <see cref="JsonElement"/> value.</summary>
        public static ValueAssertions<JsonElement?> That(this Asserter a, JsonElement? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<JsonElement?>(p, value);
        }
    }

    /// <summary>Provides projections for <see cref="JsonElement"/> assertion chains.</summary>
    public static class JsonElementAssertExtensions
    {
        /// <summary>Projects the JSON value kind.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<JsonValueKind?> ValueKind(this ValueAssertions<JsonElement?> a)
        {
            a.Link("ValueKind");
            return new ValueAssertions<JsonValueKind?>(a.GetPipeline(), a.GetValue()?.ValueKind);
        }
    }
}



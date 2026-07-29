using System.Diagnostics;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    public static partial class AsserterExtensions
    {
        /// <summary>Starts a fluent assertion chain from a stateful asserter instance.</summary>
        public static ValueAssertions<StatefulAsserter> That(this StatefulAsserter a,
            __._ _ = default,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(
                asserterExpr: aExpr, methodName: "That",
                file: file, line: line, member: member);
            return new global::Catchy.ValueAssertions<StatefulAsserter>(p, a);
        }
    }

    /// <summary>Provides assertion-chain projections for <see cref="StatefulAsserter"/> values.</summary>
    public static class StatefulAsserterExtensions
    {
        /// <summary>Projects the current assertion chain to the underlying <see cref="SoftState"/>.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static global::Catchy.ValueAssertions<SoftState> SoftState(this global::Catchy.ValueAssertions<StatefulAsserter> a)
        {
            a.Link("SoftState");
            return new global::Catchy.ValueAssertions<SoftState>(a.GetPipeline(), a.GetValue().Soft.SoftState);
        }
    }
}

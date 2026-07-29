using System.Diagnostics;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    public sealed class FailAssertion : ValueAssertions<FailAssertion, object?>
    {
        internal FailAssertion(AssertionPipeline pipeline) : base(pipeline, (object?)null) { }
    }

    public static class FailAssertExtensions
    {
        /// <summary>Forces an assertion failure.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static FailAssertion Fail(this Asserter a,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "Fail", valueExpr: null, file: file, line: line, member: member);
            var assertion = new FailAssertion(p);
            assertion.Link("Fail");
            assertion.Op(a => CheckOperation.Sync(() => false, () => "Explicitly failed", isSkipped: false));
            return assertion;
        }
    }
}


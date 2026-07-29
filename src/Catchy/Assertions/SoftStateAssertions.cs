using System.Diagnostics;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    public static partial class AsserterExtensions
    {
        /// <summary>Starts assertions for a <see cref="SoftAsserter"/> state.</summary>
        public static global::Catchy.ValueAssertions<SoftState> That(this StatelessAsserter a, SoftAsserter softAssert,
            __._ _ = default,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(softAssert))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(
                asserterExpr: aExpr, methodName: "That",
                valueExpr: vExpr, file: file, line: line, member: member);
            return new global::Catchy.ValueAssertions<SoftState>(p, softAssert.SoftState);
        }

        /// <summary>Starts assertions for an existing <see cref="SoftState"/> instance.</summary>
        public static global::Catchy.ValueAssertions<SoftState> That(this StatelessAsserter a, SoftState state,
            __._ _ = default,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(state))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(
                asserterExpr: aExpr, methodName: "That",
                valueExpr: vExpr, file: file, line: line, member: member);
            return new global::Catchy.ValueAssertions<SoftState>(p, state);
        }
    }

    public static class SoftStateExtensions
    {
        /// <summary>Asserts that soft-state errors are empty.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static global::Catchy.ValueAssertions<SoftState> HasNoErrors(this global::Catchy.ValueAssertions<SoftState> a)
        {
            a.Link("HasNoErrors");
            var softState = a.GetValue();
            if (softState.HasFailures && !softState.AlreadyFlushed)
            {
                softState.AlreadyFlushed = true;
            }
            a.Op(a => CheckOperation.Sync(
                passes: () => !softState.HasFailures,
                failBody: () => Catchy.Sdk.SoftStateExtensions.BuildAggregateMessage(softState.Errors, a.GetPipeline().Source, null)
                             ?? "Soft asserter reported failures.",
                isSkipped: a.IsSkipped(),
                innerExceptionsFactory: () => softState.HasFailures ? softState.Errors : null));
            return a;
        }

        /// <summary>Projects collected soft-state errors.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static global::Catchy.ValueAssertions<IEnumerable<Exception>?> Errors(this global::Catchy.ValueAssertions<SoftState> a)
        {
            a.Link("Errors");
            var softState = a.GetValue();
            return new global::Catchy.ValueAssertions<IEnumerable<Exception>?>(a.GetPipeline(), softState.Errors);
        }

        /// <summary>Skips further soft-state assertions when the state was already flushed.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static global::Catchy.ValueAssertions<SoftState> UnlessAlreadyFlushed(this global::Catchy.ValueAssertions<SoftState> a)
        {
            a.Link("UnlessAlreadyFlushed");
            var softState = a.GetValue();
            if(softState.AlreadyFlushed)
            {
                a.Skip("Soft state has already been flushed. Skipping further assertions.");
            }
            return a;
        }
    }
}


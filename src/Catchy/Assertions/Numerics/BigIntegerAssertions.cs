using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    // BigInteger: no concrete wrapper needed.
    // ValueAssertions<BigInteger> satisfies the numeric extension constraints directly.
    public static partial class AsserterExtensions
    {
        /// <summary>Starts assertions for a <see cref="BigInteger"/> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<BigInteger> That(this Asserter a,
            BigInteger value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
            => new(a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member), value);

        /// <summary>Starts assertions for a nullable <see cref="BigInteger"/> value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<BigInteger?> That(this Asserter a,
            BigInteger? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
            => new(a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member), value);
    }
}

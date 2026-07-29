using System.Diagnostics;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    /// <summary>Provides IL-level dependency assertions for type collections and single types.</summary>
    public static class CecilTypeCollectionAssertionsExtensions
    {
        /// <summary>
        /// IL-level dependency check. More thorough than the reflection-based
        /// <c>DoNotHaveDependencyOn</c> in core — covers locals, casts, ldtoken, etc.
        /// Works on both <see cref="ValueAssertions{TValue}"/> over <see cref="IReadOnlyList{Type}"/>
        /// and quantified interception is automatic via AddOp.
        /// </summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>> DoNotHaveDependencyOnIL(
            this ValueAssertions<IEnumerable<Type>> a,
            string namespaceOrAssemblyName,
            [CallerArgumentExpression(nameof(namespaceOrAssemblyName))] string? expr = null)
        {
            a.Link("DoNotHaveDependencyOnIL", expr);
            a.Op(op => CecilDependencyChecks.NoneHaveDependencyOn(
                a.GetValue().ToList(), namespaceOrAssemblyName, op.IsSkipped()));
            return a;
        }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<Type>> HaveDependencyOnIL(
            this ValueAssertions<IEnumerable<Type>> a,
            string namespaceOrAssemblyName,
            [CallerArgumentExpression(nameof(namespaceOrAssemblyName))] string? expr = null)
        {
            a.Link("HaveDependencyOnIL", expr);
            a.Op(op => CecilDependencyChecks.AllHaveDependencyOn(
                a.GetValue().ToList(), namespaceOrAssemblyName, op.IsSkipped()));
            return a;
        }

        /// <summary>
        /// IL-level dependency check on a single <see cref="Type"/>.
        /// Works transparently on <see cref="QuantifiedAssertions{Type}"/> because
        /// <see cref="QuantifiedAssertions{T}"/> intercepts every AddOp and applies
        /// Each/Any/None semantics automatically — no special quantified overload needed.
        /// </summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type> DoNotHaveDependencyOnIL(
            this ValueAssertions<Type> a,
            string namespaceOrAssemblyName,
            [CallerArgumentExpression(nameof(namespaceOrAssemblyName))] string? expr = null)
        {
            a.Link("DoNotHaveDependencyOnIL", expr);

            string? failure = null;
            a.Op(a => CheckOperation.Sync(
                () =>
                {
                    var op = CecilDependencyChecks.NoneHaveDependencyOn(
                        [a.GetValue()], namespaceOrAssemblyName, a.IsSkipped());
                    var ok = op.PassesSync!();
                    if (!ok) failure = op.FailBecause();
                    return ok;
                },
                () => failure ?? $"Expected no types to depend on '{namespaceOrAssemblyName}'",
                a.IsSkipped()));
            return a;
        }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type> HaveDependencyOnIL(
            this ValueAssertions<Type> a,
            string namespaceOrAssemblyName,
            [CallerArgumentExpression(nameof(namespaceOrAssemblyName))] string? expr = null)
        {
            a.Link("HaveDependencyOnIL", expr);

            string? failure = null;
            a.Op(a => CheckOperation.Sync(
                () =>
                {
                    var op = CecilDependencyChecks.AllHaveDependencyOn(
                        [a.GetValue()], namespaceOrAssemblyName, a.IsSkipped());
                    var ok = op.PassesSync!();
                    if (!ok) failure = op.FailBecause();
                    return ok;
                },
                () => failure ?? $"Expected all types to depend on '{namespaceOrAssemblyName}'",
                a.IsSkipped()));
            return a;
        }
    }
}


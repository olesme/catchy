using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    /// <summary>Provides IL-level assembly dependency assertions for <see cref="Assembly"/> values.</summary>
    public static class CecilAssemblyAssertionsExtensions
    {
        /// <summary>Asserts that the assembly does not reference <paramref name="assemblyName"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Assembly> DoesNotReference(this ValueAssertions<Assembly> a, string assemblyName,
            [CallerArgumentExpression(nameof(assemblyName))] string? expr = null)
        {
            a.Link("DoesNotReference", expr);
            a.Op(a => CecilDependencyChecks.AssemblyDoesNotReference(a.GetValue(), assemblyName, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the assembly does not reference <paramref name="other"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Assembly> DoesNotReference(this ValueAssertions<Assembly> a, Assembly other,
            [CallerArgumentExpression(nameof(other))] string? expr = null)
        {
            a.Link("DoesNotReference", expr);
            a.Op(a => CecilDependencyChecks.AssemblyDoesNotReference(
                a.GetValue(), other.GetName().Name!, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the assembly references <paramref name="assemblyName"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Assembly> References(this ValueAssertions<Assembly> a, string assemblyName,
            [CallerArgumentExpression(nameof(assemblyName))] string? expr = null)
        {
            a.Link("References", expr);
            a.Op(a => CecilDependencyChecks.AssemblyReferences(a.GetValue(), assemblyName, a.IsSkipped()));
            return a;
        }
    }
}

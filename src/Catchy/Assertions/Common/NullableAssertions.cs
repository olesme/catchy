using System.Diagnostics;
using Catchy.Sdk;

// WHY ValueAssertions<T> AS RECEIVER (not TSelf + constraint):
// C# type inference does NOT infer type parameters from generic constraints.
// For no-parameter methods (IsNull, IsDefault), T can only come from the
// receiver type itself. Using 'where TSelf : ValueAssertions<T>' leaves T
// uninferred and causes CS0411 or CS1929. Using ValueAssertions<T> directly
// as the receiver lets C# infer T from the concrete receiver type.
//
// CRTP subtypes (StructuralAssertions<X>, etc.) inherit from ValueAssertions<X>,
// so they resolve to these extensions — the return type is ValueAssertions<T>
// not the concrete subtype, which is an acceptable tradeoff for null checks
// (callers rarely chain more typed assertions after a null assertion).
//
// Nullable constraint gates availability so that IsNull/IsNotNull are never
// offered on non-nullable value types (int, bool, DateTime, etc.):
//   where T : class?  -> reference types, including nullable-annotated ones
//   ValueAssertions<T?> where T : struct  -> Nullable<T> struct variants
// IsDefault is unconstrained: default(T) is meaningful for every T.
namespace Catchy
{
    public static class NullableAssertionMethods
    {
        /// <summary>Asserts that the value is <see langword="null"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T> IsNull<T>(this ValueAssertions<T> a)
            where T : class?
        {
            a.Link("IsNull");
            a.Op(a => NullChecks.IsNull(a.GetValue(), a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the nullable struct value is <see langword="null"/> (has no value).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T?> IsNull<T>(this ValueAssertions<T?> a)
            where T : struct
        {
            a.Link("IsNull");
            a.Op(a => NullChecks.IsNull(a.GetValue(), a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the value is not <see langword="null"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T> IsNotNull<T>(this ValueAssertions<T> a)
            where T : class?
        {
            a.Link("IsNotNull");
            a.Op(a => NullChecks.IsNotNull(a.GetValue(), a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the nullable struct value is not <see langword="null"/> (has a value).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T?> IsNotNull<T>(this ValueAssertions<T?> a)
            where T : struct
        {
            a.Link("IsNotNull");
            a.Op(a => NullChecks.IsNotNull(a.GetValue(), a.IsSkipped()));
            return a;
        }

        /// <summary>
        /// Asserts that the nullable struct value has a value (is not <see langword="null"/>).
        /// Alias for <see cref="IsNotNull{T}(ValueAssertions{T?})"/>.
        /// </summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T?> HasValue<T>(this ValueAssertions<T?> a)
            where T : struct
        {
            a.Link("HasValue");
            a.Op(a => NullChecks.IsNotNull(a.GetValue(), a.IsSkipped()));
            return a;
        }

        /// <summary>
        /// Asserts that the reference value is not <see langword="null"/>.
        /// Alias for <see cref="IsNotNull{T}(ValueAssertions{T})"/>.
        /// </summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T> HasValue<T>(this ValueAssertions<T> a)
            where T : class?
        {
            a.Link("HasValue");
            a.Op(a => NullChecks.IsNotNull(a.GetValue(), a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the value equals the default for its type (<see langword="null"/> for reference types, <c>default(T)</c> for value types).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<T> IsDefault<T>(this ValueAssertions<T> a)
        {
            a.Link("IsDefault");
            a.Op(a => NullChecks.IsDefault<T>(a.GetValue(), a.IsSkipped()));
            return a;
        }
    }
}


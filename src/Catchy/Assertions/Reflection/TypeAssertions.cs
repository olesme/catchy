using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    public static partial class AsserterExtensions
    {
        /// <summary>Starts assertions for a <see cref="Type"/> value.</summary>
        public static ValueAssertions<Type?> That(this Asserter a, Type? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<Type?>(p, value);
        }

        /// <summary>Starts assertions for the compile-time type argument <typeparamref name="T"/>.</summary>
        public static ValueAssertions<Type?> ThatType<T>(this Asserter a,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "ThatType", genericType: typeof(T), valueExpr: typeof(T).Name, file: file, line: line, member: member);
            return new ValueAssertions<Type?>(p, typeof(T));
        }
    }

    /// <summary>Provides fluent assertions and projections for <see cref="Type"/> values.</summary>
    public static class TypeAssertExtensions
    {
        /// <summary>Asserts that the type equals <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> Is(this ValueAssertions<Type?> a, Type expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("Is", expr); a.Op(a => CheckOperation.Sync(() => a.GetValue() == expected, () => $"Expected Type to be {expected}", a.IsSkipped())); return a; }

        /// <summary>Asserts that the type equals <typeparamref name="T"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> Is<T>(this ValueAssertions<Type?> a)
        { a.Link("Is", typeof(T)); a.Op(a => CheckOperation.Sync(() => a.GetValue() == typeof(T), () => $"Expected Type to be {typeof(T)}", a.IsSkipped())); return a; }


        /// <summary>Asserts that the type does not equal <paramref name="unexpected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsNot(this ValueAssertions<Type?> a, Type unexpected,
            [CallerArgumentExpression(nameof(unexpected))] string? expr = null)
        { a.Link("IsNot", expr); a.Op(a => CheckOperation.Sync(() => a.GetValue() != unexpected, () => $"Expected Type to not be {unexpected}", a.IsSkipped())); return a; }

        /// <summary>Asserts that the type does not equal <typeparamref name="T"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsNot<T>(this ValueAssertions<Type?> a)
        { a.Link("IsNot", typeof(T)); a.Op(a => CheckOperation.Sync(() => a.GetValue() != typeof(T), () => $"Expected Type to not be {typeof(T)}", a.IsSkipped())); return a; }


        /// <summary>Asserts that the type is assignable to <typeparamref name="T"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsAssignableTo<T>(this ValueAssertions<Type?> a)
        { a.Link("IsAssignableTo", typeof(T)); a.Op(a => TypeChecks.IsAssignableTo(a.GetValue(), typeof(T), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type is assignable to <paramref name="targetType"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsAssignableTo(this ValueAssertions<Type?> a, Type targetType, [CallerArgumentExpression(nameof(targetType))] string? targetTypeExpression = null)
        { a.Link("IsAssignableTo", targetTypeExpression); a.Op(a => TypeChecks.IsAssignableTo(a.GetValue(), targetType, a.IsSkipped())); return a; }


        /// <summary>Asserts that the type is not assignable to <typeparamref name="T"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsNotAssignableTo<T>(this ValueAssertions<Type?> a)
        { a.Link("IsNotAssignableTo", typeof(T)); a.Op(a => TypeChecks.IsNotAssignableTo(a.GetValue(), typeof(T), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type is assignable from <typeparamref name="T"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsAssignableFrom<T>(this ValueAssertions<Type?> a)
        { a.Link("IsAssignableFrom", typeof(T)); a.Op(a => TypeChecks.IsAssignableFrom(a.GetValue(), typeof(T), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type is assignable from <paramref name="targetType"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsAssignableFrom(this ValueAssertions<Type?> a, Type targetType, [CallerArgumentExpression(nameof(targetType))] string? targetTypeExpression = null)
        { a.Link("IsAssignableFrom", targetTypeExpression); a.Op(a => TypeChecks.IsAssignableFrom(a.GetValue(), targetType, a.IsSkipped())); return a; }


        /// <summary>Asserts that the type is not assignable from <typeparamref name="T"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsNotAssignableFrom<T>(this ValueAssertions<Type?> a)
        { a.Link("IsNotAssignableFrom", typeof(T)); a.Op(a => TypeChecks.IsNotAssignableFrom(a.GetValue(), typeof(T), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type implements <typeparamref name="TInterface"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> Implements<TInterface>(this ValueAssertions<Type?> a)
        { a.Link("Implements", typeof(TInterface)); a.Op(a => TypeChecks.Implements(a.GetValue(), typeof(TInterface), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type does not implement <typeparamref name="TInterface"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> DoesNotImplement<TInterface>(this ValueAssertions<Type?> a)
        { a.Link("DoesNotImplement", typeof(TInterface)); a.Op(a => TypeChecks.DoesNotImplement(a.GetValue(), typeof(TInterface), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type implements <paramref name="interfaceType"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> Implements(this ValueAssertions<Type?> a, Type interfaceType, [CallerArgumentExpression(nameof(interfaceType))] string? interfaceTypeExpression = null)
        { a.Link("Implements", interfaceTypeExpression); a.Op(a => TypeChecks.Implements(a.GetValue(), interfaceType, a.IsSkipped())); return a; }

        /// <summary>Asserts that the type does not implement <paramref name="interfaceType"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> DoesNotImplement(this ValueAssertions<Type?> a, Type interfaceType, [CallerArgumentExpression(nameof(interfaceType))] string? interfaceTypeExpression = null)
        { a.Link("DoesNotImplement", interfaceTypeExpression); a.Op(a => TypeChecks.DoesNotImplement(a.GetValue(), interfaceType, a.IsSkipped())); return a; }


        /// <summary>Asserts that the type inherits from <typeparamref name="TBase"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> Inherits<TBase>(this ValueAssertions<Type?> a)
        { a.Link("Inherits", typeof(TBase)); a.Op(a => TypeChecks.Inherits(a.GetValue(), typeof(TBase), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type does not inherit from <typeparamref name="TBase"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> DoesNotInherit<TBase>(this ValueAssertions<Type?> a)
        { a.Link("DoesNotInherit", typeof(TBase)); a.Op(a => TypeChecks.DoesNotInherit(a.GetValue(), typeof(TBase), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type inherits from <paramref name="type"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> Inherits(this ValueAssertions<Type?> a, Type type, [CallerArgumentExpression(nameof(type))] string? typeExpression = null)
        { a.Link("Inherits", typeExpression); a.Op(a => TypeChecks.Inherits(a.GetValue(), type, a.IsSkipped())); return a; }

        /// <summary>Asserts that the type does not inherit from <paramref name="type"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> DoesNotInherit(this ValueAssertions<Type?> a, Type type, [CallerArgumentExpression(nameof(type))] string? typeExpression = null)
        { a.Link("DoesNotInherit", typeExpression); a.Op(a => TypeChecks.DoesNotInherit(a.GetValue(), type, a.IsSkipped())); return a; }


        /// <summary>Asserts that the type has attribute <typeparamref name="TAttribute"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> HasAttribute<TAttribute>(this ValueAssertions<Type?> a) where TAttribute : Attribute
        { a.Link("HasAttribute", typeof(TAttribute)); a.Op(a => TypeChecks.HasAttribute(a.GetValue(), typeof(TAttribute), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type does not have attribute <typeparamref name="TAttribute"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> DoesNotHaveAttribute<TAttribute>(this ValueAssertions<Type?> a) where TAttribute : Attribute
        { a.Link("DoesNotHaveAttribute", typeof(TAttribute)); a.Op(a => TypeChecks.DoesNotHaveAttribute(a.GetValue(), typeof(TAttribute), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type is abstract.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsAbstract(this ValueAssertions<Type?> a)
        { a.Link("IsAbstract"); a.Op(a => TypeChecks.IsAbstract(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type is not abstract.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsNotAbstract(this ValueAssertions<Type?> a)
        { a.Link("IsNotAbstract"); a.Op(a => TypeChecks.IsNotAbstract(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type is sealed.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsSealed(this ValueAssertions<Type?> a)
        { a.Link("IsSealed"); a.Op(a => TypeChecks.IsSealed(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type is not sealed.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsNotSealed(this ValueAssertions<Type?> a)
        { a.Link("IsNotSealed"); a.Op(a => TypeChecks.IsNotSealed(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type is an interface.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsInterface(this ValueAssertions<Type?> a)
        { a.Link("IsInterface"); a.Op(a => TypeChecks.IsInterface(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type is not an interface.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsNotInterface(this ValueAssertions<Type?> a)
        { a.Link("IsNotInterface"); a.Op(a => TypeChecks.IsNotInterface(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type is a class.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsClass(this ValueAssertions<Type?> a)
        { a.Link("IsClass"); a.Op(a => TypeChecks.IsClass(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type is not a class.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsNotClass(this ValueAssertions<Type?> a)
        { a.Link("IsNotClass"); a.Op(a => TypeChecks.IsNotClass(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type is a value type.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsValueType(this ValueAssertions<Type?> a)
        { a.Link("IsValueType"); a.Op(a => TypeChecks.IsValueType(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type is not a value type.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsNotValueType(this ValueAssertions<Type?> a)
        { a.Link("IsNotValueType"); a.Op(a => TypeChecks.IsNotValueType(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type is an enum.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsEnum(this ValueAssertions<Type?> a)
        { a.Link("IsEnum"); a.Op(a => TypeChecks.IsEnum(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type is not an enum.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsNotEnum(this ValueAssertions<Type?> a)
        { a.Link("IsNotEnum"); a.Op(a => TypeChecks.IsNotEnum(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type is generic.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsGenericType(this ValueAssertions<Type?> a)
        { a.Link("IsGenericType"); a.Op(a => TypeChecks.IsGenericType(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type is not generic.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsNotGenericType(this ValueAssertions<Type?> a)
        { a.Link("IsNotGenericType"); a.Op(a => TypeChecks.IsNotGenericType(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type is a generic type definition.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsGenericTypeDefinition(this ValueAssertions<Type?> a)
        { a.Link("IsGenericTypeDefinition"); a.Op(a => TypeChecks.IsGenericTypeDefinition(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type is not a generic type definition.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsNotGenericTypeDefinition(this ValueAssertions<Type?> a)
        { a.Link("IsNotGenericTypeDefinition"); a.Op(a => TypeChecks.IsNotGenericTypeDefinition(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type is public.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsPublic(this ValueAssertions<Type?> a)
        { a.Link("IsPublic"); a.Op(a => TypeChecks.IsPublic(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type is not public.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsNotPublic(this ValueAssertions<Type?> a)
        { a.Link("IsNotPublic"); a.Op(a => TypeChecks.IsNotPublic(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type is static.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsStatic(this ValueAssertions<Type?> a)
        { a.Link("IsStatic"); a.Op(a => TypeChecks.IsStatic(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type is not static.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsNotStatic(this ValueAssertions<Type?> a)
        { a.Link("IsNotStatic"); a.Op(a => TypeChecks.IsNotStatic(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the type declares a property named <paramref name="propertyName"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> HasProperty(this ValueAssertions<Type?> a, string propertyName,
            [CallerArgumentExpression(nameof(propertyName))] string? expr = null)
        { a.Link("HasProperty", expr); a.Op(a => TypeChecks.HasProperty(a.GetValue(), propertyName, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the type does not declare a property named <paramref name="propertyName"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> DoesNotHaveProperty(this ValueAssertions<Type?> a, string propertyName,
            [CallerArgumentExpression(nameof(propertyName))] string? expr = null)
        { a.Link("DoesNotHaveProperty", expr); a.Op(a => TypeChecks.DoesNotHaveProperty(a.GetValue(), propertyName, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the type declares a method named <paramref name="methodName"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> HasMethod(this ValueAssertions<Type?> a, string methodName,
            [CallerArgumentExpression(nameof(methodName))] string? expr = null)
        { a.Link("HasMethod", expr); a.Op(a => TypeChecks.HasMethod(a.GetValue(), methodName, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the type does not declare a method named <paramref name="methodName"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> DoesNotHaveMethod(this ValueAssertions<Type?> a, string methodName,
            [CallerArgumentExpression(nameof(methodName))] string? expr = null)
        { a.Link("DoesNotHaveMethod", expr); a.Op(a => TypeChecks.DoesNotHaveMethod(a.GetValue(), methodName, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the type belongs to <paramref name="namespaceName"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsInNamespace(this ValueAssertions<Type?> a, string namespaceName,
            [CallerArgumentExpression(nameof(namespaceName))] string? expr = null)
        { a.Link("IsInNamespace", expr); a.Op(a => TypeChecks.IsInNamespace(a.GetValue(), namespaceName, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the type does not belong to <paramref name="namespaceName"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsNotInNamespace(this ValueAssertions<Type?> a, string namespaceName,
            [CallerArgumentExpression(nameof(namespaceName))] string? expr = null)
        { a.Link("IsNotInNamespace", expr); a.Op(a => TypeChecks.IsNotInNamespace(a.GetValue(), namespaceName, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the type has exactly <paramref name="count"/> generic arguments.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> HasGenericArgumentCount(this ValueAssertions<Type?> a, int count,
            [CallerArgumentExpression(nameof(count))] string? expr = null)
        { a.Link("HasGenericArgumentCount", expr); a.Op(a => TypeChecks.HasGenericArgumentCount(a.GetValue(), count, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the type is a reference type.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Type?> IsReferenceType(this ValueAssertions<Type?> a)
        { a.Link("IsReferenceType"); a.Op(a => TypeChecks.IsReferenceType(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Projects the type name.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<string> Name(this ValueAssertions<Type?> a)
        { a.Link("Name"); return new ValueAssertions<string>(a.GetPipeline(), a.GetValue()?.Name!); }

        /// <summary>Projects the full type name.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<string> FullName(this ValueAssertions<Type?> a)
        { a.Link("FullName"); return new ValueAssertions<string>(a.GetPipeline(), a.GetValue()?.FullName!); }

        /// <summary>Projects the type namespace.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<string> Namespace(this ValueAssertions<Type?> a)
        { a.Link("Namespace"); return new ValueAssertions<string>(a.GetPipeline(), a.GetValue()?.Namespace!); }

        /// <summary>Projects the generic type definition.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<Type?> GenericTypeDefinition(this ValueAssertions<Type?> a)
        {
            a.Link("GenericTypeDefinition");
            return new ValueAssertions<Type?>(a.GetPipeline(), a.GetValue()?.GetGenericTypeDefinition());
        }
    }
}


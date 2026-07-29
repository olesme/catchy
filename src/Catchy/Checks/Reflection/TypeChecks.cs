using System;
using System.Reflection;

namespace Catchy.Sdk
{
    public static class TypeChecks
    {
        static string Fmt(object? v, string? expr = null) => ExprFormat.Inline(v, expr);

        public static CheckOperation IsAssignableTo(Type? actual, Type target, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && target.IsAssignableFrom(actual),
                () => $"Expected {actual?.Name ?? "null"} to be assignable to {target.Name}",
                isSkipped);

        public static CheckOperation IsNotAssignableTo(Type? actual, Type target, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || !target.IsAssignableFrom(actual),
                () => $"Expected {actual?.Name ?? "null"} not to be assignable to {target.Name}",
                isSkipped);

        public static CheckOperation IsAssignableFrom(Type? actual, Type source, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.IsAssignableFrom(source),
                () => $"Expected {actual?.Name ?? "null"} to be assignable from {source.Name}",
                isSkipped);

        public static CheckOperation IsNotAssignableFrom(Type? actual, Type source, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || !actual.IsAssignableFrom(source),
                () => $"Expected {actual?.Name ?? "null"} not to be assignable from {source.Name}",
                isSkipped);

        public static CheckOperation Implements(Type? actual, Type interfaceType, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && interfaceType.IsInterface && interfaceType.IsAssignableFrom(actual),
                () => $"Expected {actual?.Name ?? "null"} to implement {interfaceType.Name}",
                isSkipped);

        public static CheckOperation DoesNotImplement(Type? actual, Type interfaceType, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || !interfaceType.IsInterface || !interfaceType.IsAssignableFrom(actual),
                () => $"Expected {actual?.Name ?? "null"} not to implement {interfaceType.Name}",
                isSkipped);

        public static CheckOperation Inherits(Type? actual, Type baseType, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.IsSubclassOf(baseType),
                () => $"Expected {actual?.Name ?? "null"} to inherit from {baseType.Name}",
                isSkipped);

        public static CheckOperation DoesNotInherit(Type? actual, Type baseType, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || !actual.IsSubclassOf(baseType),
                () => $"Expected {actual?.Name ?? "null"} not to inherit from {baseType.Name}",
                isSkipped);

        public static CheckOperation HasAttribute(Type? actual, Type attributeType, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.GetCustomAttribute(attributeType, inherit: true) is not null,
                () => $"Expected {actual?.Name ?? "null"} to have [{attributeType.Name}]",
                isSkipped);

        public static CheckOperation DoesNotHaveAttribute(Type? actual, Type attributeType, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || actual.GetCustomAttribute(attributeType, inherit: true) is null,
                () => $"Expected {actual?.Name ?? "null"} not to have [{attributeType.Name}]",
                isSkipped);

        public static CheckOperation IsAbstract(Type? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.IsAbstract,
                () => $"Expected {actual?.Name ?? "null"} to be abstract",
                isSkipped);

        public static CheckOperation IsNotAbstract(Type? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || !actual.IsAbstract,
                () => $"Expected {actual?.Name ?? "null"} not to be abstract",
                isSkipped);

        public static CheckOperation IsSealed(Type? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.IsSealed,
                () => $"Expected {actual?.Name ?? "null"} to be sealed",
                isSkipped);

        public static CheckOperation IsNotSealed(Type? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || !actual.IsSealed,
                () => $"Expected {actual?.Name ?? "null"} not to be sealed",
                isSkipped);

        public static CheckOperation IsInterface(Type? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.IsInterface,
                () => $"Expected {actual?.Name ?? "null"} to be an interface",
                isSkipped);

        public static CheckOperation IsNotInterface(Type? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || !actual.IsInterface,
                () => $"Expected {actual?.Name ?? "null"} not to be an interface",
                isSkipped);

        public static CheckOperation IsClass(Type? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.IsClass,
                () => $"Expected {actual?.Name ?? "null"} to be a class",
                isSkipped);

        public static CheckOperation IsNotClass(Type? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || !actual.IsClass,
                () => $"Expected {actual?.Name ?? "null"} not to be a class",
                isSkipped);

        public static CheckOperation IsValueType(Type? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.IsValueType,
                () => $"Expected {actual?.Name ?? "null"} to be a value type",
                isSkipped);

        public static CheckOperation IsNotValueType(Type? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || !actual.IsValueType,
                () => $"Expected {actual?.Name ?? "null"} not to be a value type",
                isSkipped);

        public static CheckOperation IsEnum(Type? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.IsEnum,
                () => $"Expected {actual?.Name ?? "null"} to be an enum",
                isSkipped);

        public static CheckOperation IsNotEnum(Type? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || !actual.IsEnum,
                () => $"Expected {actual?.Name ?? "null"} not to be an enum",
                isSkipped);

        public static CheckOperation IsGenericType(Type? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.IsGenericType,
                () => $"Expected {actual?.Name ?? "null"} to be a generic type",
                isSkipped);

        public static CheckOperation IsNotGenericType(Type? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || !actual.IsGenericType,
                () => $"Expected {actual?.Name ?? "null"} not to be a generic type",
                isSkipped);

        public static CheckOperation IsGenericTypeDefinition(Type? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.IsGenericTypeDefinition,
                () => $"Expected {actual?.Name ?? "null"} to be an open generic type definition",
                isSkipped);

        public static CheckOperation IsNotGenericTypeDefinition(Type? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || !actual.IsGenericTypeDefinition,
                () => $"Expected {actual?.Name ?? "null"} not to be an open generic type definition",
                isSkipped);

        public static CheckOperation IsPublic(Type? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.IsPublic,
                () => $"Expected {actual?.Name ?? "null"} to be public",
                isSkipped);

        public static CheckOperation IsNotPublic(Type? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || !actual.IsPublic,
                () => $"Expected {actual?.Name ?? "null"} not to be public",
                isSkipped);

        public static CheckOperation IsStatic(Type? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.IsAbstract && actual.IsSealed,
                () => $"Expected {actual?.Name ?? "null"} to be a static class",
                isSkipped);

        public static CheckOperation IsNotStatic(Type? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || !(actual.IsAbstract && actual.IsSealed),
                () => $"Expected {actual?.Name ?? "null"} not to be a static class",
                isSkipped);

        public static CheckOperation HasProperty(Type? actual, string propertyName, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && actual.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static) is not null,
                () => $"Expected {actual?.Name ?? "null"} to have property {Fmt(propertyName, expr)}",
                isSkipped);

        public static CheckOperation DoesNotHaveProperty(Type? actual, string propertyName, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is null || actual.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static) is null,
                () => $"Expected {actual?.Name ?? "null"} not to have property {Fmt(propertyName, expr)}",
                isSkipped);

        public static CheckOperation HasMethod(Type? actual, string methodName, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && actual.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).Any(m => m.Name == methodName),
                () => $"Expected {actual?.Name ?? "null"} to have method {Fmt(methodName, expr)}",
                isSkipped);

        public static CheckOperation DoesNotHaveMethod(Type? actual, string methodName, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is null || !actual.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).Any(m => m.Name == methodName),
                () => $"Expected {actual?.Name ?? "null"} not to have method {Fmt(methodName, expr)}",
                isSkipped);

        public static CheckOperation IsInNamespace(Type? actual, string namespaceName, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && string.Equals(actual.Namespace, namespaceName, StringComparison.Ordinal),
                () => $"Expected {actual?.Name ?? "null"} to be in namespace {Fmt(namespaceName, expr)}, but was in '{actual?.Namespace ?? "(global)"}'",
                isSkipped);

        public static CheckOperation IsNotInNamespace(Type? actual, string namespaceName, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is null || !string.Equals(actual.Namespace, namespaceName, StringComparison.Ordinal),
                () => $"Expected {actual?.Name ?? "null"} not to be in namespace {Fmt(namespaceName, expr)}",
                isSkipped);

        public static CheckOperation HasGenericArgumentCount(Type? actual, int count, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && actual.GetGenericArguments().Length == count,
                () => $"Expected {actual?.Name ?? "null"} to have {Fmt(count, expr)} generic argument(s), but had {actual?.GetGenericArguments().Length.ToString() ?? "N/A"}",
                isSkipped);

        public static CheckOperation IsReferenceType(Type? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && !actual.IsValueType,
                () => $"Expected {actual?.Name ?? "null"} to be a reference type",
                isSkipped);
    }
}

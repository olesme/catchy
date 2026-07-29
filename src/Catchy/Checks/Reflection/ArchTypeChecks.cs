using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Catchy.Sdk
{
    public static class ArchTypeChecks
    {
        private static string FmtType(Type t) => t.FullName ?? t.Name;

        private static string FmtViolations(IReadOnlyList<Type> types, int max = 10)
        {
            var sb = new StringBuilder();
            foreach (var t in types.Take(max))
                sb.AppendLine($"  {FmtType(t)}");
            if (types.Count > max)
                sb.AppendLine($"  … and {types.Count - max} more");
            return sb.ToString().TrimEnd();
        }

        // Core helper — all checks follow this pattern
        private static CheckOperation Make(
            IReadOnlyList<Type> types,
            Func<Type, bool> violates,
            Func<List<Type>, string> failMsg,
            bool isSkipped)
        {
            List<Type>? violations = null;
            return CheckOperation.Sync(
                () => { violations = [.. types.Where(violates)]; return violations.Count == 0; },
                () => failMsg(violations!),
                isSkipped);
        }

        // Sealed

        public static CheckOperation AllAreSealed(IReadOnlyList<Type> types, bool isSkipped)
            => Make(types, t => !t.IsSealed, v =>
                $"Expected all types to be sealed, but {v.Count} were not:\n{FmtViolations(v)}", isSkipped);

        public static CheckOperation NoneAreSealed(IReadOnlyList<Type> types, bool isSkipped)
            => Make(types, t => t.IsSealed, v =>
                $"Expected no types to be sealed, but {v.Count} were:\n{FmtViolations(v)}", isSkipped);

        // Abstract

        public static CheckOperation AllAreAbstract(IReadOnlyList<Type> types, bool isSkipped)
            => Make(types, t => !t.IsAbstract, v =>
                $"Expected all types to be abstract, but {v.Count} were not:\n{FmtViolations(v)}", isSkipped);

        public static CheckOperation NoneAreAbstract(IReadOnlyList<Type> types, bool isSkipped)
            => Make(types, t => t.IsAbstract, v =>
                $"Expected no types to be abstract, but {v.Count} were:\n{FmtViolations(v)}", isSkipped);

        // Access modifiers

        public static CheckOperation AllArePublic(IReadOnlyList<Type> types, bool isSkipped)
            => Make(types, t => !IsPublic(t), v =>
                $"Expected all types to be public, but {v.Count} were not:\n{FmtViolations(v)}", isSkipped);

        public static CheckOperation NoneArePublic(IReadOnlyList<Type> types, bool isSkipped)
            => Make(types, IsPublic, v =>
                $"Expected no types to be public, but {v.Count} were:\n{FmtViolations(v)}", isSkipped);

        public static CheckOperation AllAreInternal(IReadOnlyList<Type> types, bool isSkipped)
            => Make(types, t => !IsInternal(t), v =>
                $"Expected all types to be internal, but {v.Count} were not:\n{FmtViolations(v)}", isSkipped);

        public static CheckOperation NoneAreInternal(IReadOnlyList<Type> types, bool isSkipped)
            => Make(types, IsInternal, v =>
                $"Expected no types to be internal, but {v.Count} were:\n{FmtViolations(v)}", isSkipped);

        public static CheckOperation AllAreNestedPublic(IReadOnlyList<Type> types, bool isSkipped)
            => Make(types, t => !(t.IsNested && t.IsNestedPublic), v =>
                $"Expected all types to be nested public, but {v.Count} were not:\n{FmtViolations(v)}", isSkipped);

        public static CheckOperation AllAreNestedPrivate(IReadOnlyList<Type> types, bool isSkipped)
            => Make(types, t => !(t.IsNested && t.IsNestedPrivate), v =>
                $"Expected all types to be nested private, but {v.Count} were not:\n{FmtViolations(v)}", isSkipped);

        private static bool IsPublic(Type t)
            => t.IsPublic || (t.IsNested && t.IsNestedPublic);

        private static bool IsInternal(Type t)
            => (!t.IsNested && !t.IsPublic) || (t.IsNested && t.IsNestedAssembly);

        // Static

        public static CheckOperation AllAreStatic(IReadOnlyList<Type> types, bool isSkipped)
            => Make(types, t => !(t.IsAbstract && t.IsSealed && t.IsClass), v =>
                $"Expected all types to be static classes, but {v.Count} were not:\n{FmtViolations(v)}", isSkipped);

        public static CheckOperation NoneAreStatic(IReadOnlyList<Type> types, bool isSkipped)
            => Make(types, t => t.IsAbstract && t.IsSealed && t.IsClass, v =>
                $"Expected no types to be static classes, but {v.Count} were:\n{FmtViolations(v)}", isSkipped);

        // Nested

        public static CheckOperation AllAreNested(IReadOnlyList<Type> types, bool isSkipped)
            => Make(types, t => !t.IsNested, v =>
                $"Expected all types to be nested, but {v.Count} were not:\n{FmtViolations(v)}", isSkipped);

        public static CheckOperation NoneAreNested(IReadOnlyList<Type> types, bool isSkipped)
            => Make(types, t => t.IsNested, v =>
                $"Expected no types to be nested, but {v.Count} were:\n{FmtViolations(v)}", isSkipped);

        // Generic

        public static CheckOperation AllAreGeneric(IReadOnlyList<Type> types, bool isSkipped)
            => Make(types, t => !t.IsGenericTypeDefinition, v =>
                $"Expected all types to be generic, but {v.Count} were not:\n{FmtViolations(v)}", isSkipped);

        public static CheckOperation NoneAreGeneric(IReadOnlyList<Type> types, bool isSkipped)
            => Make(types, t => t.IsGenericTypeDefinition, v =>
                $"Expected no types to be generic, but {v.Count} were:\n{FmtViolations(v)}", isSkipped);

        // Immutability

        private const string IsExternalInitFqn = "System.Runtime.CompilerServices.IsExternalInit";

        public static CheckOperation AllAreImmutable(IReadOnlyList<Type> types, bool isSkipped)
            => Make(types, t => !IsImmutable(t), v =>
                $"Expected all types to be immutable, but {v.Count} were not:\n{FmtViolations(v)}", isSkipped);

        public static CheckOperation AllAreMutable(IReadOnlyList<Type> types, bool isSkipped)
            => Make(types, IsImmutable, v =>
                $"Expected all types to be mutable, but {v.Count} were not:\n{FmtViolations(v)}", isSkipped);

        private static bool IsImmutable(Type type)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            if (!type.GetFields(flags).All(f => f.IsInitOnly)) return false;
            return type.GetProperties(flags).All(p =>
            {
                var setter = p.GetSetMethod(nonPublic: true);
                if (setter is null) return true;
                // init-only setters have IsExternalInit modreq
                return setter.ReturnParameter
                    .GetRequiredCustomModifiers()
                    .Any(m => m.FullName == IsExternalInitFqn);
            });
        }

        // Interfaces

        public static CheckOperation AllImplement(IReadOnlyList<Type> types, Type iface, bool isSkipped)
            => Make(types, t => !iface.IsAssignableFrom(t), v =>
                $"Expected all types to implement {iface.Name}, but {v.Count} did not:\n{FmtViolations(v)}", isSkipped);

        public static CheckOperation NoneImplement(IReadOnlyList<Type> types, Type iface, bool isSkipped)
            => Make(types, t => iface.IsAssignableFrom(t), v =>
                $"Expected no types to implement {iface.Name}, but {v.Count} did:\n{FmtViolations(v)}", isSkipped);

        // Inheritance

        public static CheckOperation AllInheritFrom(IReadOnlyList<Type> types, Type baseType, bool isSkipped)
            => Make(types, t => !t.IsSubclassOf(baseType), v =>
                $"Expected all types to inherit from {baseType.Name}, but {v.Count} did not:\n{FmtViolations(v)}", isSkipped);

        public static CheckOperation NoneInheritFrom(IReadOnlyList<Type> types, Type baseType, bool isSkipped)
            => Make(types, t => t.IsSubclassOf(baseType), v =>
                $"Expected no types to inherit from {baseType.Name}, but {v.Count} did:\n{FmtViolations(v)}", isSkipped);

        // Attributes

        public static CheckOperation AllHaveAttribute(IReadOnlyList<Type> types, Type attrType, bool inherit, bool isSkipped)
            => Make(types, t => t.GetCustomAttribute(attrType, inherit) is null, v =>
                $"Expected all types to have [{attrType.Name}], but {v.Count} did not:\n{FmtViolations(v)}", isSkipped);

        public static CheckOperation NoneHaveAttribute(IReadOnlyList<Type> types, Type attrType, bool inherit, bool isSkipped)
            => Make(types, t => t.GetCustomAttribute(attrType, inherit) is not null, v =>
                $"Expected no types to have [{attrType.Name}], but {v.Count} did:\n{FmtViolations(v)}", isSkipped);

        // Name patterns

        public static CheckOperation AllHaveNameStartingWith(IReadOnlyList<Type> types, string prefix, bool isSkipped)
            => Make(types, t => !t.Name.StartsWith(prefix, StringComparison.Ordinal), v =>
                $"Expected all type names to start with \"{prefix}\", but {v.Count} did not:\n{FmtViolations(v)}", isSkipped);

        public static CheckOperation NoneHaveNameStartingWith(IReadOnlyList<Type> types, string prefix, bool isSkipped)
            => Make(types, t => t.Name.StartsWith(prefix, StringComparison.Ordinal), v =>
                $"Expected no type names to start with \"{prefix}\", but {v.Count} did:\n{FmtViolations(v)}", isSkipped);

        public static CheckOperation AllHaveNameEndingWith(IReadOnlyList<Type> types, string suffix, bool isSkipped)
            => Make(types, t => !t.Name.EndsWith(suffix, StringComparison.Ordinal), v =>
                $"Expected all type names to end with \"{suffix}\", but {v.Count} did not:\n{FmtViolations(v)}", isSkipped);

        public static CheckOperation NoneHaveNameEndingWith(IReadOnlyList<Type> types, string suffix, bool isSkipped)
            => Make(types, t => t.Name.EndsWith(suffix, StringComparison.Ordinal), v =>
                $"Expected no type names to end with \"{suffix}\", but {v.Count} did:\n{FmtViolations(v)}", isSkipped);

        public static CheckOperation AllHaveNameContaining(IReadOnlyList<Type> types, string value, bool isSkipped)
            => Make(types, t => !t.Name.Contains(value, StringComparison.Ordinal), v =>
                $"Expected all type names to contain \"{value}\", but {v.Count} did not:\n{FmtViolations(v)}", isSkipped);

        public static CheckOperation NoneHaveNameContaining(IReadOnlyList<Type> types, string value, bool isSkipped)
            => Make(types, t => t.Name.Contains(value, StringComparison.Ordinal), v =>
                $"Expected no type names to contain \"{value}\", but {v.Count} did:\n{FmtViolations(v)}", isSkipped);

        public static CheckOperation AllHaveNameMatching(IReadOnlyList<Type> types, Regex pattern, bool isSkipped)
            => Make(types, t => !pattern.IsMatch(t.Name), v =>
                $"Expected all type names to match /{pattern}/, but {v.Count} did not:\n{FmtViolations(v)}", isSkipped);

        public static CheckOperation NoneHaveNameMatching(IReadOnlyList<Type> types, Regex pattern, bool isSkipped)
            => Make(types, t => pattern.IsMatch(t.Name), v =>
                $"Expected no type names to match /{pattern}/, but {v.Count} did:\n{FmtViolations(v)}", isSkipped);

        // Namespace

        public static CheckOperation AllResideInNamespace(IReadOnlyList<Type> types, string ns, bool exactMatch, bool isSkipped)
        {
            bool violates(Type t) => exactMatch
                ? t.Namespace != ns
                : t.Namespace?.StartsWith(ns, StringComparison.Ordinal) != true;
            var qualifier = exactMatch ? "(exact)" : "(or sub-namespace)";
            return Make(types, violates, v =>
                $"Expected all types to reside in namespace '{ns}' {qualifier}, but {v.Count} did not:\n{FmtViolations(v)}", isSkipped);
        }

        public static CheckOperation NoneResideInNamespace(IReadOnlyList<Type> types, string ns, bool exactMatch, bool isSkipped)
        {
            bool violates(Type t) => exactMatch
                ? t.Namespace == ns
                : t.Namespace?.StartsWith(ns, StringComparison.Ordinal) == true;
            return Make(types, violates, v =>
                $"Expected no types to reside in namespace '{ns}', but {v.Count} did:\n{FmtViolations(v)}", isSkipped);
        }

        // Custom predicate

        public static CheckOperation AllSatisfy(IReadOnlyList<Type> types,
            Func<Type, bool> predicate, string? predicateExpr, bool isSkipped)
            => Make(types, t => !predicate(t), v =>
                $"Expected all types to satisfy {predicateExpr ?? "<predicate>"}, but {v.Count} did not:\n{FmtViolations(v)}", isSkipped);

        public static CheckOperation NoneSatisfy(IReadOnlyList<Type> types,
            Func<Type, bool> predicate, string? predicateExpr, bool isSkipped)
            => Make(types, predicate, v =>
                $"Expected no types to satisfy {predicateExpr ?? "<predicate>"}, but {v.Count} did:\n{FmtViolations(v)}", isSkipped);

        // Count

        public static CheckOperation IsNotEmpty(IReadOnlyList<Type> types, bool isSkipped)
            => CheckOperation.Sync(
                () => types.Count > 0,
                () => "Expected at least one type, but the collection was empty",
                isSkipped);

        public static CheckOperation HasCount(IReadOnlyList<Type> types, int expected, bool isSkipped)
            => CheckOperation.Sync(
                () => types.Count == expected,
                () => $"Expected {expected} type(s), but found {types.Count}",
                isSkipped);

        public static CheckOperation HasCountGreaterThan(IReadOnlyList<Type> types, int count, bool isSkipped)
            => CheckOperation.Sync(
                () => types.Count > count,
                () => $"Expected more than {count} type(s), but found {types.Count}",
                isSkipped);

        public static CheckOperation HasCountLessThan(IReadOnlyList<Type> types, int count, bool isSkipped)
            => CheckOperation.Sync(
                () => types.Count < count,
                () => $"Expected fewer than {count} type(s), but found {types.Count}",
                isSkipped);

        // Reflection-based dependency (no Cecil needed)

        public static CheckOperation AllHaveDependencyOn(IReadOnlyList<Type> types, string target, bool isSkipped)
        {
            List<Type>? violations = null;
            return CheckOperation.Sync(
                () => { violations = [.. types.Where(t => !ReflectionDependency.HasDependencyOn(t, target))]; return violations.Count == 0; },
                () => $"Expected all types to have a dependency on '{target}', but {violations!.Count} did not:\n{FmtViolations(violations)}",
                isSkipped);
        }

        public static CheckOperation NoneHaveDependencyOn(IReadOnlyList<Type> types, string target, bool isSkipped)
        {
            List<Type>? violations = null;
            return CheckOperation.Sync(
                () => { violations = [.. types.Where(t => ReflectionDependency.HasDependencyOn(t, target))]; return violations.Count == 0; },
                () => $"Expected no types to have a dependency on '{target}', but {violations!.Count} did:\n{FmtViolations(violations)}",
                isSkipped);
        }
    }

    /// <summary>
    /// Reflection-based dependency analysis for loaded assemblies.
    /// Covers fields, properties, method signatures, base types, and interfaces.
    /// For IL-level analysis (local variables, casts, etc.) use Catchy.Cecil.
    /// </summary>
    internal static class ReflectionDependency
    {
        private static readonly BindingFlags All =
            BindingFlags.Instance | BindingFlags.Static |
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        public static bool HasDependencyOn(Type type, string namespaceOrAssembly)
        {
            foreach (var referenced in CollectReferencedTypes(type))
            {
                if (referenced.Namespace?.StartsWith(namespaceOrAssembly, StringComparison.Ordinal) == true)
                    return true;
                if (referenced.Assembly.GetName().Name == namespaceOrAssembly)
                    return true;
            }
            return false;
        }

        private static IEnumerable<Type> CollectReferencedTypes(Type type)
        {
            var seen = new HashSet<Type>();

            void Visit(Type? t)
            {
                if (t is null || t == type || !seen.Add(t)) return;
                // unwrap generics, arrays, pointers
                if (t.IsGenericType)
                    foreach (var arg in t.GetGenericArguments()) Visit(arg);
                if (t.IsArray || t.IsPointer || t.IsByRef)
                    Visit(t.GetElementType());
            }

            // Base type + interfaces
            Visit(type.BaseType);
            foreach (var iface in type.GetInterfaces()) Visit(iface);

            // Fields
            foreach (var f in type.GetFields(All)) Visit(f.FieldType);

            // Properties
            foreach (var p in type.GetProperties(All)) Visit(p.PropertyType);

            // Methods (declared only to avoid pulling in inherited noise)
            foreach (var m in type.GetMethods(All))
            {
                Visit(m.ReturnType);
                foreach (var p in m.GetParameters()) Visit(p.ParameterType);
            }

            // Constructor parameters
            foreach (var ctor in type.GetConstructors(All))
                foreach (var p in ctor.GetParameters()) Visit(p.ParameterType);

            return seen;
        }
    }
}

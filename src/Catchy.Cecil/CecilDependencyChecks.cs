using System.Reflection;
using System.Text;
using Mono.Cecil;

namespace Catchy
{
    namespace Sdk
    {
        public static class CecilDependencyChecks
        {
            private static string FmtViolations(IEnumerable<(Type type, IReadOnlyList<string> reasons)> violations,
                int maxTypes = 10, int maxReasons = 3)
            {
                var sb = new StringBuilder();
                int count = 0;
                foreach (var (type, reasons) in violations)
                {
                    if (count++ >= maxTypes) { sb.AppendLine($"  … and more"); break; }
                    sb.AppendLine($"  {type.FullName ?? type.Name}");
                    foreach (var r in reasons.Take(maxReasons))
                        sb.AppendLine($"    ← {r}");
                    if (reasons.Count > maxReasons)
                        sb.AppendLine($"    … and {reasons.Count - maxReasons} more");
                }
                return sb.ToString().TrimEnd();
            }

            /// <summary>
            /// Checks that none of the given types (via IL analysis) reference types
            /// in the given namespace or assembly name.
            /// </summary>
            public static CheckOperation NoneHaveDependencyOn(
                IReadOnlyList<Type> types,
                string namespaceOrAssemblyName,
                bool isSkipped)
            {
                List<(Type, IReadOnlyList<string>)>? violations = null;

                return CheckOperation.Sync(
                    () =>
                    {
                        violations = [];
                        var resolver = new DefaultAssemblyResolver();
                        foreach (var type in types)
                        {
                            var assemblyPath = type.Assembly.Location;
                            if (string.IsNullOrEmpty(assemblyPath)) continue;

                            var moduleDef = ModuleDefinition.ReadModule(assemblyPath,
                                new ReaderParameters { AssemblyResolver = resolver });
                            var typeDef = moduleDef.GetType(type.FullName);
                            if (typeDef is null) continue;

                            var deps = CollectDependencies(typeDef)
                                .Where(r => MatchesTarget(r.typeRef, namespaceOrAssemblyName))
                                .Select(r => r.context)
                                .Distinct()
                                .ToList();

                            if (deps.Count > 0)
                                violations.Add((type, deps));
                        }
                        return violations.Count == 0;
                    },
                    () => $"Expected no types to depend on '{namespaceOrAssemblyName}', " +
                          $"but {violations!.Count} did:\n{FmtViolations(violations!)}",
                    isSkipped);
            }

            /// <summary>
            /// Checks that all of the given types have at least one dependency on the target.
            /// </summary>
            public static CheckOperation AllHaveDependencyOn(
                IReadOnlyList<Type> types,
                string namespaceOrAssemblyName,
                bool isSkipped)
            {
                List<Type>? violations = null;

                return CheckOperation.Sync(
                    () =>
                    {
                        violations = [];
                        var resolver = new DefaultAssemblyResolver();
                        foreach (var type in types)
                        {
                            var assemblyPath = type.Assembly.Location;
                            if (string.IsNullOrEmpty(assemblyPath)) { violations.Add(type); continue; }

                            var moduleDef = ModuleDefinition.ReadModule(assemblyPath,
                                new ReaderParameters { AssemblyResolver = resolver });
                            var typeDef = moduleDef.GetType(type.FullName);
                            if (typeDef is null) { violations.Add(type); continue; }

                            bool found = CollectDependencies(typeDef)
                                .Any(r => MatchesTarget(r.typeRef, namespaceOrAssemblyName));

                            if (!found) violations.Add(type);
                        }
                        return violations.Count == 0;
                    },
                    () =>
                    {
                        var names = string.Join("\n", violations!.Take(10).Select(t => $"  {t.FullName ?? t.Name}"));
                        return $"Expected all types to depend on '{namespaceOrAssemblyName}', " +
                               $"but {violations!.Count} did not:\n{names}";
                    },
                    isSkipped);
            }

            /// <summary>
            /// Checks that the assembly itself does not reference another assembly by name.
            /// Useful for layer boundary rules at the assembly level.
            /// Checks both metadata-level assembly references AND IL-level type usage.
            /// </summary>
            public static CheckOperation AssemblyDoesNotReference(
                Assembly assembly,
                string referencedAssemblyName,
                bool isSkipped)
            {
                List<string>? found = null;

                return CheckOperation.Sync(
                    () =>
                    {
                        found = [];
                        if (string.IsNullOrEmpty(assembly.Location)) return true;

                        var moduleDef = ModuleDefinition.ReadModule(assembly.Location,
                            new ReaderParameters { AssemblyResolver = new DefaultAssemblyResolver() });

                        // Check metadata-level assembly references
                        var metadataRefs = moduleDef.AssemblyReferences
                            .Where(r => AssemblyNameMatches(r.Name, referencedAssemblyName))
                            .Select(r => r.FullName)
                            .ToList();
                        found.AddRange(metadataRefs);

                        // Check IL-level type usage
                        var ilRefs = new HashSet<string>();
                        try
                        {
                            foreach (var type in moduleDef.Types)
                            {
                                try
                                {
                                    foreach (var (typeRef, _) in CollectDependencies(type))
                                    {
                                        if (MatchesTarget(typeRef, referencedAssemblyName))
                                        {
                                            ilRefs.Add(typeRef.FullName ?? typeRef.Name);
                                        }
                                    }
                                }
                                catch
                                {
                                    // Skip types that cause issues during dependency collection
                                }
                            }
                        }
                        catch
                        {
                            // If we can't scan types, at least we have metadata results
                        }
                        found.AddRange(ilRefs);

                        return found.Count == 0;
                    },
                    () => $"Expected assembly '{assembly.GetName().Name}' not to reference " +
                          $"'{referencedAssemblyName}', but it does:\n" +
                          string.Join("\n", found!.Distinct().Select(r => $"  {r}")),
                    isSkipped);
            }

            /// <summary>
            /// Checks that the assembly references an expected assembly.
            /// Checks both metadata-level assembly references AND IL-level type usage.
            /// </summary>
            public static CheckOperation AssemblyReferences(
                Assembly assembly,
                string referencedAssemblyName,
                bool isSkipped)
            {
                return CheckOperation.Sync(
                    () =>
                    {
                        if (string.IsNullOrEmpty(assembly.Location)) return false;

                        var moduleDef = ModuleDefinition.ReadModule(assembly.Location,
                            new ReaderParameters { AssemblyResolver = new DefaultAssemblyResolver() });

                        // Check metadata-level assembly references
                        if (moduleDef.AssemblyReferences
                            .Any(r => AssemblyNameMatches(r.Name, referencedAssemblyName)))
                            return true;

                        // Check IL-level type usage
                        try
                        {
                            foreach (var type in moduleDef.Types)
                            {
                                try
                                {
                                    foreach (var (typeRef, _) in CollectDependencies(type))
                                    {
                                        if (MatchesTarget(typeRef, referencedAssemblyName))
                                            return true;
                                    }
                                }
                                catch
                                {
                                    // Skip types that cause issues during dependency collection
                                }
                            }
                        }
                        catch
                        {
                            // If we can't scan types, at least we have metadata results
                        }

                        return false;
                    },
                    () => $"Expected assembly '{assembly.GetName().Name}' to reference '{referencedAssemblyName}', but it does not",
                    isSkipped);
            }

            // Cecil helpers

            private static bool MatchesTarget(TypeReference typeRef, string target)
            {
                var ns = typeRef.Namespace ?? "";
                var scope = typeRef.Scope?.Name ?? "";

                // Normalize: remove .dll extension if present
                var normalizedScope = scope;
                if (normalizedScope.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                    normalizedScope = normalizedScope.Substring(0, normalizedScope.Length - 4);

                var normalizedTarget = target;
                if (normalizedTarget.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                    normalizedTarget = normalizedTarget.Substring(0, normalizedTarget.Length - 4);

                // Check if namespace starts with target (e.g., "xunit.core.xyz" starts with "xunit")
                if (ns.StartsWith(target, StringComparison.Ordinal))
                    return true;

                // Check exact assembly name match (case-insensitive)
                if (string.Equals(normalizedScope, normalizedTarget, StringComparison.OrdinalIgnoreCase))
                    return true;

                // Check if scope name starts with target (e.g., "xunit.core" starts with "xunit")
                if (normalizedScope.StartsWith(normalizedTarget, StringComparison.OrdinalIgnoreCase))
                    return true;

                return false;
            }

            private static bool AssemblyNameMatches(string assemblyName, string target)
            {
                // Normalize: remove .dll extension if present
                var normalized = assemblyName;
                if (normalized.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                    normalized = normalized.Substring(0, normalized.Length - 4);

                var normalizedTarget = target;
                if (normalizedTarget.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                    normalizedTarget = normalizedTarget.Substring(0, normalizedTarget.Length - 4);

                // Exact match (case-insensitive)
                if (string.Equals(normalized, normalizedTarget, StringComparison.OrdinalIgnoreCase))
                    return true;

                // Check if assembly name starts with target (e.g., "xunit.core" matches "xunit")
                if (normalized.StartsWith(normalizedTarget, StringComparison.OrdinalIgnoreCase))
                    return true;

                return false;
            }

            private static IEnumerable<(TypeReference typeRef, string context)> CollectDependencies(TypeDefinition type)
            {
                // Skip compiler-generated types (they often have generated names)
                if (type.Name.StartsWith("<") || type.Name.Contains("__"))
                    yield break;

                // Base type
                if (type.BaseType is not null)
                    yield return (type.BaseType, $"base type: {type.BaseType.FullName}");

                // Interfaces
                foreach (var iface in type.Interfaces)
                    yield return (iface.InterfaceType, $"interface: {iface.InterfaceType.FullName}");

                // Fields
                foreach (var field in type.Fields)
                    yield return (field.FieldType, $"field {field.Name}: {field.FieldType.FullName}");

                // Properties
                foreach (var prop in type.Properties)
                    yield return (prop.PropertyType, $"property {prop.Name}: {prop.PropertyType.FullName}");

                // Methods — signatures + body
                foreach (var method in type.Methods)
                {
                    // Skip compiler-generated methods
                    if (method.Name.StartsWith("<") || method.Name.Contains("__"))
                        continue;

                    yield return (method.ReturnType, $"method {method.Name} return: {method.ReturnType.FullName}");

                    foreach (var param in method.Parameters)
                        yield return (param.ParameterType, $"method {method.Name} param {param.Name}: {param.ParameterType.FullName}");

                    if (!method.HasBody) continue;

                    // Local variables
                    foreach (var variable in method.Body.Variables)
                        yield return (variable.VariableType, $"method {method.Name} local: {variable.VariableType.FullName}");

                    // IL instructions — operand type references (newobj, castclass, isinst, ldtoken, etc.)
                    foreach (var instruction in method.Body.Instructions)
                    {
                        if (instruction.Operand is TypeReference tr)
                            yield return (tr, $"method {method.Name} IL {instruction.OpCode.Name}: {tr.FullName}");
                        else if (instruction.Operand is MethodReference mr)
                            yield return (mr.DeclaringType, $"method {method.Name} calls: {mr.FullName}");
                        else if (instruction.Operand is FieldReference fr)
                            yield return (fr.DeclaringType, $"method {method.Name} field access: {fr.FullName}");
                    }
                }

                // Generic parameters constraints
                foreach (var gp in type.GenericParameters)
                    foreach (var constraint in gp.Constraints)
                        yield return (constraint.ConstraintType, $"generic constraint: {constraint.ConstraintType.FullName}");

                // Custom attributes (skip Microsoft.CodeAnalysis and System.Diagnostics attributes which are compiler-injected)
                foreach (var attr in type.CustomAttributes)
                {
                    // Skip compiler-injected attributes
                    var attrName = attr.AttributeType.FullName ?? "";
                    if (attrName.StartsWith("System.Runtime.CompilerServices.") ||
                        attrName.StartsWith("System.Diagnostics.CodeAnalysis.") ||
                        attrName.StartsWith("Microsoft.CodeAnalysis."))
                        continue;

                    yield return (attr.AttributeType, $"attribute: {attr.AttributeType.FullName}");
                }
            }
        }
    }
}

namespace Catchy.Sdk
{
    public static class TypeHelper
    {
        private static readonly Dictionary<Type, string> _cache = new();

        public static string FriendlyName(Type type)
        {
            if (_cache.TryGetValue(type, out var cached))
                return cached;

            var result = FriendlyNameCore(type);
            _cache[type] = result;
            return result;
        }

        private static string FriendlyNameCore(Type type)
        {
            if (type.IsByRef)
                return "ref " + FriendlyName(type.GetElementType()!);

            if (type.IsArray)
                return FriendlyName(type.GetElementType()!) + "[]";

            if (type.IsGenericParameter)
                return type.Name;

            if (!type.IsGenericType)
            {
                if (type == typeof(void)) return "void";
                if (type == typeof(int)) return "int";
                if (type == typeof(long)) return "long";
                if (type == typeof(short)) return "short";
                if (type == typeof(byte)) return "byte";
                if (type == typeof(bool)) return "bool";
                if (type == typeof(string)) return "string";
                if (type == typeof(object)) return "object";
                if (type == typeof(char)) return "char";
                if (type == typeof(float)) return "float";
                if (type == typeof(double)) return "double";
                if (type == typeof(decimal)) return "decimal";

                return type.Name;
            }

            var def = type.GetGenericTypeDefinition();

            if (def == typeof(Nullable<>))
                return FriendlyName(type.GetGenericArguments()[0]) + "?";

            #if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER || NET5_0_OR_GREATER
            if (def == typeof(Span<>) || def == typeof(ReadOnlySpan<>))
            {
                var name = def.Name.StartsWith("ReadOnly") ? "ReadOnlySpan" : "Span";
                var arg = FriendlyName(type.GetGenericArguments()[0]);
                return $"{name}<{arg}>";
            }
            #endif

            if (def.FullName != null && def.FullName.StartsWith("System.ValueTuple"))
            {
                var args = type.GetGenericArguments().Select(FriendlyName);
                return $"({string.Join(", ", args)})";
            }

            var name2 = type.Name;
            var tickIndex = name2.IndexOf('`');
            if (tickIndex >= 0)
                name2 = name2.Substring(0, tickIndex);

            var args2 = string.Join(", ", type.GetGenericArguments().Select(FriendlyName));
            return $"{name2}<{args2}>";
        }
    }
}

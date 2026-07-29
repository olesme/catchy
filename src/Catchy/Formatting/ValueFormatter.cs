using System.Collections;
using System.Text;

namespace Catchy.Sdk
{
    public static class ValueFormatter
    {
        private const int MaxStringLength = 120;
        private const int MaxCollectionItems = 10;

        public static string Format(object? value, int depth = 0)
        {
            if (value is null) return "<null>";

            if (ValueFormatterRegistry.TryFormat(value, out var custom)) return custom;

            if (value is string s) return FormatString(s);
            if (value is char c) return $"'{EscapeChar(c)}'";
            if (value is bool b) return b ? "true" : "false";
            if (value is Guid g) return g.ToString();
            if (value is DateTime dt) return $"{dt:yyyy-MM-dd HH:mm:ss}";
            if (value is DateTimeOffset dto) return $"{dto:yyyy-MM-dd HH:mm:ss zzz}";
#if NET6_0_OR_GREATER
            if (value is DateOnly d) return d.ToString("yyyy-MM-dd");
            if (value is TimeOnly t) return t.ToString("HH:mm:ss");
#endif
            if (value is TimeSpan ts) return ts.ToString();
            if (value is Type tp) return tp.Name;
            if (value is Exception ex) return $"[{ex.GetType().Name}: {ex.Message}]";
            if (depth < 2 && value is IReadOnlyDictionary<string, object?> dict)
                return FormatDict(dict, depth);
            if (depth < 2 && value is not string && value is IEnumerable e)
                return FormatCollection(e, depth);

            // IFormattable covers all numeric types + DateTimeOffset + custom structs
            if (value is IFormattable fmt)
                return fmt.ToString(null, System.Globalization.CultureInfo.InvariantCulture);

            var str = value.ToString();
            if (str is null || str == value.GetType().ToString()) return $"<{value.GetType().Name}>";
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            return str.Length > MaxStringLength
                ? string.Concat(str.AsSpan(0, MaxStringLength), "…") : str;
#else
            return str.Length > MaxStringLength ? str.Substring(0, MaxStringLength) + "…" : str;
#endif
        }

        private static string FormatString(string s)
        {
            var sb = new StringBuilder("\"");
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            var span = s.Length > MaxStringLength ? s.AsSpan(0, MaxStringLength) : s.AsSpan();
            foreach (var ch in span)
#else
            var src = s.Length > MaxStringLength ? s.Substring(0, MaxStringLength) : s;
            foreach (var ch in src)
#endif
            {
                sb.Append(ch switch
                {
                    '"' => "\\\"",
                    '\\' => "\\\\",
                    '\n' => "\\n",
                    '\r' => "\\r",
                    '\t' => "\\t",
                    _ => ch.ToString()
                });
            }
            sb.Append('"');
            if (s.Length > MaxStringLength) sb.Append('…');
            return sb.ToString();
        }

        private static string FormatCollection(IEnumerable e, int depth)
        {
            var parts = new List<string>(); int count = 0; bool trunc = false;
            foreach (var item in e)
            {
                if (count++ >= MaxCollectionItems) { trunc = true; break; }
                parts.Add(Format(item, depth + 1));
            }
            var inner = string.Join(", ", parts);
            return trunc ? $"[{inner}, …]" : $"[{inner}]";
        }

        private static string FormatDict<V>(IReadOnlyDictionary<string, V> dict, int depth)
        {
            var parts = new List<string>(); int count = 0;
            foreach (var kv in dict)
            {
                if (count++ >= 5) { parts.Add("…"); break; }
                parts.Add($"{Format(kv.Key, depth + 1)}: {Format(kv.Value, depth + 1)}");
            }
            return "{" + string.Join(", ", parts) + "}";
        }

        private static string EscapeChar(char c) => c switch
        {
            '\'' => "\\'",
            '\\' => "\\\\",
            '\n' => "\\n",
            '\r' => "\\r",
            '\t' => "\\t",
            _ => c.ToString()
        };
    }
}

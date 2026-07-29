namespace Catchy.Sdk
{
    public static class ExprFormat
    {
        private static readonly HashSet<string> Literals = new(StringComparer.OrdinalIgnoreCase)
            { "null", "true", "false" };

        public static bool IsLiteral(string? expr)
        {
            if (expr is null) return false;
            if (Literals.Contains(expr)) return true;
            // numeric literal
            return double.TryParse(expr, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out _);
        }

        /// <summary>Format a value, showing the caller expression only if it's non-trivial.</summary>
        public static string Inline(object? value, string? expr = null)
        {
            var formatted = ValueFormatter.Format(value);
            if (expr is null || IsLiteral(expr) || expr == formatted) return formatted;
            // Don't repeat if expr is a simple string literal matching the value
            if (value is string s && (expr == $"\"{s}\"" || expr == $"@\"{s}\"")) return formatted;
            return $"{formatted} [{expr}]";
        }
    }
}

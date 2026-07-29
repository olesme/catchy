namespace Catchy
{
    public static class OrderingRuleRegistry
    {
        public static readonly Dictionary<Type, object> _rules = [];
        public static readonly object _lock = new();

        public static void Register<T>(IOrderingRule<T> rule, bool replace = true)
        {
            var key = typeof(T);
            lock (_lock)
            {
                if (!replace && _rules.ContainsKey(key)) return;
                _rules[key] = rule;
            }
        }

        public static bool TryGet<T>(out IOrderingRule<T>? rule)
        {
            lock (_lock)
            {
                if (_rules.TryGetValue(typeof(T), out var r))
                {
                    rule = (IOrderingRule<T>)r;
                    return true;
                }
            }
            rule = null;
            return false;
        }

        // Used by CollectionChecks.IsOrdered when no rule passed explicitly
        internal static IOrderingRule<T>? GetOrDefault<T>()
        {
            TryGet<T>(out var rule);
            return rule;
        }
    }
}

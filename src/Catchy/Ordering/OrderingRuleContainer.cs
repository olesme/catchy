namespace Catchy.Sdk
{
    /// <summary>
    /// Per-asserter collection of ordering rules.
    /// Lookup order: per-chain → this container → global <see cref="OrderingRuleRegistry"/>.
    /// </summary>
    public sealed class OrderingRuleContainer
    {
        private readonly Dictionary<Type, object> _rules = [];
        private readonly object _lock = new();

        public void Add<T>(IOrderingRule<T> rule, bool replace = true)
        {
            lock (_lock)
            {
                if (!replace && _rules.ContainsKey(typeof(T))) return;
                _rules[typeof(T)] = rule;
            }
        }

        public IOrderingRule<T>? TryGet<T>()
        {
            lock (_lock)
            {
                _rules.TryGetValue(typeof(T), out var r);
                return r as IOrderingRule<T>;
            }
        }

        /// <summary>Returns a new independent copy.</summary>
        public OrderingRuleContainer Clone()
        {
            var copy = new OrderingRuleContainer();
            lock (_lock)
                foreach (var kv in _rules)
                    copy._rules[kv.Key] = kv.Value;
            return copy;
        }
    }
}

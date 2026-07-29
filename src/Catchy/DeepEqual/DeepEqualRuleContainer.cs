namespace Catchy.Sdk
{
    /// <summary>
    /// Per-asserter collection of deep-equal rules.
    /// Lookup order: per-chain → this container → global <see cref="DeepEqualRuleRegistry"/>.
    /// </summary>
    public sealed class DeepEqualRuleContainer
    {
        private readonly Dictionary<(Type, Type), Func<object?, object?, bool>> _rules = [];
        private readonly object _lock = new();

        public void Add<TSource, TTarget>(DeepEqualRule<TSource, TTarget> rule, bool replace = true)
        {
            var compiled = rule.GetCompiled();
            Add<TSource, TTarget>((s, t) => compiled.AreEqual(s, t), replace);
        }

        public void Add<TSource, TTarget>(Func<TSource, TTarget, bool> compare, bool replace = true)
        {
            var key = (typeof(TSource), typeof(TTarget));
            lock (_lock)
            {
                if (!replace && _rules.ContainsKey(key)) return;
                _rules[key] = (a, b) => a is TSource s && b is TTarget t && compare(s, t);
            }
        }

        public void RegisterRule<TSource, TTarget>(DeepEqualRule<TSource, TTarget> rule, bool replace = true)
            => Add(rule, replace);

        public void RegisterRule<TSource, TTarget>(Func<TSource, TTarget, bool> compare, bool replace = true)
            => Add(compare, replace);

        public void RegisterRule(IDeepEqualRule rule, bool replace = true)
        {
            // Store the rule using reflection to get the generic type parameters
            if (rule is null) return;

            var ruleType = rule.GetType();

            // Check if ruleType itself is DeepEqualRule<TSource, TTarget>
            if (ruleType.IsGenericType && ruleType.GetGenericTypeDefinition() == typeof(DeepEqualRule<,>))
            {
                var args = ruleType.GetGenericArguments();
                var method = typeof(DeepEqualRuleContainer)
                    .GetMethod("RegisterRule", [ruleType, typeof(bool)]);
                if (method != null)
                    method.Invoke(this, [rule, replace]);
                return;
            }

            // Find the DeepEqualRule<TSource, TTarget> base type if ruleType is derived from it
            var baseType = ruleType.BaseType;
            while (baseType != null && !baseType.IsGenericType)
                baseType = baseType.BaseType;

            if (baseType?.GetGenericTypeDefinition() == typeof(DeepEqualRule<,>))
            {
                var args = baseType.GetGenericArguments();
                var method = typeof(DeepEqualRuleContainer)
                    .GetMethod("RegisterRule", [baseType, typeof(bool)]);
                if (method != null)
                    method.Invoke(this, [rule, replace]);
            }
        }

        public bool TryCompare(object? a, object? b, out bool result)
        {
            result = false;
            if (a is null || b is null) return false;
            lock (_lock)
            {
                if (_rules.TryGetValue((a.GetType(), b.GetType()), out var fn))
                { result = fn(a, b); return true; }
            }
            return false;
        }

        public void MergeFrom(DeepEqualRuleContainer? other, bool replace = true)
        {
            if (other is null || ReferenceEquals(this, other)) return;

            var snapshot = other.Snapshot();
            lock (_lock)
            {
                foreach (var kv in snapshot)
                {
                    if (!replace && _rules.ContainsKey(kv.Key)) continue;
                    _rules[kv.Key] = kv.Value;
                }
            }
        }

        private Dictionary<(Type, Type), Func<object?, object?, bool>> Snapshot()
        {
            lock (_lock)
            {
                return new Dictionary<(Type, Type), Func<object?, object?, bool>>(_rules);
            }
        }

        /// <summary>Returns a new independent copy.</summary>
        public DeepEqualRuleContainer Clone()
        {
            var copy = new DeepEqualRuleContainer();
            lock (_lock)
                foreach (var kv in _rules)
                    copy._rules[kv.Key] = kv.Value;
            return copy;
        }
    }
}

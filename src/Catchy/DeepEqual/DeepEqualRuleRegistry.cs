namespace Catchy.Sdk
{
    public static class DeepEqualRuleRegistry
    {
        private static readonly Dictionary<(Type, Type), Func<object?, object?, bool>> _rules = [];
        private static readonly object _lock = new();

        public static void Register<TSource, TTarget>(
            Func<TSource, TTarget, bool> compare, bool replace = true)
        {
            var key = (typeof(TSource), typeof(TTarget));
            lock (_lock)
            {
                if (!replace && _rules.ContainsKey(key)) return;
                _rules[key] = (a, b) => a is TSource s && b is TTarget t && compare(s, t);
            }
        }

        /// <summary>Returns true if a rule is already registered for the given type pair.</summary>
        public static bool HasRule<TSource, TTarget>()
        {
            lock (_lock) return _rules.ContainsKey((typeof(TSource), typeof(TTarget)));
        }

        public static bool TryCompare(object? a, object? b, out bool result)
        {
            result = false;
            if (a is null || b is null) return false;
            lock (_lock)
            {
                if (_rules.TryGetValue((a.GetType(), b.GetType()), out var fn))
                {
                    result = fn(a, b);
                    return true;
                }
            }
            return false;
        }
    }
}

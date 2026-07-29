namespace Catchy.Sdk
{
    /// <summary>
    /// Allows registration of custom string formatters for user types used in failure messages.
    /// Register once at test-suite startup (e.g. in [BeforeTestRun]).
    /// </summary>
    public static class ValueFormatterRegistry
    {
        private static readonly Dictionary<Type, Func<object, string>> _formatters = [];
        private static readonly object _lock = new();

        public static void Register<T>(Func<T, string> formatter)
        {
            lock (_lock) _formatters[typeof(T)] = obj => formatter((T)obj);
        }

        internal static bool TryFormat(object value, out string result)
        {
            lock (_lock)
            {
                if (_formatters.TryGetValue(value.GetType(), out var fn))
                { result = fn(value); return true; }
            }
            result = null!;
            return false;
        }
    }
}

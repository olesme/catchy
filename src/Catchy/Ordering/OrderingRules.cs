namespace Catchy
{

    public static class OrderingRules
    {
        // Generic
        public static IOrderingRule<T> Ascending<T>() where T : IComparable<T>
            => AscendingRule<T>.Instance;

        public static IOrderingRule<T> Descending<T>() where T : IComparable<T>
            => DescendingRule<T>.Instance;

        // String-specific
        public static IOrderingRule<string> AlphaAscending() => StringRules.OrdinalAscending;
        public static IOrderingRule<string> AlphaDescending() => StringRules.OrdinalDescending;
        public static IOrderingRule<string> AlphaAscendingIgnoreCase() => StringRules.OrdinalIgnoreCaseAscending;
        public static IOrderingRule<string> AlphaDescendingIgnoreCase() => StringRules.OrdinalIgnoreCaseDescending;
        public static IOrderingRule<string> ByLengthAscending() => StringRules.LengthAscending;
        public static IOrderingRule<string> ByLengthDescending() => StringRules.LengthDescending;

        // Custom
        public static IOrderingRule<T> From<T>(Comparison<T> comparison) => new DelegateRule<T>(comparison);
        public static IOrderingRule<T> From<T>(IComparer<T> comparer) => new ComparerRule<T>(comparer);

        // Implementations

        private sealed class AscendingRule<T> : IOrderingRule<T>, IOrderingDirectionProvider where T : IComparable<T>
        {
            public static readonly AscendingRule<T> Instance = new();
            public OrderingDirection Direction => OrderingDirection.Ascending;
            public int Compare(T x, T y) => x.CompareTo(y);
        }

        private sealed class DescendingRule<T> : IOrderingRule<T>, IOrderingDirectionProvider where T : IComparable<T>
        {
            public static readonly DescendingRule<T> Instance = new();
            public OrderingDirection Direction => OrderingDirection.Descending;
            public int Compare(T x, T y) => y.CompareTo(x);
        }

        private sealed class DelegateRule<T>(Comparison<T> comparison, OrderingDirection direction = OrderingDirection.Unknown)
            : IOrderingRule<T>, IOrderingDirectionProvider
        {
            public OrderingDirection Direction { get; } = direction;
            public int Compare(T x, T y) => comparison(x, y);
        }

        private sealed class ComparerRule<T>(IComparer<T> comparer, OrderingDirection direction = OrderingDirection.Unknown)
            : IOrderingRule<T>, IOrderingDirectionProvider
        {
            public OrderingDirection Direction { get; } = direction;
            public int Compare(T x, T y) => comparer.Compare(x, y);
        }

        internal static class StringRules
        {
            public static readonly IOrderingRule<string> OrdinalAscending =
                new ComparerRule<string>(StringComparer.Ordinal, OrderingDirection.Ascending);
            public static readonly IOrderingRule<string> OrdinalDescending =
                new ComparerRule<string>(Comparer<string>.Create((a, b) => StringComparer.Ordinal.Compare(b, a)), OrderingDirection.Descending);
            public static readonly IOrderingRule<string> OrdinalIgnoreCaseAscending =
                new ComparerRule<string>(StringComparer.OrdinalIgnoreCase, OrderingDirection.Ascending);
            public static readonly IOrderingRule<string> OrdinalIgnoreCaseDescending =
                new ComparerRule<string>(Comparer<string>.Create((a, b) => StringComparer.OrdinalIgnoreCase.Compare(b, a)), OrderingDirection.Descending);
            public static readonly IOrderingRule<string> LengthAscending =
                new DelegateRule<string>((a, b) => (a?.Length ?? 0).CompareTo(b?.Length ?? 0), OrderingDirection.Ascending);
            public static readonly IOrderingRule<string> LengthDescending =
                new DelegateRule<string>((a, b) => (b?.Length ?? 0).CompareTo(a?.Length ?? 0), OrderingDirection.Descending);
        }
    }

    public static class OrderingRulesExtensions
    {
        public static void Register<T>(this IOrderingRule<T> rule, bool replace = true)
            => OrderingRuleRegistry.Register(rule, replace);
    }
}

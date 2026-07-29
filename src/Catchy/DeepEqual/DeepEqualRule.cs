using System.Linq.Expressions;
using Catchy.Sdk;

namespace Catchy
{
    public interface IDeepEqualRule
    {
        bool AreEqualObjects(object? a, object? b);
    }

    public sealed class CompiledDeepEqualRule<TSource, TTarget>
    {
        private readonly Func<TSource, TTarget, bool> _areEqual;
        private readonly Func<TSource, TTarget, IEnumerable<string>> _differences;

        internal CompiledDeepEqualRule(
            Func<TSource, TTarget, bool> areEqual,
            Func<TSource, TTarget, IEnumerable<string>> differences)
        {
            _areEqual = areEqual;
            _differences = differences;
        }

        public bool AreEqual(TSource actual, TTarget expected) => _areEqual(actual, expected);
        public IEnumerable<string> Differences(TSource actual, TTarget expected) => _differences(actual, expected);
    }

    public sealed class DeepEqualRule<TSource, TTarget> : IDeepEqualRule
    {
        private readonly IReadOnlyList<ProjectionEntry> _projections;
        private readonly IReadOnlyList<string> _excludedProperties;
        private readonly bool _forceReflection;
        private readonly bool _disableReflection;

        // Lazy compile — built once per instance, safe because instances are immutable.
        private readonly Lazy<CompiledDeepEqualRule<TSource, TTarget>> _compiled;

        public DeepEqualRule()
            : this([], [], forceReflection: false, disableReflection: false) { }

        internal DeepEqualRule(bool disableReflection)
            : this([], [], forceReflection: false, disableReflection: disableReflection) { }

        private DeepEqualRule(
            IReadOnlyList<ProjectionEntry> projections,
            IReadOnlyList<string> excluded,
            bool forceReflection,
            bool disableReflection)
        {
            _projections = projections;
            _excludedProperties = excluded;
            _forceReflection = forceReflection;
            _disableReflection = disableReflection;
            _compiled = new(BuildRuleCore);
        }

        /// <summary>
        /// Forces reflection-based auto-match even when explicit Match calls exist.
        /// </summary>
        public DeepEqualRule<TSource, TTarget> WithAutoMatch()
            => new(_projections, _excludedProperties, forceReflection: true, _disableReflection);

        public DeepEqualRule<TSource, TTarget> Exclude<TProp>(Expression<Func<TSource, TProp>> expr)
        {
            var name = (expr.Body as MemberExpression)?.Member.Name
                ?? throw new ArgumentException("Must be a member expression", nameof(expr));
            return new(_projections, [.._excludedProperties, name], _forceReflection, _disableReflection);
        }

        public DeepEqualRule<TSource, TTarget> Match<TProp>(
            Func<TSource, TProp> left,
            Func<TTarget, TProp> right,
            IEqualityComparer<TProp>? comparer = null,
            string? name = null)
        {
            Func<object?, object?, bool>? cmp = comparer is null ? null :
                (a, b) => a is TProp pa && b is TProp pb && comparer.Equals(pa, pb);
            var entry = new ProjectionEntry(name ?? "field", x => left(x), y => right(y), cmp);
            return new([.._projections, entry], _excludedProperties, _forceReflection, _disableReflection);
        }

        public DeepEqualRule<TSource, TTarget> Match<TProp>(
            Expression<Func<TSource, TProp>> leftExpr,
            Expression<Func<TTarget, TProp>> rightExpr)
        {
            var name = (leftExpr.Body as MemberExpression)?.Member.Name ?? "field";
            return Match(leftExpr.Compile(), rightExpr.Compile(), null, name);
        }

        /// <summary>
        /// Returns the compiled rule. The result is cached — safe to call repeatedly.
        /// </summary>
        public CompiledDeepEqualRule<TSource, TTarget> GetCompiled() => _compiled.Value;

        private CompiledDeepEqualRule<TSource, TTarget> BuildRuleCore()
        {
            bool useReflection = !_disableReflection && (_forceReflection || _projections.Count == 0);

            if (useReflection)
            {
                var opts = new EqualsOptions();
                foreach (var p in _excludedProperties) opts.ExcludedProperties.Add(p);
                return new CompiledDeepEqualRule<TSource, TTarget>(
                    (a, e) => DeepEqualEngine.AreEqualSkippingRegistry(a, e, opts),
                    (a, e) => DeepEqualEngine.GetDiffs(a, e, opts));
            }

            var projections = _projections.ToArray(); // snapshot
            return new CompiledDeepEqualRule<TSource, TTarget>(
                (a, e) => projections.All(p =>
                    p.Comparer is not null
                        ? p.Comparer(p.Left(a), p.Right(e))
                        : Equals(p.Left(a), p.Right(e))),
                (a, e) =>
                {
                    var diffs = new List<string>();
                    foreach (var p in projections)
                    {
                        var lv = p.Left(a);
                        var rv = p.Right(e);
                        bool eq = p.Comparer is not null ? p.Comparer(lv, rv) : Equals(lv, rv);
                        if (!eq)
                            diffs.Add($"  {p.Name}: {ValueFormatter.Format(lv)} != {ValueFormatter.Format(rv)}");
                    }
                    return diffs;
                });
        }

        /// <summary>
        /// Registers this rule in the global registry so that
        /// <see cref="DeepEqualEngine"/> picks it up automatically for all
        /// <c>IsEquivalentTo</c> calls between <typeparamref name="TSource"/>
        /// and <typeparamref name="TTarget"/>.
        ///
        /// <para>
        /// Intended for project-level setup (e.g. in <c>[BeforeTestRun]</c> hooks
        /// or application startup). For per-assertion overrides use the rule
        /// directly: <c>.IsEquivalentTo(expected, rule)</c>.
        /// </para>
        ///
        /// <para>
        /// Passing <paramref name="replace"/> = <see langword="false"/> (the default
        /// when called a second time) throws <see cref="InvalidOperationException"/>
        /// if a rule is already registered, helping catch accidental double-registration
        /// in tests that forget cleanup.
        /// </para>
        /// </summary>
        public DeepEqualRule<TSource, TTarget> Register()
        {
            if (DeepEqualRuleRegistry.HasRule<TSource, TTarget>())
                throw new InvalidOperationException(
                    $"Rule for ({typeof(TSource).Name} → {typeof(TTarget).Name}) already registered. " +
                    "Register once in BeforeTestRun. For per-test overrides use .IsEquivalentTo(expected, rule).");

            var compiled = _compiled.Value;
            DeepEqualRuleRegistry.Register<TSource, TTarget>((s, t) => compiled.AreEqual(s, t));
            return this;
        }

        bool IDeepEqualRule.AreEqualObjects(object? a, object? b)
            => a is TSource src && b is TTarget tgt && _compiled.Value.AreEqual(src, tgt);

        private sealed record ProjectionEntry(
            string Name,
            Func<TSource, object?> Left,
            Func<TTarget, object?> Right,
            Func<object?, object?, bool>? Comparer);
    }

    public static class DeepEqualRule
    {
        /// <param name="disableReflection">
        /// When true, only explicit Match() calls are used — reflection fallback is disabled.
        /// </param>
        public static DeepEqualRule<T, T> For<T>(bool disableReflection = false) => new(disableReflection);

        public static DeepEqualRule<TSource, TTarget> For<TSource, TTarget>(bool disableReflection = false)
            => new(disableReflection);
    }
}

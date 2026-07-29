namespace Catchy.Sdk
{
    public static class CollectionChecks
    {
        static string Fmt(object? v, string? e = null) => ExprFormat.Inline(v, e);
        static string FmtItems<T>(IEnumerable<T> items, int max = 10)
        {
            var parts = new List<string>(); int count = 0;
            foreach (var x in items) { if (count++ >= max) { parts.Add("…"); break; } parts.Add(ValueFormatter.Format(x)); }
            return string.Join(", ", parts);
        }
        static EqualityComparer<T> Eq<T>() => EqualityComparer<T>.Default;

        public static CheckOperation IsEmpty<T>(IEnumerable<T>? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && !actual.Any(),
                () => actual is null ? "Expected a collection, but was null" : $"Expected empty collection, but had {actual.Count()} item(s)",
                isSkipped);

        public static CheckOperation IsNotEmpty<T>(IEnumerable<T>? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.Any(),
                () => actual is null ? "Expected a collection, but was null" : "Expected non-empty collection, but was empty",
                isSkipped);

        public static CheckOperation HasCount<T>(IEnumerable<T>? actual, int expected, bool isSkipped)
            => CheckOperation.Sync(
                () => actual?.Count() == expected,
                () => actual is null ? "Expected a collection, but was null" : $"Expected {expected} item(s), but had {actual.Count()}",
                isSkipped);

        public static CheckOperation HasCountGreaterThan<T>(IEnumerable<T>? actual, int count, bool isSkipped)
            => CheckOperation.Sync(
                () => actual?.Count() > count,
                () => actual is null ? "Expected a collection, but was null" : $"Expected count > {count}, but was {actual.Count()}",
                isSkipped);

        public static CheckOperation HasCountLessThan<T>(IEnumerable<T>? actual, int count, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.Count() < count,
                () => actual is null ? "Expected a collection, but was null" : $"Expected count < {count}, but was {actual.Count()}",
                isSkipped);

        public static CheckOperation HasCountAtLeast<T>(IEnumerable<T>? actual, int min, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.Count() >= min,
                () => actual is null ? "Expected a collection, but was null"
                    : $"Expected count at least {min}, but was {actual.Count()}",
                isSkipped);

        public static CheckOperation HasCountAtMost<T>(IEnumerable<T>? actual, int max, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.Count() <= max,
                () => actual is null ? "Expected a collection, but was null"
                    : $"Expected count at most {max}, but was {actual.Count()}",
                isSkipped);

        public static CheckOperation HasCountInRange<T>(IEnumerable<T>? actual, int min, int max,
            string? minExpr, string? maxExpr, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is { } c && c.Count() >= min && c.Count() <= max,
                () => actual is null ? "Expected a collection, but was null"
                    : $"Expected count in [{ExprFormat.Inline(min, minExpr)}, {ExprFormat.Inline(max, maxExpr)}], but was {actual.Count()}",
                isSkipped);

        public static CheckOperation HasCountGreaterThanOrEqualTo<T>(IEnumerable<T>? actual, int min,
            string? expr, bool isSkipped)
            => CheckOperation.Sync(
                () => actual?.Count() >= min,
                () => actual is null ? "Expected a collection, but was null"
                    : $"Expected count >= {ExprFormat.Inline(min, expr)}, but was {actual.Count()}",
                isSkipped);

        public static CheckOperation HasCountLessThanOrEqualTo<T>(IEnumerable<T>? actual, int max,
            string? expr, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.Count() <= max,
                () => actual is null ? "Expected a collection, but was null"
                    : $"Expected count <= {ExprFormat.Inline(max, expr)}, but was {actual.Count()}",
                isSkipped);

        public static CheckOperation HasSameCountAs<T>(IEnumerable<T>? actual, int otherCount, bool isSkipped)
            => CheckOperation.Sync(
                () => actual?.Count() == otherCount,
                () => actual is null ? "Expected a collection, but was null"
                    : $"Expected count to equal {otherCount}, but was {actual.Count()}",
                isSkipped);
        public static bool TryFindFirstMatch<T>(IEnumerable<T>? actual, Func<T, bool> predicate, out T? item)
        {
            item = default;
            if (actual is null)
            {
                return false;
            }

            foreach (var candidate in actual)
            {
                if (!predicate(candidate))
                {
                    continue;
                }

                item = candidate;
                return true;
            }

            return false;
        }

        public static bool TryFindLastMatch<T>(IEnumerable<T>? actual, Func<T, bool> predicate, out T? item)
        {
            item = default;
            if (actual is null)
            {
                return false;
            }

            for (var i = actual.Count() - 1; i >= 0; i--)
            {
                var candidate = actual.ElementAt(i);
                if (!predicate(candidate))
                {
                    continue;
                }

                item = candidate;
                return true;
            }

            return false;
        }

        public static CheckOperation HasMatch<T>(IEnumerable<T>? actual, bool foundAny, string failurePrefix, string? predicateExpr, bool isSkipped)
            => CheckOperation.Sync(
                () => foundAny,
                () => actual is null
                    ? "Expected a collection, but was null"
                    : $"{failurePrefix} {predicateExpr ?? "<predicate>"}",
                isSkipped);

        public static CheckOperation Contains<T>(IEnumerable<T>? actual, T expected, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.Any(x => Eq<T>().Equals(x, expected)),
                () => actual is null ? "Expected a collection, but was null" : $"Expected collection to contain {Fmt(expected)}",
                isSkipped);

        public static CheckOperation DoesNotContain<T>(IEnumerable<T>? actual, T item, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null || !actual.Any(x => Eq<T>().Equals(x, item)),
                () => $"Expected collection not to contain {Fmt(item)}",
                isSkipped);

        public static CheckOperation ContainsAll<T>(IEnumerable<T>? actual, IEnumerable<T> expected, bool isSkipped)
        {
            var expList = expected is IEnumerable<T> r ? r : [.. expected];
            return CheckOperation.Sync(
                () => actual is not null && expList.All(e => actual.Any(x => Eq<T>().Equals(x, e))),
                () =>
                {
                    if (actual is null) return "Expected a collection, but was null";
                    var missing = expList.Where(e => !actual.Any(x => Eq<T>().Equals(x, e))).ToList();
                    return $"Expected collection to contain all of [{FmtItems(expList)}], missing: [{FmtItems(missing)}]";
                },
                isSkipped);
        }

        public static CheckOperation ContainsAny<T>(IEnumerable<T>? actual, IEnumerable<T> expected, bool isSkipped)
        {
            var expList = expected is IEnumerable<T> r ? r : [.. expected];
            return CheckOperation.Sync(
                () => actual is not null && expList.Any(e => actual.Any(x => Eq<T>().Equals(x, e))),
                () => actual is null ? "Expected a collection, but was null" : $"Expected collection to contain any of [{FmtItems(expList)}]",
                isSkipped);
        }

        public static CheckOperation ContainsEquivalentOf<T>(IEnumerable<T>? actual, T item,
            EqualsOptions opts, string? expr, bool isSkipped)
            => CheckOperation.Sync(
                () => actual?.Any(x => DeepEqualEngine.AreEqualObjects(x, item, opts)) == true,
                () => actual is null ? "Expected a collection, but was null"
                    : $"Expected collection to contain an item equivalent to {ExprFormat.Inline(item, expr)}",
                isSkipped);

        public static CheckOperation HasDistinctItems<T>(IEnumerable<T>? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.Distinct().Count() == actual.Count(),
                () => actual is null ? "Expected a collection, but was null" : "Expected collection to have distinct items",
                isSkipped);

        public static CheckOperation HasDistinctItemsBy<T, TKey>(IEnumerable<T>? actual,
            Func<T, TKey> keySelector, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && actual.Select(keySelector).Distinct().Count() == actual.Count(),
                () => actual is null ? "Expected a collection, but was null"
                    : "Expected collection to have distinct items by key, but found duplicates",
                isSkipped);
        public static CheckOperation HasNullItems<T>(IEnumerable<T>? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual?.Any(x => x is null) == true,
                () => actual is null ? "Expected a collection, but was null"
                    : "Expected collection to contain at least one null item",
                isSkipped);

        public static CheckOperation HasNoNullItems<T>(IEnumerable<T>? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is { } col && col.All(x => x is not null),
                () => actual is null ? "Expected a collection, but was null"
                    : "Expected collection to contain no null items, but found at least one",
                isSkipped);

        public static CheckOperation IntersectsWith<T>(IEnumerable<T>? actual, IEnumerable<T> other, bool isSkipped)
            => CheckOperation.Sync(
                () => actual?.Any(x => other.Any(o => Eq<T>().Equals(x, o))) == true,
                () => actual is null ? "Expected a collection, but was null"
                    : "Expected collections to intersect, but no common elements found",
                isSkipped);

        public static CheckOperation ContainsItemsAssignableTo<T, TTarget>(IEnumerable<T>? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null && !actual.Any(x => x is not null && x is not TTarget),
                () =>
                {
                    if (actual is null) return "Expected a collection, but was null";
                    var wrong = actual
                        .Select((x, i) => (x, i))
                        .Where(t => t.x is not null && t.x is not TTarget)
                        .Take(5)
                        .Select(t => $"[{t.i}] {ValueFormatter.Format(t.x)}")
                        .ToList();
                    return $"Expected all items to be assignable to {TypeHelper.FriendlyName(typeof(TTarget))}, " +
                           $"but some were not: {string.Join(", ", wrong)}";
                },
                isSkipped);

        public static CheckOperation IsOrderedBy<T, TKey>(IEnumerable<T>? actual,
            Func<T, TKey> keySelector, bool isSkipped)
            => CheckOperation.Sync(
                () =>
                {
                    if (actual is null) return false;
                    if (actual.Count() <= 1) return true;
                    var cmp = Comparer<TKey>.Default;
                    for (int i = 1; i < actual.Count(); i++)
                        if (cmp.Compare(keySelector(actual.ElementAt(i - 1)), keySelector(actual.ElementAt(i))) > 0) return false;
                    return true;
                },
                () => actual is null ? "Expected a collection, but was null"
                    : "Expected collection to be ordered ascending by key",
                isSkipped);

        public static CheckOperation IsOrderedDescendingBy<T, TKey>(IEnumerable<T>? actual,
            Func<T, TKey> keySelector, bool isSkipped)
            => CheckOperation.Sync(
                () =>
                {
                    if (actual is null) return false;
                    if (actual.Count() <= 1) return true;
                    var cmp = Comparer<TKey>.Default;
                    for (int i = 1; i < actual.Count(); i++)
                        if (cmp.Compare(keySelector(actual.ElementAt(i - 1)), keySelector(actual.ElementAt(i))) < 0) return false;
                    return true;
                },
                () => actual is null ? "Expected a collection, but was null"
                    : "Expected collection to be ordered descending by key",
                isSkipped);

        public static CheckOperation SatisfyRespectively<T>(IEnumerable<T>? actual,
            Action<T>[] inspectors, bool isSkipped)
            => CheckOperation.Sync(
                () =>
                {
                    if (actual is null || actual.Count() != inspectors.Length) return false;
                    for (int i = 0; i < inspectors.Length; i++)
                        try { inspectors[i](actual.ElementAt(i)); }
                        catch (AssertionException) { return false; }
                    return true;
                },
                () =>
                {
                    if (actual is null) return "Expected a collection, but was null";
                    if (actual.Count() != inspectors.Length)
                        return $"Expected {inspectors.Length} items for respective inspection, but had {actual.Count()}";
                    var errors = new List<string>();
                    for (int i = 0; i < inspectors.Length; i++)
                        try { inspectors[i](actual.ElementAt(i)); }
                        catch (AssertionException ex) { errors.Add($"[{i}] {ex.Body ?? ex.Message}"); }
                    return $"Collection failed {errors.Count} respective inspection(s):\n{string.Join("\n", errors)}";
                },
                isSkipped);

        public static CheckOperation AllSatisfy<T>(
            IEnumerable<T>? actual, Func<T, bool> predicate,
            bool isSkipped, int parallelThreshold = int.MaxValue, string? predicateExpr = null)
        {
            string failMsg() => actual is null
                ? "Expected a collection, but was null"
                : $"Expected all items to satisfy: {predicateExpr ?? "<predicate>"}";

            if (actual is not null && actual.Count() >= parallelThreshold)
                return CheckOperation.Async(async () =>
                {
                    var results = await Task.WhenAll(actual.Select(item => Task.Run(() => predicate(item))))
                        .ConfigureAwait(false);
                    return results.All(r => r);
                }, failMsg, isSkipped);

            return CheckOperation.Sync(() => actual is not null && actual.All(predicate), failMsg, isSkipped);
        }

        public static CheckOperation AnySatisfy<T>(
            IEnumerable<T>? actual, Func<T, bool> predicate,
            bool isSkipped, int parallelThreshold = int.MaxValue, string? predicateExpr = null)
        {
            string failMsg() => actual is null
                ? "Expected a collection, but was null"
                : $"Expected at least one item to satisfy: {predicateExpr ?? "<predicate>"}";

            if (actual is not null && actual.Count() >= parallelThreshold)
                return CheckOperation.Async(async () =>
                {
                    var results = await Task.WhenAll(actual.Select(item => Task.Run(() => predicate(item))))
                        .ConfigureAwait(false);
                    return results.Any(r => r);
                }, failMsg, isSkipped);

            return CheckOperation.Sync(() => actual is not null && actual.Any(predicate), failMsg, isSkipped);
        }

        public static CheckOperation NoneSatisfy<T>(
            IEnumerable<T>? actual, Func<T, bool> predicate,
            bool isSkipped, int parallelThreshold = int.MaxValue, string? predicateExpr = null)
        {
            var failMsg = () => actual is null
                ? "Expected a collection, but was null"
                : $"Expected no items to satisfy: {predicateExpr ?? "<predicate>"}";

            if (actual is not null && actual.Count() >= parallelThreshold)
                return CheckOperation.Async(async () =>
                {
                    var results = await Task.WhenAll(actual.Select(item => Task.Run(() => predicate(item))))
                        .ConfigureAwait(false);
                    return results.All(r => !r);
                }, failMsg, isSkipped);

            return CheckOperation.Sync(() => actual is not null && !actual.Any(predicate), failMsg, isSkipped);
        }

        public static CheckOperation IsEquivalentTo<T>(IEnumerable<T>? actual, IEnumerable<T> expected, bool isSkipped)
            => IsEquivalentTo(
                actual,
                expected,
                () => new EqualsOptions(),
                () => null,
                isSkipped);

        public static CheckOperation IsEquivalentTo<T>(
            IEnumerable<T>? actual,
            IEnumerable<T> expected,
            Func<EqualsOptions> getOptions,
            Func<DeepEqualRuleContainer?> getLocalRules,
            bool isSkipped)
        {
            var expList = expected is IEnumerable<T> r ? r : [.. expected];
            return CheckOperation.Sync(
                () =>
                {
                    if (actual is null || actual.Count() != expList.Count()) return false;
                    var opts = getOptions();
                    var localRules = getLocalRules();
                    var rem = new List<T>(expList);
                    foreach (var x in actual)
                    {
                        int idx = rem.FindIndex(e => DeepEqualEngine.AreEqualObjects(x, e, opts, localRules));
                        if (idx < 0) return false;
                        rem.RemoveAt(idx);
                    }
                    return rem.Count == 0;
                },
                () => actual is null ? "Expected a collection, but was null" : $"Expected collection to be equivalent to [{FmtItems(expList)}]",
                isSkipped);
        }

        public static CheckOperation IsNotEquivalentTo<T>(
            IEnumerable<T>? actual,
            IEnumerable<T> expected,
            Func<EqualsOptions> getOptions,
            Func<DeepEqualRuleContainer?> getLocalRules,
            bool isSkipped)
        {
            var expList = expected is IEnumerable<T> r ? r : [.. expected];
            return CheckOperation.Sync(
                () =>
                {
                    if (actual is null) return expList.Any();
                    if (actual.Count() != expList.Count()) return true;
                    var opts = getOptions();
                    var localRules = getLocalRules();
                    var rem = new List<T>(expList);
                    foreach (var x in actual)
                    {
                        int idx = rem.FindIndex(e => DeepEqualEngine.AreEqualObjects(x, e, opts, localRules));
                        if (idx < 0) return true;
                        rem.RemoveAt(idx);
                    }
                    return rem.Count != 0;
                },
                () => $"Expected collection not to be equivalent to [{FmtItems(expList)}], but it was",
                isSkipped);
        }

        public static CheckOperation IsEquivalentToWithRule<T>(IEnumerable<T>? actual, IEnumerable<T> expList,
            IDeepEqualRule rule, bool isSkipped)
            => CheckOperation.Sync(
                () =>
                {
                    if (actual is null || actual.Count() != expList.Count()) return false;
                    var rem = new List<T>(expList);
                    foreach (var x in actual)
                    {
                        int idx = rem.FindIndex(e => rule.AreEqualObjects(x, e));
                        if (idx < 0) return false;
                        rem.RemoveAt(idx);
                    }
                    return rem.Count == 0;
                },
                () => actual is null ? "Expected a collection, but was null" : $"Expected collection to be equivalent to [{FmtItems(expList)}] (by rule)",
                isSkipped);

        public static CheckOperation IsSequenceEqualTo<T>(IEnumerable<T>? actual, IEnumerable<T> expected, bool isSkipped)
        {
            var expList = expected is IEnumerable<T> r ? r : expected.ToList();
            return CheckOperation.Sync(
                () => actual is not null && actual.Count() == expList.Count()
                    && !actual.Where((x, i) => !Eq<T>().Equals(x, expList.ElementAt(i))).Any(),
                () => actual is null ? "Expected a collection, but was null" : $"Expected collection to equal [{FmtItems(expList)}] (same order)",
                isSkipped);
        }

        public static bool IsOrderedCheck<T>(IEnumerable<T>? actual, IOrderingRule<T>? rule)
        {
            if (actual is null || actual.Count() <= 1) return actual is not null;

            var hasViolation = Enumerable.Range(1, actual.Count() - 1).Any(i =>
            {
                int cmp = rule is not null
                    ? rule.Compare(actual.ElementAt(i - 1), actual.ElementAt(i))
                    : Comparer<T>.Default.Compare(actual.ElementAt(i - 1), actual.ElementAt(i));
                return cmp > 0;
            });

            return !hasViolation;
        }

        public static CheckOperation IsOrdered<T>(IEnumerable<T>? actual, IOrderingRule<T>? rule, bool isSkipped)
            => CheckOperation.Sync(
                () => IsOrderedCheck(actual, rule ?? OrderingRuleRegistry.GetOrDefault<T>()),
                () =>
                {
                    var label = GetExpectedOrderLabel(rule ?? OrderingRuleRegistry.GetOrDefault<T>());
                    return actual is null ? "Expected a collection, but was null" : $"Expected collection to be ordered in {label}";
                },
                isSkipped);

        public static CheckOperation IsOrderedDescending<T>(IEnumerable<T>? actual, IOrderingRule<T>? rule, bool isSkipped)
        {
            var effectiveRule = rule ?? OrderingRuleRegistry.GetOrDefault<T>();
            return CheckOperation.Sync(
                () =>
                {
                    if (actual is null) return false;
                    if (actual.Count() <= 1) return true;
                    return !Enumerable.Range(1, actual.Count() - 1).Any(i =>
                    {
                        int cmp = effectiveRule is not null
                            ? effectiveRule.Compare(actual.ElementAt(i - 1), actual.ElementAt(i))
                            : Comparer<T>.Default.Compare(actual.ElementAt(i - 1), actual.ElementAt(i));
                        return cmp < 0;
                    });
                },
                () => actual is null ? "Expected a collection, but was null" : "Expected collection to be ordered descending",
                isSkipped);
        }

        // Lazy overloads that resolve the rule at execution time to support cascade lookup and trailing modifiers
        public static CheckOperation IsOrdered<T>(IEnumerable<T>? actual, Func<IOrderingRule<T>?> ruleResolver, bool isSkipped)
        {
            IOrderingRule<T>? resolvedRule = null;
            return CheckOperation.Sync(
                () =>
                {
                    resolvedRule = ruleResolver() ?? OrderingRuleRegistry.GetOrDefault<T>();
                    return IsOrderedCheck(actual, resolvedRule);
                },
                () =>
                {
                    var label = GetExpectedOrderLabel(resolvedRule ?? ruleResolver() ?? OrderingRuleRegistry.GetOrDefault<T>());
                    return actual is null
                        ? "Expected a collection, but was null"
                        : $"Expected collection to be ordered in {label}";
                },
                isSkipped);
        }

        public static CheckOperation IsOrderedDescending<T>(IEnumerable<T>? actual, Func<IOrderingRule<T>?> ruleResolver, bool isSkipped)
        {
            return CheckOperation.Sync(
                () =>
                {
                    if (actual is null) return false;
                    if (actual.Count() <= 1) return true;
                    var effectiveRule = ruleResolver() ?? OrderingRuleRegistry.GetOrDefault<T>();
                    return !Enumerable.Range(1, actual.Count() - 1).Any(i =>
                    {
                        int cmp = effectiveRule is not null
                            ? effectiveRule.Compare(actual.ElementAt(i - 1), actual.ElementAt(i))
                            : Comparer<T>.Default.Compare(actual.ElementAt(i - 1), actual.ElementAt(i));
                        return cmp < 0;
                    });
                },
                () => actual is null ? "Expected a collection, but was null" : "Expected collection to be ordered descending",
                isSkipped);
        }

        private static string GetExpectedOrderLabel<T>(IOrderingRule<T>? rule)
        {
            if (rule is IOrderingDirectionProvider provider)
            {
                return provider.Direction switch
                {
                    OrderingDirection.Ascending => "ascending",
                    OrderingDirection.Descending => "descending",
                    _ => "specified order"
                };
            }

            return "ascending";
        }

        public static CheckOperation IsSubsetOf<T>(IEnumerable<T>? actual, IEnumerable<T> superset, bool isSkipped)
        {
            var superList = superset is IEnumerable<T> r ? r : superset.ToList();
            return CheckOperation.Sync(
                () => actual is not null && actual.All(x => superList.Any(s => Eq<T>().Equals(x, s))),
                () => actual is null ? "Expected a collection, but was null" : "Expected collection to be a subset",
                isSkipped);
        }

        public static CheckOperation IsSupersetOf<T>(IEnumerable<T>? actual, IEnumerable<T> subset, bool isSkipped)
        {
            var subList = subset is IEnumerable<T> r ? r : [.. subset];
            return CheckOperation.Sync(
                () => actual is not null && subList.All(s => actual.Any(x => Eq<T>().Equals(x, s))),
                () => actual is null ? "Expected a collection, but was null" : "Expected collection to be a superset",
                isSkipped);
        }

        public static CheckOperation IsNull<T>(IEnumerable<T>? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null,
                () => $"Expected null, but was {ValueFormatter.Format(actual)}",
                isSkipped);

        public static CheckOperation IsNotNull<T>(IEnumerable<T>? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null,
                () => "Expected a collection, but was null",
                isSkipped);

        public static CheckOperation ContainsInOrder<T>(IEnumerable<T>? actual, IEnumerable<T> expList, bool isSkipped)
            => CheckOperation.Sync(
                () =>
                {
                    if (actual is null) return false;
                    int ei = 0;
                    foreach (var item in actual)
                    {
                        if (ei >= expList.Count()) break;
                        if (Eq<T>().Equals(item, expList.ElementAt(ei))) ei++;
                    }
                    return ei == expList.Count();
                },
                () => actual is null ? "Expected a collection, but was null"
                    : $"Expected collection to contain [{FmtItems(expList)}] in order",
                isSkipped);

        public static CheckOperation ContainsNone<T>(IEnumerable<T>? actual, IEnumerable<T> unexpected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => actual is not null && !unexpected.Any(u => actual.Any(x => Eq<T>().Equals(x, u))),
                () =>
                {
                    if (actual is null) return "Expected a collection, but was null";
                    var found = unexpected.Where(u => actual.Any(x => Eq<T>().Equals(x, u))).ToList();
                    return $"Expected collection to contain none of the items, but found: [{string.Join(", ", found.Select(v => ValueFormatter.Format(v)))}]";
                },
                isSkipped);

        public static CheckOperation ContainsExactly<T>(IEnumerable<T>? actual, IEnumerable<T> expected, bool isSkipped)
            => CheckOperation.Sync(
                () =>
                {
                    if (actual is null || actual.Count() != expected.Count()) return false;
                    var remaining = new List<T>(expected);
                    foreach (var x in actual)
                    {
                        int idx = remaining.FindIndex(e => Eq<T>().Equals(x, e));
                        if (idx < 0) return false;
                        remaining.RemoveAt(idx);
                    }
                    return remaining.Count == 0;
                },
                () =>
                {
                    if (actual is null) return "Expected a collection, but was null";
                    var extra = actual.Where(x => !expected.Any(e => Eq<T>().Equals(x, e))).ToList();
                    var missing = expected.Where(e => !actual.Any(x => Eq<T>().Equals(x, e))).ToList();
                    var parts = new List<string>();
                    if (actual.Count() != expected.Count()) parts.Add($"count was {actual.Count()} (expected {expected.Count()})");
                    if (missing.Count > 0) parts.Add($"missing: [{string.Join(", ", missing.Select(v => ValueFormatter.Format(v)))}]");
                    if (extra.Count > 0) parts.Add($"unexpected: [{string.Join(", ", extra.Select(v => ValueFormatter.Format(v)))}]");
                    return $"Expected collection to contain exactly these items, but {string.Join("; ", parts)}";
                },
                isSkipped);
    }

    public static class CollectionExtensions
    {
        public static bool HasKey<K, V>(this IEnumerable<KeyValuePair<K, V>> d, K key)
            => d.Any(kv => EqualityComparer<K>.Default.Equals(kv.Key, key));

        public static bool TryGetValue<K, V>(this IEnumerable<KeyValuePair<K, V>> d, K key, out V? value)
        {
            foreach (var kv in d)
            {
                if (EqualityComparer<K>.Default.Equals(kv.Key, key))
                { value = kv.Value; return true; }
            }
            value = default;
            return false;
        }
    }
}

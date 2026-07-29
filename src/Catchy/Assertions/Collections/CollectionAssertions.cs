using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    public static partial class AsserterExtensions
    {
        /// <summary>Starts assertions for an <see cref="IEnumerable{T}"/> sequence.</summary>
        [GenerateArityOverloads(target: nameof(value))]
        public static ValueAssertions<IEnumerable<T>?> That<T>(this Asserter a, IEnumerable<T>? value, __._ _ = default,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(
                        asserterExpr: aExpr,
                        methodName: "That",
                        genericType: typeof(T),
                        valueExpr: vExpr,
                        file: file, line: line, member: member);
            return new ValueAssertions<IEnumerable<T>?>(p, value);
        }
    }

    public static partial class CollectionAssertionsExtensions
    {
        private static IReadOnlyList<TItem>? Materialize<TItem>(IEnumerable<TItem>? value) => value as IReadOnlyList<TItem> ?? value?.ToList();
        /// <summary>Applies an ordering <paramref name="rule"/> to subsequent ordering assertions in this chain.</summary>
		[DebuggerHidden, StackTraceHidden, AssertionMethod]
		public static ValueAssertions<IEnumerable<T>?> With<T>(
			this ValueAssertions<IEnumerable<T>?> a, IOrderingRule<T> rule,
			[CallerArgumentExpression(nameof(rule))] string? ruleExpr = null)
		{
			((IAssertions)a).GetPipeline().SetOrderingRule(rule);
			a.Link("With", ruleExpr);
			return a;
		}

        /// <summary>Asserts that the collection has exactly one item, and captures it in <paramref name="item"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> HasSingleItem<T>(this ValueAssertions<IEnumerable<T>?> a, out T? item,
            [CallerArgumentExpression(nameof(item))] string? itemExpr = null)
        {
            var captured = Materialize(a.GetValue());
            item = captured is { Count: 1 } ? captured[0] : default;
            a.Link("HasSingleItem", itemExpr);
            a.Op(op => CheckOperation.Sync(
                () => captured is { Count: 1 },
                () => captured is null ? "Expected a collection, but was null" : $"Expected exactly one item, but had {captured.Count}",
                op.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the collection has an element at position <paramref name="index"/>, and captures it in <paramref name="item"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> HasItemAt<T>(this ValueAssertions<IEnumerable<T>?> a, int index, out T? item,
            [CallerArgumentExpression(nameof(item))] string? itemExpr = null)
        {
            var captured = Materialize(a.GetValue());
            item = captured is not null && captured.Count > index ? captured[index] : default;
            a.Link("HasItemAt", itemExpr);
            a.Op(op => CheckOperation.Sync(
                () => captured is not null && captured.Count > index,
                () => captured is null ? "Expected a collection, but was null" : $"Expected at least {index + 1} items, but had {captured.Count}",
                op.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the collection is non-empty and captures its first element in <paramref name="item"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> HasFirst<T>(this ValueAssertions<IEnumerable<T>?> a, out T? item,
            [CallerArgumentExpression(nameof(item))] string? itemExpr = null)
        {
            var captured = Materialize(a.GetValue());
            item = captured is { Count: > 0 } ? captured[0] : default;
            a.Link("HasFirst", itemExpr);
            a.Op(op => CheckOperation.Sync(
                () => captured is { Count: > 0 },
                () => captured is null ? "Expected a collection, but was null" : "Expected non-empty collection",
                op.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the collection is non-empty and captures its last element in <paramref name="item"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> HasLast<T>(this ValueAssertions<IEnumerable<T>?> a, out T? item,
            [CallerArgumentExpression(nameof(item))] string? itemExpr = null)
        {
            var captured = Materialize(a.GetValue());
            item = captured is { Count: > 0 } ? captured.Last() : default;
            a.Link("HasLast", itemExpr);
            a.Op(op => CheckOperation.Sync(
                () => captured is { Count: > 0 },
                () => captured is null ? "Expected a collection, but was null" : "Expected non-empty collection",
                op.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the collection has a valid element at <paramref name="index"/> and captures it in <paramref name="item"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> HasElementAt<T>(this ValueAssertions<IEnumerable<T>?> a, int index, out T? item,
            [CallerArgumentExpression(nameof(index))] string? expr = null,
            [CallerArgumentExpression(nameof(item))] string? itemExpr = null)
        {
            var captured = Materialize(a.GetValue());
            bool valid = captured is not null && index >= 0 && index < captured.Count;
            item = valid ? captured![index] : default;
            a.Link("HasElementAt", expr, itemExpr);
            a.Op(op => CheckOperation.Sync(
                () => valid,
                () => captured is null ? "Expected a collection, but was null" : $"Expected element at index {index}, but count was {captured.Count}",
                op.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the collection is empty (has no elements).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> IsEmpty<T>(this ValueAssertions<IEnumerable<T>?> a)
        { a.Link("IsEmpty"); a.Op(a => CollectionChecks.IsEmpty(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the collection is not empty.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> IsNotEmpty<T>(this ValueAssertions<IEnumerable<T>?> a)
        { a.Link("IsNotEmpty"); a.Op(a => CollectionChecks.IsNotEmpty(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the collection has exactly <paramref name="expected"/> elements.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> HasCount<T>(this ValueAssertions<IEnumerable<T>?> a, int expected,
        [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("HasCount", expr); a.Op(a => CollectionChecks.HasCount(a.GetValue(), expected, a.IsSkipped())); return a; }

        /// <summary>Asserts that the collection has exactly <paramref name="expected"/> elements. Alias for <c>HasCount</c>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> HasCountOf<T>(this ValueAssertions<IEnumerable<T>?> a, int expected,
         [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("HasCountOf", expr); a.Op(a => CollectionChecks.HasCount(a.GetValue(), expected, a.IsSkipped())); return a; }

        /// <summary>Asserts that the collection has more than <paramref name="n"/> elements.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> HasCountGreaterThan<T>(this ValueAssertions<IEnumerable<T>?> a, int n,
         [CallerArgumentExpression(nameof(n))] string? expr = null)
        { a.Link("HasCountGreaterThan", expr); a.Op(a => CollectionChecks.HasCountGreaterThan(a.GetValue(), n, a.IsSkipped())); return a; }

        /// <summary>Asserts that the collection has fewer than <paramref name="n"/> elements.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> HasCountLessThan<T>(this ValueAssertions<IEnumerable<T>?> a, int n,
        [CallerArgumentExpression(nameof(n))] string? expr = null)
        { a.Link("HasCountLessThan", expr); a.Op(a => CollectionChecks.HasCountLessThan(a.GetValue(), n, a.IsSkipped())); return a; }

        /// <summary>Asserts that the collection contains <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> Contains<T>(this ValueAssertions<IEnumerable<T>?> a, T expected,
         [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("Contains", expr); a.Op(a => CollectionChecks.Contains(a.GetValue(), expected, a.IsSkipped())); return a; }

        /// <summary>Asserts that the collection contains an element matching <paramref name="predicate"/> and captures it in <paramref name="item"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> Contains<T>(this ValueAssertions<IEnumerable<T>?> a, Func<T, bool> predicate, out T? item,
            [CallerArgumentExpression(nameof(predicate))] string? expr = null,
            [CallerArgumentExpression(nameof(item))] string? itemExpr = null)
        {
            var captured = Materialize(a.GetValue());
            var foundAny = CollectionChecks.TryFindFirstMatch(captured, predicate, out item);
            a.Link("Contains", expr, itemExpr);
            a.Op(op => CollectionChecks.HasMatch(captured, foundAny, "Expected collection to contain an item matching:", expr, op.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the collection does not contain <paramref name="item"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> DoesNotContain<T>(this ValueAssertions<IEnumerable<T>?> a, T item,
       [CallerArgumentExpression(nameof(item))] string? expr = null)
        { a.Link("DoesNotContain", expr); a.Op(a => CollectionChecks.DoesNotContain(a.GetValue(), item, a.IsSkipped())); return a; }

        /// <summary>Asserts that the collection contains ALL of the <paramref name="expected"/> elements.</summary>
        [GenerateArityOverloads(target: nameof(expected))]
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> ContainsAll<T>(this ValueAssertions<IEnumerable<T>?> a, IEnumerable<T> expected,
      [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("ContainsAll", expr); a.Op(a => CollectionChecks.ContainsAll(a.GetValue(), expected, a.IsSkipped())); return a; }

        /// <summary>Asserts that the collection contains AT LEAST ONE of the <paramref name="expected"/> elements.</summary>
        [GenerateArityOverloads(target: nameof(expected))]
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> ContainsAny<T>(this ValueAssertions<IEnumerable<T>?> a, IEnumerable<T> expected,
       [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("ContainsAny", expr); a.Op(a => CollectionChecks.ContainsAny(a.GetValue(), expected, a.IsSkipped())); return a; }

        /// <summary>Asserts that the collection contains the <paramref name="expected"/> elements in the specified order (contiguous subsequence).</summary>
        [GenerateArityOverloads(target: nameof(expected))]
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> ContainsInOrder<T>(this ValueAssertions<IEnumerable<T>?> a, IEnumerable<T> expected,
          [CallerArgumentExpression(nameof(expected))] string? expr = null)
        {
            var expList = Materialize(expected)!;
            a.Link("ContainsInOrder", expr);
            a.Op(a => CollectionChecks.ContainsInOrder(a.GetValue(), expList, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that every element in the collection satisfies <paramref name="predicate"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> AllSatisfy<T>(this ValueAssertions<IEnumerable<T>?> a,
        Func<T, bool> predicate,
        [CallerArgumentExpression(nameof(predicate))] string? expr = null)
        {
            a.Link("AllSatisfy", expr);
            a.Op(a => QuantifiedModeChecks.Apply(
                QuantifiedMode.Each,
                a.GetValue(),
                item => CheckOperation.Sync(
                    () => predicate(item),
                    () => $"Expected item to satisfy: {expr ?? "<predicate>"}",
                    false),
                a.IsSkipped(),
                () => a.GetPipeline().Settings.CollectionFailFast,
                () => a.GetPipeline().Settings.SyncParallelThreshold));
            return a;
        }

        /// <summary>Asserts that at least one element in the collection satisfies <paramref name="predicate"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> AnySatisfy<T>(this ValueAssertions<IEnumerable<T>?> a,
         Func<T, bool> predicate,
         [CallerArgumentExpression(nameof(predicate))] string? expr = null)
        {
            a.Link("AnySatisfy", expr);
            a.Op(a => QuantifiedModeChecks.Apply(
                QuantifiedMode.Any,
                a.GetValue(),
                item => CheckOperation.Sync(
                    () => predicate(item),
                    () => $"Expected item to satisfy: {expr ?? "<predicate>"}",
                    false),
                a.IsSkipped(),
                () => a.GetPipeline().Settings.CollectionFailFast,
                () => a.GetPipeline().Settings.SyncParallelThreshold));
            return a;
        }

        /// <summary>Asserts that no element in the collection satisfies <paramref name="predicate"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> NoneSatisfy<T>(this ValueAssertions<IEnumerable<T>?> a,
        Func<T, bool> predicate,
        [CallerArgumentExpression(nameof(predicate))] string? expr = null)
        {
            a.Link("NoneSatisfy", expr);
            a.Op(a => QuantifiedModeChecks.Apply(
                QuantifiedMode.None,
                a.GetValue(),
                item => CheckOperation.Sync(
                    () => predicate(item),
                    () => $"Expected item to satisfy: {expr ?? "<predicate>"}",
                    false),
                a.IsSkipped(),
                () => a.GetPipeline().Settings.CollectionFailFast,
                () => a.GetPipeline().Settings.SyncParallelThreshold));
            return a;
        }

        /// <summary>Asserts that all elements in the collection are distinct (no duplicates).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> HasDistinctItems<T>(this ValueAssertions<IEnumerable<T>?> a)
        { a.Link("HasDistinctItems"); a.Op(a => CollectionChecks.HasDistinctItems(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the collection contains the same elements as <paramref name="expected"/>, in any order (deep-equal comparison).</summary>
        [GenerateArityOverloads(target: nameof(expected))]
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> IsEquivalentTo<T>(this ValueAssertions<IEnumerable<T>?> a, IEnumerable<T> expected,
        [CallerArgumentExpression(nameof(expected))] string? expr = null)
        {
            var expList = Materialize(expected)!;
            a.Link("IsEquivalentTo", expr);
            a.Op(a => CollectionChecks.IsEquivalentTo(
                a.GetValue(),
                expList,
                () => a.GetPipeline().Settings.EqualsOptions ?? new EqualsOptions(),
                () => a.GetPipeline().GetEffectiveDeepEqualRuleContainer(),
                a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the collection is not deeply equivalent to <paramref name="expected"/>.</summary>
        [GenerateArityOverloads(target: nameof(expected))]
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> IsNotEquivalentTo<T>(this ValueAssertions<IEnumerable<T>?> a, IEnumerable<T> expected,
        [CallerArgumentExpression(nameof(expected))] string? expr = null)
        {
            var expList = Materialize(expected)!;
            a.Link("IsNotEquivalentTo", expr);
            a.Op(a => CollectionChecks.IsNotEquivalentTo(
                a.GetValue(),
                expList,
                () => a.GetPipeline().Settings.EqualsOptions ?? new EqualsOptions(),
                () => a.GetPipeline().GetEffectiveDeepEqualRuleContainer(),
                a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the collection is sequence-equal to <paramref name="expected"/> (same elements in the same order).</summary>
        [GenerateArityOverloads(target: "expected")]
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> IsSequenceEqualTo<T>(this ValueAssertions<IEnumerable<T>?> a, IEnumerable<T> expected,
        [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("IsSequenceEqualTo", expr); a.Op(a => CollectionChecks.IsSequenceEqualTo(a.GetValue(), expected, a.IsSkipped())); return a; }

        /// <summary>
		/// Configures ascending ordering for this chain.
		/// Use with ordering assertions such as <c>IsOrdered</c>, <c>IsInAscendingOrder</c>, and <c>IsOrderedAscending</c>.
		/// </summary>
		[DebuggerHidden, StackTraceHidden, AssertionMethod]
		public static ValueAssertions<IEnumerable<T>?> Ascending<T>(this ValueAssertions<IEnumerable<T>?> a)
			where T : IComparable<T>
		{
			((IAssertions)a).GetPipeline().SetOrderingRule(OrderingRules.Ascending<T>());
			a.Link("Ascending");
			return a;
		}

		/// <summary>
		/// Configures descending ordering for this chain.
		/// Use with ordering assertions such as <c>IsInDescendingOrder</c> and <c>IsOrderedDescending</c>.
		/// </summary>
		[DebuggerHidden, StackTraceHidden, AssertionMethod]
		public static ValueAssertions<IEnumerable<T>?> Descending<T>(this ValueAssertions<IEnumerable<T>?> a)
			where T : IComparable<T>
		{
			((IAssertions)a).GetPipeline().SetOrderingRule(OrderingRules.Descending<T>());
			a.Link("Descending");
			return a;
		}

        /// <summary>Asserts that the collection is in ascending order according to the active ordering rule.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> IsOrdered<T>(this ValueAssertions<IEnumerable<T>?> a,
        IOrderingRule<T>? rule = null)
        {
            a.Link("IsOrdered");
            if (rule != null)
            {
                a.Op(a => CollectionChecks.IsOrdered(a.GetValue(), rule, a.IsSkipped()));
            }
            else
            {
                a.Op(a => CollectionChecks.IsOrdered(
                    a.GetValue(),
                    () => a.GetPipeline().GetEffectiveOrderingRule<T>(),
                    a.IsSkipped()));
            }
            return a;
        }

        /// <summary>Asserts that the collection is in ascending order. Alias for <c>IsOrdered</c>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> IsOrderedAscending<T>(this ValueAssertions<IEnumerable<T>?> a,
        IOrderingRule<T>? rule = null)
        {
            a.Link("IsOrderedAscending");
            if (rule != null)
            {
                a.Op(a => CollectionChecks.IsOrdered(a.GetValue(), rule, a.IsSkipped()));
            }
            else
            {
                a.Op(a => CollectionChecks.IsOrdered(
                    a.GetValue(),
                    () => a.GetPipeline().GetEffectiveOrderingRule<T>(),
                    a.IsSkipped()));
            }
            return a;
        }

        /// <summary>Asserts that the collection is in ascending order. Alias for <c>IsOrdered</c>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> IsInAscendingOrder<T>(this ValueAssertions<IEnumerable<T>?> a,
        IOrderingRule<T>? rule = null)
        {
            a.Link("IsInAscendingOrder");
            if (rule != null)
            {
                a.Op(a => CollectionChecks.IsOrdered(a.GetValue(), rule, a.IsSkipped()));
            }
            else
            {
                a.Op(a => CollectionChecks.IsOrdered(
                    a.GetValue(),
                    () => a.GetPipeline().GetEffectiveOrderingRule<T>(),
                    a.IsSkipped()));
            }
            return a;
        }

        /// <summary>Asserts that the collection is in descending order.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> IsInDescendingOrder<T>(this ValueAssertions<IEnumerable<T>?> a,
        IOrderingRule<T>? rule = null)
        {
            a.Link("IsInDescendingOrder");
            if (rule != null)
            {
                a.Op(a => CollectionChecks.IsOrderedDescending(a.GetValue(), rule, a.IsSkipped()));
            }
            else
            {
                a.Op(a => CollectionChecks.IsOrderedDescending(
                    a.GetValue(),
                    () => a.GetPipeline().GetEffectiveOrderingRule<T>(),
                    a.IsSkipped()));
            }
            return a;
        }

        /// <summary>Asserts that the collection is in descending order. Alias for <c>IsInDescendingOrder</c>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> IsOrderedDescending<T>(this ValueAssertions<IEnumerable<T>?> a,
        IOrderingRule<T>? rule = null)
        {
            a.Link("IsOrderedDescending");
            if (rule != null)
            {
                a.Op(a => CollectionChecks.IsOrderedDescending(a.GetValue(), rule, a.IsSkipped()));
            }
            else
            {
                a.Op(a => CollectionChecks.IsOrderedDescending(
                    a.GetValue(),
                    () => a.GetPipeline().GetEffectiveOrderingRule<T>(),
                    a.IsSkipped()));
            }
            return a;
        }

        /// <summary>Asserts that the collection is a subset of <paramref name="superset"/>.</summary>
        [GenerateArityOverloads(target: nameof(superset))]
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> IsSubsetOf<T>(this ValueAssertions<IEnumerable<T>?> a, IEnumerable<T> superset,
        [CallerArgumentExpression(nameof(superset))] string? expr = null)
        { a.Link("IsSubsetOf", expr); a.Op(a => CollectionChecks.IsSubsetOf(a.GetValue(), superset, a.IsSkipped())); return a; }

        /// <summary>Asserts that the collection is a superset of <paramref name="subset"/>.</summary>
        [GenerateArityOverloads(target: nameof(subset))]
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> IsSupersetOf<T>(this ValueAssertions<IEnumerable<T>?> a, IEnumerable<T> subset,
       [CallerArgumentExpression(nameof(subset))] string? expr = null)
        { a.Link("IsSupersetOf", expr); a.Op(a => CollectionChecks.IsSupersetOf(a.GetValue(), subset, a.IsSkipped())); return a; }

        /// <summary>Asserts that the collection has at least <paramref name="min"/> elements.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> HasCountAtLeast<T>(this ValueAssertions<IEnumerable<T>?> a, int min,
         [CallerArgumentExpression(nameof(min))] string? expr = null)
        { a.Link("HasCountAtLeast", expr); a.Op(a => CollectionChecks.HasCountAtLeast(a.GetValue(), min, a.IsSkipped())); return a; }

        /// <summary>Asserts that the collection has at most <paramref name="max"/> elements.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> HasCountAtMost<T>(this ValueAssertions<IEnumerable<T>?> a, int max,
            [CallerArgumentExpression(nameof(max))] string? expr = null)
        { a.Link("HasCountAtMost", expr); a.Op(a => CollectionChecks.HasCountAtMost(a.GetValue(), max, a.IsSkipped())); return a; }

        /// <summary>Asserts that the collection has between <paramref name="min"/> and <paramref name="max"/> elements (inclusive).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> HasCountInRange<T>(this ValueAssertions<IEnumerable<T>?> a, int min, int max,
            [CallerArgumentExpression(nameof(min))] string? minExpr = null,
            [CallerArgumentExpression(nameof(max))] string? maxExpr = null)
        { a.Link("HasCountInRange", minExpr, maxExpr); a.Op(a => CollectionChecks.HasCountInRange(a.GetValue(), min, max, minExpr, maxExpr, a.IsSkipped())); return a; }

        /// <summary>Asserts that the collection has at least <paramref name="min"/> elements (greater than or equal to).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> HasCountGreaterThanOrEqualTo<T>(this ValueAssertions<IEnumerable<T>?> a, int min,
            [CallerArgumentExpression(nameof(min))] string? expr = null)
        { a.Link("HasCountGreaterThanOrEqualTo", expr); a.Op(a => CollectionChecks.HasCountGreaterThanOrEqualTo(a.GetValue(), min, expr, a.IsSkipped())); return a; }

        /// <summary>Asserts that the collection has at most <paramref name="max"/> elements (less than or equal to).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> HasCountLessThanOrEqualTo<T>(this ValueAssertions<IEnumerable<T>?> a, int max,
            [CallerArgumentExpression(nameof(max))] string? expr = null)
        { a.Link("HasCountLessThanOrEqualTo", expr); a.Op(a => CollectionChecks.HasCountLessThanOrEqualTo(a.GetValue(), max, expr, a.IsSkipped())); return a; }

        /// <summary>Asserts that the collection has the same number of elements as <paramref name="other"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> HasSameCountAs<T>(this ValueAssertions<IEnumerable<T>?> a, IEnumerable<T> other,
            [CallerArgumentExpression(nameof(other))] string? expr = null)
        {
            var otherCount = other is IReadOnlyCollection<T> rc ? rc.Count : other.Count();
            a.Link("HasSameCountAs", expr);
            a.Op(a => CollectionChecks.HasSameCountAs(a.GetValue(), otherCount, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the collection contains an element deep-equal to <paramref name="item"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> ContainsEquivalentOf<T>(this ValueAssertions<IEnumerable<T>?> a, T item,
            Action<EqualsOptions>? configure = null,
            [CallerArgumentExpression(nameof(item))] string? expr = null)
        {
            var opts = new EqualsOptions();
            configure?.Invoke(opts);
            a.Link("ContainsEquivalentOf", expr);
            a.Op(a => CollectionChecks.ContainsEquivalentOf(a.GetValue(), item, opts, expr, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that all elements are distinct when projected by <paramref name="keySelector"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> HasDistinctItemsBy<T, TKey>(this ValueAssertions<IEnumerable<T>?> a, Func<T, TKey> keySelector,
            [CallerArgumentExpression(nameof(keySelector))] string? expr = null)
        { a.Link("HasDistinctItemsBy", expr); a.Op(a => CollectionChecks.HasDistinctItemsBy(a.GetValue(), keySelector, a.IsSkipped())); return a; }

        /// <summary>Asserts that the collection contains at least one null element.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> HasNullItems<T>(this ValueAssertions<IEnumerable<T>?> a)
        { a.Link("HasNullItems"); a.Op(a => CollectionChecks.HasNullItems(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the collection contains no null elements.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> HasNoNullItems<T>(this ValueAssertions<IEnumerable<T>?> a)
        { a.Link("HasNoNullItems"); a.Op(a => CollectionChecks.HasNoNullItems(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the collection shares at least one element with <paramref name="other"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> IntersectsWith<T>(this ValueAssertions<IEnumerable<T>?> a, IEnumerable<T> other,
            [CallerArgumentExpression(nameof(other))] string? expr = null)
        {
            var otherList = Materialize(other)!;
            a.Link("IntersectsWith", expr);
            a.Op(a => CollectionChecks.IntersectsWith(a.GetValue(), otherList, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that all elements in the collection are assignable to <typeparamref name="TTarget"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> ContainsItemsAssignableTo<T, TTarget>(this ValueAssertions<IEnumerable<T>?> a)
        { a.Link("ContainsItemsAssignableTo", typeof(TTarget)); a.Op(a => CollectionChecks.ContainsItemsAssignableTo<T, TTarget>(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the collection is in ascending order when elements are projected by <paramref name="keySelector"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> IsOrderedBy<T, TKey>(this ValueAssertions<IEnumerable<T>?> a, Func<T, TKey> keySelector,
            [CallerArgumentExpression(nameof(keySelector))] string? expr = null)
        { a.Link("IsOrderedBy", expr); a.Op(a => CollectionChecks.IsOrderedBy(a.GetValue(), keySelector, a.IsSkipped())); return a; }

        /// <summary>Asserts that the collection is in descending order when elements are projected by <paramref name="keySelector"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> IsOrderedDescendingBy<T, TKey>(this ValueAssertions<IEnumerable<T>?> a, Func<T, TKey> keySelector,
            [CallerArgumentExpression(nameof(keySelector))] string? expr = null)
        { a.Link("IsOrderedDescendingBy", expr); a.Op(a => CollectionChecks.IsOrderedDescendingBy(a.GetValue(), keySelector, a.IsSkipped())); return a; }

        /// <summary>Asserts that each element satisfies the corresponding inspector in order.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IEnumerable<T>?> SatisfyRespectively<T>(this ValueAssertions<IEnumerable<T>?> a,
            params Action<T>[] inspectors)
        { a.Link("SatisfyRespectively"); a.Op(a => CollectionChecks.SatisfyRespectively(a.GetValue(), inspectors, a.IsSkipped())); return a; }
    }
}



using Catchy;
using CatchyCoreTests.Helpers;
using CatchyTestHelpers;

namespace CatchyCoreTests.Assertions.Collections
{
    /// <summary>
    /// Integration tests for CollectionAssertions&lt;T&gt;.
    /// Covers cardinality, membership, position, and quantified operations.
    /// </summary>
    public class CollectionAssertionsTests
    {
        // ===== Cardinality =====

        [Fact]
        public async Task HasCount_CorrectCount_Passes()
        {
            // Arrange
            var items = new[] { 1, 2, 3 };

            // Act & Verify
            await Stateless.Assert.That(items).HasCount(3);
        }

        [Fact]
        public async Task HasCount_IncorrectCount_Throws()
        {
            // Arrange
            var items = new[] { 1, 2 };

            // Act
            try
            {
                await Stateless.Assert.That(items).HasCount(3);
                await Stateless.Assert.Fail("Expected AssertionException to be thrown");
            }
            catch (AssertionException)
            {
                // Expected
            }
        }

        [Fact]
        public async Task HasCountAtLeast_SufficientCount_Passes()
        {
            // Arrange
            var items = new[] { 1, 2, 3, 4, 5 };

            // Act & Verify
            await Stateless.Assert.That(items).HasCountAtLeast(3);
        }

        [Fact]
        public async Task HasCountAtMost_SufficientCount_Passes()
        {
            // Arrange
            var items = new[] { 1, 2 };

            // Act & Verify
            await Stateless.Assert.That(items).HasCountAtMost(5);
        }

        [Fact]
        public async Task HasSingleItem_SingleItem_Passes()
        {
            // Arrange
            var items = new[] { 42 };

            // Act & Verify
            await Stateless.Assert.That(items).HasSingleItem(out var item);
            await Stateless.Assert.That(item).Is(42);
        }

        [Fact]
        public async Task HasSingleItem_MultipleItems_Throws()
        {
            // Arrange
            var items = new[] { 1, 2 };

            // Act
            try
            {
                await Stateless.Assert.That(items).HasSingleItem(out var item);
                await Stateless.Assert.Fail("Expected AssertionException to be thrown");
            }
            catch (AssertionException)
            {
                // Expected
            }
        }

        [Fact]
        public async Task IsEmpty_EmptyCollection_Passes()
        {
            // Arrange
            var items = new int[] { };

            // Act & Verify
            await Stateless.Assert.That(items).IsEmpty();
        }

        [Fact]
        public async Task IsNotEmpty_NonEmptyCollection_Passes()
        {
            // Arrange
            var items = new[] { 1 };

            // Act & Verify
            await Stateless.Assert.That(items).IsNotEmpty();
        }

        // ===== Membership =====

        [Fact]
        public async Task Contains_ItemExists_Passes()
        {
            // Arrange
            var items = new[] { 1, 2, 3 };

            // Act & Verify
            await Stateless.Assert.That(items).Contains(2);
        }

        [Fact]
        public async Task Contains_ItemMissing_Throws()
        {
            // Arrange
            var items = new[] { 1, 2, 3 };

            // Act
            try
            {
                await Stateless.Assert.That(items).Contains(4);
                await Stateless.Assert.Fail("Expected AssertionException to be thrown");
            }
            catch (AssertionException)
            {
                // Expected
            }
        }

        [Fact]
        public async Task Contains_WithPredicate_MatchFound_Passes()
        {
            // Arrange
            var items = new[] { 1, 2, 3, 4, 5 };

            // Act & Verify
            await Stateless.Assert.That(items).Contains(x => x > 3, out var matched);
            await Stateless.Assert.That(matched).IsGreaterThan(3);
        }

        [Fact]
        public async Task Contains_WithPredicate_NoMatch_Throws()
        {
            // Arrange
            var items = new[] { 1, 2, 3 };

            // Act
            try
            {
                await Stateless.Assert.That(items).Contains(x => x > 10, out var matched);
                await Stateless.Assert.Fail("Expected AssertionException to be thrown");
            }
            catch (AssertionException)
            {
                // Expected
            }
        }

        [Fact]
        public async Task DoesNotContain_ItemMissing_Passes()
        {
            // Arrange
            var items = new[] { 1, 2, 3 };

            // Act & Verify
            await Stateless.Assert.That(items).DoesNotContain(4);
        }

        [Fact]
        public async Task HasCountOf_CorrectCount_Passes()
        {
            // Arrange
            var items = new[] { 1, 2, 3 };

            // Act & Verify
            await Stateless.Assert.That(items).HasCountOf(3);
        }

        [Fact]
        public async Task HasCountOf_WrongCount_Throws()
        {
            // Arrange
            var items = new[] { 1, 2, 3 };

            // Act
            try
            {
                await Stateless.Assert.That(items).HasCountOf(2);
                await Stateless.Assert.Fail("Expected AssertionException to be thrown");
            }
            catch (AssertionException)
            {
                // Expected
            }
        }

        [Fact]
        public async Task ContainsAll_AllPresent_Passes()
        {
            // Arrange
            var items = new[] { 1, 2, 3, 4 };

            // Act & Verify
            await Stateless.Assert.That(items).ContainsAll(1, 3, 4);
        }

        [Fact]
        public async Task ContainsAll_MissingItem_Throws()
        {
            // Arrange
            var items = new[] { 1, 2, 3 };

            // Act
            try
            {
                await Stateless.Assert.That(items).ContainsAll(1, 2, 4);
                await Stateless.Assert.Fail("Expected AssertionException to be thrown");
            }
            catch (AssertionException)
            {
                // Expected
            }
        }

        [Fact]
        public async Task ContainsAny_AnyPresent_Passes()
        {
            // Arrange
            var items = new[] { 1, 2, 3 };

            // Act & Verify
            await Stateless.Assert.That(items).ContainsAny(4, 2, 9);
        }

        [Fact]
        public async Task ContainsAny_NoMatches_Throws()
        {
            // Arrange
            var items = new[] { 1, 2, 3 };

            // Act
            try
            {
                await Stateless.Assert.That(items).ContainsAny(4, 5, 6);
                await Stateless.Assert.Fail("Expected AssertionException to be thrown");
            }
            catch (AssertionException)
            {
                // Expected
            }
        }

        [Fact]
        public async Task ContainsInOrder_CorrectOrder_Passes()
        {
            // Arrange
            var items = new[] { 1, 2, 3, 4 };

            // Act & Verify
            await Stateless.Assert.That(items).ContainsInOrder(2, 3);
        }

        [Fact]
        public async Task ContainsInOrder_WrongOrder_Throws()
        {
            // Arrange
            var items = new[] { 1, 2, 3, 4 };

            // Act
            try
            {
                await Stateless.Assert.That(items).ContainsInOrder(3, 2);
                await Stateless.Assert.Fail("Expected AssertionException to be thrown");
            }
            catch (AssertionException)
            {
                // Expected
            }
        }

        // ===== Position =====

        [Fact]
        public async Task HasFirst_NonEmpty_Passes()
        {
            // Arrange
            var items = new[] { 10, 20, 30 };

            // Act & Verify
            await Stateless.Assert.That(items).HasFirst(out var first);
            await Stateless.Assert.That(first).Is(10);
        }

        [Fact]
        public async Task HasFirst_EmptyCollection_Throws()
        {
            // Arrange
            var items = Array.Empty<int>();

            // Act
            try
            {
                await Stateless.Assert.That(items).HasFirst(out var first);
                await Stateless.Assert.Fail("Expected AssertionException to be thrown");
            }
            catch (AssertionException)
            {
                // Expected
            }
        }

        [Fact]
        public async Task HasLast_NonEmpty_Passes()
        {
            // Arrange
            var items = new[] { 10, 20, 30 };

            // Act & Verify
            await Stateless.Assert.That(items).HasLast(out var last);
            await Stateless.Assert.That(last).Is(30);
        }

        [Fact]
        public async Task HasItemAt_ValidIndex_Passes()
        {
            // Arrange
            var items = new[] { "a", "b", "c" };

            // Act & Verify
            await Stateless.Assert.That(items).HasItemAt(1, out var item);
            await Stateless.Assert.That(item).Is("b");
        }

        [Fact]
        public async Task HasItemAt_OutOfRange_Throws()
        {
            // Arrange
            var items = new[] { "a", "b", "c" };

            // Act
            try
            {
                await Stateless.Assert.That(items).HasItemAt(3, out var item);
                await Stateless.Assert.Fail("Expected AssertionException to be thrown");
            }
            catch (AssertionException)
            {
                // Expected
            }
        }

        [Fact]
        public async Task HasElementAt_ValidIndex_Passes()
        {
            // Arrange
            var items = new[] { "a", "b", "c" };

            // Act & Verify
            await Stateless.Assert.That(items).HasElementAt(2, out var item);
            await Stateless.Assert.That(item).Is("c");
        }

        [Fact]
        public async Task HasElementAt_OutOfRange_Throws()
        {
            // Arrange
            var items = new[] { "a", "b", "c" };

            // Act
            try
            {
                await Stateless.Assert.That(items).HasElementAt(3, out var item);
                await Stateless.Assert.Fail("Expected AssertionException to be thrown");
            }
            catch (AssertionException)
            {
                // Expected
            }
        }

        // ===== Quantified =====

        [Fact]
        public async Task IsNotEmpty_NonEmpty_Passes()
        {
            // Arrange
            var items = new[] { 1, 2, 3 };

            // Act & Verify
            await Stateless.Assert.That(items).IsNotEmpty();
        }

        [Fact]
        public async Task AnySatisfy_WithPredicate_MatchFound_Passes()
        {
            // Arrange
            var items = new[] { 1, 2, 3, 4, 5 };

            // Act & Verify
            await Stateless.Assert.That(items).AnySatisfy(x => x > 3);
        }

        [Fact]
        public async Task AnySatisfy_WithPredicate_NoMatch_Throws()
        {
            await Assert.ThrowsAsync<AssertionException>(async () =>
            {
            // Arrange
            var items = new[] { 1, 2, 3 };

            // Act
            await Stateless.Assert.That(items).AnySatisfy(x => x > 10);
            });
        }

        [Fact]
        public async Task AllSatisfy_AllMatch_Passes()
        {
            // Arrange
            var items = new[] { 2, 4, 6, 8 };

            // Act & Verify
            await Stateless.Assert.That(items).AllSatisfy(x => x % 2 == 0);  // All even
        }

        [Fact]
        public async Task AllSatisfy_OneDoesntMatch_Throws()
        {
            await Assert.ThrowsAsync<AssertionException>(async () =>
            {
            // Arrange
            var items = new[] { 2, 4, 5, 8 };  // 5 is odd

            // Act
            await Stateless.Assert.That(items).AllSatisfy(x => x % 2 == 0);
            });
        }

        // ===== Ordering =====

        [Fact]
        public async Task IsInAscendingOrder_SortedAsc_Passes()
        {
            // Arrange
            var items = new[] { 1, 2, 3, 4, 5 };

            // Act & Verify
            await Stateless.Assert.That(items).IsInAscendingOrder();
        }

        [Fact]
        public async Task IsOrdered_DefaultComparer_Passes()
        {
            // Arrange
            var items = new[] { 1, 2, 3, 4, 5 };

            // Act & Verify
            await Stateless.Assert.That(items).IsOrdered();
        }

        [Fact]
        public async Task IsOrdered_TrailingAscendingModifier_DescendingData_ThrowsWithAscendingMessage()
        {
            // Arrange
            var items = new[] { 5, 4, 3, 2, 1 };

            // Act
            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(items).IsOrdered().Ascending());
            await Stateless.Assert.That(message).Contains("ordered in ascending");
        }

        [Fact]
        public async Task IsOrdered_TrailingDescendingModifier_DescendingData_Passes()
        {
            // Arrange
            var items = new[] { 5, 4, 3, 2, 1 };

            // Act & Verify
            await Stateless.Assert.That(items).IsOrdered().Descending();
        }

        [Fact]
        public async Task IsOrdered_Unsorted_Throws()
        {
            // Arrange
            var items = new[] { 1, 3, 2, 4, 5 };

            // Act
            try
            {
                await Stateless.Assert.That(items).IsOrdered();
                await Stateless.Assert.Fail("Expected AssertionException to be thrown");
            }
            catch (AssertionException)
            {
                // Expected
            }
        }

        [Fact]
        public async Task IsOrderedAscending_DefaultComparer_Passes()
        {
            // Arrange
            var items = new[] { 1, 2, 3, 4, 5 };

            // Act & Verify
            await Stateless.Assert.That(items).IsOrderedAscending();
        }

        [Fact]
        public async Task IsOrderedDescending_DefaultComparer_Passes()
        {
            // Arrange
            var items = new[] { 5, 4, 3, 2, 1 };

            // Act & Verify
            await Stateless.Assert.That(items).IsOrderedDescending();
        }

        [Fact]
        public async Task IsInAscendingOrder_Unsorted_Throws()
        {
            // Arrange
            var items = new[] { 1, 3, 2, 4, 5 };

            // Act
            try
            {
                await Stateless.Assert.That(items).IsInAscendingOrder();
                await Stateless.Assert.Fail("Expected AssertionException to be thrown");
            }
            catch (AssertionException)
            {
                // Expected
            }
        }

        [Fact]
        public async Task IsInDescendingOrder_SortedDesc_Passes()
        {
            // Arrange
            var items = new[] { 5, 4, 3, 2, 1 };

            // Act & Verify
            await Stateless.Assert.That(items).IsInDescendingOrder();
        }

        [Fact]
        public async Task IsOrderedDescending_WrongOrder_Throws()
        {
            // Arrange
            var items = new[] { 1, 2, 3 };

            // Act
            try
            {
                await Stateless.Assert.That(items).IsOrderedDescending();
                await Stateless.Assert.Fail("Expected AssertionException to be thrown");
            }
            catch (AssertionException)
            {
                // Expected
            }
        }

        [Fact]
        public async Task IsOrderedAscending_WrongOrder_Throws()
        {
            // Arrange
            var items = new[] { 3, 2, 1 };

            // Act
            try
            {
                await Stateless.Assert.That(items).IsOrderedAscending();
                await Stateless.Assert.Fail("Expected AssertionException to be thrown");
            }
            catch (AssertionException)
            {
                // Expected
            }
        }

        // ===== Equality =====

        [Fact]
        public async Task Is_IdenticalCollections_Passes()
        {
            // Arrange
            var items1 = new[] { 1, 2, 3 };
            var items2 = new[] { 1, 2, 3 };

            // Act & Verify
            await Stateless.Assert.That(items1).IsEquivalentTo(items2);
        }

        [Fact]
        public async Task IsNotEquivalentTo_DifferentCollections_Passes()
        {
            // Arrange
            var items1 = new[] { 1, 2, 3 };
            var items2 = new[] { 1, 2, 4 };

            // Act & Verify
            await Stateless.Assert.That(items1).IsNotEquivalentTo(items2);
        }

        [Fact]
        public async Task IsNotEquivalentTo_EquivalentCollections_Throws()
        {
            // Arrange
            var items1 = new[] { 1, 2, 3 };
            var items2 = new[] { 3, 2, 1 };

            // Act & Verify
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(items1).IsNotEquivalentTo(items2));
            Assert.Contains("not to be equivalent", msg, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Is_DifferentCollections_Throws()
        {
            // Arrange
            var items1 = new[] { 1, 2, 3 };
            var items2 = new[] { 1, 2, 4 };

            // Act
            try
            {
                await Stateless.Assert.That(items1).Is(items2);
                await Stateless.Assert.Fail("Expected AssertionException to be thrown");
            }
            catch (AssertionException)
            {
                // Expected
            }
        }

        // ===== AmbientSoft Mode =====

        [Fact]
        public async Task CollectionAssertions_SoftMode_AccumulatesFailures()
        {
            // Arrange
            var verify = Asserter.NewSoft();
            var items = new[] { 1, 2, 3 };

            // Act
            await verify.That(items).HasCount(3);  // Pass
            await verify.That(items).HasCount(5);  // Fail
            await verify.That(items).Contains(4);  // Fail

            // Verify
            await Stateless.Assert.That(verify.ErrorCount).Is(2);
        }

        // ===== Nullable =====

        [Fact]
        public async Task CollectionAssertions_WithNull_IsNull_Passes()
        {
            // Arrange
            int[]? items = null;

            // Act & Verify
            await Stateless.Assert.That(items).IsNull();
        }

        [Fact]
        public async Task HasCountInRange_CountInsideRange_Passes()
        {
            var items = new[] { 1, 2, 3 };
            await Stateless.Assert.That(items).HasCountInRange(2, 4);
        }

        [Fact]
        public async Task HasSameCountAs_MatchingCounts_Passes()
        {
            var left = new[] { 1, 2, 3 };
            var right = new[] { 10, 20, 30 };
            await Stateless.Assert.That(left).HasSameCountAs(right);
        }

        [Fact]
        public async Task HasNullItems_WithNullElement_Passes()
        {
            string?[] items = ["a", null, "c"];
            await Stateless.Assert.That(items).HasNullItems();
        }

        [Fact]
        public async Task HasNoNullItems_WithoutNulls_Passes()
        {
            string?[] items = ["a", "b", "c"];
            await Stateless.Assert.That(items).HasNoNullItems();
        }

        [Fact]
        public async Task IntersectsWith_WithSharedElements_Passes()
        {
            var left = new[] { 1, 2, 3 };
            var right = new[] { 3, 4, 5 };
            await Stateless.Assert.That(left).IntersectsWith(right);
        }

        [Fact]
        public async Task ContainsEquivalentOf_passes_for_equivalent_object()
        {
            var items = new[] { "a", "bb", "ccc" };
            await Stateless.Assert.That(items).ContainsEquivalentOf("bb");
        }

        [Fact]
        public async Task ContainsEquivalentOf_fails_for_missing_item()
        {
            var items = new[] { "a", "bb", "ccc" };
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(items).ContainsEquivalentOf("dddd"));
            Assert.Contains("dddd", msg);
        }

        [Fact]
        public async Task ContainsItemsAssignableTo_passes_when_all_items_match_target_type()
        {
            object[] items = ["x", "y", "z"];
            await Stateless.Assert.That(items).ContainsItemsAssignableTo<object, string>();
        }

        [Fact]
        public async Task ContainsItemsAssignableTo_fails_when_no_item_matches_target_type()
        {
            object[] items = [1, 2, 3];
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(items).ContainsItemsAssignableTo<object, string>());
            Assert.Contains("string", msg, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task IsOrderedBy_passes_for_ascending_key_order()
        {
            var items = new[] { "a", "bb", "ccc" };
            await Stateless.Assert.That(items).IsOrderedBy(x => x.Length);
        }

        [Fact]
        public async Task IsOrderedBy_fails_for_unsorted_key_order()
        {
            var items = new[] { "aaa", "b", "cc" };
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(items).IsOrderedBy(x => x.Length));
            Assert.Contains("ordered", msg, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task IsOrderedDescendingBy_passes_for_descending_key_order()
        {
            var items = new[] { "ccc", "bb", "a" };
            await Stateless.Assert.That(items).IsOrderedDescendingBy(x => x.Length);
        }

        [Fact]
        public async Task IsOrderedDescendingBy_fails_for_wrong_order()
        {
            var items = new[] { "a", "ccc", "bb" };
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(items).IsOrderedDescendingBy(x => x.Length));
            Assert.Contains("descending", msg, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task SatisfyRespectively_passes_when_all_inspectors_match_items()
        {
            var items = new[] { 1, 2, 3 };
            await Stateless.Assert.That(items).SatisfyRespectively(
                i => Assert.Equal(1, i),
                i => Assert.Equal(2, i),
                i => Assert.Equal(3, i));
        }

        [Fact]
        public async Task SatisfyRespectively_fails_when_inspector_assertion_fails()
        {
            var items = new[] { 1, 2, 3 };
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(items).SatisfyRespectively(
                i => Assert.Equal(1, i),
                i => Assert.Equal(99, i),
                i => Assert.Equal(3, i)));
            Assert.Contains("99", msg);
        }

        [Fact]
        public async Task IsSequenceEqualTo_passes_for_same_order()
        {
            await Stateless.Assert.That(new[] { 1, 2, 3 }).IsSequenceEqualTo(1, 2, 3);
        }

        [Fact]
        public async Task IsSequenceEqualTo_fails_for_different_order()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(new[] { 1, 2, 3 }).IsSequenceEqualTo(1, 3, 2));
            Assert.Contains("sequence", msg, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task IsSubsetOf_passes_when_all_items_in_superset()
        {
            await Stateless.Assert.That(new[] { 1, 2 }).IsSubsetOf(1, 2, 3, 4);
        }

        [Fact]
        public async Task IsSubsetOf_fails_when_item_missing_in_superset()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(new[] { 1, 5 }).IsSubsetOf(1, 2, 3));
            Assert.Contains("subset", msg, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task IsSupersetOf_passes_when_contains_all_subset_items()
        {
            await Stateless.Assert.That(new[] { 1, 2, 3, 4 }).IsSupersetOf(2, 3);
        }

        [Fact]
        public async Task IsSupersetOf_fails_when_missing_subset_item()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(new[] { 1, 2 }).IsSupersetOf(2, 3));
            Assert.Contains("superset", msg, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task HasCountGreaterThan_passes_when_count_is_greater()
        {
            await Stateless.Assert.That(new[] { 1, 2, 3 }).HasCountGreaterThan(2);
        }

        [Fact]
        public async Task HasCountGreaterThan_fails_when_count_not_greater()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(new[] { 1, 2 }).HasCountGreaterThan(2));
            Assert.Contains("greater", msg, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task HasCountLessThan_passes_when_count_is_less()
        {
            await Stateless.Assert.That(new[] { 1, 2 }).HasCountLessThan(3);
        }

        [Fact]
        public async Task HasCountLessThan_fails_when_count_not_less()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(new[] { 1, 2, 3 }).HasCountLessThan(3));
            Assert.Contains("less", msg, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task HasCountGreaterThanOrEqualTo_passes_when_count_equal()
        {
            await Stateless.Assert.That(new[] { 1, 2, 3 }).HasCountGreaterThanOrEqualTo(3);
        }

        [Fact]
        public async Task HasCountLessThanOrEqualTo_passes_when_count_equal()
        {
            await Stateless.Assert.That(new[] { 1, 2, 3 }).HasCountLessThanOrEqualTo(3);
        }

        [Fact]
        public async Task NoneSatisfy_passes_when_no_items_match_predicate()
        {
            await Stateless.Assert.That(new[] { 1, 2, 3 }).NoneSatisfy(x => x > 10);
        }

        [Fact]
        public async Task NoneSatisfy_fails_when_any_item_matches_predicate()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(new[] { 1, 2, 3 }).NoneSatisfy(x => x > 2));
            Assert.Contains("no", msg, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task HasDistinctItemsBy_passes_when_projected_keys_unique()
        {
            await Stateless.Assert.That(new[] { "a1", "b2", "c3" }).HasDistinctItemsBy(x => x[0]);
        }

        [Fact]
        public async Task HasDistinctItemsBy_fails_when_projected_keys_repeat()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(new[] { "a1", "a2", "b3" }).HasDistinctItemsBy(x => x[0]));
            Assert.Contains("distinct", msg, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task HasDistinctItems_passes_when_items_are_unique()
        {
            await Stateless.Assert.That(new[] { 1, 2, 3 }).HasDistinctItems();
        }

        [Fact]
        public async Task HasDistinctItems_fails_when_items_repeat()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(new[] { 1, 2, 2, 3 }).HasDistinctItems());
            Assert.Contains("distinct", msg, StringComparison.OrdinalIgnoreCase);
        }
    }
}






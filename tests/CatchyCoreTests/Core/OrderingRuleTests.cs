using Catchy;
using Catchy.Sdk;
using CatchyCoreTests;
using CatchyCoreTests.Helpers;
using CatchyTestHelpers;
using System;

namespace CatchyCoreTests.Core
{
    [Collection("RegistryTests")]
    public class OrderingRuleTests : IDisposable
    {
        public OrderingRuleTests()
        {
            // Clean registry BEFORE this test runs
            RegistryTestHelper.ClearOrderingRuleRegistry();
        }

        public void Dispose()
        {
            // Clean up any registered global ordering rules after each test
            RegistryTestHelper.ClearOrderingRuleRegistry();
        }

        [Fact]
        public async Task IsOrdered_default_ascending_passes()
        {
            await Stateless.Assert.That([1, 2, 3, 4]).IsOrdered();
        }

        [Fact]
        public async Task IsOrdered_default_fails_on_descending()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(new[] { 4, 3, 2, 1 }).IsOrdered());
            Assert.Contains("Expected collection to be ordered in ascending", msg);
        }

        [Fact]
        public async Task IsOrdered_descending_rule_passes()
        {
            await Stateless.Assert.That([5, 4, 3, 2, 1])
                .IsOrdered(OrderingRules.Descending<int>());
        }

        [Fact]
        public async Task IsOrdered_custom_rule_by_length_passes()
        {
            await Stateless.Assert.That([ "a", "bb", "ccc" ])
                .IsOrdered(OrderingRules.ByLengthAscending());
        }

        [Fact]
        public async Task IsOrdered_alpha_ignore_case_passes()
        {
            await Stateless.Assert.That([ "apple", "Banana", "cherry" ])
                .IsOrdered(OrderingRules.AlphaAscendingIgnoreCase());
        }

        [Fact]
        public async Task Registered_ordering_rule_used_by_default()
        {
            // Register descending rule for int
            OrderingRuleRegistry.Register(OrderingRules.Descending<int>());

            await Stateless.Assert.That([ 10, 7, 3, 1 ]).IsOrdered();
        }

        [Fact]
        public async Task Custom_comparison_rule_passes()
        {
            await Stateless.Assert.That([ -3, -2, -1, 0, 1 ])
                .IsOrdered(OrderingRules.From<int>((a, b) => a.CompareTo(b)));
        }

        [Fact]
        public async Task Trailing_ordering_rule_applies_to_prior_collection_chain_via_user_api()
        {
            await Stateless.Assert.That([5, 4, 3, 2, 1])
                .IsOrdered()
                .And().IsInAscendingOrder()
                .With(OrderingRules.Descending<int>());
        }

        [Fact]
        public async Task Trailing_descending_modifier_applies_to_prior_ordered_assertion()
        {
            await Stateless.Assert.That([5, 4, 3, 2, 1])
                .IsOrdered()
                .Descending();
        }

        [Fact]
        public async Task Trailing_ascending_modifier_overrides_prior_descending_rule_for_ordered_assertion()
        {
            await Stateless.Assert.That([1, 2, 3, 4, 5])
                .IsOrdered()
                .With(OrderingRules.Descending<int>())
                .Ascending();
        }

        [Fact]
        public async Task Trailing_equals_options_apply_to_prior_collection_equivalence_via_user_api()
        {
            await Stateless.Assert.That(["A", "b"])
                .IsEquivalentTo(new[] { "a", "B" })
                .With(opts => opts.StringComparison = StringComparison.OrdinalIgnoreCase);
        }
    }
}

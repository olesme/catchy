using Catchy;
using CatchyCoreTests.Helpers;

namespace CatchyCoreTests.Assertions.Collections
{
    public class DictionaryAssertionTests
    {
        private static Dictionary<string, int> D() => new() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };

        [Fact] public async Task IsNotEmpty_passes() => await Stateless.Assert.That(D()).IsNotEmpty();
        [Fact] public async Task IsEmpty_passes() => await Stateless.Assert.That(new Dictionary<string, int>()).IsEmpty();
        [Fact] public async Task HasCountOf_passes() => await Stateless.Assert.That(D()).HasCountOf(3);
        [Fact] public async Task ContainsKey_passes() => await Stateless.Assert.That(D()).ContainsKey("b");
        [Fact] public async Task DoesNotContainKey_passes() => await Stateless.Assert.That(D()).DoesNotContainKey("z");
        [Fact] public async Task ContainsEntry_passes() => await Stateless.Assert.That(D()).ContainsEntry("a", 1);
        [Fact] public async Task IsNull_passes() => await Stateless.Assert.That((Dictionary<string, int>?)null).IsNull();
        [Fact] public async Task IsNotNull_passes() => await Stateless.Assert.That(D()).IsNotNull();

        [Fact]
        public async Task Works_with_IDictionary()
        {
            IDictionary<string, int> d = D();
            await Stateless.Assert.That(d).ContainsKey("a");
        }

        [Fact]
        public async Task Works_with_IReadOnlyDictionary()
        {
            IReadOnlyDictionary<string, int> d = D();
            await Stateless.Assert.That(d).ContainsKey("b");
        }

        [Fact]
        public async Task ContainsKey_fails_shows_key()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(D()).ContainsKey("z"));
            Assert.Contains("z", msg);
        }

        [Fact]
        public async Task ContainsEntry_fails_wrong_value()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(D()).ContainsEntry("a", 99));
            Assert.Contains("99", msg);
        }

        [Fact]
        public async Task Chain_passes()
            => await Stateless.Assert.That(D()).IsNotNull().And().IsNotEmpty().And().HasCountOf(3)
                .And().ContainsKey("a").And().ContainsEntry("b", 2);

        [Fact]
        public async Task Soft_mode_collects_failures()
        {
            var softAssert = new SoftAsserter();
            await softAssert.That(D()).ContainsKey("z");
            await softAssert.That(D()).HasCountOf(99);
            Assert.Equal(2, softAssert.ErrorCount);
        }

        [Fact]
        public async Task ContainsKey_returns_collection_value_assertions()
        {
            var d = new Dictionary<string, int> { ["a"] = 1 };
            var chain = Stateless.Assert.That(d);
            await chain.ContainsKey("a");
        }

        [Fact]
        public async Task IsEmpty_via_collection_chain_passes()
            => await Stateless.Assert.That(new Dictionary<string, int>()).IsEmpty();

        [Fact]
        public async Task HasCountOf_via_collection_chain_passes()
            => await Stateless.Assert.That(new Dictionary<string, int> { ["a"] = 1 }).HasCountOf(1);

        [Fact]
        public async Task ContainsAnyKey_passes_when_any_key_exists()
            => await Stateless.Assert.That(D()).ContainsAnyKey("z", "b");

        [Fact]
        public async Task ContainsNoneOfKeys_passes_when_no_keys_exist()
            => await Stateless.Assert.That(D()).ContainsNoneOfKeys("x", "y");

        [Fact]
        public async Task ContainsAllValues_passes()
            => await Stateless.Assert.That(D()).ContainsAllValues(1, 3);

        [Fact]
        public async Task AnyValueSatisfies_passes()
            => await Stateless.Assert.That(D()).AnyValueSatisfies(v => v > 2);

        [Fact]
        public async Task HasKeyWithValue_passes()
            => await Stateless.Assert.That(D()).HasKeyWithValue("c", v => v == 3);

        [Fact]
        public async Task KeysAreEquivalentTo_passes()
            => await Stateless.Assert.That(D()).KeysAreEquivalentTo("c", "a", "b");

        [Fact]
        public async Task ValuesAreEquivalentTo_passes()
            => await Stateless.Assert.That(D()).ValuesAreEquivalentTo(3, 1, 2);

        [Fact]
        public async Task HasDistinctValues_passes()
            => await Stateless.Assert.That(D()).HasDistinctValues();

        [Fact]
        public async Task ContainsAnyKey_fails_when_none_match()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(D()).ContainsAnyKey("x", "y"));
            Assert.Contains("x", msg);
        }

        [Fact]
        public async Task ContainsNoneOfKeys_fails_when_key_exists()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(D()).ContainsNoneOfKeys("z", "b"));
            Assert.Contains("b", msg);
        }

        [Fact]
        public async Task ContainsAllValues_fails_when_value_missing()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(D()).ContainsAllValues(1, 99));
            Assert.Contains("99", msg);
        }

        [Fact]
        public async Task AnyValueSatisfies_fails_when_none_match()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(D()).AnyValueSatisfies(v => v > 100));
            Assert.Contains("any", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task HasKeyWithValue_fails_when_predicate_does_not_match()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(D()).HasKeyWithValue("a", v => v > 10));
            Assert.Contains("a", msg);
        }

        [Fact]
        public async Task KeysAreEquivalentTo_fails_when_expected_keys_differ()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(D()).KeysAreEquivalentTo("a", "b", "z"));
            Assert.Contains("z", msg);
        }

        [Fact]
        public async Task ValuesAreEquivalentTo_fails_when_expected_values_differ()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(D()).ValuesAreEquivalentTo(1, 2, 99));
            Assert.Contains("99", msg);
        }

        [Fact]
        public async Task HasDistinctValues_fails_when_values_repeat()
        {
            var repeated = new Dictionary<string, int> { ["a"] = 1, ["b"] = 1, ["c"] = 2 };
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(repeated).HasDistinctValues());
            Assert.Contains("distinct", msg.ToLowerInvariant());
        }
    }
}

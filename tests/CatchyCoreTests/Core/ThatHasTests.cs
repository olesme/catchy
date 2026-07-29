using Catchy;
using CatchyCoreTests.Core;
using CatchyTestHelpers;

namespace CatchyCoreTests.Core
{
    public class ThatHasTests
    {
        private record Person(string Name, int Age, Address? Address = null);
        private record Address(string City, string Zip);

        [Fact]
        public async Task ThatHas_string_property_Is_passes()
        {
            var p = new Person("Alice", 30);
            await Stateless.Assert.That<Person>(p).ThatHas(x => x.Name).Is("Alice");
        }

        [Fact]
        public async Task ThatHas_numeric_via_Satisfies_passes()
        {
            var p = new Person("Alice", 30);
            await Stateless.Assert.That<Person>(p).ThatHas(x => x.Age).Satisfies(age => age >= 18);
        }

        [Fact]
        public async Task ThatHas_numeric_out_pattern_gives_proper_chain()
        {
            var p = new Person("Alice", 30);
            await Stateless.Assert.That<Person>(p).ThatHas(x => x.Age).Is(30);
        }

        [Fact]
        public async Task ThatHas_chained_twice_passes()
        {
            var p = new Person("Alice", 30, new Address("Kyiv", "01001"));
            await Stateless.Assert.That<Person>(p)
                .ThatHas(x => x.Address)
                .ThatHas(a => a!.City)
                .Is("Kyiv");
        }

        [Fact]
        public async Task ThatHas_Because_available_after_transition()
        {
            var p = new Person("Alice", 30);
            var msg = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That<Person>(p)
                    .ThatHas(x => x.Age)
                    .Satisfies(age => age >= 100)
                    .Because("must be a centenarian"));
            Assert.Contains("centenarian", msg);
        }

        [Fact]
        public async Task ThatHas_With_SoftState_flows_through()
        {
            var state = new SoftState();
            var p = new Person("Alice", 30);
            await Stateless.Assert.That<Person>(p)
                .ThatHas(x => x.Age)
                .Satisfies(age => age >= 100)
                .With(state);
            Assert.True(state.HasFailures);
        }

        [Fact]
        public async Task ThatHas_null_source_gives_clear_failure()
        {
            Person? p = null;
            var msg = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That<Person>(p).ThatHas(x => x!.Name).Is("Alice"));
            Assert.Contains("to equal \"Alice\"", msg);
        }

        [Fact]
        public async Task ThatHas_chain_link_contains_projection_expr()
        {
            var p = new Person("Bob", 25);
            var msg = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That<Person>(p).ThatHas(x => x.Age).Is(99));
            Assert.Contains("to be 99, but was 25", msg);
        }

        [Fact]
        public async Task ThatHas_IsNotNull_passes()
        {
            var p = new Person("Alice", 30, new Address("Kyiv", "01001"));
            await Stateless.Assert.That<Person>(p).ThatHas(x => x.Address).IsNotNull();
        }

        [Fact]
        public async Task ThatHas_IsNull_fails_with_message()
        {
            var p = new Person("Alice", 30, new Address("Kyiv", "01001"));
            var msg = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That<Person>(p).ThatHas(x => x.Address).IsNull());
            Assert.Contains("Expected null, but was", msg);
        }

        [Fact]
        public async Task ThatHas_Is_fails_shows_actual()
        {
            var p = new Person("Alice", 30);
            var msg = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That<Person>(p).ThatHas(x => x.Name).Is("Bob"));
            Assert.Contains("\"Alice\" to equal \"Bob\"", msg);
        }
    }
}

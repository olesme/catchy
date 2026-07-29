using Catchy;
using CatchyCoreTests.Helpers;

namespace CatchyCoreTests.Core
{
    public class DeepEqualTests
    {
        private record Person(string Name, int Age);
        private record PersonDto(string Name, int Age, string? Email = null);

        [Fact]
        public async Task IsEquivalentTo_same_type_passes()
        {
            var a = new Person("Alice", 30);
            var b = new Person("Alice", 30);
            await Stateless.Assert.That(a).IsEquivalentTo(b);
        }

        [Fact]
        public async Task IsEquivalentTo_different_values_fails()
        {
            var a = new Person("Alice", 30);
            var b = new Person("Alice", 31);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(a).IsEquivalentTo(b));
            Assert.Contains("Age", msg);
            Assert.Contains("30", msg);
            Assert.Contains("31", msg);
        }

        [Fact]
        public async Task IsEquivalentTo_with_configure_ignore_property_passes()
        {
            var a = new PersonDto("Alice", 30, Email: "a@test.com");
            var b = new PersonDto("Alice", 30, Email: "b@test.com");
            await Stateless.Assert.That(a).IsEquivalentTo(b)
                .With(opts => opts.ExcludedProperties.Add("Email"));
        }

        [Fact]
        public async Task IsEquivalentTo_with_string_ignore_case_passes()
        {
            var a = new Person("alice", 30);
            var b = new Person("ALICE", 30);
            await Stateless.Assert.That(a).IsEquivalentTo(b)
                .With(opts =>
                {
                    opts.StringComparison = StringComparison.OrdinalIgnoreCase;
                    opts.IgnoreCase = true;
                });
        }

        [Fact]
        public async Task DeepEqualRule_AutoFor_passes_when_equal()
        {
            var rule = DeepEqualRule.For<Person, PersonDto>();
            var person = new Person("Alice", 30);
            var dto = new PersonDto("Alice", 30);
            await Stateless.Assert.That(person).IsEquivalentTo(dto).With(rule);
        }

        [Fact]
        public async Task DeepEqualRule_AutoFor_fails_on_mismatch()
        {
            var rule = DeepEqualRule.For<Person, PersonDto>();
            var person = new Person("Alice", 30);
            var dto = new PersonDto("Bob", 30);
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(person).IsEquivalentTo(dto).With(rule));
            Assert.Contains("Name", msg);
        }

        [Fact]
        public async Task DeepEqualRule_with_Exclude_ignores_property()
        {
            var rule = DeepEqualRule.For<PersonDto, PersonDto>()
                .Exclude(p => p.Email);

            var a = new PersonDto("Alice", 30, "a@x.com");
            var b = new PersonDto("Alice", 30, "b@x.com");
            await Stateless.Assert.That(a).IsEquivalentTo(b).With(rule);
        }

        [Fact]
        public async Task DeepEqualRule_with_Match_custom_projection_passes()
        {
            var rule = DeepEqualRule.For<Person, PersonDto>()
                .Match(p => p.Name.ToLower(), d => d.Name.ToLower())
                .Match(p => p.Age, d => d.Age);

            var person = new Person("ALICE", 30);
            var dto = new PersonDto("alice", 30);
            await Stateless.Assert.That(person).IsEquivalentTo(dto).With(rule);
        }

        [Fact]
        public async Task DeepEqualRule_inline_configure_passes()
        {
            var person = new Person("Alice", 30);
            var dto = new PersonDto("Alice", 30, Email: "ignored@x.com");

            await Stateless.Assert.That(person).IsEquivalentTo(dto)
                .With((DeepEqualRule<Person, PersonDto> b) => b
                    .WithAutoMatch()
                    .Exclude(p => p.Name) // exclude Name comparison
                    .Match(p => p.Age, d => d.Age));
        }
    }
}

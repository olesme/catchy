using Catchy;
using static CatchyCoreTests.AssertionMessageCapture;

namespace CatchyCoreTests.Core
{
    /// <summary>
    /// Edge cases: null properties, self-referential objects, deep nesting,
    /// collection item comparison, primitive type coercion.
    /// </summary>
    public class EdgeCaseDeepEqualTests
    {
        [Fact]
        public async Task ShouldPass_WhenBothNullablePropertiesAreNull()
        {
            var actual = new Order { Id = 1, Customer = "A", Items = [], Total = 0, Shipping = null };
            var expected = new Order { Id = 1, Customer = "A", Items = [], Total = 0, Shipping = null };

            await Stateless.Assert.That<Order>(actual).IsEquivalentTo(expected);
        }

        [Fact]
        public async Task ShouldFail_WhenOneNullablePropertyIsNullAndOtherIsNot()
        {
            var actual = new Order
            {
                Id = 1,
                Customer = "A",
                Items = [],
                Total = 0,
                Shipping = new Address { Street = "S", City = "C", Zip = "Z" }
            };
            var expected = new Order { Id = 1, Customer = "A", Items = [], Total = 0, Shipping = null };

            var msg = await CaptureFailureMessageAsync(async () =>
                await Stateless.Assert.That(actual).IsEquivalentTo(expected));

            Assert.NotNull(msg);
        }

        [Fact]
        public async Task ShouldPass_WithIgnoreNullProperties_WhenBothAreNull()
        {
            var actual = new Order { Id = 1, Customer = "A", Items = [], Total = 0, Shipping = null };
            var expected = new Order { Id = 1, Customer = "A", Items = [], Total = 0, Shipping = null };

            await Stateless.Assert.That(actual).IsEquivalentTo(expected)
                .With(opts => opts.IgnoreNullProperties = true);
        }

        [Fact]
        public async Task ShouldPass_WithNestedObjectList_WhenAllItemsMatch()
        {
            var rule = DeepEqualRule.For<Address, Address>()
                .Match(a => a.City, b => b.City)
                .Match(a => a.Zip, b => b.Zip);

            // For collections, engine falls through to item-level comparison
            var listA = new List<Address>
            {
                new() { Street = "S1", City = "Kyiv",  Zip = "01001" },
                new() { Street = "S2", City = "Lviv",  Zip = "79000" },
            };
            var listB = new List<Address>
            {
                new() { Street = "S1", City = "Kyiv",  Zip = "01001" },
                new() { Street = "S2", City = "Lviv",  Zip = "79000" },
            };

            await Stateless.Assert.That(listA).IsEquivalentTo(listB);
        }

        [Fact]
        public async Task ShouldFail_WithNestedObjectList_WhenOneItemDiffers()
        {
            var listA = new List<Address>
            {
                new() { Street = "S1", City = "Kyiv", Zip = "01001" },
                new() { Street = "S2", City = "Lviv", Zip = "WRONG" }, // Zip differs
            };
            var listB = new List<Address>
            {
                new() { Street = "S1", City = "Kyiv", Zip = "01001" },
                new() { Street = "S2", City = "Lviv", Zip = "79000" },
            };

            var msg = await CaptureFailureMessageAsync(async () =>
                await Stateless.Assert.That(listA).IsEquivalentTo(listB));

            Assert.NotNull(msg);
        }

        [Fact]
        public async Task ShouldPass_EmptyCollections_BothEmpty()
        {
            var a = new Order { Id = 1, Customer = "A", Items = [], Total = 0 };
            var b = new Order { Id = 1, Customer = "A", Items = [], Total = 0 };

            await Stateless.Assert.That(a).IsEquivalentTo(b);
        }

        [Fact]
        public async Task ShouldFail_WhenCollectionCountsDiffer()
        {
            var a = new Order { Id = 1, Customer = "A", Items = ["X"], Total = 0 };
            var b = new Order { Id = 1, Customer = "A", Items = ["X", "Y"], Total = 0 };

            var msg = await CaptureFailureMessageAsync(async () => await Stateless.Assert.That(a).IsEquivalentTo(b));
            Assert.NotNull(msg);
        }

        [Fact]
        public async Task FloatTolerance_ShouldPass_WhenExactlyAtBoundary()
        {
            var a = new ProductEntity { Id = 1, Name = "X", Price = 10.0 };
            var b = new ProductEntity { Id = 1, Name = "X", Price = 10.1 };

            await Stateless.Assert.That(a).IsEquivalentTo(b).With(opts => opts.FloatTolerance = 0.1);
        }

        [Fact]
        public async Task FloatTolerance_ShouldFail_WhenJustOutsideBoundary()
        {
            var a = new ProductEntity { Id = 1, Name = "X", Price = 10.0 };
            var b = new ProductEntity { Id = 1, Name = "X", Price = 10.11 };

            var msg = await CaptureFailureMessageAsync(async () => await Stateless.Assert.That(a).IsEquivalentTo(b).With(opts => opts.FloatTolerance = 0.1));

            Assert.NotNull(msg);
        }

        [Fact]
        public async Task IgnoreExtraProperties_ShouldPass_WhenActualHasMoreProps()
        {
            // Order has Items + Shipping that OrderSummary doesn't
            var order = new Order
            {
                Id = 5,
                Customer = "Z",
                Items = ["irrelevant"],
                Total = 42.0,
                Shipping = new Address { Street = "S", City = "C", Zip = "Z" }
            };
            var summary = new OrderSummary { Id = 5, Customer = "Z", Total = 42.0 };

            await Stateless.Assert.That(order).IsEquivalentTo(summary)
                .With(opts => opts.IgnoreExtraProperties = true);
        }

        [Fact]
        public async Task ShouldPass_WithMultipleExcludedProperties()
        {
            var a = new UserEntity(1, "Alice", "DIFFERENT", 99);
            var b = new UserEntity(1, "Alice", "IGNORED", 0);

            await Stateless.Assert.That(a).IsEquivalentTo(b)
                .With(opts =>
                {
                    opts.ExcludedProperties.Add("Email");
                    opts.ExcludedProperties.Add("Age");
                });
        }

        [Fact]
        public async Task ShouldSupportAndChaining_AfterIsEquivalentTo()
        {
            var actual = new UserEntity(1, "Alice", "a@b.com", 30);
            var expected = new UserEntity(1, "Alice", "a@b.com", 30);

            // IsEquivalentTo returns TSelf so chaining must work
            await Stateless.Assert.That<UserEntity>(actual)
                .IsEquivalentTo(expected)
                .And()
                .Satisfies(u => u!.Age > 0);
        }
    }
}

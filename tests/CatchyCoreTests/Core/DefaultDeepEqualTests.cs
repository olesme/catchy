using Catchy;

using static CatchyCoreTests.AssertionMessageCapture;

namespace CatchyCoreTests.Core
{
    /// <summary>
    /// DefaultStateless deep-equal (no explicit rule): same-type property-walk comparisons.
    /// </summary>
    public class DefaultDeepEqualTests
    {
        [Fact]
        public async Task ShouldPass_WhenObjectsHaveIdenticalProperties()
        {
            var actual = new UserEntity(1, "Alice", "alice@test.com", 30);
            var expected = new UserEntity(1, "Alice", "alice@test.com", 30);

            await Stateless.Assert.That<UserEntity>(actual).IsEquivalentTo(expected);
        }

        [Fact]
        public async Task ShouldPass_ForNestedObjects_WhenAllPropertiesMatch()
        {
            var actual = new Order
            {
                Id = 42,
                Customer = "Alice",
                Items = ["A", "B"],
                Total = 99.99,
                Shipping = new Address { Street = "Main St", City = "Kyiv", Zip = "01001" }
            };
            var expected = new Order
            {
                Id = 42,
                Customer = "Alice",
                Items = ["A", "B"],
                Total = 99.99,
                Shipping = new Address { Street = "Main St", City = "Kyiv", Zip = "01001" }
            };

            await Stateless.Assert.That<Order>(actual).IsEquivalentTo(expected);
        }

        [Fact]
        public async Task ShouldPass_WhenCollectionOrderMatches()
        {
            var actual = new Order { Id = 1, Customer = "Bob", Items = ["X", "Y", "Z"], Total = 0 };
            var expected = new Order { Id = 1, Customer = "Bob", Items = ["X", "Y", "Z"], Total = 0 };

            await Stateless.Assert.That<Order>(actual).IsEquivalentTo(expected);
        }

        [Fact]
        public async Task ShouldPass_WithIgnoreCollectionOrder_WhenElementsMatch()
        {
            var actual = new Order { Id = 1, Customer = "Bob", Items = ["Z", "X", "Y"], Total = 0 };
            var expected = new Order { Id = 1, Customer = "Bob", Items = ["X", "Y", "Z"], Total = 0 };

            await Stateless.Assert.That<Order>(actual).IsEquivalentTo(expected).With(opts => opts.IgnoreCollectionOrder = true);
        }

        [Fact]
        public async Task ShouldPass_WithExcludedProperty_WhenOnlyExcludedDiffers()
        {
            var actual = new UserEntity(1, "Alice", "alice@test.com", 30);
            var expected = new UserEntity(1, "Alice", "different@test.com", 30); // Email differs

            await Stateless.Assert.That(actual).IsEquivalentTo(expected).With(opts => opts.ExcludedProperties.Add("Email"));
        }

        [Fact]
        public async Task ShouldPass_WithFloatTolerance_WhenDifferenceWithinBounds()
        {
            var actual = new ProductEntity { Id = 1, Name = "Widget", Price = 9.999 };
            var expected = new ProductEntity { Id = 1, Name = "Widget", Price = 10.0 };

            await Stateless.Assert.That(actual).IsEquivalentTo(expected).With(opts => opts.FloatTolerance = 0.01);
        }

        [Fact]
        public async Task ShouldPass_WithCaseInsensitiveStringComparison()
        {
            var actual = new UserEntity(1, "ALICE", "alice@test.com", 30);
            var expected = new UserEntity(1, "alice", "alice@test.com", 30);

            await Stateless.Assert.That(actual).IsEquivalentTo(expected).With(opts => opts.StringComparison = StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task ShouldFail_WhenTopLevelPropertyDiffers()
        {
            var actual = new UserEntity(1, "Alice", "alice@test.com", 30);
            var expected = new UserEntity(1, "Bob", "alice@test.com", 30); // Name differs

            var msg = await CaptureFailureMessageAsync(async () =>
                await Stateless.Assert.That(actual).IsEquivalentTo(expected));

            Assert.NotNull(msg);
            Assert.Contains("Name", msg);
        }

        [Fact]
        public async Task ShouldFail_WhenNestedObjectPropertyDiffers()
        {
            var actual = new Order
            {
                Id = 1,
                Customer = "A",
                Items = [],
                Total = 0,
                Shipping = new Address { Street = "Main St", City = "Kyiv", Zip = "01001" }
            };
            var expected = new Order
            {
                Id = 1,
                Customer = "A",
                Items = [],
                Total = 0,
                Shipping = new Address { Street = "Main St", City = "Lviv", Zip = "01001" } // City differs
            };

            var msg = await CaptureFailureMessageAsync(async () =>
                await Stateless.Assert.That(actual).IsEquivalentTo(expected));

            Assert.NotNull(msg);
        }

        [Fact]
        public async Task ShouldFail_WhenCollectionOrderDiffers_AndOrderNotIgnored()
        {
            var actual = new Order { Id = 1, Customer = "A", Items = ["Z", "X"], Total = 0 };
            var expected = new Order { Id = 1, Customer = "A", Items = ["X", "Z"], Total = 0 };

            var msg = await CaptureFailureMessageAsync(async () =>
                await Stateless.Assert.That(actual).IsEquivalentTo(expected));

            Assert.NotNull(msg);
        }

        [Fact]
        public async Task ShouldFail_WithFloatTolerance_WhenDifferenceExceedsBounds()
        {
            var actual = new ProductEntity { Id = 1, Name = "W", Price = 9.5 };
            var expected = new ProductEntity { Id = 1, Name = "W", Price = 10.0 };

            var msg = await CaptureFailureMessageAsync(async () =>
                await Stateless.Assert.That(actual).IsEquivalentTo(expected).With(opts => opts.FloatTolerance = 0.1));

            Assert.NotNull(msg);
        }

        [Fact]
        public async Task ErrorMessage_ShouldContainAssertionChain()
        {
            var actual = new UserEntity(1, "Alice", "a@b.com", 30);
            var expected = new UserEntity(1, "Bob", "a@b.com", 30);

            var msg = await CaptureFailureMessageAsync(async () =>
                await Stateless.Assert.That(actual).IsEquivalentTo(expected));

            Assert.NotNull(msg);
            Assert.Contains("IsEquivalentTo", msg);
        }

        [Fact]
        public async Task ErrorMessage_ShouldContainDiff_WithMismatchedPropertyValues()
        {
            var actual = new UserEntity(1, "Alice", "alice@a.com", 30);
            var expected = new UserEntity(2, "Bob", "bob@b.com", 25);

            var msg = await CaptureFailureMessageAsync(async () =>
                await Stateless.Assert.That(actual).IsEquivalentTo(expected));

            Assert.NotNull(msg);
            // Message should show at least one differing field
            Assert.True(
                msg.Contains("Id") || msg.Contains("Name") || msg.Contains("differ"),
                $"Expected diff info in message, got:\n{msg}");
        }

        [Fact]
        public async Task ErrorMessage_ShouldMentionNullActual_WhenActualIsNull()
        {
            UserEntity? actual = null;
            var expected = new UserEntity(1, "Alice", "a@b.com", 30);

            var msg = await CaptureFailureMessageAsync(async () =>
                await Stateless.Assert.That(actual).IsEquivalentTo(expected));

            Assert.NotNull(msg);
            Assert.Contains("null", msg, StringComparison.OrdinalIgnoreCase);
        }
    }
}

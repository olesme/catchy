using Catchy;
using CatchyCoreTests.Core;
using CatchyTestHelpers;

namespace CatchyCoreTests.Core
{
    /// <summary>
    /// Duck-typing: compare objects of different types by matching property names.
    /// Previously broken — <c>ArePropertiesEqual</c> called <c>prop.GetValue(b)</c>
    /// with a PropertyInfo from typeA, causing TargetException on different types.
    /// </summary>
    public class DuckTypingTests
    {
        [Fact]
        public async Task ShouldPass_WhenAnonymousTypeMatchesEntityProperties()
        {
            var actual = new UserEntity(1, "Alice", "alice@test.com", 30);

            // Anonymous type — different CLR type, same property names/values
            await Stateless.Assert.That<UserEntity>(actual).IsEquivalentTo(new { Id = 1, Name = "Alice", Email = "alice@test.com", Age = 30 });
        }

        [Fact]
        public async Task ShouldPass_WithPartialAnonymousType_WhenIgnoringExtraProperties()
        {
            var actual = new UserEntity(1, "Alice", "alice@test.com", 30);

            // Anonymous type with only some properties — actual has MORE properties
            await Stateless.Assert.That(actual).IsEquivalentTo(
                new { Name = "Alice", Age = 30 })
                .With(opts => opts.IgnoreExtraProperties = true);
        }

        [Fact]
        public async Task ShouldFail_WhenAnonymousTypePropertyValueDiffers()
        {
            var actual = new UserEntity(1, "Alice", "alice@test.com", 30);

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(actual).IsEquivalentTo(new { Id = 1, Name = "Bob", Email = "alice@test.com", Age = 30 }));

            Assert.Contains("Name: \"Alice\" != \"Bob\"", message);
        }

        [Fact]
        public async Task ShouldFail_WithPartialAnonymousType_WhenExtraPropertiesNotIgnored()
        {
            var actual = new UserEntity(1, "Alice", "alice@test.com", 30);

            // Without IgnoreExtraProperties: actual has Email+Age that anonymous type doesn't — mismatch
            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(actual).IsEquivalentTo(new { Id = 1, Name = "Alice" }));

            // Should fail because expected has fewer properties and we're not ignoring extras
            Assert.Contains("present on actual, missing on expected", message);
        }

        [Fact]
        public async Task ShouldPass_AnonymousType_WithCaseInsensitiveStringComparison()
        {
            var actual = new UserEntity(1, "ALICE", "alice@test.com", 30);

            await Stateless.Assert.That(actual).IsEquivalentTo(
                new { Id = 1, Name = "alice", Email = "alice@test.com", Age = 30 })
                .With(opts => opts.StringComparison = StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task ShouldPass_AnonymousType_WithExcludedProperty()
        {
            var actual = new UserEntity(1, "Alice", "original@test.com", 30);

            await Stateless.Assert.That(actual).IsEquivalentTo(
                new { Id = 1, Name = "Alice", Email = "different@test.com", Age = 30 })
                .With(opts => opts.ExcludedProperties.Add("Email"));
        }

        [Fact]
        public async Task ShouldPass_WhenDtoMatchesEntityWithSameProperties()
        {
            var entity = new UserEntity(1, "Alice", "alice@test.com", 30);
            var dto = new UserDto(1, "Alice", "alice@test.com", 30);

            await Stateless.Assert.That(entity).IsEquivalentTo(dto);
        }

        [Fact]
        public async Task ShouldFail_WhenDtoPropertyDiffersFromEntity()
        {
            var entity = new UserEntity(1, "Alice", "alice@test.com", 30);
            var dto = new UserDto(1, "Alice", "alice@test.com", 99); // Age differs

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(entity).IsEquivalentTo(dto));

            Assert.Contains("Age: 30 != 99", message);
        }

        [Fact]
        public async Task ShouldPass_OrderEntity_AgainstPartialSummary_WhenIgnoringExtras()
        {
            var order = new Order
            {
                Id = 10,
                Customer = "Alice",
                Items = ["Book", "Pen"],
                Total = 55.0,
                Shipping = new Address { Street = "S", City = "C", Zip = "Z" }
            };

            // OrderSummary only has Id, Customer, Total
            var summary = new OrderSummary { Id = 10, Customer = "Alice", Total = 55.0 };

            await Stateless.Assert.That(order).IsEquivalentTo(summary)
                .With(opts => opts.IgnoreExtraProperties = true);
        }

        [Fact]
        public async Task ShouldFail_OrderEntity_AgainstPartialSummary_WhenTotalDiffers()
        {
            var order = new Order
            {
                Id = 10,
                Customer = "Alice",
                Items = [],
                Total = 55.0,
                Shipping = null
            };
            var summary = new OrderSummary { Id = 10, Customer = "Alice", Total = 99.0 };

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(order).IsEquivalentTo(summary)
                    .With(opts => opts.IgnoreExtraProperties = true));

            Assert.Contains("  Total: 55 != 99", message);
        }

        [Fact]
        public async Task ErrorMessage_ShouldNotThrowTargetException_ForCrossTypeComparison()
        {
            // Previously threw System.Reflection.TargetException
            var actual = new UserEntity(1, "Alice", "a@b.com", 30);

            var ex = await Record.ExceptionAsync(async () =>
                await Stateless.Assert.That(actual).IsEquivalentTo(new { Id = 1, Name = "Bob", Email = "a@b.com", Age = 30 }));

            Assert.IsType<AssertionException>(ex); // AssertionException, NOT TargetException
        }

        [Fact]
        public async Task ErrorMessage_ShouldMentionDifferingProperty_ForAnonymousType()
        {
            var actual = new UserEntity(1, "Alice", "a@b.com", 30);

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(actual).IsEquivalentTo(new { Id = 1, Name = "Bob", Email = "a@b.com", Age = 30 }));

            Assert.NotNull(message);
            Assert.Contains("  Name: \"Alice\" != \"Bob\"", message);
        }
    }

    // Extension to convert awaitable assertion to Task for Record.ExceptionAsync
    file static class ValueAssertionsExtensions
    {
        public static Task AsTask(this System.Runtime.CompilerServices.TaskAwaiter awaiter)
        {
            var tcs = new TaskCompletionSource();
            try { awaiter.GetResult(); tcs.SetResult(); }
            catch (Exception ex) { tcs.SetException(ex); }
            return tcs.Task;
        }
    }
}

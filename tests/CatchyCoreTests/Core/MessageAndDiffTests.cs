using Catchy;
using CatchyCoreTests.Core;
using CatchyTestHelpers;

namespace CatchyCoreTests.Core
{
    /// <summary>
    /// Verifies that error messages are informative: contain diffs, chain info,
    /// Because() context, and helpful null messages.
    /// </summary>
    public class MessageAndDiffTests
    {
        [Fact]
        public async Task Diff_ShouldShowPropertyName_WhenTopLevelPropDiffers()
        {
            var actual = new UserEntity(1, "Alice", "a@b.com", 30);
            var expected = new UserEntity(1, "Alice", "a@b.com", 99); // Age differs

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(actual).IsEquivalentTo(expected));

            Assert.NotNull(message);
            Assert.Contains("  Age: 30 != 99", message);
        }

        [Fact]
        public async Task Diff_ShouldShowBothActualAndExpectedValues()
        {
            var actual = new UserEntity(1, "Alice", "a@b.com", 30);
            var expected = new UserEntity(1, "Alice", "a@b.com", 99);

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(actual).IsEquivalentTo(expected));

            Assert.NotNull(message);
            Assert.Contains("  Age: 30 != 99", message);
        }

        [Fact]
        public async Task Diff_ShouldShowMultipleMismatchedProperties()
        {
            var actual = new UserEntity(1, "Alice", "a@b.com", 30);
            var expected = new UserEntity(2, "Bob", "b@c.com", 25);

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(actual).IsEquivalentTo(expected));

            Assert.NotNull(message);
            Assert.Contains("  Name: \"Alice\" != \"Bob\"", message);
            Assert.Contains("  Id: 1 != 2", message);
        }

        [Fact]
        public async Task Diff_ForAnonymousType_ShouldMentionDifferingProp()
        {
            var actual = new UserEntity(1, "Alice", "a@b.com", 30);
            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(actual).IsEquivalentTo(new { Id = 1, Name = "Bob", Email = "a@b.com", Age = 30 }));

            Assert.NotNull(message);
            Assert.Contains("  Name: \"Alice\" != \"Bob\"", message);
        }

        [Fact]
        public async Task Diff_ForTypedRule_ShouldMentionDifferingMappedProp()
        {
            var entity = new UserEntity(1, "Alice", "a@b.com", 30);
            var dto = new UserDto(1, "Alice", "a@b.com", 99);

            var rule = DeepEqualRule.For<UserEntity, UserDto>();

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That<UserEntity>(entity).IsEquivalentTo(dto).With(rule));

            Assert.NotNull(message);
            Assert.Contains("  Age: 30 != 99", message);
        }

        [Fact]
        public async Task Message_ShouldContainAssertionChain_WithHardAssertAndThat()
        {
            var actual = new UserEntity(1, "Alice", "a@b.com", 30);
            var expected = new UserEntity(1, "Bob", "a@b.com", 30);

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(actual).IsEquivalentTo(expected));

            Assert.NotNull(message);
            // Chain header
            Assert.Contains("Assertion failed:", message);
            Assert.Contains("Stateless.Assert", message);
            Assert.Contains(".That(", message);
            Assert.Contains("IsEquivalentTo", message);
            // Body
            Assert.Contains("but differ:", message);
            Assert.Contains("  Name: \"Alice\" != \"Bob\"", message);
        }

        [Fact]
        public async Task Message_ShouldContainBecauseReason_WhenBecauseIsChained()
        {
            var actual = new UserEntity(1, "Alice", "a@b.com", 30);
            var expected = new UserEntity(1, "Bob", "a@b.com", 30);

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(actual)
                    .IsEquivalentTo(expected)
                    .Because("the user name must match the session"));

            Assert.NotNull(message);
            Assert.Contains("Because  : the user name must match the session", message);
            Assert.Contains("  Name: \"Alice\" != \"Bob\"", message);
        }

        [Fact]
        public async Task Message_WhenActualIsNull_ShouldSayExpectedValue()
        {
            UserEntity? actual = null;
            var expected = new UserEntity(1, "Alice", "a@b.com", 30);

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(actual).IsEquivalentTo(expected));

            Assert.Contains("Expected a value, but was null", message);
        }

        [Fact]
        public async Task Message_WhenExpectedIsNull_ShouldMentionNull()
        {
            var actual = new UserEntity(1, "Alice", "a@b.com", 30);

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(actual).IsEquivalentTo((UserEntity?)null));

            Assert.Contains("Expected equivalent to null, but was", message);
        }

        [Fact]
        public async Task SoftMode_AggregateMessage_ShouldContainAllFailures()
        {
            var softAssert = new SoftAsserter();

            var actual = new UserEntity(1, "Alice", "a@b.com", 30);

            await softAssert.That(actual).IsEquivalentTo(new UserEntity(2, "Alice", "a@b.com", 30)); // Id differs
            await softAssert.That(actual).IsEquivalentTo(new UserEntity(1, "Bob", "a@b.com", 30)); // Name differs

            Assert.Equal(2, softAssert.ErrorCount);

            var aggEx = await Assert.ThrowsAsync<AggregateAssertionException>(async () => throw softAssert.SoftState.AggregateException!);
            Assert.NotNull(aggEx);
            Assert.Equal(2, aggEx.InnerExceptions.Count);
        }

        [Fact]
        public async Task Message_ShouldNotContainTarget_Exception_ForCrossTypeComparison()
        {
            var actual = new UserEntity(1, "Alice", "a@b.com", 30);

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(actual).IsEquivalentTo(new { Id = 1, Name = "Bob", Email = "a@b.com", Age = 30 }));

            Assert.NotNull(message);
            Assert.DoesNotContain("TargetException", message);
            Assert.DoesNotContain("Object does not match target type", message);
        }
    }
}

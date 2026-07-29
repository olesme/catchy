using Catchy;
using CatchyTestHelpers;
using static CatchyCoreTests.AssertionMessageCapture;

namespace CatchyCoreTests.Core
{
    [CollectionDefinition("RegistryTests", DisableParallelization = true)]
    public class RegistryCollection { }

    [Collection("RegistryTests")]
    /// <summary>
    /// Global DeepEqualRuleRegistry: register once, apply automatically in all assertions.
    /// Each test isolates registry state via RegistryScope.
    /// </summary>
    public class RegistryTests : IDisposable
    {
        public void Dispose()
        {
            // Clean up any registered global rules after each test
            RegistryTestHelper.ClearDeepEqualRuleRegistry();
            // Also clean up per-instance containers to prevent state leakage
            RegistryTestHelper.ClearPerInstanceContainers();
        }
        [Fact]
        public async Task RegisteredRule_ShouldBeUsedAutomatically_InDefaultIsEquivalentTo()
        {
            // Register a rule that maps only Id + Name (ignores Email, Age)
            DeepEqualRule.For<UserEntity, UserDto>()
                .Match(e => e.Id, d => d.Id)
                .Match(e => e.Name, d => d.Name)
                .Register();

            var entity = new UserEntity(1, "Alice", "ignored@x.com", 99); // Email/Age differ
            var dto = new UserDto(1, "Alice", "also-ignored", 0);

            // Should pass because registered rule only checks Id + Name
            await Stateless.Assert.That(entity).IsEquivalentTo((object)dto);
        }

        [Fact]
        public async Task RegisteredRule_ShouldFail_WhenMappedPropertyDiffers()
        {
            DeepEqualRule.For<UserEntity, UserDto>()
                .Match(e => e.Id, d => d.Id)
                .Match(e => e.Name, d => d.Name)
                .Register();

            var entity = new UserEntity(1, "Alice", "a@b.com", 30);
            var dto = new UserDto(1, "Bob", "a@b.com", 30); // Name differs

            var msg = await CaptureFailureMessageAsync(async () =>
                await Stateless.Assert.That(entity).IsEquivalentTo((object)dto));

            Assert.NotNull(msg);
        }

        [Fact]
        public async Task RegisteredAutoMatchRule_ShouldCompareAllCommonProperties()
        {
            DeepEqualRule.For<UserEntity, UserDto>().Register();

            var entity = new UserEntity(1, "Alice", "a@b.com", 30);
            var dto = new UserDto(1, "Alice", "a@b.com", 30);

            await Stateless.Assert.That(entity).IsEquivalentTo((object)dto);
        }

        [Fact]
        public async Task Register_WithReplaceFalse_ShouldThrow()
        {
            // First registration: Name only
            DeepEqualRule.For<UserEntity, UserDto>()
                .Match(e => e.Name, d => d.Name)
                .Register();

            await Assert.ThrowsAsync<InvalidOperationException>(async () => DeepEqualRule.For<UserEntity, UserDto>()
                .Match(e => e.Name, d => d.Name)
                .Match(e => e.Age, d => d.Age)
                .Register());
        }

        [Fact]
        public async Task RegisteredRule_ShouldWorkInSoftMode()
        {
            DeepEqualRule.For<UserEntity, UserDto>()
                .Match(e => e.Id, d => d.Id)
                .Register();

            var softAssert = new SoftAsserter();

            var entity = new UserEntity(1, "Alice", "a@b.com", 30);
            var dto = new UserDto(99, "Alice", "a@b.com", 30); // Id differs

            await softAssert.That(entity).IsEquivalentTo((object)dto);

            Assert.True(softAssert.HasFailures);
            Assert.Single(softAssert.Errors);
        }
    }
}

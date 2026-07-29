using Catchy;
using CatchyTestHelpers;
using static CatchyCoreTests.AssertionMessageCapture;

namespace CatchyCoreTests.Core
{
    /// <summary>
    /// Typed DeepEqualRule with explicit Match / Exclude mappings.
    /// Covers: inline builder passed to assertion, inline configure lambda, registry.
    /// </summary>
    public class CustomRuleTests : IDisposable
    {
        public void Dispose()
        {
            // Clean up any registered global rules after each test
            RegistryTestHelper.ClearDeepEqualRuleRegistry();
            // Also clean up per-instance containers to prevent state leakage
            RegistryTestHelper.ClearPerInstanceContainers();
        }
        // Overload (2): pre-built rule passed directly

        [Fact]
        public async Task PreBuiltRule_ShouldPass_WhenMappedPropertiesMatch()
        {
            var entity = new UserEntity(1, "Alice", "alice@test.com", 30);
            var dto = new UserDto(99, "Alice", "different@test.com", 30); // Id and Email differ

            // Rule: only compare Name + Age
            var rule = DeepEqualRule.For<UserEntity, UserDto>()
                .Match(e => e.Name, d => d.Name)
                .Match(e => e.Age, d => d.Age);

            await Stateless.Assert.That<UserEntity>(entity).IsEquivalentTo(dto).With(rule);
        }

        [Fact]
        public async Task PreBuiltRule_ShouldFail_WhenMappedPropertyDiffers()
        {
            var entity = new UserEntity(1, "Alice", "a@b.com", 30);
            var dto = new UserDto(1, "Bob", "a@b.com", 30); // Name differs

            var rule = DeepEqualRule.For<UserEntity, UserDto>()
                .Match(e => e.Name, d => d.Name)
                .Match(e => e.Age, d => d.Age);

            var msg = await CaptureFailureMessageAsync(async () =>
                await Stateless.Assert.That<UserEntity>(entity).IsEquivalentTo(dto).With(rule));

            Assert.NotNull(msg);
            Assert.Contains("Name", msg);
        }

        [Fact]
        public async Task PreBuiltRule_WithAutoMatch_ShouldMapCommonPropertiesAutomatically()
        {
            var entity = new UserEntity(1, "Alice", "a@b.com", 30);
            var dto = new UserDto(1, "Alice", "a@b.com", 30);

            var rule = DeepEqualRule.For<UserEntity, UserDto>();

            await Stateless.Assert.That<UserEntity>(entity).IsEquivalentTo(dto).With(rule);
        }

        [Fact]
        public async Task PreBuiltRule_WithAutoMatchAndExclusion_ShouldPassWhenExcludedDiffers()
        {
            var entity = new UserEntity(1, "Alice", "a@b.com", 30);
            var dto = new UserDto(1, "Alice", "totally-different@x.com", 30);

            var rule = DeepEqualRule.For<UserEntity, UserDto>()
                .Exclude(e => e.Email);

            await Stateless.Assert.That<UserEntity>(entity).IsEquivalentTo(dto).With(rule);
        }

        [Fact]
        public async Task PreBuiltRule_WithCustomComparer_ShouldUseIt()
        {
            var entity = new UserEntity(1, "ALICE", "a@b.com", 30);
            var dto = new UserDto(1, "alice", "a@b.com", 30);

            var rule = DeepEqualRule.For<UserEntity, UserDto>()
                .Match(e => e.Name, d => d.Name,
                    StringComparer.OrdinalIgnoreCase)
                .Match(e => e.Id, d => d.Id)
                .Match(e => e.Email, d => d.Email)
                .Match(e => e.Age, d => d.Age);

            await Stateless.Assert.That<UserEntity>(entity).IsEquivalentTo(dto).With(rule);
        }

        // Overload (3): inline configure lambda

        [Fact]
        public async Task InlineLambdaRule_ShouldPass_WhenConfiguredMatchPasses()
        {
            var entity = new UserEntity(1, "Alice", "a@b.com", 30);
            var dto = new UserDto(99, "Alice", "different@x.com", 30);

            await Stateless.Assert.That(entity).IsEquivalentTo(dto)
                .With((DeepEqualRule<UserEntity, UserDto> b) => b
                    .Match(e => e.Name, d => d.Name)
                    .Match(e => e.Age, d => d.Age));
        }

        [Fact]
        public async Task InlineLambdaRule_ShouldFail_WhenConfiguredMatchFails()
        {
            var entity = new UserEntity(1, "Alice", "a@b.com", 30);
            var dto = new UserDto(1, "Alice", "a@b.com", 99); // Age differs

            var msg = await CaptureFailureMessageAsync(async () =>
                await Stateless.Assert.That<UserEntity>(entity).IsEquivalentTo(dto)
                    .With((DeepEqualRule<UserEntity, UserDto> b) => b
                        .Match(e => e.Name, d => d.Name)
                        .Match(e => e.Age, d => d.Age)));

            Assert.NotNull(msg);
            Assert.Contains("Age", msg);
        }

        [Fact]
        public async Task InlineLambdaRule_AutoMatchWithExclusion_ShouldIgnoreExcluded()
        {
            var entity = new UserEntity(1, "Alice", "a@b.com", 30);
            var dto = new UserDto(1, "Alice", "DIFFERENT@b.com", 30);

            await Stateless.Assert.That<UserEntity>(entity).IsEquivalentTo(dto)
                .With((DeepEqualRule<UserEntity, UserDto> b) => b
                    .WithAutoMatch()
                    .Exclude(e => e.Email));
        }

        // Error message quality for typed rules

        [Fact]
        public async Task ErrorMessage_ShouldListDifferingMappedProperties()
        {
            var entity = new UserEntity(1, "Alice", "a@b.com", 30);
            var dto = new UserDto(1, "Bob", "a@b.com", 99); // Name+Age differ

            var rule = DeepEqualRule.For<UserEntity, UserDto>();
            var msg = await CaptureFailureMessageAsync(async () =>
                await Stateless.Assert.That<UserEntity>(entity).IsEquivalentTo(dto).With(rule));

            Assert.NotNull(msg);
            // At least one differing field should be mentioned
            Assert.True(
                msg.Contains("Name") || msg.Contains("Age") || msg.Contains("differ"),
                $"Expected diff in message:\n{msg}");
        }

        [Fact]
        public async Task ErrorMessage_ShouldContainAssertionChain_ForTypedRule()
        {
            var entity = new UserEntity(1, "Alice", "a@b.com", 30);
            var dto = new UserDto(1, "Bob", "a@b.com", 30);
            var rule = DeepEqualRule.For<UserEntity, UserDto>();

            var msg = await CaptureFailureMessageAsync(async () =>
                await Stateless.Assert.That(entity).IsEquivalentTo(dto).With(rule));

            Assert.NotNull(msg);
            Assert.Contains("IsEquivalentTo", msg);
        }
    }
}

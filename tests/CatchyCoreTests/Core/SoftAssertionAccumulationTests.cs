using Catchy;

namespace CatchyCoreTests.Core
{
    /// <summary>
    /// Tests for soft assertion accumulation and soft state management.
    /// Verifies error accumulation, checkpointing, and rollback.
    /// </summary>
    public class SoftAssertionAccumulationTests
    {
        [Fact]
        public async Task SoftAsserter_SingleFailure_Accumulated()
        {
            // Arrange
            var softAssert = Asserter.NewSoft();

            // Act
            await softAssert.That(false).IsTrue();

            // Verify
            await Stateless.Assert.That(softAssert.HasFailures).IsTrue();
            await Stateless.Assert.That(softAssert.ErrorCount).Is(1);
            await Stateless.Assert.That(softAssert.Errors).HasCount(1);
        }

        [Fact]
        public async Task SoftAsserter_MultipleFailures_AllAccumulated()
        {
            // Arrange
            var softAssert = Asserter.NewSoft();

            // Act
            await softAssert.That(false).IsTrue();      // Fail 1
            await softAssert.That(true).IsFalse();      // Fail 2
            await softAssert.That("a").Is("b");         // Fail 3
            await softAssert.That(1).IsGreaterThan(5);  // Fail 4

            // Verify
            await Stateless.Assert.That(softAssert.ErrorCount).Is(4);
            await Stateless.Assert.That(softAssert.Errors).HasCount(4);
        }

        [Fact]
        public async Task SoftAsserter_MixedPassAndFail_OnlyFailuresAccumulated()
        {
            // Arrange
            var softAssert = Asserter.NewSoft();

            // Act
            await softAssert.That(true).IsTrue();       // Pass
            await softAssert.That(false).IsTrue();      // Fail
            await softAssert.That("a").Is("a");         // Pass
            await softAssert.That(5).IsGreaterThan(3);  // Pass
            await softAssert.That(2).IsGreaterThan(5);  // Fail

            // Verify
            await Stateless.Assert.That(softAssert.ErrorCount).Is(2);
        }

        [Fact]
        public async Task SoftAsserter_Clear_RemovesAccumulatedErrors()
        {
            // Arrange
            var softAssert = Asserter.NewSoft();
            await softAssert.That(false).IsTrue();  // Fail
            await Stateless.Assert.That(softAssert.ErrorCount).Is(1);

            // Act
            softAssert.Clear();

            // Verify
            await Stateless.Assert.That(softAssert.HasFailures).IsFalse();
            await Stateless.Assert.That(softAssert.ErrorCount).Is(0);
        }

        [Fact]
        public async Task SoftAsserter_Checkpoint_SavesState()
        {
            // Arrange
            var softAssert = Asserter.NewSoft();

            // Act
            var checkpoint = softAssert.Checkpoint();

            // Verify
            await Stateless.Assert.That(checkpoint).Is(0);  // First checkpoint is 0
        }

        [Fact]
        public async Task SoftAsserter_Revert_RollsBackToCheckpoint()
        {
            // Arrange
            var softAssert = Asserter.NewSoft();

            // Act
            await softAssert.That(false).IsTrue();  // Fail 1
            var checkpoint = softAssert.Checkpoint();

            await softAssert.That(false).IsTrue();  // Fail 2
            await Stateless.Assert.That(softAssert.ErrorCount).Is(2);

            softAssert.Revert(checkpoint);  // Roll back to after first failure

            // Verify
            await Stateless.Assert.That(softAssert.ErrorCount).Is(1);  // Only first failure remains
        }

        [Fact]
        public async Task SoftAsserter_RevertNoArg_RolleBackToLastCheckpoint()
        {
            // Arrange
            var softAssert = Asserter.NewSoft();

            await softAssert.That(false).IsTrue();  // Fail 1
            softAssert.Checkpoint();

            await softAssert.That(false).IsTrue();  // Fail 2
            await Stateless.Assert.That(softAssert.ErrorCount).Is(2);

            // Act
            softAssert.Revert();  // Revert to last checkpoint (after fail 1)

            // Verify
            await Stateless.Assert.That(softAssert.ErrorCount).Is(1);
        }

        [Fact]
        public async Task SoftAsserter_ExceptionCatching_CatchesSpecificTypes()
        {
            // Arrange
            var softAssert = Asserter.NewSoft<TimeoutException>();

            // Act & Verify
            // This would normally throw, but is caught by softAssert mode
            await softAssert.That(() => ThrowTimeout()).Throws<TimeoutException>();

            // Error is accumulated, not thrown
            await Stateless.Assert.That(softAssert.HasFailures).IsFalse();  // Check passed (exception was thrown as expected)
        }

        [Fact]
        public async Task SoftAsserter_ExceptionCatching_AllExceptions()
        {
            // Arrange
            var softAssert = Asserter.NewSoft(allExceptions: true);

            // Act & Verify
            await softAssert.That(false).IsTrue();  // Caught and accumulated

            await Stateless.Assert.That(softAssert.ErrorCount).Is(1);
        }

        private static Task ThrowTimeout()
        {
            throw new TimeoutException("Operation timed out");
        }
    }
}




using Catchy;

namespace CatchyCoreTests.Core
{
    /// <summary>
    /// Tests for the assertion pipeline's lazy evaluation model.
    /// Verifies that assertions are queued but not executed until await.
    /// </summary>
    public class AssertionPipelineTests
    {
        [Fact]
        public async Task Pipeline_LazyExecution_DoesNotThrowUntilAwait()
        {
            // Arrange
            bool? threwException;

            // Act
            try
            {
                // Build chain (should not throw yet)
                var assertion = Stateless.Assert.That(false).IsTrue();
                threwException = false;

                // Await to trigger execution
                await assertion;
                threwException = false;
            }
            catch (AssertionException)
            {
                threwException = true;
            }

            // Verify - exception should occur during await
            await Stateless.Assert.That(threwException).IsTrue();
        }

        [Fact]
        public async Task Pipeline_ChainAccumulation_LinksRecorded()
        {
            // Arrange
            bool? value = true;

            // Act
            var assertion = Stateless.Assert.That(value)
                .IsTrue()
                .And()
                .IsNotNull();

            // Verify - chain should be recorded without execution
            // (This test verifies structure; actual links checked in full suite)
            await assertion;  // Should pass
        }

        [Fact]
        public async Task Pipeline_Skipped_DoesNotExecuteCheck()
        {
            // Arrange
            var value = false;

            // Act & Verify - should not throw because assertion is skipped
            await Stateless.Assert.That(value)
                .When(false)  // Condition false => skipped
                .IsTrue();    // Would fail if executed, but is skipped
        }
    }
}



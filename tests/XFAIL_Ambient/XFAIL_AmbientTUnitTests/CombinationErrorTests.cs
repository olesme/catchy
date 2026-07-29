using Catchy.TUnit;
using Catchy;

namespace AmbientTUnitTests
{
    /// <summary>
    /// Expected-to-fail tests for soft assertion combinations (TUnit version).
    /// These tests intentionally fail to verify error accumulation and callback behavior.
    /// Tests in this file are expected to throw during lifecycle cleanup (DisposeAsync).
    /// 
    /// Pattern: Do NOT use bare try-catch. Instead:
    /// - Use Assert.Throws/Assert.ThrowsAsync for expected hard exceptions
    /// - For soft errors, verify state, then let DisposeAsync auto-flush and fail
    /// - All these tests MUST fail when run by the test runner
    /// </summary>
    public class CombinationErrorTests
    {
        /// <summary>
        /// XFAIL: Soft accumulates multiple errors in state
        /// Fails: Auto-flush in DisposeAsync will throw with all accumulated errors
        /// </summary>
        [Test]
        public async Task XFAIL_SoftAccumulates_Multiple_Errors_FailsOnFlush()
        {
            // Accumulate three soft errors
            await Ambient.Assert.Soft.That(1).Is(2);
            await Ambient.Assert.Soft.That("a").Is("b");
            await Ambient.Assert.Soft.That(true).Is(false);

            await Assert.That(Ambient.Assert.Soft.ErrorCount).IsEqualTo(3);
            // DisposeAsync will flush and throw AggregateAssertionException with all 3 errors
        }

        /// <summary>
        /// XFAIL: Hard and soft errors are independent
        /// Fails: Soft accumulates despite hard exception
        /// </summary>
        [Test]
        public async Task XFAIL_Hard_Throws_But_Soft_Accumulates()
        {
            // Soft error
            await Ambient.Assert.Soft.That(1).Is(2);

            // Hard should throw immediately without affecting soft
            await Assert.ThrowsAsync<AssertionException>(async () =>
                await Ambient.Assert.That(3).Is(4)
            );

            // Soft still has its error; DisposeAsync will flush it
        }

        /// <summary>
        /// XFAIL: Soft errors are instance-specific
        /// Fails: Soft state accumulates in the stateful ambient instance
        /// </summary>
        [Test]
        public async Task XFAIL_Soft_Errors_Are_Instance_Specific()
        {
            // Add soft error to ambient
            await Ambient.Assert.Soft.That(1).Is(2);

            // Verify it's there
            await Assert.That(Ambient.Assert.Soft.HasFailures).IsTrue();
            await Assert.That(Ambient.Assert.Soft.ErrorCount).IsEqualTo(1);
            // DisposeAsync will flush it
        }

        /// <summary>
        /// XFAIL: Soft errors accumulate - verify they're there before flush
        /// Fails: Soft accumulated errors will be flushed on DisposeAsync
        /// </summary>
        [Test]
        public async Task XFAIL_Callbacks_Fire_On_Soft_Failures()
        {
            await Ambient.Assert.Soft.That(1).Is(2);
            await Ambient.Assert.Soft.That(3).Is(4);

            await Assert.That(Ambient.Assert.Soft.ErrorCount).IsEqualTo(2);
            // DisposeAsync will flush the 2 accumulated soft errors
        }

        /// <summary>
        /// XFAIL: Hard exception caught, but soft continues accumulating
        /// Fails: Soft state preserved despite hard exception
        /// </summary>
        [Test]
        public async Task XFAIL_Hard_Throw_Caught_But_Soft_Continues()
        {
            // Add soft errors
            await Ambient.Assert.Soft.That(2).Is(999);
            await Ambient.Assert.Soft.That(3).Is(999);

            // Throw hard (caught in test)
            await Assert.ThrowsAsync<AssertionException>(async () =>
                await Ambient.Assert.That(5).Is(6)
            );

            // Soft errors are still there
            await Assert.That(Ambient.Assert.Soft.HasFailures).IsTrue();
            // DisposeAsync will auto-flush them
        }

        /// <summary>
        /// XFAIL: Soft state persists across multiple accesses
        /// Fails: Multiple accesses accumulate in same state then auto-flush
        /// </summary>
        [Test]
        public async Task XFAIL_Soft_State_Persists_Across_Access()
        {
            var soft1 = Ambient.Assert.Soft;
            await soft1.That(1).Is(2);

            var soft2 = Ambient.Assert.Soft;
            await soft2.That(3).Is(4);

            // Same instance, same error count
            await Assert.That(soft1).IsEquivalentTo(soft2);
            await Assert.That(Ambient.Assert.Soft.ErrorCount).IsEqualTo(2);
            // DisposeAsync will flush both
        }

        /// <summary>
        /// XFAIL: Single soft error is flushed
        /// Fails: Single soft error throws on flush
        /// </summary>
        [Test]
        public async Task XFAIL_Single_Soft_Error_Flush()
        {
            await Ambient.Assert.Soft.That(42).Is(43);
            // DisposeAsync will flush this single error
        }
    }
}

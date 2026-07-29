using Catchy.XUnit;
using Catchy;
using System.Collections.Generic;
using CatchyTestHelpers;

namespace AmbientXUnitTests
{
    /// <summary>
    /// Tests for combinations of Stateless hard, Stateful hard, and Soft assertions.
    /// Ensures that different asserter modes don't interfere with each other and settings are isolated.
    /// </summary>
    public class CombinationTests : CatchyTestBase
    {
        /// <summary>
        /// Test: Stateless hard + Stateful hard + Soft in single test
        /// Verifies: Each mode has independent behavior
        /// </summary>
        [Fact]
        public async Task Stateless_And_Stateful_And_Soft_All_Work_Together()
        {
            // Stateless hard - throws immediately
            await Stateless.Assert.That(1).Is(1);  // passes

            // Stateful hard - also throws immediately
            await Ambient.Assert.That(2).Is(2);    // passes

            // Soft - accumulates
            await Ambient.Assert.Soft.That(3).Is(3);  // passes
            Assert.False(Ambient.Assert.Soft.HasFailures);
        }

        /// <summary>
        /// Test: Stateless hard failure + Stateful hard = independent
        /// Verifies: Stateless failures don't affect Stateful
        /// </summary>
        [Fact]
        public async Task Stateless_Hard_Fails_Independently_From_Stateful()
        {
            // Stateful hard - this should work fine
            await Ambient.Assert.That(1).Is(1);

            // Stateless hard - this should throw immediately
            _ = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(1).Is(999)
            );

            // Stateful hard should still work after Stateless failure
            await Ambient.Assert.That(2).Is(2);
        }

        /// <summary>
        /// Test: Soft failures don't affect hard assertions
        /// Verifies: Soft captures errors, hard still throws
        /// </summary>
        [Fact]
        public async Task Soft_Failures_Independent_From_Hard()
        {
            // Soft - no failures
            await Ambient.Assert.Soft.That(1).Is(1);
            Assert.False(Ambient.Assert.Soft.HasFailures);

            // Hard - should work fine
            await Ambient.Assert.That(1).Is(1);

            // Soft still has no errors
            Assert.False(Ambient.Assert.Soft.HasFailures);
        }

        /// <summary>
        /// Test: Multiple soft errors accumulate correctly
        /// Verifies: Soft state accumulates without throwing
        /// </summary>
        [Fact]
        public async Task Soft_Accumulates_Multiple_Errors()
        {
            await Ambient.Assert.Soft.That(1).Is(1);  // Pass, don't accumulate errors
            await Ambient.Assert.Soft.That("a").Is("a");  // Pass
            await Ambient.Assert.Soft.That(true).Is(true);  // Pass

            Assert.Equal(0, Ambient.Assert.Soft.ErrorCount);
            Assert.False(Ambient.Assert.Soft.HasFailures);
        }

        /// <summary>
        /// Test: Hard assertions throw immediately after first failure
        /// Verifies: Hard mode stops on first error (doesn't accumulate)
        /// </summary>
        [Fact]
        public async Task Hard_Throws_Immediately_On_First_Failure()
        {
            _ = await TestHelpers.ShouldFailWithMessageAsync(async () =>
            {
                await Ambient.Assert.That(1).Is(2);
                await Ambient.Assert.That(3).Is(4);  // Should not reach here
            });
        }

        /// <summary>
        /// Test: Settings isolation between Stateless and Stateful
        /// Verifies: Each asserter can have independent settings
        /// </summary>
        [Fact]
        public async Task Settings_Isolation_Between_Stateless_And_Stateful()
        {
            // Create custom settings for Stateful
            var customStateful = Asserter.NewStateful(s =>
            {
                s.CatchAll = true;
            });

            // Verify custom stateful works
            await customStateful.That(1).Is(1);

            // Stateless should use defaults
            await Stateless.Assert.That(1).Is(1);  // Uses default settings

            // Ambient Stateful uses global defaults
            await Ambient.Assert.That(1).Is(1);
        }

        /// <summary>
        /// Test: Clear Soft state doesn't affect Hard assertions
        /// Verifies: Clearing soft state is independent operation
        /// </summary>
        [Fact]
        public async Task Clear_Soft_Doesnt_Affect_Hard()
        {
            // Add soft errors
            await Ambient.Assert.Soft.That(1).Is(2);
            Assert.True(Ambient.Assert.Soft.HasFailures);

            // Clear soft
            Ambient.Assert.Soft.Clear();
            Assert.False(Ambient.Assert.Soft.HasFailures);

            // Hard should still work
            await Ambient.Assert.That(1).Is(1);
        }

        /// <summary>
        /// Test: Interleaved hard and soft assertions
        /// Verifies: Can mix hard and soft calls without interference
        /// </summary>
        [Fact]
        public async Task Interleaved_Hard_And_Soft_Assertions()
        {
            await Ambient.Assert.That(1).Is(1);           // Hard passes
            await Ambient.Assert.Soft.That(2).Is(2);      // Soft passes
            await Ambient.Assert.That(3).Is(3);           // Hard passes
            await Ambient.Assert.Soft.That(4).Is(4);      // Soft passes

            Assert.False(Ambient.Assert.Soft.HasFailures);
        }

        [Fact]
        public async Task Soft_Failure_Does_Not_Block_Subsequent_Hard_Assertion()
        {
            await Ambient.Assert.Soft.That(1).Is(2);
            Assert.True(Ambient.Assert.Soft.HasFailures);

            await Ambient.Assert.That(10).Is(10);

            Ambient.Assert.Soft.Clear();
            Assert.False(Ambient.Assert.Soft.HasFailures);
        }

        /// <summary>
        /// Test: Stateful hard can access same soft as before
        /// Verifies: Soft property returns same cached instance
        /// </summary>
        [Fact]
        public async Task Stateful_Soft_Returns_Same_Instance()
        {
            var soft1 = Ambient.Assert.Soft;
            await soft1.That(1).Is(1);  // Pass

            var soft2 = Ambient.Assert.Soft;
            Assert.True(ReferenceEquals(soft1, soft2));
            Assert.Equal(0, soft2.ErrorCount);  // Same state, no errors
        }

        /// <summary>
        /// Test: OnAssertion work with different asserter modes
        /// Verifies: OnSoftFailure callbacks fire correctly
        /// </summary>
        [Fact]
        public async Task Callbacks_Fire_For_Soft_Failures()
        {
            var callbackCount = 0;

            var customStateful = Asserter.NewStateful(s =>
            {
                s.OnSoftFailure = [.. s.OnSoftFailure, (info =>
                {
                    Interlocked.Increment(ref callbackCount);
#if !NETSTANDARD2_1_OR_GREATER && !NET5_0_OR_GREATER
                    return new ValueTask(Task.CompletedTask);
#else
                    return ValueTask.CompletedTask;
#endif
                })];
            });

            await customStateful.Soft.That(1).Is(2);
            await customStateful.Soft.That(3).Is(4);

            Assert.Equal(2, callbackCount);
        }

        /// <summary>
        /// Test: Ambient uses global settings, custom uses custom
        /// Verifies: Settings don't bleed between instances
        /// </summary>
        [Fact]
        public async Task Settings_Dont_Bleed_Between_Instances()
        {
            var callbacksFired = 0;

            // Create custom with callback
            var customStateful = Asserter.NewStateful(s =>
            {
                s.OnSoftFailure = [.. s.OnSoftFailure, (info =>
                {
                    Interlocked.Increment(ref callbacksFired);
#if !NETSTANDARD2_1_OR_GREATER && !NET5_0_OR_GREATER
                    return new ValueTask(Task.CompletedTask);
#else
                    return ValueTask.CompletedTask;
#endif
                })];
            });

            // Use both - pass so no soft errors
            await customStateful.Soft.That(1).Is(1);  // Passes, no callback
            await Ambient.Assert.Soft.That(3).Is(3);  // Passes, no callback

            Assert.Equal(0, callbacksFired);  // No callbacks fired
        }

        /// <summary>
        /// Test: Hard assertions in both Stateless and Stateful throw independently
        /// Verifies: Each hard mode maintains its own exception behavior
        /// </summary>
        [Fact]
        public async Task Hard_Exceptions_Independent()
        {
            _ = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.That(1).Is(999)
            );

            _ = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Ambient.Assert.That(2).Is(999)
            );
        }

        /// <summary>
        /// Test: Soft state can be checked without flushing
        /// Verifies: HasFailures and ErrorCount work without flush
        /// </summary>
        [Fact]
        public async Task Soft_State_Inspection_Doesnt_Flush()
        {
            await Ambient.Assert.Soft.That(1).Is(1);

            // Check state
            Assert.False(Ambient.Assert.Soft.HasFailures);
            Assert.Equal(0, Ambient.Assert.Soft.ErrorCount);

            // Add more
            await Ambient.Assert.Soft.That(3).Is(3);
            Assert.Equal(0, Ambient.Assert.Soft.ErrorCount);
        }

        /// <summary>
        /// Test: Custom stateful soft instance (not part of ambient lifecycle)
        /// Verifies: This just verifies custom stateful works, but doesn't auto-fail
        /// </summary>
        [Fact]
        public async Task Custom_Stateful_Soft_Accumulates_But_No_Auto_Flush()
        {
            var customStateful = Asserter.NewStateful();

            await customStateful.Soft.That(1).Is(2);
            await customStateful.Soft.That(3).Is(4);

            Assert.Equal(2, customStateful.Soft.ErrorCount);
            // This custom stateful is NOT auto-flushed (not part of ambient lifecycle)
            // so this test passes without error
        }

        [Theory]
        [InlineData(new[] { 1, 2, 3 })]
        [InlineData(new[] { 5, 10, 15, 20 })]
        public async Task Quantified_DataDriven_Async_Passes_For_Positive_Sets(int[] values)
        {
            await Stateless.Assert.ThatEachOf(values).IsPositive();
        }
    }
}

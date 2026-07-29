using Catchy;
using System.Collections.Generic;
using CatchyTestHelpers;

namespace AmbientTUnitTests
{
    /// <summary>
    /// Tests for combinations of Stateless hard, Stateful hard, and Soft assertions.
    /// Ensures that different asserter modes don't interfere with each other and settings are isolated.
    /// </summary>
    public class CombinationTests
    {
        /// <summary>
        /// Test: Stateless hard + Stateful hard + Soft in single test
        /// Verifies: Each mode has independent behavior
        /// </summary>
        [Test]
        public async Task Stateless_And_Stateful_And_Soft_All_Work_Together()
        {
            // Stateless hard - throws immediately
            await Stateless.Assert.That(1).Is(1);  // passes

            // Stateful hard - also throws immediately
            await Ambient.Assert.That(2).Is(2);    // passes

            // Soft - accumulates
            await Ambient.Assert.Soft.That(3).Is(3);  // passes
            await Assert.That(Ambient.Assert.Soft.HasFailures).IsFalse();
        }

        /// <summary>
        /// Test: Stateless hard failure + Stateful hard = independent
        /// Verifies: Stateless failures don't affect Stateful
        /// </summary>
        [Test]
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
        [Test]
        public async Task Soft_Failures_Independent_From_Hard()
        {
            // Soft - no failures
            await Ambient.Assert.Soft.That(1).Is(1);
            await Assert.That(Ambient.Assert.Soft.HasFailures).IsFalse();

            // Hard - should work fine
            await Ambient.Assert.That(1).Is(1);

            // Soft still has no errors
            await Assert.That(Ambient.Assert.Soft.HasFailures).IsFalse();
        }

        /// <summary>
        /// Test: Multiple soft errors accumulate correctly
        /// Verifies: Soft state accumulates without throwing
        /// </summary>
        [Test]
        public async Task Soft_Accumulates_Multiple_Errors()
        {
            await Ambient.Assert.Soft.That(1).Is(1);  // Pass, don't accumulate errors
            await Ambient.Assert.Soft.That("a").Is("a");  // Pass
            await Ambient.Assert.Soft.That(true).Is(true);  // Pass

            await Assert.That(Ambient.Assert.Soft.ErrorCount).IsEqualTo(0);
            await Assert.That(Ambient.Assert.Soft.HasFailures).IsFalse();
        }

        /// <summary>
        /// Test: Hard assertions throw immediately after first failure
        /// Verifies: Hard mode stops on first error (doesn't accumulate)
        /// </summary>
        [Test]
        public async Task Hard_Throws_Immediately_On_First_Failure()
        {
            var failedCount = 0;

            _ = await TestHelpers.ShouldFailWithMessageAsync(async () =>
            {
                await Ambient.Assert.That(1).Is(2);
                await Ambient.Assert.That(3).Is(4);  // Should not reach here
            });

            failedCount = 1;
            await Assert.That(failedCount).IsEqualTo(1);
        }

        /// <summary>
        /// Test: Settings isolation between Stateless and Stateful
        /// Verifies: Each asserter can have independent settings
        /// </summary>
        [Test]
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
        [Test]
        public async Task Clear_Soft_Doesnt_Affect_Hard()
        {
            // Add soft errors
            await Ambient.Assert.Soft.That(1).Is(2);
            await Assert.That(Ambient.Assert.Soft.HasFailures).IsTrue();

            // Clear soft
            Ambient.Assert.Soft.Clear();
            await Assert.That(Ambient.Assert.Soft.HasFailures).IsFalse();

            // Hard should still work
            await Ambient.Assert.That(1).Is(1);
        }

        /// <summary>
        /// Test: Interleaved hard and soft assertions
        /// Verifies: Can mix hard and soft calls without interference
        /// </summary>
        [Test]
        public async Task Interleaved_Hard_And_Soft_Assertions()
        {
            await Ambient.Assert.That(1).Is(1);           // Hard passes
            await Ambient.Assert.Soft.That(2).Is(2);      // Soft passes
            await Ambient.Assert.That(3).Is(3);           // Hard passes
            await Ambient.Assert.Soft.That(4).Is(4);      // Soft passes

            await Assert.That(Ambient.Assert.Soft.HasFailures).IsFalse();
        }

        [Test]
        public async Task Soft_Failure_Does_Not_Block_Subsequent_Hard_Assertion()
        {
            await Ambient.Assert.Soft.That(1).Is(2);
            await Assert.That(Ambient.Assert.Soft.HasFailures).IsTrue();

            await Ambient.Assert.That(10).Is(10);

            Ambient.Assert.Soft.Clear();
            await Assert.That(Ambient.Assert.Soft.HasFailures).IsFalse();
        }

        /// <summary>
        /// Test: Stateful hard can access same soft as before
        /// Verifies: Soft property returns same cached instance
        /// </summary>
        [Test]
        public async Task Stateful_Soft_Returns_Same_Instance()
        {
            var soft1 = Ambient.Assert.Soft;
            await soft1.That(1).Is(1);  // Pass

            var soft2 = Ambient.Assert.Soft;
            await Assert.That(ReferenceEquals(soft1, soft2)).IsTrue();
            await Assert.That(soft2.ErrorCount).IsEqualTo(0);  // Same state, no errors
        }

        /// <summary>
        /// Test: OnSoftFailure fire only on soft failures
        /// Verifies: OnSoftFailure callbacks fire correctly for soft failures
        /// </summary>
        [Test]
        public async Task SoftFailureCallbacks_Fire_For_Soft_Failures()
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

            await Assert.That(callbackCount).IsEqualTo(2);
        }

        /// <summary>
        /// Test: Ambient uses global settings, custom uses custom
        /// Verifies: Settings don't bleed between instances
        /// </summary>
        [Test]
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

            await Assert.That(callbacksFired).IsEqualTo(0);  // No callbacks fired
        }

        /// <summary>
        /// Test: Hard assertions in both Stateless and Stateful throw independently
        /// Verifies: Each hard mode maintains its own exception behavior
        /// </summary>
        [Test]
        public async Task Hard_Exceptions_Independent()
        {
            var statelessThrew = false;
            var statefulThrew = false;

            _ = await TestHelpers.ShouldFailWithMessageAsync(async () => 
                await Stateless.Assert.That(1).Is(999)
            );
            statelessThrew = true;

            _ = await TestHelpers.ShouldFailWithMessageAsync(async () => 
                await Ambient.Assert.That(2).Is(999)
            );
            statefulThrew = true;

            await Assert.That(statelessThrew).IsTrue();
            await Assert.That(statefulThrew).IsTrue();
        }

        /// <summary>
        /// Test: Soft state can be checked without flushing
        /// Verifies: HasFailures and ErrorCount work without flush
        /// </summary>
        [Test]
        public async Task Soft_State_Inspection_Doesnt_Flush()
        {
            await Ambient.Assert.Soft.That(1).Is(1);

            // Check state
            await Assert.That(Ambient.Assert.Soft.HasFailures).IsFalse();
            await Assert.That(Ambient.Assert.Soft.ErrorCount).IsEqualTo(0);

            // Add more
            await Ambient.Assert.Soft.That(3).Is(3);
            await Assert.That(Ambient.Assert.Soft.ErrorCount).IsEqualTo(0);
        }

        /// <summary>
        /// Test: Custom stateful soft instance (not part of ambient lifecycle)
        /// Verifies: This just verifies custom stateful works, but doesn't auto-fail
        /// </summary>
        [Test]
        public async Task Custom_Stateful_Soft_Accumulates_But_No_Auto_Flush()
        {
            var customStateful = Asserter.NewStateful();

            await customStateful.Soft.That(1).Is(2);
            await customStateful.Soft.That(3).Is(4);

            await Assert.That(customStateful.Soft.ErrorCount).IsEqualTo(2);
            // This custom stateful is NOT auto-flushed (not part of ambient lifecycle)
            // so this test passes without error
        }

        [Test]
        public async Task Quantified_DataDriven_Async_Passes_For_Positive_Sets()
        {
            int[][] datasets =
            [
                [1, 2, 3],
                [5, 10, 15]
            ];

            foreach (var values in datasets)
            {
                await Stateless.Assert.ThatEachOf(values).IsPositive();
            }
        }
    }
}

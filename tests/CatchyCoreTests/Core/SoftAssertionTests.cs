using Catchy;
using Catchy.Sdk;
using Catchy.Configuration;
using CatchyCoreTests.Helpers;

namespace CatchyCoreTests.Core
{
    public class SoftAssertionTests
    {
        [Fact]
        public async Task SoftAsserter_collects_failures_without_throwing()
        {
            var verify = new SoftAsserter();
            await verify.That(1).Is(2);       // fails
            await verify.That("a").Is("b");   // fails
            await verify.That(true).IsTrue(); // passes

            Assert.True(verify.HasFailures);
            Assert.Equal(2, verify.ErrorCount);
        }

        [Fact]
        public async Task SoftAsserter_TryFlush_throws_aggregate()
        {
            var verify = new SoftAsserter();
            await verify.That(1).Is(2);
            await verify.That("a").Is("b");

            var ex = await Assert.ThrowsAsync<AggregateAssertionException>(async () => throw verify.SoftState.AggregateException!);
            Assert.Equal(2, ex.InnerExceptions.Count);
        }

        [Fact]
        public async Task SoftAsserter_TryFlush_message_contains_count()
        {
            var verify = new SoftAsserter();
            await verify.That(1).Is(99);
            await verify.That(2).Is(99);
            await verify.That(3).Is(99);

            var ex = await Assert.ThrowsAsync<AggregateAssertionException>(async () => throw verify.SoftState.AggregateException!);
            Assert.Contains("3", ex.Message);
        }

        [Fact]
        public async Task SoftAsserter_SoftState_message_contains_count()
        {
            var verify = new SoftAsserter();
            await verify.That(1).Is(99);
            await verify.That(2).Is(99);
            await verify.That(3).Is(99);

            var ex = await Assert.ThrowsAsync<AggregateAssertionException>(async () => await Stateless.Assert.That(verify.SoftState).HasNoErrors());
            Assert.Contains("[3]", ex.Message);
        }

        [Fact]
        public async Task SoftAsserter_no_failures_TryFlush_does_not_throw()
        {
            var verify = new SoftAsserter();
            await verify.That(42).Is(42);
            var ex = verify.SoftState.AggregateException;
            Assert.Null(ex); // should not throw, should return null
        }

        [Fact]
        public async Task SoftAsserter_no_failures_SoftState_does_not_throw()
        {
            var verify = new SoftAsserter();
            await verify.That(42).Is(42);
            await Stateless.Assert.That(verify.SoftState).HasNoErrors(); // should not throw
        }

        [Fact]
        public async Task SoftAsserter_Clear_resets_failures()
        {
            var verify = new SoftAsserter();
            await verify.That(1).Is(2);
            verify.Clear();

            Assert.False(verify.HasFailures);
            Assert.Equal(0, verify.ErrorCount);
        }

        [Fact]
        public async Task Checkpoint_Revert_removes_errors_after_checkpoint()
        {
            var verify = new SoftAsserter();
            await verify.That(1).Is(2); // error 1 – before checkpoint

            var cp = verify.Checkpoint();

            await verify.That(2).Is(99); // error 2 – after checkpoint
            await verify.That(3).Is(99); // error 3 – after checkpoint

            verify.Revert(cp);

            // Only error 1 should remain
            Assert.Equal(1, verify.ErrorCount);
        }

        [Fact]
        public async Task Checkpoint_Revert_no_arg_uses_last_checkpoint()
        {
            var verify = Asserter.NewSoft();
            await verify.That(1).Is(2);

            verify.Checkpoint();
            await verify.That(2).Is(99);

            verify.Revert(); // revert to last checkpoint

            Assert.Equal(1, verify.ErrorCount);
        }

        [Fact]
        public async Task With_SoftState_collects_instead_of_throwing()
        {
            var state = new SoftState();

            await Stateless.Assert.That(1).Is(999).With(state);
            await Stateless.Assert.That("x").Is("y").With(state);

            Assert.True(state.HasFailures);
            Assert.Equal(2, state.ErrorCount);
        }

        [Fact]
        public async Task With_SoftState_TryFlush_throws()
        {
            var state = new SoftState();
            await Stateless.Assert.That(1).Is(2).With(state);

            await Assert.ThrowsAsync<AggregateAssertionException>(() => throw state.AggregateException!);
        }

        [Fact]
        public async Task With_SoftAsserter_routes_to_shared_state()
        {
            var verify = new SoftAsserter();
            await Stateless.Assert.That(1).Is(999).With(verify);

            Assert.True(verify.HasFailures);
        }

        // StatefulAsserter tests
        [Fact]
        public async Task StatefulAsserter_Hard_throws_immediately()
        {
            var assert = Asserter.NewStateful();

            await Assert.ThrowsAsync<AssertionException>(async () =>
                await assert.That(1).Is(2)
            );
        }

        [Fact]
        public async Task StatefulAsserter_Soft_accumulates_failures()
        {
            var assert = Asserter.NewStateful();

            await assert.Soft.That(1).Is(2);       // fails
            await assert.Soft.That("a").Is("b");   // fails
            await assert.Soft.That(true).IsTrue(); // passes

            Assert.True(assert.Soft.HasFailures);
            Assert.Equal(2, assert.Soft.ErrorCount);
        }

        [Fact]
        public async Task StatefulAsserter_Hard_and_Soft_share_state()
        {
            var assert = Asserter.NewStateful();

            // Add soft failures first
            await assert.Soft.That(1).Is(2);
            await assert.Soft.That("a").Is("b");

            // Verify shared state
            Assert.Equal(2, assert.Soft.ErrorCount);
        }

        [Fact]
        public async Task StatefulAsserter_Soft_property_returns_same_instance()
        {
            var assert = Asserter.NewStateful();

            var soft1 = assert.Soft;
            var soft2 = assert.Soft;

            // Same instance (lazy cached)
            Assert.Same(soft1, soft2);
        }

        [Fact]
        public async Task StatefulAsserter_with_settings_shares_settings()
        {
            var settings = new AssertionSettings().Clone(s => s.CatchAll = true);
            var assert = Asserter.NewStateful(settings);

            // Stateless and soft should use same settings
            Assert.Same(assert.Settings(), assert.Soft.Settings());
        }

        [Fact]
        public async Task StatefulAsserter_Soft_accumulates_multiple_failures()
        {
            var assert = Asserter.NewStateful();

            for (int i = 0; i < 3; i++)
            {
                await assert.Soft.That(i).Is(99);
            }

            Assert.Equal(3, assert.Soft.ErrorCount);
        }

        [Fact]
        public async Task StatefulAsserter_Clear_resets_soft_state()
        {
            var assert = Asserter.NewStateful();

            await assert.Soft.That(1).Is(2);
            assert.Soft.Clear();

            Assert.False(assert.Soft.HasFailures);
            Assert.Equal(0, assert.Soft.ErrorCount);
        }
    }
}

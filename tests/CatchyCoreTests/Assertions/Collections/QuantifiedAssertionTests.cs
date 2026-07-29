using Catchy;
using CatchyCoreTests.Helpers;
using System.Threading;

namespace CatchyCoreTests.Assertions.Collections
{
    public class QuantifiedAssertionTests
    {
        private static readonly int[] _pos = [2, 4, 6, 8];
        private static readonly int[] _mixed = [1, -2, 3, -4];
        private static readonly double[] _temps = [20.1, 20.4, 19.9];

        [Fact]
        public async Task ThatEachOf_IsPositive_passes()
        {
            await Stateless.Assert.ThatEachOf(_pos).IsPositive();
        }

        [Fact]
        public async Task ThatEachOf_IsEven_passes()
            => await Stateless.Assert.ThatEachOf(_pos).IsEven();

        [Fact]
        public async Task ThatEachOf_fails_shows_failed_items()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.ThatEachOf(_mixed).IsPositive());
            Assert.Contains("failed", msg.ToLower());
            Assert.Contains("-2", msg);
        }

        [Fact]
        public async Task ThatAnyOf_IsPositive_passes_with_mixed()
            => await Stateless.Assert.ThatAnyOf(_mixed).IsPositive();

        [Fact]
        public async Task ThatAnyOf_fails_when_none_pass()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.ThatAnyOf(_pos).IsNegative());
            Assert.Contains("at least one", msg.ToLower());
        }

        [Fact]
        public async Task ThatNoneOf_IsNegative_passes_for_all_positive()
            => await Stateless.Assert.ThatNoneOf(_pos).IsNegative();

        [Fact]
        public async Task ThatNoneOf_fails_when_one_passes()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.ThatNoneOf(_mixed).IsPositive());
            Assert.Contains("no items", msg.ToLower());
        }

        [Fact]
        public async Task ThatEachOf_strings_list_passes()
            => await Stateless.Assert.ThatEachOf(new[] { "alpha", "beta", "gamma" })
                .IsNotNullOrEmpty()
                .And().IsLowerCase();

        [Fact]
        public async Task ThatEachOf_strings_passes()
        => await Stateless.Assert.ThatEachOf("alpha", "beta", "gamma")
            .IsNotNullOrEmpty()
            .And().IsLowerCase();

        [Fact]
        public async Task ThatEachOf_strings_fails_on_one_upper()
        {
            var msg = await Catch.FailureOf(async ()
                => await Stateless.Assert.ThatEachOf(new[] { "alpha", "BETA", "gamma" }).IsLowerCase());
            Assert.Contains("BETA", msg);
        }

        [Fact]
        public async Task ThatEachOf_params_2_items_passes()
            => await Stateless.Assert.ThatEachOf(10, 20).IsGreaterThan(5);

        [Fact]
        public async Task ThatAnyOf_params_3_items_passes()
            => await Stateless.Assert.ThatAnyOf(1, 2, 3).IsGreaterThan(2);

        [Fact]
        public async Task ThatNoneOf_params_2_items_passes()
            => await Stateless.Assert.ThatNoneOf(-1, -2).IsPositive();

        [Fact]
        public async Task ThatEachOf_IsInRange_passes()
            => await Stateless.Assert.ThatEachOf(_pos).IsInRange(1, 10);

        [Fact]
        public async Task ThatAnyOf_IsInRange_passes_when_any_matches()
            => await Stateless.Assert.ThatAnyOf(_mixed).IsInRange(3, 5);

        [Fact]
        public async Task ThatEachOf_IsCloseTo_passes_for_doubles()
            => await Stateless.Assert.ThatEachOf(_temps).IsCloseTo(20.0, 0.5);

        [Fact]
        public async Task ThatEachOf_IsCloseTo_fails_shows_indexed_reason()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.ThatEachOf([20.1, 21.2, 19.9]).IsCloseTo(20.0, 0.5));
            Assert.Contains("[1]", msg);
            Assert.Contains("within", msg.ToLower());
        }

        [Fact]
        public async Task ThatEachOf_IsMultipleOf_passes()
            => await Stateless.Assert.ThatEachOf(6, 12, 18).IsMultipleOf(6);

        [Fact]
        public async Task ThatAnyOf_IsMultipleOf_passes_when_any_matches()
            => await Stateless.Assert.ThatAnyOf(5, 10, 15).IsMultipleOf(10);

        [Fact]
        public async Task ThatEachOf_IsEven_fails_shows_failing_item()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.ThatEachOf(2, 4, 5).IsEven());
            Assert.Contains("[2]", msg);
            Assert.Contains("5", msg);
        }

        [Fact]
        public async Task Quantified_user_api_subpipelines_do_not_trigger_wrappers_or_hooks_while_outer_pipeline_does_on_success()
        {
            int wrapperCalls = 0;
            int assertionHookCalls = 0;

            var asserter = Asserter.NewStateful(s =>
            {
                s.OnExecution.Add(async (_, next) =>
                {
                    Interlocked.Increment(ref wrapperCalls);
                    await next();
                });
                s.OnAssertion.Add(_ =>
                {
                    Interlocked.Increment(ref assertionHookCalls);
                    return default;
                });
            });

            await asserter.ThatEachOf(2, 4, 6).IsEven();

            Assert.Equal(1, wrapperCalls);
            Assert.Equal(1, assertionHookCalls);
        }

        [Fact]
        public async Task Quantified_user_api_subpipelines_do_not_trigger_wrappers_or_hooks_while_outer_pipeline_does_on_failure()
        {
            int wrapperCalls = 0;
            int assertionHookCalls = 0;

            var asserter = Asserter.NewStateful(s =>
            {
                s.OnExecution.Add(async (_, next) =>
                {
                    Interlocked.Increment(ref wrapperCalls);
                    await next();
                });
                s.OnAssertion.Add(_ =>
                {
                    Interlocked.Increment(ref assertionHookCalls);
                    return default;
                });
            });

            await Catch.FailureOf(async () => await asserter.ThatEachOf(2, 3, 4).IsEven());

            Assert.Equal(1, wrapperCalls);
            Assert.Equal(1, assertionHookCalls);
        }
    }
}

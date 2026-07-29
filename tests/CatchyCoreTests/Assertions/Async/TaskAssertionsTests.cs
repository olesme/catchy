using Catchy;

namespace CatchyCoreTests.Assertions.Async
{
    /// <summary>
    /// Integration tests for Task and Func assertions.
    /// Covers completion, cancellation, faulting, exception handling, and execution modes.
    /// </summary>
    public class TaskAssertionsTests
    {
        [Fact]
        public async Task TaskAssertions_IsCompleted_WithCompletedTask_Passes()
        {
            // Arrange
            var task = Task.CompletedTask;

            // Act & Verify
            await Stateless.Assert.That(task).IsCompleted();
        }

        [Fact]
        public async Task TaskAssertions_IsCompleted_WithPendingTask_Throws()
        {
            var tcs = new TaskCompletionSource();
            await Assert.ThrowsAsync<AssertionException>(async () =>
            {
                await Stateless.Assert.That(tcs.Task).Already().IsCompleted();
            });
        }

        [Fact]
        public async Task TaskAssertions_Completes_WithCompletedTask_Passes()
        {
            // Arrange
            var task = Task.Delay(10, TestContext.Current.CancellationToken);

            // Act & Verify
            await Stateless.Assert.That(task).Completes();
        }

        [Fact]
        public async Task TaskAssertions_Already_WithCompletedTask_Passes()
        {
            await Stateless.Assert.That(Task.CompletedTask).Already();
        }

        [Fact]
        public async Task TaskAssertions_Completes_WithCancelledTask_Throws()
        {
            await Assert.ThrowsAsync<AssertionException>(async () =>
            {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();
            var task = Task.Delay(1000, cts.Token);

            // Act
            await Stateless.Assert.That(task).Completes();
            });
        }

        [Fact]
        public async Task TaskAssertions_Completes_WithFaultedTask_Throws()
        {
            await Assert.ThrowsAsync<AssertionException>(async () =>
            {
                await Stateless.Assert.That(Task.FromException(new InvalidOperationException("boom"))).Completes();
            });
        }

        [Fact]
        public async Task FuncAssertions_Throws_WithThrowingFunc_Passes()
        {
            // Arrange
            static async Task ThrowingFunc() => throw new InvalidOperationException("Test");

            // Act & Verify
            await Stateless.Assert.That(ThrowingFunc).Throws<InvalidOperationException>();
        }

        [Fact]
        public async Task FuncAssertions_Throws_WithNonThrowingFunc_Throws()
        {
            await Assert.ThrowsAsync<AssertionException>(async () =>
            {
            // Arrange
            static async Task NonThrowingFunc() => await Task.Delay(10);

            // Act
            await Stateless.Assert.That(NonThrowingFunc).Throws<InvalidOperationException>();
            });
        }

        [Fact]
        public async Task FuncAssertions_DoesNotThrow_WithNonThrowingFunc_Passes()
        {
            // Arrange
            static async Task NonThrowingFunc() => await Task.Delay(10);

            // Act & Verify
            await Stateless.Assert.That(NonThrowingFunc).DoesNotThrow();
        }

        [Fact]
        public async Task FuncAssertions_WithinTimeout_EventuallySucceeds_Passes()
        {
            // Arrange
            int attempts = 0;
            async Task EventuallySucceeds()
            {
                attempts++;
                if (attempts > 3) throw new InvalidOperationException();
                await Task.Delay(10);
            }

            // Act & Verify
            await Stateless.Assert.That(EventuallySucceeds)
                .DoesNotThrow()
                .Within(TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(20));

            // Act & Verify
            await Stateless.Assert.That(EventuallySucceeds)
                .Throws<InvalidOperationException>()
                .Within(TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(40)); // Should throw!
        }

        // ===== Additional Func Tests from XUnit =====

        [Fact]
        public async Task DoesNotThrow_passes_when_completes()
            => await Stateless.Assert.That(() => Task.CompletedTask).DoesNotThrow();

        [Fact]
        public async Task Throws_passes_when_correct_exception()
            => await Stateless.Assert.That(() => throw new ArgumentException("bad arg"))
                .Throws<ArgumentException>();

        [Fact]
        public async Task Throws_with_predicate_passes()
            => await Stateless.Assert.That(() => throw new InvalidOperationException("specific message"))
                .Throws<InvalidOperationException>(e => e.Message.Contains("specific"));

        [Fact]
        public async Task ThrowsAny_passes()
            => await Stateless.Assert.That(() => throw new Exception("any")).ThrowsAny();

        [Fact]
        public async Task CompletesWithin_passes_fast_task()
            => await Stateless.Assert.That(() => Task.CompletedTask)
                .CompletesWithin(TimeSpan.FromSeconds(1));

        [Fact]
        public async Task Within_polling_retries_until_passes()
        {
            int attempts = 0;
            await Stateless.Assert.That(() =>
            {
                attempts++;
                if (attempts < 3) throw new InvalidOperationException("not yet");
                return Task.CompletedTask;
            })
                .DoesNotThrow()
                .Within(TimeSpan.FromSeconds(5), every: TimeSpan.FromMilliseconds(20));

            Assert.True(attempts >= 3);
        }

        [Fact]
        public async Task WithRetry_passes_on_second_attempt()
        {
            int attempts = 0;
            await Stateless.Assert.That(() =>
            {
                attempts++;
                if (attempts == 1) throw new InvalidOperationException("first fail");
                return Task.CompletedTask;
            })
                .DoesNotThrow()
                .WithRetry(3);

            Assert.Equal(2, attempts);
        }

        [Fact]
        public async Task Typed_Succeeds_passes()
            => await Stateless.Assert.That(() => Task.FromResult(42)).Succeeds();

        [Fact]
        public async Task Typed_DoesNotThrow_passes()
            => await Stateless.Assert.That(() => Task.FromResult("ok")).DoesNotThrow();

        [Fact]
        public async Task Typed_Succeeds_out_captures_result()
        {
            await Stateless.Assert.That(() => Task.FromResult(99))
                .Succeeds(out var result);

            Assert.Equal(99, result.Value);
        }

        [Fact]
        public async Task Typed_Succeeds_out_then_assert_on_result()
        {
            await Stateless.Assert.That(() => Task.FromResult(42))
                .Succeeds(out var result);
            await Stateless.Assert.That(result.Value).Is(42).And().IsPositive();
        }

        [Fact]
        public async Task TaskAssertions_IsCancelled_WithCancelledTask_Passes()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            var task = Task.FromCanceled(cts.Token);
            await Stateless.Assert.That(task).IsCancelled();
        }

        [Fact]
        public async Task TaskAssertions_IsCancelled_WithCompletedTask_Throws()
        {
            await Assert.ThrowsAsync<AssertionException>(async () =>
            {
                await Stateless.Assert.That(Task.CompletedTask).IsCancelled();
            });
        }

        [Fact]
        public async Task TaskAssertions_IsNotCancelled_WithCompletedTask_Passes()
        {
            await Stateless.Assert.That(Task.CompletedTask).IsNotCancelled();
        }

        [Fact]
        public async Task TaskAssertions_IsNotCancelled_WithCancelledTask_Throws()
        {
            await Assert.ThrowsAsync<AssertionException>(async () =>
            {
                var cts = new CancellationTokenSource();
                cts.Cancel();
                await Stateless.Assert.That(Task.FromCanceled(cts.Token)).IsNotCancelled();
            });
        }

        [Fact]
        public async Task TaskAssertions_HasStatus_WithCompletedTask_Passes()
        {
            await Stateless.Assert.That(Task.CompletedTask).HasStatus(TaskStatus.RanToCompletion);
        }

        [Fact]
        public async Task TaskAssertions_HasStatus_WithWrongStatus_Throws()
        {
            await Assert.ThrowsAsync<AssertionException>(async () =>
            {
                await Stateless.Assert.That(Task.CompletedTask).HasStatus(TaskStatus.Faulted);
            });
        }

        [Fact]
        public async Task TaskAssertions_Completes_WithWithinModifier_Passes()
        {
            await Stateless.Assert.That(Task.Delay(20, TestContext.Current.CancellationToken)).Completes().Within(TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task TaskAssertions_CompletesWithin_WithCancelledTask_Throws()
        {
            await Assert.ThrowsAsync<AssertionException>(async () =>
            {
                var cts = new CancellationTokenSource();
                cts.Cancel();
                await Stateless.Assert.That(Task.Delay(1000, cts.Token)).CompletesWithin(TimeSpan.FromMilliseconds(100));
            });
        }

        [Fact]
        public async Task TaskAssertions_CompletesWithin_WithFaultedTask_Throws()
        {
            await Assert.ThrowsAsync<AssertionException>(async () =>
            {
                await Stateless.Assert.That(Task.FromException(new InvalidOperationException("boom"))).CompletesWithin(TimeSpan.FromMilliseconds(100));
            });
        }

        [Fact]
        public async Task TypedTask_IsCompleted_WithCompletedTask_Passes()
        {
            await Stateless.Assert.That(Task.FromResult(123)).IsCompleted();
        }

        [Fact]
        public async Task TypedTask_CompletesWithin_WithFastTask_Passes()
        {
            await Stateless.Assert.That(Task.FromResult(123)).CompletesWithin(TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task TypedTask_Throws_WithFaultedTask_Passes()
        {
            var task = Task.FromException<int>(new InvalidOperationException("boom"));
            await Stateless.Assert.That(task).Throws<int, InvalidOperationException>();
        }

        [Fact]
        public async Task FuncAssertions_ThrowsAny_WithThrowingFunc_Passes()
            => await Stateless.Assert.That(() => Task.FromException<int>(new Exception("any"))).ThrowsAny();

        [Fact]
        public async Task TypedTask_DoesNotThrow_out_captures_result()
        {
            await Stateless.Assert.That(Task.FromResult("ok")).DoesNotThrow(out var result);
            Assert.Equal("ok", result.Value);
        }

        [Fact]
        public async Task FuncAssertions_DoesNotThrow_WithThrowingFunc_Throws()
        {
            await Assert.ThrowsAsync<AssertionException>(async () =>
                await Stateless.Assert.That(() => throw new InvalidOperationException("boom")).DoesNotThrow());
        }

        [Fact]
        public async Task TaskAssertions_HasStatus_WithFaultedTask_Passes()
        {
            await Stateless.Assert.That(Task.FromException(new InvalidOperationException("boom"))).HasStatus(TaskStatus.Faulted);
        }
    }
}






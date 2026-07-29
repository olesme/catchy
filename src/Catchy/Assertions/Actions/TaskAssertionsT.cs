using System.Diagnostics;
using System.Runtime.CompilerServices;
using Catchy.Sdk;
using Catchy.Sdk.Checks.Actions;

namespace Catchy
{
    public static partial class AsserterExtensions
    {
        /// <summary>Starts assertions for a <see cref="Task{T}"/> that produces a result of type <typeparamref name="T"/>.</summary>
        public static ValueAssertions<Task<T>> That<T>(this Asserter a, Task<T> task,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(task))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(
                        asserterExpr: aExpr,
                        methodName: "That",
                        genericType: typeof(T),
                        valueExpr: vExpr,
                        file: file, line: line, member: member);
            return new ValueAssertions<Task<T>>(p, task);
        }
    }

    public static partial class TaskAssertionsExtensions
    {
        /// <summary>Configures the assertion to check the task in its current state without waiting (synchronous mode).</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<Task<T>> Already<T>(this ValueAssertions<Task<T>> a)
        {
            a.GetOpts().Mode = TaskExecutionMode.Now;
            a.Link("Already");
            return a;
        }

        /// <summary>Defers assertion checks until the task reaches a terminal state.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<Task<T>> OnCompletion<T>(this ValueAssertions<Task<T>> a)
        {
            a.GetOpts().Mode = TaskExecutionMode.OnCompletion;
            a.Link("OnCompletion");
            return a;
        }

        /// <summary>Polls the assertion until it passes or <paramref name="timeout"/> expires, with an optional poll <paramref name="every"/> interval.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<Task<T>> Within<T>(this ValueAssertions<Task<T>> a, TimeSpan timeout, TimeSpan? every = null,
            [CallerArgumentExpression(nameof(timeout))] string? timeoutExpr = null,
            [CallerArgumentExpression(nameof(every))] string? everyExpr = null)
        {
            var opts = a.GetOpts();
            opts.Mode = TaskExecutionMode.Polling;
            opts.Timeout = timeout;
            if (every.HasValue) opts.Interval = every.Value;
            a.Link("Within", timeoutExpr, everyExpr);
            return a;
        }

        /// <summary>Asserts that the task completes successfully (without throwing).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Task<T>> Succeeds<T>(this ValueAssertions<Task<T>> a)
        {
            a.Link("Succeeds");
            a.Op(a => TaskChecks.Succeeds(a.GetTask(), a.GetOpts(), a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the task throws an exception of type <typeparamref name="TException"/>, optionally matching <paramref name="predicate"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Task<T>> Throws<T, TException>(this ValueAssertions<Task<T>> a,
            Func<TException, bool>? predicate = null,
            [CallerArgumentExpression(nameof(predicate))] string? expr = null)
            where TException : Exception
        {
            a.Link("DoesThrow", typeof(TException), expr);
            a.Op(a => TaskChecks.Throws<T, TException>(a.GetTask(), a.GetOpts(), predicate, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that awaiting the task does not throw an exception.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Task<T>> DoesNotThrow<T>(this ValueAssertions<Task<T>> a)
        {
            a.Link("DoesNotThrow");
            a.Op(a => TaskChecks.DoesNotThrow(a.GetTask(), a.GetOpts(), a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the task completes successfully and captures the result into <paramref name="result"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Task<T>> Succeeds<T>(this ValueAssertions<Task<T>> a, out AssertionResult<T> result,
            [CallerArgumentExpression(nameof(result))] string? expr = null)
        {
            a.Link("Succeeds", expr);
            return CaptureTaskResult(a, out result);
        }

        /// <summary>Asserts that awaiting the task does not throw and captures the result into <paramref name="result"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Task<T>> DoesNotThrow<T>(this ValueAssertions<Task<T>> a, out AssertionResult<T> result,
            [CallerArgumentExpression(nameof(result))] string? expr = null)
        {
            a.Link("DoesNotThrow", expr);
            return CaptureTaskResult(a, out result);
        }

        private static ValueAssertions<Task<T>> CaptureTaskResult<T>(ValueAssertions<Task<T>> a, out AssertionResult<T> result)
        {
            var box = new AssertionResult<T>();
            result = box;
            a.Op(a => TaskChecks.CaptureResult(a.GetTask(), box, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the typed task has completed (any terminal status).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Task<T>> IsCompleted<T>(this ValueAssertions<Task<T>> a)
        {
            a.Link("IsCompleted");
            a.Op(a => TaskChecks.IsCompleted(a.GetTask(), a.GetOpts(), a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the typed task completes successfully within the specified <paramref name="timeout"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Task<T>> CompletesWithin<T>(this ValueAssertions<Task<T>> a, TimeSpan timeout,
            [CallerArgumentExpression(nameof(timeout))] string? expr = null)
        {
            a.Link("CompletesWithin", expr);
            a.Op(a => TaskChecks.CompletesWithin(a.GetTask(), a.GetOpts(), timeout, a.IsSkipped()));
            return a;
        }

        /// <summary>
        /// Asserts the typed task completes without exception.
        /// Chain <c>.Within(timeout)</c> to add a deadline.
        /// </summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Task<T>> Completes<T>(this ValueAssertions<Task<T>> a)
        {
            a.Link("Completes");
            a.Op(a => TaskChecks.Completes(a.GetTask(), a.GetOpts(), a.IsSkipped()));
            return a;
        }
    }

    namespace Sdk
    {
        // Execution options (shared mutable object captured by all ops in the chain)
        // Modifiers like .Already() / .Within() set the mode AFTER ops are added,
        // so ops must read the mode lazily via this shared reference.

        public enum TaskExecutionMode { Default, Now, OnCompletion, Polling, Retry }

        public sealed class TaskExecutionOptions
        {
            public TaskExecutionMode Mode { get; set; } = TaskExecutionMode.Default;
            public TimeSpan? Timeout { get; set; }
            public TimeSpan Interval { get; set; } = TimeSpan.FromMilliseconds(50);
            public int MaxRetries { get; set; } = 0;
        }

        public static class TaskExecutionOptionsSlots
        {
            private static readonly SlotKey<TaskExecutionOptions> Key = new();

            public static bool TryGet(AssertionPipeline pipeline, out TaskExecutionOptions opts)
                => pipeline.Slots.TryGet(Key, out opts);

            public static TaskExecutionOptions GetOrCreate(AssertionPipeline pipeline)
            {
                if (pipeline.Slots.TryGet(Key, out TaskExecutionOptions opts))
                    return opts;

                opts = new TaskExecutionOptions();
                pipeline.Slots.Set(Key, opts);
                return opts;
            }
        }

        public static partial class TaskAssertionsAccessors
        {
            public static Task<T> GetTask<T>(this ValueAssertions<Task<T>> taskAssertions)
            {
                return taskAssertions.GetValue();
            }

            public static TaskExecutionOptions GetOpts<T>(this ValueAssertions<Task<T>> taskAssertions)
            {
                return TaskExecutionOptionsSlots.GetOrCreate(taskAssertions.GetPipeline());
            }

            public static Task RunWithModeAsync<T>(this ValueAssertions<Task<T>> taskAssertions)
            {
                return ExecutionModes.RunWithMode(
                    () => taskAssertions.GetPipeline().RunAsync(),
                    taskAssertions.GetOpts(),
                    taskAssertions.GetPipeline().Settings);
            }

        }
    }
}


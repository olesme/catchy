using System.Diagnostics;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    public static partial class AsserterExtensions
    {
        /// <summary>Starts assertions for a <see cref="Task"/>.</summary>
        public static ValueAssertions<Task> That(this Asserter a, Task task,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(task))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "That",
                valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<Task>(p, task);
        }
    }

    public static partial class TaskAssertionsExtensions
    {
        /// <summary>Configures the assertion to check the task in its current state without waiting (synchronous mode).</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<Task> Already(this ValueAssertions<Task> a)
        {
            a.GetOpts().Mode = TaskExecutionMode.Now;
            a.Link("Already");
            return a;
        }

        /// <summary>
        /// Sets a maximum wait time. Must follow <c>Completes()</c>.
        /// <code>await Stateless.Verify.That(task).Completes().Within(5.Seconds());</code>
        /// </summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<Task> Within(this ValueAssertions<Task> a, TimeSpan timeout,
            [CallerArgumentExpression(nameof(timeout))] string? expr = null)
        {
            a.GetOpts().Timeout = timeout;
            a.Link("Within", expr);
            return a;
        }

        /// <summary>Asserts that the task has completed (in any terminal state: <see cref="TaskStatus.RanToCompletion"/>, <see cref="TaskStatus.Canceled"/>, or <see cref="TaskStatus.Faulted"/>).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Task> IsCompleted(this ValueAssertions<Task> a)
        {
            a.Link("IsCompleted");
            a.Op(a => TaskChecks.IsCompleted(a.GetValue(), a.GetOpts(), a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the task was cancelled.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Task> IsCancelled(this ValueAssertions<Task> a)
        {
            a.Link("IsCancelled");
            a.Op(a => TaskChecks.IsCancelled(a.GetValue(), a.GetOpts(), a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the task was not cancelled.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Task> IsNotCancelled(this ValueAssertions<Task> a)
        {
            a.Link("IsNotCancelled");
            a.Op(a => TaskChecks.IsNotCancelled(a.GetValue(), a.GetOpts(), a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the task's <see cref="Task.Status"/> equals <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Task> HasStatus(this ValueAssertions<Task> a, TaskStatus expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        {
            a.Link("HasStatus", expr);
            a.Op(a => TaskChecks.HasStatus(a.GetValue(), expected, a.IsSkipped()));
            return a;
        }

        /// <summary>
        /// Asserts the task completes without exception. Chain <c>.Within(timeout)</c> for a deadline.
        /// </summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Task> Completes(this ValueAssertions<Task> a)
        {
            a.Link("Completes");
            a.Op(a => TaskChecks.Completes(a.GetValue(), a.GetOpts(), a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the task completes successfully within the specified <paramref name="timeout"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Task> CompletesWithin(this ValueAssertions<Task> a, TimeSpan timeout,
            [CallerArgumentExpression(nameof(timeout))] string? expr = null)
        {
            a.Link("CompletesWithin", expr);
            a.Op(a => TaskChecks.CompletesWithin(a.GetValue(), timeout, a.IsSkipped(), expr));
            return a;
        }

        /// <summary>Asserts that awaiting the task does not throw an exception.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Task> DoesNotThrow(this ValueAssertions<Task> a)
        {
            a.Link("DoesNotThrow");
            a.Op(a => TaskChecks.DoesNotThrow(a.GetValue(), a.GetOpts(), a.IsSkipped()));
            return a;
        }
    }

    namespace Sdk
    {
        public static partial class TaskAssertionsAccessors
        {
            public static Task GetTask(this ValueAssertions<Task> a) => a.GetValue();
            public static TaskExecutionOptions GetOpts(this ValueAssertions<Task> a)
                => TaskExecutionOptionsSlots.GetOrCreate(a.GetPipeline());
        }
    }
}


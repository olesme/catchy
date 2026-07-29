using System.Diagnostics;
using System.Runtime.CompilerServices;
using Catchy.Sdk;
using Catchy.Sdk.Checks.Actions;

namespace Catchy
{
    public static partial class AsserterExtensions
    {
        /// <summary>Starts assertions for an async delegate (<see cref="Func{Task}"/>).</summary>
        public static ValueAssertions<Func<Task>> That(this Asserter a, Func<Task> func,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(func))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(
                        asserterExpr: aExpr,
                        methodName: "That",
                        valueExpr: vExpr,
                        file: file, line: line, member: member);
            return new ValueAssertions<Func<Task>>(p, func);
        }
    }

    public static partial class FuncAssertionsExtensions
    {
        /// <summary>Polls the assertion until it passes or <paramref name="timeout"/> expires, with an optional <paramref name="every"/> poll interval.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<Func<Task>> Within(this ValueAssertions<Func<Task>> a, TimeSpan timeout, TimeSpan? every = null,
            [CallerArgumentExpression(nameof(timeout))] string? expr = null,
            [CallerArgumentExpression(nameof(every))] string? everyExpr = null)
        {
            var opts = a.GetOpts();
            opts.Mode = TaskExecutionMode.Polling;
            opts.Timeout = timeout;
            if (every.HasValue) opts.Interval = every.Value;
            a.Link("Within", expr, everyExpr);
            return a;
        }

        /// <summary>Retries the delegate up to <paramref name="times"/> times with an optional <paramref name="every"/> delay between attempts.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<Func<Task>> WithRetry(this ValueAssertions<Func<Task>> a, int times, TimeSpan? every = null,
            [CallerArgumentExpression(nameof(times))] string? expr = null,
            [CallerArgumentExpression(nameof(every))] string? everyExpr = null)
        {
            var opts = a.GetOpts();
            opts.Mode = TaskExecutionMode.Retry;
            opts.MaxRetries = times;
            if (every.HasValue) opts.Interval = every.Value;
            a.Link("WithRetry", expr, everyExpr);
            return a;
        }

        /// <summary>Asserts that invoking the delegate throws a <typeparamref name="TException"/>, optionally matching <paramref name="predicate"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Func<Task>> Throws<TException>(this ValueAssertions<Func<Task>> a,
            Func<TException, bool>? predicate = null,
            [CallerArgumentExpression(nameof(predicate))] string? expr = null)
            where TException : Exception
        {
            a.Link("DoesThrow", typeof(TException), expr);
            a.Op(a => FuncChecks.DoesThrow<TException>(a.GetFunc(), predicate, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that invoking the delegate throws any exception.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Func<Task>> ThrowsAny(this ValueAssertions<Func<Task>> a)
        {
            a.Link("DoesThrowAny");
            a.Op(a => FuncChecks.DoesThrowAny(a.GetFunc(), a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that invoking the delegate does not throw any exception.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Func<Task>> DoesNotThrow(this ValueAssertions<Func<Task>> a)
        {
            a.Link("DoesNotThrow");
            a.Op(a => FuncChecks.DoesNotThrow(a.GetFunc(), a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the delegate completes without exception within the specified <paramref name="timeout"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Func<Task>> CompletesWithin(this ValueAssertions<Func<Task>> a, TimeSpan timeout,
            [CallerArgumentExpression(nameof(timeout))] string? expr = null)
        {
            a.Link("CompletesWithin", expr);
            a.Op(a => FuncChecks.CompleteWithin(a.GetFunc(), timeout, a.IsSkipped(), expr));
            return a;
        }
    }

    namespace Sdk
    {
        public static partial class FuncAssertionsAccessors
        {
            public static Func<Task> GetFunc(this ValueAssertions<Func<Task>> funcAssertions)
            {
                return funcAssertions.GetValue();
            }

            public static TaskExecutionOptions GetOpts(this ValueAssertions<Func<Task>> funcAssertions)
            {
                return TaskExecutionOptionsSlots.GetOrCreate(funcAssertions.GetPipeline());
            }

            public static Task RunWithModeAsync(this ValueAssertions<Func<Task>> a)
            {
                return ExecutionModes.RunWithMode(
                    () => a.GetPipeline().RunAsync(),
                    a.GetOpts(),
                    a.GetPipeline().Settings);
            }
        }
    }
}


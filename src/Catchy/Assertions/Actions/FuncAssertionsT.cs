using System.Diagnostics;
using System.Runtime.CompilerServices;
using Catchy.Sdk;
using Catchy.Sdk.Checks.Actions;

namespace Catchy
{
    public static partial class AsserterExtensions
    {
        public static ValueAssertions<Func<Task<T>>> That<T>(this Asserter a, Func<Task<T>> func,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(func))] string? vExpr = null,
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
            return new ValueAssertions<Func<Task<T>>>(p, func);
        }
    }

    public static partial class FuncAssertionsExtensions
    {
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<Func<Task<T>>> Within<T>(this ValueAssertions<Func<Task<T>>> a, TimeSpan timeout, TimeSpan? every = null,
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

        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<Func<Task<T>>> WithRetry<T>(this ValueAssertions<Func<Task<T>>> a, int times, TimeSpan? every = null,
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

        /// <summary>Asserts that executing the function throws <typeparamref name="TException"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Func<Task<T>>> Throws<T, TException>(this ValueAssertions<Func<Task<T>>> a,
            Func<TException, bool>? predicate = null,
            [CallerArgumentExpression(nameof(predicate))] string? expr = null)
            where TException : Exception
        {
            a.Link("Throws", typeof(TException), expr);
            a.Op(a => FuncChecks.DoesThrow<TException>(a.Wrap(), predicate, a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that executing the function throws any exception.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Func<Task<T>>> ThrowsAny<T>(this ValueAssertions<Func<Task<T>>> a)
        {
            a.Link("ThrowsAny");
            a.Op(a => FuncChecks.DoesThrowAny(a.Wrap(), a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that executing the function does not throw.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Func<Task<T>>> DoesNotThrow<T>(this ValueAssertions<Func<Task<T>>> a)
        {
            a.Link("DoesNotThrow");
            a.Op(a => FuncChecks.DoesNotThrow(a.Wrap(), a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that executing the function succeeds without throwing.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Func<Task<T>>> Succeeds<T>(this ValueAssertions<Func<Task<T>>> a)
        {
            a.Link("Succeeds");
            a.Op(a => FuncChecks.DoesNotThrow(a.Wrap(), a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the function completes within <paramref name="timeout"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Func<Task<T>>> CompletesWithin<T>(this ValueAssertions<Func<Task<T>>> a, TimeSpan timeout,
            [CallerArgumentExpression(nameof(timeout))] string? expr = null)
        {
            a.Link("CompletesWithin", expr);
            a.Op(a => FuncChecks.CompleteWithin(a.Wrap(), timeout, a.IsSkipped(), expr));
            return a;
        }

        /// <summary>Asserts success and captures the execution result.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Func<Task<T>>> Succeeds<T>(this ValueAssertions<Func<Task<T>>> a, out AssertionResult<T> result,
            [CallerArgumentExpression(nameof(result))] string? expr = null)
        {
            a.Link("Succeeds", expr);
            return CaptureResult(a, out result);
        }

        /// <summary>Asserts no exception and captures the execution result.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Func<Task<T>>> DoesNotThrow<T>(this ValueAssertions<Func<Task<T>>> a, out AssertionResult<T> result,
            [CallerArgumentExpression(nameof(result))] string? expr = null)
        {
            a.Link("DoesNotThrow", expr);
            return CaptureResult(a, out result);
        }

        private static Func<Task> Wrap<T>(this ValueAssertions<Func<Task<T>>> a)
            => async () => { await a.GetValue()().ConfigureAwait(false); };

        private static ValueAssertions<Func<Task<T>>> CaptureResult<T>(ValueAssertions<Func<Task<T>>> a, out AssertionResult<T> result)
        {
            var box = new AssertionResult<T>();
            result = box;
            a.Op(a => FuncChecks.CaptureResult(a.GetValue(), box, a.IsSkipped()));
            return a;
        }
    }

    namespace Sdk
    {
        public static partial class FuncAssertionsAccessors
        {
            public static Func<Task<T>> GetFunc<T>(this ValueAssertions<Func<Task<T>>> a) => a.GetValue();
            public static TaskExecutionOptions GetOpts<T>(this ValueAssertions<Func<Task<T>>> a)
                => TaskExecutionOptionsSlots.GetOrCreate(a.GetPipeline());

            public static Task RunWithModeAsync<T>(this ValueAssertions<Func<Task<T>>> a)
            {
                return ExecutionModes.RunWithMode(
                    () => a.GetPipeline().RunAsync(),
                    a.GetOpts(),
                    a.GetPipeline().Settings);
            }
        }
    }
}


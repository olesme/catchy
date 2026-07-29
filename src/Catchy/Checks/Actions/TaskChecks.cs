namespace Catchy.Sdk
{
    public static class TaskChecks
    {
        public static CheckOperation IsCompleted(Task task, TaskExecutionOptions opts, bool isSkipped)
            => CheckOperation.Async(async () =>
            {
                if (opts.Mode == TaskExecutionMode.Now) return task.IsCompleted;
                try { await task.ConfigureAwait(false); } catch { }
                return task.IsCompleted;
            },
            () => $"Expected task to be completed, but status was {task.Status}",
            isSkipped);

        public static CheckOperation IsCancelled(Task task, TaskExecutionOptions opts, bool isSkipped)
            => CheckOperation.Async(async () =>
            {
                if (opts.Mode == TaskExecutionMode.Now) return task.IsCanceled;
                try { await task.ConfigureAwait(false); } catch { }
                return task.IsCanceled;
            },
            () => $"Expected task to be cancelled, but status was {task.Status}",
            isSkipped);

        public static CheckOperation IsNotCancelled(Task task, TaskExecutionOptions opts, bool isSkipped)
            => CheckOperation.Async(async () =>
            {
                if (opts.Mode == TaskExecutionMode.Now) return !task.IsCanceled;
                try { await task.ConfigureAwait(false); } catch { }
                return !task.IsCanceled;
            },
            () => "Expected task not to be cancelled, but it was",
            isSkipped);

        public static CheckOperation HasStatus(Task task, TaskStatus expected, bool isSkipped)
            => CheckOperation.Sync(
                () => task.Status == expected,
                () => $"Expected task status {expected}, but was {task.Status}",
                isSkipped);

        public static CheckOperation Completes(Task task, TaskExecutionOptions opts, bool isSkipped)
        {
            Exception? caught = null;
            var timedOut = false;
            return CheckOperation.Async(async () =>
            {
                caught = null;
                timedOut = false;
                if (opts.Timeout.HasValue)
                {
                    bool done = await Task.WhenAny(task, Task.Delay(opts.Timeout.Value)).ConfigureAwait(false) == task;
                    timedOut = !done;
                    if (done)
                    {
                        try { await task.ConfigureAwait(false); }
                        catch (Exception ex) { caught = ex; }
                    }
                }
                else
                {
                    try { await task.ConfigureAwait(false); }
                    catch (Exception ex) { caught = ex; }
                }

                return caught is null && !timedOut;
            },
            () => timedOut
                ? $"Expected task to complete within {opts.Timeout}, but timed out"
                : caught is not null
                    ? $"Expected task to complete, but threw {TypeHelper.FriendlyName(caught.GetType())}"
                    : "Expected task to complete",
            isSkipped);
        }

        public static CheckOperation CompletesWithin(Task task, TimeSpan timeout, bool isSkipped, string? timeoutExpr = null)
        {
            Exception? caught = null;
            var timedOut = false;
            return CheckOperation.Async(async () =>
            {
                caught = null;
                timedOut = false;
                bool done = await Task.WhenAny(task, Task.Delay(timeout)).ConfigureAwait(false) == task;
                timedOut = !done;
                if (done)
                {
                    try { await task.ConfigureAwait(false); }
                    catch (Exception ex) { caught = ex; }
                }

                return caught is null && !timedOut;
            },
            () => timedOut
                ? $"Expected task to complete within {ExprFormat.Inline(timeout, timeoutExpr)}, but timed out"
                : $"Expected task to complete, but threw {TypeHelper.FriendlyName(caught!.GetType())}",
            isSkipped);
        }

        public static CheckOperation DoesNotThrow(Task task, TaskExecutionOptions opts, bool isSkipped)
        {
            Exception? caught = null;
            return CheckOperation.Async(async () =>
            {
                caught = null;
                if (opts.Mode == TaskExecutionMode.Now)
                {
                    return !task.IsFaulted;
                }

                try { await task.ConfigureAwait(false); }
                catch (Exception ex) { caught = ex; }
                return caught is null;
            },
            () => $"Expected task not to throw, but threw {TypeHelper.FriendlyName((caught ?? task.Exception?.InnerException ?? task.Exception)!.GetType())}",
            isSkipped);
        }

        public static CheckOperation IsCompleted<T>(Task<T> task, TaskExecutionOptions opts, bool isSkipped)
            => CheckOperation.Async(async () =>
            {
                if (opts.Mode == TaskExecutionMode.Now) return task.IsCompleted;
                try { _ = await task.ConfigureAwait(false); } catch { }
                return task.IsCompleted;
            },
            () => $"Expected task to be completed, but status was {task.Status}",
            isSkipped);

        public static CheckOperation Succeeds<T>(Task<T> task, TaskExecutionOptions opts, bool isSkipped)
        {
            Exception? caught = null;
            return CheckOperation.Async(async () =>
            {
                caught = null;
                if (opts.Mode == TaskExecutionMode.Now)
                {
                    return IsCompletedSuccessfully(task);
                }

                try { _ = await task.ConfigureAwait(false); }
                catch (Exception ex) { caught = ex; }
                return caught is null;
            },
            () => caught is not null
                ? $"Expected task to succeed, but threw {caught.GetType().Name}"
                : "Expected task to succeed but it did not complete successfully",
            isSkipped);
        }

        public static CheckOperation CaptureResult<T>(Task<T> task, global::Catchy.AssertionResult<T> result, bool isSkipped)
        {
            Exception? caught = null;
            return CheckOperation.Async(async () =>
            {
                caught = null;
                try { result.Set(await task.ConfigureAwait(false)); }
                catch (Exception ex) { caught = ex; }
                return caught is null;
            },
            () => caught is not null
                ? $"Expected task to succeed, but threw {TypeHelper.FriendlyName(caught.GetType())}"
                : "Expected task to succeed",
            isSkipped);
        }

        public static CheckOperation Throws<T, TException>(Task<T> task, TaskExecutionOptions opts, Func<TException, bool>? predicate, bool isSkipped)
            where TException : Exception
        {
            Exception? caught = null;
            Exception? wrong = null;
            return CheckOperation.Async(async () =>
            {
                caught = null;
                wrong = null;

                if (opts.Mode == TaskExecutionMode.Now)
                {
                    if (!task.IsFaulted) return false;
                    var inner = task.Exception?.InnerException ?? task.Exception?.InnerExceptions[0];
                    if (inner is TException tex && (predicate is null || predicate(tex)))
                    {
                        caught = tex;
                        return true;
                    }

                    wrong = inner;
                    return false;
                }

                try { _ = await task.ConfigureAwait(false); }
                catch (TException ex) when (predicate is null || predicate(ex)) { caught = ex; }
                catch (TException ex) { wrong = ex; }
                catch (Exception ex) { wrong = ex; }

                return wrong is null && caught is not null;
            },
            () => wrong is not null
                ? $"Expected task to throw {typeof(TException).Name}, but threw {wrong.GetType().Name}"
                : $"Expected task to throw {typeof(TException).Name}, but completed",
            isSkipped);
        }

        public static CheckOperation DoesNotThrow<T>(Task<T> task, TaskExecutionOptions opts, bool isSkipped)
        {
            Exception? caught = null;
            return CheckOperation.Async(async () =>
            {
                caught = null;
                if (opts.Mode == TaskExecutionMode.Now)
                {
                    return !task.IsFaulted;
                }

                try { _ = await task.ConfigureAwait(false); }
                catch (Exception ex) { caught = ex; }
                return caught is null;
            },
            () => $"Expected task not to throw, but threw {TypeHelper.FriendlyName((caught ?? task.Exception?.InnerException ?? task.Exception)!.GetType())}",
            isSkipped);
        }

        public static CheckOperation CompletesWithin<T>(Task<T> task, TaskExecutionOptions opts, TimeSpan timeout, bool isSkipped)
        {
            Exception? caught = null;
            var timedOut = false;
            return CheckOperation.Async(async () =>
            {
                timedOut = false;
                caught = null;
                if (opts.Mode == TaskExecutionMode.Now) return task.IsCompleted;

                try
                {
                    bool done = await Task.WhenAny(task, Task.Delay(timeout)).ConfigureAwait(false) == task;
                    timedOut = !done;
                    if (done) _ = await task.ConfigureAwait(false);
                }
                catch (Exception ex) { caught = ex; }

                return caught is null && !timedOut;
            },
            () => caught is not null
                ? $"Expected task to complete within {timeout}, but threw {TypeHelper.FriendlyName(caught.GetType())}"
                : $"Expected task to complete within {timeout}, but timed out",
            isSkipped);
        }

        public static CheckOperation Completes<T>(Task<T> task, TaskExecutionOptions opts, bool isSkipped)
        {
            Exception? caught = null;
            var timedOut = false;
            return CheckOperation.Async(async () =>
            {
                caught = null;
                timedOut = false;
                if (opts.Timeout.HasValue)
                {
                    bool done = await Task.WhenAny(task, Task.Delay(opts.Timeout.Value)).ConfigureAwait(false) == task;
                    timedOut = !done;
                    if (done)
                    {
                        try { _ = await task.ConfigureAwait(false); }
                        catch (Exception ex) { caught = ex; }
                    }
                }
                else
                {
                    try { _ = await task.ConfigureAwait(false); }
                    catch (Exception ex) { caught = ex; }
                }

                return caught is null && !timedOut;
            },
            () => timedOut
                ? $"Expected task to complete within {opts.Timeout}, but timed out"
                : caught is not null
                    ? $"Expected task to complete, but threw {TypeHelper.FriendlyName(caught.GetType())}"
                    : "Expected task to complete",
            isSkipped);
        }

        private static bool IsCompletedSuccessfully(Task task) =>
#if NET5_0_OR_GREATER
            task.IsCompletedSuccessfully;
#else
            task.IsCompleted && !task.IsFaulted && !task.IsCanceled;
#endif
    }
}

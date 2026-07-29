namespace Catchy.Sdk
{
    public static class FuncChecks
    {
        public static CheckOperation DoesThrow<TException>(Func<Task> func, Func<TException, bool>? predicate, bool isSkipped)
            where TException : Exception
        {
            Exception? caught = null; Exception? wrong = null;
            return CheckOperation.Async(async () =>
            {
                caught = null; wrong = null;
                try { await func().ConfigureAwait(false); }
                catch (TException ex) when (predicate is null || predicate(ex)) { caught = ex; }
                catch (TException ex) { wrong = ex; }
                catch (Exception ex) { wrong = ex; }
                return wrong is null && caught is not null;
            },
            () => wrong is not null
                ? $"Expected func to throw {typeof(TException).Name}, but threw {wrong.GetType().Name}"
                : $"Expected func to throw {typeof(TException).Name}, but completed",
             isSkipped);
        }

        public static CheckOperation DoesThrowAny(Func<Task> func, bool isSkipped)
        {
            Exception? caught = null;
            return CheckOperation.Async(async () =>
            {
                caught = null;
                try { await func().ConfigureAwait(false); } catch (Exception ex) { caught = ex; }
                return caught is not null;
            },
            () => "Expected func to throw, but completed",  isSkipped);
        }

        public static CheckOperation DoesNotThrow(Func<Task> func, bool isSkipped)
        {
            Exception? caught = null;
            return CheckOperation.Async(async () =>
            {
                caught = null;
                try { await func().ConfigureAwait(false); } catch (Exception ex) { caught = ex; }
                return caught is null;
            },
            () => $"Expected func not to throw, but threw {caught!.GetType().Name}",
             isSkipped);
        }

        public static CheckOperation CompleteWithin(Func<Task> func, TimeSpan timeout, bool isSkipped, string? timeoutExpr = null)
        {
            Exception? caught = null; bool timedOut = false;
            return CheckOperation.Async(async () =>
            {
                caught = null; timedOut = false;
                try
                {
                    var t = func();
                    bool completed = await Task.WhenAny(t, Task.Delay(timeout)).ConfigureAwait(false) == t;
                    timedOut = !completed;
                    if (completed) await t.ConfigureAwait(false);
                }
                catch (Exception ex) { caught = ex; }
                return caught is null && !timedOut;
            },
            () => caught is not null ? $"Expected func to complete within {ExprFormat.Inline(timeout, timeoutExpr)}, but threw {caught.GetType().Name}"
                : $"Expected func to complete within {ExprFormat.Inline(timeout, timeoutExpr)}, but timed out",
             isSkipped);
        }

        public static CheckOperation CaptureResult<T>(Func<Task<T>> func, global::Catchy.AssertionResult<T> result, bool isSkipped)
        {
            Exception? caught = null;
            return CheckOperation.Async(async () =>
            {
                caught = null;
                try { result.Set(await func().ConfigureAwait(false)); }
                catch (Exception ex) { caught = ex; }
                return caught is null;
            },
            () => caught is not null
                ? $"Expected func to succeed, but threw {TypeHelper.FriendlyName(caught.GetType())}"
                : "Expected func to succeed",
            isSkipped);
        }
    }
}

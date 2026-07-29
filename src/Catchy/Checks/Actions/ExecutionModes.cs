namespace Catchy.Sdk.Checks.Actions
{
    public static class ExecutionModes
    {
        public static async Task RunWithMode(
            Func<Task> execute, TaskExecutionOptions opts, AssertionSettings settings,
            Action? onBeforeRetry = null)
        {
            var mode = opts.Mode == TaskExecutionMode.Default
                ? settings.DefaultFuncMode : opts.Mode;
            switch (mode)
            {
                case TaskExecutionMode.Polling: await RunPolling(execute, opts, settings, onBeforeRetry); break;
                case TaskExecutionMode.Retry: await RunRetry(execute, opts, onBeforeRetry); break;
                default: await execute(); break;
            }
        }

        public static async Task RunPolling(
            Func<Task> execute, TaskExecutionOptions opts, AssertionSettings settings,
            Action? onBeforeRetry = null)
        {
            var timeout = opts.Timeout ?? settings.DefaultPollingTimeout;
            var deadline = DateTime.UtcNow + timeout;
            AssertionException? last = null;
            while (DateTime.UtcNow < deadline)
            {
                onBeforeRetry?.Invoke();
                last = null;
                try { await execute().ConfigureAwait(false); return; }
                catch (AssertionException ex) { last = ex; }
                var rem = deadline - DateTime.UtcNow;
                if (rem <= TimeSpan.Zero) break;
                await Task.Delay(opts.Interval < rem ? opts.Interval : rem).ConfigureAwait(false);
            }
            if (last is not null) throw last;
        }

        public static async Task RunRetry(
            Func<Task> execute, TaskExecutionOptions opts,
            Action? onBeforeRetry = null)
        {
            AssertionException? last = null;
            for (int i = 0; i <= opts.MaxRetries; i++)
            {
                if (i > 0) await Task.Delay(opts.Interval).ConfigureAwait(false);
                onBeforeRetry?.Invoke();
                last = null;
                try { await execute().ConfigureAwait(false); return; }
                catch (AssertionException ex) { last = ex; }
            }
            if (last is not null) throw last;
        }
    }
}

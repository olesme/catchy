using Microsoft.Playwright;

namespace Catchy.Sdk
{
    public static class PwPolling
    {
        /// <summary>
        /// Polls every 50 ms until <paramref name="predicate"/> returns <c>true</c>
        /// or the timeout from <paramref name="opts"/> elapses.
        /// Captures ANY exception from Playwright operations and stores it.
        /// When polling deadline is exceeded, rethrows the last exception so the
        /// assertion pipeline can wrap it with AssertionException and execute all hooks.
        /// OnInterimError are invoked for each captured exception to allow logging/diagnostics.
        /// </summary>
        public static async Task<bool> PollUntilAsync(Func<Task<bool>> predicate, Func<float?> timeoutMs, AssertionPipeline? pipeline = null)
        {
            var timeout = TimeSpan.FromMilliseconds(timeoutMs() ?? 5_000);
            var deadline = DateTime.UtcNow + timeout;
            Exception? lastException = null;

            while (true)
            {
                try
                {
                    if (await predicate().ConfigureAwait(false)) return true;
                    lastException = null; // Reset on successful predicate execution
                }
                catch (Exception ex)
                {
                    // Capture ANY exception from Playwright operations (timeout, element not found, etc.)
                    lastException = ex;

                    // Invoke interim error callbacks if pipeline provided
                    if (pipeline != null)
                    {
                        var callbacks = pipeline.Settings.OnInterimError;
                        if (callbacks.Count > 0)
                        {
                            foreach (var callback in callbacks)
                            {
                                try
                                {
                                    await callback(ex, pipeline).ConfigureAwait(false);
                                }
                                catch
                                {
                                    // Silently ignore callback errors - don't break polling
                                }
                            }
                        }
                    }
                    // Don't rethrow immediately - let polling loop continue with next attempt
                }

                var remaining = deadline - DateTime.UtcNow;
                if (remaining <= TimeSpan.Zero)
                {
                    // Polling timeout exceeded. Rethrow the last captured exception so the
                    // assertion pipeline can handle it properly with wrapping and hooks.
                    if (lastException is not null)
                        throw lastException;
                    return false;
                }

                await Task.Delay(remaining < TimeSpan.FromMilliseconds(50)
                    ? remaining
                    : TimeSpan.FromMilliseconds(50)).ConfigureAwait(false);
            }
        }
    }
}

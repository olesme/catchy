using System.Diagnostics;
using Catchy.Sdk;

namespace Catchy.Sdk
{
    public static class ExecutionWrapperExamples
    {
        /// <summary>
        /// Simple telemetry wrapper that measures pipeline duration and reports via callback.
        /// </summary>
        public static Func<AssertionPipeline, Func<Task>, Task> Telemetry(string spanName,
            Action<string, TimeSpan, bool>? report = null)
        {
            return async (pipeline, next) =>
            {
                var sw = Stopwatch.StartNew();
                try
                {
                    await next().ConfigureAwait(false);
                    sw.Stop();
                    report?.Invoke(spanName, sw.Elapsed, true);
                }
                catch
                {
                    sw.Stop();
                    report?.Invoke(spanName, sw.Elapsed, false);
                    throw;
                }
            };
        }

        /// <summary>
        /// Evidence-on-failure wrapper. When the pipeline throws, runs the provided
        /// async softAssert-capture function with the live pipeline instance before rethrowing.
        /// </summary>
        public static Func<AssertionPipeline, Func<Task>, Task> EvidenceOnFailure(Func<AssertionPipeline, Task> capture)
        {
            return async (pipeline, next) =>
            {
                try
                {
                    await next().ConfigureAwait(false);
                }
                catch (Exception)
                {
                    try { await capture(pipeline).ConfigureAwait(false); } catch { }
                    throw;
                }
            };
        }
    }
}

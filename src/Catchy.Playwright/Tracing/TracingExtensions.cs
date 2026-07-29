using System.Diagnostics;
using Microsoft.Playwright;

namespace Catchy.Tracing
{
    public static  class TracingExtensions
    {
        /// <summary>
        /// Inject a message into the Playwright trace. This will create a new group in the trace with the given message as the title. 
        /// The group will be automatically closed after the method returns.
        /// Use this method to add custom messages to the trace, which can help with debugging and understanding the flow of the test.
        /// </summary>
        /// <param name="browserContext">The browser context to trace.</param>
        /// <param name="message">The message to inject into the trace.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [DebuggerHidden, StackTraceHidden]
        public static async Task AddTraceMessage(this IBrowserContext browserContext, string message)
        {
            if (browserContext is null) return;
            await browserContext.Tracing.GroupAsync(message);
            await browserContext.Tracing.GroupEndAsync();
        }

        /// <summary>
        /// Writes an error message to the trace output for the specified browser context, prefixing the message with an
        /// error indicator.
        /// </summary>
        /// <param name="browserContext">The browser context to which the error message will be traced. Cannot be null.</param>
        /// <param name="message">The error message to write to the trace output.</param>
        /// <returns>A task that represents the asynchronous trace operation.</returns>
        [DebuggerHidden, StackTraceHidden]
        public static async Task TraceError(this IBrowserContext browserContext, string message)
        {
            await browserContext.AddTraceMessage($"❌ {message}");
        }

        /// <summary>
        /// Writes a success message to the trace output of the specified browser context, prefixed with a check mark
        /// symbol.
        /// </summary>
        /// <param name="browserContext">The browser context to which the trace message is written. Cannot be null.</param>
        /// <param name="message">The message to include in the trace output. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous trace operation.</returns>
        [DebuggerHidden, StackTraceHidden]
        public static async Task TraceSuccess(this IBrowserContext browserContext, string message)
        {
            await browserContext.AddTraceMessage($"✔ {message}");
        }

        /// <summary>
        /// Writes an informational trace message to the specified browser context asynchronously.
        /// </summary>
        /// <param name="browserContext">The browser context to which the trace message is written. Cannot be null.</param>
        /// <param name="message">The informational message to include in the trace. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous trace operation.</returns>
        [DebuggerHidden, StackTraceHidden]
        public static async Task TraceInfo(this IBrowserContext browserContext, string message)
        {
            await browserContext.AddTraceMessage($"ℹ {message}");
        }


        /// <summary>
        /// Writes a warning-level trace message to the specified browser context asynchronously.
        /// </summary>
        /// <remarks>The message is prefixed with a warning indicator to distinguish it from other trace
        /// messages.</remarks>
        /// <param name="browserContext">The browser context to which the warning message is written. Cannot be null.</param>
        /// <param name="message">The warning message to write. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous trace operation.</returns>
        [DebuggerHidden, StackTraceHidden]
        public static async Task TraceWarning(this IBrowserContext browserContext, string message)
        {
            await browserContext.AddTraceMessage($"⚠ {message}");
        }
    }
}

using System;
using System.Reflection;
using Reqnroll;
using System.Diagnostics;

namespace Catchy.ReqnrollPlugin
{
    internal static class ScenarioContextExtensions
    {
        private static readonly PropertyInfo? _testErrorProp =
            typeof(ScenarioContext).GetProperty("TestError",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        private static readonly PropertyInfo? _executionStatusProp =
            typeof(ScenarioContext).GetProperty("ScenarioExecutionStatus",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        private static readonly PropertyInfo? _stopwatchProp =
            typeof(ScenarioContext).GetProperty("Stopwatch",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        private const string SoftFailInjectionKey = "_-=SoftFailInjeCtionKeY=-_";

        /// <summary>
        /// Marks scenario as failed without throwing so subsequent AfterScenario hooks
        /// see ctx.TestError != null and can react (screenshots, reports, etc.).
        /// </summary>
        public static void InjectError(this ScenarioContext ctx, Exception ex)
        {
            if (ctx.TestError != null) return;
            _testErrorProp?.SetValue(ctx, ex);
            _executionStatusProp?.SetValue(ctx,
                    Enum.Parse(_executionStatusProp.PropertyType, "TestError"));
            ctx.Set(true, SoftFailInjectionKey);
        }

        public static Stopwatch? GetStopwatch(this ScenarioContext ctx)
        {
            return _stopwatchProp?.GetValue(ctx) as Stopwatch;
        }

        public static bool IsSoftFailInjected(this ScenarioContext ctx)
        {
            if (ctx.TryGetValue<bool>(SoftFailInjectionKey, out bool found))
            {
                return found;
            }
            return false;
        }
    }
}

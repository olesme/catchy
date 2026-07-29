using System.Reflection;
using Catchy;
using Catchy.Sdk;

namespace CatchyTestHelpers
{
    /// <summary>
    /// Helper for test isolation - clears global registry state using reflection
    /// to access private fields without exposing them publicly.
    /// </summary>
    public static class RegistryTestHelper
    {
        /// <summary>
        /// Clears all registered deep-equal rules from the global registry.
        /// Use at the start or end of tests that register global rules.
        /// </summary>
        public static void ClearDeepEqualRuleRegistry()
        {
            var registryType = typeof(DeepEqualRuleRegistry);
            // Try multiple BindingFlags since reflection can be finicky with access to static fields
            var rulesField = registryType.GetField("_rules", BindingFlags.NonPublic | BindingFlags.Static) ??
                registryType.GetField("_rules", BindingFlags.Static) ??
                registryType.GetField("_rules", BindingFlags.NonPublic) ??
                registryType.GetField("_rules");

            if (rulesField?.GetValue(null) is System.Collections.IDictionary rules)
            {
                rules.Clear();
            }
        }

        /// <summary>
        /// Gets the count of registered deep-equal rules in the global registry.
        /// Useful for asserting that cleanup worked.
        /// </summary>
        public static int GetRegisteredDeepEqualRuleCount()
        {
            var registryType = typeof(DeepEqualRuleRegistry);
            // Try multiple BindingFlags since reflection can be finicky with access to static fields
            var rulesField = registryType.GetField("_rules", BindingFlags.NonPublic | BindingFlags.Static) ??
                registryType.GetField("_rules", BindingFlags.Static) ??
                registryType.GetField("_rules", BindingFlags.NonPublic) ??
                registryType.GetField("_rules");

            if (rulesField?.GetValue(null) is System.Collections.IDictionary rules)
            {
                return rules.Count;
            }

            return 0;
        }

        /// <summary>
        /// Clears all registered ordering rules from the global registry.
        /// Use at the start or end of tests that register global rules.
        /// </summary>
        public static void ClearOrderingRuleRegistry()
        {
            var registryType = typeof(OrderingRuleRegistry);
            // Try multiple BindingFlags since reflection can be finicky with access to static fields
            var rulesField = registryType.GetField("_rules", BindingFlags.NonPublic | BindingFlags.Static) ??
                registryType.GetField("_rules", BindingFlags.Static) ??
                registryType.GetField("_rules", BindingFlags.NonPublic) ??
                registryType.GetField("_rules");

            if (rulesField?.GetValue(null) is System.Collections.IDictionary rules)
            {
                rules.Clear();
            }
        }

        /// <summary>
        /// Gets the count of registered ordering rules in the global registry.
        /// Useful for asserting that cleanup worked.
        /// </summary>
        public static int GetRegisteredOrderingRuleCount()
        {
            var registryType = typeof(OrderingRuleRegistry);
            // Try multiple BindingFlags since reflection can be finicky with access to static fields
            var rulesField = registryType.GetField("_rules", BindingFlags.NonPublic | BindingFlags.Static) ??
                registryType.GetField("_rules", BindingFlags.Static) ??
                registryType.GetField("_rules", BindingFlags.NonPublic) ??
                registryType.GetField("_rules");

            if (rulesField?.GetValue(null) is System.Collections.IDictionary rules)
            {
                return rules.Count;
            }

            return 0;
        }

        /// <summary>
        /// Clears all per-instance containers on AssertionSettings.Global.
        /// This includes OrderingRules and DeepEqualRules that persist on the singleton asserter.
        /// Must be called after tests that modify global asserter state to prevent leakage.
        /// </summary>
        public static void ClearPerInstanceContainers()
        {
            try
            {
                // Directly create new empty containers instead of trying to clear existing ones
                AssertionSettings.Global.OrderingRules = null;
                AssertionSettings.Global.DeepEqualRules = null;
                AssertionSettings.Global.EqualsOptions = null;
            }
            catch
            {
                // If direct assignment fails, try the reflection approach
                var settingsType = typeof(AssertionSettings);
                var globalProperty = settingsType.GetProperty("Global",
                    BindingFlags.Public | BindingFlags.Static);

                if (globalProperty?.GetValue(null) is AssertionSettings global)
                {
                    // Try to clear OrderingRules container if it exists
                    try
                    {
                        if (global.OrderingRules != null)
                        {
                            var containerType = global.OrderingRules.GetType();
                            var lockField = containerType.GetField("_lock",
                                BindingFlags.NonPublic | BindingFlags.Instance);
                            var rulesField = containerType.GetField("_rules",
                                BindingFlags.NonPublic | BindingFlags.Instance);

                            if (lockField?.GetValue(global.OrderingRules) is object lockObj &&
                                rulesField?.GetValue(global.OrderingRules) is System.Collections.IDictionary orderingRules)
                            {
                                lock (lockObj)
                                {
                                    orderingRules.Clear();
                                }
                            }
                        }
                        global.OrderingRules = null;
                    }
                    catch { }

                    // Try to clear DeepEqualRules container if it exists
                    try
                    {
                        if (global.DeepEqualRules != null)
                        {
                            var containerType = global.DeepEqualRules.GetType();
                            var lockField = containerType.GetField("_lock",
                                BindingFlags.NonPublic | BindingFlags.Instance);
                            var rulesField = containerType.GetField("_rules",
                                BindingFlags.NonPublic | BindingFlags.Instance);

                            if (lockField?.GetValue(global.DeepEqualRules) is object lockObj &&
                                rulesField?.GetValue(global.DeepEqualRules) is System.Collections.IDictionary deepEqualRules)
                            {
                                lock (lockObj)
                                {
                                    deepEqualRules.Clear();
                                }
                            }
                        }
                        global.DeepEqualRules = null;
                    }
                    catch { }
                }
            }
        }
    }
}

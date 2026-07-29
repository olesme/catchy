using Catchy.Sdk;

namespace Catchy
{
    public enum ShowDurationMode { Never, AsyncOnly, Always }

    /// <summary>
    /// Configuration for assertion behavior, callbacks, tolerances, and rules.
    /// 
    /// There are three configuration scopes with different mutability contracts:
    /// 
    /// 1. **GLOBAL** (<see cref="AssertionSettings.Global"/>) — Process-wide singleton
    ///    - Affects all asserters that don't specify their own settings
    ///    - Use <c>Set*</c> methods to mutate (unsafe in parallel tests)
    ///    - Should only be configured before any tests run
    ///    - Example: <c>AssertionSettings.Global.CatchAll = true</c>
    /// 
    /// 2. **INSTANCE** (per-asserter settings) — Isolated to one asserter
    ///    - Each asserter can have its own <see cref="AssertionSettings"/> instance
    ///    - Safe for parallel tests if each test gets its own asserter
    ///    - Configure via lambda before creating asserter
    ///    - Example: <c>Asserter.NewStateful(s => s.CatchAll = true)</c>
    /// 
    /// 3. **PER-CHAIN** (within assertion chains) — Inline overrides
    ///    - Use trailing <c>.With(...)</c> modifiers on assertion chains
    ///    - Never mutates; always creates new instances via snapshot
    ///    - Thread-safe, use freely
    /// 
    /// For detailed guidance, see docs/CONFIGURATION_IMPLEMENTATION_CONTRACT.md
    /// </summary>
    public sealed class AssertionSettings
    {
        /// <summary>
        /// Global process-wide singleton settings.
        /// 
        /// WARNING: Mutations via <c>Set*</c> methods are NOT thread-safe and will cause
        /// race conditions and flaky tests if modified during parallel test execution.
        /// 
        /// IMPORTANT: Configure this ONLY before any tests run:
        /// - Module initialization ([ModuleInitializer])
        /// - Assembly load hooks
        /// - Test class setup methods (if tests run sequentially)
        /// 
        /// For per-test configuration, use <see cref="Asserter.NewStateful(Action{AssertionSettings})"/> instead.
        /// </summary>
        public static readonly AssertionSettings Global = new();

        /// <summary>
        /// Fires for every assertion result (passed, failed, skipped).
        /// Filter by <see cref="AssertionInfo.Status"/> for status-specific handling.
        /// </summary>
        public List<Func<AssertionInfo, ValueTask>> OnAssertion { get; set; } = [];

        /// <summary>
        /// Fires immediately when a soft assertion is captured, before execution continues.
        /// Primary integration point for screenshots, trace attachments, and report hooks.
        /// </summary>
        public List<Func<AssertionInfo, ValueTask>> OnSoftFailure { get; set; } = [];

        /// <summary>
        /// Fires for interim/intermediate errors during polling or retry loops.
        /// Receives the exception, pipeline reference, and chain/source info for logging/diagnostics.
        /// Useful for capturing debug traces, logs, or partial failures without stopping the pipeline.
        /// Remains optional - by default silently captured for final error reporting.
        /// </summary>
        public List<Func<Exception, AssertionPipeline, ValueTask>> OnInterimError { get; set; } = [];

        /// <summary>
        /// Lightweight execution wrappers executed around the pipeline run.
        /// Each wrapper receives the live <see cref="AssertionPipeline"/> and a <c>next</c>
        /// delegate which it must invoke to continue pipeline execution.
        ///
        /// Use wrappers for pre/post-run instrumentation (telemetry spans, temporary
        /// slot overrides, softAssert capture) or for adaptive retry/short-circuiting.
        /// </summary>
        public List<Func<AssertionPipeline, Func<ValueTask>, ValueTask>> OnExecution { get; set; } = [];

        public bool CatchAll { get; set; } = false;
        public List<Type> CaughtExceptionTypes { get; set; } = [];
        public double? DefaultFloatTolerance { get; set; }
        public bool TreatNullAsDefault { get; set; }
        public StringComparison DefaultStringComparison { get; set; } = StringComparison.Ordinal;
        public int MaxStringDisplayLength { get; set; } = 120;
        public int MaxCollectionDisplayItems { get; set; } = 20;
        public int MaxChainLinkLength { get; set; } = 60;
        public int MaxHintLength { get; set; } = 200;
        public TimeSpan DefaultPollingTimeout { get; set; } = TimeSpan.FromSeconds(5);
        public TimeSpan DefaultPollingInterval { get; set; } = TimeSpan.FromMilliseconds(50);
        public TaskExecutionMode DefaultTaskMode { get; set; } = TaskExecutionMode.Default;
        public TaskExecutionMode DefaultFuncMode { get; set; } = TaskExecutionMode.Default;
        public bool CollectionFailFast { get; set; } = false;
        public ShowDurationMode ShowDuration { get; set; } = ShowDurationMode.AsyncOnly;
        /// <summary>
        /// Collection size at which sync predicate checks switch to parallel Task.WhenAll.
        /// Below threshold: sequential, zero Task allocation.
        /// </summary>
        public int SyncParallelThreshold { get; set; } = 64;

        /// <summary>
        /// Per-asserter deep-equal rules. Checked before the global
        /// <see cref="DeepEqualRuleRegistry"/>. Useful for test-suite isolation
        /// without touching global state.
        /// </summary>
        public DeepEqualRuleContainer? DeepEqualRules { get; set; }

        /// <summary>
        /// Per-asserter equals options (like ignore case, ignore collection order, etc.)
        /// Used when comparing objects with deep equality checks.
        /// </summary>
        public EqualsOptions? EqualsOptions { get; set; }

        /// <summary>
        /// Per-asserter ordering rules. Checked before the global
        /// <see cref="OrderingRuleRegistry"/>.
        /// </summary>
        public OrderingRuleContainer? OrderingRules { get; set; }

        public AssertionSettings Clone(Action<AssertionSettings>? configure = null)
        {
            var clone = new AssertionSettings
            {
                // Simple/value-like properties (copied)
                CatchAll = this.CatchAll,
                DefaultFloatTolerance = this.DefaultFloatTolerance,
                TreatNullAsDefault = this.TreatNullAsDefault,
                DefaultStringComparison = this.DefaultStringComparison,
                MaxStringDisplayLength = this.MaxStringDisplayLength,
                MaxCollectionDisplayItems = this.MaxCollectionDisplayItems,
                MaxChainLinkLength = this.MaxChainLinkLength,
                MaxHintLength = this.MaxHintLength,
                DefaultPollingTimeout = this.DefaultPollingTimeout,
                DefaultPollingInterval = this.DefaultPollingInterval,
                DefaultTaskMode = this.DefaultTaskMode,
                DefaultFuncMode = this.DefaultFuncMode,
                CollectionFailFast = this.CollectionFailFast,
                ShowDuration = this.ShowDuration,
                SyncParallelThreshold = this.SyncParallelThreshold,
                // Clone lists (create independent lists; elements are shared references where safe)
                OnAssertion = this.OnAssertion != null
                    ? [.. this.OnAssertion]
                    : [],

                OnSoftFailure = this.OnSoftFailure != null
                    ? [.. this.OnSoftFailure]
                    : [],

                OnInterimError = this.OnInterimError != null
                    ? [.. this.OnInterimError]
                    : [],

                OnExecution = this.OnExecution != null
                    ? [.. this.OnExecution]
                    : [],

                CaughtExceptionTypes = this.CaughtExceptionTypes != null
                    ? [.. this.CaughtExceptionTypes]
                    : [],

                // Clone complex containers using their Clone implementations (if present)
                DeepEqualRules = this.DeepEqualRules?.Clone(),
                OrderingRules = this.OrderingRules?.Clone(),

                // Clone EqualsOptions to avoid sharing ExcludedProperties list etc.
                EqualsOptions = this.EqualsOptions?.Clone()
            };

            // Call configure last so it mutates only the independent clone
            configure?.Invoke(clone);

            return clone;
        }
    }
}

using System.Diagnostics;
using System.Text;

namespace Catchy.Sdk
{
    public static partial class PipelineExtensions
    {
        /// <summary>
        /// Creates an assertion pipeline and builds the entry-point chain link.
        /// All parameters are named — pass only what applies to your entry point.
        /// </summary>
        /// <param name="asserterExpr">
        ///   CAE-captured name of the asserter variable (e.g. "Stateless.Verify", "softAssert").
        ///   Defaults to "Stateless.Verify" when null.
        /// </param>
        /// <param name="methodName">Entry-point method name ("That", "ThatEachOf", etc.).</param>
        /// <param name="file">Pass your [CallerFilePath] value.</param>
        /// <param name="line">Pass your [CallerLineNumber] value.</param>
        /// <param name="member">Pass your [CallerMemberName] value.</param>
        /// <param name="valueExpr">Single CAE-captured value expression (most entry points).</param>
        /// <param name="valueExprs">
        ///   Multiple CAE-captured expressions (arity-N generated overloads).
        ///   Mutually exclusive with <paramref name="valueExpr"/>.
        /// </param>
        /// <param name="genericType">Single generic type argument (e.g. That&lt;T&gt;).</param>
        /// <param name="genericTypes">
        ///   Multiple generic type arguments.
        ///   Mutually exclusive with <paramref name="genericType"/>.
        /// </param>
        public static AssertionPipeline NewPipeline(
            this Asserter a,
            string? asserterExpr = null,
            string methodName = "That",
            string? file = null,
            int line = 0,
            string? member = null,
            string? valueExpr = null,
            string?[]? valueExprs = null,
            Type? genericType = null,
            IEnumerable<Type>? genericTypes = null)
        {
            var p = new AssertionPipeline(
                a._settings,
                new SourceLocation(file, line, member),
                a._softState,
                a._isNoOp);

            BuildEntryLink(
                p.Links,
                asserterExpr,
                methodName,
                types: genericType is not null ? (IEnumerable<Type>)[genericType] : genericTypes,
                exprs: valueExpr is not null ? [valueExpr] : valueExprs);

            return p;
        }

        private static void BuildEntryLink(
            List<string> links,
            string? asserterExpr,
            string methodName,
            IEnumerable<Type>? types,
            string?[]? exprs)
        {
            links.Add(asserterExpr ?? "");

            if (types is not null)
            {
                links.Add($".{methodName}<");
                bool firstType = true;
                foreach (var t in types)
                {
                    if (!firstType) links.Add(", ");
                    links.Add(TypeHelper.FriendlyName(t));
                    firstType = false;
                }
                links.Add(">(");
            }
            else
            {
                links.Add($".{methodName}(");
            }

            if (exprs is not null)
            {
                bool first = true;
                foreach (var e in exprs)
                {
                    if (string.IsNullOrEmpty(e)) continue;
                    if (!first) links.Add(", ");
                    links.Add(e!);
                    first = false;
                }
            }

            links.Add(")");
        }
    }

    public sealed class AssertionPipeline
    {
        public AssertionSettings Settings { get; set; }
        public SoftState? SoftState { get; set; }
        internal bool _isNoOp;
        public readonly List<string> Links = [];
        public readonly List<CheckOperation> Ops = [];

        public readonly SourceLocation Source;
        public bool IsSkipped { get; set; } = false;
        public string? BecauseMessage { get; set; } = null;

        private SlotContainer? _container;
        private Dictionary<Type, object>? _orderingRuleOverrides;
        private DeepEqualRuleContainer? _deepEqualRuleContainer;
        private IDeepEqualRule? _deepEqualRule;
        private readonly bool _bypassExecutionWrappersAndHooks;

        public SlotContainer Slots { get { _container ??= new(); return _container; } }
        public bool IsSoft => SoftState is not null;
        public bool HasFailures => SoftState?.HasFailures ?? false;

        internal AssertionPipeline(AssertionSettings settings, SourceLocation source, SoftState? softState = null, bool isNoOp = false, bool bypassExecutionWrappersAndHooks = false)
        {
            Settings = settings;
            SoftState = softState;
            _isNoOp = isNoOp;
            _bypassExecutionWrappersAndHooks = bypassExecutionWrappersAndHooks;
            Source = source;
        }

        public void SetOrderingRule<T>(IOrderingRule<T> rule)
        {
            _orderingRuleOverrides ??= [];
            _orderingRuleOverrides[typeof(T)] = rule;
        }

        public IOrderingRule<T>? GetOrderingRule<T>()
        {
            if (_orderingRuleOverrides?.TryGetValue(typeof(T), out var obj) == true)
                return obj as IOrderingRule<T>;
            return null;
        }

        public IOrderingRule<T>? GetEffectiveOrderingRule<T>()
            => GetOrderingRule<T>() ?? Settings.OrderingRules?.TryGet<T>();

        public DeepEqualRuleContainer GetDeepEqualRuleContainer()
        {
            _deepEqualRuleContainer ??= new();
            return _deepEqualRuleContainer;
        }

        public DeepEqualRuleContainer? GetEffectiveDeepEqualRuleContainer()
        {
            var chainRules = _deepEqualRuleContainer;
            var settingsRules = Settings.DeepEqualRules;

            if (chainRules is null) return settingsRules;
            if (settingsRules is null) return chainRules;

            var merged = settingsRules.Clone();
            merged.MergeFrom(chainRules);
            return merged;
        }

        public void SetDeepEqualRule(IDeepEqualRule? rule)
        {
            _deepEqualRule = rule;
        }

        public IDeepEqualRule? GetDeepEqualRule()
        {
            return _deepEqualRule;
        }

        public IDeepEqualRule? GetEffectiveDeepEqualRule()
            => GetDeepEqualRule();

        public void AddOp(CheckOperation op)
        {
            Ops.Add(op);

        }

        [DebuggerHidden, StackTraceHidden]
        internal async Task RunAsync()
        {
            if (Ops.Count == 0 || IsSkipped || _isNoOp) return;

            var wrappers = _bypassExecutionWrappersAndHooks ? [] : Settings.OnExecution;

            [DebuggerHidden, StackTraceHidden]
            async ValueTask core()
            {
                IReadOnlyList<string> links = Links;

                int i = 0;
                while (i < Ops.Count)
                {
                    var op = Ops[i];
                    if (i + 1 < Ops.Count && Ops[i + 1].IsOr)
                    {
                        var group = new List<CheckOperation> { op };
                        int j = i + 1;
                        while (j < Ops.Count && Ops[j].IsOr)
                        {
                            group.Add(Ops[j]);
                            j++;
                        }
                        await ExecuteOrGroupAsync(group, links).ConfigureAwait(false);
                        i = j;
                    }
                    else
                    {
                        await ExecuteAsync(op, links).ConfigureAwait(false);
                        i++;
                    }
                }
            }

            Func<ValueTask> next = core;
            if (wrappers.Count > 0)
            {
                // Build wrapper chain: last added wrapper executes outermost.
                for (int w = wrappers.Count - 1; w >= 0; w--)
                {
                    var wrapper = wrappers[w];
                    var curNext = next;
                    next = () => wrapper(this, curNext);
                }
            }

            try
            {
                await next().ConfigureAwait(false);
            }
            catch (AssertionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                // Final guardrail: never leak raw exceptions out of assertion await path.
                var (chain, trunc) = ChainRenderer.Render(Links, Settings.MaxChainLinkLength);
                var sb = new StringBuilder();

                if (!string.IsNullOrEmpty(chain))
                {
                    sb.AppendLine("Assertion failed:");
                    sb.Append("  ");
                    sb.AppendLine(chain);
                    sb.AppendLine();
                }

                if (!string.IsNullOrEmpty(BecauseMessage)) sb.AppendLine($"Because  : {BecauseMessage}");
                sb.Append($"{ex.GetType().Name}: {ex.Message}");
                AppendTruncations(sb, trunc);
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine($"Source   : {Path.GetFileName(Source.File)}:{Source.Line} ({Source.Member})");

                var wrapped = new AssertionException(sb.ToString().TrimEnd(), ex)
                {
                    Chain = chain,
                    Body = ex.Message,
                    AssertionSource = Source
                };

                var info = new AssertionInfo(Links, Source, null, null, BecauseMessage, AssertionStatus.Failed, default, wrapped);
                await NotifyAsync(info).ConfigureAwait(false);
                throw wrapped;
            }
        }

        public AssertionPipeline NewPipeline()
            => new(Settings, Source, SoftState, _isNoOp, _bypassExecutionWrappersAndHooks);

        internal AssertionPipeline NewPipeline(bool bypassExecutionWrappersAndHooks)
            => new(Settings, Source, SoftState, _isNoOp, bypassExecutionWrappersAndHooks);

        public bool GetIsSkipped() => IsSkipped;
        public AssertionSettings GetSettings() => Settings;
        public List<string> GetLinks() => Links;

        [DebuggerHidden, StackTraceHidden]
        public Task ExecuteAsync(CheckOperation op, IReadOnlyList<string> links)
        {
            if (_isNoOp) return Task.CompletedTask;
            if (IsSkipped)
            {
                var info = BuildInfo(op, links, AssertionStatus.Skipped, null, default);
                return NotifyAsync(info);
            }
            return IsSoft ? ExecuteSoftAsync(op, links) : CheckAsync(op, links);
        }

        private async Task ExecuteOrGroupAsync(List<CheckOperation> group, IReadOnlyList<string> links)
        {
            if (_isNoOp) return;
            if (IsSkipped)
            {
                foreach (var op in group)
                {
                    var info = BuildInfo(op, links, AssertionStatus.Skipped, null, default);
                    await NotifyAsync(info).ConfigureAwait(false);
                }
                return;
            }
            if (IsSoft)
                await ExecuteSoftOrGroupAsync(group, links).ConfigureAwait(false);
            else
                await CheckOrGroupAsync(group, links).ConfigureAwait(false);
        }

        private async Task CheckOrGroupAsync(List<CheckOperation> group, IReadOnlyList<string> links)
        {
            var sw = Stopwatch.StartNew();
            var failures = new List<string>();

            foreach (var op in group)
            {
                bool passes;
                try
                {
                    try
                    {
                        passes = op.IsAsync
                            ? await op.PassesAsync!().ConfigureAwait(false)
                            : op.PassesSync!();
                    }
                    catch (AssertionException)
                    {
                        sw.Stop();
                        throw;
                    }
                    catch (Exception originalEx)
                    {
                        sw.Stop();
                        await ThrowWrappedAsync(originalEx, op, links, ShouldShowDuration(op) ? sw.Elapsed : default).ConfigureAwait(false);
                        return;
                    }

                    if (passes)
                    {
                        sw.Stop();
                        var passInfo = BuildInfo(op, links, AssertionStatus.Passed, null,
                            ShouldShowDuration(op) ? sw.Elapsed : default);
                        await NotifyAsync(passInfo).ConfigureAwait(false);
                        return;
                    }
                    failures.Add(op.FailBecause());
                }
                catch (AssertionException)
                {
                    throw;
                }
            }

            sw.Stop();
            await ThrowOrGroupAsync(failures, links, ShouldShowDuration(group[0]) ? sw.Elapsed : default)
                .ConfigureAwait(false);
        }

        private async Task ExecuteSoftOrGroupAsync(List<CheckOperation> group, IReadOnlyList<string> links)
        {
            try { await CheckOrGroupAsync(group, links).ConfigureAwait(false); }
            catch (AssertionException ex) when (ShouldCatch(ex)) { SoftState!.Capture(ex); await NotifySoftFailAsync(group[0], links, ex).ConfigureAwait(false); }
            catch (Exception ex) when (ShouldCatch(ex)) { SoftState!.Capture(ex); await NotifySoftFailAsync(group[0], links, ex).ConfigureAwait(false); }
        }

        [DebuggerHidden, StackTraceHidden]
        public async Task CheckAsync(CheckOperation op, IReadOnlyList<string> links)
        {
            var sw = Stopwatch.StartNew();
            bool passes;
            try
            {
                try
                {
                    passes = op.IsAsync
                        ? await op.PassesAsync!().ConfigureAwait(false)
                        : op.PassesSync!();
                }
                catch (AssertionException)
                {
                    sw.Stop();
                    throw;
                }
                catch (Exception originalEx)
                {
                    sw.Stop();
                    await ThrowWrappedAsync(originalEx, op, links, ShouldShowDuration(op) ? sw.Elapsed : default).ConfigureAwait(false);
                    return;
                }
                sw.Stop();

                bool showDur = ShouldShowDuration(op);

                if (!passes)
                    await ThrowAsync(op, links, showDur ? sw.Elapsed : default).ConfigureAwait(false);
                else
                {
                    var info = BuildInfo(op, links, AssertionStatus.Passed, null, showDur ? sw.Elapsed : default);
                    await NotifyAsync(info).ConfigureAwait(false);
                }
            }
            catch (AssertionException)
            {
                throw;
            }
        }

        [DebuggerHidden, StackTraceHidden]
        public async Task ExecuteSoftAsync(CheckOperation op, IReadOnlyList<string> links)
        {
            try { await CheckAsync(op, links).ConfigureAwait(false); }
            catch (AssertionException ex) when (ShouldCatch(ex)) { SoftState!.Capture(ex); await NotifySoftFailAsync(op, links, ex).ConfigureAwait(false); }
            catch (Exception ex) when (ShouldCatch(ex)) { SoftState!.Capture(ex); await NotifySoftFailAsync(op, links, ex).ConfigureAwait(false); }
        }

        public bool ShouldCatch(Exception ex)
        {
            var s = Settings;
            if (s.CatchAll) return true;
            var types = s.CaughtExceptionTypes;
            if (types.Count == 0) return ex is AssertionException;
            foreach (var t in types) if (t.IsInstanceOfType(ex)) return true;
            return false;
        }

        [DebuggerHidden, StackTraceHidden]
        public async Task ThrowAsync(CheckOperation op, IReadOnlyList<string> links, TimeSpan duration)
        {
            var (chain, trunc) = ChainRenderer.Render(links, Settings.MaxChainLinkLength);
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(chain))
            {
                sb.AppendLine("Assertion failed:");
                sb.Append("  ");
                sb.AppendLine(chain);
                sb.AppendLine();
            }

            if (!string.IsNullOrEmpty(BecauseMessage)) sb.AppendLine($"Because  : {BecauseMessage}");
            sb.Append(op.FailBecause());

            var hints = BuildHints(op.HintsFactory?.Invoke());
            if (hints is not null) { sb.AppendLine(); sb.Append(hints); }

            AppendTruncations(sb, trunc);
            AppendDuration(sb, duration);
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine($"Source   : {Path.GetFileName(Source.File)}:{Source.Line} ({Source.Member})");

            var message = sb.ToString().TrimEnd();
            var innerExceptions = op.InnerExceptionsFactory?.Invoke();

            AssertionException ex = innerExceptions is { Count: > 0 }
                ? new AggregateAssertionException(innerExceptions, message)
                {
                    Chain = chain,
                    Body = op.FailBecause(),
                    AssertionSource = Source
                }
                : new AssertionException(message)
                {
                    Chain = chain,
                    Body = op.FailBecause(),
                    AssertionSource = Source
                };

            await NotifyAsync(BuildInfo(op, links, AssertionStatus.Failed, ex, duration)).ConfigureAwait(false);
            throw ex;
        }

        [DebuggerHidden, StackTraceHidden]
        private async Task ThrowWrappedAsync(Exception originalEx, CheckOperation op, IReadOnlyList<string> links, TimeSpan duration)
        {
            var (chain, trunc) = ChainRenderer.Render(links, Settings.MaxChainLinkLength);
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(chain))
            {
                sb.AppendLine("Assertion failed:");
                sb.Append("  ");
                sb.AppendLine(chain);
                sb.AppendLine();
            }

            if (!string.IsNullOrEmpty(BecauseMessage)) sb.AppendLine($"Because  : {BecauseMessage}");

            // Include original exception details
            sb.Append($"{originalEx.GetType().Name}: {originalEx.Message}");

            AppendTruncations(sb, trunc);
            AppendDuration(sb, duration);
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine($"Source   : {Path.GetFileName(Source.File)}:{Source.Line} ({Source.Member})");

            var message = sb.ToString().TrimEnd();

            // Wrap the original exception in AssertionException
            var ex = new AssertionException(message, originalEx)
            {
                Chain = chain,
                Body = originalEx.Message,
                AssertionSource = Source
            };

            await NotifyAsync(BuildInfo(op, links, AssertionStatus.Failed, ex, duration)).ConfigureAwait(false);
            throw ex;
        }

        [DebuggerHidden, StackTraceHidden]
        private async Task ThrowOrGroupAsync(List<string> failures, IReadOnlyList<string> links, TimeSpan duration)
        {
            var (chain, trunc) = ChainRenderer.Render(links, Settings.MaxChainLinkLength);
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(chain))
            {
                sb.AppendLine("Assertion failed:");
                sb.Append("  ");
                sb.AppendLine(chain);
                sb.AppendLine();
            }

            if (!string.IsNullOrEmpty(BecauseMessage)) sb.AppendLine($"Because  : {BecauseMessage}");

            for (int i = 0; i < failures.Count; i++)
            {
                if (i > 0) sb.AppendLine("  Or:");
                sb.Append(failures[i]);
                if (i < failures.Count - 1) sb.AppendLine();
            }

            AppendTruncations(sb, trunc);
            AppendDuration(sb, duration);

            var ex = new AssertionException(sb.ToString().TrimEnd());
            var info = new AssertionInfo(links, Source, null, null, BecauseMessage, AssertionStatus.Failed, duration, ex);
            await NotifyAsync(info).ConfigureAwait(false);
            throw ex;
        }

        public async Task NotifyAsync(AssertionInfo info)
        {
            if (_isNoOp || _bypassExecutionWrappersAndHooks) return;
            var loggers = Settings.OnAssertion;
            for (int i = 0; i < loggers.Count; i++)
                await SafeInvokeAsync(loggers[i], info).ConfigureAwait(false);
        }

        public async Task NotifySoftFailAsync(CheckOperation op, IReadOnlyList<string> links, Exception ex)
        {
            if (_bypassExecutionWrappersAndHooks) return;
            var handlers = Settings.OnSoftFailure;
            if (handlers.Count == 0) return;
            var info = BuildInfo(op, links, AssertionStatus.Failed, ex, default);
            for (int i = 0; i < handlers.Count; i++)
                await SafeInvokeAsync(handlers[i], info).ConfigureAwait(false);
        }

        private static async Task SafeInvokeAsync(Func<AssertionInfo, ValueTask> fn, AssertionInfo info)
        {
            try { await fn(info).ConfigureAwait(false); }
            catch (Exception ex) { Console.Error.WriteLine($"[CATCHY HOOK ERROR]: {ex.Message}"); }
        }

        private AssertionInfo BuildInfo(CheckOperation op, IReadOnlyList<string> links,
            AssertionStatus status, Exception? ex, TimeSpan dur)
            => new(links, Source, op?.ActualObject, null, BecauseMessage, status, dur, ex);

        private bool ShouldShowDuration(CheckOperation op)
            => Settings.ShowDuration == ShowDurationMode.Always
            || (Settings.ShowDuration == ShowDurationMode.AsyncOnly && op.IsAsync);

        private static void AppendTruncations(StringBuilder sb, IReadOnlyList<(string ph, string full)> trunc)
        {
            if (trunc.Count == 0) return;
            sb.AppendLine();
            sb.AppendLine(trunc.Count == 1 ? "One chain link was truncated:" : $"{trunc.Count} chain links were truncated:");
            foreach (var (ph, full) in trunc) sb.AppendLine($"  {ph} => \"{full}\"");
        }

        private static void AppendDuration(StringBuilder sb, TimeSpan duration)
        {
            if (duration == default) return;
            var durMs = duration.TotalMilliseconds;
            if (durMs > 0) { sb.AppendLine(); sb.AppendLine($"Duration : {durMs} ms"); }
        }

        private static string? BuildHints((string label, object? value, string? expr)[]? hints)
        {
            if (hints is null || hints.Length == 0) return null;
            var lines = new List<string>();
            foreach (var h in hints)
            {
                var v = ValueFormatter.Format(h.value);
                bool vL = v.Length > 50;
                bool eL = h.expr is { Length: > 20 } && !ExprFormat.IsLiteral(h.expr);
                if (!vL && !eL) continue;
                var vT = v.Length <= 200 ? v : v.Substring(0, 200) + "…";
                lines.Add(eL && !vL ? $"  ↳ {h.label}: {h.expr!}" : $"  ↳ {h.label}: {vT}{(eL ? $" [{h.expr}]" : "")}");
            }
            return lines.Count > 0 ? string.Join("\n", lines) : null;
        }
    }
}

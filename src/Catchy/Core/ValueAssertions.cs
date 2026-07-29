using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Catchy.Sdk;
using Catchy.Sdk.Checks.Actions;

namespace Catchy
{
    public class ValueAssertions : IAssertions
    {
        internal readonly AssertionPipeline _pipeline;

        protected ValueAssertions(AssertionPipeline pipeline) { _pipeline = pipeline; }

        AssertionPipeline IAssertions.GetPipeline() => _pipeline;
        bool IAssertions.IsSkipped() => _pipeline.IsSkipped;
        void IAssertions.AddOp(CheckOperation op) => _pipeline.AddOp(op);
        void IAssertions.AddLink(string link) { if (!string.IsNullOrEmpty(link)) _pipeline.Links.Add(link); }
        void IAssertions.AddLinks(string?[] links) { foreach (var p in links) if (!string.IsNullOrEmpty(p)) _pipeline.Links.Add(p!); }
        void IAssertions.Skip(string? reason) { _pipeline.IsSkipped = true; if (!string.IsNullOrEmpty(reason)) _pipeline.BecauseMessage += reason; }
        [DebuggerHidden, StackTraceHidden, EditorBrowsable(EditorBrowsableState.Never)]
        public TaskAwaiter GetAwaiter()
        {
            if (TaskExecutionOptionsSlots.TryGet(_pipeline, out var opts))
            {
                return ExecutionModes.RunWithMode(() => _pipeline.RunAsync(), opts, _pipeline.Settings).GetAwaiter();
            }

            return _pipeline.RunAsync().GetAwaiter();
        }
    }

    public class ValueAssertions<TValue> : ValueAssertions
    {
        internal readonly TValue _value;
        internal readonly Func<Task<TValue?>>? _asyncProvider;

        internal virtual TValue GetValue() => _value;

        public ValueAssertions(AssertionPipeline pipeline)
            : base(pipeline)
        {
            _value = default!;
            _asyncProvider = null;
        }

        public ValueAssertions(AssertionPipeline pipeline, TValue value)
            : base(pipeline)
        {
            _value = value;
            _asyncProvider = null;
        }

        public ValueAssertions(AssertionPipeline pipeline, Func<Task<TValue?>> asyncProvider)
            : base(pipeline)
        {
            _value = default!;
            _asyncProvider = asyncProvider;
        }

        public ValueAssertions<TValue> And()
        {
            this.Link("And");
            return this;
        }

        public ValueAssertions<TValue> But()
        {
            this.Link("But");
            return this;
        }

        public ValueAssertions<TValue> Then()
        {
            this.Link("Then");
            return this;
        }

        public ValueAssertions<TValue> When(bool condition,
            [CallerArgumentExpression(nameof(condition))] string? conditionExpr = null)
        {
            this.Link("When", conditionExpr);
            if (!condition) _pipeline.IsSkipped = true;
            return this;
        }

        public ValueAssertions<TValue> WhenNot(bool condition,
            [CallerArgumentExpression(nameof(condition))] string? conditionExpr = null)
        {
            this.Link("WhenNot", conditionExpr);
            if (condition) _pipeline.IsSkipped = true;
            return this;
        }

        public ValueAssertions<TValue> With(Func<AssertionInfo, ValueTask> callback,
            [CallerArgumentExpression(nameof(callback))] string? callbackExpr = null)
        {
            this.Link("With", callbackExpr);
            _pipeline.Settings = _pipeline.Settings.Clone(s => s.OnAssertion = [.. s.OnAssertion, callback]);
            return this;
        }

        public ValueAssertions<TValue> With(Action<AssertionInfo> callback,
            [CallerArgumentExpression(nameof(callback))] string? callbackExpr = null)
            => With(info => { callback(info);
#if !NETSTANDARD2_1_OR_GREATER && !NET5_0_OR_GREATER
                return default;
#else
                return ValueTask.CompletedTask;
#endif
            }, callbackExpr);

        public ValueAssertions<TValue> Because(string reason, __._ _ = default,
            [CallerArgumentExpression(nameof(reason))] string? reasonExpr = null)
        {
            _pipeline.BecauseMessage = reason;
            this.Link("Because", reasonExpr);
            return this;
        }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public ValueAssertions<TValue> Is(TValue? expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        {
            this.Link("Is", expr);

            if (typeof(TValue) == typeof(string))
            {
                this.Op(a =>
                {
                    var actualString = a.GetValue() is string s ? s : null;
                    var expectedString = expected is string es ? es : null;
                    return StringChecks.EqualTo(
                        actualString,
                        expectedString,
                        () => a.GetPipeline().Settings.DefaultStringComparison,
                        a.IsSkipped(),
                        expr);
                });
                return this;
            }

            this.Op(a => CheckOperation.Sync(
                () => EqualityComparer<TValue>.Default.Equals(a.GetValue()!, expected!),
                () => $"Expected {expr ?? "value"} to be {expected}, but was {a.GetValue()}",
                a.IsSkipped()));
            return this;
        }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public ValueAssertions<TValue> IsNot(TValue? unexpected,
            [CallerArgumentExpression(nameof(unexpected))] string? expr = null)
        {
            this.Link("IsNot", expr);

            if (typeof(TValue) == typeof(string))
            {
                this.Op(a =>
                {
                    var actualString = a.GetValue() is string s ? s : null;
                    var unexpectedString = unexpected is string us ? us : null;
                    return StringChecks.NotEqualTo(
                        actualString,
                        unexpectedString,
                        a.GetPipeline().Settings.DefaultStringComparison,
                        a.IsSkipped(),
                        expr);
                });
                return this;
            }

            this.Op(a => CheckOperation.Sync(
                () => !EqualityComparer<TValue>.Default.Equals(a.GetValue()!, unexpected!),
                () => $"Expected value not to be {unexpected}, but it was",
                a.IsSkipped()));
            return this;
        }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public ValueAssertions<TValue> Satisfies(Func<TValue?, bool> predicate,
            [CallerArgumentExpression(nameof(predicate))] string? predicateExpr = null)
        {
            this.Link("Satisfies", predicateExpr);
            this.Op(a => ObjectChecks.Satisfies(a.GetValue(), predicate, predicateExpr, a.IsSkipped()));
            return this;
        }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public ValueAssertions<TValue> Satisfies(Func<TValue?, bool> predicate, string? predicateDesc,
            [CallerArgumentExpression(nameof(predicate))] string? predicateExpr = null)
        {
            this.Link("Satisfies", predicateExpr, ", ", predicateDesc);
            this.Op(a => ObjectChecks.Satisfies(a.GetValue(), predicate, predicateDesc ?? predicateExpr, a.IsSkipped()));
            return this;
        }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public ValueAssertions<TValue> DoesNotSatisfy(Func<TValue?, bool> predicate,
            [CallerArgumentExpression(nameof(predicate))] string? predicateExpr = null)
        {
            this.Link("DoesNotSatisfy", predicateExpr);
            this.Op(a => ObjectChecks.DoesNotSatisfy(a.GetValue(), predicate, predicateExpr, a.IsSkipped()));
            return this;
        }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public ValueAssertions<TValue> IsOneOf(IEnumerable<TValue> values,
            [CallerArgumentExpression(nameof(values))] string? expr = null)
        {
            var list = values is IReadOnlyList<TValue> r ? r : new List<TValue>(values);
            this.Link("IsOneOf", expr);
            this.Op(a => ObjectChecks.IsOneOf(a.GetValue(), list, a.IsSkipped()));
            return this;
        }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public ValueAssertions<TValue> IsNotOneOf(IEnumerable<TValue> values,
            [CallerArgumentExpression(nameof(values))] string? expr = null)
        {
            var list = values is IReadOnlyList<TValue> r ? r : new List<TValue>(values);
            this.Link("IsNotOneOf", expr);
            this.Op(a => ObjectChecks.IsNotOneOf(a.GetValue(), list, a.IsSkipped()));
            return this;
        }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public ValueAssertions<TValue> IsDefault()
        {
            this.Link("IsDefault");
            this.Op(a => ObjectChecks.IsDefault(a.GetValue(), a.IsSkipped()));
            return this;
        }
    }

    public class ValueAssertions<TSelf, TValue> : ValueAssertions<TValue>
        where TSelf : ValueAssertions<TSelf, TValue>
    {
        protected ValueAssertions(AssertionPipeline pipeline, TValue value)
            : base(pipeline, value)
        {
        }

        protected ValueAssertions(AssertionPipeline pipeline, Func<Task<TValue?>> asyncProvider)
            : base(pipeline, asyncProvider)
        {
        }

        public new TSelf And()
        {
            this.Link("And");
            return (TSelf)this;
        }

        public new TSelf But()
        {
            this.Link("But");
            return (TSelf)this;
        }

        public new TSelf Then()
        {
            this.Link("Then");
            return (TSelf)this;
        }

        public new TSelf When(bool condition,
            [CallerArgumentExpression(nameof(condition))] string? conditionExpr = null)
        {
            base.When(condition, conditionExpr);
            return (TSelf)this;
        }

        public new TSelf WhenNot(bool condition,
            [CallerArgumentExpression(nameof(condition))] string? conditionExpr = null)
        {
            base.WhenNot(condition, conditionExpr);
            return (TSelf)this;
        }

        public new TSelf With(Func<AssertionInfo, ValueTask> callback,
            [CallerArgumentExpression(nameof(callback))] string? callbackExpr = null)
        {
            base.With(callback, callbackExpr);
            return (TSelf)this;
        }

        public new TSelf With(Action<AssertionInfo> callback,
            [CallerArgumentExpression(nameof(callback))] string? callbackExpr = null)
        {
            base.With(callback, callbackExpr);
            return (TSelf)this;
        }

        public new TSelf Because(string reason, __._ _ = default,
            [CallerArgumentExpression(nameof(reason))] string? reasonExpr = null)
        {
            base.Because(reason, _, reasonExpr);
            return (TSelf)this;
        }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public new TSelf Is(TValue? expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { base.Is(expected, expr); return (TSelf)this; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public new TSelf IsNot(TValue? unexpected,
            [CallerArgumentExpression(nameof(unexpected))] string? expr = null)
        { base.IsNot(unexpected, expr); return (TSelf)this; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public new TSelf Satisfies(Func<TValue?, bool> predicate,
            [CallerArgumentExpression(nameof(predicate))] string? predicateExpr = null)
        { base.Satisfies(predicate, predicateExpr); return (TSelf)this; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public new TSelf Satisfies(Func<TValue?, bool> predicate, string? predicateDesc,
            [CallerArgumentExpression(nameof(predicate))] string? predicateExpr = null)
        { base.Satisfies(predicate, predicateDesc, predicateExpr); return (TSelf)this; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public new TSelf DoesNotSatisfy(Func<TValue?, bool> predicate,
            [CallerArgumentExpression(nameof(predicate))] string? predicateExpr = null)
        { base.DoesNotSatisfy(predicate, predicateExpr); return (TSelf)this; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public new TSelf IsOneOf(IEnumerable<TValue> values,
            [CallerArgumentExpression(nameof(values))] string? expr = null)
        { base.IsOneOf(values, expr); return (TSelf)this; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public new TSelf IsNotOneOf(IEnumerable<TValue> values,
            [CallerArgumentExpression(nameof(values))] string? expr = null)
        { base.IsNotOneOf(values, expr); return (TSelf)this; }

        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public new TSelf IsDefault()
        { base.IsDefault(); return (TSelf)this; }
    }

    public static partial class AssertionsExtensions
    {
        /// <summary>Registers a deep-equality <paramref name="rule"/> for subsequent equivalence checks in this chain.</summary>
        public static TSelf With<TSelf, TSource, TTarget>(
            this TSelf a, DeepEqualRule<TSource, TTarget> rule,
            [CallerArgumentExpression(nameof(rule))] string? ruleExpr = null)
            where TSelf : ValueAssertions
        {
            a.GetPipeline().GetDeepEqualRuleContainer().RegisterRule(rule);
            a.Link("With", ruleExpr);
            return a;
        }

        /// <summary>Builds and registers a deep-equality rule using <paramref name="configure"/> for this chain.</summary>
        public static TSelf With<TSelf, TSource, TTarget>(
            this TSelf a, Func<DeepEqualRule<TSource, TTarget>, DeepEqualRule<TSource, TTarget>>? configure,
            [CallerArgumentExpression(nameof(configure))] string? configureExpr = null)
            where TSelf : ValueAssertions
        {
            var rule = new DeepEqualRule<TSource, TTarget>();
            if (configure != null)
                rule = configure(rule);
            a.GetPipeline().GetDeepEqualRuleContainer().RegisterRule(rule);
            a.Link("With", configureExpr);
            return a;
        }

        /// <summary>Creates a deep-equality rule, applies <paramref name="configure"/>, and registers it for this chain.</summary>
        public static TSelf With<TSelf, TSource, TTarget>(
            this TSelf a, Action<DeepEqualRule<TSource, TTarget>>? configure,
            [CallerArgumentExpression(nameof(configure))] string? configureExpr = null)
            where TSelf : ValueAssertions
        {
            var rule = new DeepEqualRule<TSource, TTarget>();
            configure?.Invoke(rule);
            a.GetPipeline().GetDeepEqualRuleContainer().RegisterRule(rule);
            a.Link("With", configureExpr);
            return a;
        }

        // EqualsOptions extensions
        public static TSelf With<TSelf>(
            this TSelf a, Action<EqualsOptions>? configure,
            [CallerArgumentExpression(nameof(configure))] string? configureExpr = null)
            where TSelf : ValueAssertions
        {
            var opts = new EqualsOptions();
            configure?.Invoke(opts);
            a.GetPipeline().Settings = a.GetPipeline().Settings.Clone(s => s.EqualsOptions = opts);
            a.Link("With", configureExpr);
            return a;
        }

        // General SoftAsserter/SoftState extensions
        public static TSelf With<TSelf>(
            this TSelf a, SoftAsserter softAsserter,
            [CallerArgumentExpression(nameof(softAsserter))] string? softExpr = null)
            where TSelf : ValueAssertions
        {
            a.Link("With", softExpr);
            ((IAssertions)a).GetPipeline().SoftState = softAsserter._softState;
            ((IAssertions)a).GetPipeline().Settings = softAsserter._settings;
            return a;
        }

        public static TSelf With<TSelf>(
            this TSelf a, SoftState softState,
            [CallerArgumentExpression(nameof(softState))] string? softExpr = null)
            where TSelf : ValueAssertions
        {
            a.Link("With", softExpr);
            ((IAssertions)a).GetPipeline().SoftState = softState;
            return a;
        }

    }

    namespace Sdk
    {
        public static partial class StringComparisonExtensions
        {
            public static StringComparison AddIgnoreCase(this StringComparison c)
            {
                int v = (int)c;
                return (v & 1) == 0 ? (StringComparison)(v + 1) : c;
            }

            public static StringComparison RemoveIgnoreCase(this StringComparison c)
            {
                int v = (int)c;
                return (v & 1) == 1 ? (StringComparison)(v - 1) : c;
            }
        }
    }
}

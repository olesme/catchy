using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    /// <summary>
    /// Assertion builder for state change tracking with fluent From/To modifiers.
    /// Inherits all trailing modifiers from ValueAssertions.
    /// </summary>
    public sealed class StateChangeBuilder<TState> : ValueAssertions<StateChangeBuilder<TState>>
    {
        private readonly ValueAssertions<Func<Task>> _parent;
        private readonly Func<TState> _selector;
        private readonly string? _selectorExpr;

        private TState? _from;
        private TState? _to;
        private bool _hasFrom;
        private bool _hasTo;
        private string? _fromExpr;
        private string? _toExpr;

        private bool _finalized;

        internal StateChangeBuilder(ValueAssertions<Func<Task>> parent, Func<TState> selector, string? selectorExpr)
            : base(parent.GetPipeline())
        {
            _parent = parent;
            _selector = selector;
            _selectorExpr = selectorExpr;
        }

        [DebuggerHidden, StackTraceHidden]
        public StateChangeBuilder<TState> From(TState value,
            [CallerArgumentExpression(nameof(value))] string? expr = null)
        {
            this.Link("From", expr);
            _from = value;
            _hasFrom = true;
            _fromExpr = expr;
            return this;
        }

        [DebuggerHidden, StackTraceHidden]
        public StateChangeBuilder<TState> To(TState value,
            [CallerArgumentExpression(nameof(value))] string? expr = null)
        {
            this.Link("To", expr);
            _to = value;
            _hasTo = true;
            _toExpr = expr;
            return this;
        }

        public new ValueAssertions<Func<Task>> And()
        {
            Complete();
            this.Link("And");
            return _parent;
        }

        public new ValueAssertions<Func<Task>> But()
        {
            Complete();
            this.Link("But");
            return _parent;
        }

        public new ValueAssertions<Func<Task>> Then()
        {
            Complete();
            this.Link("Then");
            return _parent;
        }

        private void Complete()
        {
            if (_finalized) return;
            _finalized = true;

            this.Op(a => StateChangeChecks.Changes(
                _parent.GetFunc(), _selector,
                _from, _to, _hasFrom, _hasTo,
                _selectorExpr, _fromExpr, _toExpr,
                _pipeline.Slots, this.IsSkipped()));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new TaskAwaiter GetAwaiter()
        {
            Complete();
            return _parent.RunWithModeAsync().GetAwaiter();
        }
    }

    public static partial class StateChangeAssertions
    {
        /// <summary>
        /// Asserts that the selector's value changes after func execution.
        /// Chain <c>.From(x)</c> and/or <c>.To(y)</c> for specific values.
        /// </summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StateChangeBuilder<TState> Changes<TState>(this ValueAssertions<Func<Task>> a, Func<TState> selector,
            [CallerArgumentExpression(nameof(selector))] string? expr = null)
        {
            a.Link("Changes", expr);
            return new StateChangeBuilder<TState>(a, selector, expr);
        }

        /// <summary>
        /// Asserts that the selector's value does NOT change after func execution.
        /// </summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Func<Task>> DoesNotChange<TState>(this ValueAssertions<Func<Task>> a, Func<TState> selector,
            [CallerArgumentExpression(nameof(selector))] string? expr = null)
        {
            a.Link("DoesNotChange", expr);
            a.Op(a => StateChangeChecks.DoesNotChange(a.GetFunc(), selector, expr,
                a.GetPipeline().Slots, a.IsSkipped()));
            return a;
        }

        /// <summary>
        /// Asserts that a numeric selector changes by exactly the specified delta.
        /// </summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Func<Task>> ChangesBy<TState>(this ValueAssertions<Func<Task>> a, Func<TState> selector, TState delta,
            [CallerArgumentExpression(nameof(selector))] string? selExpr = null,
            [CallerArgumentExpression(nameof(delta))] string? deltaExpr = null)
            where TState : struct, IComparable<TState>
        {
            a.Link("ChangesBy", selExpr, deltaExpr);
            a.Op(a => StateChangeChecks.ChangesBy(a.GetFunc(), selector, delta, selExpr, deltaExpr,
                a.GetPipeline().Slots, a.IsSkipped()));
            return a;
        }

        /// <summary>
        /// Asserts that a numeric selector increases (after > before).
        /// </summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Func<Task>> Increments<TState>(this ValueAssertions<Func<Task>> a, Func<TState> selector,
            [CallerArgumentExpression(nameof(selector))] string? expr = null)
            where TState : IComparable<TState>
        {
            a.Link("Increments", expr);
            a.Op(a => StateChangeChecks.Increments(a.GetFunc(), selector, expr,
                a.GetPipeline().Slots, a.IsSkipped()));
            return a;
        }

        /// <summary>
        /// Asserts that a numeric selector decreases (after < before).
        /// </summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Func<Task>> Decrements<TState>(this ValueAssertions<Func<Task>> a, Func<TState> selector,
            [CallerArgumentExpression(nameof(selector))] string? expr = null)
            where TState : IComparable<TState>
        {
            a.Link("Decrements", expr);
            a.Op(a => StateChangeChecks.Decrements(a.GetFunc(), selector, expr,
                a.GetPipeline().Slots, a.IsSkipped()));
            return a;
        }
    }
}


using System.Diagnostics;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    public static class StructuralDeepEqualConfigurationExtensions
    {
        /// <summary>Registers a deep-equality <paramref name="rule"/> for subsequent equivalence checks in this chain.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<T> With<T, TSource, TTarget>(this StructuralAssertions<T> a, DeepEqualRule<TSource, TTarget> rule,
            [CallerArgumentExpression(nameof(rule))] string? ruleExpr = null)
        {
            a.GetPipeline().GetDeepEqualRuleContainer().RegisterRule(rule);
            a.Link("With", ruleExpr);
            return a;
        }

        /// <summary>Builds and registers a deep-equality rule using <paramref name="configure"/> for this chain.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<T> With<T, TSource, TTarget>(this StructuralAssertions<T> a,
            Func<DeepEqualRule<TSource, TTarget>, DeepEqualRule<TSource, TTarget>>? configure,
            [CallerArgumentExpression(nameof(configure))] string? configureExpr = null)
        {
            var rule = new DeepEqualRule<TSource, TTarget>();
            if (configure != null)
                rule = configure(rule);
            a.GetPipeline().GetDeepEqualRuleContainer().RegisterRule(rule);
            a.Link("With", configureExpr);
            return a;
        }

        /// <summary>Creates a deep-equality rule, applies <paramref name="configure"/>, and registers it for this chain.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static StructuralAssertions<T> With<T, TSource, TTarget>(this StructuralAssertions<T> a,
            Action<DeepEqualRule<TSource, TTarget>>? configure,
            [CallerArgumentExpression(nameof(configure))] string? configureExpr = null)
        {
            var rule = new DeepEqualRule<TSource, TTarget>();
            configure?.Invoke(rule);
            a.GetPipeline().GetDeepEqualRuleContainer().RegisterRule(rule);
            a.Link("With", configureExpr);
            return a;
        }
    }
}

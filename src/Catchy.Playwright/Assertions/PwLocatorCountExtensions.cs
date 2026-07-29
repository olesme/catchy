using System.Diagnostics;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    public static class PwLocatorCountExtensions
    {
        /// <summary>Asserts that the count is greater than <paramref name="count"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<int> GreaterThan(this ValueAssertions<int> assertions, int count,
            [CallerArgumentExpression(nameof(count))] string? expr = null)
        {
            assertions.Link("GreaterThan", expr);
            assertions.Op(a => CheckOperation.Sync(
                () => assertions.GetValue() > count,
                () => $"Expected {assertions.GetValue()} to be greater than {count}",
                assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that the count is greater than or equal to <paramref name="count"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<int> GreaterThanOrEqual(this ValueAssertions<int> assertions, int count,
            [CallerArgumentExpression(nameof(count))] string? expr = null)
        {
            assertions.Link("GreaterThanOrEqual", expr);
            assertions.Op(a => CheckOperation.Sync(
                () => assertions.GetValue() >= count,
                () => $"Expected {assertions.GetValue()} to be greater than or equal to {count}",
                assertions.IsSkipped()));
            return assertions;
        }

        /// <summary>Asserts that the count is less than <paramref name="count"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<int> LessThan(this ValueAssertions<int> assertions, int count,
            [CallerArgumentExpression(nameof(count))] string? expr = null)
        {
            assertions.Link("LessThan", expr);
            assertions.Op(a => CheckOperation.Sync(
                () => assertions.GetValue() < count,
                () => $"Expected {assertions.GetValue()} to be less than {count}",
                assertions.IsSkipped()));
            return assertions;
        }
    }
}

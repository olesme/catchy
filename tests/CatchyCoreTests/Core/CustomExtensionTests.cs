using Catchy;


namespace CatchyCoreTests.Core
{
    using CatchyTestHelpers;
    using CustomExtensions;

    public class CustomExtensionTests
    {
        [Fact]
        public async Task Custom_IsFreezing_passes_below_zero()
            => await Stateless.Assert.ThatTemperature(-5).IsFreezing();

        [Fact]
        public async Task Custom_IsFreezing_passes_at_zero()
            => await Stateless.Assert.ThatTemperature(0).IsFreezing();

        [Fact]
        public async Task Custom_IsBoiling_passes()
            => await Stateless.Assert.ThatTemperature(100).IsBoiling();

        [Fact]
        public async Task Custom_IsInComfortZone_passes()
            => await Stateless.Assert.ThatTemperature(22).IsInComfortZone();

        [Fact]
        public async Task Custom_chain_passes()
            => await Stateless.Assert.ThatTemperature(-10).IsFreezing().And().IsFreezing();

        [Fact]
        public async Task Custom_IsFreezing_fails_on_warm()
        {
            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
            await Stateless.Assert.ThatTemperature(20).IsFreezing());
            Assert.Contains("20", message);
            Assert.Contains("0°C", message);
        }

        [Fact]
        public async Task Custom_IsBoiling_fails_on_cold()
        {
            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
            await Stateless.Assert.ThatTemperature(50).IsBoiling());
            Assert.Contains("50", message);
            Assert.Contains("100°C", message);
        }

        [Fact]
        public async Task Custom_error_contains_chain_link()
        {
            var message = await TestHelpers.ShouldFailWithMessageAsync(async () => await Stateless.Assert.ThatTemperature(50).IsBoiling());
            Assert.Contains("IsBoiling", message);
            Assert.Contains("ThatTemperature", message);
        }

        [Fact]
        public async Task Custom_Because_works()
        {
            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Stateless.Assert.ThatTemperature(50).IsBoiling().Because("boiling point required"));
            Assert.Contains("boiling point required", message);
        }

        [Fact]
        public async Task Custom_With_SoftState_collects_failure()
        {
            var state = new SoftState();
            await Stateless.Assert.ThatTemperature(50).IsBoiling().With(state);
            Assert.True(state.HasFailures);
        }
    }
}

namespace CustomExtensions
{
    using System.Runtime.CompilerServices;
    using Catchy.Sdk;

    public sealed class TemperatureAssertions : ValueAssertions<TemperatureAssertions, double>
    {
        internal TemperatureAssertions(double value, AssertionPipeline pipeline) : base(pipeline, value) { }
    }

    public static partial class AsserterExtensions
    {
        public static TemperatureAssertions ThatTemperature(this Asserter a, double celsius,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(celsius))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(
                asserterExpr: aExpr,
                methodName: "ThatTemperature",
                valueExpr: vExpr,
                file: file, line: line, member: member);
            return new TemperatureAssertions(celsius, p);
        }
    }

    public static class TemperatureAssertionsExtensions
    {
        [System.Diagnostics.DebuggerHidden]
        public static TemperatureAssertions IsFreezing(this TemperatureAssertions a)
        {
            a.Link("IsFreezing");
            a.Op(CheckOperation.Sync(
                () => a.GetValue() <= 0,
                () => $"Expected temperature to be at or below 0°C, but was {a.GetValue()}°C",
                a.IsSkipped()));
            return a;
        }

        [System.Diagnostics.DebuggerHidden]
        public static TemperatureAssertions IsBoiling(this TemperatureAssertions a)
        {
            a.Link("IsBoiling");
            a.Op(CheckOperation.Sync(
                () => a.GetValue() >= 100,
                () => $"Expected temperature to be at or above 100°C, but was {a.GetValue()}°C",
                a.IsSkipped()));
            return a;
        }

        [System.Diagnostics.DebuggerHidden]
        public static TemperatureAssertions IsInComfortZone(this TemperatureAssertions a)
        {
            a.Link("IsInComfortZone");
            a.Op(CheckOperation.Sync(
                () => a.GetValue() is >= 18 and <= 26,
                () => $"Expected temperature between 18–26°C, but was {a.GetValue()}°C",
                a.IsSkipped()));
            return a;
        }
    }
}


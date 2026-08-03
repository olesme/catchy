using Catchy;
using Catchy.Configuration;
using Catchy.Tracing;
using CatchyPlaywrightTests.Support;

namespace CatchyPlaywrightTests.Tracing
{
    // Wires OnAssertion to the Playwright trace via IBrowserContext.TraceSuccess/TraceError.
    //
    // Why not Stateless.Assert: global singleton, no per-test identity, can't carry a context ref.
    //
    // Why not a global [BeforeEvery(Test)] hook: has no access to the IBrowserContext that lives
    // in the test class instance — parallel tests would race on a shared reference.
    //
    // This pattern: [Before(Test)] in the class captures Context via closure.
    //   • PlaywrightTestFixture.SetUpTestAsync() creates Context before this runs (base [Before]).
    //   • CatchyTUnitHooks.Before() materialises the ambient asserter first ([BeforeEvery, Order=MinValue]).
    //   • Each parallel test instance has its own Context and its own ThreadLocal asserter slot.
    //
    // Explicit-factory alternative (no ambient, no TUnit name conflict):
    //   _assert = Asserter.NewStateful(s => s.OnAssertion.Add(async info => { ... ctx ... }));
    //   await _assert.That(...).IsVisible();
    public sealed class PwTracingIntegrationTests : PlaywrightTestFixture
    {
        [Before(Test)]
        public async Task StartTracingAndWireHooks()
        {
            await Context!.Tracing.StartAsync(new() { Screenshots = true, Snapshots = true });

            // Capture this test's IBrowserContext in the closure.
            // TracingExtensions (Catchy.Tracing) call IBrowserContext.Tracing.GroupAsync internally.
            var ctx = Context!;
            Ambient.Assert.Settings().OnAssertion.Add(async info =>
            {
                var label = string.Join(" → ", info.Links);
                await (info.Status switch
                {
                    AssertionStatus.Passed  => ctx.TraceSuccess(label),
                    AssertionStatus.Failed  => ctx.TraceError($"{label}: {info.Exception?.Message}"),
                    AssertionStatus.Skipped => ctx.TraceInfo($"{label} (skipped)"),
                    _                       => Task.CompletedTask
                });
            });
        }

        [After(Test)]
        public async Task StopTracing()
        {
            await (Context?.Tracing.StopAsync(new()) ?? Task.CompletedTask);
        }

        [Test]
        public async Task Assertion_outcomes_appear_in_trace()
        {
            var page = EnsurePage();

            await Ambient.Assert.That(page.Locator("#todo-input")).IsVisible();
            await Ambient.Assert.That(page.Locator("#todo-input")).IsEnabled();
        }

        [Test]
        public async Task Soft_failures_are_traced_at_accumulation_not_on_flush()
        {
            var page = EnsurePage();

            // OnAssertion fires per-assertion before any flush — the browser is still
            // in the failure state when TraceError is called, so a screenshot hook
            // on OnSoftFailure would capture exactly the right moment.
            await Ambient.Assert.Soft.That(page.Locator("#todo-input")).IsChecked(); // fails → TraceError
            await Ambient.Assert.Soft.That(page.Locator("#todo-input")).IsVisible(); // passes → TraceSuccess

            await Stateless.Assert.That(Ambient.Assert.Soft.ErrorCount).Is(1);
            Ambient.Assert.Soft.Clear();
        }
    }
}

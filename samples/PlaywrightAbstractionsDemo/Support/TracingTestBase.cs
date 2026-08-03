using Catchy;
using Catchy.Configuration;
using Catchy.Tracing;
using Microsoft.Playwright;

namespace PlaywrightAbstractionsDemo.Support;

// Base class for tests that want each assertion result visible in the Playwright trace.
//
// Setup order per test (TUnit runs [BeforeEvery] before [Before]):
//   1. CatchyTUnitHooks.[BeforeEvery(Test), Order=MinValue]  — creates the per-test ambient asserter
//   2. TracingTestBase.[Before(Test)]                        — opens context, starts tracing,
//                                                              wires OnAssertion → IBrowserContext
//
// Why Ambient.Assert and not Stateless.Assert:
//   Stateless.Assert is a shared singleton; its settings are global and permanent.
//   Ambient.Assert (via Catchy.TUnit) is scoped to the current test and safe for parallel runs.
//
// Why capture IBrowserContext in the closure:
//   Each test has its own Context instance — closing over `ctx` ensures parallel tests
//   never write to each other's trace.
public abstract class TracingTestBase
{
    private string _traceId = "trace";
    protected IBrowserContext? Context { get; private set; }
    protected IPage? Page { get; private set; }

    [Before(Test)]
    public async Task SetUpAsync()
    {
        _traceId = Guid.NewGuid().ToString("N")[..8];

        Context = await BrowserSession.Browser.NewContextAsync();
        Page = await Context.NewPageAsync();
        await Page.SetContentAsync(DemoPageHtml);

        await Context.Tracing.StartAsync(new() { Screenshots = true, Snapshots = true });

        var ctx = Context;
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
    public async Task TearDownAsync()
    {
        Directory.CreateDirectory("traces");
        await (Context?.Tracing.StopAsync(new() { Path = $"traces/{_traceId}.zip" }) ?? Task.CompletedTask);

        if (Page is not null) { try { await Page.CloseAsync(); } catch { } }
        if (Context is not null) { try { await Context.CloseAsync(); } catch { } }
        Page = null;
        Context = null;
    }

    protected IPage EnsurePage() =>
        Page ?? throw new InvalidOperationException("Page not initialized.");

    private const string DemoPageHtml = """
        <!DOCTYPE html>
        <html>
        <head><title>Demo</title></head>
        <body>
          <h1>Catchy Demo</h1>
          <input id="search" placeholder="Search..." />
          <button id="submit">Submit</button>
          <div id="result" style="display:none">Result</div>
        </body>
        </html>
        """;
}

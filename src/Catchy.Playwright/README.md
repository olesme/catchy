# Catchy.Playwright

[![NuGet](https://img.shields.io/nuget/v/Catchy.Playwright.svg)](https://www.nuget.org/packages/Catchy.Playwright)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/olesme/catchy/blob/main/LICENSE)

[Microsoft.Playwright](https://playwright.dev/dotnet/) integration for Catchy — fluent browser and page assertions with async hooks for screenshots and traces on failure.

```sh
dotnet add package Catchy
dotnet add package Catchy.Playwright
```

## Usage

```csharp
using Catchy;
using Catchy.Playwright;
using static Catchy.Ambient;

public class LoginTests : CatchyTestBase  // or your framework base
{
    [Fact]
    public async Task Login_page_loads()
    {
        await using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var page = await browser.NewPageAsync();
        await page.GotoAsync("https://example.com/login");

        await Assert.That(page).HasTitle("Login");
        await Assert.Soft.That(page).HasUrl("*/login");
        await Assert.Soft.That(page.Locator("h1")).HasText("Sign in");
    }
}
```

## Assertion output in the Playwright trace

`Catchy.Tracing.TracingExtensions` adds `TraceSuccess`, `TraceError`, `TraceInfo`, and `TraceWarning`
to `IBrowserContext`. Wire them to `OnAssertion` so every assertion result appears as a labelled group
in the Playwright Trace Viewer:

```csharp
using Catchy;
using Catchy.Tracing;

// In your [Before(Test)] / SetUp — after IBrowserContext is created:
await context.Tracing.StartAsync(new() { Screenshots = true, Snapshots = true });

var ctx = context; // capture in closure; each test has its own IBrowserContext
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

// In your [After(Test)] / TearDown:
await context.Tracing.StopAsync(new() { Path = "trace.zip" });
```

`OnAssertion` fires per-assertion at accumulation time (before any soft-failure flush), so a
screenshot hook on `OnSoftFailure` captures the browser state at exactly the right moment.

> **Why not a global hook?** A global `[BeforeEvery(Test)]` has no access to the `IBrowserContext`
> living in the test class instance — parallel tests would race on a shared reference.
> Capture `context` via closure inside the test's own `[Before(Test)]` method.

If you prefer an explicit asserter instead of the ambient one:

```csharp
var assert = Asserter.NewStateful(cfg =>
    cfg.OnAssertion.Add(async info =>
    {
        var label = string.Join(" → ", info.Links);
        await (info.Status switch
        {
            AssertionStatus.Passed  => ctx.TraceSuccess(label),
            AssertionStatus.Failed  => ctx.TraceError($"{label}: {info.Exception?.Message}"),
            AssertionStatus.Skipped => ctx.TraceInfo($"{label} (skipped)"),
            _                       => Task.CompletedTask
        });
    }));

await assert.That(page.Locator("h1")).HasText("Dashboard");
```

## Observability on failure

Wire `OnSoftFailure` to capture a screenshot automatically when a soft assertion fails:

```csharp
var assert = Asserter.NewStateful(cfg =>
{
    cfg.OnSoftFailure = [async info => await page.ScreenshotAsync(
        new() { Path = $"failure-{info.Links[0]}.png" })];
});

await assert.Soft.That(page).HasTitle("Dashboard");
```

> **Pre-1.0 — API will change.**
> [Full docs](https://github.com/olesme/catchy) · [Usage guide](https://github.com/olesme/catchy/blob/main/docs/USAGE_GUIDE.md)

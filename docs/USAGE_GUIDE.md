# Catchy.Assertions — Usage Guide

This guide documents the current state of Catchy.Assertions and recommended usage patterns.

## Core Principles

### 1. Always Await

**Every assertion chain must be awaited.** The chain builds a lazy pipeline; `await` triggers execution.

```csharp
// ✅ Correct - awaited
await Assert.That(user.Age).IsAtLeast(18);

// ❌ Wrong - not awaited, assertion never runs
task = Assert.That(user.Age).IsAtLeast(18);
```

### 2. Entry Points

#### Standard entry points

```csharp
using static Catchy.Stateless;

// Global singleton, thread-safe for read-only chains
await Assert.That(value).Is(expected);
```

```csharp
using static Catchy.Ambient;

// Per-test stateful asserter (manages both hard and soft)
await Assert.That(x).Is(1);        // Hard - throws immediately
await Assert.Soft.That(y).Is(2);   // Soft - accumulates
```

```csharp
using static Catchy.AmbientSoft;

// Per-test soft asserter only
await Verify.That(y).Is(2);        // Soft - accumulates
```

#### Alternative entry points (naming + conflict avoidance)

If `Assert` conflicts with another framework (for example xUnit), use built-in aliases:

```csharp
using static Catchy.StatelessAlias;
using static Catchy.AmbientAlias;

await Check.That(value).Is(expected);      // Stateless alias
await Check.Soft.That(other).Is(123);      // Ambient alias
```

If you want custom naming style, create your own entry wrapper in test project:

```csharp
public static class MyEntry
{
    public static StatelessAsserter Ensure => Catchy.Stateless.Assert;
}
```

```csharp
using static MyEntry;

await Ensure.That(total).IsGreaterThan(0);
```

If `using static` creates ambiguity, use type alias instead:

```csharp
using C = Catchy.StatelessAlias;

await C.Check.That(value).Is(expected);
```

#### Explicit instantiation

```csharp
// Hard asserter with custom settings
var hard = Asserter.NewStateful(s => s.OnAssertion = [myLogger]);
await hard.That(x).Is(1);

// Soft asserter with custom settings
var verify = Asserter.NewSoft(s =>
    s.OnSoftFailure = [async info => await CaptureScreenshot(info)]);
await verify.That(y).Is(2);

// Typed soft asserter (catches only specific exceptions)
var typedSoft = Asserter.NewSoft<AssertionException>();
```

## Soft Assertions

### Models

1. **Ambient Model** (provider-managed state scope):

   ```csharp
   // Automatically managed by test framework integration package
   await Assert.Soft.That(x).Is(1);
   await Assert.Soft.That(y).Is(2);

   // Option A: explicit flush
   await Ambient.Assert.Soft.SoftState.FlushIfNeeded();

   // Option B: assertion-style check
   await Assert.That(Ambient.Assert.Soft.SoftState).HasNoErrors();
   ```

   Provider details by framework package:

   - xUnit: `AsyncLocal` scope provider.
   - Reqnroll plugin default: `AsyncLocal` scenario provider.
   - MSTest: `TestContext.Properties` preferred, `AsyncLocal` fallback.
   - TUnit: `ThreadLocal` provider (not `AsyncLocal`).

   Reqnroll also supports non-`AsyncLocal` usage patterns in step code:

   - DI step-constructor pattern: inject `StatefulAsserter` directly into `[Binding]` classes.
   - Base-steps pattern: derive step classes from a shared base that holds injected `StatefulAsserter`.
   - Scenario container/body-object pattern: register/reuse scenario-scoped stateful instance in container and consume from step classes.

   `AsyncLocal` is generally safe for normal awaited test flow in one scenario context.
   Prefer explicit/scenario-owned stateful instance for detached/parallel execution
   (`Task.Run`, `Parallel.*`, fire-and-forget).

1. **Instance Model** (recommended for parallel/detached contexts):

   ```csharp
   var verify = new SoftAsserter();
   await verify.That(x).Is(1);
   await verify.That(y).Is(2);

   // Checkpoint/revert support
   var cp = verify.Checkpoint();
   await verify.That(z).Is(3);
   verify.Revert(cp); // Revert to checkpoint

   // Flush explicitly
   if (verify.HasFailures)
       throw verify.SoftState.AggregateException!;
   ```

1. **Per-Chain Model**:

   ```csharp
   var state = new SoftState();
   await Stateless.Assert.That(x).Is(1).With(state);
   await Stateless.Assert.That(y).Is(2).With(state);

   // Flush
   await Stateless.Assert.That(state).HasNoErrors();
   ```

### Hooks

```csharp
// OnSoftFailure - immediate callback when soft assertion fails
var verify = Asserter.NewSoft(s =>
    s.OnSoftFailure = [
        async info => await CaptureScreenshot(info),
        async info => await LogToTelemetry(info)
    ]);

// OnFlush - called on flush before throw/action
var state = new SoftState();
state.OnFlush = [
    ex =>
    {
        Telemetry.TrackErrors(ex.InnerExceptions.Count);
        return Task.CompletedTask;
    },
    ex =>
    {
        Logger.LogErrors(ex);
        return Task.CompletedTask;
    }
];
```

### Flush lifecycle: accumulation, auto/manual flush, and callbacks

Soft failures are always accumulated in `SoftState` (`Errors`, `ErrorCount`, `HasFailures`).

Manual flush options:

```csharp
// 1) Explicit flush by API (throws aggregate by default)
await verify.SoftState.FlushIfNeeded();

// 2) Assertion-based flush check
await Assert.That(verify.SoftState).HasNoErrors();

// 3) Manual throw from aggregate (when custom flow is needed)
if (verify.HasFailures)
    throw verify.SoftState.AggregateException!;
```

Flush behavior controls:

```csharp
var state = new SoftState
{
    FlushOnce = true, // default: first successful flush marks AlreadyFlushed
    FlushAction = async aggregate =>
    {
        // Custom sink instead of throw (e.g. test framework integration)
        await ReportAggregate(aggregate);
    }
};

// Callbacks triggered before FlushAction/throw
state.OnFlush = [
    async aggregate => await CaptureArtifacts(aggregate)
];
```

Checkpoint/revert for staged verification:

```csharp
var cp = verify.Checkpoint();
await verify.That(stepA).IsTrue();
await verify.That(stepB).IsTrue();

// Discard failures from this stage if needed
verify.Revert(cp);
```

Auto-flush patterns are typically provided by integration packages/hooks:

- xUnit: `CatchyTestBase.DisposeAsync()` flushes soft state.
- NUnit: `AmbientNUnitBase` tear-down flushes soft state.
- MSTest: `AmbientMSTestBase` cleanup flushes soft state.
- TUnit: `CatchyHooks.After` flushes soft state.
- Reqnroll: scenario hooks inject/flush aggregated soft failures.

## Stateful Asserter: hooks, wrappers, logging/reporting/screenshot flows

Use stateful asserter when you need per-test hooks and shared soft lifecycle:

```csharp
var assert = Asserter.NewStateful(cfg =>
{
    cfg.OnAssertion = [
        info =>
        {
            // Central assertion telemetry/logging hook
            return ValueTask.CompletedTask;
        }
    ];

    cfg.OnSoftFailure = [
        async info =>
        {
            // Called immediately when soft failure is captured
            await CaptureScreenshot(info);
            await AttachToReport(info);
        }
    ];

    cfg.OnExecution = [
        async (pipeline, next) =>
        {
            var started = DateTime.UtcNow;
            try
            {
                await next();
            }
            finally
            {
                var elapsed = DateTime.UtcNow - started;
                await TrackDuration(elapsed);
            }
        }
    ];
});

await assert.That(pageTitle).Contains("Dashboard");
await assert.Soft.That(userName).Is("admin");
```

Hook roles:

- `OnAssertion` — runs for assertion outcomes (logging/telemetry/reporting).
- `OnSoftFailure` — runs when a soft failure is added to collection (screenshots, attachments, traces).
- `OnExecution` — wrapper pipeline around execution (`next`) for cross-cutting behaviors.

Use wrappers for concerns like:

- timing/metrics,
- fail-evidence collection,
- temporary context slot injection,
- custom retry/short-circuit policies.

### Agentic and spec-driven usage (Copilot / Codex / Claude Code)

For agent-assisted and SDD workflows, treat assertion events as trace records.
`AssertionInfo` already gives core fields needed for reproducible diagnostics:

- `Source.File`, `Source.Line`, `Source.Member` (trace location),
- `Links` (chain replay in user DSL order),
- `Status` + `Success` (passed/failed/skipped),
- `UserMessage` (`Because(...)` context from spec intent),
- `Exception` (failure payload),
- `Duration` (timing signal).

Recommended pattern:

- use `OnAssertion` for both success and failure traces,
- use `OnSoftFailure` for immediate failure artifacts (screenshots, dumps),
- enrich each record with spec/test metadata (`SpecId`, `CaseId`, `StepId`).

```csharp
var assert = Asserter.NewStateful(cfg =>
{
    cfg.OnAssertion = [async info =>
    {
        var trace = new
        {
            status = info.Status.ToString(),
            ok = info.Success,
            file = info.Source.File,
            line = info.Source.Line,
            member = info.Source.Member,
            chain = string.Join(" -> ", info.Links),
            because = info.UserMessage,
            durationMs = info.Duration.TotalMilliseconds,
            error = info.Exception?.Message
        };

        await AgentTraceSink.WriteAsync(trace); // JSONL/OTel/etc.
    }];

    cfg.OnSoftFailure = [async info =>
    {
        await ScreenshotService.CaptureAsync(info.Source.File, info.Source.Line);
        await ArtifactSink.AttachAsync("assertion-failure", info.ToString());
    }];
});
```

Why log successful assertions too:

- agents can compare expected spec path vs actual executed chain,
- flaky behavior investigation benefits from timing and pass history,
- replayable traces improve automated remediation prompts.

Per-chain custom formatting is supported with `.With(...)`:

```csharp
await Assert.That(order.Total).IsGreaterThan(0)
    .Because("SPEC-ORD-014: order total must be positive")
    .With(info => SpecLogger.Log("SPEC-ORD-014", info));
```

## Trailing Modifiers

Trailing modifiers attach to the pipeline and are executed when awaited:

### `.Because(reason)`

```csharp
await Assert.That(user.Age).IsAtLeast(18)
    .Because("registration requires adulthood");
```

### `.With(...)` (per-chain hooks/config/context)

`With(...)` is the unified trailing extension family. Use overloads for the needed concern:

```csharp
// Assertion callback hook
await Assert.That(pageTitle).Contains("Home")
    .With(async info =>
    {
        await SendToMyService(info);
        await LogToFile(info);
    });

// Soft context injection for this chain
var state = new SoftState();
await Assert.That(x).Is(1).With(state);

// Soft asserter context injection (settings + soft state)
var verify = Asserter.NewSoft();
await Assert.That(y).Is(2).With(verify);

// Per-chain equals/deep-equality configuration
await Assert.That(actual).IsEquivalentTo(expected)
    .With((EqualsOptions o) => o.StringComparison = StringComparison.OrdinalIgnoreCase);
```

### `.Within(timeout)` (for async operations)

```csharp
await Assert.That(async () => await FetchData())
    .Completes().Within(TimeSpan.FromSeconds(2));
```

### `.WithRetry(every, within, attempts)`

```csharp
await Assert.That(async () => await FetchData())
    .Completes().WithRetry(
        every: TimeSpan.FromMilliseconds(100),
        within: TimeSpan.FromSeconds(1),
        attempts: 5
    );
```

### `.When(condition)` / `.WhenNot(condition)`

```csharp
await Assert.That(value).Is(1).When(isEnabled);
await Assert.That(value).Is(1).WhenNot(isDisabled);
```

## Execution Wrappers (Advanced)

Wrappers provide cross-cutting concerns around assertion execution:

```csharp
var myAsserter = Asserter.NewStateful(cfg =>
{
    // Telemetry wrapper
    cfg.OnExecution = [
        (pipeline, next) => ExecutionWrapperExamples.Telemetry("assertion", (name, dur, ok) =>
            Console.WriteLine($"{name} {dur.TotalMilliseconds}ms ok={ok}"))(pipeline, next)
    ];

    // Evidence-on-failure wrapper
    cfg.OnExecution = [.. cfg.OnExecution,
        (pipeline, next) => ExecutionWrapperExamples.EvidenceOnFailure(async pipeline =>
        {
            // Capture screenshot, DOM dump, etc.
            if (pipeline.Slots.TryGet<Playwright.IPage>(out var page))
                await page.ScreenshotAsync();
        })(pipeline, next)
    ];
});

await myAsserter.That(x).Is(y);
```

## Provider-Backed Assertions

For async value providers (e.g., Playwright):

```csharp
// Instead of blocking with .Result, use provider-backed chain
return new StringAssertions(
    () => page.TitleAsync(), 
    pipeline
);

// Now string checks will invoke provider asynchronously
await Assert.That(pageProvider).Contains("Home");
```

## Integration patterns across frameworks (stateful usage strategies)

This section summarizes practical ways to use stateful/soft flows across xUnit, NUnit, MSTest, TUnit, Reqnroll, and custom environments.

### 1) Explicit variable (most explicit and portable)

```csharp
var assert = Asserter.NewStateful();

await assert.That(value).Is(expected);
await assert.Soft.That(other).Is(otherExpected);
await assert.Soft.SoftState.FlushIfNeeded();
```

Use when:

- you need explicit ownership/lifecycle,
- you run detached/parallel flows,
- you want framework-agnostic composition.

Tradeoff: more plumbing in each test.

### 2) Base class integration (framework convenience)

Available integration base classes/hooks provide per-test setup + auto flush:

- `Catchy.XUnit.CatchyTestBase`
- `Catchy.NUnit.AmbientNUnitBase`
- `Catchy.MSTest.AmbientMSTestBase`
- `Catchy.TUnit.CatchyHooks` (global hooks)

Use when:

- team prefers minimal per-test boilerplate,
- ambient lifecycle should be standardized by framework package.

Tradeoff: lifecycle is less explicit than local variable ownership.

### 3) DI/container-managed stateful asserter (integration/acceptance tests)

```csharp
// Example registration idea (project-level)
// services.AddScoped(_ => Asserter.NewStateful());

// Then inject StatefulAsserter into test/step classes and flush at scope end.
```

Use when:

- test host already uses dependency injection,
- you need shared assertion context per scenario/request.

Tradeoff: scope boundaries and flush ownership must be well-defined.

### 4) Ambient integration packages

Ambient entry points (`using static Catchy.Ambient`, `using static Catchy.AmbientSoft`) are convenient when provider/hook package controls lifecycle.

Examples in repository packages:

- `Catchy.XUnit` (provider + base/fixture integration)
- `Catchy.NUnit` (provider + base class integration)
- `Catchy.MSTest` (provider + base class integration)
- `Catchy.TUnit` (provider + global hooks)
- `Catchy.Reqnroll` (plugin provider + scenario hooks; also supports DI/base-steps/container-owned stateful patterns)

Nuances:

- Ambient is convenient for regular test flow.
- xUnit/Reqnroll ambient providers use `AsyncLocal`: good for normal awaited flow in one logical test context.
- MSTest provider prefers `TestContext.Properties` (with `AsyncLocal` fallback), reducing context-loss risk in MSTest lifecycle.
- TUnit provider uses `ThreadLocal` because `AsyncLocal` is not reliable for its multi-context per-test model.
- For detached async/parallel work (`Task.Run`, `Parallel.ForEach`, background fire-and-forget), prefer explicit `SoftAsserter` / `StatefulAsserter` ownership.
- Keep a single clear flush owner per test/scenario to avoid duplicate or missed flush.

## Best Practices

### 1. Prefer `using static` for Default Entry Points

```csharp
// For hard assertions
using static Catchy.Stateless;

// For ambient stateful (hard + soft)
using static Catchy.Ambient;

// For ambient soft only
using static Catchy.AmbientSoft;
```

### 2. Use Explicit Instantiation for Custom Settings

```csharp
// Create once per test/fixture
var verify = Asserter.NewSoft(s =>
{
    s.OnAssertion = [myLogger];
    s.OnSoftFailure = [screenshotCapture];
});
```

### 3. Trail Modifiers After Assertion Methods

```csharp
// ✅ Correct - trailing modifiers after assertion operation
await Assert.That(x).Is(1)
    .Because("reason")
    .With(info => Logger.Log(info.ToString()));

// ❌ Wrong - callback attached before assertion operation
await Assert.That(x)
    .With(info => Logger.Log(info.ToString()))
    .Is(1); // callback is not attached to the intended final operation flow
```

### 4. Use Hooks for Cross-Cutting Concerns

```csharp
// Soft failure hooks for screenshots, logging, telemetry
var verify = Asserter.NewSoft(s =>
    s.OnSoftFailure = [
        async info => await CaptureScreenshot(info),
        async info => await LogToTelemetry(info)
    ]);

// Execution wrappers for telemetry, evidence capture
var asserter = Asserter.NewStateful(cfg =>
    cfg.OnExecution = [telemetryWrapper]);
```

### 5. Prefer Instance Model for Parallel/Detached Contexts

```csharp
// In Task.Run or parallel scenarios, use explicit instance
var verify = new SoftAsserter();
await Task.Run(async () =>
{
    await verify.That(x).Is(1);
});
```

## Examples

### Basic Assertions

```csharp
// Primitives
await Assert.That(a).Is(b);
await Assert.That(n).IsGreaterThan(0);
await Assert.That(text).Contains("hello");

// Collections
await Assert.That(collection).HasCountOf(5);
await Assert.That(collection).Contains(item);

// Exceptions
await Assert.That(() => ThrowingMethod()).Throws<InvalidOperationException>();

// Tasks
await Assert.That(async () => await FetchData()).Completes().Within(TimeSpan.FromSeconds(1));
```

### Soft Assertions with Hooks

```csharp
var verify = Asserter.NewSoft(s =>
    s.OnSoftFailure = [
        async info => 
        {
            var ex = info.Exception as AssertionException;
            Console.WriteLine($"Soft failure: {ex?.Body}");
        }
    ]);

await verify.That(1).Is(2);
await verify.That("a").Is("b");

// Flush and verify
if (verify.HasFailures)
    throw verify.SoftState.AggregateException!;
```

### Quantified Assertions

```csharp
// All items satisfy condition
await Assert.ThatEachOf(users).Satisfies(u => u.Age >= 18);

// Any item satisfies condition
await Assert.ThatAnyOf(users).Satisfies(u => u.IsAdmin);

// None satisfy condition
await Assert.ThatNoneOf(users).Satisfies(u => u.IsDeleted);
```

### Chaining with Connectors

```csharp
await Assert.That(x)
    .IsGreaterThan(0).And
    .IsLessThan(100).And
    .IsEven();

await Assert.That(text)
    .IsNotNull().But
    .IsNotEmpty();
```

### Sub-Chain Projection

```csharp
await Assert.That(() => ThrowingMethod())
    .Throws<InvalidOperationException>().WhoseMessage()
    .Contains("invalid");

await Assert.That(user)
    .ThatHas(u => u.Address).ThatHas(a => a.City)
    .Is("New York");
```

### Deep Equality with Rules

```csharp
var rules = DeepEqualRule.For<Order>()
    .Excluding(o => o.Timestamp)
    .WithStrictOrdering();

await Assert.That(actualOrder).IsEquivalentTo(expectedOrder).UsingRules(rules);
```

## Practical Notes

- Use this guide as the canonical behavior reference for current source.
- Keep examples aligned with existing APIs in `src/` and integration packages.
- Keep primary docs current-state and remove migration/history sections.

## Related docs

- [readme.md](readme.md)
- [ARCHITECTURE.md](ARCHITECTURE.md)
- [SourceGenerator_Architecture.md](SourceGenerator_Architecture.md)
- [agentic-observability.md](agentic-observability.md)
- [repository-structure.md](repository-structure.md)

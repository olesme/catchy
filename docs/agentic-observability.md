# Agentic Observability and Spec-Driven Assertion Tracing

This guide shows how to use Catchy hooks and wrappers for agent-assisted workflows
(Copilot, Codex, Claude Code, and similar) and spec-driven development (SDD).

## Why this matters for agentic + SDD flows

In agentic and SDD workflows, assertion output is not only for humans.
It also becomes machine-readable execution evidence used to:

- map spec steps to executed assertion chains,
- diagnose failures with reproducible trace context,
- compare passing vs failing runs,
- generate better repair prompts and regression checks.

## Core trace payload from `AssertionInfo`

For each assertion event, capture these fields:

- `Source.File`, `Source.Line`, `Source.Member` — trace location,
- `Links` — replayable chain in DSL call order,
- `Status` and `Success` — passed/failed/skipped classification,
- `UserMessage` (from `.Because(...)`) — spec intent/context,
- `Exception` — failure payload when present,
- `Duration` — timing signal for flaky/slowness analysis.

## Telemetry budget policy (minimal by default)

Keep built-in assertion telemetry small and cheap.
Do not add broad run-level or edge-case fields to core unless they are universally needed.

Recommended split:

- **Core event (always):** source, chain, status/success, because, exception, duration.
- **Optional context (hooks/wrappers/slots):** spec ids, agent ids, correlation ids,
  attempt counters, artifact references, environment metadata.

This matches Catchy design: advanced needs are implemented through hooks/wrappers,
not by bloating default payload for every project.

### `Duration` vs `timestampStartUtc` / `timestampEndUtc`

For assertion-level telemetry, `Duration` is usually enough.
Start/end timestamps are optional and should be added only when needed, for example:

- cross-system time alignment with external traces,
- ordering events across distributed sinks,
- strict timeline reconstruction requirements.

If needed, add start/end via wrapper logic instead of expanding core payload for everyone.

## Unified logging for success and failure

Do not log only failures. In agentic pipelines, successful events are also valuable
for baseline and execution-path comparison.

```csharp
var assert = Asserter.NewStateful(cfg =>
{
    cfg.OnAssertion = [async info =>
    {
        var evt = new
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

        await AgentTraceSink.WriteAsync(evt); // JSONL, OTel, custom sink
    }];
});

await assert.That(order.Total).IsGreaterThan(0)
    .Because("SPEC-ORD-014: order total must be positive");
```

## Immediate failure artifacts for soft assertions

Use `OnSoftFailure` for immediate evidence capture (screenshots, DOM, API response dump)
right when failure is added to soft collection.

```csharp
var verify = Asserter.NewSoft(cfg =>
{
    cfg.OnSoftFailure = [
        async info => await ScreenshotService.CaptureAsync(info.Source.File, info.Source.Line),
        async info => await ArtifactSink.AttachAsync("soft-failure", info.ToString())
    ];
});
```

## Flush-level aggregation hooks

Use `SoftState.OnFlush` for aggregate-level reporting and final bundle export.

```csharp
var state = new SoftState
{
    OnFlush = [async aggregate => await ReportSink.PublishAsync(new
    {
        total = aggregate.InnerExceptions.Count,
        message = aggregate.Message
    })]
};

var verify = new SoftAsserter(state);
```

## Per-chain custom logging (`.With(...)`)

For spec step granularity, add per-chain callbacks that format output as needed.

```csharp
await Assert.That(user.Age).IsAtLeast(18)
    .Because("SPEC-USER-007: legal age gate")
    .With(info => SpecLogger.Log("SPEC-USER-007", info));
```

## Wrapper-based context enrichment

Use `OnExecution` wrappers and pipeline slots to attach cross-cutting metadata around each pipeline run:

- scenario/test id,
- agent run id,
- branch/commit,
- external trace correlation id.

```csharp
var assert = Asserter.NewStateful(cfg =>
{
    cfg.OnExecution = [async (pipeline, next) =>
    {
        var startedUtc = DateTime.UtcNow;

        // Example: inject optional context via slots/wrapper scope
        pipeline.Slots.Set("SpecId", "SPEC-ORD-014");
        pipeline.Slots.Set("AgentRunId", Environment.GetEnvironmentVariable("AGENT_RUN_ID"));

        try
        {
            await next();
        }
        finally
        {
            var endedUtc = DateTime.UtcNow;
            await CorrelationSink.TrackAsync(new
            {
                startedUtc,
                endedUtc,
                durationMs = (endedUtc - startedUtc).TotalMilliseconds,
                specId = pipeline.Slots.Get<string>("SpecId"),
                agentRunId = pipeline.Slots.Get<string>("AgentRunId")
            });
        }
    }];
});
```

## Suggested event shape for agent pipelines

### Minimal schema (default)

- `status` / `success`
- `file` / `line` / `member`
- `chain`
- `because`
- `durationMs`
- `errorType` / `errorMessage`

### Extended schema (optional by wrapper/hook)

- `timestampStartUtc` / `timestampEndUtc`
- `specId` / `stepId`
- `agentRunId` / `correlationId`
- `attempt`
- `artifacts[]`

Keep minimal schema as baseline. Add extended fields only when your workflow truly consumes them.

## Practical agent workflows (not just sink snippets)

### 1) Copilot-style PR fix loop

1. Run tests with assertion traces enabled.
2. Build a compact prompt payload from failed assertions:
   - `file:line`, chain, because, failure message.
3. Ask agent for patch + test update suggestion.
4. Re-run and compare pass/fail traces (including successful assertions around changed area).

Useful output bundle per failure:

- `source` (`file`, `line`, `member`)
- `chain`
- `because`
- `error`
- `artifacts`

### 2) Codex-style batch remediation

Use nightly jobs to export JSONL assertion events, group by `(chain + source + error signature)`,
and open batched fix tasks for high-frequency clusters.

Minimum grouping keys:

- `file`, `line`, `chain`, `errorType`, normalized `errorMessage`.

### 3) Claude Code-style spec mismatch triage

Map each assertion to a spec step via `.Because("SPEC-...")` or wrapper slot metadata.
When mismatch occurs, send agent:

- expected spec step id,
- actual executed chain,
- source location,
- evidence artifacts.

This gives enough context for targeted patch suggestions instead of generic "test failed" prompts.

## Framework integration notes

No framework lock-in is required for this model.
You can apply the same hooks/wrappers with:

- explicit asserter variable ownership,
- ambient provider integrations,
- base-class lifecycle integrations,
- DI/container-managed scenario scope.

Keep one clear flush owner per test/scenario to avoid duplicate flush or missed finalization.

# Samples

This folder contains runnable, focused examples covering Catchy usage patterns across:

- extensibility mechanisms,
- stateful hooks/wrappers and soft-flush flows,
- framework runner integrations.

## Coverage matrix

| Area | Sample project | Status | Notes |
|---|---|---|---|
| AssertEntry + AssertVia (UI abstractions) | `PlaywrightAbstractionsDemo` | ✅ | Existing sample covering two entry-point strategies |
| Extensibility mechanisms (`AssertFor`, `Assertable`, typed overloads, delegation) | `ExtensibilityMatrixDemo` | ✅ | Added in this expansion |
| Stateful hooks/wrappers (`OnAssertion`, `OnSoftFailure`, `OnExecution`) | `StatefulHooksAndFlushDemo` | ✅ | Includes trace-style logging examples |
| Manual soft flush | `StatefulHooksAndFlushDemo` | ✅ | Explicit `FlushIfNeeded` and assertion-based flush |
| Auto flush via runner lifecycle | `RunnerXUnitDemo`, `RunnerNUnitDemo`, `RunnerMSTestDemo`, `RunnerTUnitDemo` | ✅ | Base/hook-driven lifecycle examples |
| Reqnroll integration patterns | `RunnerReqnrollDemo` | ✅ | DI constructor, base-steps and scenario-container style notes |

## Sample index

### 1) `PlaywrightAbstractionsDemo`

Demonstrates:

- `AssertEntry` for typed hierarchy entry points,
- `AssertVia` for delegated entry points to inner locator-like objects.

### 2) `ExtensibilityMatrixDemo`

Demonstrates:

- plain extension methods,
- `[AssertFor]` generated assertions,
- `[Assertable]` member-level generation,
- `[GenerateTypedOverloads]` typed fan-out,
- composition/delegation patterns.

### 3) `StatefulHooksAndFlushDemo`

Demonstrates:

- per-asserter hooks (`OnAssertion`, `OnSoftFailure`),
- execution wrappers (`OnExecution`) for telemetry/evidence,
- manual and assertion-style soft flush,
- checkpoint/revert flow.

### 4) Runner samples

- `RunnerXUnitDemo` — ambient + base class/fixture pattern.
- `RunnerNUnitDemo` — ambient + base class teardown flush.
- `RunnerMSTestDemo` — provider behavior with `TestContext`-oriented lifecycle.
- `RunnerTUnitDemo` — ambient + TUnit hook style.
- `RunnerReqnrollDemo` — plugin/ambient + DI/base-steps/container-owned usage notes.

## Running samples

Each sample has its own project and can be run/built individually from repository root.

Examples:

- `dotnet build samples/ExtensibilityMatrixDemo/ExtensibilityMatrixDemo.csproj`
- `dotnet build samples/StatefulHooksAndFlushDemo/StatefulHooksAndFlushDemo.csproj`

Runner-oriented samples are test projects; run them via test runner or `dotnet test`.

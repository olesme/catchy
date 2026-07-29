# Repository and Solution Structure

This document defines the intended repository layout so the Git tree itself is the primary documentation surface.

## Top-level directories

- `src/` — production packages and shared implementation.
- `tests/` — automated test projects and package-consumption smoke validation.
- `samples/` — runnable examples.
- `docs/` — canonical technical documentation.
- `.github/workflows/` — CI workflows.

## `src/` organization

- `Catchy` — core assertion library.
- `Catchy.SourceGenerator` — Roslyn source generator package.
- `Catchy.*` integrations — framework/package-specific extensions (NUnit, MSTest, TUnit, XUnit, Playwright, Cecil, Reqnroll, etc.).
- `Catchy.Analyzers` — analyzer project space.

## `tests/` organization

- `CatchyCoreTests` — core behavior tests.
- `CatchySourceGenTests` — source generator tests.
- integration-specific test projects (`CatchyPlaywrightTests`, ambient suites, etc.).
- `NuGetPackageSmoke` — package-only consumption check (no project references).

## Solution organization guidance

Keep the solution grouped by responsibility:

1. Core (`Catchy`, `Catchy.SourceGenerator`, `Catchy.Analyzers`)
2. Integrations (`Catchy.*` package projects)
3. Tests (`tests/*`)
4. Samples (`samples/*`)

When adding projects:

- place implementation under `src/` and tests under `tests/`.
- avoid status/progress snapshots in repository docs; keep docs current-state only.
- prefer package-consumption validation tests for cross-package behavior.

## Documentation maintenance rule

If code behavior changes, update canonical docs in the same change:

- `docs/readme.md`
- `docs/USAGE_GUIDE.md`
- `docs/ARCHITECTURE.md`
- `docs/SourceGenerator_Architecture.md`
- this file when structure changes

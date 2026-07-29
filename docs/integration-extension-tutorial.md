# Integration Package Tutorial (End-to-End)

This tutorial describes how to implement a new Catchy integration package with publish-ready quality and package behavior.

It applies to integrations such as Selenium, PuppeteerSharp, RestSharp, Playwright-style adapters, and other domain packages.

## Goal

Build an integration package that:

1. exposes focused assertion APIs for the integration domain,
2. composes with core Catchy pipeline semantics,
3. ships precompiled package internals,
4. supports consumer-side source generation through package delivery.

## Package blueprint

Example package: `Catchy.Selenium`

Recommended project structure:

- `src/Catchy.Selenium/Catchy.Selenium.csproj`
- `src/Catchy.Selenium/Assertions/*`
- `src/Catchy.Selenium/Checks/*` (when logic is reusable/complex)
- `tests/CatchySeleniumTests/*`

Project references in repo development:

- `ProjectReference` to `src/Catchy/Catchy.csproj`
- no direct analyzer project reference wiring in integration projects

NuGet consumer behavior should follow the explicit model:
`Catchy` for precompiled integration/core APIs,
and `Catchy.SourceGenerator` only when consumer-side generation is needed.

## Step-by-step implementation

### 1) Define scope and API surface

Decide what is in-scope for first package version:

- target objects/types (e.g., Selenium `IWebElement`, `IWebDriver`)
- core assertions (visibility, text, attributes, state)
- async/polling semantics expected by real tests

Keep first version narrow and stable.

### 2) Implement assertions as typed extensions

Add extension methods on `ValueAssertions<T>` in `Assertions/*`.

Use:

- `a.Link(...)` for chain rendering,
- `a.Op(...)` for operation registration,
- `CheckOperation.Sync/Async` depending on behavior.

When logic is shared across methods/types, move it to `Checks/*`.

### 3) Respect chain rendering and pipeline contracts

- visible chain must match user DSL call site,
- trailing modifiers must work as expected (`Because`, logger hooks, wrappers),
- failures should point to caller source location.

For composed assertions, use `DelegateTo(...)` to avoid leaking internal links.

### 4) Package and optional generator behavior

Integration packages should rely on core package delivery model:

- package internals are compiled into integration assembly,
- consumers use `Catchy` alone when no generation attributes are used,
- consumers add `Catchy.SourceGenerator` explicitly for `[AssertFor]`/`[Assertable]`/template generation,
- no manual analyzer project wiring is needed after adding `Catchy.SourceGenerator`.

Validate this through package-consumption smoke tests.

### 5) Add tests

At minimum:

- positive/negative assertion behavior tests,
- chain rendering expectations,
- trailing-modifier interactions,
- null/edge behavior,
- package-consumption smoke scenario where relevant.

### 6) Validate pack/consume path

For local validation:

1. pack required packages to local feed,
2. restore/build smoke consumer against that feed,
3. verify compilation + runtime behavior.

Use the same approach as `tests/NuGetPackageSmoke`.

## Definition of Done for integration package

A package is done when:

1. API surface is documented and tested.
2. Build/test pass on supported TFMs.
3. Package packs and restores cleanly.
4. NuGet consumption path is validated.
5. Canonical docs are updated in same change set.
6. Quality gates from [quality-gates.md](quality-gates.md) are satisfied.

## Planned integrations (candidate list)

Current high-value candidates:

- `Catchy.Selenium`
- `Catchy.PuppeteerSharp`
- `Catchy.RestSharp`

Keep this list as implementation guidance only.

## Backlog management policy

Document candidates and acceptance criteria in docs; move backlog management to GitHub Issues/Projects, and keep docs focused on architecture, tutorials, and quality standards.

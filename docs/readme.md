# Catchy.Assertions Documentation

![Cat Chy catching a bug in documentation](../assets/branding/cat-chy-bug-catch-2.png)

Catchy is an async-native fluent assertion library for .NET with:

- hard and soft assertion modes,
- lazy await-triggered execution,
- structured observability hooks,
- extensibility through typed extensions and source generation.

This page is the canonical documentation entry for the repository.

## Start here

- **Usage:** [USAGE_GUIDE.md](USAGE_GUIDE.md)
  - stateful asserter hooks/wrappers (logging, reporting, screenshots)
  - soft assertions lifecycle (accumulation, OnSoftFailure/OnFlush hooks, manual/auto flush)
  - framework integration patterns (explicit variable, base class, DI, ambient packages)
  - agentic + spec-driven tracing patterns (Copilot/Codex/Claude Code workflows)
- **Core architecture:** [ARCHITECTURE.md](ARCHITECTURE.md)
- **Source generation model:** [SourceGenerator_Architecture.md](SourceGenerator_Architecture.md)
- **Repository structure:** [repository-structure.md](repository-structure.md)
- **Samples index:** [../samples/README.md](../samples/README.md)
- **Agentic observability + SDD tracing:** [agentic-observability.md](agentic-observability.md)
- **Versioning and maturity policy:** [versioning-policy.md](versioning-policy.md)
- **AI instructions policy (local vs shared):** [ai-instructions-policy.md](ai-instructions-policy.md)
- **SonarLint adoption guide:** [sonarlint-adoption.md](sonarlint-adoption.md)

## Extensibility

- **Extensibility guide (all mechanisms):** [extensibility-guide.md](extensibility-guide.md)
  - includes plain extensions, `[AssertFor]`, `[Assertable]`, cross-type rules, `[GenerateTypedOverloads]`, `[AssertEntry]`, and `[AssertVia]`
- **Integration package tutorial (end-to-end):** [integration-extension-tutorial.md](integration-extension-tutorial.md)
- **Quality gates (core + integration packages):** [quality-gates.md](quality-gates.md)
- **Candidate integration catalog:** [integration-catalog.md](integration-catalog.md)
- **Contributing workflow:** [../CONTRIBUTING.md](../CONTRIBUTING.md)

## Core behavior (quick reference)

### Async-first execution

All assertions are awaitable; `await` executes the built pipeline.

```csharp
await Assert.That(user.Age).IsAtLeast(18);
```

### Entry points

- `using static Catchy.Stateless` → `Assert.That(value)`
- `using static Catchy.StatelessAlias` → `Check.That(value)`
- `using static Catchy.Ambient` → ambient `Assert.That(value)` and `Assert.Soft.That(value)`
- `using static Catchy.AmbientSoft` → `Verify.That(value)`

### Quantifiers

- `Assert.ThatEachOf(values)`
- `Assert.ThatAnyOf(values)`
- `Assert.ThatNoneOf(values)`

### Soft assertions

Use instance or ambient soft flows and flush explicitly:

```csharp
var verify = Asserter.NewSoft();
await verify.That(a).Is(expectedA);
await verify.That(b).Is(expectedB);
await Assert.That(verify.SoftState).HasNoErrors();
```

### Observability

Per-asserter logger chains and soft-failure callbacks are supported via settings and pipeline hooks.

## Source generation and package delivery

Catchy uses package-delivered source generation with explicit opt-in:

- `Catchy` package ships precompiled runtime/assertion behavior,
- `Catchy.SourceGenerator` ships analyzer assets,
- consumers add `Catchy.SourceGenerator` only when they need attribute-driven generation,
- consumer generation applies only to consumer source.

See [SourceGenerator_Architecture.md](SourceGenerator_Architecture.md) and `tests/NuGetPackageSmoke` for the package-consumption validation flow.

## Mascot assets

Cat Chy artwork is used in repository docs to visualize bug-catching and QA checklist flows.
For consistent rendering on GitHub light/dark themes, prefer transparent PNG backgrounds with good contrast.

## Project state

Canonical docs describe the current behavior in this branch.
Obsolete status/progress docs are not kept in `docs/`.

## Documentation maintenance rule

When implementation behavior changes, update canonical docs in the same change set:

- `docs/readme.md`
- `docs/USAGE_GUIDE.md`
- `docs/ARCHITECTURE.md`
- `docs/SourceGenerator_Architecture.md`
- `docs/repository-structure.md` (when layout changes)

Process templates (post-publication workflow):

- integration proposals: `.github/ISSUE_TEMPLATE/integration-package.yml`
- integration regressions: `.github/ISSUE_TEMPLATE/integration-bug.yml`
- PR checklist: `.github/pull_request_template.md`

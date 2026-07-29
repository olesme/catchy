# Contributing

Thank you for contributing to Catchy.Assertions.

## Start points

- Documentation entry: [docs/readme.md](docs/readme.md)
- Usage guide: [docs/USAGE_GUIDE.md](docs/USAGE_GUIDE.md)
- Architecture: [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)
- Source generation architecture: [docs/SourceGenerator_Architecture.md](docs/SourceGenerator_Architecture.md)
- Extensibility guide: [docs/extensibility-guide.md](docs/extensibility-guide.md)
- Integration package tutorial: [docs/integration-extension-tutorial.md](docs/integration-extension-tutorial.md)
- Quality gates: [docs/quality-gates.md](docs/quality-gates.md)
- Versioning policy: [docs/versioning-policy.md](docs/versioning-policy.md)
- AI instructions policy: [docs/ai-instructions-policy.md](docs/ai-instructions-policy.md)
- SonarLint adoption: [docs/sonarlint-adoption.md](docs/sonarlint-adoption.md)

## Contribution workflow

1. Open or pick an issue.
2. Implement minimal scoped change.
3. Update canonical docs when behavior changes.
4. Ensure quality gates pass.
5. Open PR with checklist completed.

## Issues and templates

- New integration proposal:
  - `.github/ISSUE_TEMPLATE/integration-package.yml`
- Integration bug/regression:
  - `.github/ISSUE_TEMPLATE/integration-bug.yml`

## Pull requests

Use `.github/pull_request_template.md` and complete relevant sections:

- build/tests,
- packaging,
- NuGet consumption path,
- documentation updates.

## Documentation policy

Canonical docs are current-state only.

When implementation behavior changes, update canonical docs in the same PR:

- `docs/readme.md`
- `docs/USAGE_GUIDE.md`
- `docs/ARCHITECTURE.md`
- `docs/SourceGenerator_Architecture.md`
- `docs/extensibility-guide.md`
- `docs/integration-extension-tutorial.md`
- `docs/quality-gates.md`

## Cat Chy QA mindset

![Cat Chy QA checklist](assets/docs/testing/cat-chy-checklist-pass-1.png)

Cat Chy catches bugs, not mice: keep validation explicit, and preserve clear pass/fail expectations in tests and docs.

## Build and validation

Recommended minimum local validation:

1. `dotnet build CatchyAssertions.slnx`
2. relevant tests for touched areas
3. pack/build smoke path when package behavior changes

## Local quality hooks (Husky.NET)

Repository includes Husky.NET local hook tasks:

- `pre-commit`: markdown lint + fast solution build
- `pre-push`: targeted core and source-generator test suites

Useful commands:

1. `dotnet tool restore`
2. `dotnet husky install`
3. `dotnet husky run --group pre-commit`
4. `dotnet husky run --group pre-push`

Manual local gate runner (same core checks as hooks):

- `powershell -ExecutionPolicy Bypass -File ./scripts/quality-local.ps1`
- optional flags: `-SkipBuild`, `-SkipTests`, `-SkipMarkdown`

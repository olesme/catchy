# Integration Catalog (Candidate Packages)

This catalog lists candidate integration packages and intended initial scope.

## Active / existing

- `Catchy.Playwright`
- `Catchy.Playwright.Visual`
- `Catchy.Cecil`

## Candidate integrations

### `Catchy.Selenium`

Initial scope:

- visibility/state assertions on `IWebElement`,
- attribute/text assertions,
- optional wait-aware helpers aligned with Catchy pipeline semantics.

### `Catchy.PuppeteerSharp`

Initial scope:

- page/element assertions,
- text/attribute/value checks,
- async-first checks for browser/page operations.

### `Catchy.RestSharp`

Initial scope:

- response status/content assertions,
- header/body assertions,
- serialization-aware checks where practical.

## Quality expectations

All integrations must satisfy [quality-gates.md](quality-gates.md) and the implementation flow in [integration-extension-tutorial.md](integration-extension-tutorial.md).

## Backlog handling

Candidate tracking and prioritization is managed via GitHub Issues/Projects.

Recommended issue workflow:

- use `.github/ISSUE_TEMPLATE/integration-package.yml` for new integration proposals,
- map each approved proposal to implementation tasks and quality-gate tracking.

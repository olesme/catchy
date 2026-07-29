# Run this once after pushing to GitHub to wire up branch protection and create backlog issues.
# Prerequisites: gh auth login  (GitHub CLI installed and authenticated)
#
# Usage: pwsh ./scripts/setup-github.ps1
#        pwsh ./scripts/setup-github.ps1 -SkipIssues
#        pwsh ./scripts/setup-github.ps1 -SkipProtection

param(
    [switch]$SkipProtection,
    [switch]$SkipIssues
)

$repo = "olesme/catchy"

# ── Branch protection ──────────────────────────────────────────────────────────
if (-not $SkipProtection) {
    Write-Host "Setting up branch protection for main..." -ForegroundColor Cyan

    # Required status checks mirror the job names in ci.yml
    $protection = @{
        required_status_checks = @{
            strict   = $true
            contexts = @("markdown-lint", "build-and-smoke")
        }
        enforce_admins                  = $false
        required_pull_request_reviews   = @{
            required_approving_review_count = 0
            dismiss_stale_reviews           = $true
        }
        restrictions = $null
        allow_force_pushes  = $false
        allow_deletions     = $false
    } | ConvertTo-Json -Depth 10

    gh api "repos/$repo/branches/main/protection" `
        --method PUT `
        --header "Accept: application/vnd.github+json" `
        --input ([System.IO.MemoryStream][System.Text.Encoding]::UTF8.GetBytes($protection))

    Write-Host "Branch protection configured." -ForegroundColor Green
}

# ── GitHub Issues (backlog) ────────────────────────────────────────────────────
if (-not $SkipIssues) {
    Write-Host "Creating backlog issues..." -ForegroundColor Cyan

    $issues = @(
        @{
            title  = "Performance benchmarks: Catchy vs FluentAssertions / Shouldly / NUnit Assert"
            labels = @("enhancement", "performance", "backlog")
            body   = @"
## Goal
Establish baseline perf data before 1.0 to understand overhead of the lazy pipeline and guide any hot-path work.

## Scope
- BenchmarkDotNet suite under ``tests/CatchyBenchmarks/``
- Scenarios: single hard assert, chain of 5, collection quantifier (100 items), soft-flush of 10 failures
- Competitors: FluentAssertions 7.x, Shouldly 4.x, bare NUnit/xUnit Assert
- Metrics: mean allocation, mean time, Gen0/Gen1 GC

## Acceptance criteria
- Benchmark project added to solution (not to gate suites — it is opt-in)
- README table summarising results against each competitor
- No regression gate in CI (benchmarks are advisory pre-1.0)

## Notes
Run with ``dotnet run -c Release --project tests/CatchyBenchmarks``
"@
        },
        @{
            title  = "Integration: Catchy.Selenium — WebDriver assertion surface"
            labels = @("enhancement", "integration", "backlog")
            body   = @"
## Goal
Provide a fluent async assertion surface for Selenium WebDriver element and page assertions.

## Proposed API sketch
``````csharp
await Assert.That(driver).CurrentUrl().Contains("checkout");
await Assert.That(element).IsVisible().HasText("Submit");
``````

## Scope
- New package ``Catchy.Selenium`` targeting ``netstandard2.0;net8.0;net9.0``
- Assertable surfaces: ``IWebDriver``, ``IWebElement``
- Async wrappers via ``WebDriverWait`` / ``IJavaScriptExecutor``
- Smoke-test entry in ``tests/NuGetPackageSmoke``
- Integration catalog entry in ``docs/integration-catalog.md``

## Out of scope for v1
- Page Object Model helpers (separate package)
- Grid / parallel session management

## Prerequisite
Integration proposal approved via integration issue template.
"@
        },
        @{
            title  = "Integration: Catchy.Puppeteer — PuppeteerSharp browser assertion surface"
            labels = @("enhancement", "integration", "backlog")
            body   = @"
## Goal
Fluent async assertions for PuppeteerSharp ``IPage`` / ``IElementHandle``.

## Proposed API sketch
``````csharp
await Assert.That(page).HasTitle("Dashboard");
await Assert.That(element).IsVisible().HasAttribute("aria-disabled", "false");
``````

## Scope
- New package ``Catchy.Puppeteer`` targeting ``net8.0;net9.0`` (PuppeteerSharp 20.x is .NET 8+)
- Assertable surfaces: ``IPage``, ``IElementHandle``
- Screenshot-on-failure hook via ``PwSlots``-style pipeline slot
- Integration catalog entry in ``docs/integration-catalog.md``

## Notes
Shares patterns established by ``Catchy.Playwright`` — review that package first.
"@
        },
        @{
            title  = "Integration: Catchy.Http — HttpClient / HttpResponseMessage assertion surface"
            labels = @("enhancement", "integration", "backlog")
            body   = @"
## Goal
Readable async assertions for HTTP API test scenarios without a browser dependency.

## Proposed API sketch
``````csharp
await Assert.That(response)
    .HasStatusCode(HttpStatusCode.OK)
    .HasHeader("Content-Type", "application/json")
    .BodyJson().Property("id").IsNotNull();
``````

## Scope
- New package ``Catchy.Http`` targeting ``netstandard2.0;net8.0;net9.0``
- Assertable surfaces: ``HttpResponseMessage``, response body (JSON via ``System.Text.Json``, XML)
- No extra dependencies beyond BCL (no RestSharp/Refit coupling)
- Integration catalog entry

## Notes
JSON body traversal can piggyback on the existing ``StructuralAssertions`` / deep-equal infrastructure.
"@
        },
        @{
            title  = "Integration: Catchy.Appium — Appium WebDriver mobile assertion surface"
            labels = @("enhancement", "integration", "backlog")
            body   = @"
## Goal
Extend ``Catchy.Selenium``-style patterns to mobile automation via Appium.NET client.

## Scope
- New package ``Catchy.Appium`` targeting ``net8.0;net9.0``
- Assertable surfaces: ``AppiumDriver``, ``AppiumElement``
- iOS and Android element assertions (accessibility ID, resource ID, XPath)
- Integration catalog entry

## Prerequisite
``Catchy.Selenium`` should be shipped first — shares the WebDriver abstraction.
"@
        },
        @{
            title  = "Publish pipeline: GitHub release → NuGet.org automation"
            labels = @("ci-cd", "infrastructure", "backlog")
            body   = @"
## Goal
One-click NuGet publication triggered by creating a GitHub release, using NuGet Trusted Publishing (OIDC) — no long-lived API key secret required.

## One-time setup on NuGet.org (per package)
For each of the 11 packages, go to nuget.org → package → Manage → Trusted Publishers → Add:
- **Repository Owner**: ``olesme``
- **Repository**: ``catchy``
- **Workflow File**: ``publish.yml``
- **Environment**: *(leave blank)*

## Steps
1. Reserve package IDs on NuGet.org:
   - ``Catchy``
   - ``Catchy.SourceGenerator``
   - ``Catchy.Analyzers``
   - ``Catchy.Cecil``
   - ``Catchy.Playwright``
   - ``Catchy.Playwright.Visual``
   - ``Catchy.XUnit``
   - ``Catchy.NUnit``
   - ``Catchy.MSTest``
   - ``Catchy.TUnit``
   - ``Catchy.Reqnroll``
2. Configure Trusted Publisher on nuget.org for each package (see above)
3. First release: create GitHub release with tag ``v0.0.0`` (semver, ``v``-prefixed)
4. ``.github/workflows/publish.yml`` exchanges the GitHub OIDC token for a short-lived NuGet token and pushes all packages
5. Verify all packages appear on NuGet.org within ~10 min

## Versioning policy
Patch (0.0.x) increments automatically with each release. Minor (0.x.0) and major (x.0.0)
only on explicit decision. No pre-release suffixes.

## Checklist
- [ ] All package IDs reserved on nuget.org
- [ ] Trusted Publisher policy configured on nuget.org (olesme/catchy, workflow: publish.yml)
- [ ] Test run with ``workflow_dispatch`` and version ``0.0.0`` before first real release
- [ ] Branch protection requires CI green before release
"@
        },
        @{
            title  = "v1.0 API stability: freeze public contract and write migration guide"
            labels = @("enhancement", "documentation", "milestone-1.0")
            body   = @"
## Goal
Declare a stable 1.0 API surface before incrementing the major version.

## Work items
- Audit all ``public`` types across ``src/Catchy`` and ``src/Catchy.SourceGenerator``
- Mark anything not ready for stability with ``[EditorBrowsable(Never)]`` or move to ``Catchy.Sdk``
- Write ``docs/MIGRATION.md`` covering 0.x → 1.0 breaking changes
- Enable ``PackageValidation`` (``Catchy.ApiCompat`` or ``Microsoft.DotNet.ApiCompat``) in CI

## Non-goals
- Compatibility shims (CLAUDE.md: no back-compat shims pre-1.0)

## Tracking
This issue tracks the 1.0 milestone. Tag all breaking-change PRs with ``breaking-change``.
"@
        },
        @{
            title  = "Assertion coverage: DateTime, TimeSpan, Guid, Uri typed surfaces"
            labels = @("enhancement", "assertion-surface", "backlog")
            body   = @"
## Goal
First-class typed assertion surfaces for common BCL value types currently falling through to ``StructuralAssertions``.

## Types
- ``DateTime`` / ``DateTimeOffset`` — Before/After/IsBetween/HasKind/WithinPrecision
- ``DateOnly`` / ``TimeOnly`` (net6+) — partial overlap with DateTime surface
- ``TimeSpan`` — IsLongerThan/IsShorterThan/IsCloseTo
- ``Guid`` — IsEmpty/IsNotEmpty/HasVersion
- ``Uri`` — HasScheme/HasHost/HasPath/HasQuery

## Approach
Follow the Checks/Assertions two-layer pattern. Typed surfaces via ``[Assertable]`` or manual extension methods as appropriate.
"@
        },
        @{
            title  = "Source generator: implement bundle fan-out ([ApplyToBundle])"
            labels = @("enhancement", "source-generator", "backlog")
            body   = @"
## Status
The bundle attributes exist in ``src/Catchy/Attributes/Bundles.cs`` but the generator does not implement fan-out yet. The ``copilot-instructions.md`` describes the intended semantics.

## Goal
Implement ``[ApplyToBundle]`` so that a single attribute on a bundle class fans out ``[GenerateTypedOverloads]`` across all registered types.

## Acceptance criteria
- ``CatchySourceGenTests`` covers at least: single-type bundle, multi-type bundle, bundle with exclusion
- No hand-maintained generated files; all output in ``obj/``
- ``SourceGenerator_Architecture.md`` updated to document the bundle pipeline
"@
        },
        @{
            title  = "Source generator: [GenerateNonNullableOverload] for reference-type assertion methods"
            labels = @("enhancement", "source-generator", "backlog")
            body   = @"
## Problem
String assertion methods use ``ValueAssertions<string?>`` receivers so they accept null strings.
When users call ``ThatEachOf(new[] { "a", "b" })`` the compiler infers ``QuantifiedAssertions<string>``
(non-nullable), which produces CS8620 when passing it to ``ValueAssertions<string?>`` extension methods.
The runtime is unaffected (``string`` and ``string?`` are the same IL type), but the warning is noise.

## Goal
Add a ``[GenerateNonNullableOverload]`` attribute to the source generator so that any assertion method
targeting ``ValueAssertions<T?>`` (where T is a reference type) automatically gets a companion overload
for ``ValueAssertions<T>`` — generated into ``obj/``, not hand-maintained.

## Alternative considered and rejected
Hand-written non-generic string overloads on the three entry points (ThatEachOf/ThatAnyOf/ThatNoneOf).
Rejected: doesn't scale to other reference types; 15 boilerplate methods for a cosmetic warning.

## Acceptance criteria
- Generator emits companion ``T`` overloads from ``T?``-receiver methods when attribute is present
- No manually maintained generated source
- CS8620 eliminated at call sites using non-nullable reference type collections
"@
        },
        @{
            title  = "Integration: Catchy.XUnit2 — xUnit v2 support (low priority)"
            labels = @("enhancement", "integration", "backlog")
            body   = @"
## Context
``Catchy.XUnit`` targets xUnit v3 (``net8.0;net9.0`` only — no netstandard because xUnit v3 dropped it).
Teams still on xUnit v2 cannot use ambient assertions or the ``ITestOutputHelper`` log sink.

## Goal
A separate ``Catchy.XUnit2`` package targeting ``netstandard2.0;net8.0`` that provides an
``IAmbientAsserterProvider`` backed by xUnit v2 ``ITestOutputHelper`` and ``AsyncLocal``.

## Notes
- Low priority: xUnit v3 migration is recommended
- Must not break ``Catchy.XUnit`` (v3) — entirely separate package
- May share helper code via ``CatchyTestHelpers`` or a new ``Catchy.XUnit.Shared`` internal project
"@
        }
    )

    foreach ($issue in $issues) {
        $labelArgs = $issue.labels | ForEach-Object { "--label", $_ }
        Write-Host "  Creating: $($issue.title)" -ForegroundColor Gray
        gh issue create `
            --repo $repo `
            --title $issue.title `
            --body $issue.body `
            @labelArgs
        Start-Sleep -Milliseconds 500
    }

    Write-Host "All issues created." -ForegroundColor Green
}

Write-Host "Done. Next steps:" -ForegroundColor Yellow
Write-Host "  1. Reserve all 11 NuGet package IDs on nuget.org"
Write-Host "  2. Configure Trusted Publisher policy on nuget.org (olesme/catchy, workflow: publish.yml)"
Write-Host "  3. First release tag: v0.0.0"

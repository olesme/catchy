# Catchy.Playwright.Visual

[![NuGet](https://img.shields.io/nuget/v/Catchy.Playwright.Visual.svg)](https://www.nuget.org/packages/Catchy.Playwright.Visual)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/olesme/catchy/blob/main/LICENSE)

Visual regression assertions for [Catchy.Playwright](https://www.nuget.org/packages/Catchy.Playwright) — pixel-diff powered by [SkiaSharp](https://github.com/mono/SkiaSharp).

```sh
dotnet add package Catchy
dotnet add package Catchy.Playwright
dotnet add package Catchy.Playwright.Visual
```

## Usage

```csharp
using Catchy;
using Catchy.Playwright;
using Catchy.Playwright.Visual;
using static Catchy.Ambient;

public class DashboardVisualTests : CatchyTestBase
{
    [Fact]
    public async Task Dashboard_matches_baseline()
    {
        var page = await GetPageAsync();
        await page.GotoAsync("https://example.com/dashboard");

        // Captures a screenshot and diffs against the stored baseline.
        // First run saves the baseline; subsequent runs compare to it.
        await Assert.That(page).MatchesSnapshot("dashboard-baseline");
    }
}
```

Pixel differences above the configured threshold fail the assertion and attach a diff image via the `OnSoftFailure` hook when used in soft mode.

> **Pre-1.0 — API will change.**
> [Full docs](https://github.com/olesme/catchy) · [Usage guide](https://github.com/olesme/catchy/blob/main/docs/USAGE_GUIDE.md)

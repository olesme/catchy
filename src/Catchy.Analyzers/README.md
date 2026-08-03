# Catchy.Analyzers

[![NuGet](https://img.shields.io/nuget/v/Catchy.Analyzers.svg)](https://www.nuget.org/packages/Catchy.Analyzers)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/olesme/catchy/blob/main/LICENSE)

Roslyn analyzers for Catchy — catches common mistakes at compile time.

```sh
dotnet add package Catchy.Analyzers
```

## Diagnostics

| ID | Severity | Description |
|----|----------|-------------|
| CATCHY001 | Warning | Unawaited assertion chain — the chain builds a lazy pipeline; without `await` no assertion runs. |
| CATCHY002 | Warning | Assertion chain usage issue — with code fix. |

### CATCHY001 — unawaited chain

```csharp
// ❌ warning CATCHY001: assertion chain is not awaited — no assertion will run
Assert.That(value).IsNotNull();

// ✅ correct
await Assert.That(value).IsNotNull();
```

`Catchy.Analyzers` ships with `Catchy` — you do not need to reference it separately unless you want to control the analyzer version independently.

> **Pre-1.0 — API will change.**
> [Full docs](https://github.com/olesme/catchy)

# Catchy.Assertions

![Cat Chy catching a bug](assets/branding/cat-chy-bug-catch-1.png)

Async-native fluent assertion library for .NET — real soft assertions,
observability hooks, and source-generator-powered typed surfaces.

[![NuGet](https://img.shields.io/nuget/v/Catchy.svg)](https://www.nuget.org/packages/Catchy)
[![CI](https://github.com/olesme/catchy/actions/workflows/ci.yml/badge.svg)](https://github.com/olesme/catchy/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

```sh
dotnet add package Catchy
dotnet add package Catchy.XUnit   # or .NUnit / .MSTest / .TUnit / .Reqnroll
```

## Why Catchy?

- **Async-native.** Every chain is `await`ed — attach structured logs, distributed
  traces, or AI/spec-driven output via async hooks. No threading hacks, no fire-and-forget.
- **Real soft assertions.** Not scoped exception aggregation. Interleave soft and hard
  assertions freely; pass failure state across helpers and step definitions via ambient,
  DI, or explicit instance.
- **No IntelliSense pollution.** `Assert.That(value)` — not `.Should()` appended to
  every type in every file.
- **Source-generator extensibility.** Annotate your type with `[Assertable]` or
  `[AssertFor]`, reference `Catchy.SourceGenerator` — a full typed assertion surface
  is generated. No boilerplate.
- **Trailing modifiers.** `.Because("reason").IgnoringCase()` goes after the assertion,
  not before. Reads like a sentence; the lazy pipeline applies it to the whole chain.
- **Alias-friendly.** Built-in `Check` / `Verify` aliases and support for custom ones —
  no conflicts with `xUnit.Assert`, `NUnit.Assert`, or anything else.
- **MIT, permanently.**

## Examples

### Fluent hard assertions

```csharp
using static Catchy.StatelessAlias;

await Assert.That(user.Name).IsNotNull().And().IsNotEmpty();
await Assert.That(order.Total).IsGreaterThan(0m).Because("free orders must go through promotions");
```

### Real soft assertions — all failures collected, reported together

```csharp
var verify = Asserter.NewSoft();

await verify.That(order.Id).IsGreaterThan(0);
await verify.That(order.Total).IsGreaterThan(0m);
await verify.That(order.Status).IsNotEmpty();
// throws AggregateAssertionException with all three failures at end of test
```

Or ambient (no instance management — the integration package flushes automatically):

```csharp
using static Catchy.AmbientSoft;

await Verify.That(order.Id).IsGreaterThan(0);
await Verify.That(order.Total).IsGreaterThan(0m);
```

### Async observability hook — structured logging, tracing, AI output

```csharp
var asserter = Asserter.NewStateful(cfg =>
    cfg.OnAssertion = [info =>
    {
        logger.LogInformation("[{Status}] {Chain}", info.Status, string.Join(" → ", info.Links));
        return ValueTask.CompletedTask;
    }]);

await asserter.That(response.StatusCode).Is(200);
```

### Source-generator typed surface

```csharp
// Domain.cs
[Assertable]
public sealed class Customer
{
    [AssertMember] public string Name { get; init; }
    [AssertMember] public int Age  { get; init; }
}

// Test.cs — no hand-written extension methods
await Check.That(customer).HasName("Olena").And().HasAge(21);
```

## Status

Early-stage, pre-1.0. The API will change — intentionally.
[Open a Discussion](https://github.com/olesme/catchy/discussions) before writing code
so direction is agreed first. Issues and PRs are very welcome.

## Documentation

- [Usage guide](docs/USAGE_GUIDE.md)
- [Architecture](docs/ARCHITECTURE.md)
- [Source generation](docs/SourceGenerator_Architecture.md)
- [Extensibility guide](docs/extensibility-guide.md)
- [Integration tutorial](docs/integration-extension-tutorial.md)
- [Contributing](CONTRIBUTING.md)

## Repository layout

- `src/` — production packages
- `tests/` — automated tests and NuGet smoke validation
- `samples/` — runnable example projects
- `docs/` — canonical documentation

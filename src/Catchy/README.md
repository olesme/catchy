# Catchy

[![NuGet](https://img.shields.io/nuget/v/Catchy.svg)](https://www.nuget.org/packages/Catchy)
[![CI](https://github.com/olesme/catchy/actions/workflows/ci.yml/badge.svg)](https://github.com/olesme/catchy/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/olesme/catchy/blob/main/LICENSE)

Async-native fluent assertion library for .NET — real soft assertions, observability hooks, and source-generator-powered typed surfaces.

```sh
dotnet add package Catchy
dotnet add package Catchy.XUnit   # or .NUnit / .MSTest / .TUnit / .ReqnrollPlugin
```

## Entry points

```csharp
using static Catchy.Stateless;       // Assert  (hard, global singleton)
using static Catchy.Ambient;         // Assert  (hard) + Assert.Soft (soft, per-test)
using static Catchy.AmbientSoft;     // Verify  (soft only)
using static Catchy.StatelessAlias;  // Check   (when Assert conflicts with a framework)
```

## Hard assertions

```csharp
using static Catchy.Stateless;

await Assert.That(user.Name).IsNotNull();
await Assert.That(order.Total).IsGreaterThan(0m).Because("free orders go through promotions");
await Assert.That(tags).Contains("urgent").And().HasCount(3);
```

## Soft assertions — all failures collected, reported together

```csharp
using static Catchy.AmbientSoft;

await Verify.That(order.Id).IsGreaterThan(0);
await Verify.That(order.Total).IsGreaterThan(0m);
await Verify.That(order.Status).IsNotEmpty();
// AggregateAssertionException thrown on flush (auto via framework package, or manual)
```

## Mixed hard and soft

```csharp
using static Catchy.Ambient;

await Assert.That(order.Id).IsGreaterThan(0);            // hard — throws immediately
await Assert.Soft.That(order.Total).IsGreaterThan(0m);   // soft — accumulates
await Assert.Soft.That(order.Status).IsNotEmpty();        // soft — accumulates
```

## Quantifiers

```csharp
await Assert.ThatEachOf(items).IsNotNull();
await Assert.ThatAnyOf(statuses).Is("active");
await Assert.ThatNoneOf(errors).IsNotEmpty();
```

## Observability hooks

```csharp
var assert = Asserter.NewStateful(cfg =>
{
    cfg.OnAssertion   = [async info => await LogAssertion(info)];
    cfg.OnSoftFailure = [async info => await CaptureScreenshot(info)];
});

await assert.That(pageTitle).Contains("Dashboard");
await assert.Soft.That(userName).Is("admin");
```

## Why Catchy?

- **Async-native.** Every chain is `await`ed — async hooks with no threading workarounds.
- **Real soft assertions.** Interleave soft and hard; pass failure state across helpers, step definitions, and DI.
- **No IntelliSense pollution.** `Assert.That(value)` — not `.Should()` on every type.
- **Source-generator extensibility.** Add `[Assertable]` to your type, get a full typed assertion surface generated.
- **Trailing modifiers.** `.Because("reason").IgnoringCase()` after the assertion — reads like a sentence.
- **Conflict-free aliases.** Built-in `Check` / `Verify` and support for custom entry points.
- **MIT, permanently.**

> **Pre-1.0 — API will change.**
> [Full docs](https://github.com/olesme/catchy) · [Usage guide](https://github.com/olesme/catchy/blob/main/docs/USAGE_GUIDE.md) · [Contributing](https://github.com/olesme/catchy/blob/main/CONTRIBUTING.md)

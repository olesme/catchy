# Catchy.NUnit

[![NuGet](https://img.shields.io/nuget/v/Catchy.NUnit.svg)](https://www.nuget.org/packages/Catchy.NUnit)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/olesme/catchy/blob/main/LICENSE)

NUnit integration for Catchy — per-test ambient asserter with automatic soft-assertion flush on teardown.

```sh
dotnet add package Catchy
dotnet add package Catchy.NUnit
```

## Setup

Derive your test class from `AmbientNUnitBase`:

```csharp
using Catchy;
using Catchy.NUnit;
using static Catchy.Ambient;

[TestFixture]
public class OrderTests : AmbientNUnitBase
{
    [Test]
    public async Task Order_is_valid()
    {
        var order = GetOrder();

        // Hard — throws immediately on failure
        await Assert.That(order.Id).IsGreaterThan(0);

        // Soft — accumulates, flushed automatically at end of test
        await Assert.Soft.That(order.Total).IsGreaterThan(0m);
        await Assert.Soft.That(order.Status).IsNotEmpty();
    }
}
```

`AmbientNUnitBase` sets up the ambient provider and flushes accumulated soft failures in teardown. No extra configuration needed.

## Conflict-free alias

NUnit ships its own `Assert`. Use the built-in alias when both are in scope:

```csharp
using static Catchy.StatelessAlias;  // Check instead of Assert
using static Catchy.AmbientAlias;    // Check.Soft instead of Assert.Soft

await Check.That(order.Id).IsGreaterThan(0);
await Check.Soft.That(order.Total).IsGreaterThan(0m);
```

> **Pre-1.0 — API will change.**
> [Full docs](https://github.com/olesme/catchy) · [Usage guide](https://github.com/olesme/catchy/blob/main/docs/USAGE_GUIDE.md)

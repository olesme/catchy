# Catchy.MSTest

[![NuGet](https://img.shields.io/nuget/v/Catchy.MSTest.svg)](https://www.nuget.org/packages/Catchy.MSTest)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/olesme/catchy/blob/main/LICENSE)

MSTest integration for Catchy — per-test ambient asserter with automatic soft-assertion flush on teardown.

```sh
dotnet add package Catchy
dotnet add package Catchy.MSTest
```

## Setup

Derive your test class from `AmbientMSTestBase`:

```csharp
using Catchy;
using Catchy.MSTest;
using static Catchy.Ambient;

[TestClass]
public class OrderTests : AmbientMSTestBase
{
    [TestMethod]
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

`AmbientMSTestBase` uses `TestContext.Properties` for scope and flushes accumulated soft failures in cleanup. No extra configuration needed.

## Conflict-free alias

MSTest ships its own `Assert`. Use the built-in alias when both are in scope:

```csharp
using static Catchy.StatelessAlias;  // Check instead of Assert
using static Catchy.AmbientAlias;    // Check.Soft instead of Assert.Soft

await Check.That(order.Id).IsGreaterThan(0);
await Check.Soft.That(order.Total).IsGreaterThan(0m);
```

> **Pre-1.0 — API will change.**
> [Full docs](https://github.com/olesme/catchy) · [Usage guide](https://github.com/olesme/catchy/blob/main/docs/USAGE_GUIDE.md)

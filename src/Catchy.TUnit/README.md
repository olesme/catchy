# Catchy.TUnit

[![NuGet](https://img.shields.io/nuget/v/Catchy.TUnit.svg)](https://www.nuget.org/packages/Catchy.TUnit)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/olesme/catchy/blob/main/LICENSE)

TUnit integration for Catchy — per-test ambient asserter via global hooks with automatic soft-assertion flush on teardown.

```sh
dotnet add package Catchy
dotnet add package Catchy.TUnit
```

## Setup

TUnit uses global hooks — no base class required. Add `Catchy.TUnit` and the hooks register automatically.

```csharp
using Catchy;
using static Catchy.Ambient;

public class OrderTests
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

`CatchyTUnitHooks` registers before/after-test hooks globally. The ambient provider uses `ThreadLocal` (TUnit runs multiple async contexts per test). Soft failures are flushed automatically after each test.

> **Pre-1.0 — API will change.**
> [Full docs](https://github.com/olesme/catchy) · [Usage guide](https://github.com/olesme/catchy/blob/main/docs/USAGE_GUIDE.md)

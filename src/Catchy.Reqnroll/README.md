# Catchy.ReqnrollPlugin

[![NuGet](https://img.shields.io/nuget/v/Catchy.ReqnrollPlugin.svg)](https://www.nuget.org/packages/Catchy.ReqnrollPlugin)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/olesme/catchy/blob/main/LICENSE)

Reqnroll runtime plugin for Catchy — scenario-scoped ambient asserter with automatic soft-assertion flush at scenario teardown.

```sh
dotnet add package Catchy
dotnet add package Catchy.ReqnrollPlugin
```

## Setup

The plugin registers itself automatically via Reqnroll's plugin discovery. Inject `StatefulAsserter` into step classes via the Reqnroll container:

```csharp
using Catchy;
using Reqnroll;

[Binding]
public class OrderSteps
{
    private readonly StatefulAsserter _assert;

    public OrderSteps(StatefulAsserter assert)
    {
        _assert = assert;
    }

    [Then("the order total is greater than zero")]
    public async Task Order_total_is_positive()
    {
        // Hard — throws immediately
        await _assert.That(order.Id).IsGreaterThan(0);

        // Soft — accumulates across steps, flushed at scenario end
        await _assert.Soft.That(order.Total).IsGreaterThan(0m);
        await _assert.Soft.That(order.Status).IsNotEmpty();
    }
}
```

Soft failures accumulated across all steps in a scenario are flushed automatically at scenario teardown. The `FlushAction` is replaced with the Reqnroll reporting sink so failures appear in the Reqnroll report rather than throwing.

## Ambient usage

```csharp
using static Catchy.Ambient;

[Then("the order is valid")]
public async Task Order_is_valid()
{
    await Assert.That(order.Id).IsGreaterThan(0);
    await Assert.Soft.That(order.Total).IsGreaterThan(0m);
}
```

> **Pre-1.0 — API will change.**
> [Full docs](https://github.com/olesme/catchy) · [Usage guide](https://github.com/olesme/catchy/blob/main/docs/USAGE_GUIDE.md)

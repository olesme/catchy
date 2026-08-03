# Catchy.SourceGenerator

[![NuGet](https://img.shields.io/nuget/v/Catchy.SourceGenerator.svg)](https://www.nuget.org/packages/Catchy.SourceGenerator)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/olesme/catchy/blob/main/LICENSE)

Roslyn incremental source generator for Catchy — generates typed assertion surfaces and entry points from attributes.

```sh
dotnet add package Catchy
dotnet add package Catchy.SourceGenerator
```

`Catchy.SourceGenerator` is opt-in. Add it only when you use attribute-driven generation. Consumers who use only precompiled Catchy APIs need only `Catchy`.

## Generate a typed assertion surface for your type

```csharp
using Catchy;

[Assertable]
public class Order
{
    public int Id { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = "";
}
```

The generator emits a typed assertion surface so `Assert.That(order)` produces `OrderAssertions` with members like `.HasId(...)`, `.HasTotal(...)`, `.HasStatus(...)`.

## Author domain assertions with `[AssertFor]`

```csharp
using Catchy;
using Catchy.Sdk;

[AssertFor<Order>]
public static partial class OrderAssertions
{
    [Assertion]
    public static ValueAssertions<Order> IsPaid(this ValueAssertions<Order> a,
        [CallerArgumentExpression(nameof(a))] string? expr = null)
    {
        a.Link("IsPaid", expr);
        a.Op(a => CheckOperation.Sync(
            () => a.GetValue().Status == "paid",
            () => $"Expected order to be paid but was '{a.GetValue().Status}'",
            a.IsSkipped()));
        return a;
    }
}
```

The generator wires the method into the fluent chain automatically.

## Typed overloads with `[GenerateTypedOverloads]`

```csharp
[GenerateTypedOverloads(typeof(int), typeof(long), typeof(decimal))]
public static ValueAssertions<int> IsPositive(this ValueAssertions<int> a, ...)
{ ... }
```

Generates `IsPositive` overloads for every listed type using textual substitution.

> **Pre-1.0 — API will change.**
> [Source generator architecture](https://github.com/olesme/catchy/blob/main/docs/SourceGenerator_Architecture.md) · [Extensibility guide](https://github.com/olesme/catchy/blob/main/docs/extensibility-guide.md)

# Catchy Extensibility Guide

This document describes the supported extensibility mechanisms in Catchy —
from simple extension methods to source-generator-driven entry points and
cross-type structural comparison rules.

> **Chain rendering rule**
> Public chain output must match the user DSL call site 1:1.
> Internal delegated assertions must not leak internal links.
> Use `DelegateTo(...)` for composed checks and render only the outer DSL method.
> Failure source mapping still points to the user call site through normal pipeline capture.

---

## Overview of extensibility mechanisms

For package-level implementation flow and delivery requirements, see [integration-extension-tutorial.md](integration-extension-tutorial.md).

## Source generator package usage (opt-in)

- `Catchy` package contains precompiled runtime/assertion behavior and can be used alone.
- Source generation is optional and requires explicit `Catchy.SourceGenerator` package reference in your project.
- When `Catchy.SourceGenerator` is referenced, it auto-registers itself as an analyzer via package assets (`analyzers` + `buildTransitive`).
- If you do not use generation attributes (`[AssertFor]`, `[Assertable]`, `[GenerateTypedOverloads]`), do not add `Catchy.SourceGenerator`.

Minimal package setup:

```xml
<ItemGroup>
  <PackageReference Include="Catchy" Version="1.0.0" />
  <!-- Add only when you need source generation -->
  <PackageReference Include="Catchy.SourceGenerator" Version="1.0.0" PrivateAssets="all" />
</ItemGroup>
```

| Mechanism | File/attribute | Best for |
| --- | --- | --- |
| [Plain extension method](#1-plain-extension-method) | Any `.cs` file | Single custom assertion on an existing type |
| [Delegating to an existing assertion](#2-delegating-to-an-existing-assertion) | Any `.cs` file | Reusing built-in checks from a more specific context |
| [`[AssertFor]` — assertion class](#3-assertfor--assertion-class) | `[AssertFor(typeof(T))]` on a `static partial` class | A suite of named assertions for one type |
| [`[Assertable]` — full field-level generation](#4-assertable--full-field-level-generation) | `[Assertable]` on the target class | Rich field/member assertions + method-based transitions on `ValueAssertions<T>` |
| [`[Assertable]` cross-type rules](#5-assertable-cross-type-rules) | `CrossType` attribute on `[Assertable]` | Structural comparison between two related types |
| [Cross-type rules for external types](#6-external-cross-type-rules) | `[CrossTypeRule]` | Cross-type rules for types you cannot modify |
| [`[GenerateTypedOverloads]`](#7-generatetypedoverloads) | On a template method | Fan-out a method across multiple numeric/value types |
| [`[AssertEntry]`](#8-assertentry--generic-hierarchy-entry-point) | On a base class | Generic entry point for a class hierarchy |
| [`[AssertVia]`](#9-assertvia--property-based-delegation) | On a class with an inner assertion target | Entry point that extracts a property transparently |

---

## 1. Plain extension method

The most straightforward approach: write a static extension method on
`ValueAssertions<T>` for any `T`.

```csharp
using Catchy;
using Catchy.Sdk;

public static class OrderAssertions
{
    /// <summary>Asserts that the order is in a shippable state.</summary>
    public static ValueAssertions<Order> IsShippable(this ValueAssertions<Order> a)
    {
        a.Link("IsShippable");
        a.Op(_ => CheckOperation.Sync(
            passes:    () => a.GetValue().Status == OrderStatus.Confirmed
                         && a.GetValue().Items.Count > 0,
            failBody:  () => $"Expected order {a.GetValue().Id} to be shippable " +
                             $"(status={a.GetValue().Status}, items={a.GetValue().Items.Count}).",
            isSkipped: a.IsSkipped()));
        return a;
    }
}
```

Usage:

```csharp
await Assert.That(order).IsShippable();
```

---

## 2. Delegating to an existing assertion

You do not always need to reimplement a check from scratch. When the assertion
target exposes a simpler value, compose built-in assertions through `DelegateTo(...)`
so only outer DSL links are rendered.

### Delegation with clean outer chain

Use `DelegateTo(...)` for DSL-style custom assertions so internal delegated links never appear
in the public chain.

```csharp
public static ValueAssertions<Product> HasValidSku(this ValueAssertions<Product> a)
{
    return a.DelegateTo(
        select: p => p.Sku,
        delegated: sku => sku.StartsWith("SKU-"),
        outerMethod: "HasValidSku");
}
```

Usage:

```csharp
await Assert.That(product).HasValidSku();
```

Visible chain stays 1:1 with the user call site while delegated checks still execute and contribute failures.

### Delegating to logic in `[AssertFor]` methods

Define reusable boolean checks in `[AssertFor]` classes and compose them without
duplication:

```csharp
[AssertFor(typeof(InvoiceLineItem))]
public static partial class InvoiceLineAssertions
{
    [Assertion("have a positive quantity")]
    public static bool HasPositiveQuantity(InvoiceLineItem item) => item.Quantity > 0;

    [Assertion("have a non-empty description")]
    public static bool HasDescription(InvoiceLineItem item) =>
        !string.IsNullOrWhiteSpace(item.Description);

    // Reuse domain logic without duplication
    [Assertion("be taxable")]
    public static bool IsTaxable(InvoiceLineItem item) =>
        TaxRules.IsTaxable(item.Category, item.Amount);
}
```

### Complex delegation with multiple properties

When building a more complex assertion from multiple sub-values and keeping only outer chain links:

```csharp
public static ValueAssertions<UserProfile> IsValid(this ValueAssertions<UserProfile> a)
{
    a.DelegateTo(u => u.Email.Address,
        delegated: email => email.Contains("@"),
        outerMethod: "IsValid");

    a.DelegateTo(u => u.Username,
        delegated: username => username.IsNotEmpty().And().HasLength(d => d.Between(3, 20)),
        outerMethod: null); // do not add another outer link

    return a;
}
```

### Required rendering rule for delegation

For custom DSL methods, delegated internals are implementation details.
Visible chain must contain only outer DSL links in code order.

`DelegateTo(...)` reuses existing assertion methods while scoping away internal links,
so failures remain accurate but rendered chain stays clean.

---

## 3. `[AssertFor]` — assertion class

`[AssertFor]` generates an `Asserter.That(T)` entry point and makes each
`[Assertion]`-annotated static `bool`-returning method available as a fluent
assertion method.

Required authoring rule:

- Assertion methods: `[DebuggerHidden, StackTraceHidden, AssertionMethod]`
- Non-assert helper/delegation methods that may throw: `[DebuggerHidden, StackTraceHidden]`

Why:

- `DebuggerHidden` + `StackTraceHidden` keep internal plumbing out of step-through and stack traces.
- `AssertionMethod` marks real assertion operations so pipeline semantics and assertion diagnostics remain correct.

```csharp
public class Temperature
{
    public decimal Celsius { get; set; }
}

[AssertFor(typeof(Temperature))]
public static partial class TemperatureAssertions
{
    [Assertion("be freezing (at or below 0°C)")]
    public static bool IsFreezing(Temperature t) => t.Celsius <= 0;

    [Assertion("be boiling (at or above 100°C)")]
    public static bool IsBoiling(Temperature t) => t.Celsius >= 100;
}
```

Usage:

```csharp
await Assert.That(new Temperature { Celsius = -5 }).IsFreezing();
```

### Generic-attribute shorthand

```csharp
[AssertFor<Humidity>]
public static partial class HumidityAssertions
{
    [Assertion("be comfortable (30–60%)")]
    public static bool IsComfortable(Humidity h) => h.Percent is >= 30 and <= 60;
}
```

### Extensions-only mode (no entry point)

Use `Mode = AssertForGenerationMode.ExtensionsOnly` when an entry point already
exists (e.g. from `[Assertable]`) and you only want to add assertion methods:

```csharp
[AssertFor(typeof(MyModel), Mode = AssertForGenerationMode.ExtensionsOnly)]
public static partial class MyModelExtraAssertions
{
    [Assertion("have a non-empty name")]
    public static bool HasNonEmptyName(MyModel m) => !string.IsNullOrEmpty(m.Name);
}
```

---

## 4. `[Assertable]` — full field-level generation

`[Assertable]` on your domain class generates:

- A typed `That(T)` entry point
- Fluent assertion methods (`HasName`, `HasAge`, etc.) for each `[AssertMember]` field
- Method-based transitions (`.Name()`, `.Address()`, etc.) for nested navigation
- **Default transition return type:** `ValueAssertions<TMember>` (no implicit concrete wrapper transition)
- **Optional per-member override:** `[AssertMember(TransitionType = typeof(...))]`
- Quantified entry points (`ThatEachOf`, `ThatAnyOf`, `ThatNoneOf`)

```csharp
[Assertable]
public class Invoice
{
    [AssertMember]
    public string Number { get; set; } = string.Empty;

    [AssertMember]
    public decimal Total { get; set; }

    [AssertMember]
    public InvoiceStatus Status { get; set; }
}
```

Usage:

```csharp
await Assert.That(invoice)
    .HasNumber("INV-001")
    .And()
    .HasTotal(250m)
    .And()
    .HasStatus(InvoiceStatus.Paid);

// Method-based property transition
await Assert.That(invoice).Status().Is(InvoiceStatus.Paid);
```

### Customising member behaviour

```csharp
[Assertable]
public class Product
{
    [AssertMember(UseStringComparison = true,
                  StringComparison = StringComparison.OrdinalIgnoreCase)]
    public string Sku { get; set; } = string.Empty;

    [AssertMember(MessageFormat = "Expected price {1} but found {0}")]
    public decimal Price { get; set; }

    // Optional: transition return type override for this member only
    [AssertMember(TransitionType = typeof(CustomAddressAssertions))]
    public Address ShippingAddress { get; set; } = new();

    [AssertMember(Skip = true)]   // excluded from generated assertions
    public string InternalId { get; set; } = string.Empty;
}
```

### Targeting a custom base assertion surface

```csharp
[Assertable(BaseAssertionType = typeof(StructuralAssertions<>))]
public class AuditRecord
{
    [AssertMember]
    public string Action { get; set; } = string.Empty;
}
```

`BaseAssertionType` is optional. If omitted, generated assertions use `ValueAssertions<T>`.
When set to `StructuralAssertions<>`, the generated assertions surface includes structural APIs
like `IsEquivalentTo(...)`.

You can also provide other assertion bases (open generic or concrete edge-case types)
as long as constructor expectations match the generated wrapper construction.

---

## 5. `[Assertable]` cross-type rules

Describe a structural comparison mapping between the current class and another
type. The generator emits a module-initializer that registers the rule with the
`DeepEqualRuleRegistry` automatically.

```csharp
[Assertable("MyApp.Contracts.PersonDto")]
public class PersonEntity
{
    [AssertMember(UseStringComparison = true,
                  StringComparison = StringComparison.OrdinalIgnoreCase)]
    public string Name { get; set; } = string.Empty;

    [AssertMember(MapTo = "YearsOld")]   // maps to a differently-named property
    public int Age { get; set; }

    [AssertMember(Skip = true)]          // excluded from cross-type comparison
    public string PasswordHash { get; set; } = string.Empty;
}
```

Usage (no wiring needed — the rule is auto-registered):

```csharp
await Assert.That<object>(entity).IsEquivalentTo(dto);
```

---

## 6. External cross-type rules

Use this when you cannot add `[Assertable]` to the source type (third-party,
generated, or sealed).

```csharp
[CrossTypeRule(
    typeof(OrderEntity), typeof(OrderDto),
    AutoMapFields    = false,
    IgnoreExtraFields = true,
    StringComparison = StringComparison.OrdinalIgnoreCase)]
[CrossTypeMemberMap(
    nameof(OrderEntity.CustomerName), nameof(OrderDto.Name),
    UseStringComparison = true)]
[CrossTypeMemberMap(
    nameof(OrderEntity.Quantity), nameof(OrderDto.Qty))]
public static partial class OrderComparisonRules { }
```

The container class must be `static partial`. The generator emits the same
module-initializer registration as inline `[Assertable]` rules.

---

## 7. `[GenerateTypedOverloads]`

When you write an assertion method for one numeric type and need identical
overloads for other types, annotate the template method and list the target
types. The generator clones the method body, substituting the inferred or
explicit template type.

```csharp
public static partial class NumericAssertions
{
    // Generator clones this for float and decimal, substituting double→float/decimal
    [GenerateTypedOverloads(typeof(float), typeof(decimal))]
    public static ValueAssertions<double> IsApproximately(
        this ValueAssertions<double> a, double expected, double tolerance = 1e-9)
    {
        a.Link("IsApproximately", expected.ToString());
        a.Op(_ => CheckOperation.Sync(
            passes:    () => Math.Abs(a.GetValue() - expected) <= tolerance,
            failBody:  () => $"Expected ~{expected} (±{tolerance}) but found {a.GetValue()}.",
            isSkipped: a.IsSkipped()));
        return a;
    }
}
```

Explicit template type override (when inference is ambiguous):

```csharp
[GenerateTypedOverloads(typeof(long), TemplateType = typeof(int))]
public static ValueAssertions<int> IsPositive(this ValueAssertions<int> a)
{
    // implementation
    return a;
}
```

---

## 8. `[AssertEntry]` — generic hierarchy entry point

`[AssertEntry]` is general-purpose (not UI-specific). Use it when you have a base abstraction with capability-based subtypes.

`[AssertEntry]` on a base class generates:

```csharp
public static ValueAssertions<T> That<T>(this Asserter a, T value /*, caller metadata args */)
    where T : YourBaseType
```

Because `T` remains the concrete subtype, constrained extension methods surface only where they are valid.

### Setup

```csharp
public interface IApprovable
{
    bool IsApproved { get; }
}

public interface IAssignable
{
    string? Assignee { get; }
}

[AssertEntry]
public abstract class WorkflowStep
{
    public string Name { get; init; } = string.Empty;
}

public sealed class ApprovalStep : WorkflowStep, IApprovable
{
    public bool IsApproved { get; init; }
}

public sealed class AssignmentStep : WorkflowStep, IAssignable
{
    public string? Assignee { get; init; }
}
```

### Extension methods with capability constraints

```csharp
public static class WorkflowStepAssertions
{
    public static ValueAssertions<T> IsApproved<T>(this ValueAssertions<T> a)
        where T : WorkflowStep, IApprovable
    {
        a.Link("IsApproved");
        a.Op(_ => CheckOperation.Sync(
            passes:    () => a.GetValue().IsApproved,
            failBody:  () => "Expected step to be approved.",
            isSkipped: a.IsSkipped()));
        return a;
    }

    public static ValueAssertions<T> HasAssignee<T>(this ValueAssertions<T> a)
        where T : WorkflowStep, IAssignable
    {
        a.Link("HasAssignee");
        a.Op(_ => CheckOperation.Sync(
            passes:    () => !string.IsNullOrWhiteSpace(a.GetValue().Assignee),
            failBody:  () => "Expected step to have an assignee.",
            isSkipped: a.IsSkipped()));
        return a;
    }
}
```

### Usage

```csharp
await Assert.That(approvalStep).IsApproved();
await Assert.That(assignmentStep).HasAssignee();
```

### Assembly-level registration (for types you don't own)

```csharp
[assembly: AssertEntry(typeof(ThirdPartyBaseType))]
```

---

## 9. `[AssertVia]` — property-based delegation

`[AssertVia]` is general-purpose delegation through an inner property.

`[AssertVia("PropName")]` generates an entry point that extracts the named
property and returns `ValueAssertions<PropertyType>`. Existing assertions for
that property type become available immediately.

### Setup (`AssertVia`)

```csharp
[AssertVia("Amount")]
public sealed class PricedLine
{
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "USD";
}
```

Generator emits (conceptually):

```csharp
public static ValueAssertions<decimal> That(this Asserter a, PricedLine value /*, caller metadata args */)
{
    var pipeline = a.NewPipeline(/* caller metadata args */);
    return new ValueAssertions<decimal>(pipeline, value.Amount);
}
```

### Usage (`AssertVia`)

```csharp
await Assert.That(line).IsGreaterThan(0m);
await Assert.That(line).IsLessThanOrEqualTo(1000m);
```

### Assembly-level registration (`AssertVia`, for types you don't own)

```csharp
[assembly: AssertVia(typeof(ThirdPartyPricedLine), "Amount")]
```

---

## Combining mechanisms

The mechanisms compose freely. A realistic layered scenario:

```csharp
// 1. Domain model — rich field assertions + cross-type mapping
[Assertable("MyApp.Api.Contracts.OrderResponse")]
public class OrderAggregate
{
    [AssertMember] public string OrderId { get; set; } = string.Empty;
    [AssertMember] public decimal Total { get; set; }
    [AssertMember(Skip = true)] public string InternalRef { get; set; } = string.Empty;
}

// 2. Extra business-rule assertions (no entry point needed — Assertable already provides one)
[AssertFor(typeof(OrderAggregate), Mode = AssertForGenerationMode.ExtensionsOnly)]
public static partial class OrderBusinessRules
{
    [Assertion("be above the minimum order value")]
    public static bool IsAboveMinimum(OrderAggregate o) => o.Total >= 10m;
}

// 3. UI interaction layer — keeps ILocator internal
[AssertEntry]
public abstract class PageComponent
{
    internal ILocator Root { get; }
    protected PageComponent(ILocator root) => Root = root;
}

// 4. Numeric assertion template fan-out
public static partial class PriceAssertions
{
    [GenerateTypedOverloads(typeof(float), typeof(decimal))]
    public static ValueAssertions<double> IsWithinBudget(
        this ValueAssertions<double> a, double budget)
    {
        a.Link("IsWithinBudget", budget.ToString());
        a.Op(_ => CheckOperation.Sync(
            passes:    () => a.GetValue() <= budget,
            failBody:  () => $"Expected value ≤ {budget} but found {a.GetValue()}.",
            isSkipped: a.IsSkipped()));
        return a;
    }
}
```

---

## Working demos

- [`samples/PlaywrightAbstractionsDemo/`](../samples/PlaywrightAbstractionsDemo/) shows `[AssertEntry]` and `[AssertVia]` on a UI abstraction layer (a concrete example of general patterns).
- [`tests/CatchySourceGenTests/`](../tests/CatchySourceGenTests/) covers `[Assertable]`, `[AssertFor]`, cross-type rules, and `[GenerateTypedOverloads]`.
- [`tests/NuGetPackageSmoke/`](../tests/NuGetPackageSmoke/) validates package-consumption behavior for source generation.

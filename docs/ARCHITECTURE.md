# Catchy.Assertions — Architecture & Extension Guide

> Status: living document describing the current architecture.  
> Language: English (technical reference); code examples use C#.

---

## Table of Contents

1. [Assertion pipeline overview](#1-assertion-pipeline-overview)
2. [Checks layer — when to use it](#2-checks-layer--when-to-use-it)
3. [Direct inline assertions — when to skip Checks](#3-direct-inline-assertions--when-to-skip-checks)
4. [Template methods — per-type overload generation](#4-template-methods--per-type-overload-generation)
5. [Bundles — selective API surface via metadata](#5-bundles--selective-api-surface-via-metadata)
6. [Choosing the right approach: decision guide](#6-choosing-the-right-approach-decision-guide)
7. [Source generator rules & constraints](#7-source-generator-rules--constraints)
8. [Extension authoring (user extensions)](#8-extension-authoring-user-extensions)
9. [Extension compatibility notes](#9-extension-compatibility-notes)

---

## 1. Assertion pipeline overview

Every assertion call builds a lazy `AssertionPipeline`. No logic runs until `await` is reached.

```text
await                  // pipeline executes after all operations are appended
Assert.That(value)          // → ValueAssertions<T>
    .IsSomething()          // appends CheckOperation to pipeline
    .And.IsOtherThing()     // chains another operation
    .Because("reason")      // trailing modifier, affects whole chain
    .With(info => Log(info)); // trailing callback/config modifier via With(...)
```

Key types:

| Type | Role |
| --- | --- |
| `AssertionPipeline` | Shared execution context; carries operations, soft/hard mode, logger chain, slots |
| `ValueAssertions<T>` | Per-segment assertion surface; wraps a lazy value provider and a reference to the pipeline |
| `CheckOperation` | Single check unit; holds a predicate and a message factory; can be sync or async |
| `AssertionInfo` | Structured data about an assertion (source location, link chain, etc.) |

`ValueAssertions<T>` is **recreated on type-changing transitions**
(e.g. `.WhoseMessage()` on an exception chain produces a new `ValueAssertions<string>`).
The `AssertionPipeline` is shared for the whole chain.

---

## 2. Checks layer — when to use it

`Checks` (e.g. `NumericChecks`, `StringChecks`) are **static helpers that build `CheckOperation` values**. They are appropriate when:

- The logic is genuinely reusable across multiple assertion methods or assertion types.
- The logic is complex enough (e.g. `IsBetween` with `BetweenOptions`, range clamping) that inlining it would hurt readability in every call site.
- The check must work with both a direct value **and** a `Func<T?>` provider (lazy re-evaluation for polling/retry scenarios) — checks provide both overloads generically.

**Good use of Checks:**

```csharp
// In NumericChecks:
public static CheckOperation IsBetween<T>(
    Func<T?> actualProvider,
    T min,
    T max,
    BetweenOptions opts,
    bool isSkipped)
    where T : struct, IComparable<T>
{
    return CheckOperation.Sync(
        passes: () =>
        {
            var value = actualProvider();
            if (!value.HasValue) return false;
            return value.Value.CompareTo(min) >= 0 && value.Value.CompareTo(max) <= 0;
        },
        failBody: () => $"Expected value to be between {min} and {max}.",
        isSkipped: isSkipped);
}

// In the assertion extension:
public static ValueAssertions<T> IsBetween<T>(this ValueAssertions<T> a, T min, T max)
    where T : struct, IComparable<T>
{
    a.Link("IsBetween", min.ToString(), max.ToString());
    a.Op(x => NumericChecks.IsBetween(() => x.GetValue(), min, max, BetweenOptions.Inclusive, x.IsSkipped()));
    return a;
}
```

**When Checks hurt — avoid them:**

- When the logic is a one-liner that does not benefit from reuse (e.g. `x % 2 == 0`). Wrapping a trivial predicate in a `Check` method just adds indirection without value.
- When only one assertion method uses the logic — it should stay inline.
- When a type-template approach already handles the type variation, adding a polymorphic `Check` overload is a **second layer of duplication to maintain**.

---

## 3. Direct inline assertions — when to skip Checks

Prefer inlining the `CheckOperation.Sync(...)` call directly in the assertion extension when:

- The predicate and message factory are short (one expression each).
- There is no reuse across other assertion methods.
- A template will generate the overloads — adding a `Check` overload per target type
  would mean templating *both* the assertion and the check,
  doubling the generated surface for no benefit.

```csharp
// Template method — logic is inline, no Check layer needed:
[GenerateTypedOverloads(typeof(long), typeof(short), typeof(byte), typeof(uint))]
public static ValueAssertions<int> IsEven(this ValueAssertions<int> a)
{
    a.Link("IsEven");
    a.Op(x => CheckOperation.Sync(
        () => x.GetValue() % 2 == 0,
        () => $"Expected {x.GetValue()} to be even",
        x.IsSkipped()));
    return a;
}
```

---

## 4. Template methods — per-type overload generation

Templates are **hand-authored methods** decorated with `[GenerateTypedOverloads(...)]`
listing concrete target types. The source generator clones each method,
substituting the template type token with each target type token
in the method body, signature, and XML documentation.

### How the generator infers the template type

The template type is inferred from the **method syntax**, not the Roslyn semantic model.
This guarantees that the inferred name matches whatever identifier
the template author wrote in the body
(including aliases and `using`-shortened names):

1. Read `ParameterList.Parameters[0].Type` from the `MethodDeclarationSyntax`.
2. If the first parameter type is `GenericNameSyntax` (i.e. `ValueAssertions<X>`), take `TypeArgumentList.Arguments[0]` as the template type text.
3. If that argument is `NullableTypeSyntax` (i.e. `X?`), unwrap to `X` — nullable and non-nullable templates share the same substitution tokens.
4. Fallback to the Roslyn semantic `ToDisplayString()` path
   only when syntax is unavailable (metadata-only symbols,
   which should never occur because referenced-assembly templates
   are skipped by design).

You can always override inference with `TemplateType = typeof(X)` on the attribute, but it is rarely needed.

### Template substitution rules

The generator performs **textual substitution with word-boundary matching**. Substitution order matters:

1. `TypeParam?` is replaced before bare `TypeParam` — nullable occurrences are handled first.
2. Each target type's full display string is used in the substituted output; no `using` directives are emitted by the generator.

### Template type safety — limitations and responsibilities

Textual substitution is deliberately simple. This means:

| Scenario | Behaviour |
| --- | --- |
| Primitive alias (`int` → `long`) | ✅ Works reliably; alias names match the source text. |
| BCL short name in scope (`BigInteger` via `using System.Numerics;`) | ✅ Works — inferred from syntax, matching the body text exactly. |
| Fully-qualified name (`System.Numerics.BigInteger`) | ⚠️ Works only if the body also uses the fully-qualified name. Prefer short names in templates. |
| Tuple types `(int, string)` | ⚠️ Works if the template type and all body occurrences use the same form. |
| Generic types `List<T>` | ❌ Not reliable — the generator substitutes at the outer token level only; nested generics may produce invalid identifiers. Avoid as template receiver types. |
| Method-specific APIs that do not exist on the target type | ❌ Compile error in generated code. The template author is responsible for ensuring all substituted types support every API called in the body. No runtime guard is generated. |

**Responsibility model:** the template author is responsible for ensuring
that all target types support the operations used in the body.
This is the same model used by C++ templates and T4 texts —
the generator is a mechanical substitution tool, not a type-checker.

Template authors should keep target type sets intentionally narrow and validated in tests.

### When to use templates vs explicit overloads

Use templates when:

- The same logic applies to ≥ 3 types and the only variation is the concrete type token.
- All target types support the same operations (same operator set, same method names).
- The type set is open-ended (users may subscribe additional types later).

Use explicit overloads when:

- Only 2 types exist and the variation is small.
- Target types have materially different APIs (e.g. `ulong` vs `BigInteger` — different divisor parameter types, different operator behaviour for signed/unsigned overflow).
- The method is a one-off for a special type (e.g. `double.IsNaN` has no `decimal` or `BigInteger` equivalent).

---

## 5. Bundles — selective API surface via metadata

A **bundle** is a metadata token (a constant or attribute)
that gates which assertion methods are emitted for which types.
Bundles answer: *"does this type participate in this assertion group?"*

Bundles are the right choice when:

- A coherent set of methods should appear together — a user subscribing to a bundle gets all methods or none of them.
- The set is complete and stable for all subscribers: every subscribed type genuinely supports every method in the bundle.
- You want IntelliSense to surface methods **only** on types that legitimately support them.

Bundles are **not** appropriate when:

- Type parity is incomplete — some types support only a subset
  of the bundle's methods. In that case, there is no guarantee
  of the "subscribe and get everything" contract,
  which is the entire point of bundles.
- The method set is a one-off expansion for a single type.

### Integer numerics — why no IntegerNumeric bundle

The integer numeric assertions (`IsEven`, `IsOdd`, `IsDivisibleBy`, ...) currently do **not** use a bundle, and this is intentional:

- `ulong`, `nuint`, `BigInteger`, `Int128`, `UInt128` have different divisor parameter types or require type-specific signed/unsigned arithmetic.
- A bundle would promise that every subscribed type gets all methods with compatible signatures — that promise cannot be kept here.
- Instead, a **template method** approach is used for the common types
  (`int` → `long`, `short`, `byte`, `sbyte`, `uint`, `ushort`, `nint`),
  and explicit overloads are kept for the exceptions.

When the non-primitive types are eventually unified (e.g. via a `INumber<T>` generic numeric interface on .NET 7+), revisit whether a bundle becomes appropriate.

---

## 6. Choosing the right approach: decision guide

```text
Q1: Does this method apply to ≥ 3 types with identical logic?
  → YES → use a template method.
  → NO  → use an explicit overload (or consider if 2 copies is acceptable).

Q2 (for templates): Do ALL target types support ALL operations used in the method body?
  → YES → proceed.
  → NO  → either split into multiple templates with different target sets,
           or keep explicit overloads for the exceptions.

Q3: Should this method be visible ONLY on types that explicitly opt in?
  → YES → gate with a bundle.
  → NO  → keep it as a standalone extension / template without bundle subscription.

Q4: Is the logic complex (>3 lines, reused across ≥2 methods, needs Func<T?> provider variant)?
  → YES → push logic into a Check helper.
  → NO  → inline in the assertion method body.
```

Summary matrix:

| Scenario | Approach |
| --- | --- |
| Simple 1-liner, many types | Template, inline |
| Complex shared logic, many types | Template + Check |
| Simple 1-liner, 1–2 types | Explicit inline overload |
| Complex shared logic, 1–2 types | Explicit overload + Check |
| Method group with full type parity, IntelliSense selectivity wanted | Bundle + template or bundle + explicit |
| Method group with partial type parity | Template (no bundle) + explicit exceptions |
| Special-type-only method (e.g. `IsNaN` for floating-point) | Explicit overload, inline or Check |

---

## 7. Source generator rules & constraints

1. **Templates are processed only for the currently-compiled assembly.**
   Methods in referenced assemblies are already expanded;
   re-processing them would fail (no syntax)
   and produce duplicate output.

2. **Target types are specified as `typeof(T)` in the attribute.**
   The generator uses `ITypeSymbol.ToDisplayString()`
   for the output type token in the generated signature.
   This produces fully-qualified names for non-BCL types
   (`System.Numerics.BigInteger`), which are valid in the generated C#
   without extra `using` directives.

3. **Template type inference reads syntax.** See §4 above. Do not use `ToDisplayString()` for the template token — it would not match the body text for short-named types.

4. **Nullable target types in attributes** (e.g. `typeof(long?)`) work correctly:
   the generated receiver is `ValueAssertions<long?>` and the template token `int`
   (unwrapped from `ValueAssertions<int?>`) is substituted with `long` throughout,
   including the divisor parameter type. This means divisor parameters
   correctly become `long` for `long?` receivers.

5. **Arity forwarding** (`[GenerateArityOverloads]`) produces additional method overloads with different parameter counts. Arity forwarding is independent of type-template substitution.

6. **Bundle applicability** is metadata-driven and generator-emitted.
   Do not widen extension method receivers to `ValueAssertions<T>` globally
   as a substitute for missing generator bundle support.
   See copilot-instructions for full invariants.

7. **Generated files live in `obj/`.**
   Do not add or maintain source files under `src/Catchy/Assertions/Generated/` manually.
   If a file was there, it must be removed and replaced by generator output.

8. **XML documentation** is cloned and type-tokens in `<typeparam>`, `<param>`, and prose descriptions are substituted alongside the method signature.

---

## 8. Extension authoring (user extensions)

This section describes the extension authoring model for custom assertion types and templates.

To add assertions for a custom type `MyType`:

```csharp
using Catchy;
using Catchy.Sdk;

namespace MyProject.Assertions
{
    public static class MyTypeAssertionsExtensions
    {
        public static ValueAssertions<MyType> IsValid(this ValueAssertions<MyType> a)
        {
            a.Link("IsValid");
            a.Op(x => CheckOperation.Sync(
                () => x.GetValue().IsValid,
                () => $"Expected MyType to be valid, but it was not",
                x.IsSkipped()));
            return a;
        }
    }
}
```

To generate typed overloads across a type family, apply `[GenerateTypedOverloads]` exactly as the built-in templates do — the same generator processes user assemblies.

Reusable SDK helpers (`CheckOperation`, `ExprFormat`, `AssertionInfo`, `AssertionPipeline` slots)
are **public in the `Catchy.Sdk` namespace**
and are part of the stable extension authoring API.

---

## 9. Extension compatibility notes

- Keep extension methods additive and type-specific; avoid broad generic receivers unless behavior is identical across all target types.
- Prefer generator-based typed surfaces for repeated type families to keep IntelliSense focused.
- Validate extension behavior in project tests and, for package scenarios, with package-consumption smoke projects.
- Keep primary docs current-state only.

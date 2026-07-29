# Catchy Source Generator Architecture

## Scope

Catchy uses a Roslyn incremental generator to produce assertion APIs from source attributes during compilation.

Primary attributes in active use:

- `[Assertable]`
- `[AssertFor]` and `[AssertFor<T>]`
- `[GenerateArityOverloads]`
- `[GenerateTypedOverloads]`

## Generation model

### `[Assertable]`

Generates assertion entry surfaces and transitions for attributed types, including quantified entry points where applicable.

Typical generated artifacts include:

- assertable entry wrappers
- member transition/field assertions
- quantified assertion surfaces
- generated arity overloads

### `[AssertFor]`

Generates assertion extension methods from user-authored static partial classes with `[Assertion]` methods.

This enables package consumers to define domain assertions in their own code while keeping fluent chain behavior aligned with core Catchy APIs.

## Packaging and delivery strategy

The generator is delivered as an opt-in package asset, not as consumer-authored analyzer wiring:

1. `Catchy` ships precompiled assertion/runtime behavior in the core package.
2. `Catchy.SourceGenerator` packages analyzer assets under `analyzers/dotnet/cs` and is activated when referenced as a package.
3. Consumers who need attribute-driven generation (`[AssertFor]`, `[Assertable]`, etc.) add `Catchy.SourceGenerator` explicitly.
4. Consumers who only use precompiled Catchy APIs can reference only `Catchy` and avoid generator overhead.

This keeps generator usage explicit while still avoiding manual analyzer project references when the generator package is added.

## Repository implementation points

- Generator project: `src/Catchy.SourceGenerator`
- NuGet smoke validation project: `tests/NuGetPackageSmoke`

## Validation path

Current validation path for package consumption:

1. Pack `Catchy.SourceGenerator` and `Catchy` to a local feed.
2. Build `tests/NuGetPackageSmoke` using only package references.
3. Verify explicit generator opt-in works (for `[AssertFor]`) and access to pre-generated package APIs from `Catchy`.

CI workflow includes this path via local-feed pack + smoke build.

## Documentation policy for generator docs

- Keep this document focused on implemented behavior and package-delivery mechanics.
- Keep generator docs current-state and publish-ready only.

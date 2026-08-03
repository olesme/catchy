# Catchy.Cecil

[![NuGet](https://img.shields.io/nuget/v/Catchy.Cecil.svg)](https://www.nuget.org/packages/Catchy.Cecil)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/olesme/catchy/blob/main/LICENSE)

Catchy integration for [Mono.Cecil](https://github.com/jbevain/cecil) — fluent assertions over .NET assembly IL for scenarios that require compiled-artifact inspection.

```sh
dotnet add package Catchy
dotnet add package Catchy.Cecil
```

## Usage

Use when you need to assert on the emitted IL or metadata of a compiled assembly — for example, verifying that a source generator produced the expected members, or that specific attributes were emitted.

```csharp
using Catchy;
using Catchy.Cecil;
using static Catchy.Stateless;

var assembly = AssemblyDefinition.ReadAssembly("MyLibrary.dll");

await Assert.That(assembly).HasType("MyNamespace.MyGeneratedClass");
await Assert.That(assembly.MainModule.GetType("MyNamespace.MyType"))
    .HasMethod("GeneratedMethod");
```

> **Pre-1.0 — API will change.**
> [Full docs](https://github.com/olesme/catchy) · [Integration tutorial](https://github.com/olesme/catchy/blob/main/docs/integration-extension-tutorial.md)

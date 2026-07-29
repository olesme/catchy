# NuGet package smoke validation

This project validates package-only consumption (no project references):

- `Catchy` is consumed as a NuGet package.
- Consumer-side source generation works for `[AssertFor]` in this project.
- Pre-generated package API is usable (`Check.ThatAnyOf(1, 2, 3)`).

## Package versions

Package versions are not pinned here. Both the `PackageReference` versions below and `<Version>` for
the `src/` packages come from `$(CatchyPackageVersion)` in the root `Directory.Build.props`
(default `0.0.0`), so producer and consumer cannot drift apart.

To smoke-test a specific version, pass the same override to every pack and to the build:

```powershell
dotnet pack src/Catchy/Catchy.csproj -c Debug -o artifacts/smoke-pack -m:1 -p:CatchyPackageVersion=1.2.3
dotnet build tests/NuGetPackageSmoke/NuGetPackageSmoke.csproj -m:1 -p:CatchyPackageVersion=1.2.3 ...
```

## Run locally

All five referenced packages must be in the local feed — restore fails with `NU1101` if any is
missing. A previously populated `~/.nuget/packages` can mask an incomplete feed, so verify on a clean
machine or add `-p:RestorePackagesPath=<temp dir>`.

```powershell
$feed = "artifacts/smoke-pack"
foreach ($p in "Catchy.SourceGenerator","Catchy","Catchy.Cecil","Catchy.Playwright","Catchy.Playwright.Visual") {
    dotnet pack "src/$p/$p.csproj" -c Debug -o $feed -m:1
}
dotnet build tests/NuGetPackageSmoke/NuGetPackageSmoke.csproj -m:1 -p:RestoreAdditionalProjectSources="$(Resolve-Path $feed)"
```

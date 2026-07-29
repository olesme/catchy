param(
	[switch]$SkipBuild,
	[switch]$SkipTests,
	[switch]$SkipMarkdown
)

$ErrorActionPreference = "Stop"

Write-Host "[quality-local] Starting local quality gates..."

if (-not $SkipMarkdown) {
	Write-Host "[quality-local] Markdown lint"
	npx --yes markdownlint-cli2@0.14.0 "README.md" "CONTRIBUTING.md" "docs/**/*.md"
}

if (-not $SkipBuild) {
	Write-Host "[quality-local] Build solution"
	dotnet build CatchyAssertions.slnx -m:1
}

if (-not $SkipTests) {
	Write-Host "[quality-local] Targeted test suites"
	dotnet test tests/CatchyCoreTests/CatchyCoreTests.csproj -c Release -m:1
	dotnet test tests/CatchySourceGenTests/CatchySourceGenTests.csproj -c Release -m:1
}

Write-Host "[quality-local] All selected gates passed."

## Summary

Describe what this PR changes.

## Package scope

- [ ] Core (`Catchy`)
- [ ] Source generator (`Catchy.SourceGenerator`)
- [ ] Integration package (`Catchy.*`)
- [ ] Documentation only

## Quality gates checklist

### Build and tests
- [ ] Solution builds successfully
- [ ] Relevant tests were added/updated
- [ ] Relevant tests pass locally/CI

### Packaging
- [ ] Changed package projects pack successfully
- [ ] Package assets/layout verified (runtime/build/analyzers/buildTransitive)

### NuGet consumption path
- [ ] Local-feed package consumption validated (when package behavior changed)
- [ ] Smoke project path validated (`tests/NuGetPackageSmoke` where applicable)

### Documentation
- [ ] Canonical docs updated in same PR when behavior changed
- [ ] Docs remain current-state (no legacy/archive/status-snapshot references)

## Integration-specific checklist (if applicable)
- [ ] Scope matches approved integration proposal issue
- [ ] Assertion API examples included in docs/tutorial where needed
- [ ] Definition of Done from integration tutorial satisfied

## Notes

Link related issues/proposals and add any reviewer context.

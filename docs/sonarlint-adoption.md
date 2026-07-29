# SonarLint Adoption Guide

This guide defines a practical SonarLint rollout for Catchy.

## Goal

Use SonarLint as early feedback in IDE without creating excessive churn.

## Scope

Apply to all production projects under `src/` and tests under `tests/`.

## Rollout levels

### Level 1 (now)

- Enable SonarLint in IDE.
- Treat high-confidence correctness/safety findings as must-fix in touched code.
- Keep stylistic/low-signal findings as advisory.

### Level 2 (after baseline cleanup)

- Promote selected reliability/security findings to required in PR review.
- Track repeated findings and clean them incrementally per touched area.

### Level 3 (stabilization toward 1.0)

- Establish a stricter repository baseline.
- Require no new critical findings in changed files.

## Practical rules for contributors

- Fix SonarLint issues in files you touch when changes are small and clear.
- Do not run large unrelated refactor sweeps in feature PRs.
- If a finding is noisy/incorrect, document rationale in PR notes.

## Suggested focus categories

- nullability and flow correctness,
- exception handling misuse,
- dead code and unreachable branches,
- allocation/perf footguns in hot paths.

## Integration with quality gates

SonarLint is local IDE feedback and complements, not replaces:

- markdown lint,
- build/test gates,
- package/smoke validation.

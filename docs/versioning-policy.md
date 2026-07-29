# Versioning and Maturity Policy

This policy defines how Catchy evolves API and architecture by public maturity phase.

## Phase A: `0.0.x` to `<0.5.0`

Goals:

- iterate quickly while shaping canonical API,
- keep architecture clean and coherent,
- keep docs current-state only.

Rules:

- breaking API and architecture changes are allowed,
- remove dead/obsolete APIs instead of keeping temporary aliases,
- avoid compatibility shims unless technically unavoidable,
- update canonical docs in the same change set.

## Phase B: `0.5.x` to `<1.0.0`

Goals:

- reduce churn,
- prepare a stable 1.0 contract,
- make migration intent explicit.

Rules:

- do not remove widely used APIs immediately,
- introduce `[Obsolete]` first with clear message,
- include target removal version in obsolete message,
- provide migration notes in docs/changelog.

Recommended obsolete message format:

- `"Use X instead. Planned removal in 1.0."`

## Phase C: `1.0.0+`

Goals:

- stable public API,
- predictable compatibility.

Rules:

- breaking changes only with explicit major-version planning,
- deprecate first, remove later in planned major wave,
- document impact and migration path before removal.

## Legacy handling policy by phase

- `0.0.x` to `<0.5.0`: removal-first cleanup is allowed when it improves API coherence.
- `0.5.x` to `<1.0.0`: keep temporary compatibility paths only via explicit deprecation.
- `1.0.0+`: maintain compatibility discipline; remove deprecated APIs only in planned major releases.

## Release checklist alignment

Before releasing a new maturity phase:

- verify docs and examples match current APIs,
- verify obsolete annotations/messages are accurate,
- verify CI quality gates and smoke path are green.

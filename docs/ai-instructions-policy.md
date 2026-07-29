# AI Instructions Policy (Local vs Repository-Shared)

This document defines where AI-assistant instructions should live and what can be shared.

## 1) Local/private instructions (not committed)

Use local instructions for personal workflow and machine-specific preferences, for example:

- local IDE/extension behavior preferences,
- personal prompt style,
- local tooling paths,
- temporary experiment notes.

Do not commit local/private instructions to repository docs.

## 2) Repository-shared instructions (committed)

Repository-shared instructions should contain only team-relevant engineering rules, for example:

- architecture constraints,
- API design invariants,
- versioning/deprecation policy,
- quality gate expectations,
- documentation policy.

Repository instructions must be:

- vendor-neutral where possible,
- concise and enforceable,
- current-state only.

## 3) What belongs in user-facing docs

User-facing docs (`docs/*.md`) should describe product behavior and usage.

Avoid embedding tool-specific assistant instructions in user docs unless the section is
explicitly about integration patterns and remains useful independent of one AI vendor.

## 4) Agentic/SDD guidance placement

Agentic and spec-driven guidance should be placed in dedicated technical docs
(e.g. `docs/agentic-observability.md`) as workflow patterns and data contracts,
not as personal assistant configuration snippets.

## 5) Change management

When instruction policy changes:

- update this policy,
- update `.github/copilot-instructions.md` if repository rules changed,
- ensure related canonical docs stay consistent.

# Workflow Verification Checklist

Use this checklist after completing any significant mechanic and before continuing
with the next feature branch.

Suggested branch format:

`codex/<mechanic>-workflow-verification`

## Required Checks

- Build passes.
- Smoke tests pass.
- Assertion counts by group are updated in the final report.
- Documentation is updated for the mechanic or workflow being verified.
- No parallel systems were introduced for existing concepts.
- Stateful mechanics have save/restore coverage.
- Registries validate stable ids for definitions and state restoration.
- Old JSON compatibility is preserved when snapshots exist.
- Domain code remains Unity-free.
- No generator-wide rewrites were made unless explicitly requested.
- At least one integrated E2E flow covers the mechanic across its expected boundaries.

## Verification Goal

Detect technical debt and integration problems early before continuing to expand
the backend.

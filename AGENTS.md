# AGENTS.md — Repository entry point

**The sole authoritative project specification is [docs/Fragments_of_Wysdom_Bible.md](docs/Fragments_of_Wysdom_Bible.md).** This file is intentionally a minimal instruction pointer, not a second design/architecture source.

Before work, read the Bible's active design, architecture, implementation ledger, executed audit findings and audit-driven backlog. Historical source records in the Bible preserve technical details but do not override newer verified findings.

## Mandatory execution guardrails

- Follow **Foundation → Workflow Verification → Documentation → Audit → Next Atomic Step**.
- Derive the next atomic branch from the current audit backlog; do not create parallel systems.
- Domain/application owns rules and state; Unity adapts presentation and consumes results/events.
- Preserve serialization compatibility and the legacy 38-event guardrail.
- Do not combine correctness repairs with naming cleanup or unrelated gameplay.
- Require explicit approval before implementation, merging, deployment or destructive repository cleanup.
- Verify build, smoke tests, and diff cleanliness; document any unverified Unity behavior.

See the Bible for all design decisions, code contracts, historical provenance, risks and detailed acceptance criteria.

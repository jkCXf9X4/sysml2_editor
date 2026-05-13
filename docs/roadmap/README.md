# Implementation Workspace

This folder is the implementation entry point for humans and AI agents. It merges the former `docs/plan.md` to centralize implementation planning.

Read these files in order when setting up or changing the project:

1. [Product vision](../../PRODUCT_VISION.md)
2. [Architecture overview](../architecture/README.md)
3. [Root layout](../../README.md) — repository top-level shape
4. [Backend layout](../../src/backend/README.md)
5. [Frontend layout](../../src/frontend/README.md)
6. [Shared contracts layout](../../src/shared/README.md)
7. [Phases](./phases.md) — testing focus and exit criteria by phase
8. [starter-test-matrix.md](../testing/starter-test-matrix.md)
9. [runtime.md](../architecture/runtime.md)
10. [api-contract.md](../implementation/api/api-contract.md)
11. [parser-contract.md](../implementation/parser-contract.md)
12. [model-graph.md](../architecture/model-graph.md)
13. [write-policy.md](../architecture/write-policy.md)
14. [sysml-v2.md](../reference/sysml-v2.md)
15. [syntax-examples.md](../implementation/syntax-examples.md)
16. [roadmap.md](./roadmap.md)

## Documentation Traceability

- Use [PRODUCT_VISION.md](../../PRODUCT_VISION.md) as the central product intent and traceability source.
- Use [../implementation/README.md](../implementation/README.md) for implementation details and contracts.
- Use [../ui/design.md](../ui/design.md) for UI/UX direction derived from the product vision.
- Use [../architecture/README.md](../architecture/README.md) for cross-cutting system decisions.
- Use individual source area READMEs ([../../src/backend/README.md](../../src/backend/README.md), [../../src/frontend/README.md](../../src/frontend/README.md), [../../src/shared/README.md](../../src/shared/README.md)) for repo structure during scaffolding and project setup.
- If the repository layout diverges from these READMEs, update the relevant source area README first so the setup path stays accurate.

## Readiness Source

[starter-test-matrix.md](../testing/starter-test-matrix.md) is the canonical implementation readiness and gate document. It defines the scaffold gate, read-only browser gate, writer gate, and Git diff gate.

## Agent Working Contract

- Follow [working-rules.md](../ai/working-rules.md) for implementation behavior.
- Use architecture documents for system guarantees and implementation documents for concrete contracts.
- Use [starter-test-matrix.md](../testing/starter-test-matrix.md) for implementation readiness and slice gates.

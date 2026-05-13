# Implementation Workspace

This folder is the implementation entry point for humans and AI agents. It merges the former `docs/plan.md` to centralize implementation planning.

Read these files in order when setting up or changing the project:

1. [Product vision](../../PRODUCT_VISION.md)
2. [Architecture overview](../architecture/README.md)
3. [project-structure.md](../project-structure.md)
4. [starter-test-matrix.md](../testing/starter-test-matrix.md)
5. [runtime.md](../architecture/runtime.md)
6. [api-contract.md](../implementation/api-contract.md)
7. [parser-contract.md](../implementation/parser-contract.md)
8. [model-graph.md](../architecture/model-graph.md)
9. [write-policy.md](../architecture/write-policy.md)
10. [sysml-v2.md](../reference/sysml-v2.md)
11. [syntax-examples.md](../implementation/syntax-examples.md)
12. [roadmap.md](./roadmap.md)

## Documentation Traceability

- Use [PRODUCT_VISION.md](../../PRODUCT_VISION.md) as the central product intent and traceability source.
- Use [../implementation/README.md](../implementation/README.md) for implementation details and contracts.
- Use [../ui/design.md](../ui/design.md) for UI/UX direction derived from the product vision.
- Use [../architecture/README.md](../architecture/README.md) for cross-cutting system decisions.
- Use [project-structure.md](../project-structure.md) during repo scaffolding and project setup.

## Readiness Source

[starter-test-matrix.md](../testing/starter-test-matrix.md) is the canonical implementation readiness and gate document. It defines the scaffold gate, read-only browser gate, writer gate, and Git diff gate.

## Agent Working Contract

- Follow [working-rules.md](../ai/working-rules.md) for implementation behavior.
- Use architecture documents for system guarantees and implementation documents for concrete contracts.
- Use [starter-test-matrix.md](../testing/starter-test-matrix.md) for implementation readiness and slice gates.

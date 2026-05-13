# Implementation Workspace

This folder is the implementation entry point for humans and AI agents.

Read these files in order when setting up or changing the project:

1. [project-structure.md](./project-structure.md)
2. [starter-test-matrix.md](./starter-test-matrix.md)
3. [runtime-decision.md](./runtime-decision.md)
4. [api-contract.md](./api-contract.md)
5. [parser-contract.md](./parser-contract.md)
6. [model-schema.md](./model-schema.md)
7. [write-policy.md](./write-policy.md)
8. [spec-reference.md](./spec-reference.md)
9. [syntax-examples.md](./syntax-examples.md)
10. [plan.md](./plan.md)

## Current Build Direction

- Runtime: local web app
- Backend: ASP.NET Core on `localhost`
- Frontend: React + TypeScript
- Parser: MVP custom subset parser
- Syntax origin: OMG SysML v2 reference, scoped by local MVP examples
- Git integration: backend-owned Git CLI wrapper
- Model: stable graph of nodes, edges, files, source ranges, and lifecycle state
- Save policy: deterministic owning-file writes only

These bullets are a summary only. The linked decision documents are authoritative when details differ.

## Readiness Source

[starter-test-matrix.md](./starter-test-matrix.md) is the canonical implementation readiness and gate document. It defines the scaffold gate, read-only browser gate, writer gate, and Git diff gate.

## Agent Working Contract

- Follow [working-rules.md](../ai/working-rules.md) for implementation behavior.
- Use decision docs in this folder as contracts.
- Use [starter-test-matrix.md](./starter-test-matrix.md) for implementation readiness and slice gates.

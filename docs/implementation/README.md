# Implementation Workspace

This folder is the implementation entry point for humans and AI agents.

Read these files in order when setting up or changing the project:

1. [readiness-checklist.md](./readiness-checklist.md)
2. [project-structure.md](./project-structure.md)
3. [spec-reference.md](./spec-reference.md)
4. [syntax-examples.md](./syntax-examples.md)
5. [runtime-decision.md](./runtime-decision.md)
6. [api-contract.md](./api-contract.md)
7. [parser-contract.md](./parser-contract.md)
8. [model-schema.md](./model-schema.md)
9. [write-policy.md](./write-policy.md)
10. [starter-test-matrix.md](./starter-test-matrix.md)
11. [plan.md](./plan.md)

## Current Build Direction

- Runtime: local web app
- Backend: ASP.NET Core on `localhost`
- Frontend: React + TypeScript
- Parser: MVP custom subset parser
- Syntax origin: OMG SysML v2 reference, scoped by local MVP examples
- Git integration: backend-owned Git CLI wrapper
- Model: stable graph of nodes, edges, files, source ranges, and lifecycle state
- Save policy: deterministic owning-file writes only

## Agent Working Contract

- Follow [working-rules.md](../ai/working-rules.md) for implementation behavior.
- Use decision docs in this folder as contracts.
- Use [readiness-checklist.md](./readiness-checklist.md) for the first implementation slice.

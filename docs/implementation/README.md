# Implementation Workspace

This folder is the implementation entry point for humans and AI agents.

Read these files in order when setting up or changing the project:

1. [Product vision](../../PRODUCT_VISION.md)
2. [project-structure.md](./project-structure.md)
3. [starter-test-matrix.md](./starter-test-matrix.md)
4. [runtime-decision.md](./runtime-decision.md)
5. [api-contract.md](./api-contract.md)
6. [parser-contract.md](./parser-contract.md)
7. [model-schema.md](./model-schema.md)
8. [write-policy.md](./write-policy.md)
9. [spec-reference.md](./spec-reference.md)
10. [syntax-examples.md](./syntax-examples.md)
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

These bullets are a summary only. The linked decision documents are authoritative when details differ.

## Vision Trace

All implementation decisions must trace back to [PRODUCT_VISION.md](../../PRODUCT_VISION.md). If a decision optimizes for implementation speed, parser scope, or runtime simplicity, the tradeoff must be explicit and must not weaken the core product promise: visual modeling backed by precise, traceable, reviewable SysML text in Git.

## Readiness Source

[starter-test-matrix.md](./starter-test-matrix.md) is the canonical implementation readiness and gate document. It defines the scaffold gate, read-only browser gate, writer gate, and Git diff gate.

## Agent Working Contract

- Follow [working-rules.md](../ai/working-rules.md) for implementation behavior.
- Use decision docs in this folder as contracts.
- Use [starter-test-matrix.md](./starter-test-matrix.md) for implementation readiness and slice gates.

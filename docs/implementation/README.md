# Implementation

Implementation contracts, API specs, parser rules, syntax examples, and current build direction. Derived from the former `docs/design.md`.

## Contents

- [API Contract](./api-contract.md) — Contract overview, ownership, path and status rules
- [Core DTOs](./core-dtos.md) — All DTO definitions with JSON examples
- [Session and context models](./session-models.md) — Repository session, workspace context, multi-context view model
- [Endpoints](./endpoints.md) — Full endpoint contracts with behavior and errors
- [Deferred contracts](./deferred-contracts.md) — Future DTOs and endpoints reserved for later slices
- [Parser Contract](./parser-contract.md)
- [Syntax Examples](./syntax-examples.md)

## Current Implementation Direction

- Runtime: local web app
- Backend: ASP.NET Core on `localhost`
- Frontend: React + TypeScript
- Parser: MVP custom subset parser
- Syntax origin: OMG SysML v2 reference, scoped by local MVP examples
- Git integration: backend-owned Git CLI wrapper
- Model: stable graph of nodes, edges, files, source ranges, and lifecycle state
- Save policy: deterministic owning-file writes only

These bullets are a summary only. The linked decision documents are authoritative when details differ.

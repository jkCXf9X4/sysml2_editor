# Implementation

Implementation contracts, API specs, parser rules, syntax examples, and current product scope. Derived from the former `docs/design.md`.

### API Contracts (`api/`)

- [API Contract](./api/api-contract.md) — Contract overview, ownership, path and status rules
- [Core DTOs](./api/core-dtos.md) — All DTO definitions with JSON examples
- [Session and context models](./api/session-models.md) — Repository session, workspace context, multi-context view model
- [Endpoints](./api/endpoints.md) — Full endpoint contracts with behavior and errors
- [Deferred contracts](./api/deferred-contracts.md) — Future DTOs and endpoints reserved for later slices

### Model Graph (`graph/`)

- [Model graph](./graph/model-graph.md) — Entry point with links to sub-documents
- [Graph entities](./graph/graph-entities.md) — Core entity field specs (context, node, edge, source range, trace link, multi-context view)
- [Graph code generation](./graph/graph-codegen.md) — JSON serialization, C# records, TypeScript interfaces
- [Stable IDs](./graph/stable-ids.md) — ID annotation format, persistence rules, derived fallback IDs
- [Graph traceability](./graph/graph-traceability.md) — Trace link derivation rules
- [Graph supplemental](./graph/graph-supplemental.md) — File records, opaque spans, diagnostics, lifecycle state, derived views

### Other

- [Parser Contract](./parser-contract.md)
- [Syntax Examples](./syntax-examples.md)

## Current Implementation Direction

- Runtime: local web app
- Backend: ASP.NET Core on `localhost`
- Frontend: React + TypeScript
- Parser: supported SysML subset parser
- Syntax origin: OMG SysML v2 reference, scoped by the supported syntax examples
- Git integration: backend-owned Git CLI wrapper
- Model: stable graph of nodes, edges, files, source ranges, and lifecycle state
- Save policy: deterministic owning-file writes only

These bullets are a summary only. The linked decision documents are authoritative when details differ.

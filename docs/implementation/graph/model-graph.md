# Model Graph Implementation

Backend C# records are canonical for the Phase 1 API path in the [implementation roadmap](../../roadmap/roadmap.md). Frontend types should be generated from OpenAPI as described in [api-contract.md](../api/api-contract.md).

## Documents

| Document | Content |
| --- | --- |
| [Entities](./graph-entities.md) | Field specs for ModelContext, Node, Edge, SourceRange, TraceLink, MultiContextView, kind enums |
| [Code generation](./graph-codegen.md) | JSON serialization rules, C# record shapes, TypeScript interfaces |
| [Stable IDs](./stable-ids.md) | ID annotation format, persistence rules, derived fallback IDs |
| [Traceability](./graph-traceability.md) | Trace link derivation for first and later slices |
| [Supplemental](./graph-supplemental.md) | File records, opaque spans, diagnostics, lifecycle state, derived views |

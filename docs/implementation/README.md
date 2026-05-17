# Implementation

Implementation contracts, parser rules, syntax examples, and current product scope. Implementation documents link upward to implementation decisions.

## Governing Implementation Decisions

- [IDEC-001: Local web runtime](../implementation-decisions/IDEC-001-local-web-runtime.md)
- [IDEC-002: Supported SysML subset](../implementation-decisions/IDEC-002-supported-sysml-subset.md)
- [IDEC-003: Stable model graph schema](../implementation-decisions/IDEC-003-stable-model-graph-schema.md)
- [IDEC-004: Deterministic write policy](../implementation-decisions/IDEC-004-deterministic-write-policy.md)
- [IDEC-005: SysML-native identity metadata](../implementation-decisions/IDEC-005-sysml-native-identity-metadata.md)

## Documents

- [Stable IDs](./graph/stable-ids.md) — ID annotation format, persistence rules, derived fallback IDs
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

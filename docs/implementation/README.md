# Implementation

Implementation contracts, parser rules, syntax examples, and current product scope. Implementation documents link upward to technical decisions.

## Governing Technical Decisions

- [TDEC-001: Local web runtime](../technical-decisions/TDEC-001-local-web-runtime.md)
- [TDEC-002: Supported SysML subset](../technical-decisions/TDEC-002-supported-sysml-subset.md)
- [TDEC-003: Stable model graph schema](../technical-decisions/TDEC-003-stable-model-graph-schema.md)
- [TDEC-004: Deterministic write policy](../technical-decisions/TDEC-004-deterministic-write-policy.md)
- [TDEC-005: SysML-native identity metadata](../technical-decisions/TDEC-005-sysml-native-identity-metadata.md)

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

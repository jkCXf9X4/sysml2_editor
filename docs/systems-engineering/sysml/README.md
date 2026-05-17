# SysML Development Overview Model

This folder contains a derived SysML v2-oriented overview model for `sysml2_editor`.

The source definitions live in the markdown files under [docs/systems-engineering](../). This SysML model is a development navigation artifact. It is not constrained by the application's current MVP SysML parser and is not a fixture for parser tests.

## Purpose

- Mirror the markdown systems-engineering hierarchy in a model-oriented form.
- Port the important structure from `docs/architecture`, `docs/functionality`, `docs/testing`, and `docs/systems-engineering`.
- Make the main breakdown easier to inspect: product definition, high-level design, detailed design, and verification.
- Keep implementation status in a parallel structure that references the main breakdown.
- Provide a candidate source for future diagrams or model-based planning views.

## Entry Point

Start with [00-overview/project-overview.sysml](./00-overview/project-overview.sysml).

The main breakdown follows this reading path:

- [00-overview/](./00-overview/) - one entry point and top-level trace path
- [01-product-definition/](./01-product-definition/) - product definition, use cases, and requirements
- [02-high-level-design/](./02-high-level-design/) - architecture, functional breakdown, and product breakdown
- [03-detailed-design/](./03-detailed-design/) - implementation details, runtime contracts, graph contracts, and write-policy constraints
- [04-verification/](./04-verification/) - verification evidence groups

The parallel status track is outside the main breakdown:

- [90-implementation-status/](./90-implementation-status/) - product capability scope and current implementation status, referencing the main breakdown

## Maintenance Rule

When the source docs change materially, update the matching SysML package in the same change:

- Use-case changes update `use-cases.sysml`.
- Functional breakdown changes update `functions.sysml`.
- Product breakdown changes update `product-breakdown.sysml`.
- High-level architecture changes update `02-high-level-design/architecture.sysml`.
- Implementation detail changes update `03-detailed-design/implementation-details.sysml`.
- Functionality status changes update `90-implementation-status/capabilities.sysml`.
- Product requirement changes update `requirements.sysml`.
- Verification strategy or evidence changes update `verification.sysml`.

Prefer contextual owned usages for file-local concepts that do not need their own standalone definition. Leave only cross-file leaf concepts at package scope.

Keep the SysML model at overview level. Do not copy every implementation detail into this folder, and do not treat the SysML files as more authoritative than the markdown source definitions.

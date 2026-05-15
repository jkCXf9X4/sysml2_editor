# Systems Engineering Basis

This folder turns the product vision into analysis artifacts that can be decomposed into product definition, high-level design, detailed design, and verification.

Implementation status is held in a separate parallel track that references the main breakdown.

## How The Layers Relate

```text
main breakdown:
product definition -> high-level design -> detailed design -> verification

parallel status track:
implementation status -> references product definition, requirements, design, and verification
```

The markdown files in this folder are the source definitions. The SysML folder is a derived development overview model and should not become the primary reading surface unless it becomes easier to maintain than the markdown.

## Working Rules

- Use cases describe user goals and externally visible outcomes.
- Requirements define testable product obligations derived from the use cases.
- High-level design groups what the product must do, which major product blocks provide it, and the architecture-level choices that shape the system.
- Detailed design captures implementation-facing contracts, APIs, graph rules, parser rules, write policy, runtime, and similar design details.
- Implementation status is not part of the main breakdown. It tracks current capability maturity by referencing the product definition, requirements, design, and verification.
- Verification links requirements to current or planned evidence.
- The roadmap decides build order; it does not redefine the systems-engineering basis.

## Level Overview

### 00 Overview

Purpose: give one entry point for understanding the full project shape.

Current source: this README.

Derived SysML: [sysml/00-overview/](./sysml/00-overview/).

### 01 Product Definition

Purpose: explain why the product exists, who it serves, what users must accomplish, and what product obligations follow from that intent.

Source definitions:

- [Product vision](../../PRODUCT_VISION.md)
- [use-cases.md](./use-cases.md)
- [product-requirements.md](./product-requirements.md)

Derived SysML: [sysml/01-product-definition/](./sysml/01-product-definition/).

### 02 High-Level Design

Purpose: define the product's major functions, product blocks, and high-level architecture without implementation detail.

Source definitions:

- [functional-breakdown.md](./functional-breakdown.md)
- [product-breakdown.md](./product-breakdown.md)
- [architecture overview](../architecture/README.md)
- [overall design](../architecture/overall_design.md)

Derived SysML: [sysml/02-high-level-design/](./sysml/02-high-level-design/).

### 03 Detailed Design

Purpose: capture implementation-facing design details and constraints that implement the high-level architecture.

Source definitions:

- [runtime decision](../architecture/runtime.md)
- [model graph](../architecture/model-graph.md)
- [write policy](../architecture/write-policy.md)
- [implementation contracts](../implementation/README.md)

Derived SysML: [sysml/03-detailed-design/](./sysml/03-detailed-design/).

### 04 Verification

Purpose: connect requirements and design choices to current or planned evidence.

Source definitions:

- [testing strategy](../testing/test-strategy.md)
- [coverage tracker](../testing/coverage-tracker.md)
- [MVP coverage](../testing/mvp-coverage.md)

Derived SysML: [sysml/04-verification/](./sysml/04-verification/).

## Parallel Implementation Status

Purpose: show the full product capability scope and how much of it is currently available, partial, or future.

Source definitions:

- [functionality index](../functionality/README.md)
- [fixtures and risk gaps](../functionality/fixtures-and-risks.md)
- [roadmap](../roadmap/roadmap.md)

Derived SysML: [sysml/90-implementation-status/](./sysml/90-implementation-status/).

Implementation status is deliberately outside the main breakdown. It references the main breakdown; it does not sit inside it and does not redefine product scope.

## Document Map

- [use-cases.md](./use-cases.md) - product-purpose use cases with trace links back to the vision and forward links to current docs
- [product-requirements.md](./product-requirements.md) - atomic product requirements derived from product intent
- [functional-breakdown.md](./functional-breakdown.md) - high-level functional groups derived from use cases and requirements
- [product-breakdown.md](./product-breakdown.md) - high-level product and subsystem decomposition derived from the functional breakdown
- [sysml/](./sysml/) - derived SysML v2-oriented development overview model

## Trace Rule

Every material product decision should be traceable through this chain:

```text
main breakdown:
Vision pillar -> Use case -> Requirement -> High-level design -> Detailed design -> Verification evidence

parallel implementation status:
Implementation status -> references Product definition, Requirements, High-level design, Detailed design, Verification evidence
```

If a decision skips a layer, the reason should be explicit.

## Maintenance Rule

Update the markdown source definition first. Update the derived SysML overview only when it helps preserve or inspect the hierarchy.

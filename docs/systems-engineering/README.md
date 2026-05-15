# Systems Engineering Basis

This folder turns the product vision into analysis artifacts that can be decomposed into use cases, a functional breakdown, a product breakdown, and product requirements.

## How The Layers Relate

```text
Product vision -> Use cases -> Functional breakdown -> Product breakdown -> Product requirements -> Roadmap and tests
```

## Working Rules

- Use cases describe user goals and externally visible outcomes.
- The functional breakdown groups what the product must do.
- The product breakdown groups the product subsystems and deliverables.
- Product requirements are atomic, testable statements traced back to use cases.
- The roadmap decides build order; it does not redefine the basis.

## Document Map

- [use-cases.md](./use-cases.md) - Product-purpose use cases with trace links back to the vision and forward links to the current docs
- [functional-breakdown.md](./functional-breakdown.md) - Functional groups derived from the use cases
- [product-breakdown.md](./product-breakdown.md) - Product and subsystem decomposition derived from the functional breakdown
- [product-requirements.md](./product-requirements.md) - Atomic product requirements and verification hooks

## Trace Rule

Every material product decision should be traceable through this chain:

```text
Vision pillar -> Use case -> Functional block -> Product block -> Requirement -> Test
```

If a decision skips a layer, the reason should be explicit.

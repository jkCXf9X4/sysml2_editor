# Documentation

This folder contains the project documentation, organized by abstraction level to minimize duplication and improve navigation. The top-level [README.md](../README.md) links here as the single entry point, and [PRODUCT_VISION.md](../PRODUCT_VISION.md) captures the product intent.

## Trace Layers

Trace links point upward only:

```text
Product Vision <- Product Decisions <- Architecture <- Implementation Decisions <- Implementation <- Testing
```

Higher layers describe intent and constraints without linking down into lower-level implementation details. Lower layers name the decisions or contracts they satisfy.

## Principles

- Product vision is the source for product decisions
- Architecture describes stable system guarantees, not implementation mechanics
- Implementation decisions bridge architecture to build details
- Frontend and backend boundaries are explicit
- Domain logic is separate from transport and UI
- Tests live close to the behavior they verify
- Fixtures are checked in and deterministic

## Folder Structure

- [product-decisions/](./product-decisions/) - Accepted decisions that translate product vision into architecture.
- [architecture/](./architecture/) - Stable system concepts, boundaries, invariants, and guarantees.
- [implementation-decisions/](./implementation-decisions/) - Accepted decisions that translate architecture into concrete build choices.
- [implementation/](./implementation/) - Contracts, parser rules, graph specs, and syntax examples governed by implementation decisions.
- [testing/](./testing/) - Test strategy, fixtures, and end-to-end coverage focused on text fidelity, visual editing, and Git behavior.
- [ui/](./ui/) - User interface design, workflows, and feature specifications.
- [reference/](./reference/) - External standards and language references.

## Navigation

1. Start here (docs/README.md)
2. Follow links to the relevant subfolder for your needs
3. Each subfolder's README.md provides detailed content and links to its files

Cross-reference between folders uses links, not content duplication.

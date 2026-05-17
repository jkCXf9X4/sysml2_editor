# Documentation

This folder contains the project documentation, organized by abstraction level to minimize duplication and improve navigation. The top-level [README.md](../README.md) links here as the single entry point, and [PRODUCT_VISION.md](../PRODUCT_VISION.md) captures the product intent.

## Trace Layers

The documentation chain moves from intent to proof:

```text
Intent -> Product Commitments -> System Architecture -> Technical Decisions -> Implementation -> Verification
```

Trace links point backward to the layer being satisfied. Higher layers describe intent and constraints without linking down into lower-level implementation details.

## Principles

- Product vision captures intent
- Product commitments translate intent into durable product promises
- System architecture describes stable guarantees, concepts, and boundaries
- Technical decisions bridge architecture to build details
- Frontend and backend boundaries are explicit
- Domain logic is separate from transport and UI
- Tests live close to the behavior they verify
- Fixtures are checked in and deterministic

## Folder Structure

- [product-commitments/](./product-commitments/) - Accepted commitments that translate product vision into architecture.
- [architecture/](./architecture/) - System architecture: stable concepts, boundaries, invariants, and guarantees.
- [technical-decisions/](./technical-decisions/) - Accepted technical decisions that translate architecture into concrete build choices.
- [implementation/](./implementation/) - Contracts, parser rules, graph specs, and syntax examples governed by technical decisions.
- [testing/](./testing/) - Verification strategy, fixtures, and end-to-end coverage focused on text fidelity, visual editing, and Git behavior.
- [ui/](./ui/) - User interface design, workflows, and feature specifications.
- [reference/](./reference/) - External standards and language references.

## Navigation

1. Start here (docs/README.md)
2. Follow links to the relevant subfolder for your needs
3. Each subfolder's README.md provides detailed content and links to its files

Cross-reference between folders uses links, not content duplication.

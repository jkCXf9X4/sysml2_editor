# Documentation

This folder contains all project documentation, organized by abstraction level to minimize duplication and improve navigation. The top-level [README.md](../README.md) links here as the single entry point.

## Principles

- Frontend and backend boundaries are explicit
- Domain logic is separate from transport and UI
- Tests live close to the behavior they verify
- Fixtures are checked in and deterministic

## Folder Structure

- [architecture/](./architecture/) - High-level system design decisions, architectural overviews, and key tradeoffs. Summary of decisions in `architecture/README.md`, with details in subfiles.
- [systems-engineering/](./systems-engineering/) - Use cases, functional breakdown, product breakdown, and product requirements derived from the product vision.
- [functionality/](./functionality/) - Current and planned product capabilities, test evidence, stability level, and development plans.
- [implementation/](./implementation/) - Implementation contracts, API specs, parser rules, syntax examples, and current build direction. Contains `api/`, `graph/`, `parser-contract.md`, and `syntax-examples.md`.
- [roadmap/](./roadmap/) - Forward implementation plan from the current application state: milestones, gates, success criteria, and deferred work.
- [testing/](./testing/) - Supporting test strategy, MVP coverage, fixtures, CI guidance, and quality risks. Phase gates live in the roadmap.
- [ui/](./ui/) - User interface design, workflows, and feature specifications derived from the former `docs/design.md`.
- [ai/](./ai/) - Agent orientation, working rules, and context for AI-assisted development.
- [reference/](./reference/) - External standards (e.g., SysML v2) and source material.

The `decisions` and `setup` folders are intentionally deferred. Add them only when they contain real project material; do not create empty documentation folders.

## Navigation

1. Start here (docs/README.md)
2. Follow links to the relevant subfolder for your needs
3. Each subfolder's README.md provides detailed content and links to its files

Cross-reference between folders uses links, not content duplication.

# Documentation

This folder contains all project documentation, organized by abstraction level to minimize duplication and improve navigation. The top-level [README.md](../README.md) links here as the single entry point.

## Folder Structure

| Folder | Purpose |
|--------|---------|
| [architecture/](./architecture/) | High-level system design decisions, architectural overviews, and key tradeoffs. Summary of decisions in `architecture/README.md`, with details in subfiles. |
| [implementation/](./implementation/) | Implementation contracts, API specs, parser rules, syntax examples, and current build direction. Contains the moved `api-contract.md`, `parser-contract.md`, and `syntax-examples.md`. |
| [roadmap/](./roadmap/) | Planning, integration order, reasoning, roadmaps, and project structure. Merged from the former `docs/plan.md`. |
| [testing/](./testing/) | Test strategy, readiness gates, and test matrices. Split into focused documents: core strategy, MVP coverage, phases, fixtures, CI gates, quality and risk. |
| [ui/](./ui/) | User interface design, workflows, and feature specifications derived from the former `docs/design.md`. |
| [ai/](./ai/) | Agent orientation, working rules, and context for AI-assisted development. |
| [reference/](./reference/) | External standards (e.g., SysML v2) and source material. |

## Navigation

1. Start here (docs/README.md)
2. Follow links to the relevant subfolder for your needs
3. Each subfolder's README.md provides detailed content and links to its files

Cross-reference between folders uses links, not content duplication.

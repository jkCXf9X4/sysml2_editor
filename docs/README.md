# Documentation

This folder separates documentation by abstraction level.

## Start Here

1. [Plan index](./plan.md)
2. [Product and UX design](./design.md)
3. [Architecture decisions](./architecture/README.md)
4. [Implementation contracts](./implementation/README.md)
5. [Test strategy](./test-strategy.md)

## Abstraction Levels

- `design.md`: product-facing workflows, UX direction, and user-level model concepts.
- `architecture/`: high-level system decisions that shape implementation but are not tied to one code artifact.
- `implementation/`: concrete contracts, scaffold shape, API/parser details, syntax subset, and delivery roadmap.
- `testing/`: readiness gates and test matrices.
- `reference/`: external standards and source material.
- `ai/`: agent orientation and working rules.

Keep new documents at the highest abstraction level that can accurately own the decision. Do not put architecture policy, API details, and fixture syntax in the same document unless the coupling is intentional and explained.

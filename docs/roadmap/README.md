# Roadmap

This folder contains the single implementation roadmap for `sysml2_editor`.

[roadmap.md](./roadmap.md) is the canonical source for:

- MVP scope
- Development phases
- Test focus by phase
- Fixture expectations
- Minimum gate tests
- Setup order
- Phase exit criteria

Do not add separate phase plans, test roadmaps, setup plans, or readiness matrices in this folder. If phase scope, gate tests, or exit criteria change, update [roadmap.md](./roadmap.md) directly.

## Supporting References

Use these documents for detail behind the roadmap:

- [Product vision](../../PRODUCT_VISION.md) for product intent and tradeoffs
- [UI design](../ui/design.md) for the target workbench experience
- [Architecture overview](../architecture/README.md) for system decisions
- [Runtime decision](../architecture/runtime.md) for local-web runtime rules
- [Write policy](../architecture/write-policy.md) for save and context safety
- [Implementation contracts](../implementation/README.md) for API, parser, and graph details
- [Testing strategy](../testing/test-strategy.md) for testing principles
- [Fixtures](../testing/fixtures.md) for fixture catalog and rules

## Source Layout References

Use source-area READMEs for repository structure during implementation:

- [Backend layout](../../src/backend/README.md)
- [Frontend layout](../../src/frontend/README.md)
- [Shared contracts layout](../../src/shared/README.md)

If the repository layout changes, update the relevant source-area README first, then update the roadmap only if the implementation order, gates, or exit criteria change.

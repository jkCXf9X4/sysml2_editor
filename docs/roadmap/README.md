# Roadmap

This folder contains the forward implementation roadmap for `sysml2_editor`.

[roadmap.md](./roadmap.md) is the source for:

- Current implementation baseline
- Next milestones in recommended order
- Milestone scope
- Success and exit criteria
- Required verification gates
- Deferred work

The current capability inventory lives in [Functionality index](../functionality/README.md). Update that file when functionality status, stability, test evidence, or development plans change. Update the roadmap when milestone order, scope, success criteria, or gates change.

## Supporting References

Use these documents for detail behind the roadmap:

- [Product vision](../../PRODUCT_VISION.md) for product intent and tradeoffs
- [Systems engineering basis](../systems-engineering/README.md) for use cases, breakdowns, and requirements derived from the vision
- [Functionality index](../functionality/README.md) for current and planned capability status
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

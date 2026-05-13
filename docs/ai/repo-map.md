# Repository Map

Use [PRODUCT_VISION.md](../../PRODUCT_VISION.md) for product intent and [plan.md](../../plan.md) for the full documentation index.

## Intended Source Layout

The setup target is:

```text
PRODUCT_VISION.md
src/backend/
src/frontend/
src/shared/
tests/
fixtures/
docs/ai/
docs/implementation/
```

## How To Use The Docs During Implementation

- Start with the product vision, AI guide, and implementation workspace.
- Use the decision docs as the contract for implementation work.
- Check each material decision against the product vision trace rule.
- Use the structure doc when creating or rearranging folders.
- Use the test matrix as the readiness gate before adding new behavior.
- Use the write policy when deciding where a change belongs.

## Update Rule

If you add a new root-level document or major setup folder, update this map and [plan.md](../../plan.md). Update the README only for new top-level entry points. If you add or change a material decision, add a vision trace.

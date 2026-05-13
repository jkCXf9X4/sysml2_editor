# AI Implementation Guide

This is the first document an AI agent should read when working in this repository.

It exists to route agents to the authoritative planning and implementation documents without duplicating them.

## Start Here

- Implementation work: [docs/implementation/README.md](../implementation/README.md)
- Test strategy: [test-strategy.md](../../test-strategy.md)
- Product and UX direction: [design.md](../../design.md)

## What This Guide Is For

- Point to the authoritative docs for decisions and constraints
- Keep the setup path, coding path, and test path aligned

## What The Agent Should Optimize For

- Preserve textual SysML as the source of truth
- Preserve stable IDs and source ranges
- Minimize unrelated file churn
- Keep view state separate from model state
- Verify every change with the narrowest useful test

## When A Decision Changes

If any implementation assumption changes:

1. Update the relevant decision doc first.
2. Update this guide only when navigation changes.
3. Update the implementation plan if the change affects sequencing.
4. Update tests if the behavior contract changed.

## Helpful Companion Docs

- [Context pointers](./context.md)
- [Repository map](./repo-map.md)
- [Working rules](./working-rules.md)
- [Deferred topics](./open-questions.md)

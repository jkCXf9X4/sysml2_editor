# PDEC-003: Multi-Context Workspace

Status: Accepted

## Decision

The product must support multiple active repository, branch, worktree, and comparison contexts in one workspace without losing context identity.

## Reason

The product vision requires users to inspect, compare, and edit multiple branches of the same repository and multiple related repositories side by side.

## Tradeoffs

Every view, trace, diff, save action, and commit action must preserve repository and branch context. Combined views need explicit context identity instead of merged context-free collections.

## Applies To

- Workspace context model
- Branch and repository comparison
- Safe write targets
- Multi-context UI labeling

## Trace

- [Product Vision: Multi-Context Workspace](../../PRODUCT_VISION.md#multi-context-workspace)
- [Product Vision: Traceability Scope](../../PRODUCT_VISION.md#traceability-scope)

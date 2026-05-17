# PCOM-001: Git-Native Source Of Truth

Status: Accepted

## Decision

Textual SysML files in Git are the durable source of truth for model semantics.

## Reason

The product vision requires models to remain precise, reviewable, version-controlled, and understandable outside the visual editor.

## Tradeoffs

This makes file ownership, round-trip fidelity, and Git diffs core product constraints. It also prevents the canvas or layout files from becoming a competing source of model semantics.

## Applies To

- Source ownership
- Save behavior
- Git integration
- Text and visual editing consistency

## Trace

- [Product Vision: Vision Pillars](../../PRODUCT_VISION.md#vision-pillars)
- [Product Vision: Product Promise](../../PRODUCT_VISION.md#product-promise)

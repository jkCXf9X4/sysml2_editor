# PDEC-002: Visual Editing As Model Projection

Status: Accepted

## Decision

Visual diagrams and workbench views are editable projections of the model, not independent models.

## Reason

The product vision requires PowerPoint-level editing ease without losing SysML precision, traceability, or source-text reviewability.

## Tradeoffs

The UI must preserve model identity and source ownership during editing. Layout and view files may improve presentation, but they must not carry hidden model semantics.

## Applies To

- Canvas and diagram behavior
- Inspector behavior
- Saved views and layouts
- Text and visual synchronization

## Trace

- [Product Vision: Vision Pillars](../../PRODUCT_VISION.md#vision-pillars)
- [Product Vision: Product Promise](../../PRODUCT_VISION.md#product-promise)

# IDEC-003: Stable Model Graph Schema

Status: Accepted

## Decision

Represent parsed model state with a stable graph schema containing explicit context, node, edge, file, trace-link, and source-range concepts.

## Reason

The architecture requires all UI projections, diffs, inspectors, trace views, and write flows to operate from the same model representation.

## Tradeoffs

The graph carries source and lifecycle metadata even when a specific UI view only needs part of it. This keeps traceability and write safety consistent across views.

## Applies To

- Model graph records
- DTO shape
- Trace link derivation
- Source range handling

## Trace

- [Architecture: Model Graph](../architecture/model-graph.md)

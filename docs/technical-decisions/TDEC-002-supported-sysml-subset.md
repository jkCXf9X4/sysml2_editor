# TDEC-002: Supported SysML Subset

Status: Accepted

## Decision

Implement a supported SysML textual subset before attempting full SysML v2 language coverage.

## Reason

The architecture requires reliable parse, graph, visual edit, write, and reparse behavior before broader language support can be safe.

## Tradeoffs

The product does not claim full SysML v2 conformance. Supported examples must stay close to the OMG textual notation where practical and document deliberate simplifications.

## Applies To

- Parser behavior
- Syntax examples
- Test fixtures
- Conformance statements

## Trace

- [Architecture: Model Graph](../architecture/model-graph.md)
- [Architecture: Write Policy](../architecture/write-policy.md)

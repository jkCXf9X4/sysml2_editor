# IDEC-005: SysML-Native Identity Metadata

Status: Accepted

## Decision

Persist stable editable-element identity with SysML-native metadata annotations rather than tool-owned comments or side files.

## Reason

The architecture requires model identity to remain tied to source text so traceability, layout recovery, rename behavior, and Git review remain stable.

## Tradeoffs

The project uses `Sysml2EditorIdentity` metadata until a standard SysML identity convention is available. Elements without persisted identity may be loaded read-only with derived IDs.

## Applies To

- Stable IDs
- Parser metadata support
- Writer safety
- Layout and trace references

## Trace

- [Architecture: Model Graph](../architecture/model-graph.md)
- [Architecture: Write Policy](../architecture/write-policy.md)

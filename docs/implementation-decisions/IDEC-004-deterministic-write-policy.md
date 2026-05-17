# IDEC-004: Deterministic Write Policy

Status: Accepted

## Decision

Save operations must target exactly one writable workspace context and preserve deterministic file ownership.

## Reason

The architecture requires visual edits to produce reviewable Git changes without rewriting unrelated files or losing repository and branch context.

## Tradeoffs

Writes are blocked when the parser or writer cannot safely round-trip the target file. This favors source integrity over permissive editing.

## Applies To

- Save operations
- File ownership rules
- Worktree safety
- Dirty and concurrent file handling

## Trace

- [Architecture: Write Policy](../architecture/write-policy.md)

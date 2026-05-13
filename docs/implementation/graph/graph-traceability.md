# Traceability Links

## First Slice

Phase 1 in the [implementation roadmap](../../roadmap/roadmap.md) must derive trace links for:

- Item-to-item semantic relationships that are already represented as model edges.
- Item-to-file source ownership for every structured node.
- File-to-file import relationships when the imported target resolves to another repository-relative file.

## Later Slices

Later roadmap phases extend trace links for:

- Branch-to-branch links from semantic diff results.
- Repo-to-repo links from imported libraries, supplier models, shared views, or related engineering repositories.
- Context-to-context links from side-by-side branch and repository views.

## ID Derivation

Trace link IDs are deterministic UUID v5-style IDs derived from kind, source workspace, source endpoint, target workspace, target endpoint, relationship, and source range when present.

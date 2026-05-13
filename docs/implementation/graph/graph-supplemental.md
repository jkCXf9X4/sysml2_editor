# Supplemental Graph Concepts

## File-Level Records

Track file metadata separately from model nodes.

Required fields:

- `path`
- `lineEnding`
- `contentHash`
- `role`
- `isDirty`

`contentHash` is `sha256:` plus the lowercase hex SHA-256 hash of exact file bytes.

Recommended `FileRole` values:

- `Model`
- `View`
- `Config`
- `ImportedFragment`

Recommended `LineEndingKind` values:

- `LF`
- `CRLF`
- `Mixed`
- `Unknown`

## Opaque Spans

Unsupported but preservable text is represented as `OpaqueSpanDto`.

Rules:

- `sourceFile` and `sourceRange` identify the exact unsupported span.
- `reason` explains why the parser did not structure the span.
- `blocksWrite` is `true` when editing or saving could damage the span.
- The first read-only slice may display opaque spans only as diagnostics/source annotations.

## Diagnostics

Diagnostics are structured and stable enough for tests.

Recommended `DiagnosticSeverity` values:

- `Info`
- `Warning`
- `Error`

`code` should be a stable value such as `MissingStableId`, `UnsupportedSyntax`, or `ExpectedElementName`.

## Lifecycle State

Nodes and edges have a lifecycle state for UI overlays and commit summaries.

Recommended `ModelStatus` values:

- `Committed`
- `Modified`
- `Added`
- `Deleted`
- `Unresolved`

## Derived Views

The following UI projections derive from the same graph:

- Tree view from containment edges
- Graph view from semantic edges
- Inspector from node attributes and incident edges
- Trace matrix from traceability edges
- Impact analysis from changed nodes and dependent edges
- Source ownership view from nodes, files, imports, and source ranges
- Cross-repository dependency view from imported libraries, related repositories, and unresolved external references once multi-repo support is introduced

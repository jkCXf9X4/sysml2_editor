# Model Schema

## Decision

The implementation should use a stable graph schema with explicit node, edge, file, and source-range concepts.

The UI, diff engine, inspector, and traceability features all read from the same graph.

Vision trace:

- Supports: every model element remains precise, traceable, reviewable, and tied back to source text; item, file, branch, and repository traceability can be projected from one shared model.
- Tradeoff: the schema carries explicit source and lifecycle metadata from the start, even when early UI slices only consume part of it.

## Core Entities

Backend C# records are canonical for the first slice. Frontend types should be generated from OpenAPI as described in [api-contract.md](./api-contract.md).

### Node

Each node represents one semantic model element.

Required fields:

- `stableId`: UUID string, required
- `kind`: `NodeKind`, required
- `name`: string, required; empty only for `Unknown`
- `qualifiedName`: string, required when resolvable
- `owningPackageId`: UUID string or `null`
- `sourceFile`: repo-relative path, required
- `sourceRange`: `SourceRange`, required
- `attributes`: string-keyed JSON object, required
- `modelStatus`: `ModelStatus`, required

Recommended `kind` values for the MVP:

- `Package`
- `PartDefinition`
- `PartUsage`
- `Port`
- `Connection`
- `Requirement`
- `Import`
- `Unknown`

### Edge

Each edge represents a semantic relationship.

Required fields:

- `stableId`: UUID string, required
- `kind`: `EdgeKind`, required
- `sourceId`: UUID string, required
- `targetId`: UUID string, required when resolved
- `sourceFile`: repo-relative path, required
- `sourceRange`: `SourceRange`, required
- `attributes`: string-keyed JSON object, required
- `modelStatus`: `ModelStatus`, required

Edge IDs are derived by the backend for relationships that do not have source-level identity. The first implementation uses UUID v5-style deterministic IDs derived from repository-relative source file, edge kind, source node ID, target node ID or unresolved target text, and source range. If a relationship later gets explicit source identity, that explicit ID replaces the derived ID only through a migration rule.

Recommended `kind` values for the MVP:

- `Contains`
- `References`
- `ConnectsTo`
- `Satisfies`
- `Imports`
- `TracesTo`

### Source Range

Source ranges are 1-based and inclusive at the start, exclusive at the end.

Fields:

- `startLine`: integer, required
- `startColumn`: integer, required
- `endLine`: integer, required
- `endColumn`: integer, required

For file-level synthetic nodes and edges, use the smallest source range that identifies the declaration that caused the model item. Do not use zero values.

## API Serialization

JSON field names use camelCase. Enum values use PascalCase strings matching the names in this document.

Minimal node JSON:

```json
{
  "stableId": "11111111-1111-4111-8111-111111111111",
  "kind": "PartDefinition",
  "name": "BatteryPack",
  "qualifiedName": "Vehicle::BatteryPack",
  "owningPackageId": "00000000-0000-4000-8000-000000000001",
  "sourceFile": "model/root.sysml",
  "sourceRange": {
    "startLine": 3,
    "startColumn": 3,
    "endLine": 4,
    "endColumn": 24
  },
  "attributes": {},
  "modelStatus": "Committed"
}
```

## Canonical C# Shapes

These records are the starting point for backend DTOs. Names may move by namespace, but field meaning should stay stable.

```csharp
public sealed record ModelGraphDto(
    IReadOnlyList<ModelNodeDto> Nodes,
    IReadOnlyList<ModelEdgeDto> Edges,
    IReadOnlyList<ModelFileDto> Files,
    IReadOnlyList<OpaqueSpanDto> OpaqueSpans,
    IReadOnlyList<DiagnosticDto> Diagnostics);

public sealed record OpenRepositoryResponseDto(
    string RepositoryId,
    string RootPath,
    string Branch,
    ModelGraphDto Graph);

public sealed record ModelNodeDto(
    Guid StableId,
    NodeKind Kind,
    string Name,
    string QualifiedName,
    Guid? OwningPackageId,
    string SourceFile,
    SourceRangeDto SourceRange,
    IReadOnlyDictionary<string, object?> Attributes,
    ModelStatus ModelStatus);

public sealed record ModelEdgeDto(
    Guid StableId,
    EdgeKind Kind,
    Guid SourceId,
    Guid? TargetId,
    string SourceFile,
    SourceRangeDto SourceRange,
    IReadOnlyDictionary<string, object?> Attributes,
    ModelStatus ModelStatus);

public sealed record SourceRangeDto(
    int StartLine,
    int StartColumn,
    int EndLine,
    int EndColumn);

public sealed record ModelFileDto(
    string Path,
    LineEndingKind LineEnding,
    string ContentHash,
    FileRole Role,
    bool IsDirty);

public sealed record SourceFileDto(
    string Path,
    string Content,
    LineEndingKind LineEnding,
    string ContentHash);

public sealed record DiagnosticDto(
    DiagnosticSeverity Severity,
    string Code,
    string Message,
    string SourceFile,
    SourceRangeDto SourceRange);

public sealed record OpaqueSpanDto(
    string SourceFile,
    SourceRangeDto SourceRange,
    string Reason,
    bool BlocksWrite);
```

## Generated TypeScript Shape

TypeScript types should be generated from OpenAPI. Do not hand-maintain these by default.

Expected generated shape:

```typescript
export interface ModelNodeDto {
  stableId: string;
  kind: NodeKind;
  name: string;
  qualifiedName: string;
  owningPackageId?: string | null;
  sourceFile: string;
  sourceRange: SourceRangeDto;
  attributes: Record<string, unknown>;
  modelStatus: ModelStatus;
}
```

## Stable ID Rule

Stable IDs for supported semantic elements are persisted in the source text using a SysML-native metadata annotation immediately above the element definition.

Example:

```sysml
@Sysml2EditorIdentity { id = "3c4c3f6a-5d49-4ec7-8d5f-0d792df0a8f1"; }
```

Rules:

- Every editable supported semantic element, including packages, must have `@Sysml2EditorIdentity` metadata.
- The identity value is the metadata attribute `id`.
- IDs are generated once when the element is created.
- IDs do not change on rename.
- IDs do not change when a node moves within the same owning file.
- In the read-only first slice, elements without identity metadata may be loaded with deterministic derived IDs, but they are marked read-only with a diagnostic.
- Writer support must not silently save a file with missing identity metadata. A later explicit backfill operation may add metadata annotations and then enable editing.

Derived read-only IDs use a deterministic UUID v5-style hash of repository-relative source file, node kind, qualified name, and source range. They are process-stable but not a substitute for persisted IDs.

## File-Level Records

The implementation should also track file metadata separately from model nodes.

Required file metadata:

- `path`
- `lineEnding`
- `contentHash`
- `role`
- `isDirty`

`contentHash` is `sha256:` plus the lowercase hex SHA-256 hash of exact file bytes.

Recommended `role` values:

- `Model`
- `View`
- `Config`
- `ImportedFragment`

Recommended `lineEnding` values:

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

Recommended `severity` values:

- `Info`
- `Warning`
- `Error`

`code` should be a stable value such as `MissingStableId`, `UnsupportedSyntax`, or `ExpectedElementName`.

## Lifecycle State

Nodes and edges should have a lifecycle state for UI overlays and commit summaries.

Recommended values:

- `Committed`
- `Modified`
- `Added`
- `Deleted`
- `Unresolved`

## Derived Views

The following UI projections should be derived from the same graph:

- Tree view from containment edges
- Graph view from semantic edges
- Inspector from node attributes and incident edges
- Trace matrix from traceability edges
- Impact analysis from changed nodes and dependent edges
- Source ownership view from nodes, files, imports, and source ranges
- Cross-repository dependency view from imported libraries, related repositories, and unresolved external references once multi-repo support is introduced

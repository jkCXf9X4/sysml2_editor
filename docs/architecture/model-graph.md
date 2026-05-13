# Model Schema

## Decision

The implementation should use a stable graph schema with explicit context, node, edge, file, trace-link, and source-range concepts.

The UI, diff engine, inspector, and traceability features all read from the same graph.

Vision trace:

- Supports: every model element remains precise, traceable, reviewable, and tied back to source text; item, file, branch, repository, and workspace-context traceability can be projected from one shared model.
- Tradeoff: the schema carries explicit source and lifecycle metadata from the start, even when early UI slices only consume part of it.

## Core Entities

Backend C# records are canonical for the first slice. Frontend types should be generated from OpenAPI as described in [api-contract.md](../implementation/api-contract.md).

### Model Context

Each model graph belongs to one context. A context identifies the repository, branch or worktree, commit state, and whether the graph may be edited.

Required fields:

- `workspaceId`: backend-local workspace ID, required
- `repositoryId`: backend-local repository ID, required
- `repositoryAlias`: display name for the repository, required
- `rootPath`: absolute root path for the backing repository or worktree, required in backend responses
- `branch`: branch name or `"HEAD"` for detached HEAD, required
- `commitSha`: current commit SHA or `null` when unavailable
- `isWritable`: boolean, required
- `writableReason`: human-readable explanation, required

Rules:

- Multiple contexts may reference the same repository if they represent distinct branches, commits, or worktrees.
- Multiple contexts may reference different repositories in the same workspace.
- Save and commit operations must target exactly one writable context.
- Read-only contexts still support graph browsing, source viewing, traceability, and diff overlays.

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

### Trace Link

Trace links are derived navigation facts that make product traceability explicit for UI, tests, and future APIs. They do not replace semantic model edges; they give a stable cross-level trace view over model items, files, branches, and repositories.

Required fields:

- `stableId`: UUID string, required
- `kind`: `TraceLinkKind`, required
- `sourceKind`: `TraceEndpointKind`, required
- `sourceWorkspaceId`: workspace ID for the source endpoint, required when the source endpoint belongs to an open context
- `sourceId`: string, required
- `targetKind`: `TraceEndpointKind`, required
- `targetWorkspaceId`: workspace ID for the target endpoint, required when the target endpoint belongs to an open context
- `targetId`: string, required
- `relationship`: string, required
- `sourceFile`: repo-relative path or `null`
- `sourceRange`: `SourceRange` or `null`
- `attributes`: string-keyed JSON object, required

Recommended `kind` values:

- `ItemToItem`
- `ItemToFile`
- `FileToFile`
- `BranchToBranch`
- `RepoToRepo`

Recommended `sourceKind` and `targetKind` values:

- `ModelItem`
- `File`
- `Branch`
- `Repository`
- `WorkspaceContext`

### Multi-Context View

`ModelGraphDto` always represents exactly one workspace context. Combined branch, worktree, or repository views use a separate projection so context identity cannot be lost.

Required fields:

- `viewId`: backend-local view ID, required
- `kind`: `MultiContextViewKind`, required
- `title`: display title, required
- `contexts`: ordered `ModelContext` list, required
- `graphs`: ordered `ModelGraph` list, required
- `projections`: ordered `ContextProjection` list, required
- `crossContextTraceLinks`: `TraceLink` list, required
- `diagnostics`: `Diagnostic` list, required

Rules:

- A combined view must never merge nodes, files, or trace links into a context-free collection.
- `graphs` contains the single-context graph snapshots used by the view so the frontend can render the view without guessing which graph owns a projected ID.
- `contexts` and `graphs` are ordered together; each graph's `context.workspaceId` must match one listed context.
- Every projected node, edge, file, and trace link must carry the `workspaceId` it came from.
- Cross-context trace links use `WorkspaceContext`, `Branch`, or `Repository` endpoints when the relationship is between contexts instead of inside one graph.
- The first implementation may expose no combined views, but the first branch/repository comparison feature must use this projection instead of overloading `ModelGraphDto`.

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
    ModelContextDto Context,
    IReadOnlyList<ModelNodeDto> Nodes,
    IReadOnlyList<ModelEdgeDto> Edges,
    IReadOnlyList<ModelFileDto> Files,
    IReadOnlyList<TraceLinkDto> TraceLinks,
    IReadOnlyList<OpaqueSpanDto> OpaqueSpans,
    IReadOnlyList<DiagnosticDto> Diagnostics);

public sealed record OpenRepositoryResponseDto(
    string RepositoryId,
    string WorkspaceId,
    string RootPath,
    string Branch,
    ModelGraphDto Graph);

public sealed record TraceLinksResponseDto(
    IReadOnlyList<TraceLinkDto> TraceLinks);

public sealed record MultiContextViewDto(
    string ViewId,
    MultiContextViewKind Kind,
    string Title,
    IReadOnlyList<ModelContextDto> Contexts,
    IReadOnlyList<ModelGraphDto> Graphs,
    IReadOnlyList<ContextProjectionDto> Projections,
    IReadOnlyList<TraceLinkDto> CrossContextTraceLinks,
    IReadOnlyList<DiagnosticDto> Diagnostics);

public sealed record ContextProjectionDto(
    string WorkspaceId,
    IReadOnlyList<Guid> NodeIds,
    IReadOnlyList<Guid> EdgeIds,
    IReadOnlyList<string> FilePaths,
    IReadOnlyList<Guid> TraceLinkIds,
    IReadOnlyDictionary<string, object?> Attributes);

public sealed record ModelContextDto(
    string WorkspaceId,
    string RepositoryId,
    string RepositoryAlias,
    string RootPath,
    string Branch,
    string? CommitSha,
    bool IsWritable,
    string WritableReason);

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

public sealed record TraceLinkDto(
    Guid StableId,
    TraceLinkKind Kind,
    TraceEndpointKind SourceKind,
    string? SourceWorkspaceId,
    string SourceId,
    TraceEndpointKind TargetKind,
    string? TargetWorkspaceId,
    string TargetId,
    string Relationship,
    string? SourceFile,
    SourceRangeDto? SourceRange,
    IReadOnlyDictionary<string, object?> Attributes);

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

Expected generated trace link shape:

```typescript
export interface TraceLinkDto {
  stableId: string;
  kind: TraceLinkKind;
  sourceKind: TraceEndpointKind;
  sourceWorkspaceId?: string | null;
  sourceId: string;
  targetKind: TraceEndpointKind;
  targetWorkspaceId?: string | null;
  targetId: string;
  relationship: string;
  sourceFile?: string | null;
  sourceRange?: SourceRangeDto | null;
  attributes: Record<string, unknown>;
}
```

Expected generated multi-context view shape:

```typescript
export interface MultiContextViewDto {
  viewId: string;
  kind: MultiContextViewKind;
  title: string;
  contexts: ModelContextDto[];
  graphs: ModelGraphDto[];
  projections: ContextProjectionDto[];
  crossContextTraceLinks: TraceLinkDto[];
  diagnostics: DiagnosticDto[];
}

export interface ContextProjectionDto {
  workspaceId: string;
  nodeIds: string[];
  edgeIds: string[];
  filePaths: string[];
  traceLinkIds: string[];
  attributes: Record<string, unknown>;
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

## Workspace Contexts

Workspace context is the unit of safe viewing and editing.

The first implementation slice may open one context. Later slices must be able to hold several contexts at once:

- Same repository, different branches, read-only comparison contexts.
- Same repository, different worktrees, independently writable contexts.
- Different repositories, independently writable contexts.
- Related repositories used as dependencies or supplier/library models.

Combined views must not collapse IDs across contexts. When a view contains data from multiple contexts, the API must return `MultiContextViewDto` and UI projections must include context identity so duplicate stable IDs from different branches or repositories are not ambiguous.

Recommended `MultiContextViewKind` values:

- `BranchComparison`
- `RepositoryComparison`
- `TraceMatrix`
- `ImpactAnalysis`
- `CustomWorkspaceView`

Recommended `lineEnding` values:

- `LF`
- `CRLF`
- `Mixed`
- `Unknown`

## Traceability Links

The first implementation slice must derive trace links for:

- Item-to-item semantic relationships that are already represented as model edges.
- Item-to-file source ownership for every structured node.
- File-to-file import relationships when the imported target resolves to another repository-relative file.

Later slices extend trace links for:

- Branch-to-branch links from semantic diff results.
- Repo-to-repo links from imported libraries, supplier models, shared views, or related engineering repositories.
- Context-to-context links from side-by-side branch and repository views.

Trace link IDs are deterministic UUID v5-style IDs derived from kind, source workspace, source endpoint, target workspace, target endpoint, relationship, and source range when present.

Trace links are read-only derived data. Edits update the model graph or files; trace links are recomputed from the resulting graph, file records, Git state, and repository dependency metadata.

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

# Graph Code Generation

## API Serialization

JSON field names use camelCase. Enum values use PascalCase strings.

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

Backend DTO records. Names may move by namespace, but field meaning should stay stable.

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

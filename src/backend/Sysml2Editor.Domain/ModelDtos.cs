using System.Text.Json.Serialization;

namespace Sysml2Editor.Domain;

public sealed record ModelContextDto(
    string WorkspaceId,
    string RepositoryId,
    string RepositoryAlias,
    string RootPath,
    string Branch,
    string? CommitSha,
    bool IsWritable,
    string WritableReason);

public sealed record SourceRangeDto(int StartLine, int StartColumn, int EndLine, int EndColumn);

public sealed record ModelNodeDto(
    string StableId,
    string Kind,
    string Name,
    string QualifiedName,
    string? OwningPackageId,
    string SourceFile,
    SourceRangeDto SourceRange,
    Dictionary<string, string> Attributes,
    string ModelStatus);

public sealed record ModelEdgeDto(
    string StableId,
    string Kind,
    string SourceId,
    string? TargetId,
    string SourceFile,
    SourceRangeDto SourceRange,
    Dictionary<string, string> Attributes,
    string ModelStatus);

public sealed record ModelFileDto(string Path, string LineEnding, string ContentHash, string Role, bool IsDirty);

public sealed record TraceLinkDto(
    string StableId,
    string Kind,
    string SourceKind,
    string SourceWorkspaceId,
    string SourceId,
    string TargetKind,
    string TargetWorkspaceId,
    string? TargetId,
    string Relationship,
    string SourceFile,
    SourceRangeDto SourceRange,
    Dictionary<string, string> Attributes);

public sealed record DiagnosticDto(string Severity, string Code, string Message, string SourceFile, SourceRangeDto SourceRange);

public sealed record OpaqueSpanDto(string SourceFile, SourceRangeDto SourceRange, string Text);

public sealed record ModelGraphDto(
    ModelContextDto Context,
    List<ModelNodeDto> Nodes,
    List<ModelEdgeDto> Edges,
    List<ModelFileDto> Files,
    List<TraceLinkDto> TraceLinks,
    List<OpaqueSpanDto> OpaqueSpans,
    List<DiagnosticDto> Diagnostics);

public sealed record SourceFileDto(string Path, string Content, string LineEnding, string ContentHash);

public sealed record SavePreviewDto(List<string> ChangedFiles, List<string> UnchangedFiles);

public sealed record BranchDiffDto(
    string BaseWorkspaceId,
    string HeadWorkspaceId,
    string BaseBranch,
    string HeadBranch,
    List<ChangedModelItemDto> Added,
    List<ChangedModelItemDto> Modified,
    List<ChangedModelItemDto> Removed,
    List<ChangedFileDto> ChangedFiles,
    List<TraceLinkDto> TraceLinks);

public sealed record ChangedModelItemDto(string StableId, string Kind, string Name, string SourceFile);

public sealed record ChangedFileDto(string Path, string Status);

public sealed record MultiContextViewDto(
    string ViewId,
    string Kind,
    string Title,
    List<ModelContextDto> Contexts,
    List<ModelGraphDto> Graphs,
    List<ProjectionDto> Projections,
    List<TraceLinkDto> CrossContextTraceLinks,
    List<DiagnosticDto> Diagnostics);

public sealed record ProjectionDto(
    string WorkspaceId,
    List<string> NodeIds,
    List<string> EdgeIds,
    List<string> FilePaths,
    List<string> TraceLinkIds,
    Dictionary<string, string> Attributes);

public sealed record WriteResult(bool Succeeded, string? Content, List<DiagnosticDto> Diagnostics);

public sealed record GitStatusEntryDto(string Status, string Path);

public sealed record GitStatusDto(
    string RepositoryRoot,
    string Branch,
    string HeadSha,
    bool IsDirty,
    List<GitStatusEntryDto> ChangedFiles);

public sealed record GitCommitResultDto(
    bool Succeeded,
    string? CommitSha,
    string Summary,
    List<DiagnosticDto> Diagnostics);

public sealed record MergeConflictPreviewDto(
    string BaseBranch,
    string HeadBranch,
    bool HasConflicts,
    List<string> ConflictingFiles,
    string Summary);

public sealed record WorkspaceContextDto(
    string WorkspaceId,
    string RepositoryId,
    string RepositoryAlias,
    string RootPath,
    string Branch,
    string? CommitSha,
    bool IsWritable,
    string WritableReason,
    DateTime OpenedAt);

public sealed record OpenRepositoryResponseDto(
    string RepositoryId,
    string WorkspaceId,
    string RootPath,
    string Branch,
    bool IsWritable,
    ModelGraphDto Graph);

public sealed record WorkspaceListDto(List<WorkspaceContextDto> Contexts);

public sealed record WorktreeRequestDto(
    string RepositoryId,
    string Branch,
    string Path,
    bool CreateBranch);

public sealed record WorktreeResponseDto(
    string WorkspaceId,
    string RepositoryId,
    string RootPath,
    string Branch,
    bool IsWritable,
    string WritableReason,
    List<DiagnosticDto> Diagnostics);

public sealed record SavedViewDto(
    string ViewId,
    string Name,
    string Kind,
    string? WorkspaceId,
    string? RepositoryId,
    string? Branch,
    List<string> IncludedNodeIds,
    List<string> ExcludedNodeIds,
    Dictionary<string, string> Filters,
    Dictionary<string, string> Attributes,
    string StorageMode,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public sealed record SavedViewListDto(List<SavedViewDto> Views);

public sealed record SavedViewCreateDto(
    string Name,
    string Kind,
    string? WorkspaceId,
    string? RepositoryId,
    string? Branch,
    List<string>? IncludedNodeIds,
    List<string>? ExcludedNodeIds,
    Dictionary<string, string>? Filters,
    Dictionary<string, string>? Attributes,
    string StorageMode);

public sealed record SavedViewUpdateDto(
    string? Name,
    List<string>? IncludedNodeIds,
    List<string>? ExcludedNodeIds,
    Dictionary<string, string>? Filters,
    Dictionary<string, string>? Attributes);

public sealed record TraceMatrixDto(
    string ViewId,
    string Kind,
    string Title,
    string WorkspaceId,
    List<string> SourceNodeIds,
    List<string> TargetNodeIds,
    List<TraceMatrixCellDto> Cells);

public sealed record TraceMatrixCellDto(
    string SourceId,
    string TargetId,
    string Relationship,
    string? TraceLinkId,
    string SourceFile,
    bool IsDirect);

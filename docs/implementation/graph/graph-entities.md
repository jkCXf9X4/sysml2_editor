# Graph Entities

Field-level specifications for all core DTO entities in the model graph.

## ModelContextDto

Required fields:

- `workspaceId`: backend-local workspace ID
- `repositoryId`: backend-local repository ID
- `repositoryAlias`: display name for the repository
- `rootPath`: absolute root path for the backing repository or worktree, required in backend responses
- `branch`: branch name or `"HEAD"` for detached HEAD
- `commitSha`: current commit SHA or `null` when unavailable
- `isWritable`: boolean
- `writableReason`: human-readable explanation

## ModelNodeDto

Required fields:

- `stableId`: UUID string
- `kind`: `NodeKind`
- `name`: string, empty only for `Unknown`
- `qualifiedName`: string, required when resolvable
- `owningPackageId`: UUID string or `null`
- `sourceFile`: repo-relative path
- `sourceRange`: `SourceRangeDto`
- `attributes`: string-keyed JSON object
- `modelStatus`: `ModelStatus`

Recommended `NodeKind` values for the MVP:

- `Package`
- `PartDefinition`
- `PartUsage`
- `Port`
- `Connection`
- `Requirement`
- `Import`
- `Unknown`

## ModelEdgeDto

Required fields:

- `stableId`: UUID string
- `kind`: `EdgeKind`
- `sourceId`: UUID string
- `targetId`: UUID string, required when resolved
- `sourceFile`: repo-relative path
- `sourceRange`: `SourceRangeDto`
- `attributes`: string-keyed JSON object
- `modelStatus`: `ModelStatus`

Edge IDs are derived by the backend for relationships that do not have source-level identity. The first implementation uses UUID v5-style deterministic IDs derived from repository-relative source file, edge kind, source node ID, target node ID or unresolved target text, and source range. If a relationship later gets explicit source identity, that explicit ID replaces the derived ID only through a migration rule.

Recommended `EdgeKind` values for the MVP:

- `Contains`
- `References`
- `ConnectsTo`
- `Satisfies`
- `Imports`
- `TracesTo`

## SourceRangeDto

Source ranges are 1-based and inclusive at the start, exclusive at the end.

Fields:

- `startLine`: integer
- `startColumn`: integer
- `endLine`: integer
- `endColumn`: integer

For file-level synthetic nodes and edges, use the smallest source range that identifies the declaration that caused the model item. Do not use zero values.

## TraceLinkDto

Required fields:

- `stableId`: UUID string
- `kind`: `TraceLinkKind`
- `sourceKind`: `TraceEndpointKind`
- `sourceWorkspaceId`: workspace ID for the source endpoint, required when the source endpoint belongs to an open context
- `sourceId`: string
- `targetKind`: `TraceEndpointKind`
- `targetWorkspaceId`: workspace ID for the target endpoint, required when the target endpoint belongs to an open context
- `targetId`: string
- `relationship`: string
- `sourceFile`: repo-relative path or `null`
- `sourceRange`: `SourceRangeDto` or `null`
- `attributes`: string-keyed JSON object

Recommended `TraceLinkKind` values:

- `ItemToItem`
- `ItemToFile`
- `FileToFile`
- `BranchToBranch`
- `RepoToRepo`

Recommended `TraceEndpointKind` values:

- `ModelItem`
- `File`
- `Branch`
- `Repository`
- `WorkspaceContext`

## MultiContextViewDto

`ModelGraphDto` always represents exactly one workspace context. Combined branch, worktree, or repository views use a separate projection so context identity cannot be lost.

Required fields:

- `viewId`: backend-local view ID
- `kind`: `MultiContextViewKind`
- `title`: display title
- `contexts`: ordered `ModelContextDto` list
- `graphs`: ordered `ModelGraphDto` list
- `projections`: ordered `ContextProjectionDto` list
- `crossContextTraceLinks`: `TraceLinkDto` list
- `diagnostics`: `DiagnosticDto` list

Rules:

- A combined view must never merge nodes, files, or trace links into a context-free collection.
- `graphs` contains the single-context graph snapshots used by the view so the frontend can render the view without guessing which graph owns a projected ID.
- `contexts` and `graphs` are ordered together; each graph's `context.workspaceId` must match one listed context.
- Every projected node, edge, file, and trace link must carry the `workspaceId` it came from.
- Cross-context trace links use `WorkspaceContext`, `Branch`, or `Repository` endpoints when the relationship is between contexts instead of inside one graph.
- The first implementation may expose no combined views, but the first branch/repository comparison feature must use this projection instead of overloading `ModelGraphDto`.

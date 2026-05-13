# API Contract

The ASP.NET Core backend owns repository access, parsing, file IO, and Git execution. The frontend consumes API DTOs and does not shell out or write files directly.

## Contract Ownership

- Backend C# DTOs are canonical for the first implementation slice.
- The backend must expose OpenAPI during development.
- Frontend TypeScript API clients should be generated from OpenAPI into `src/shared/contracts/generated/`.
- Generated files should not be edited manually.

Vision trace:

- Supports: precise, traceable, reviewable model data exposed consistently to UI views; generated contracts prevent frontend/backend drift.
- Tradeoff: backend DTOs are canonical first, with generated TypeScript clients instead of hand-maintained shared types.

## Development Endpoints

Base URL:

```text
http://localhost:5087/api
```

Frontend dev URL:

```text
http://localhost:5173
```

## Core DTOs

### ModelGraphDto

```json
{
  "context": {
    "workspaceId": "workspace-main",
    "repositoryId": "local-vehicle-demo",
    "repositoryAlias": "vehicle-demo",
    "rootPath": "/absolute/path/to/repo",
    "branch": "main",
    "commitSha": null,
    "isWritable": true,
    "writableReason": "Current working tree is writable."
  },
  "nodes": [],
  "edges": [],
  "files": [],
  "traceLinks": [],
  "opaqueSpans": [],
  "diagnostics": []
}
```

### OpenRepositoryResponseDto

```json
{
  "repositoryId": "local-vehicle-demo",
  "workspaceId": "workspace-main",
  "rootPath": "/absolute/path/to/repo",
  "branch": "main",
  "graph": {
    "context": {
      "workspaceId": "workspace-main",
      "repositoryId": "local-vehicle-demo",
      "repositoryAlias": "vehicle-demo",
      "rootPath": "/absolute/path/to/repo",
      "branch": "main",
      "commitSha": null,
      "isWritable": true,
      "writableReason": "Current working tree is writable."
    },
    "nodes": [],
    "edges": [],
    "files": [],
    "traceLinks": [],
    "opaqueSpans": [],
    "diagnostics": []
  }
}
```

### ModelContextDto

```json
{
  "workspaceId": "workspace-main",
  "repositoryId": "local-vehicle-demo",
  "repositoryAlias": "vehicle-demo",
  "rootPath": "/absolute/path/to/repo",
  "branch": "main",
  "commitSha": null,
  "isWritable": true,
  "writableReason": "Current working tree is writable."
}
```

### ModelNodeDto

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

### SourceFileDto

```json
{
  "path": "model/root.sysml",
  "content": "package Vehicle { }",
  "lineEnding": "LF",
  "contentHash": "sha256:..."
}
```

### TraceLinkDto

```json
{
  "stableId": "66666666-6666-4666-8666-666666666666",
  "kind": "ItemToFile",
  "sourceKind": "ModelItem",
  "sourceWorkspaceId": "workspace-main",
  "sourceId": "11111111-1111-4111-8111-111111111111",
  "targetKind": "File",
  "targetWorkspaceId": "workspace-main",
  "targetId": "model/root.sysml",
  "relationship": "DefinedIn",
  "sourceFile": "model/root.sysml",
  "sourceRange": {
    "startLine": 3,
    "startColumn": 3,
    "endLine": 4,
    "endColumn": 24
  },
  "attributes": {}
}
```

### MultiContextViewDto

Multi-context screens use this projection instead of merging several `ModelGraphDto` responses into one context-free graph.

```json
{
  "viewId": "view-branch-main-vs-experiment",
  "kind": "BranchComparison",
  "title": "main vs experiment",
  "contexts": [
    {
      "workspaceId": "workspace-main",
      "repositoryId": "local-vehicle-demo",
      "repositoryAlias": "vehicle-demo",
      "rootPath": "/absolute/path/to/repo",
      "branch": "main",
      "commitSha": null,
      "isWritable": true,
      "writableReason": "Current working tree is writable."
    },
    {
      "workspaceId": "workspace-experiment",
      "repositoryId": "local-vehicle-demo",
      "repositoryAlias": "vehicle-demo",
      "rootPath": "/absolute/path/to/repo-experiment-worktree",
      "branch": "experiment",
      "commitSha": null,
      "isWritable": true,
      "writableReason": "Backed by a distinct writable worktree."
    }
  ],
  "graphs": [
    {
      "context": {
        "workspaceId": "workspace-main",
        "repositoryId": "local-vehicle-demo",
        "repositoryAlias": "vehicle-demo",
        "rootPath": "/absolute/path/to/repo",
        "branch": "main",
        "commitSha": null,
        "isWritable": true,
        "writableReason": "Current working tree is writable."
      },
      "nodes": [],
      "edges": [],
      "files": [],
      "traceLinks": [],
      "opaqueSpans": [],
      "diagnostics": []
    },
    {
      "context": {
        "workspaceId": "workspace-experiment",
        "repositoryId": "local-vehicle-demo",
        "repositoryAlias": "vehicle-demo",
        "rootPath": "/absolute/path/to/repo-experiment-worktree",
        "branch": "experiment",
        "commitSha": null,
        "isWritable": true,
        "writableReason": "Backed by a distinct writable worktree."
      },
      "nodes": [],
      "edges": [],
      "files": [],
      "traceLinks": [],
      "opaqueSpans": [],
      "diagnostics": []
    }
  ],
  "projections": [
    {
      "workspaceId": "workspace-main",
      "nodeIds": [],
      "edgeIds": [],
      "filePaths": [],
      "traceLinkIds": [],
      "attributes": {}
    },
    {
      "workspaceId": "workspace-experiment",
      "nodeIds": [],
      "edgeIds": [],
      "filePaths": [],
      "traceLinkIds": [],
      "attributes": {}
    }
  ],
  "crossContextTraceLinks": [],
  "diagnostics": []
}
```

Rules:

- `ModelGraphDto` represents one context only.
- `MultiContextViewDto` represents a view over two or more contexts.
- `contexts` and `graphs` are ordered together; every graph context must match one listed context by `workspaceId`.
- Projected IDs are resolved by pairing the ID with the projection `workspaceId`.
- Trace links include source and target workspace IDs so endpoint IDs remain unambiguous across branches and repositories.
- Cross-context links must use explicit `WorkspaceContext`, `Branch`, or `Repository` endpoints and source/target workspace IDs when those endpoints belong to open contexts.

### DiagnosticDto

```json
{
  "severity": "Error",
  "code": "ExpectedElementName",
  "message": "Expected element name.",
  "sourceFile": "model/root.sysml",
  "sourceRange": {
    "startLine": 3,
    "startColumn": 12,
    "endLine": 3,
    "endColumn": 13
  }
}
```

### ErrorResponseDto

All non-2xx API responses use the same envelope:

```json
{
  "code": "RepositoryNotFound",
  "message": "The repository path does not exist.",
  "details": {
    "path": "/absolute/path/to/repo"
  }
}
```

`code` is stable and machine-readable. `message` is human-readable. `details` is optional and must not contain secrets.

## Repository Session Model

`repositoryId` is a backend-local session ID for an opened repository. It is stable only for the running backend process. The initial implementation may keep repository sessions in memory.

Repository IDs are generated by the backend and must not be derived from the full filesystem path. The backend stores the mapping from `repositoryId` to absolute root path.

Opening the same absolute path twice should return the existing `repositoryId` for the current backend process unless the existing session has been explicitly closed by a later API.

## Workspace Context Model

`workspaceId` identifies one open model context. A context combines repository, branch or worktree, commit state, and writable status.

Rules:

- The first implementation slice may create one context per opened repository.
- Later slices must allow multiple contexts for the same repository when branches, commits, or worktrees are shown side by side.
- Later slices must allow multiple repositories in the same workspace.
- Save and commit operations must always target one writable `workspaceId`.
- Read-only contexts may still expose model graph, source files, trace links, and comparisons.

## Multi-Context View Model

The backend exposes multi-context comparison as derived projections, not as mutable graphs.

Rules:

- `ModelGraphDto` is never used for more than one context.
- Branch comparison, repository comparison, trace matrices, and impact views use `MultiContextViewDto`.
- A multi-context view includes the single-context graph snapshots it projects, and every projected reference is scoped by `workspaceId`.
- Editing from a multi-context view must first resolve to exactly one writable `workspaceId`, one file path, and one intended operation.
- The first implementation slice may skip this endpoint entirely; the first branch comparison implementation must add it before exposing side-by-side comparison in the UI.

Recommended view kinds:

- `BranchComparison`
- `RepositoryComparison`
- `TraceMatrix`
- `ImpactAnalysis`
- `CustomWorkspaceView`

## Path Rules

- All file paths in API DTOs are repository-relative paths using `/`.
- Absolute paths are accepted only in `POST /repositories/open`.
- `..`, absolute paths, drive-qualified paths, and symlink escapes outside the repository root must be rejected.
- File content endpoints must use a query parameter for paths instead of a path segment so nested model paths are unambiguous.

## HTTP Status Rules

- `200 OK`: request succeeded.
- `400 Bad Request`: malformed request or invalid path syntax.
- `404 Not Found`: repository session or file does not exist.
- `409 Conflict`: repository state prevents the operation, such as a later save hash mismatch.
- `422 Unprocessable Entity`: the repository or SysML content was readable but failed validation/parsing rules.
- `500 Internal Server Error`: unexpected backend failure.

## Endpoint Contracts

### Open Repository

```text
POST /repositories/open
```

Request:

```json
{
  "path": "/absolute/path/to/repo"
}
```

Response:

```json
{
  "repositoryId": "local-vehicle-demo",
  "workspaceId": "workspace-main",
  "rootPath": "/absolute/path/to/repo",
  "branch": "main",
  "graph": {
    "context": {
      "workspaceId": "workspace-main",
      "repositoryId": "local-vehicle-demo",
      "repositoryAlias": "vehicle-demo",
      "rootPath": "/absolute/path/to/repo",
      "branch": "main",
      "commitSha": null,
      "isWritable": true,
      "writableReason": "Current working tree is writable."
    },
    "nodes": [],
    "edges": [],
    "files": [],
    "traceLinks": [],
    "opaqueSpans": [],
    "diagnostics": []
  }
}
```

Behavior:

- The backend validates that `path` exists, is a directory, and is inside a Git work tree.
- The backend discovers `*.sysml` files, parses them synchronously for the first slice, and returns the current graph.
- Parse diagnostics do not make the open request fail unless no model graph can be created at all.
- Git branch is read from the local repository. Detached HEAD is reported as `"HEAD"`.
- The backend creates a `workspaceId` for this repository context and marks whether it is writable.

Errors:

- `400 Bad Request` for a missing, relative, or syntactically invalid path.
- `404 Not Found` when the path does not exist.
- `422 Unprocessable Entity` when the path is not usable as a Git-backed SysML repository.

### Get Model Graph

```text
GET /repositories/{repositoryId}/model
```

Response: `ModelGraphDto`

Errors:

- `404 Not Found` when `repositoryId` is unknown.

Compatibility rule:

- This repository-scoped endpoint is valid while one repository has one active context.
- Multi-branch or multi-worktree views must use the workspace-scoped endpoint below to avoid ambiguity.

### Get Workspace Model Graph

```text
GET /workspace-contexts/{workspaceId}/model
```

Response: `ModelGraphDto`

Behavior:

- Returns the graph for one explicit repository/branch/worktree context.
- This is the preferred endpoint once multiple contexts are open for the same repository.

Errors:

- `404 Not Found` when `workspaceId` is unknown.

### Get Workspace Contexts

```text
GET /workspace-contexts
```

Response:

```json
{
  "contexts": []
}
```

Behavior:

- Returns all open repository/branch/worktree contexts in the running backend process.
- The first implementation slice may return one context.
- Later slices use this endpoint to drive side-by-side branch and repository views.

### Create Worktree Context

```text
POST /workspace-contexts/worktrees
```

Request:

```json
{
  "repositoryId": "local-vehicle-demo",
  "branch": "experiment",
  "path": "/absolute/path/to/repo-experiment-worktree",
  "createBranch": false
}
```

Response: `ModelContextDto`

Behavior:

- The backend creates or opens a Git worktree at the requested absolute path.
- The operation is explicit and user-initiated; opening a comparison must not create a worktree implicitly.
- The response marks the context writable only when the worktree root is distinct from every other writable context for the same repository and passes filesystem/Git validation.

Errors:

- `400 Bad Request` when the path is relative, the branch name is invalid, or the requested worktree would overlap an existing context root.
- `404 Not Found` when `repositoryId` is unknown.
- `409 Conflict` when the branch is already checked out in another writable context and no distinct worktree can be created.

### Close Workspace Context

```text
DELETE /workspace-contexts/{workspaceId}
```

Behavior:

- Closes the in-memory workspace context.
- Does not delete repository files or Git worktrees.
- A later explicit worktree deletion endpoint may be added, but it must not be coupled to context close.

Errors:

- `404 Not Found` when `workspaceId` is unknown.

### Get Source File

```text
GET /repositories/{repositoryId}/files?path={repoRelativePath}
```

Response: `SourceFileDto`

Behavior:

- The endpoint returns the exact file content for display.
- It must not normalize line endings.
- `contentHash` is `sha256:` plus the lowercase hex SHA-256 hash of the exact bytes read from disk.

Errors:

- `400 Bad Request` when `path` is invalid or escapes the repository.
- `404 Not Found` when the repository or file does not exist.

Compatibility rule:

- This repository-scoped endpoint is valid while one repository has one active context.
- Multi-branch or multi-worktree views must use the workspace-scoped endpoint below.

### Get Workspace Source File

```text
GET /workspace-contexts/{workspaceId}/files?path={repoRelativePath}
```

Response: `SourceFileDto`

Behavior:

- Returns exact file content from one explicit repository/branch/worktree context.
- This prevents same-repository multi-branch views from reading the wrong checked-out file.

Errors:

- `400 Bad Request` when `path` is invalid or escapes the context root.
- `404 Not Found` when the workspace context or file does not exist.

### Get Trace Links

```text
GET /repositories/{repositoryId}/trace-links
```

Response: `TraceLinksResponseDto`

```json
{
  "traceLinks": []
}
```

Behavior:

- The endpoint returns the current derived trace links for the repository session.
- The first implementation slice must include `ItemToItem`, `ItemToFile`, and resolvable `FileToFile` trace links.
- Intra-context trace links set both endpoint workspace IDs to the active workspace context.
- Trace links are recomputed from the current model graph, file records, imports, and Git state; they are not edited directly.

Errors:

- `404 Not Found` when `repositoryId` is unknown.

Compatibility rule:

- This repository-scoped endpoint is valid while one repository has one active context.
- Multi-context views must use the workspace-scoped endpoint below.

### Get Workspace Trace Links

```text
GET /workspace-contexts/{workspaceId}/trace-links
```

Response: `TraceLinksResponseDto`

Behavior:

- Returns derived trace links for one explicit repository/branch/worktree context.
- Intra-context trace links set both endpoint workspace IDs to this `workspaceId`.
- Combined views use `MultiContextViewDto.crossContextTraceLinks` instead of client-side context-free merging.

Errors:

- `404 Not Found` when `workspaceId` is unknown.

### Create Multi-Context View

```text
POST /multi-context-views
```

Request:

```json
{
  "kind": "BranchComparison",
  "title": "main vs experiment",
  "workspaceIds": ["workspace-main", "workspace-experiment"]
}
```

Response: `MultiContextViewDto`

Behavior:

- The backend validates that every `workspaceId` is open.
- The backend builds a read-only projection over the requested contexts.
- The response must not include unscoped node, edge, file, or trace-link references.

Errors:

- `400 Bad Request` when fewer than two contexts are supplied or the view kind is unsupported.
- `404 Not Found` when any workspace context is unknown.

## Deferred Trace Contracts

Branch-to-branch and repo-to-repo traceability are not first-slice behavior, but their contract boundaries are reserved so implementation choices do not block the product vision.

### BranchTraceDto

```json
{
  "baseWorkspaceId": "workspace-main",
  "headWorkspaceId": "workspace-experiment",
  "baseBranch": "main",
  "headBranch": "experiment",
  "traceLinks": [],
  "changedFiles": [],
  "changedItems": []
}
```

### RepositoryDependencyDto

```json
{
  "workspaceId": "workspace-main",
  "relatedWorkspaceId": "workspace-supplier",
  "repositoryId": "local-vehicle-demo",
  "relatedRepositoryId": "local-supplier-model",
  "relationship": "DependsOn",
  "source": "model/root.sysml",
  "target": "../supplier/model/root.sysml",
  "traceLinks": []
}
```

## Deferred Endpoints

- Save file
- Commit changes
- Branch comparison
- Visual diff
- Clone repository

These are not required for the read-only first slice.

# API Endpoints

## Open Repository

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
- The backend discovers `*.sysml` files, parses them synchronously for Phase 1, and returns the current graph.
- Parse diagnostics do not make the open request fail unless no model graph can be created at all.
- Git branch is read from the local repository. Detached HEAD is reported as `"HEAD"`.
- The backend creates a `workspaceId` for this repository context and marks whether it is writable.

Errors:

- `400 Bad Request` for a missing, relative, or syntactically invalid path.
- `404 Not Found` when the path does not exist.
- `422 Unprocessable Entity` when the path is not usable as a Git-backed SysML repository.

## Get Model Graph

```text
GET /repositories/{repositoryId}/model
```

Response: `ModelGraphDto`

Errors:

- `404 Not Found` when `repositoryId` is unknown.

Compatibility rule:

- This repository-scoped endpoint is valid while one repository has one active context.
- Multi-branch or multi-worktree views must use the workspace-scoped endpoint below to avoid ambiguity.

## Get Workspace Model Graph

```text
GET /workspace-contexts/{workspaceId}/model
```

Response: `ModelGraphDto`

Behavior:

- Returns the graph for one explicit repository/branch/worktree context.
- This is the preferred endpoint once multiple contexts are open for the same repository.

Errors:

- `404 Not Found` when `workspaceId` is unknown.

## Get Workspace Contexts

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
- Phase 1 may return one context.
- Later roadmap phases use this endpoint to drive side-by-side branch and repository views.

## Create Worktree Context

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

## Close Workspace Context

```text
DELETE /workspace-contexts/{workspaceId}
```

Behavior:

- Closes the in-memory workspace context.
- Does not delete repository files or Git worktrees.
- A later explicit worktree deletion endpoint may be added, but it must not be coupled to context close.

Errors:

- `404 Not Found` when `workspaceId` is unknown.

## Get Source File

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

## Get Workspace Source File

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

## Get Trace Links

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
- Phase 1 must include `ItemToItem`, `ItemToFile`, and resolvable `FileToFile` trace links.
- Intra-context trace links set both endpoint workspace IDs to the active workspace context.
- Trace links are recomputed from the current model graph, file records, imports, and Git state; they are not edited directly.

Errors:

- `404 Not Found` when `repositoryId` is unknown.

Compatibility rule:

- This repository-scoped endpoint is valid while one repository has one active context.
- Multi-context views must use the workspace-scoped endpoint below.

## Get Workspace Trace Links

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

## Create Multi-Context View

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

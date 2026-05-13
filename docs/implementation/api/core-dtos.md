# Core DTOs

DTO definitions for the API contract. Backend C# DTOs are canonical for the Phase 1 API path in the [implementation roadmap](../../roadmap/roadmap.md).

## ModelGraphDto

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

## OpenRepositoryResponseDto

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

## ModelContextDto

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

## ModelNodeDto

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

## SourceFileDto

```json
{
  "path": "model/root.sysml",
  "content": "package Vehicle { }",
  "lineEnding": "LF",
  "contentHash": "sha256:..."
}
```

## TraceLinkDto

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

## MultiContextViewDto

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

## DiagnosticDto

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

## ErrorResponseDto

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

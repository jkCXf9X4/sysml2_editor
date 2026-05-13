# Deferred Contracts

Branch-to-branch and repo-to-repo traceability are not first-slice behavior, but their contract boundaries are reserved so implementation choices do not block the product vision.

## BranchTraceDto

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

## RepositoryDependencyDto

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

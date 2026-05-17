# Runtime

## Governing Product Decisions

- [PDEC-001: Git-native source of truth](../product-decisions/PDEC-001-git-native-source-of-truth.md)
- [PDEC-003: Multi-context workspace](../product-decisions/PDEC-003-multi-context-workspace.md)

## Runtime Guarantees

`sysml2_editor` runs as a local web app:

- React frontend in the browser
- ASP.NET Core backend on `localhost`
- Backend serves the API and static frontend assets
- Backend owns repository access, file IO, and Git process spawning
- No Electron or Avalonia wrapper by default

## Operational Model

- Users launch the backend with a local dev command.
- The frontend loads from the backend URL.
- The frontend never writes files directly and never shells out to Git directly.
- All repo writes and status refreshes go through the backend API.
- The backend owns workspace contexts for opened repositories, branches, commits, and worktrees.
- Multiple contexts may be open at once; every write operation must target one writable context.

## Multi-Context Runtime Rules

The runtime must distinguish repository identity from workspace context identity:

- `repositoryId` identifies a Git repository known to the backend.
- `workspaceId` identifies one open view/edit context for a repository, branch, commit, or worktree.
- Two branches of the same repository may be open at the same time only as distinct workspace contexts.
- Two writable branches of the same repository require distinct safe write locations, such as separate Git worktrees.
- Multiple repositories may be open at the same time as separate workspace contexts.
- Multi-context comparison views are derived projections over workspace contexts, not replacement model graphs.
- Worktree creation is an explicit backend operation requested by the user. The runtime must not create or delete worktrees as hidden side effects of comparison or context close.
- Opening a repository path or existing worktree creates a workspace context.
- Closing a workspace context only removes the in-memory context. It does not delete a worktree.
- `ModelGraphDto` remains a single-context graph. Side-by-side branch or repository screens use `MultiContextViewDto`.
- Save and commit actions are disabled from combined views until the selected change resolves to exactly one writable `workspaceId`.

## Backend Responsibilities

- Repository open/clone/status operations
- Workspace context management for multiple repositories, branches, and worktrees
- SysML file discovery
- Parse/index pipeline
- Stable model graph API
- Save operations that preserve file boundaries
- Git diff/status/commit integration

## Frontend Responsibilities

- Dockable workbench
- Canvas/tree/matrix/text views
- Visual editing gestures
- Inspector and validation presentation
- Change overlays and traceability navigation across items, files, branches, and repositories

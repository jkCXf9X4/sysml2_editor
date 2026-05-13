# Runtime Decision

## Decision

For Phase 1 in the [implementation roadmap](../roadmap/roadmap.md), `sysml2_editor` runs as a local web app:

- React frontend in the browser
- ASP.NET Core backend on `localhost`
- Backend serves the API and static frontend assets
- Backend owns repository access, file IO, and Git process spawning
- No Electron or Avalonia wrapper in Phase 1

## Why This Is The Starting Point

- It is the lowest-risk cross-platform path.
- It keeps Git and filesystem operations in one place.
- It avoids committing to desktop packaging before the product behavior is proven.
- It lets the frontend iterate quickly while the backend stays authoritative for model state.

Vision trace:

- Supports: textual SysML in Git as the durable source of truth; Git operations as visible modeling workflow; visual editing backed by backend-owned model state; multiple repository and branch contexts in one local workspace.
- Tradeoff: defers desktop packaging so early roadmap phases can prove parsing, source mapping, and Git-backed workflows.

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

## Development Command Shape

The initial local-web runtime should standardize on a simple launch model:

- Backend project: `src/backend/Sysml2Editor.Api`
- Backend command: `dotnet run --project src/backend/Sysml2Editor.Api`
- Backend URL: `http://localhost:5087`
- Frontend project: `src/frontend`
- Frontend command: `npm run dev`
- Frontend URL: `http://localhost:5173`
- Frontend API proxy: `/api` proxies to `http://localhost:5087/api`

During development, Vite may serve the frontend. For packaged local-web use, the ASP.NET Core backend may serve built static frontend assets.

## Recommended Stack

- React
- TypeScript
- C# / .NET backend
- Monaco Editor (text editing)
- React Flow (canvas/graph views)
- Git CLI

Why this fits:

- C# experience reduces backend risk.
- .NET runs well on Windows and Linux.
- Git and filesystem operations are straightforward from C#.
- The parser/model index can be implemented as strongly typed C# domain code.
- The React UI remains flexible for graph/canvas-heavy interaction.
- Git CLI is reliable and avoids premature complexity.

Vision trace:

- Supports: visual workbench interaction through React; precise backend-owned model state through .NET; Git-native review through Git CLI integration.
- Tradeoff: starts as a local web app and MVP parser instead of full desktop packaging or full SysML v2 grammar.

## Desktop Packaging Options

Implementation phases are defined only in [Implementation roadmap](../roadmap/roadmap.md). Runtime packaging options are:

- Start as a local web app during development.
- Package with Electron if quickest cross-platform delivery matters.
- Use Avalonia/.NET if a more native C# desktop shell becomes more important than web-based canvas flexibility.

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

Git integration should start with the `git` CLI wrapper.

SysML parsing should start with the MVP custom subset parser described in [parser-contract.md](../implementation/parser-contract.md). External parser or language-server integrations can come later if the product needs broader language coverage.
Use [sysml-v2.md](../reference/sysml-v2.md) as the external language reference and [syntax-examples.md](../implementation/syntax-examples.md) as the local implementation subset.

## CORS Policy

During development, the backend should allow `http://localhost:5173`.

Production/local packaged mode should not require broad CORS because frontend assets are served by the backend.

## Deferred Choices

The following are deferred until the product behavior is stable:

- Electron packaging
- Native desktop shell packaging
- Offline installer flow
- Multi-window desktop process management

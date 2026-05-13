# Runtime Decision

## Decision

For the first implementation slice, `sysml2_editor` runs as a local web app:

- React frontend in the browser
- ASP.NET Core backend on `localhost`
- Backend serves the API and static frontend assets
- Backend owns repository access, file IO, and Git process spawning
- No Electron or Avalonia wrapper in the first slice

## Why This Is The Starting Point

- It is the lowest-risk cross-platform path.
- It keeps Git and filesystem operations in one place.
- It avoids committing to desktop packaging before the product behavior is proven.
- It lets the frontend iterate quickly while the backend stays authoritative for model state.

Vision trace:

- Supports: textual SysML in Git as the durable source of truth; Git operations as visible modeling workflow; visual editing backed by backend-owned model state; multiple repository and branch contexts in one local workspace.
- Tradeoff: defers desktop packaging so the first slices can prove parsing, source mapping, and Git-backed workflows.

## Operational Model

- Users launch the backend with a local dev command.
- The frontend loads from the backend URL.
- The frontend never writes files directly and never shells out to Git directly.
- All repo writes and status refreshes go through the backend API.
- The backend owns workspace contexts for opened repositories, branches, commits, and worktrees.
- Multiple contexts may be open at once; every write operation must target one writable context.

## Multi-Context Runtime Rule

The runtime must distinguish repository identity from workspace context identity:

- `repositoryId` identifies a Git repository known to the backend.
- `workspaceId` identifies one open view/edit context for a repository, branch, commit, or worktree.
- Two branches of the same repository may be open at the same time only as distinct workspace contexts.
- Two writable branches of the same repository require distinct safe write locations, such as separate Git worktrees.
- Multiple repositories may be open at the same time as separate workspace contexts.
- Multi-context comparison views are derived projections over workspace contexts, not replacement model graphs.
- Worktree creation is an explicit backend operation requested by the user. The runtime must not create or delete worktrees as hidden side effects of comparison or context close.

## Development Command Shape

The first implementation slice should standardize on a simple local launch model:

- Backend project: `src/backend/Sysml2Editor.Api`
- Backend command: `dotnet run --project src/backend/Sysml2Editor.Api`
- Backend URL: `http://localhost:5087`
- Frontend project: `src/frontend`
- Frontend command: `npm run dev`
- Frontend URL: `http://localhost:5173`
- Frontend API proxy: `/api` proxies to `http://localhost:5087/api`

During development, Vite may serve the frontend. For packaged local-web use, the ASP.NET Core backend may serve built static frontend assets.

## CORS Policy

During development, the backend should allow `http://localhost:5173`.

Production/local packaged mode should not require broad CORS because frontend assets are served by the backend.

## Deferred Choices

The following are deferred until the product behavior is stable:

- Electron packaging
- Native desktop shell packaging
- Offline installer flow
- Multi-window desktop process management

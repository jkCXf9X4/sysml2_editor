# TDEC-001: Local Web Runtime

Status: Accepted

## Decision

Build the runtime as a local web app with a React frontend and ASP.NET Core backend on `localhost`.

## Reason

The architecture requires backend-owned repository access, file IO, Git operations, and model state while keeping the UI flexible for a dense workbench.

## Tradeoffs

This defers desktop shell packaging. Electron or Avalonia may be considered later if desktop-specific behavior becomes necessary.

## Runtime Stack

- Frontend: React and TypeScript.
- Backend: ASP.NET Core on `localhost`.
- Text editing: Monaco Editor.
- Canvas and graph views: React Flow.
- Git integration: Git CLI wrapper owned by the backend.

## Development Shape

- Backend project: `src/backend/Sysml2Editor.Api`.
- Backend command: `dotnet run --project src/backend/Sysml2Editor.Api`.
- Backend URL: `http://localhost:5087`.
- Frontend project: `src/frontend`.
- Frontend command: `npm run dev`.
- Frontend URL: `http://localhost:5173`.
- Frontend API proxy: `/api` proxies to `http://localhost:5087/api`.

During development, the backend should allow `http://localhost:5173`. Packaged local-web mode should serve frontend assets from the backend and should not require broad CORS.

## Applies To

- Frontend runtime
- Backend runtime
- Local development launch model
- Desktop packaging decisions

## Trace

- [Architecture: Runtime](../architecture/runtime.md)

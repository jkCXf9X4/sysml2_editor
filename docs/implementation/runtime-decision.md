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

## Operational Model

- Users launch the backend with a local dev command.
- The frontend loads from the backend URL.
- The frontend never writes files directly and never shells out to Git directly.
- All repo writes and status refreshes go through the backend API.

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

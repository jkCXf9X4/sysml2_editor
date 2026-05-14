# sysml2_editor

Git-native SysML v2 viewer and editor.

## Product Vision

`sysml2_editor` is a Git-native SysML v2 architecture workbench that lets engineers model systems visually with PowerPoint-level ease while preserving textual precision, traceability, validation, and version control.

Traceability includes links between model items, source files, branches, and related repositories.
The workbench is intended to support multiple branches of the same repository and multiple repositories open side by side, with explicit context on every view and edit.

All material design and implementation decisions should trace back to the product vision.

## Repository Layout

```text
sysml2_editor/
  README.md
  PRODUCT_VISION.md
  LICENSE
  docs/                     -- documentation index
  src/
    backend/                -- ASP.NET Core backend
    frontend/               -- React frontend
    shared/                 -- shared contracts, generated API clients
  tests/
    unit/                   -- parser, graph, write-policy unit tests
    integration/            -- backend + filesystem + Git integration tests
    e2e/                    -- full-stack user workflow tests
  fixtures/                 -- checked-in test repos and models
  scripts/                  -- dev and CI utilities
```

See [src/backend/README.md](./src/backend/README.md), [src/frontend/README.md](./src/frontend/README.md), and [src/shared/README.md](./src/shared/README.md) for details on each source area.

## Start Here

- Documentation index: [docs/README.md](./docs/README.md)

## Quick Start

Prerequisites:

- Node.js 22 or newer
- npm
- .NET SDK/runtime 10

Start the full development application from the repository root:

```bash
bash scripts/start.sh
```

The script starts:

- Backend API: `http://127.0.0.1:5087`
- Frontend app: `http://localhost:5173`

Open `http://localhost:5173` in a browser.
Press Ctrl-C in the script terminal to stop both processes.
If services are already running or detached, stop them with:

```bash
bash scripts/stop.sh
```

To verify the backend is running:

```bash
curl -fsS http://127.0.0.1:5087/api/health
```

Expected response:

```json
{"status":"ok"}
```

## Run The Application

Prerequisites:

- Node.js 22 or newer
- npm
- .NET SDK/runtime 10 for the backend scaffold

Start the backend API:

```bash
dotnet run --project src/backend/Sysml2Editor.Api --urls http://127.0.0.1:5087
```

Verify the backend in another terminal:

```bash
curl -fsS http://127.0.0.1:5087/api/health
curl -fsS http://127.0.0.1:5087/swagger/v1/swagger.json
```

Expected health response:

```json
{"status":"ok"}
```

Start the frontend:

```bash
cd src/frontend
npm install
npm run dev
```

Open the Vite URL printed by the command, normally `http://localhost:5173`.
The frontend dev server proxies `/api` requests to `http://localhost:5087`.

## Test And Verify

Run the frontend test suite:

```bash
cd src/frontend
npm test
```

Run TypeScript checks:

```bash
cd src/frontend
npm run typecheck
```

Run a production build:

```bash
cd src/frontend
npm run build
```

Current phase gate verification is:

```bash
cd src/frontend
npm test
npm run typecheck
npm run build
```

Then run backend domain and smoke gates:

```bash
dotnet run --project tests/integration/Sysml2Editor.Backend.Tests
bash tests/integration/backend-smoke.sh
```

Optionally run the frontend dev-server smoke gate:

```bash
bash tests/integration/frontend-smoke.sh
```

Run the browser-render smoke gate to catch JavaScript runtime errors during real page load:

```bash
bash tests/integration/frontend-browser-smoke.sh
```

You can also start the backend manually and verify `/api/health` and `/swagger/v1/swagger.json` as shown above.

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

Then start the backend and verify `/api/health` and `/swagger/v1/swagger.json` as shown above.

Or run the backend smoke test script:

```bash
bash tests/integration/backend-smoke.sh
```

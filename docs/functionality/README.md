# Functionality Index

This index is the current source of truth for what `sysml2_editor` can do, what is planned, how functionality is tested, and what stability level each area has.

The forward implementation order lives in [Roadmap](../roadmap/roadmap.md). Update this index when capability status, stability, test evidence, or development plans change.

## Structure

- Level 1: product capability area, represented by one file in this folder.
- Level 2: feature group, represented by section headings inside each file.
- Level 3: concrete feature or behavior, represented by feature headings.

## Legends

Status:

- `Available`: implemented and expected to work for the documented scope.
- `Partial`: implemented for a constrained slice, fixture path, backend-only path, or frontend-only path.
- `Planned`: intended but not implemented enough to rely on.
- `Future`: longer-term capability; do not build downstream assumptions on it yet.

Stability:

- `Stable slice`: covered by automated tests and safe to extend within the documented scope.
- `Development`: useful, but still evolving or not fully wired end to end.
- `Prototype`: fixture-backed or preview-oriented behavior.
- `Planned`: not yet implemented.

## Verification Gates

Run the current local gates before treating a capability as healthy:

```bash
cd src/frontend
npm test
npm run typecheck
npm run build
cd ../..
bash tests/integration/backend-smoke.sh
dotnet run --project tests/integration/Sysml2Editor.Backend.Tests
bash tests/integration/frontend-smoke.sh
bash tests/integration/frontend-browser-smoke.sh
```

For full user-workflow testing guidance, see [E2E testing](../testing/e2e-testing.md).

## Level 1 Overview

### 1. Development runtime and testability

- Status: `Available`
- Stability: `Stable slice`
- Detail: [development-runtime.md](./development-runtime.md)

### 2. Workbench UI foundation

- Status: `Available`
- Stability: `Stable slice`
- Detail: [workbench-ui.md](./workbench-ui.md)

### 3. Backend platform and API foundation

- Status: `Available`
- Stability: `Stable slice`
- Detail: [backend-platform.md](./backend-platform.md)

### 4. Repository and workspace context

- Status: `Partial`
- Stability: `Development`
- Detail: [repository-workspace.md](./repository-workspace.md)

### 5. SysML parsing, graph, and source model

- Status: `Available`
- Stability: `Stable slice`
- Detail: [sysml-graph-source.md](./sysml-graph-source.md)

### 6. Read-only model browsing

- Status: `Available`
- Stability: `Development`
- Detail: [model-browsing.md](./model-browsing.md)

### 7. Traceability and analysis

- Status: `Partial`
- Stability: `Development`
- Detail: [traceability-analysis.md](./traceability-analysis.md)

### 8. Editing and write safety

- Status: `Partial`
- Stability: `Development`
- Detail: [editing-write-safety.md](./editing-write-safety.md)

### 9. Git-native workflow

- Status: `Partial`
- Stability: `Development`
- Detail: [git-workflow.md](./git-workflow.md)

### 10. Saved views and custom projections

- Status: `Partial`
- Stability: `Development`
- Detail: [saved-views.md](./saved-views.md)

### 11. Advanced SysML, libraries, CI, and reporting

- Status: `Future`
- Stability: `Planned`
- Detail: [advanced-sysml.md](./advanced-sysml.md)

## Supporting Inventory

- [Fixtures and risk gaps](./fixtures-and-risks.md)

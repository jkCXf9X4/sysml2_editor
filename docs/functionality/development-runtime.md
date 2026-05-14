# 1. Development Runtime And Testability

## 1.1 Local Lifecycle

### Start backend and frontend together

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `bash -n scripts/start.sh`; smoke scripts
- Plan: Keep fixed ports and backend health wait.

### Stop app instances

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `bash -n scripts/stop.sh`
- Plan: Keep scoped to this app and ports.

### Reuse already-running services

- Status: `Available`
- Stability: `Stable slice`
- Tested by: manual bounded `start.sh` check
- Plan: Avoid duplicate bind failures.

### Strict frontend port

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `scripts/start.sh`
- Plan: Keep `--strictPort`.

## 1.2 Automated Gates

### Frontend unit/component tests

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `npm test`
- Plan: Keep fast fixture-backed tests.

### Type and production build gates

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `npm run typecheck`, `npm run build`
- Plan: Required before UI changes are complete.

### Backend domain/integration tests

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `dotnet run --project tests/integration/Sysml2Editor.Backend.Tests`
- Plan: Expand as APIs grow.

### Backend API smoke

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `bash tests/integration/backend-smoke.sh`
- Plan: Keep health/OpenAPI and core endpoint checks.

### Frontend HTTP smoke

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `bash tests/integration/frontend-smoke.sh`
- Plan: Keep as fast server check.

### Real browser render smoke

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `bash tests/integration/frontend-browser-smoke.sh`
- Plan: Keep as runtime JS guard.

### Full workflow E2E runner

- Status: `Planned`
- Stability: `Planned`
- Tested by: none
- Plan: Add Playwright per [E2E testing](../testing/e2e-testing.md).

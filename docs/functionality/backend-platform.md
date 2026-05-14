# 3. Backend Platform And API Foundation

## 3.1 API Runtime

### ASP.NET Core API project

- Status: `Available`
- Stability: `Stable slice`
- Tested by: backend smoke
- Plan: Keep as backend host.

### Health endpoint

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `backend-smoke.sh`
- Plan: Keep stable for orchestration.

### OpenAPI document

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `backend-smoke.sh`
- Plan: Keep current as API evolves.

### Development CORS

- Status: `Available`
- Stability: `Stable slice`
- Tested by: backend smoke and frontend usage
- Plan: Keep scoped to dev frontend.

## 3.2 Frontend API Layer

### Typed API functions

- Status: `Available`
- Stability: `Development`
- Tested by: `npm run typecheck`
- Plan: Keep aligned with backend contracts.

### Fixture fallback for isolated UI tests

- Status: `Available`
- Stability: `Development`
- Tested by: frontend tests
- Plan: Preserve but label fixture-only behavior.

### Backend graph/source/diff loading on mount

- Status: `Partial`
- Stability: `Development`
- Tested by: frontend tests plus backend smoke
- Plan: Add backend-backed browser E2E.

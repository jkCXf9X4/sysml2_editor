# 10. Saved Views And Custom Projections

## 10.1 Saved View Backend

### Create/list/get/update/delete saved views

- Status: `Available`
- Stability: `Development`
- Tested by: `saved_view_crud`
- Plan: Build UI.

### Shared vs local storage mode

- Status: `Available`
- Stability: `Development`
- Tested by: `saved_view_crud`
- Plan: Define persistence semantics.

### In-memory persistence

- Status: `Available`
- Stability: `Development`
- Tested by: `saved_view_crud`
- Plan: Replace or formalize storage before durable use.

### Stable-ID based restoration

- Status: `Partial`
- Stability: `Development`
- Tested by: get endpoint exists; stronger tests needed
- Plan: Add restoration tests.

## 10.2 Saved View UI

### Saved view creation UI

- Status: `Planned`
- Stability: `Planned`
- Tested by: none
- Plan: Build after E2E harness.

### Saved view restore UI

- Status: `Planned`
- Stability: `Planned`
- Tested by: none
- Plan: Add E2E.

### Shared repo views UI

- Status: `Planned`
- Stability: `Planned`
- Tested by: none
- Plan: Needs persistence semantics.

### Local private views UI

- Status: `Planned`
- Stability: `Planned`
- Tested by: none
- Plan: Needs persistence semantics.

### View publishing

- Status: `Planned`
- Stability: `Planned`
- Tested by: none
- Plan: Defer until restoration is reliable.

## 10.3 Custom Projection Views

### Multi-context comparison view

- Status: `Partial`
- Stability: `Development`
- Tested by: phase 3 UI/backend tests
- Plan: Unify with saved views.

### Node-centered inspector trace view

- Status: `Partial`
- Stability: `Prototype`
- Tested by: phase 1 inspector tests
- Plan: Back with query APIs.

### Source ownership view

- Status: `Partial`
- Stability: `Prototype`
- Tested by: phase 1 tests
- Plan: Back with query API.

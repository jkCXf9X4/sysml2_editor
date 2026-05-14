# 4. Repository And Workspace Context

## 4.1 Repository Opening

### Fixture-backed repository cards

- Status: `Available`
- Stability: `Prototype`
- Tested by: `phase0-workbench.test.tsx`, `phase1-browser.test.tsx`
- Plan: Replace primary path with backend workspace data.

### Open repository endpoint

- Status: `Available`
- Stability: `Development`
- Tested by: `open_repository_creates_workspace_context`
- Plan: Add UI flow.

### List workspace contexts

- Status: `Available`
- Stability: `Development`
- Tested by: backend workspace tests
- Plan: Use as frontend source of truth.

### Close workspace context

- Status: `Available`
- Stability: `Development`
- Tested by: `close_workspace_context_removes_from_list`
- Plan: Add UI flow.

## 4.2 Workspace-Scoped Data

### Workspace-scoped graph endpoint

- Status: `Available`
- Stability: `Development`
- Tested by: `get_workspace_graph_returns_parsed_repo`
- Plan: Use for browser E2E.

### Workspace-scoped source endpoint

- Status: `Available`
- Stability: `Development`
- Tested by: backend workspace/source tests
- Plan: Use for source pane E2E.

### Workspace-scoped commit endpoint

- Status: `Available`
- Stability: `Development`
- Tested by: backend Git tests
- Plan: Wire commit UI.

## 4.3 Worktrees And Multi-Context Safety

### Explicit worktree creation endpoint

- Status: `Available`
- Stability: `Development`
- Tested by: backend `CreateWorktree` coverage
- Plan: Add UI and E2E.

### Same-repository multi-branch validation

- Status: `Planned`
- Stability: `Planned`
- Tested by: none
- Plan: Add backend validation.

### Multiple-repository workspace validation

- Status: `Planned`
- Stability: `Planned`
- Tested by: none
- Plan: Add backend tests and UI.

### Save target scoping

- Status: `Planned`
- Stability: `Planned`
- Tested by: none direct
- Plan: Enforce by workspace context.

### Cross-context write rejection

- Status: `Planned`
- Stability: `Planned`
- Tested by: none
- Plan: Add safety-critical backend tests.

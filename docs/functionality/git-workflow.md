# 9. Git-Native Workflow

## 9.1 Branch Comparison UI

### Branch switcher

- Status: `Available`
- Stability: `Prototype`
- Tested by: `phase3-git-workflow.test.tsx`
- Plan: Wire to backend workspace/branch state.

### Side-by-side branch contexts

- Status: `Available`
- Stability: `Prototype`
- Tested by: `phase3-git-workflow.test.tsx`
- Plan: Backend-backed E2E.

### Visual diff

- Status: `Available`
- Stability: `Prototype`
- Tested by: `phase3-git-workflow.test.tsx`
- Plan: Backend-backed E2E.

### Text diff

- Status: `Available`
- Stability: `Prototype`
- Tested by: `phase3-git-workflow.test.tsx`
- Plan: Backend-backed E2E.

### Split comparison

- Status: `Available`
- Stability: `Prototype`
- Tested by: `phase3-git-workflow.test.tsx`
- Plan: Backend-backed E2E.

### Changed-node/file highlights

- Status: `Available`
- Stability: `Prototype`
- Tested by: `phase3-git-workflow.test.tsx`
- Plan: Backend-backed E2E.

### Merge conflict assistance UI

- Status: `Partial`
- Stability: `Prototype`
- Tested by: `phase3-git-workflow.test.tsx`
- Plan: Display backend merge-preview diagnostics.

## 9.2 Backend Git Operations

### Semantic branch diff

- Status: `Available`
- Stability: `Development`
- Tested by: `semantic_branch_diff`
- Plan: Generalize beyond fixture subset carefully.

### Git status

- Status: `Available`
- Stability: `Development`
- Tested by: `git_branch_diff_and_status`
- Plan: Wire UI.

### Git commit

- Status: `Available`
- Stability: `Development`
- Tested by: `git_commit_persists_changes`
- Plan: Wire UI.

### Merge conflict preview

- Status: `Available`
- Stability: `Development`
- Tested by: `merge_conflict_preview_detects_conflict`
- Plan: Wire UI.

### Workspace-scoped commit

- Status: `Available`
- Stability: `Development`
- Tested by: backend workspace/Git tests
- Plan: Use for commit UI.

## 9.3 Commit Workflow UI

### Commit preview

- Status: `Available`
- Stability: `Prototype`
- Tested by: `phase3-git-workflow.test.tsx`
- Plan: Keep but add real commit path.

### Commit through backend

- Status: `Planned`
- Stability: `Planned`
- Tested by: backend API exists; UI not wired
- Plan: Add Playwright E2E.

### Working-tree overlay from backend

- Status: `Planned`
- Stability: `Planned`
- Tested by: backend API exists; UI fixture-backed
- Plan: Wire UI.

# 2. Workbench UI Foundation

## 2.1 Shell Layout

### Top app bar

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `phase0-workbench.test.tsx`
- Plan: Preserve command/context visibility.

### Left rail

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `phase0-workbench.test.tsx`
- Plan: Extend with real repository/workspace actions.

### Tiled workspace panes

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `phase0-workbench.test.tsx`
- Plan: Keep visual/text/split/diff surfaces consistent.

### Inspector panel

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `phase0-workbench.test.tsx`
- Plan: Connect more backend trace/query data.

### Status bar

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `phase0-workbench.test.tsx`
- Plan: Surface backend/runtime/write diagnostics.

## 2.2 Context Identity

### Repository labels

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `phase0-workbench.test.tsx`
- Plan: Preserve on every workflow surface.

### Branch labels

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `phase0-workbench.test.tsx`, `phase3-git-workflow.test.tsx`
- Plan: Tie to backend workspace context.

### File labels

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `phase0-workbench.test.tsx`, `phase1-browser.test.tsx`
- Plan: Tie to selected source ownership.

### Mode labels

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `phase0-workbench.test.tsx`
- Plan: Keep pane-local state.

### Writable/read-only state

- Status: `Partial`
- Stability: `Prototype`
- Tested by: `phase0-workbench.test.tsx`
- Plan: Wire to backend workspace/write policy.

### Dirty/validation indicators

- Status: `Partial`
- Stability: `Prototype`
- Tested by: `phase0-workbench.test.tsx`, `phase2-editing.test.tsx`
- Plan: Wire to backend diagnostics and Git status.

## 2.3 Responsive Behavior

### CSS responsive fallback

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `phase0-workbench.test.tsx` CSS assertions
- Plan: Add viewport-level browser checks if layout becomes risky.

### Context labels preserved on narrower screens

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `phase0-workbench.test.tsx`
- Plan: Preserve as UI grows.

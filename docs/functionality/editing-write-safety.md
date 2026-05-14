# 8. Editing And Write Safety

## 8.1 Visual Draft Editing

### Type palette interaction

- Status: `Available`
- Stability: `Prototype`
- Tested by: `phase2-editing.test.tsx`
- Plan: Wire to backend save path.

### Drag-to-create

- Status: `Available`
- Stability: `Prototype`
- Tested by: `phase2-editing.test.tsx`
- Plan: Backend-backed E2E.

### Click-to-create

- Status: `Available`
- Stability: `Prototype`
- Tested by: `phase2-editing.test.tsx`
- Plan: Backend-backed E2E.

### Draft package/part/requirement creation

- Status: `Available`
- Stability: `Prototype`
- Tested by: `phase2-editing.test.tsx`
- Plan: Save/reload E2E.

### Draft connector creation

- Status: `Available`
- Stability: `Prototype`
- Tested by: `phase2-editing.test.tsx`
- Plan: Add backend relationship tests.

### Draft ports/features

- Status: `Available`
- Stability: `Prototype`
- Tested by: `phase2-editing.test.tsx`
- Plan: Add backend port/feature tests.

### Inline rename

- Status: `Available`
- Stability: `Prototype`
- Tested by: `phase2-editing.test.tsx`
- Plan: Stable-ID save/reload E2E.

### Undo/redo

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `phase2-editing.test.tsx`
- Plan: Keep UI-local.

## 8.2 Source Preview And Validation

### Generated source preview

- Status: `Available`
- Stability: `Prototype`
- Tested by: `phase2-editing.test.tsx`
- Plan: Keep before writes.

### Intended save target display

- Status: `Available`
- Stability: `Prototype`
- Tested by: `phase2-editing.test.tsx`
- Plan: Wire to backend owner-file policy.

### Draft validation feedback

- Status: `Available`
- Stability: `Prototype`
- Tested by: `phase2-editing.test.tsx`
- Plan: Wire backend diagnostics.

### Auto-layout feedback

- Status: `Available`
- Stability: `Prototype`
- Tested by: `phase2-editing.test.tsx`
- Plan: Replace with real layout later if needed.

## 8.3 Backend Write Operations

### Writer round-trip

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `parse_round_trip_minimal`
- Plan: Keep as regression gate.

### Rename/save

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `stable_id_survives_rename`, `save_touches_only_owner`
- Plan: Wire UI E2E.

### Missing identity write block

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `missing_id_blocks_write_until_backfill`
- Plan: Surface actionable UI error.

### Create/delete supported elements

- Status: `Available`
- Stability: `Development`
- Tested by: `create_and_delete_supported_element`
- Plan: Broaden test coverage.

### Create ports/features

- Status: `Partial`
- Stability: `Development`
- Tested by: backend support exists; explicit tests missing
- Plan: Add backend tests.

### Create relationships

- Status: `Partial`
- Stability: `Development`
- Tested by: backend support exists; explicit tests missing
- Plan: Add backend tests.

### Reload graph after write

- Status: `Planned`
- Stability: `Planned`
- Tested by: none complete
- Plan: Add edit-save-reload E2E.

# Product Breakdown Structure

This breakdown groups the product subsystems and deliverables. It is not the build order and it is not the source of truth for capability status.

## Shell And Coordination

- `PB-01` Workbench shell and context chrome - top app bar, rails, tiled workspace, inspector, status bar, and visible context labels. Main functional blocks: FB-01, FB-03, FB-07, FB-08.
- `PB-02` Workspace and repository coordinator - repository open/close, branch and worktree tracking, writable vs read-only context selection. Main functional blocks: FB-01, FB-06, FB-07.

## Model And Inspection

- `PB-03` Model core service - parsing, graph building, stable IDs, source mapping, and core diagnostics. Main functional blocks: FB-02, FB-11.
- `PB-04` Browsing and inspection surfaces - tree, graph, source, search, inspector, and selection sync views. Main functional blocks: FB-03, FB-04, FB-09.

## Editing And Persistence

- `PB-05` Visual editing surface - type palette, canvas, inline rename, draft staging, and generated source preview. Main functional blocks: FB-05, FB-06.
- `PB-06` Persistence and write-policy service - save planning, owner-file resolution, safety checks, and reload after write. Main functional blocks: FB-06, FB-11.

## Git And Analysis

- `PB-07` Git workflow service - status, semantic diff, commit, and merge-preview behavior. Main functional blocks: FB-07.
- `PB-08` Traceability and analysis service - source ownership, trace matrix, impact analysis, and dependency paths. Main functional blocks: FB-04, FB-09.

## Views And Extensibility

- `PB-09` Saved views and projection service - save, restore, share, and reopen view projections using stable identity. Main functional blocks: FB-08.
- `PB-10` Advanced SysML capability pack - later parser, graph, validation, and editor extensions beyond the MVP slice. Main functional blocks: FB-10.
- `PB-11` Shared contracts and identity policy - DTOs, stable IDs, context identifiers, and cross-layer invariants. Main functional blocks: FB-01, FB-02, FB-06, FB-11.

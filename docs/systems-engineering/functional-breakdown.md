# Functional Breakdown

This breakdown groups what the product must do. It is derived from the use cases and is intentionally independent of implementation order.

## Context And Core Model

- `FB-01` Workspace and context management - create, close, and label repository, branch, and worktree contexts; preserve writable vs read-only state. Use cases: UC-01, UC-05, UC-08, UC-09. Anchors: [repository-workspace](../functionality/repository-workspace.md), [workbench-ui](../functionality/workbench-ui.md).
- `FB-02` Model ingestion and indexing - discover SysML files, parse text, build the model graph, and record diagnostics. Use cases: UC-01, UC-04, UC-12. Anchors: [sysml-graph-source](../functionality/sysml-graph-source.md), [model-browsing](../functionality/model-browsing.md).
- `FB-11` Diagnostics and recovery - surface invalid input, unsafe writes, missing identity, and conflict conditions clearly. Use cases: UC-04, UC-07, UC-08. Anchors: [model-browsing](../functionality/model-browsing.md), [editing-write-safety](../functionality/editing-write-safety.md), [git-workflow](../functionality/git-workflow.md).

## Browsing And Trace Views

- `FB-03` Browsing and selection sync - keep tree, graph, source, inspector, and search views aligned to the same selection. Use cases: UC-02, UC-03, UC-04. Anchors: [model-browsing](../functionality/model-browsing.md), [workbench-ui](../functionality/workbench-ui.md).
- `FB-04` Traceability projection - expose item-to-item, item-to-file, file-to-file, and branch-to-branch trace links. Use cases: UC-03, UC-05, UC-11. Anchors: [traceability-analysis](../functionality/traceability-analysis.md).
- `FB-09` Analysis and query - answer impact, dependency, ownership, and matrix-style questions from model data. Use cases: UC-03, UC-11. Anchors: [traceability-analysis](../functionality/traceability-analysis.md), [saved-views](../functionality/saved-views.md).

## Editing And Persistence

- `FB-05` Editing and drafting - create, rename, connect, and stage supported model changes in a draft state. Use cases: UC-06. Anchors: [editing-write-safety](../functionality/editing-write-safety.md).
- `FB-06` Write policy and persistence - choose the owning file, protect stable identity, and save only to a valid writable context. Use cases: UC-07, UC-04. Anchors: [editing-write-safety](../functionality/editing-write-safety.md), [repository-workspace](../functionality/repository-workspace.md).
- `FB-07` Git workflow - expose status, diff, commit, and merge-preview behavior for the active context. Use cases: UC-08, UC-09. Anchors: [git-workflow](../functionality/git-workflow.md).

## Projections And Expansion

- `FB-08` Saved views and projections - persist, restore, and compare saved projections without turning them into separate models. Use cases: UC-10, UC-09. Anchors: [saved-views](../functionality/saved-views.md).
- `FB-10` Advanced SysML support - expand the supported SysML slice after the core browse/edit/save/commit loop is stable. Use cases: UC-12. Anchors: [advanced-sysml](../functionality/advanced-sysml.md).

# Implementation Readiness Checklist

This checklist captures the decisions that are locked before implementation starts.

Status as of 2026-05-13: the initial design is accepted for scaffolding and for the first read-only implementation slice. Editing, save, and branch-diff behavior are designed but remain gated behind later slices.

## Ready To Scaffold

- [x] Project structure selected
- [x] Runtime target selected
- [x] API contract selected
- [x] MVP parser contract selected
- [x] Internal model schema selected
- [x] File write policy selected
- [x] Starter test matrix selected
- [x] First vertical implementation slice defined

## Scope Decision

The first implementation slice is read-only. It must prove that repository opening, SysML file discovery, MVP parsing, model graph projection, and basic UI selection work from the same model graph.

The first slice must not implement save, visual editing, commit, or branch comparison. Those behaviors are specified so the architecture does not block them later, but they are not readiness blockers for the first read-only branch.

## Decision Links

- Structure: [project-structure.md](./project-structure.md)
- Runtime: [runtime-decision.md](./runtime-decision.md)
- API: [api-contract.md](./api-contract.md)
- Parser contract: [parser-contract.md](./parser-contract.md)
- Model schema: [model-schema.md](./model-schema.md)
- Write policy: [write-policy.md](./write-policy.md)
- Starter tests: [starter-test-matrix.md](./starter-test-matrix.md)

## Slice 0: Scaffold Complete When

The scaffold branch is ready when all of the following are true:

- The documented top-level project structure exists.
- Backend solution and projects exist under `src/backend`.
- Frontend app shell exists under `src/frontend`.
- Test folders exist under `tests`.
- A local dev command exists and launches the app shells.
- OpenAPI is exposed by the backend in development.

## Slice 1: Read-Only Browser Complete When

The first read-only implementation branch is ready when all of the following are true:

- A local dev command exists and launches the app.
- A tiny fixture repo can be opened without errors.
- The MVP parser can parse the tiny fixture into `ModelGraphDto`.
- Malformed input reports diagnostics without preventing valid files from loading.
- The first UI path can show tree, read-only graph, source text, and inspector views from the same model.
- No save path is exposed in the UI or API.

## Later Gates

Writer, stable-ID backfill, save ownership, semantic branch diff, visual editing, and Git commit behavior are intentionally later gates. They should not block the first read-only browser implementation.

## First Code Slice

The first code slice should be limited to one narrow path:

1. Open a local repo.
2. Parse a tiny fixture.
3. Render the model tree and inspector.
4. Show a read-only graph view.
5. Save nothing.

That slice should prove the app can load model data correctly before editing is allowed.

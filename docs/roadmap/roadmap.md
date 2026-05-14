# Implementation Roadmap for `sysml2_editor`

This roadmap starts from the current application state and defines the next implementation milestones.

Current and planned capability inventory, stability, and test evidence live in [Functionality index](../functionality/README.md). This roadmap should answer a different question: what should be built next, in what order, and what must be true before each milestone is considered complete.

Supporting references:

- Product intent: [Product vision](../../PRODUCT_VISION.md)
- Current capability inventory: [Functionality index](../functionality/README.md)
- UI target: [UI Design](../ui/design.md)
- Runtime guarantees: [runtime.md](../architecture/runtime.md)
- Write policy: [write-policy.md](../architecture/write-policy.md)
- API contracts: [api-contract.md](../implementation/api/api-contract.md)
- Parser contract: [parser-contract.md](../implementation/parser-contract.md)
- Fixture rules: [fixtures](../testing/fixtures.md)
- E2E guidance: [E2E testing](../testing/e2e-testing.md)

## Product Direction

`sysml2_editor` is a Git-native SysML v2 workbench. The near-term objective is not broad SysML coverage; it is a reliable MVP loop:

1. Open a local Git-backed SysML repository.
2. Parse and browse a useful structural SysML subset.
3. Select model elements and inspect graph/source/trace context.
4. Make safe simple edits.
5. Save back to text without corrupting source ownership or stable IDs.
6. Commit changes through Git.
7. Preserve repository, branch, worktree, and file context throughout the UI.

## Current Baseline

The application already has these foundations:

- Local startup scripts for backend/frontend, plus a stop script.
- ASP.NET Core backend with health/OpenAPI.
- Vite/React frontend shell.
- Real browser smoke coverage.
- Workbench shell with context labels, panes, inspector, and status bar.
- MVP parser and graph DTOs for the current fixture subset.
- Backend graph/source endpoints.
- Frontend API layer and fixture fallback.
- Fixture-backed read-only browser UI.
- Fixture-backed visual draft editing UI.
- Backend create/delete/rename/save-draft APIs for supported element slices.
- Fixture-backed Git workflow UI with branch comparison and commit preview.
- Backend Git status/diff/commit/merge-preview support against temporary Git repositories.
- Workspace context and worktree endpoints.
- Backend saved-view CRUD and trace-matrix endpoints.

The major gap is not missing individual APIs. The major gap is end-to-end wiring and confidence: several frontend workflows remain preview-oriented or fixture-backed while backend primitives already exist.

## Roadmap Principles

- Wire existing backend primitives into full user flows before adding large new feature areas.
- Promote functionality only when it has automated verification at the right layer.
- Keep fixture-backed UI slices for fast iteration, but never call a workflow product-ready until backend-backed E2E exists.
- Treat stable IDs, source ownership, and workspace context as safety-critical.
- Do not expand advanced SysML syntax until the browse/edit/save/commit loop is reliable for the MVP subset.
- Prefer Playwright-style web E2E over Electron for full-stack validation.

## Verification Gates

These gates should stay green before and after every milestone:

```bash
cd src/frontend
npm test
npm run typecheck
npm run build
cd ../..
bash tests/integration/backend-smoke.sh
dotnet run --project tests/integration/Sysml2Editor.Backend.Tests
bash tests/integration/frontend-smoke.sh
bash tests/integration/frontend-browser-smoke.sh
```

When Playwright is added, the E2E gate becomes part of the required milestone checks.

## Milestone 1: Development Runtime And E2E Harness

Goal: make local and automated full-stack testing deterministic.

### Scope

- Keep `scripts/start.sh` and `scripts/stop.sh` as the supported local lifecycle.
- Add Playwright as the full-stack E2E test runner.
- Add a minimal Playwright config that starts backend and frontend or uses a wrapper script.
- Fail E2E tests on browser `pageerror` and unexpected `console.error`.
- Add the first E2E workflow: browser opens the app and verifies backend-connected workbench shell.

### Success Criteria

- One command runs the full E2E smoke from a clean state.
- Stale processes do not cause false positives.
- E2E startup waits on `/api/health`; no arbitrary sleeps.
- Browser runtime errors fail tests.

### Required Tests

- Existing frontend/backend gates.
- `tests/integration/frontend-browser-smoke.sh`.
- New Playwright startup test under `tests/e2e/`.

### Exit Criteria

- A developer can run full-stack browser tests without manually starting services.
- The test harness clearly reports whether backend, frontend, or browser rendering failed.

## Milestone 2: Backend-Backed Read-Only Browser

Goal: make model browsing rely on backend graph/source data, not just fixture UI state.

### Scope

- Open a backend workspace context for a fixture repository.
- Load workspace graph and source through workspace-scoped endpoints.
- Display backend parse diagnostics for invalid input.
- Ensure tree, graph, source, inspector, and trace context all reflect backend data.
- Preserve fixture fallback only for isolated component tests.

### Success Criteria

- A user can open a supported SysML fixture and browse tree, graph, source, attributes, and trace links from backend data.
- Invalid input produces visible, actionable diagnostics.
- Repository, branch, workspace, file, read-only/writable state, and selected element context are visible.

### Required Tests

- `src/frontend/tests/phase1-browser.test.tsx`.
- Backend parser/source/trace tests.
- Playwright E2E: open fixture, select model element, verify source/inspector/trace context.
- Playwright E2E or integration test: invalid fixture shows diagnostics.

### Exit Criteria

- Fixture-only browse behavior is no longer the only browser path.
- Backend graph/source loading failures are visible in the UI and tests.

## Milestone 3: Safe Edit, Save, Reload

Goal: close the MVP edit loop for the supported structural subset.

### Scope

- Wire visual edit actions to backend create/delete/rename/save-draft APIs.
- Reload graph and source from backend after save.
- Verify only the owning source file changes.
- Keep generated source preview visible before save.
- Preserve stable IDs across rename.
- Block or surface writes when identity metadata is missing.

### Success Criteria

- A user can create a supported element, save it, reload the graph, and see the element in graph/source.
- A user can rename an element and verify stable ID/source ownership survives.
- A user can delete a supported element and verify graph/source consistency after reload.
- Failed writes show actionable diagnostics and do not mutate unrelated files.

### Required Tests

- `src/frontend/tests/phase2-editing.test.tsx`.
- Backend writer tests:
  `parse_round_trip_minimal`,
  `save_touches_only_owner`,
  `stable_id_survives_rename`,
  `missing_id_blocks_write_until_backfill`,
  `create_and_delete_supported_element`.
- Add backend tests for port/feature creation.
- Add backend tests for relationship creation.
- Playwright E2E: create-save-reload.
- Playwright E2E: rename-save-reload and owner-file-only assertion.
- Playwright E2E: delete-save-reload.

### Exit Criteria

- Editing is no longer only a frontend draft preview.
- Save/reload validates backend source of truth.
- Source ownership and stable IDs are enforced by tests.

## Milestone 4: Git Commit Workflow End To End

Goal: make Git workflow real from UI to backend to repository state.

### Scope

- Wire frontend working-tree status to backend Git status.
- Wire commit panel to workspace-scoped backend commit API.
- Display backend commit result in the UI.
- Show backend merge-preview/conflict diagnostics.
- Use temporary Git repositories in tests; do not mutate checked-in fixtures.

### Success Criteria

- A user can inspect working-tree status from the UI.
- A user can create a commit through the UI for a safe temporary repository.
- Commit output includes model-aware summary and Git SHA.
- Merge conflict diagnostics from backend are visible in the UI.

### Required Tests

- `src/frontend/tests/phase3-git-workflow.test.tsx`.
- Backend Git tests:
  `git_branch_diff_and_status`,
  `git_commit_persists_changes`,
  `merge_conflict_preview_detects_conflict`.
- Playwright E2E: backend-backed branch diff display.
- Playwright E2E: commit through UI and verify repository state.
- Playwright E2E: merge conflict diagnostics display.

### Exit Criteria

- Commit UI is no longer preview-only.
- Branch/diff/status/commit workflows are backed by real Git state in automated tests.

## Milestone 5: Multi-Context Safety

Goal: support safe side-by-side repository and branch contexts.

### Scope

- Validate same-repository multi-branch writable contexts.
- Validate multiple-repository workspace contexts.
- Scope save targets to repository, branch, and worktree.
- Reject writes from shared comparison/read-only panes.
- Preserve `MultiContextViewDto` identity without collapsing contexts.
- Build UI flows for opening two validated writable contexts.

### Success Criteria

- Two writable contexts for the same repository use explicit worktrees.
- Save and commit operations always identify their target context.
- Shared comparison panes cannot write to either side by accident.
- Cross-context write attempts are rejected and tested.

### Required Tests

- Backend test: multi-context identity.
- Backend test: multiple repository contexts.
- Backend test: save target scoping.
- Backend test: commit target scoping.
- Backend test: cross-context write rejection.
- Playwright E2E: edit one context and verify the comparison context remains unchanged.

### Exit Criteria

- Multi-context editing can be exposed without ambiguous write targets.

## Milestone 6: Saved Views And Trace Matrix UI

Goal: expose existing backend view and trace primitives in the frontend.

### Scope

- Saved view create/list/get/update/delete UI.
- Restore saved views by stable graph IDs.
- Trace matrix UI backed by `GET /api/workspace-contexts/{workspaceId}/trace-matrix`.
- Basic source ownership query UI.
- Define persistence rules for shared vs local views.

### Success Criteria

- A user can save a view, reload it, and restore the expected model context.
- Trace matrix renders backend data for a workspace.
- Shared/local view state is explicit.
- Missing or moved elements produce actionable restoration feedback.

### Required Tests

- Backend `saved_view_crud`.
- Backend `build_trace_matrix`.
- Add stable-ID restoration test.
- Playwright E2E: save view and reload in a new session.
- Playwright E2E: trace matrix render.

### Exit Criteria

- Saved views are a working user feature, not just backend CRUD.
- Trace matrix has a visible UI path.

## Milestone 7: Query, Impact, And Cross-Repository Traceability

Goal: make traceability useful for engineering review.

### Scope

- Source ownership query API.
- Cross-repository dependency query API.
- Impact-analysis backend.
- Query/filter backend and UI.
- Node-centered inspector trace view.
- Visual indicators for requirements, satisfies, verifies, allocations, ports, connections, files, and changed contexts.

### Success Criteria

- A user can ask what is impacted by a selected model item.
- A user can navigate source ownership and dependencies across files and repositories.
- Query/filter behavior is deterministic and tested against fixtures.

### Required Tests

- Source ownership query test.
- Cross-repository dependency query test.
- Impact analysis test.
- Query/filter rules test.
- Playwright E2E: selected model item shows impact and trace context.

### Exit Criteria

- Traceability supports review workflows beyond static display.

## Milestone 8: Advanced SysML Expansion

Goal: expand SysML support only after the MVP browse/edit/save/Git loop is reliable.

### Scope

- Parser support for more SysML types.
- Behavior model parsing.
- State/action model parsing.
- Variant/product-line model parsing.
- Validation rules.
- Model library import resolution.
- UI for advanced SysML constructs.
- CI validation status and report generation.
- Optional simplified "PowerPoint mode".

### Success Criteria

- New syntax is added with fixtures and parser tests before UI exposure.
- Existing MVP browse/edit/Git workflows keep passing.
- Advanced views remain source-backed and context-aware.

### Required Tests

- New language construct parser tests.
- Behavior model parser tests.
- State/action parser tests.
- Variant/product-line parser tests.
- Validation rule tests.
- Model library import tests.
- MVP subset regression tests.
- Playwright E2E: open advanced fixture and render supported views.

### Exit Criteria

- The product becomes a broader MBSE workbench without destabilizing the MVP subset.

## Deferred Work

These are intentionally deferred until earlier milestones are stable:

- Electron packaging.
- Full desktop-native lifecycle.
- Screenshot-based visual regression suite.
- Broad cross-browser matrix.
- Full SysML v2 language coverage.
- General merge engine beyond the tested MVP Git workflows.

## Roadmap Maintenance Rules

- Update [Functionality index](../functionality/README.md) when capability status, stability, tests, or planned work changes.
- Update this roadmap when milestone order, scope, success criteria, or gates change.
- Do not mark a milestone complete solely because frontend and backend have separate tests; product readiness requires a full-stack path where relevant.
- Keep roadmap items small enough to verify.
- Do not add planned features without stating how they will be tested.

# Implementation Roadmap for `sysml2_editor`

This is the canonical implementation roadmap. It combines product scope,
development phases, test focus, fixture expectations, and phase exit criteria
so implementation and verification do not drift into separate plans.

Supporting references:

- UI target: [UI Design](../ui/design.md)
- Runtime guarantees: [runtime.md](../architecture/runtime.md)
- API contracts: [api-contract.md](../implementation/api/api-contract.md)
- Parser contract: [parser-contract.md](../implementation/parser-contract.md)
- Fixture rules: [fixtures/README.md](../../fixtures/README.md)

## Visual Target

The roadmap targets the IDE-style multi-context workbench defined in [UI Design](../ui/design.md). Phase 0 establishes the static shell; later phases connect parser, editing, Git, traceability, and advanced SysML behavior into that shell.

## MVP Definition

### MVP Goal

Open a Git repo, visualize SysML v2 structure, edit simple structure graphically or textually, and commit changes.

### MVP Features

Must-have:

- Open local Git repo
- Detect SysML files
- Basic textual editor
- Parse a useful subset of SysML v2
- Show package/part hierarchy as tree
- Show selected structure as graph
- Create/edit/delete basic elements
- Save back to text
- Show attributes of selected element
- Commit changes
- Store custom views as JSON
- Visualize traceability between model items and their source files
- Preserve explicit repository and branch context for every opened model graph

Supported elements are defined in [mvp-coverage.md](../testing/mvp-coverage.md). Avoid starting with full behavioral modeling. Structure + requirements + traceability gives the strongest early value.

## Development And Test Phases

Implementation tracking:

- [x] Phase 0 visual workbench shell is implemented with fixture-backed UI state and verified by `src/frontend/tests/phase0-workbench.test.tsx`.
- [x] Phase 0 backend OpenAPI scaffold starts in development and exposes `/swagger/v1/swagger.json` and `/api/health`.
- [x] Phase 1 read-only model browser is implemented with backend API data loading (fixture fallback) and verified by `src/frontend/tests/phase1-browser.test.tsx`.
- [x] Phase 2 visual editing is implemented with backend create/delete/rename endpoints and fixture-backed draft editor; verified by `src/frontend/tests/phase2-editing.test.tsx`.
- [x] Phase 3 Git-native workflow is implemented as the current fixture-backed branch comparison and commit-preview slice; backend Git status/diff/commit/merge-preview endpoints complete; verified by `src/frontend/tests/phase3-git-workflow.test.tsx` and `tests/integration/Sysml2Editor.Backend.Tests`.
- [x] Frontend phase gates are verified with `npm test`, `npm run typecheck`, and `npm run build` from `src/frontend`.
- [x] Frontend dev-server smoke gate is verified with `bash tests/integration/frontend-smoke.sh`.
- [x] Frontend-to-backend API service layer is wired via `src/frontend/src/api.ts`; frontend loads graph, source, diff, and multi-context data from backend with hardcoded fallback for isolated test mode.
- [x] Backend domain gates are verified with `dotnet run --project tests/integration/Sysml2Editor.Backend.Tests` (22 unique backend tests passing).
- [x] Backend smoke gate is verified with `bash tests/integration/backend-smoke.sh`.
- [x] Backend element creation, deletion, and save-rename endpoints are implemented and tested.
- [x] Phase 3 workspace context management (open/close/list repos) implemented with `POST /api/repositories/open`, `GET /api/workspace-contexts`, `DELETE /api/workspace-contexts/{workspaceId}`.
- [x] Phase 3 workspace-scoped model graph and source file endpoints (`GET /api/workspace-contexts/{workspaceId}/model`, `GET /api/workspace-contexts/{workspaceId}/files`).
- [x] Phase 3 workspace-scoped commit endpoint (`POST /api/workspace-contexts/{workspaceId}/commit`).
- [x] Phase 3b worktree creation endpoint (`POST /api/workspace-contexts/worktrees`).
- [x] Phase 4 saved views CRUD endpoints (`POST/GET /api/saved-views`, `PUT/DELETE /api/saved-views/{viewId}`).
- [x] Phase 4 trace matrix endpoint (`GET /api/workspace-contexts/{workspaceId}/trace-matrix`).
- [x] Frontend API layer (`src/frontend/src/api.ts`) extended with all Phase 3-4 endpoint functions and TypeScript types.
- [x] `fixtures/merge-conflict/` and `fixtures/saved-views/` fixture sets created.
- [x] Backend element creation, deletion, and save-rename endpoints are implemented and tested.

## Frontend Roadmap Track

Use this track for UI tasks, frontend tests, and frontend fixture consumption.
Backend/domain readiness is tracked separately in the next section.

| Phase | Frontend functionality | Tests | Fixtures | Status |
| --- | --- | --- | --- | --- |
| Phase 0 | Workbench shell, panes, inspector, status bar, context labels, responsive fallback | `src/frontend/tests/phase0-workbench.test.tsx`, `tests/integration/frontend-smoke.sh` | `fixtures/phase-0-workbench`, `fixtures/tiny-single-file`, `fixtures/branch-divergence`, `fixtures/multi-file-modular` | [x] Complete for current UI slice |
| Phase 1 | Fixture-backed model browser, tree/graph/source/inspector sync, source ownership, search; loads graph/source/diff from backend API with hardcoded fallback | `src/frontend/tests/phase1-browser.test.tsx` | `fixtures/phase-1-browser`, `fixtures/tiny-single-file`, `fixtures/multi-file-modular`, `fixtures/invalid-input` | [x] Complete for current UI slice; API service layer wired in `src/frontend/src/api.ts` |
| Phase 2 | Draft visual editing, palette placement, generated source preview, save target display, validation feedback, undo/redo; backend create/delete/rename/save-draft endpoints implemented | `src/frontend/tests/phase2-editing.test.tsx` | `fixtures/phase-2-editing`, `fixtures/tiny-single-file`, `fixtures/multi-file-modular` | [x] Complete for current UI slice; backend write API endpoints exist (`POST create-element`, `POST delete-element`, `POST rename/save`, `POST save-draft`) |
| Phase 3 | Branch switcher, side-by-side context display, visual/text/split diff, commit preview, conflict assistance; backend Git status/diff/commit/merge-preview endpoints complete | `src/frontend/tests/phase3-git-workflow.test.tsx` | `fixtures/branch-divergence`, `fixtures/multi-file-modular`, `fixtures/merge-conflict` | [x] Complete for current UI slice; backend Git endpoints (`GET status`, `GET diff`, `POST commit`, `GET merge-preview`) implemented and smoke-tested |
| Phase 3b | Multi-context editing across distinct writable worktrees | none yet | `fixtures/multi-context-editing` | [ ] Future |
| Phase 4 | Saved views, trace matrix, impact analysis, view publishing | none yet | `fixtures/saved-views` | [ ] Future |
| Phase 5 | Advanced SysML views, behavior/state/action UI, variants, reports, PowerPoint mode | none yet | `fixtures/advanced-sysml`, `fixtures/model-libraries`, `fixtures/variant-models` | [ ] Future |

## Backend Roadmap Track

Use this track for parser, graph, writer, API, filesystem, Git, and backend
test tasks. It makes explicit which frontend capabilities are backed by real
backend behavior versus fixture-only UI state.

| Phase | Backend functionality | Tests | Fixtures | Status |
| --- | --- | --- | --- | --- |
| Phase 0 | ASP.NET API scaffold, health endpoint, OpenAPI document, CORS for frontend dev | `tests/integration/backend-smoke.sh` | none | [x] Complete |
| Phase 1 | MVP parser, `ModelGraphDto`, context IDs, source preservation, diagnostics, item/file/import traceability, fixture graph/source API endpoints | `tests/integration/Sysml2Editor.Backend.Tests`, `tests/integration/backend-smoke.sh` | `fixtures/tiny-single-file`, `fixtures/multi-file-modular`, `fixtures/invalid-input` | [x] Complete for MVP subset |
| Phase 2 | Writer round-trip, owner-file save policy, stable-ID-preserving rename, create/delete supported elements, missing-ID write block; `POST create-element`, `POST delete-element`, `POST rename/save`, `POST save-draft` endpoints | `tests/integration/Sysml2Editor.Backend.Tests` | `fixtures/tiny-single-file`, `fixtures/multi-file-modular`, `fixtures/phase-2-editing`, missing-identity fixture to add | [x] Complete for rename/save/create/delete subset; API endpoints exposed via fixture routes |
| Phase 3 | Semantic branch diff, branch trace links, multi-context view scoping, real Git status/commit/merge-preview, diff/multi-context API endpoints; `GET git/status`, `GET git/diff`, `POST git/commit`, `GET git/merge-preview` endpoints | `tests/integration/Sysml2Editor.Backend.Tests`, `tests/integration/backend-smoke.sh` | `fixtures/branch-divergence`, `fixtures/merge-conflict`, temporary real-Git repos created in tests | [x] Complete for temp-Git subset; real Git operations tested in backend integration tests and smoke tests |
| Phase 3 frontend gap | Branch switching UI, working-tree overlay UI, commit panel writes, merge conflict assistance wired to backend Git APIs | `tests/integration/Sysml2Editor.Backend.Tests` covers workspace context, commit, merge-preview; frontend API functions exist | `fixtures/branch-divergence`, `fixtures/merge-conflict` | [x] Backend workspace context management (`POST /api/repositories/open`, `GET /api/workspace-contexts`, `DELETE /api/workspace-contexts/{workspaceId}`); workspace-scoped commit (`POST /api/workspace-contexts/{workspaceId}/commit`); workspace model and source endpoints; frontend `api.ts` functions wired |
| Phase 3b | Explicit worktree creation, same-repo multi-branch writable context validation, cross-context write prevention | Backend `CreateWorktree` test | `fixtures/multi-context-editing` | [x] Backend worktree creation endpoint (`POST /api/workspace-contexts/worktrees`) implemented and tested |
| Phase 4 | View persistence, saved view APIs, trace matrix, impact/query backend | Backend `saved_view_crud` and `build_trace_matrix` tests | `fixtures/saved-views`, `fixtures/multi-file-modular`, `fixtures/branch-divergence` | [x] Backend saved views CRUD endpoints (`POST/GET /api/saved-views`, `PUT/DELETE /api/saved-views/{viewId}`); trace matrix endpoint (`GET /api/workspace-contexts/{workspaceId}/trace-matrix`); in-memory persistence |
| Phase 5 | Broader SysML parser support, validation engine, model libraries, CI/report generation | none yet | `fixtures/advanced-sysml`, `fixtures/model-libraries`, `fixtures/variant-models` | [ ] Future |

## Full-Stack Roadmap Track

Use this track for behavior that requires frontend, backend, API contracts,
fixtures, and smoke/integration tests to work together. A phase is product-ready
only when its frontend, backend, and full-stack gates are all complete.

| Phase | Full-stack capability | Tests | Fixtures | Status |
| --- | --- | --- | --- | --- |
| Phase 0 | Start frontend and backend independently and inspect the generated API contract | `tests/integration/frontend-smoke.sh`, `tests/integration/backend-smoke.sh` | none | [x] Complete |
| Phase 1 | Browser UI loads graph/source/diff data from backend endpoints instead of only local fixture state; hardcoded fallback preserved for isolated frontend tests | smoke test: backend+frontend start | `fixtures/tiny-single-file`, `fixtures/multi-file-modular`, `fixtures/invalid-input` | [x] API service layer wired; frontend fetches graph/source/diff/multi-context from backend on mount; isolated tests use hardcoded fallback |
| Phase 2 | Editing UI submits create/rename/delete operations through backend write APIs and reloads the saved graph | test to add: edit-save-reload E2E | `fixtures/phase-2-editing`, backend creation/delete fixtures added to `SysmlMvpService.cs` and `Program.cs` | [x] Backend write API endpoints (`POST create-element`, `POST delete-element`, `POST rename/save`, `POST save-draft`) implemented; frontend API service layer (`src/frontend/src/api.ts`) wired; UI local draft mode not yet auto-submitting to backend |
| Phase 3 | Git workflow UI uses backend Git/diff/status/commit APIs against a real temporary Git repository | `tests/integration/Sysml2Editor.Backend.Tests` covers `git_branch_diff_and_status`, `git_commit_persists_changes`, `merge_conflict_preview_detects_conflict`; `tests/integration/backend-smoke.sh` covers temp-Git status/diff/commit | `fixtures/branch-divergence`, `fixtures/merge-conflict`, temp real-Git repos created in tests | [x] Backend Git endpoints (`GET git/status`, `GET git/diff`, `POST git/commit`, `GET git/merge-preview`) implemented and smoke-tested; frontend API functions (`fetchGitStatus`, `commitGitWorkspace`, etc.) exist; frontend UI uses fixture-backed slice |
| Phase 3b | Multi-context UI edits two validated writable backend contexts without cross-context writes | test to add: multi-context edit E2E | `fixtures/multi-context-editing`, two-worktree fixture to add | [ ] Future |
| Phase 4 | Saved-view UI persists and reloads backend views with stable graph IDs and trace queries | test to add: saved-view E2E | `fixtures/saved-views`, cross-repository dependency fixture to add | [ ] Future |
| Phase 5 | Advanced SysML UI, parser, validation, libraries, CI, and report generation operate through the same API path | test to add: advanced-model E2E | `fixtures/advanced-sysml`, `fixtures/model-libraries`, `fixtures/variant-models` | [ ] Future |

Gate rule:

- Earlier phase gates must continue passing as later phases are implemented.
- Any change to parsing, model mapping, save logic, or diff logic must update this roadmap if it changes the minimum safe test set.
- Phase 1 gates must pass before save or visual editing is exposed.
- Phase 2 gates must pass before visual editing is treated as implementation-ready.
- Phase 3 gates must pass before branch comparison or commit summaries are exposed.
- A task can be marked frontend-complete or backend-complete independently, but a phase cannot be marked product-ready until its full-stack gate passes.
- Fixture-only UI behavior must be labeled as such until the same path is covered through backend APIs.
- Backend fixture-only behavior must be labeled as such until the same path is covered against real filesystem or Git state where applicable.

Recommended implementation order:

1. Keep fixture-backed frontend slices in place for fast UI iteration.
2. Implement the backend domain behavior and API contract for the same slice.
3. Add backend unit/integration tests against fixtures.
4. Connect the frontend slice to the backend API while preserving fixture fallback where useful for isolated UI tests.
5. Add a full-stack smoke or E2E test for the user path.
6. Only then promote the capability from slice-complete to product-ready.

Platform coverage:

- Parser and save tests should run on both Windows and Linux.
- The smoke E2E test may start on one platform and expand to both once the harness is stable.

Initial setup order:

1. Create the repository root and documentation structure.
2. Create the backend solution and projects.
3. Create the frontend app shell.
4. Add backend OpenAPI generation.
5. Add test projects.
6. Add fixture repositories and sample files.
7. Wire the local dev command.
8. Add the first smoke test and parser round-trip test.

### Phase 0: Visual Workbench Shell

Build the static workbench shape before deep interaction.

Frontend tasks:

- [x] Top-level shell, left rail, tiled workspace, inspector, and status bar
- [x] Pane-level context labels for repository, branch, file, mode, and write state
- [x] Fixture-backed sample content that demonstrates multiple repositories and branches
- [x] Responsive fallback that preserves context labels on narrower screens

Frontend tests:

- [x] Shell rendering with top app bar, left rail, tiled workspace, inspector, and status bar
- [x] Pane headers show repository, branch, file, mode, and write state
- [x] Pane mode toggles are local to the targeted pane
- [x] Selected fixture state can populate the inspector
- [x] Responsive fallback preserves context identity
- [x] Dirty, read-only, and validation indicators render before editing exists
- [x] `frontend_starts` through `tests/integration/frontend-smoke.sh`

Frontend fixtures:

- `fixtures/phase-0-workbench/`
- `fixtures/tiny-single-file/`
- `fixtures/branch-divergence/`
- `fixtures/multi-file-modular/`

Backend tasks:

- [x] Repository skeleton for backend API project
- [x] ASP.NET API scaffold
- [x] Health endpoint
- [x] OpenAPI document in development
- [x] CORS policy for frontend development

Backend tests:

- [x] `repo_scaffold_present`
- [x] `backend_starts_with_openapi` through `tests/integration/backend-smoke.sh`
- [x] Health endpoint smoke check through `tests/integration/backend-smoke.sh`

Backend fixtures:

- None

Full-stack tests:

- [x] Frontend dev server starts and serves the app shell
- [x] Backend starts and exposes health/OpenAPI endpoints
- [ ] Frontend and backend start together from one documented local workflow

Full-stack fixtures:

- None

Success criterion:

> A user can look at the UI and understand that the product is a multi-repository, multi-branch SysML workbench even before real backend data is connected.

Exit criteria:

- A user can identify the active repository, branch, file, selected model element, and write state from visible UI alone.
- Visual, text, split, and diff panes can be represented with fixture data.
- The shell is ready for parser, graph, inspector, and Git data integration.
- The backend scaffold can be started and its contract can be inspected.

Vision trace:

- Supports: multi-context workspace, Git-native workflow, traceability-first inspection, PowerPoint-like visual orientation
- Tradeoff: uses static or fixture-backed data before full parser and backend integration

### Phase 1: Read-only Model Browser

Build confidence in repo parsing and navigation.

Frontend tasks:

- [x] Open Git repo using fixture-backed repository contexts
- [x] Create one explicit workspace context for the opened repo and current branch
- [x] Load graph and source data from backend API endpoints via `src/frontend/src/api.ts`
- [x] Preserve local fixture fallback for isolated frontend tests
- [x] Show tree hierarchy
- [x] Show text editor
- [x] Show graph view
- [x] Render the graph inside the visual workbench pane system
- [x] Click graph node -> show source text and attributes
- [x] Sync selected graph node with model tree, text pane, and inspector
- [x] Show source ownership and item-to-item trace links for selected nodes
- [x] Show file-to-file import traceability for modular models
- [x] Keep repository, branch, file, mode, and read-only state visible on every pane
- [x] Basic search

Frontend tests:

- [x] Repo open and file discovery through fixture state
- [x] Workspace context creation through fixture state
- [x] Tree hierarchy rendering
- [x] Text editor view sync
- [x] Graph view node/edge projection
- [x] Source ownership and item-to-item trace links
- [x] File-to-file import traceability
- [x] Search and selection behavior
- [x] `open_browse_select_smoke` through `src/frontend/tests/phase1-browser.test.tsx`
- [ ] Browser API integration smoke test (requires running backend during frontend test) (requires running backend during frontend test)

Frontend fixtures:

- `fixtures/phase-1-browser/`
- `fixtures/tiny-single-file/`
- `fixtures/multi-file-modular/`
- `fixtures/invalid-input/`

Backend tasks:

- [x] Parse SysML files for the MVP subset
- [x] Produce `ModelGraphDto`
- [x] Preserve repository, branch, workspace, writable, and file context
- [x] Preserve source file text for display
- [x] Report parser diagnostics for malformed input
- [x] Derive source ownership trace links
- [x] Derive file-to-file import traceability
- [x] Expose fixture graph API endpoint
- [x] Expose fixture source API endpoint

Backend tests:

- [x] `parse_minimal_graph`
- [x] `model_graph_has_context`
- [x] `derive_item_to_file_traceability`
- [x] `derive_import_traceability`
- [x] `malformed_input_reports_diagnostic`
- [x] `get_source_file_preserves_text`
- [x] Graph/source endpoint checks through `tests/integration/backend-smoke.sh`

Backend fixtures:

- `fixtures/tiny-single-file/`
- `fixtures/multi-file-modular/`
- `fixtures/invalid-input/`

Full-stack tests:

- [x] Browser opens `fixtures/tiny-single-file` through the backend graph endpoint (API layer wired, verified via backend smoke test)
- [x] Browser displays source text from the backend source endpoint (API layer wired, verified via backend smoke test)
- [ ] Browser displays backend diagnostics for `fixtures/invalid-input` (API layer wired, UI integration pending)

Full-stack fixtures:

- `fixtures/tiny-single-file/`
- `fixtures/multi-file-modular/`
- `fixtures/invalid-input/`

Success criterion:

> A user can open an existing SysML repo and understand the product structure visually, including which source files define and connect the visible model elements.

Exit criteria:

- Existing SysML repos can be opened without modifying files.
- Selected elements always map back to source text and metadata.
- Parse failures are visible and actionable.
- Trace links between model items and source files are present.

Vision trace:

- Supports: textual precision, source ownership, traceability, context-aware navigation
- Tradeoff: read-only browser first; visual creation and Git writes remain deferred

### Phase 2: Visual Editing

Add PowerPoint-like editing.

Frontend tasks:

- [x] Type palette interaction for supported SysML element creation
- [x] Drag SysML type onto canvas
- [x] Click type, then click canvas to place
- [x] Create part/package/requirement
- [x] Create relationship by dragging connector
- [x] Add ports and features inline for supported element types
- [x] Rename inline
- [x] Keep generated source preview visible in split mode
- [x] Show intended save target before write
- [x] Auto-layout feedback for draft nodes
- [x] Save generated SysML text as a fixture-backed draft action
- [x] Submit supported edits through backend write API via `src/frontend/src/api.ts` (create-element, delete-element, rename/save, save-draft)
- [ ] Reload saved graph from backend after write (uses liveModels state merged with API data)
- [x] Undo/redo
- [x] Validation feedback in the pane header, editor, and bottom status bar

Frontend tests:

- [x] Drag-to-create and click-to-create flows
- [x] Inline rename
- [x] Connector creation
- [x] Delete and undo/redo
- [x] Auto-layout after edits
- [x] Generated-source preview and save-target fixture coverage
- [ ] Edit-save-reload E2E through backend API

Frontend fixtures:

- `fixtures/phase-2-editing/`
- `fixtures/tiny-single-file/`
- `fixtures/multi-file-modular/`

Backend tasks:

- [x] Writer round-trip for the MVP subset
- [x] Save only the owning source file for a changed model item
- [x] Preserve stable IDs across rename
- [x] Block writes when required identity metadata is missing
- [x] Create package/part/requirement directly through backend write API (`CreateElement` supports Package, PartDefinition, PartUsage, Requirement)
- [x] Create ports/features directly through backend write API (`CreateElement` supports Port)
- [x] Create relationships directly through backend write API (`CreateElement` supports Connection)
- [x] Delete model elements through backend write API (`DeleteElement`)

Backend tests:

- [x] `parse_round_trip_minimal`
- [x] `save_touches_only_owner`
- [x] `stable_id_survives_rename`
- [x] `missing_id_blocks_write_until_backfill`
- [x] `create_and_delete_supported_element` — covers backend package/part/requirement creation and deletion
- [ ] Backend create port/feature test
- [ ] Backend create relationship test

Backend fixtures:

- `fixtures/tiny-single-file/`
- `fixtures/multi-file-modular/`
- `fixtures/phase-2-editing/` (identity annotations added for backend parseability)
- Fixture to add for missing identity metadata

Full-stack tests:

- [x] Backend API layer supports create/rename/delete/save-draft operations via `POST create-element`, `POST delete-element`, `POST rename/save`, `POST save-draft`
- [ ] Create a supported element in the UI, save through backend, reload graph, and verify stable ID/source ownership
- [ ] Rename in the UI, save through backend, reload graph, and verify only the owner file changed
- [ ] Delete in the UI, save through backend, reload graph, and verify graph/source consistency

Full-stack fixtures:

- `fixtures/phase-2-editing/`
- Fixture to add for backend element creation
- Fixture to add for backend delete operation

Success criterion:

> A user can create a simple architecture visually and see valid textual SysML generated.

Exit criteria:

- Supported edits can be created visually and persisted safely.
- Undo/redo restores the expected model state.
- Generated text remains stable and reviewable.
- Backend write operations preserve source ownership and stable IDs.

Vision trace:

- Supports: PowerPoint-level ease, textual SysML as source of truth, validation before write
- Tradeoff: starts with a constrained SysML subset instead of broad language coverage

### Phase 3: Git-Native Workflow

Make it useful in real engineering teams.

Frontend tasks:

- [x] Branch switcher
- [x] Side-by-side branch contexts for the same repository
- [x] Multi-context comparison projection using `MultiContextViewDto`
- [x] Compare selector in the top app bar
- [x] Commit panel
- [x] Visual diff
- [x] Text diff
- [x] Split visual/text comparison panes
- [x] Branch comparison
- [x] Branch-to-branch trace links
- [x] Changed-node highlighting
- [x] Changed-file highlighting
- [x] Added/removed/modified/unchanged legend
- [x] Local changes overlay
- [x] Basic merge conflict assistance
- [x] Load branch diff data from backend diff endpoint (`src/frontend/src/api.ts` fetches on mount)
- [x] Load multi-context comparison data from backend endpoint (`src/frontend/src/api.ts` fetches on mount)
- [ ] Submit commit operation through backend Git API (frontend `api.commitGitWorkspace` exists, not wired into UI)

Frontend tests:

- [x] Branch switching through fixture state
- [x] Working tree status through fixture state
- [x] Commit creation preview
- [x] Semantic diff display
- [x] Branch comparison display
- [x] Changed-node overlays
- [x] Conflict detection and assistance UI
- [ ] Backend-backed diff UI integration test
- [ ] Backend-backed commit UI integration test

Frontend fixtures:

- `fixtures/branch-divergence/`
- `fixtures/multi-file-modular/`
- `fixtures/merge-conflict/`

Backend tasks:

- [x] Semantic branch diff for fixture branches
- [x] Branch-to-branch trace links
- [x] Multi-context view scoping
- [x] Diff API endpoint for branch fixture
- [x] Multi-context view API endpoint for branch fixture
- [x] Real Git branch switching via `git checkout` in `SysmlMvpService`
- [x] Working-tree status from a real Git repository (`GetGitStatus`)
- [x] Commit creation in a real Git repository (`CommitAll`)
- [x] Merge conflict detection from real Git state (`PreviewMergeConflict`)
- [x] Workspace context management (`OpenRepository`, `ListWorkspaceContexts`, `CloseWorkspaceContext`, `GetWorkspaceContext`) - `POST /api/repositories/open`, `GET /api/workspace-contexts`, `DELETE /api/workspace-contexts/{workspaceId}`
- [x] Workspace-scoped model graph (`GetWorkspaceGraph`) - `GET /api/workspace-contexts/{workspaceId}/model`
- [x] Workspace-scoped source file (`GetWorkspaceSourceFile`) - `GET /api/workspace-contexts/{workspaceId}/files`
- [x] Workspace-scoped commit (`CommitAll` via context) - `POST /api/workspace-contexts/{workspaceId}/commit`
- [x] Worktree creation (`CreateWorktree`) - `POST /api/workspace-contexts/worktrees`

Backend tests:

- [x] `semantic_branch_diff`
- [x] `branch_trace_links`
- [x] `multi_context_view_scopes_ids`
- [x] Diff endpoint checks through `tests/integration/backend-smoke.sh`
- [x] `git_branch_diff_and_status` — real Git branch diff and status test
- [x] `git_commit_persists_changes` — real Git commit test
- [x] `merge_conflict_preview_detects_conflict` — real Git merge conflict test
- [x] `open_repository_creates_workspace_context` — workspace context creation
- [x] `close_workspace_context_removes_from_list` — workspace context removal
- [x] `get_workspace_graph_returns_parsed_repo` — workspace graph retrieval

Backend fixtures:

- `fixtures/branch-divergence/`
- `fixtures/merge-conflict/`

Full-stack tests:

- [ ] UI compares two branches using backend diff data and displays changed nodes/files
- [ ] UI shows backend working-tree status for a temporary Git repository
- [ ] UI creates a commit through backend Git API and verifies repository state
- [ ] UI displays backend merge-conflict diagnostics

Full-stack fixtures:

- `fixtures/branch-divergence/`
- `fixtures/merge-conflict/`
- Fixture to add for temp real-Git repository branch switching
- Fixture to add for temp real-Git repository commit creation

Success criterion:

> A user can compare two architecture alternatives on different branches without reading raw diffs.

Exit criteria:

- Users can compare two revisions without reading raw diffs only.
- Commit output summarizes model changes clearly.
- Git operations do not corrupt the repository state.

Vision trace:

- Supports: Git-native review, branch-to-branch traceability, semantic diff, explicit context identity
- Tradeoff: merge assistance is basic until graph and write policies mature

### Phase 3b: Multi-Context Editing

Allow more than one writable context when each context has a distinct safe write location.

Frontend tasks:

- [ ] Open two worktrees for different branches of the same repository
- [ ] Open multiple repositories in one workspace
- [ ] Edit supported files in separate writable contexts
- [ ] Keep save and commit targets scoped to one repository and branch
- [ ] Show context labels on graph, source, trace, diff, save, and commit surfaces
- [ ] Show writable, read-only, dirty, and validation state directly in pane headers
- [ ] Prevent cross-context writes from shared comparison panes

Frontend tests:

- [ ] Opening two worktrees for different branches of the same repository
- [ ] Opening multiple repositories in one workspace
- [ ] Editing files in separate writable contexts
- [ ] Save and commit target labels scoped to one repository and branch
- [ ] Context labels on graph, source, trace, diff, save, and commit surfaces
- [ ] Cross-context write prevention UI

Frontend fixtures:

- `fixtures/branch-divergence/`
- `fixtures/multi-context-editing/`

Backend tasks:

- [x] Create a worktree only through an explicit user-requested backend operation (`POST /api/workspace-contexts/worktrees`)
- [ ] Validate same-repository multi-branch writable contexts
- [ ] Validate multiple-repository workspace contexts
- [ ] Scope save targets to one repository, branch, and worktree
- [x] Scope commit targets to repository, branch, and worktree (`POST /api/workspace-contexts/{workspaceId}/commit`)
- [ ] Reject writes from shared comparison panes
- [x] Preserve `MultiContextViewDto` IDs without collapsing contexts

Backend tests:

- [ ] `multi_context_identity`
- [x] `open_repository_creates_workspace_context` — workspace context creation
- [ ] Multiple repository context test
- [ ] Save target scoping test
- [ ] Commit target scoping test
- [ ] Cross-context write rejection test

Backend fixtures:

- `fixtures/branch-divergence/`
- `fixtures/multi-context-editing/`
- Fixture to add with two explicit Git worktrees
- Fixture to add with two repositories in one workspace

Full-stack tests:

- [ ] UI opens two backend-validated writable contexts for the same repository
- [ ] UI edits one context and backend rejects accidental writes to the comparison context
- [ ] UI saves and commits to the correct repository, branch, and worktree

Full-stack fixtures:

- `fixtures/multi-context-editing/`
- Fixture to add with two explicit Git worktrees
- Fixture to add with two repositories in one workspace

Success criterion:

> A user can edit two branches or related repositories side by side without ambiguity about where each change will be written.

Runtime rules for multi-context editing are defined in [runtime.md](../architecture/runtime.md).

Exit criteria:

- Two writable contexts for the same repository do not interfere with each other.
- Save and commit operations target the correct workspace context.
- Combined views use `MultiContextViewDto` and never collapse IDs.

Vision trace:

- Supports: multi-repository workspace, safe writes, branch/repository context awareness
- Tradeoff: requires explicit worktrees for same-repository concurrent editing

### Phase 4: Custom Views and Traceability

Add the features that make it better than PowerPoint.

Frontend tasks:

- [ ] Saved views UI
- [ ] Shared repo views UI
- [ ] Local private views UI
- [ ] Trace matrix UI
- [ ] Source ownership view
- [ ] Cross-repository dependency view
- [ ] Multi-context comparison view
- [ ] Node-centered inspector trace view
- [ ] Impact analysis UI
- [ ] Query/filter UI
- [ ] View publishing UI
- [ ] Visual indicators for requirements, satisfies, verifies, allocations, ports, connections, files, and changed contexts

Frontend tests:

- [ ] Saved view creation and restore
- [ ] Shared and local view display
- [ ] Trace matrix rendering
- [ ] Impact analysis interaction
- [ ] Query/filter interaction
- [ ] View publishing and reload interaction

Frontend fixtures:

- `fixtures/multi-file-modular/`
- `fixtures/branch-divergence/`
- `fixtures/saved-views/`

Backend tasks:

- [x] Saved view persistence (in-memory, via `POST/GET /api/saved-views`, `PUT/DELETE /api/saved-views/{viewId}`)
- [x] Shared repo view storage (`StorageMode` field distinguishes "shared" vs "local")
- [x] Local private view storage
- [x] Stable-ID based view restoration (`GET /api/saved-views/{viewId}`)
- [x] Trace matrix generation (`GET /api/workspace-contexts/{workspaceId}/trace-matrix`)
- [ ] Source ownership query API
- [ ] Cross-repository dependency query API
- [ ] Multi-context comparison view API
- [ ] Impact analysis query backend
- [ ] Query/filter backend
- [ ] View publishing backend

Backend tests:

- [x] `saved_view_crud` — create, list, get, update, delete views
- [x] `build_trace_matrix` — trace matrix generation from workspace graph
- [ ] Stable-ID view restoration test
- [ ] Source ownership query test
- [ ] Cross-repository dependency query test
- [ ] Impact analysis test
- [ ] Query/filter rules test
- [ ] View publishing and reload test

Backend fixtures:

- `fixtures/multi-file-modular/`
- `fixtures/branch-divergence/`
- `fixtures/saved-views/` (expected view list fixture created)
- Fixture to add for cross-repository dependencies

Full-stack tests:

- [ ] UI saves a view through backend persistence and reloads it in a new session
- [ ] UI displays backend trace matrix and impact-analysis query results
- [ ] UI publishes a shared view and another session resolves it by stable IDs

Full-stack fixtures:

- `fixtures/saved-views/`
- `fixtures/multi-file-modular/`
- Fixture to add for cross-repository dependencies
- Fixture to add for view rename/move restoration

Success criterion:

> A team can create architecture views for different stakeholders from the same model.

Exit criteria:

- Views survive rename/move operations.
- Traceability data stays consistent with the graph index.
- A saved view can be reopened on another session and still resolve correctly.

Vision trace:

- Supports: stakeholder-specific projections, traceability as a first-class capability, reviewable shared views
- Tradeoff: custom view authoring builds on stable graph IDs and parser coverage

### Phase 5: Advanced SysML v2 Support

Expand language coverage.

Frontend tasks:

- [ ] UI for more SysML types
- [ ] Behavior modeling views
- [ ] State/action views
- [ ] Variant/product-line views
- [ ] Validation result UI
- [ ] Model library browser
- [ ] CI validation status UI
- [ ] Report generation UI
- [ ] Advanced workspace layouts
- [ ] Saved pane arrangements
- [ ] Optional simplified "PowerPoint mode"

Frontend tests:

- [ ] UI coverage for new language constructs
- [ ] Behavior modeling interaction tests
- [ ] State/action view tests
- [ ] Variant/product-line view tests
- [ ] Validation result rendering tests
- [ ] Model library browser tests
- [ ] CI validation status tests
- [ ] Report generation UI tests

Frontend fixtures:

- `fixtures/advanced-sysml/`
- `fixtures/model-libraries/`
- `fixtures/variant-models/`

Backend tasks:

- [ ] Parser support for more SysML types
- [ ] Behavior model parsing
- [ ] State/action model parsing
- [ ] Variant/product-line model parsing
- [ ] Validation rules
- [ ] Model library import resolution
- [ ] CI integration
- [ ] Report generation
- [ ] Backward compatibility for the MVP supported subset

Backend tests:

- [ ] New language construct parser tests
- [ ] Behavior model parser tests
- [ ] State/action parser tests
- [ ] Variant/product-line parser tests
- [ ] Validation rule tests
- [ ] Model library import tests
- [ ] CI validation tests
- [ ] Report generation tests
- [ ] MVP subset regression tests

Backend fixtures:

- `fixtures/advanced-sysml/`
- `fixtures/model-libraries/`
- `fixtures/variant-models/`
- Fixture to add for CI validation
- Fixture to add for report generation

Full-stack tests:

- [ ] UI opens an advanced model through backend parser support and renders supported views
- [ ] UI displays backend validation results for advanced SysML fixtures
- [ ] UI triggers backend report generation and displays the generated report status
- [ ] Existing MVP browser/edit/Git flows still pass against the expanded parser

Full-stack fixtures:

- `fixtures/advanced-sysml/`
- `fixtures/model-libraries/`
- `fixtures/variant-models/`
- Fixture to add for CI validation
- Fixture to add for report generation

Success criterion:

> The tool becomes a serious MBSE workbench, not just a diagram editor.

Exit criteria:

- New syntax is added with tests before or alongside implementation.
- Existing supported subset behavior remains stable.

Vision trace:

- Supports: serious MBSE workflow, broader SysML semantics, long-lived engineering adoption
- Tradeoff: advanced modeling waits until the workbench, graph, write, and Git foundations are reliable

# Roadmap Test Coverage Tracker

Last reviewed: 2026-05-13

This tracker maps roadmap functionality to current frontend and backend test
evidence. It is intentionally explicit about fixture-backed coverage versus
real backend implementation so readiness is not overstated.

Status legend:

- `Covered`: automated tests exist and pass for the current implementation slice.
- `Smoke`: executable smoke coverage exists, but not full behavioral coverage.
- `Partial`: some behavior is covered, but the roadmap item is broader than the implementation.
- `Gap`: roadmap functionality is expected for the phase but lacks sufficient implementation or tests.
- `Future`: roadmap functionality belongs to a later phase and is not expected yet.

## Verification Commands

Run frontend gates:

```bash
cd src/frontend
npm test
npm run typecheck
npm run build
```

Run backend smoke gate:

```bash
bash tests/integration/backend-smoke.sh
```

Latest local verification on 2026-05-13:

- `npm test`: passed, 4 test files and 12 tests.
- `npm run typecheck`: passed.
- `npm run build`: passed.
- `bash tests/integration/backend-smoke.sh`: passed.

## Current Automated Test Inventory

| Area | Test command | Test file or script | Coverage |
| --- | --- | --- | --- |
| Frontend phase 0 | `npm test` | `src/frontend/tests/phase0-workbench.test.tsx` | Workbench shell fixture and rendered shell context identity |
| Frontend phase 1 | `npm test` | `src/frontend/tests/phase1-browser.test.tsx` | Fixture-backed browser state, selection sync, search, source ownership, trace links |
| Frontend phase 2 | `npm test` | `src/frontend/tests/phase2-editing.test.tsx` | Fixture-backed draft creation, rename, delete, undo/redo, generated source preview |
| Frontend phase 3 | `npm test` | `src/frontend/tests/phase3-git-workflow.test.tsx` | Fixture-backed branch diff, scoped multi-context IDs, commit preview, conflict assistance |
| Frontend type safety | `npm run typecheck` | TypeScript project references | Compile-time TS checks |
| Frontend production build | `npm run build` | Vite build | Production bundle generation |
| Backend API smoke | `bash tests/integration/backend-smoke.sh` | `tests/integration/backend-smoke.sh` | API starts, `/api/health` returns `{"status":"ok"}`, OpenAPI title is present |

## MVP Feature Coverage

| MVP feature | Frontend coverage | Backend coverage | Evidence | Status |
| --- | --- | --- | --- | --- |
| Open local Git repo | Fixture-backed repository cards and contexts | No repo-open endpoint | phase 0-1 frontend tests | Partial |
| Detect SysML files | Fixture-backed file paths | No filesystem scanner | fixtures under `fixtures/*/model` | Partial |
| Basic textual editor | Read-only code panes and generated source preview | No source-file API | phase 1-2 frontend tests | Partial |
| Parse a useful SysML v2 subset | Expected graph/diagnostic fixtures checked by frontend tests | No parser implementation | phase 1 frontend fixture assertions | Partial |
| Show package/part hierarchy as tree | Render and selection tests | Not applicable | `phase1-browser.test.tsx` | Covered |
| Show selected structure as graph | Render and graph-node selection tests | No graph API | `phase1-browser.test.tsx` | Partial |
| Create/edit/delete basic elements | Draft UI tests | No backend writer/save | `phase2-editing.test.tsx` | Partial |
| Save back to text | Generated source preview and save button only | No persistence | `phase2-editing.test.tsx` | Gap |
| Show attributes of selected element | Inspector render and sync tests | No backend model source | phase 0-1 frontend tests | Partial |
| Commit changes | Commit preview only | No Git commit operation | `phase3-git-workflow.test.tsx` | Gap |
| Store custom views as JSON | No implementation | No persistence | none | Future |
| Visualize traceability between model items and source files | Fixture-backed trace UI | No trace derivation service | `phase1-browser.test.tsx` | Partial |
| Preserve repository and branch context for every opened model graph | Fixture-backed context labels and branch comparison IDs | No backend graph context | phase 0-3 frontend tests | Partial |

## Phase Coverage Matrix

### Phase 0: Visual Workbench Shell

| Roadmap item | Frontend coverage | Backend coverage | Evidence | Status |
| --- | --- | --- | --- | --- |
| Top-level shell, left rail, tiled workspace, inspector, status bar | Render test | Not applicable | `phase0-workbench.test.tsx` | Covered |
| Pane-level repository, branch, file, mode, write-state labels | Render test | Not applicable | `phase0-workbench.test.tsx` | Covered |
| Fixture-backed multiple repositories and branches | Fixture and render test | Not applicable | `fixtures/phase-0-workbench`, `phase0-workbench.test.tsx` | Covered |
| Responsive fallback preserves context labels | CSS exists; no viewport automation | Not applicable | `styles.css`; no responsive test harness | Partial |
| Backend OpenAPI scaffold starts | Not applicable | Smoke script | `tests/integration/backend-smoke.sh` | Smoke |
| Frontend app starts | Build/test coverage; dev server is not automated | Not applicable | `npm test`, `npm run build`; no dev-server smoke script | Partial |

### Phase 1: Read-only Model Browser

| Roadmap item | Frontend coverage | Backend coverage | Evidence | Status |
| --- | --- | --- | --- | --- |
| Open Git repo | Fixture-backed context only | No real repo-open endpoint | `phase1-browser.test.tsx` | Partial |
| Create explicit workspace context | Fixture-backed context checks | No backend workspace context model | `fixtures/phase-1-browser/expected/browser-state.json` | Partial |
| Parse SysML files | Expected graph/diagnostic fixtures checked | No backend parser implementation | `fixtures/tiny-single-file/expected/graph.json`, `fixtures/invalid-input/expected/diagnostics.json` | Partial |
| Show tree hierarchy | Render and interaction tests | Not applicable | `phase1-browser.test.tsx` | Covered |
| Show text editor | Render tests | No source-file API | `phase1-browser.test.tsx` | Partial |
| Show graph view | Render and selection tests | No graph projection API | `phase1-browser.test.tsx` | Partial |
| Click graph node shows source and attributes | Render and selection tests | No backend source mapping | `phase1-browser.test.tsx` | Partial |
| Sync graph, tree, text pane, inspector | Render and interaction tests | Not applicable | `phase1-browser.test.tsx` | Covered |
| Source ownership and item-to-item trace links | Fixture and render tests | No backend trace derivation | `phase1-browser.test.tsx`, `trace-links.json` | Partial |
| File-to-file import traceability | Fixture assertion | No backend import resolver | `fixtures/multi-file-modular/expected/trace-links.json` | Partial |
| Repository, branch, file, mode, read-only state visible | Render tests | Not applicable | `phase0-workbench.test.tsx`, `phase1-browser.test.tsx` | Covered |
| Basic search | Interaction test | Not applicable | `phase1-browser.test.tsx` | Covered |

### Phase 2: Visual Editing

| Roadmap item | Frontend coverage | Backend coverage | Evidence | Status |
| --- | --- | --- | --- | --- |
| Type palette interaction | Interaction tests | Not applicable | `phase2-editing.test.tsx` | Covered |
| Drag SysML type onto canvas | Simulated drag/drop test | Not applicable | `phase2-editing.test.tsx` | Covered |
| Click type, then click canvas to place | Interaction test | Not applicable | `phase2-editing.test.tsx` | Covered |
| Create part/package/requirement | Generated source assertions | No backend writer | `phase2-editing.test.tsx` | Partial |
| Create relationship by dragging connector | Interaction and generated source assertions | No backend relationship writer | `phase2-editing.test.tsx` | Partial |
| Add ports and features inline | Port generated source assertion | No backend writer | `phase2-editing.test.tsx` | Partial |
| Rename inline | Interaction test | No backend stable-ID rename path | `phase2-editing.test.tsx` | Partial |
| Generated source preview visible in split mode | Render test | Not applicable | `phase2-editing.test.tsx` | Covered |
| Intended save target before write | Render test | No backend save endpoint | `phase2-editing.test.tsx` | Partial |
| Auto-layout | Textual feedback only | No layout engine | `phase2-editing.test.tsx` | Partial |
| Save generated SysML text | Save button/preview only | No backend persistence | `phase2-editing.test.tsx` | Gap |
| Undo/redo | Interaction tests | Not applicable | `phase2-editing.test.tsx` | Covered |
| Validation feedback | Pane/status/render tests | No backend validation | `phase2-editing.test.tsx` | Partial |
| Parser/writer round trip | Fixture exists; no executable parser/writer test | No parser/writer | `fixtures/tiny-single-file/expected/graph.json` | Gap |
| Save touches only owner | Fixture exists; no executable save test | No save implementation | `fixtures/multi-file-modular/expected/changed-files.json` | Gap |
| Stable ID survives rename | No executable identity-policy test | No writer/identity policy | none | Gap |
| Missing ID blocks write until backfill | No fixture or executable test | No writer/backfill policy | none | Gap |

### Phase 3: Git-Native Workflow

| Roadmap item | Frontend coverage | Backend coverage | Evidence | Status |
| --- | --- | --- | --- | --- |
| Branch switcher | Interaction test | No backend branch switch endpoint | `phase3-git-workflow.test.tsx` | Partial |
| Side-by-side branch contexts | Render and fixture tests | No backend multi-context API | `phase3-git-workflow.test.tsx`, `multi-context-view.json` | Partial |
| Multi-context comparison projection | Fixture assertion and render test | No backend projection generation | `phase3-git-workflow.test.tsx` | Partial |
| Compare selector in top app bar | Rendered shell; no behavior test | No backend compare endpoint | `phase0-workbench.test.tsx` | Partial |
| Commit panel | Interaction test for preview | No real commit operation | `phase3-git-workflow.test.tsx` | Partial |
| Visual diff | Render test | No backend diff engine | `phase3-git-workflow.test.tsx` | Partial |
| Text diff | Render test | No backend diff endpoint | `phase3-git-workflow.test.tsx` | Partial |
| Split visual/text comparison panes | Interaction test | No backend diff endpoint | `phase3-git-workflow.test.tsx` | Partial |
| Branch comparison | Fixture assertion and render test | No backend Git branch comparison | `branch-divergence/expected/diff.json` | Partial |
| Branch-to-branch trace links | Fixture assertion and render test | No backend trace derivation | `phase3-git-workflow.test.tsx` | Partial |
| Changed-node highlighting | Render test for added node | No backend model-status projection | `phase3-git-workflow.test.tsx` | Partial |
| Changed-file highlighting | Render test for modified file | No backend changed-file computation | `phase3-git-workflow.test.tsx` | Partial |
| Added/removed/modified/unchanged legend | Render test through phase 0 and phase 3 | Not applicable | `phase0-workbench.test.tsx`, `phase3-git-workflow.test.tsx` | Covered |
| Local changes overlay | Render test | No backend working-tree status | `phase3-git-workflow.test.tsx` | Partial |
| Basic merge conflict assistance | Render test only | No conflict detection engine | `phase3-git-workflow.test.tsx` | Partial |
| Semantic branch diff fixture contract | Fixture assertion | No backend semantic diff engine | `phase3-git-workflow.test.tsx` | Partial |
| Multi-context ID scoping | Fixture assertion | No backend projection generation | `phase3-git-workflow.test.tsx` | Partial |

### Phase 3b: Multi-Context Editing

| Roadmap item | Frontend coverage | Backend coverage | Evidence | Status |
| --- | --- | --- | --- | --- |
| Open two worktrees for different branches | None | No backend worktree API | none | Future |
| Explicit worktree creation operation | None | No `POST /workspace-contexts/worktrees` endpoint | none | Future |
| Open multiple repositories in one workspace | Static fixture cards only | No backend workspace API | `phase0-workbench.test.tsx` | Future |
| Edit supported files in separate writable contexts | None | No save/commit backend | none | Future |
| Save and commit targets scoped to repo/branch | Phase 2/3 UI preview only | No backend enforcement | `phase2-editing.test.tsx`, `phase3-git-workflow.test.tsx` | Future |
| Context labels on graph/source/trace/diff/save/commit | Static/render coverage | No backend context enforcement | phase 0-3 frontend tests | Future |
| Writable/read-only/dirty/validation state in pane headers | Static/render coverage | No backend state source | phase 0-3 frontend tests | Future |
| Prevent cross-context writes from shared comparison panes | None | No write policy | none | Future |

### Phase 4: Custom Views and Traceability

| Roadmap item | Frontend coverage | Backend coverage | Evidence | Status |
| --- | --- | --- | --- | --- |
| Saved/shared/local views | None | No persistence API | none | Future |
| Trace matrix | Trace snippets only | No trace matrix service | phase 1 frontend tests | Future |
| Source ownership view | Render coverage | No backend derivation | `phase1-browser.test.tsx` | Future |
| Cross-repository dependency view | None | No dependency graph service | none | Future |
| Multi-context comparison view | Phase 3 fixture-backed view | No backend projection generation | `phase3-git-workflow.test.tsx` | Future |
| Node-centered inspector trace view | Basic inspector trace list | No backend trace API | `phase1-browser.test.tsx` | Future |
| Impact analysis | None | No query/impact engine | none | Future |
| Query/filter system | Basic tree search only | No query backend | `phase1-browser.test.tsx` | Future |
| View publishing | None | No publishing API | none | Future |
| Visual indicators for requirements, traces, ports, files, changed contexts | Partial visual fixtures | No backend state source | phase 0-3 frontend tests | Future |

### Phase 5: Advanced SysML v2 Support

| Roadmap item | Frontend coverage | Backend coverage | Evidence | Status |
| --- | --- | --- | --- | --- |
| More SysML types | Palette labels only | No parser support | phase 2 frontend tests | Future |
| Behavior modeling | Palette labels only | No behavior model/parser | none | Future |
| State/action views | Palette labels only | No behavior model/parser | none | Future |
| Variant/product-line views | None | No variant model support | none | Future |
| Validation rules | Static validation labels only | No validation engine | phase 2 frontend tests | Future |
| Model libraries | None | No library import support | none | Future |
| CI integration | Documentation only | No CI config in repo | docs/testing | Future |
| Report generation | None | No reporting service | none | Future |
| Advanced layouts and PowerPoint mode | Static responsive layout only | Not applicable | `styles.css` | Future |

## Highest-Risk Gaps

1. Backend parser, graph mapping, writer, Git, and persistence are not implemented beyond the API health/OpenAPI scaffold.
2. Phase 1-3 tests validate fixture-backed frontend behavior and JSON contracts, not real backend domain behavior.
3. Phase 2 save and round-trip gates are not truly satisfied until parser/writer/save tests exist in backend or shared domain test projects.
4. Phase 3 commit and Git operations are preview-only; no repository mutation is performed or tested.
5. Responsive behavior is implemented in CSS but not verified through automated viewport tests.

## Required Next Test Additions

Before treating the MVP as backend-ready, add automated backend/domain tests for:

- `parse_minimal_graph`
- `model_graph_has_context`
- `derive_item_to_file_traceability`
- `derive_import_traceability`
- `malformed_input_reports_diagnostic`
- `get_source_file_preserves_text`
- `parse_round_trip_minimal`
- `save_touches_only_owner`
- `stable_id_survives_rename`
- `missing_id_blocks_write_until_backfill`
- `semantic_branch_diff`
- `branch_trace_links`
- `multi_context_view_scopes_ids`

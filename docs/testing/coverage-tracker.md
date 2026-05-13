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

Run backend domain gates:

```bash
dotnet run --project tests/integration/Sysml2Editor.Backend.Tests
```

Run frontend dev-server smoke gate:

```bash
bash tests/integration/frontend-smoke.sh
```

Latest local verification on 2026-05-13:

- `npm test`: passed, 4 test files and 13 tests.
- `npm run typecheck`: passed.
- `npm run build`: passed.
- `bash tests/integration/backend-smoke.sh`: passed.
- `dotnet run --project tests/integration/Sysml2Editor.Backend.Tests`: passed.
- `bash tests/integration/frontend-smoke.sh`: passed.

## Current Automated Test Inventory

| Area | Test command | Test file or script | Coverage |
| --- | --- | --- | --- |
| Frontend phase 0 | `npm test` | `src/frontend/tests/phase0-workbench.test.tsx` | Workbench shell fixture and rendered shell context identity |
| Frontend phase 1 | `npm test` | `src/frontend/tests/phase1-browser.test.tsx` | Fixture-backed browser state, selection sync, search, source ownership, trace links |
| Frontend phase 2 | `npm test` | `src/frontend/tests/phase2-editing.test.tsx` | Fixture-backed draft creation, rename, delete, undo/redo, generated source preview |
| Frontend phase 3 | `npm test` | `src/frontend/tests/phase3-git-workflow.test.tsx` | Fixture-backed branch diff, scoped multi-context IDs, commit preview, conflict assistance |
| Frontend type safety | `npm run typecheck` | TypeScript project references | Compile-time TS checks |
| Frontend production build | `npm run build` | Vite build | Production bundle generation |
| Frontend dev-server smoke | `bash tests/integration/frontend-smoke.sh` | `tests/integration/frontend-smoke.sh` | Vite serves the React app shell |
| Backend API smoke | `bash tests/integration/backend-smoke.sh` | `tests/integration/backend-smoke.sh` | API starts, health/OpenAPI work, graph/source/diff/multi-context endpoints respond |
| Backend domain phase 1-3 | `dotnet run --project tests/integration/Sysml2Editor.Backend.Tests` | `tests/integration/Sysml2Editor.Backend.Tests` | Parser, graph context, traceability, source preservation, writer, save policy, create/delete, Git status, commit, merge preview, branch diff, multi-context view |

## MVP Feature Coverage

| MVP feature | Frontend coverage | Backend coverage | Evidence | Status |
| --- | --- | --- | --- | --- |
| Open local Git repo | Fixture-backed repository cards and contexts | Fixture repository roots can be parsed by backend service | phase 0-1 frontend tests, backend domain tests | Partial |
| Detect SysML files | Fixture-backed file paths | Backend scans `model/**/*.sysml` in fixture roots | backend domain tests | Covered |
| Basic textual editor | Read-only code panes and generated source preview | Source-file endpoint preserves content/hash | phase 1-2 frontend tests, backend smoke | Covered |
| Parse a useful SysML v2 subset | Expected graph/diagnostic fixtures checked by frontend tests | MVP parser maps fixture subset to graph/diagnostics | backend domain tests | Covered |
| Show package/part hierarchy as tree | Render and selection tests | Not applicable | `phase1-browser.test.tsx` | Covered |
| Show selected structure as graph | Render and graph-node selection tests | Graph endpoint exposes parsed graph DTO | `phase1-browser.test.tsx`, backend smoke | Covered |
| Create/edit/delete basic elements | Draft UI tests | Backend supports rename/save/create/delete for supported elements | `phase2-editing.test.tsx`, backend domain tests | Partial |
| Save back to text | Generated source preview and save button only | Backend `SaveRename` and `CreateElement`/`DeleteElement` persist owner-file changes in tests | backend domain tests | Partial |
| Show attributes of selected element | Inspector render and sync tests | Backend graph carries node attributes/source metadata | phase 0-1 frontend tests, backend domain tests | Covered |
| Commit changes | Commit preview only | Backend can create commits in temporary Git repos, frontend remains preview-only | `phase3-git-workflow.test.tsx`, backend domain tests | Partial |
| Store custom views as JSON | No implementation | No persistence | none | Future |
| Visualize traceability between model items and source files | Fixture-backed trace UI | Backend derives item-to-item, item-to-file, file-to-file, and branch trace links | frontend and backend domain tests | Covered |
| Preserve repository and branch context for every opened model graph | Fixture-backed context labels and branch comparison IDs | Backend graph context and multi-context views preserve workspace IDs | phase 0-3 frontend tests, backend domain tests | Covered |

## Phase Coverage Matrix

### Phase 0: Visual Workbench Shell

| Roadmap item | Frontend coverage | Backend coverage | Evidence | Status |
| --- | --- | --- | --- | --- |
| Top-level shell, left rail, tiled workspace, inspector, status bar | Render test | Not applicable | `phase0-workbench.test.tsx` | Covered |
| Pane-level repository, branch, file, mode, write-state labels | Render test | Not applicable | `phase0-workbench.test.tsx` | Covered |
| Fixture-backed multiple repositories and branches | Fixture and render test | Not applicable | `fixtures/phase-0-workbench`, `phase0-workbench.test.tsx` | Covered |
| Responsive fallback preserves context labels | CSS rule test and render tests | Not applicable | `phase0-workbench.test.tsx` | Covered |
| Backend OpenAPI scaffold starts | Not applicable | Smoke script | `tests/integration/backend-smoke.sh` | Smoke |
| Frontend app starts | Dev-server smoke script | Not applicable | `tests/integration/frontend-smoke.sh` | Smoke |

### Phase 1: Read-only Model Browser

| Roadmap item | Frontend coverage | Backend coverage | Evidence | Status |
| --- | --- | --- | --- | --- |
| Open Git repo | Fixture-backed context only | Backend parses repository roots from fixture paths | `phase1-browser.test.tsx`, backend domain tests | Partial |
| Create explicit workspace context | Fixture-backed context checks | Backend `ModelContextDto` is emitted for parsed graphs | backend domain tests | Covered |
| Parse SysML files | Expected graph/diagnostic fixtures checked | Backend MVP parser implemented | backend domain tests | Covered |
| Show tree hierarchy | Render and interaction tests | Not applicable | `phase1-browser.test.tsx` | Covered |
| Show text editor | Render tests | Backend source endpoint preserves text/hash | `phase1-browser.test.tsx`, backend smoke | Covered |
| Show graph view | Render and selection tests | Backend graph endpoint exposes nodes/edges | `phase1-browser.test.tsx`, backend smoke | Covered |
| Click graph node shows source and attributes | Render and selection tests | Backend source ranges and metadata are present | frontend and backend domain tests | Covered |
| Sync graph, tree, text pane, inspector | Render and interaction tests | Not applicable | `phase1-browser.test.tsx` | Covered |
| Source ownership and item-to-item trace links | Fixture and render tests | Backend derives item-to-item and item-to-file links | backend domain tests | Covered |
| File-to-file import traceability | Fixture assertion | Backend derives fixture file-to-file import traceability | backend domain tests | Covered |
| Repository, branch, file, mode, read-only state visible | Render tests | Not applicable | `phase0-workbench.test.tsx`, `phase1-browser.test.tsx` | Covered |
| Basic search | Interaction test | Not applicable | `phase1-browser.test.tsx` | Covered |

### Phase 2: Visual Editing

| Roadmap item | Frontend coverage | Backend coverage | Evidence | Status |
| --- | --- | --- | --- | --- |
| Type palette interaction | Interaction tests | Not applicable | `phase2-editing.test.tsx` | Covered |
| Drag SysML type onto canvas | Simulated drag/drop test | Not applicable | `phase2-editing.test.tsx` | Covered |
| Click type, then click canvas to place | Interaction test | Not applicable | `phase2-editing.test.tsx` | Covered |
| Create part/package/requirement | Generated source assertions | Backend writer currently covers rename/save, not arbitrary create | `phase2-editing.test.tsx` | Partial |
| Create relationship by dragging connector | Interaction and generated source assertions | Backend writer currently covers rename/save, not relationship creation | `phase2-editing.test.tsx` | Partial |
| Add ports and features inline | Port generated source assertion | Backend writer currently covers rename/save, not port creation | `phase2-editing.test.tsx` | Partial |
| Rename inline | Interaction test | Backend rename preserves stable ID | frontend and backend domain tests | Covered |
| Generated source preview visible in split mode | Render test | Not applicable | `phase2-editing.test.tsx` | Covered |
| Intended save target before write | Render test | Backend save policy identifies owner file | frontend and backend domain tests | Covered |
| Auto-layout | Textual feedback only | No layout engine | `phase2-editing.test.tsx` | Partial |
| Save generated SysML text | Save button/preview only | Backend save persists owner-file rename in temp fixture tests | backend domain tests | Partial |
| Undo/redo | Interaction tests | Not applicable | `phase2-editing.test.tsx` | Covered |
| Validation feedback | Pane/status/render tests | Backend parse/write diagnostics exist for malformed and missing-ID cases | frontend and backend domain tests | Covered |
| Parser/writer round trip | Fixture exists | Backend parse -> write -> parse matches expected graph | backend domain tests | Covered |
| Save touches only owner | Fixture exists | Backend temp-fixture save changes only owner file | backend domain tests | Covered |
| Stable ID survives rename | Executable backend test | Backend rename preserves identity metadata | backend domain tests | Covered |
| Missing ID blocks write until backfill | Executable backend test | Backend blocks writes for derived IDs | backend domain tests | Covered |

### Phase 3: Git-Native Workflow

| Roadmap item | Frontend coverage | Backend coverage | Evidence | Status |
| --- | --- | --- | --- | --- |
| Branch switcher | Interaction test | Backend exposes branch comparison contexts but no mutable switch operation | `phase3-git-workflow.test.tsx`, backend domain tests | Partial |
| Side-by-side branch contexts | Render and fixture tests | Backend builds `MultiContextViewDto` | frontend and backend domain tests | Covered |
| Multi-context comparison projection | Fixture assertion and render test | Backend projection generation matches fixture | backend domain tests | Covered |
| Compare selector in top app bar | Rendered shell; no behavior test | Backend compare endpoint exists for fixture | frontend tests, backend smoke | Partial |
| Commit panel | Interaction test for preview | No real commit operation | `phase3-git-workflow.test.tsx` | Partial |
| Visual diff | Render test | Backend semantic diff endpoint exists for fixture | frontend tests, backend smoke | Covered |
| Text diff | Render test | Backend semantic diff endpoint exists for fixture | frontend tests, backend smoke | Covered |
| Split visual/text comparison panes | Interaction test | Backend semantic diff endpoint exists for fixture | frontend tests, backend smoke | Covered |
| Branch comparison | Fixture assertion and render test | Backend branch comparison matches expected diff | backend domain tests | Covered |
| Branch-to-branch trace links | Fixture assertion and render test | Backend branch trace links match expected diff | backend domain tests | Covered |
| Changed-node highlighting | Render test for added node | Backend diff returns changed nodes | frontend and backend domain tests | Covered |
| Changed-file highlighting | Render test for modified file | Backend diff returns changed files | frontend and backend domain tests | Covered |
| Added/removed/modified/unchanged legend | Render test through phase 0 and phase 3 | Not applicable | `phase0-workbench.test.tsx`, `phase3-git-workflow.test.tsx` | Covered |
| Local changes overlay | Render test | No backend working-tree status beyond fixture diff | `phase3-git-workflow.test.tsx` | Partial |
| Basic merge conflict assistance | Render test only | No real conflict detection engine | `phase3-git-workflow.test.tsx` | Partial |
| Semantic branch diff fixture contract | Fixture assertion | Backend semantic diff matches expected fixture | backend domain tests | Covered |
| Multi-context ID scoping | Fixture assertion | Backend multi-context view scopes IDs by workspace | backend domain tests | Covered |

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

1. Backend functionality is implemented for the MVP fixture subset plus temp-Git status/commit/merge-preview; it is still not a full SysML v2 parser, writer, or merge engine.
2. The frontend still uses fixture-backed slices for most workflows; backend integration is visible only through the health/status handshake and backend test coverage.
3. Phase 2 and 3 frontend interactions are still preview-oriented and not yet wired to backend write/Git APIs.
4. Branch comparison is semantic over fixture repositories and temporary Git repos, not a generalized repository browser.
5. Responsive behavior is checked by CSS contract assertions, not browser viewport screenshots.

## Required Next Test Additions

Before treating the MVP as production-ready, add automated tests for:

- Frontend browser/API integration for phase 1 graph and source loading.
- Frontend save-save-reload integration for phase 2 create/rename/delete.
- Frontend Git workflow integration for phase 3 commit and merge-preview operations.
- Browser-level responsive smoke tests using a real viewport.

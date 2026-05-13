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

Gate rule:

- Earlier phase gates must continue passing as later phases are implemented.
- Any change to parsing, model mapping, save logic, or diff logic must update this roadmap if it changes the minimum safe test set.
- Phase 1 gates must pass before save or visual editing is exposed.
- Phase 2 gates must pass before visual editing is treated as implementation-ready.
- Phase 3 gates must pass before branch comparison or commit summaries are exposed.

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

Features:

- Top-level shell, left rail, tiled workspace, inspector, and status bar
- Pane-level context labels for repository, branch, file, mode, and write state
- Fixture-backed sample content that demonstrates multiple repositories and branches
- Responsive fallback that preserves context labels on narrower screens

Test focus:

- Shell rendering with top app bar, left rail, tiled workspace, inspector, and status bar
- Pane headers show repository, branch, file, mode, and write state
- Pane mode toggles are local to the targeted pane
- Selected fixture state can populate the inspector
- Responsive fallback preserves context identity
- Dirty, read-only, and validation indicators render before editing exists

Minimum gate tests:

| Test | Layer | Fixture | Purpose | Expected Result |
| --- | --- | --- | --- | --- |
| `repo_scaffold_present` | Smoke | none | Prove the implementation skeleton exists | Root docs, backend, frontend, shared contracts, tests, fixtures, and scripts match the repo layout in [README.md](../../README.md) |
| `backend_starts_with_openapi` | Smoke | none | Prove backend scaffold and contract generation are wired | `dotnet run --project src/backend/Sysml2Editor.Api` exposes OpenAPI in development |
| `frontend_starts` | Smoke | none | Prove frontend scaffold runs | `npm run dev` from `src/frontend` serves the app shell |

Fixtures:

- `fixtures/phase-0-workbench/`
- `fixtures/tiny-single-file/`
- `fixtures/branch-divergence/`
- `fixtures/multi-file-modular/`

Success criterion:

> A user can look at the UI and understand that the product is a multi-repository, multi-branch SysML workbench even before real backend data is connected.

Exit criteria:

- A user can identify the active repository, branch, file, selected model element, and write state from visible UI alone.
- Visual, text, split, and diff panes can be represented with fixture data.
- The shell is ready for parser, graph, inspector, and Git data integration.

Vision trace:

- Supports: multi-context workspace, Git-native workflow, traceability-first inspection, PowerPoint-like visual orientation
- Tradeoff: uses static or fixture-backed data before full parser and backend integration

### Phase 1: Read-only Model Browser

Build confidence in repo parsing and navigation.

Features:

- Open Git repo
- Create one explicit workspace context for the opened repo and current branch
- Parse SysML files
- Show tree hierarchy
- Show text editor
- Show graph view
- Render the graph inside the visual workbench pane system
- Click graph node -> show source text and attributes
- Sync selected graph node with model tree, text pane, and inspector
- Show source ownership and item-to-item trace links for selected nodes
- Show file-to-file import traceability for modular models
- Keep repository, branch, file, mode, and read-only state visible on every pane
- Basic search

Test focus:

- Repo open and file discovery
- Workspace context creation
- Parse success and parse error reporting
- Tree hierarchy rendering
- Text editor view sync
- Graph view node/edge projection
- Source ownership and item-to-item trace links
- File-to-file import traceability
- Search and selection behavior

Minimum gate tests:

| Test | Layer | Fixture | Purpose | Expected Result |
| --- | --- | --- | --- | --- |
| `parse_minimal_graph` | Unit/Integration | `fixtures/tiny-single-file` | Prove the parser maps the supported subset into the model graph | Matches `expected/graph.json` exactly, including schema-required fields |
| `model_graph_has_context` | Unit/Integration | `fixtures/tiny-single-file` | Prove every graph is tied to repository and branch context | `ModelGraphDto.context` includes workspace ID, repository ID, branch, and writable state |
| `derive_item_to_file_traceability` | Unit/Integration | `fixtures/tiny-single-file` | Prove source ownership is first-class traceability | `ModelGraphDto.traceLinks` includes item-to-file links for every structured node |
| `derive_import_traceability` | Unit/Integration | `fixtures/multi-file-modular` | Prove file-to-file traceability starts in the read-only slice | Matches `expected/trace-links.json` for source ownership and resolved imports |
| `malformed_input_reports_diagnostic` | Parser/Error | `fixtures/invalid-input` | Fail safely | Matches `expected/diagnostics.json`; valid files in the same repo still load |
| `get_source_file_preserves_text` | Integration | `fixtures/tiny-single-file` | Prove source text can be displayed without rewriting | Returned content, line ending, and hash match the fixture |
| `open_browse_select_smoke` | End-to-End | `fixtures/tiny-single-file` | Prove the first user path | A repo opens, the tree renders, and a selected node shows inspector data and source ownership from `expected/graph.json` |

Fixtures:

- `fixtures/phase-1-browser/`
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

Features:

- Type palette interaction for supported SysML element creation
- Drag SysML type onto canvas
- Click type, then click canvas to place
- Create part/package/requirement
- Create relationship by dragging connector
- Add ports and features inline for supported element types
- Rename inline
- Keep generated source preview visible in split mode
- Show intended save target before write
- Auto-layout
- Save generated SysML text
- Undo/redo
- Validation feedback in the pane header and bottom status bar

Test focus:

- Drag-to-create and click-to-create flows
- Inline rename
- Connector creation
- Delete and undo/redo
- Auto-layout after edits
- Save and reparse round-trip

Minimum gate tests:

| Test | Layer | Fixture | Purpose | Expected Result |
| --- | --- | --- | --- | --- |
| `parse_round_trip_minimal` | Unit/Integration | `fixtures/tiny-single-file` | Prove the parser and writer agree on the supported subset | Matches `expected/graph.json` after parse -> write -> parse |
| `save_touches_only_owner` | Integration | `fixtures/multi-file-modular` | Prevent rewrite churn | Matches `expected/changed-files.json` |
| `stable_id_survives_rename` | Integration | `fixtures/tiny-single-file` | Protect identity | Renaming an element keeps the same stable ID and preserves view bindings |
| `missing_id_blocks_write_until_backfill` | Integration | fixture to add with missing identity metadata | Keep identity policy explicit | Read succeeds with diagnostics or read-only derived IDs; write is blocked until the backfill command is implemented |

Fixtures:

- `fixtures/phase-2-editing/`
- `fixtures/tiny-single-file/`
- `fixtures/multi-file-modular/`

Success criterion:

> A user can create a simple architecture visually and see valid textual SysML generated.

Exit criteria:

- Supported edits can be created visually and persisted safely.
- Undo/redo restores the expected model state.
- Generated text remains stable and reviewable.

Vision trace:

- Supports: PowerPoint-level ease, textual SysML as source of truth, validation before write
- Tradeoff: starts with a constrained SysML subset instead of broad language coverage

### Phase 3: Git-Native Workflow

Make it useful in real engineering teams.

Features:

- Branch switcher
- Side-by-side branch contexts for the same repository
- Multi-context comparison projection using `MultiContextViewDto`
- Compare selector in the top app bar
- Commit panel
- Visual diff
- Text diff
- Split visual/text comparison panes
- Branch comparison
- Branch-to-branch trace links
- Changed-node highlighting
- Changed-file highlighting
- Added/removed/modified/unchanged legend
- Local changes overlay
- Basic merge conflict assistance

Test focus:

- Branch switching
- Working tree status
- Commit creation
- Semantic diff
- Branch comparison
- Changed-node overlays
- Conflict detection and assistance

Minimum gate tests:

| Test | Layer | Fixture | Purpose | Expected Result |
| --- | --- | --- | --- | --- |
| `semantic_branch_diff` | Integration | `fixtures/branch-divergence` | Verify branch comparison | Matches `expected/diff.json` |
| `branch_trace_links` | Integration | `fixtures/branch-divergence` | Preserve branch-to-branch traceability contract | Diff output includes changed files, changed items, and branch trace links |
| `multi_context_view_scopes_ids` | Integration | `fixtures/branch-divergence` | Prevent ID ambiguity in side-by-side views | Matches `expected/multi-context-view.json`; every projected node, file, and trace-link ID is scoped by `workspaceId` |

Fixtures:

- `fixtures/branch-divergence/`
- `fixtures/multi-file-modular/`
- `fixtures/merge-conflict/`

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

Features:

- Open two worktrees for different branches of the same repository
- Create a worktree only through an explicit user-requested backend operation
- Open multiple repositories in one workspace
- Edit supported files in separate writable contexts
- Keep save and commit targets scoped to one repository and branch
- Show context labels on graph, source, trace, diff, save, and commit surfaces
- Show writable, read-only, dirty, and validation state directly in pane headers
- Prevent cross-context writes from shared comparison panes

Test focus:

- Opening two worktrees for different branches of the same repository
- Explicit backend-validated worktree creation
- Opening multiple repositories in one workspace
- Editing files in separate writable contexts
- Save and commit targets scoped to one repository and branch
- Context labels on graph, source, trace, diff, save, and commit surfaces

Minimum gate tests:

| Test | Layer | Fixture | Purpose | Expected Result |
| --- | --- | --- | --- | --- |
| `multi_context_identity` | Integration | fixture to add with two worktrees | Preserve same-repo multi-branch editing path | Two branch contexts have distinct workspace IDs, branch names, writable states, and non-ambiguous save targets |
| `worktree_creation_is_explicit` | Integration | fixture to add with two worktrees | Prevent hidden writes during comparison | Branch comparison does not create a worktree; `POST /workspace-contexts/worktrees` is required before both branches are writable |

Fixtures:

- `fixtures/branch-divergence/`
- `fixtures/multi-context-editing/`

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

Features:

- Saved views
- Shared repo views
- Local private views
- Trace matrix
- Source ownership view
- Cross-repository dependency view
- Multi-context comparison view
- Node-centered inspector trace view
- Impact analysis
- Query/filter system
- View publishing
- Visual indicators for requirements, satisfies, verifies, allocations, ports, connections, files, and changed contexts

Test focus:

- Shared and local view persistence
- Stable ID based view restoration
- Trace matrix generation
- Impact analysis
- Filter and query rules
- View publishing and reloading

Fixtures:

- `fixtures/multi-file-modular/`
- `fixtures/branch-divergence/`
- `fixtures/saved-views/`

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

Features:

- More SysML types
- Behavior modeling
- State/action views
- Variant/product-line views
- Validation rules
- Model libraries
- CI integration
- Report generation
- Advanced workspace layouts, saved pane arrangements, and optional simplified "PowerPoint mode"

Test focus:

- New language constructs and validations
- Model library imports
- Behavioral modeling features
- Variant/product-line views
- CI validation and report generation

Fixtures:

- `fixtures/advanced-sysml/`
- `fixtures/model-libraries/`
- `fixtures/variant-models/`

Success criterion:

> The tool becomes a serious MBSE workbench, not just a diagram editor.

Exit criteria:

- New syntax is added with tests before or alongside implementation.
- Existing supported subset behavior remains stable.

Vision trace:

- Supports: serious MBSE workflow, broader SysML semantics, long-lived engineering adoption
- Tradeoff: advanced modeling waits until the workbench, graph, write, and Git foundations are reliable

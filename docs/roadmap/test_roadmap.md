# Test Roadmap for `sysml2_editor`

This document turns the development phases in [roadmap.md](./roadmap.md) into testable slices.
It stays close to [starter-test-matrix.md](../testing/starter-test-matrix.md) and the fixture
rules in [fixtures/README.md](../../fixtures/README.md).

## Phase 0: Visual Workbench Shell

Primary test goal: prove the shell can communicate multi-context SysML work before backend data
is connected.

Phase 0 should stay small and deterministic. Reuse the checked-in fixture families instead of
inventing broad new coverage.

The first slice should use fixture-backed component tests for shell rendering and a small unit
test seam for state projection.

### Unit test matrix

| Test | Layer | Fixture | Purpose | Expected Result |
| --- | --- | --- | --- | --- |
| `shell_renders_workbench_chrome` | Component | `fixtures/tiny-single-file` | Prove the shell composes the workbench scaffold | Top app bar, left rail, tiled workspace, inspector, and status bar render in the expected slots |
| `pane_headers_show_context_identity` | Component | `fixtures/branch-divergence` | Prove every pane carries visible identity | Repository, branch, file, mode, and write-state labels are present on each pane header |
| `pane_mode_toggle_is_local` | Component | `fixtures/branch-divergence/expected/multi-context-view.json` | Prove `Visual / Text / Split / Diff` controls only affect the targeted pane | Switching one pane does not mutate sibling panes or lose context labels |
| `fixture_state_drives_selection_summary` | Unit | `fixtures/tiny-single-file/expected/graph.json` | Prove selected-model metadata can populate the inspector stub | Selected node summary, attributes, and source ownership can be derived from fixture state |
| `responsive_fallback_preserves_identity` | Component | `fixtures/tiny-single-file` | Prove the shell remains readable on narrow layouts | Labels may wrap or compress, but repository and branch identity remain visible |
| `dirty_readonly_validation_states_render` | Component | `fixtures/branch-divergence/expected/multi-context-view.json` | Prove shell state badges are visible before editing exists | Dirty, read-only, and validation indicators appear in the pane chrome and status bar |

### Fixture set

| Fixture | Role in Phase 0 |
| --- | --- |
| `fixtures/tiny-single-file/` | Baseline single-repository shell state for one-context rendering and inspector summary tests |
| `fixtures/branch-divergence/` | Multi-branch sample state for comparison layouts and pane identity tests |
| `fixtures/multi-file-modular/` | Optional traceability-heavy sample if the shell needs file ownership examples in the sidebar |

### Exit criteria

- A user can identify the active repository, branch, file, selected model element, and write
  state from visible UI alone.
- Visual, text, split, and diff panes can be represented with fixture data.
- The shell is ready for parser, graph, inspector, and Git data integration.

## Phase 1: Read-only Model Browser

Primary test goal: prove the parser, indexer, and browser are trustworthy.

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

Exit criteria:

- Existing SysML repos can be opened without modifying files.
- Selected elements always map back to source text and metadata.
- Parse failures are visible and actionable.
- Trace links between model items and source files are present.

## Phase 2: Visual Editing

Primary test goal: prove visual changes produce valid textual SysML.

Test focus:

- Drag-to-create and click-to-create flows
- Inline rename
- Connector creation
- Delete and undo/redo
- Auto-layout after edits
- Save and reparse round-trip

Exit criteria:

- Supported edits can be created visually and persisted safely.
- Undo/redo restores the expected model state.
- Generated text remains stable and reviewable.

## Phase 3: Git-Native Workflow

Primary test goal: prove architecture changes can be reviewed through Git.

Test focus:

- Branch switching
- Working tree status
- Commit creation
- Semantic diff
- Branch comparison
- Changed-node overlays
- Conflict detection and assistance

Exit criteria:

- Users can compare two revisions without reading raw diffs only.
- Commit output summarizes model changes clearly.
- Git operations do not corrupt the repository state.

## Phase 3b: Multi-Context Editing

Primary test goal: prove multiple writable contexts coexist safely.

Test focus:

- Opening two worktrees for different branches of the same repository
- Worktree creation is explicit and backend-validated
- Opening multiple repositories in one workspace
- Editing files in separate writable contexts
- Save and commit targets scoped to one repository and branch
- Context labels on graph, source, trace, diff, save, and commit surfaces

Exit criteria:

- Two writable contexts for the same repository do not interfere with each other.
- Save and commit operations target the correct workspace context.
- Combined views use `MultiContextViewDto` and never collapse IDs.

## Phase 4: Custom Views and Traceability

Primary test goal: prove the workbench can support stakeholder-specific lenses.

Test focus:

- Shared and local view persistence
- Stable ID based view restoration
- Trace matrix generation
- Impact analysis
- Filter and query rules
- View publishing and reloading

Exit criteria:

- Views survive rename/move operations.
- Traceability data stays consistent with the graph index.
- A saved view can be reopened on another session and still resolve correctly.

## Phase 5: Advanced SysML v2 Support

Primary test goal: expand language coverage without breaking MVP behavior.

Test focus:

- New language constructs and validations
- Model library imports
- Behavioral modeling features
- Variant/product-line views
- CI validation and report generation

Exit criteria:

- New syntax is added with tests before or alongside implementation.
- Existing supported subset behavior remains stable.

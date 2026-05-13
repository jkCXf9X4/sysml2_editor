# Test Strategy for `sysml2_editor`

## Purpose

This document defines how to test `sysml2_editor` as a Git-native SysML v2 workbench.

The plan makes three things non-negotiable:

1. Textual SysML is the source of truth.
2. Visual editing must round-trip to stable, reviewable text.
3. Git behavior must stay predictable across branches, files, and operating systems.

## Scope

The strategy covers the product areas described in [plan.md](./plan.md):

- Repository open/clone/status/checkout flows
- SysML file discovery, parsing, indexing, and validation
- Hierarchical browsing, graph views, inspector data, and search
- Visual editing of the MVP subset
- Text editing and round-trip persistence
- Git-native workflows such as diff, branch compare, commit, and conflict handling
- Saved views, layouts, and traceability artifacts
- Cross-platform behavior on Windows and Linux

Out of scope for the first iteration:

- Full SysML v2 language coverage
- Advanced behavioral modeling
- Deep merge automation beyond safe assistance and clear conflict surfacing
- Large-scale performance claims before the parser and model graph are stable

## Test Principles

### 1. Test semantics before pixels

The highest-value assertions are about model meaning:

- Does the model graph contain the right elements and relationships?
- Does editing preserve identity, ownership, and references?
- Does the saved SysML text match the intended model change?

Visual tests matter, but they are secondary to semantic correctness.

### 2. Protect round-trips

Any supported edit must survive this loop:

`Git repo -> parse -> model graph -> UI edit -> text write -> parse again`

The second parse should reconstruct the same supported semantics.

### 3. Preserve stable identity

Views, layouts, and trace links should reference stable IDs, not display names.

Tests should verify that rename and move operations do not break:

- Node selection
- Layout recovery
- Trace overlays
- Cross-file references

### 4. Keep Git diffs intentional

The editor should not rewrite unrelated content. Tests must catch:

- Whitespace churn
- Line-ending drift
- File reordering
- Spurious import changes
- Unnecessary formatting noise

### 5. Fail safely

If parsing or editing cannot be completed safely, the app should surface an error instead of silently corrupting the model.

## Test Layers

### Unit tests

Use unit tests for fast, deterministic coverage of pure logic:

- SysML parsing for the supported subset
- Model graph construction and normalization
- Relationship validation rules
- Stable ID generation and lookup
- View JSON serialization/deserialization
- Diff computation for model elements
- Git command wrapper behavior at the boundary level

### Component tests

Use component tests for isolated UI and service behavior:

- Type palette filtering and grouping
- Inspector rendering for selected nodes and multi-select
- Tree navigation and breadcrumb updates
- Canvas item creation, connection, selection, and deletion
- View configuration editing
- Validation badges and status presentation

### Integration tests

Use integration tests to verify real interaction between modules:

- Open a local repository and build the model index
- Read and write supported SysML files
- Switch branches and refresh the model graph
- Compute semantic diffs between revisions
- Persist a visual edit and reparse the result
- Handle file watcher events and refresh stale data

### End-to-end tests

Use a small number of end-to-end tests for the full user journey:

- Open repo -> browse tree -> inspect node -> edit -> save -> commit
- Open two branches -> compare architectures -> review changed nodes
- Add a new supported element visually -> confirm the written SysML is valid
- Create a saved view -> reopen it -> confirm stable layout and filters

### Visual regression tests

Use snapshot or image-based tests only for rendering-specific behavior:

- Canvas layout stability
- Node/edge styling for status states
- Inspector and diff panel presentation
- Empty states and error states

Do not rely on visual snapshots for core model correctness.

### Performance tests

Add performance coverage for workflows that can degrade on real repositories:

- Opening large repos
- Building the model graph
- Search and trace queries
- Layout generation
- Branch diff generation
- File save and refresh latency

## MVP Coverage

The MVP supports:

- Package
- Part definition
- Part usage
- Port
- Connection
- Requirement
- Satisfy / trace relationship

The initial test suite should focus on those elements first.
The minimum gate set for the first implementation slice is defined in [starter-test-matrix.md](./testing/starter-test-matrix.md).
Concrete starter fixtures live under [fixtures](../fixtures).

### MVP round-trip cases

- Single-file model with one package and a few parts
- Multi-file model with imports and cross-file references
- Rename a part and confirm references update correctly
- Add a port and connection, then verify the saved text
- Add a requirement and satisfy edge, then verify traceability output
- Delete an element and confirm all dependent edges are handled safely

### MVP negative cases

- Unsupported SysML syntax appears in an input file
- A file is partially written or malformed
- A relationship points to a missing target
- Two elements share a display name but not an identity
- A save operation would rewrite unrelated content

## Phase-Based Strategy

### Phase 1: Read-only model browser

Primary test goal: prove the parser, indexer, and browser are trustworthy.

Test focus:

- Repo open and file discovery
- Parse success and parse error reporting
- Tree hierarchy rendering
- Text editor view sync
- Graph view node/edge projection
- Search and selection behavior

Exit criteria:

- Existing SysML repos can be opened without modifying files.
- Selected elements always map back to source text and metadata.
- Parse failures are visible and actionable.

### Phase 2: Visual editing

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

### Phase 3: Git-native workflow

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

### Phase 4: Custom views and traceability

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

### Phase 5: Advanced SysML v2 support

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

## Test Fixtures

The repo should maintain a small fixture library that grows with the product.

Recommended fixture categories:

- `tiny-single-file`: one package, a few supported elements
- `multi-file-modular`: packages and cross-file references
- `branch-divergence`: two branches with semantic differences
- `merge-conflict`: overlapping edits and a resolvable conflict
- `invalid-input`: syntax errors and partial writes
- `large-model`: enough data to exercise search and layout performance
- `cross-platform-paths`: spaces, Unicode, and line-ending sensitive paths

Fixture rules:

- Keep fixtures deterministic and small unless the test is explicitly performance-oriented
- Prefer checked-in fixtures over dynamically generated ones for regression tests
- Version any generated expected output alongside the fixture
- Use the same fixtures across unit, integration, and E2E tests when possible

## Automation and CI

### Per-commit gates

Run on every change:

- Formatting and linting
- Unit tests
- Fast integration tests
- Small smoke E2E test
- Snapshot checks for key UI states

### PR gates

Run on pull requests:

- Full unit and integration suite
- Core E2E journeys
- Cross-platform sanity checks
- Semantic diff regression checks

### Nightly gates

Run on a schedule:

- Larger fixture sets
- Performance benchmarks
- Cross-platform matrix
- Flakier UI and file-watcher scenarios

## Cross-Platform Requirements

Because the plan calls for Windows and Linux support, tests must explicitly cover:

- Path separators
- Case sensitivity differences
- Line endings
- File locking and watcher behavior
- Git CLI availability and failure modes
- Local file permissions

If the app uses a desktop wrapper later, add packaging tests for each target platform.

## Non-Functional Requirements

### Reliability

Test that the app never silently drops:

- Model edits
- View settings
- Trace links
- Git status information

### Responsiveness

Test that common actions remain interactive:

- Open repo
- Select node
- Search
- Drag a node
- Save a change

### Accessibility

At minimum, verify keyboard access for:

- Search
- Tree navigation
- Inspector focus
- Core editor commands

### Data safety

Test that a failed parse or failed write leaves the repository in a recoverable state.

## Risk-Based Priorities

Highest priority risks:

- Parser or writer drift causing semantic corruption
- Visual edits producing unstable text diffs
- Git branch comparison showing the wrong change set
- View persistence breaking after rename or move
- Windows/Linux behavior diverging

Medium priority risks:

- Performance degradation on larger repos
- Layout instability
- Edge cases in traceability and search

Lower priority until later phases:

- Advanced behavioral modeling
- Rich presentation polish
- Deep merge automation

## Practical Exit Criteria for the Project

The project is ready for serious use when all of the following are true:

- A supported SysML repo opens read-only and renders correctly
- A user can create and edit supported structure elements visually
- Saved text is stable enough for code review
- Branch differences are understandable semantically
- Views and traceability survive rename and reload cycles
- The same test suite passes on Windows and Linux

## Summary

The most important test discipline for `sysml2_editor` is to keep the text model, the internal graph, and the UI synchronized without losing Git fidelity.

If a test does not protect round-trip semantics, stable identity, or reviewable diffs, it is probably not worth prioritizing early.

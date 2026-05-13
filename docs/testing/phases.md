# Phase-Based Strategy

## Phase 1: Read-only model browser

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

## Phase 2: Visual editing

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

## Phase 3: Git-native workflow

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

## Phase 4: Custom views and traceability

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

## Phase 5: Advanced SysML v2 support

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

# Phases — Testing View

Testing focus and exit criteria aligned with the development phases in [roadmap.md](./roadmap.md).

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

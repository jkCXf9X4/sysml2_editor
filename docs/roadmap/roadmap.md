# Implementation Plan for `sysml2_editor`

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

## Development Phases

### Phase 0: Visual Workbench Shell

Build the static workbench shape before deep interaction.

Features:

- Top-level shell, left rail, tiled workspace, inspector, and status bar
- Pane-level context labels for repository, branch, file, mode, and write state
- Fixture-backed sample content that demonstrates multiple repositories and branches
- Responsive fallback that preserves context labels on narrower screens

Success criterion:

> A user can look at the UI and understand that the product is a multi-repository, multi-branch SysML workbench even before real backend data is connected.

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

Success criterion:

> A user can open an existing SysML repo and understand the product structure visually, including which source files define and connect the visible model elements.

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

Success criterion:

> A user can create a simple architecture visually and see valid textual SysML generated.

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

Success criterion:

> A user can compare two architecture alternatives on different branches without reading raw diffs.

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

Success criterion:

> A user can edit two branches or related repositories side by side without ambiguity about where each change will be written.

Runtime rules for multi-context editing are defined in [runtime.md](../architecture/runtime.md).

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

Success criterion:

> A team can create architecture views for different stakeholders from the same model.

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

Success criterion:

> The tool becomes a serious MBSE workbench, not just a diagram editor.

Vision trace:

- Supports: serious MBSE workflow, broader SysML semantics, long-lived engineering adoption
- Tradeoff: advanced modeling waits until the workbench, graph, write, and Git foundations are reliable

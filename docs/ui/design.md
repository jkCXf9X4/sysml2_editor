# UI Design

UI-specific design for `sysml2_editor`, derived from the former `docs/design.md`.

## Product Vision

[PRODUCT_VISION.md](../../PRODUCT_VISION.md) is the central product intent and traceability source.

Design summary: `sysml2_editor` should be a Git-native SysML v2 architecture workbench where the diagram is easy to manipulate like PowerPoint, but the model remains precise, traceable, reviewable, and version-controlled as text.

## Suggested UI Concept

### Target Workbench Vision

The target UI is an IDE-style SysML v2 workbench: dense, context-aware, and optimized for systems engineers comparing and editing model structure across Git repositories, branches, and source files.

The default workspace should converge on this shape:

```text
Top app bar:
  repository selector, compare selector, active branches, commit, pull, push,
  validation status, search, layout/settings controls

Left workbench rail:
  repositories and branches
  model tree
  type palette

Center tiled workspace:
  visual diagram panes
  textual SysML panes
  split visual/text panes
  semantic diff panes
  each pane labeled with repository, branch, file, mode, and write state

Right inspector:
  selected element identity
  attributes, ports, parts, connections
  traceability
  source ownership
  validation and Git metadata

Bottom status bar:
  active context, dirty file counts, model validity, cursor position,
  indentation, encoding, SysML version
```

This is the long-term visual target. Early implementation slices may use fixture-backed mock data, but every visible surface should preserve the same concepts: context identity, source ownership, model traceability, validation state, and safe write targets.

#### Visual Success Criteria

The workbench is visually on track when:

- A user can tell which repository, branch, file, and model element they are looking at without opening a modal.
- Every visual, text, split, and diff pane carries an explicit context label.
- Editable and read-only contexts are visually distinct.
- Selecting a model element updates the model tree, source text, diagram selection, and inspector coherently.
- Branch and repository comparisons are represented as side-by-side contexts, not as hidden global state.
- Commit and save actions always show the target repository and branch before writing.
- Validation status remains visible while the user edits.

#### Visual Implementation Principles

- Prefer a workbench layout over a landing page or document viewer.
- Keep diagrams as editable projections over textual SysML, not as independent source-of-truth drawings.
- Make Git context visible at the pane level, not only in the global toolbar.
- Use color to reinforce context and change state, but do not rely on color alone.
- Keep controls compact and repeatable; this is an engineering tool, not a marketing interface.
- Add visual polish only where it improves scanability, selection clarity, or write safety.

### Main Layout: Flexible Workbench

The UI should be a dockable workspace, not a fixed layout. Think VS Code + Figma + systems engineering browser.

The three main areas should be movable panels.

### A. Type Palette

Purpose: create new SysML elements quickly.

This is the SysML-aware equivalent of a PowerPoint shape menu.

It should include:

- Parts
- Ports
- Interfaces
- Requirements
- Actions
- States
- Constraints
- Connections
- Allocations
- Use cases
- Views/viewpoints
- Packages
- Custom stereotypes/extensions, if supported later

Recommended behavior:

- Drag type into canvas to create.
- Click type, then click canvas to place.
- Search/filter types.
- Show recent/favorite types.
- Group by SysML category.
- Show valid drop targets based on selected context.

### B. Main Visualization Area

This should be the heart of the application.

It should support multiple simultaneous model views, not just one diagram.

Each tab should be able to show one of several view types:

1. Hierarchical structure view
2. Graph/network traceability view
3. Diagram/canvas view
4. Textual SysML editor
5. Branch/revision comparison view
6. Repository comparison view
7. Custom saved view

The key idea: the model is the source of truth; diagrams are editable views into it. Every tab should carry a visible context label for repository, branch, and writable/read-only state.

### C. Attribute / Inspector Area

Purpose: show and edit details of the selected item or items.

For a selected node, show:

- Name
- Type
- Qualified path
- Owning package
- Definition text
- Attributes
- Relationships
- Incoming traces
- Outgoing traces
- Source file
- Git status
- Validation errors
- Documentation/comments

For multiple selected items, show:

- Shared attributes
- Batch-editable properties
- Common relationships
- Alignment/layout tools

## Recommended Workspace Layout

A good default layout would be:

```text
Top bar: Repo / Branch / Search / Validation / Commit / Sync
Left: Type Palette
Center: Main Visualization Area
Right: Inspector
```

Because panel position should not be fixed, support:

- Dragging panels
- Splitting views
- Floating windows
- Saving workspace layouts
- PowerPoint mode for simple users
- Engineer mode for full model/text/git detail
- Side-by-side branch views from the same repository
- Side-by-side repository views for related system, library, or supplier models

## Core Workflows

### Workflow 1: Open Project

The user should not open a single file. They should open a Git repository.

Flow:

```text
Open Project
 -> Select local Git repo or clone remote repo
 -> Detect SysML model files
 -> Parse model
 -> Build model index
 -> Show branches
 -> Open default workspace/view
```

The repo is the project boundary.

The app should understand:

- Current branch
- Other local branches
- Remote branches
- Multiple active branches from the same repository
- Multiple active repositories in the same workspace
- Which active contexts are writable and which are read-only comparison contexts
- Uncommitted changes
- Model files
- Saved views
- Validation status

### Workflow 1b: Work Across Branches And Repositories

The user should be able to keep several contexts open at once instead of repeatedly closing and reopening projects.

Flow:

```text
Open Project
 -> Open another branch, worktree, or related repository
 -> Place contexts side by side
 -> Compare structure, traceability, source ownership, and diffs
 -> Edit in writable contexts
 -> Commit changes to the correct repository and branch
```

Rules:

- A context is the combination of repository, branch or worktree, commit state, and writable status.
- Editing is allowed only in contexts with a distinct safe write location.
- Read-only comparison contexts should still support navigation, traceability, source viewing, and diff overlays.
- Combined branch or repository screens are projections over contexts, not merged graphs.
- Creating a worktree is an explicit user-requested backend operation, not a side effect of opening a comparison.
- Save and commit UI must always show the target repository and branch before writing.

### Workflow 2: Create Architecture Visually

This is the most important user experience.

The user should be able to:

1. Drag a SysML type onto canvas.
2. Name it directly.
3. Connect it to another item.
4. Choose relationship type from a contextual menu.
5. Add ports or features inline.
6. Group elements into higher-level abstractions.
7. Save the result back into textual SysML files.

The user experience should feel like:

```text
PowerPoint simplicity
+ SysML semantic correctness
+ Git traceability
```

Avoid forcing the user to start with text. Let visual editing generate correct SysML textual notation behind the scenes.

### Workflow 3: Navigate Large Systems

Complex systems need hierarchical navigation.

Recommended navigation mechanisms:

- Breadcrumb navigation
- Drill-down
- Zoom levels
- Semantic folding
- Minimap

### Workflow 4: Traceability

Traceability should be a first-class feature, not a report hidden somewhere.

Every node should have visible trace indicators. Traceability views should make relationships navigable at several levels:

- Item-to-item: requirements, satisfies, traces, ports, connections, and dependencies
- Item-to-file: source file ownership, references, and save impact
- File-to-file: imports, package modularity, generated views, and layout artifacts
- Branch-to-branch: element and file changes between alternatives
- Repo-to-repo: external model libraries, supplier repositories, and related engineering repositories
- Context-to-context: side-by-side navigation across opened branches and repositories

Useful traceability views:

1. Node-centered trace view
2. Trace matrix
3. Impact analysis view
4. Source ownership view
5. Cross-repository dependency view
6. Multi-context comparison view

### Workflow 5: Custom Views

Treat custom views as lightweight configuration artifacts.

A view should define:

- Included nodes
- Excluded nodes
- Layout
- Filters
- Color rules
- Abstraction level
- Branch/repo source
- View type
- Optional query

Two storage modes:

- Shared view, stored in repo and committed/reviewed
- Local view, stored in user settings and not committed

Recommended folder convention:

```text
.sysml2_editor/
  views/
  layouts/
  settings.json
```

## Suggested Feature: Architecture Lens

A custom view could be called a Lens.

Examples:

- Safety Lens
- Electrical Lens
- Software Allocation Lens
- Supplier Lens
- Change Impact Lens

A lens is basically:

```json
{
  "name": "Safety Lens",
  "includeTypes": ["Requirement", "Part", "VerificationCase"],
  "includeRelations": ["satisfy", "verify", "trace"],
  "groupBy": "subsystem",
  "layout": "hierarchical",
  "storedInRepo": true
}
```

This would make the tool feel more powerful than a normal diagramming tool.

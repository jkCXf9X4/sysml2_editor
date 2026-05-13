# Implementation Plan for `sysml2_editor`

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

### Phase 1: Read-only Model Browser

Build confidence in repo parsing and navigation.

Features:

- Open Git repo
- Create one explicit workspace context for the opened repo and current branch
- Parse SysML files
- Show tree hierarchy
- Show text editor
- Show graph view
- Click graph node -> show source text and attributes
- Show source ownership and item-to-item trace links for selected nodes
- Show file-to-file import traceability for modular models
- Basic search

Success criterion:

> A user can open an existing SysML repo and understand the product structure visually, including which source files define and connect the visible model elements.

### Phase 2: Visual Editing

Add PowerPoint-like editing.

Features:

- Drag SysML type onto canvas
- Create part/package/requirement
- Create relationship by dragging connector
- Rename inline
- Auto-layout
- Save generated SysML text
- Undo/redo

Success criterion:

> A user can create a simple architecture visually and see valid textual SysML generated.

### Phase 3: Git-Native Workflow

Make it useful in real engineering teams.

Features:

- Branch switcher
- Side-by-side branch contexts for the same repository
- Multi-context comparison projection using `MultiContextViewDto`
- Commit panel
- Visual diff
- Branch comparison
- Branch-to-branch trace links
- Changed-node highlighting
- Changed-file highlighting
- Local changes overlay
- Basic merge conflict assistance

Success criterion:

> A user can compare two architecture alternatives on different branches without reading raw diffs.

### Phase 3b: Multi-Context Editing

Allow more than one writable context when each context has a distinct safe write location.

Features:

- Open two worktrees for different branches of the same repository
- Create a worktree only through an explicit user-requested backend operation
- Open multiple repositories in one workspace
- Edit supported files in separate writable contexts
- Keep save and commit targets scoped to one repository and branch
- Show context labels on graph, source, trace, diff, save, and commit surfaces

Success criterion:

> A user can edit two branches or related repositories side by side without ambiguity about where each change will be written.

Runtime rules for multi-context editing are defined in [runtime.md](../architecture/runtime.md).

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
- Impact analysis
- Query/filter system
- View publishing

Success criterion:

> A team can create architecture views for different stakeholders from the same model.

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

Success criterion:

> The tool becomes a serious MBSE workbench, not just a diagram editor.

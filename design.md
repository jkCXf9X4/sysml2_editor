# Design Plan for `sysml2_editor`

## Product Vision

[PRODUCT_VISION.md](./PRODUCT_VISION.md) is the central product intent and traceability source.

Design summary: `sysml2_editor` should be a Git-native SysML v2 architecture workbench where the diagram is easy to manipulate like PowerPoint, but the model remains precise, traceable, reviewable, and version-controlled as text.

That direction is aligned with SysML v2: textual notation is intended to support precise model definition, large-model editing, Git-based configuration management, validation, generation, and CI workflows.

Vision trace:

- Supports: visual editing without making diagrams the source of truth; textual SysML in Git as durable source of truth; visible item, file, branch, and repository traceability.
- Tradeoff: early design favors a structured engineering workbench over a generic diagramming tool.

## Suggested UI Concept

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
6. Custom saved view

The key idea: the model is the source of truth; diagrams are editable views into it.

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
- Uncommitted changes
- Model files
- Saved views
- Validation status

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

Useful traceability views:

1. Node-centered trace view
2. Trace matrix
3. Impact analysis view
4. Source ownership view
5. Cross-repository dependency view

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

## Git and Branch Model

The app should treat Git as a visible modeling concept, not just a hidden backend.

Key Git features:

- Open local repo
- Clone repo
- Checkout branch
- Show multiple branches simultaneously
- Show multiple repos simultaneously
- Diff branches visually
- Compare model elements between branches or repos
- Commit from inside the app
- Pull/push
- Show file-level and model-element-level changes
- Resolve merge conflicts at the model level where possible

## Data / Model Architecture

Recommended architecture:

```text
Git repo
  -> Textual SysML files
  -> Parser service
  -> Internal model graph
  -> View model
  -> Canvas / tree / table / editor UI
```

The source of truth should be the textual SysML files in Git.

Internally, maintain a graph database-like model:

```text
Node:
  - id
  - type
  - name
  - qualifiedName
  - sourceFile
  - sourceRange
  - branch
  - attributes

Edge:
  - source
  - target
  - relationshipType
  - sourceFile
  - sourceRange
  - branch
```

Do not make the canvas the model. The canvas is just one projection of the model.

## Key Design Principles

### 1. Text is truth, diagrams are views

Never store important model semantics only in canvas coordinates.

### 2. Make invalid modeling difficult

The UI should guide users toward valid SysML.

### 3. Separate and visualize changes compared to the committed model

The editor should continuously compare the working model against the committed baseline.

### 4. Make abstraction levels explicit

Large systems fail visually when everything is shown at once.

### 5. Make traceability a first-class part of the model hierarchy

Traceability should appear in node badges, inspector, hover cards, source ownership view, impact view, matrix view, cross-repository dependency view, and commit summary.

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

## One-Sentence Product Definition

`sysml2_editor` is a Git-native SysML v2 architecture editor that lets engineers model systems visually with PowerPoint-level ease while preserving textual precision, traceability, validation, and version control.

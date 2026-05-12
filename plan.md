
## Product vision

**sysml2_editor should be a Git-native SysML v2 architecture workbench where the diagram is easy to manipulate like PowerPoint, but the model remains precise, traceable, reviewable, and version-controlled as text.**

That direction is well aligned with SysML v2: textual notation is intended to support precise model definition, large-model editing, Git-based configuration management, validation, generation, and CI workflows. ([SysML.org][1]) The public SysML v2 pilot implementation already focuses on textual notation and visualization, and uses technologies such as Xtext and PlantUML. ([GitHub][2])

---

# 1. Suggested UI concept

## Main layout: flexible workbench

The UI should be a **dockable workspace**, not a fixed layout. Think VS Code + Figma + systems engineering browser.

The three main areas you described should be implemented as movable panels:

### A. Type palette

Purpose: create new SysML elements quickly.

This is the “PowerPoint shape menu” equivalent, but SysML-aware.

It should include:

* Parts
* Ports
* Interfaces
* Requirements
* Actions
* States
* Constraints
* Connections
* Allocations
* Use cases
* Views/viewpoints
* Packages
* Custom stereotypes/extensions, if supported later

Recommended behavior:

* Drag type into canvas to create.
* Click type, then click canvas to place.
* Search/filter types.
* Show recent/favorite types.
* Group by SysML category.
* Show valid drop targets based on selected context.

Example:

```text
Type Palette
 ├─ Structure
 │   ├─ Part
 │   ├─ Port
 │   ├─ Interface
 │   └─ Connection
 ├─ Requirements
 │   ├─ Requirement
 │   ├─ Constraint
 │   └─ Verification Case
 ├─ Behavior
 │   ├─ Action
 │   ├─ State
 │   └─ Transition
 └─ Views
     ├─ View
     ├─ Viewpoint
     └─ Custom View
```

---

### B. Main visualization area

This should be the heart of the application.

It should support **multiple simultaneous model views**, not just one diagram.

Recommended structure:

```text
Main Area
 ├─ Tab 1: Product Structure - Main
 ├─ Tab 2: Electrical Architecture
 ├─ Tab 3: Requirements Traceability
 ├─ Tab 4: Branch Comparison: main vs concept-x
 └─ Tab 5: Repo B / Supplier Model
```

Each tab should be able to show one of several view types:

1. **Hierarchical structure view**
2. **Graph/network traceability view**
3. **Diagram/canvas view**
4. **Textual SysML editor**
5. **Branch/revision comparison view**
6. **Custom saved view**

The key idea: **the model is the source of truth; diagrams are editable views into it.**

---

### C. Attribute / inspector area

Purpose: show and edit details of selected item or items.

For a selected node, show:

* Name
* Type
* Qualified path
* Owning package
* Definition text
* Attributes
* Relationships
* Incoming traces
* Outgoing traces
* Source file
* Git status
* Validation errors
* Documentation/comments

For multiple selected items, show:

* Shared attributes
* Batch-editable properties
* Common relationships
* Alignment/layout tools

Example:

```text
Inspector: BatteryController
Type: Part Definition
Path: Vehicle.Power.BatteryController
File: model/power/battery.sysml
Branch: main

Properties
 ├─ mass: MassValue
 ├─ voltage: VoltageValue
 └─ supplier: string

Relationships
 ├─ satisfies → REQ-042 Thermal Safety
 ├─ connected to → BatteryPack
 └─ allocated to → ECU-2

Git
 ├─ Modified locally
 └─ Last changed by Erik, commit a82f...
```

---

# 2. Recommended workspace layout

A good default layout would be:

```text
┌──────────────────────────────────────────────────────────────┐
│ Top bar: Repo / Branch / Search / Validation / Commit / Sync │
├───────────────┬────────────────────────────────┬─────────────┤
│ Type Palette  │ Main Visualization Area        │ Inspector   │
│               │                                │             │
│ SysML Types   │ Canvas / Tree / Text / Compare │ Attributes  │
│ Views         │                                │ Traces      │
│ Repos         │                                │ Git status  │
└───────────────┴────────────────────────────────┴─────────────┘
```

But since panel position should not be fixed, support:

* Dragging panels
* Splitting views
* Floating windows
* Saving workspace layouts
* “PowerPoint mode” for simple users
* “Engineer mode” for full model/text/git detail

---

# 3. Core workflows

## Workflow 1: Open project

The user should not open a single file. They should open a **Git repository**.

Flow:

```text
Open Project
 → Select local Git repo or clone remote repo
 → Detect SysML model files
 → Parse model
 → Build model index
 → Show branches
 → Open default workspace/view
```

Important: the repo is the project boundary.

The app should understand:

* Current branch
* Other local branches
* Remote branches
* Uncommitted changes
* Model files
* Saved views
* Validation status

---

## Workflow 2: Create architecture visually

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

---

## Workflow 3: Navigate large systems

Complex systems need hierarchical navigation.

Recommended navigation mechanisms:

### Breadcrumb navigation

```text
Vehicle > Propulsion > Battery System > BMS > Cell Balancing
```

### Drill-down

Double-click a component to enter its internal structure.

### Zoom levels

At high zoom:

```text
Vehicle
 ├─ Propulsion
 ├─ Chassis
 ├─ Thermal
 └─ Software
```

At lower level:

```text
Propulsion
 ├─ Battery System
 ├─ Inverter
 ├─ Motor
 └─ Cooling Interface
```

### Semantic folding

Allow the user to collapse:

* Parts
* Packages
* Subsystems
* Requirements groups
* Supplier components
* Variants

### Minimap

A minimap is essential for large diagrams.

---

## Workflow 4: Traceability

Traceability should be a first-class feature, not a report hidden somewhere.

Every node should have visible trace indicators:

```text
BatteryController
 ├─ satisfies 3 requirements
 ├─ verifies 1 test case
 ├─ allocated to 2 software components
 ├─ connected to 5 ports
 └─ changed in branch concept-b
```

Useful traceability views:

1. **Node-centered trace view**

Shows selected item in the center and all related items around it.

2. **Trace matrix**

Rows and columns, for example:

```text
Requirements × Components
Components × Tests
Functions × Logical Components
Logical Components × Physical Components
```

3. **Impact analysis view**

When an item changes, show what may be affected.

```text
Changed: BatteryVoltageSensor
Potentially affected:
 ├─ REQ-119 Voltage Accuracy
 ├─ TestCase-044 Sensor Calibration
 ├─ BatteryController
 └─ SafetyMonitor
```

---

## Workflow 5: Custom views

You should treat custom views as lightweight configuration artifacts.

A view should define:

* Included nodes
* Excluded nodes
* Layout
* Filters
* Color rules
* Abstraction level
* Branch/repo source
* View type
* Optional query

Two storage modes:

```text
Shared view
 → stored in repo
 → committed and reviewed

Local view
 → stored in user settings
 → not committed
```

Recommended folder convention:

```text
.sysml2_editor/
 ├─ views/
 │   ├─ product-structure.view.json
 │   ├─ safety-trace.view.json
 │   └─ branch-compare.view.json
 ├─ layouts/
 └─ settings.json
```

---

# 4. Git and branch model

Your app should treat Git as a visible modeling concept, not just a hidden backend.

## Key Git features

The app should support:

* Open local repo
* Clone repo
* Checkout branch
* Show multiple branches simultaneously
* Show multiple repos simultaneously
* Diff branches visually
* Compare model elements between branches or repos
* Commit from inside the app
* Pull/push
* Show file-level and model-element-level changes
* Resolve merge conflicts at the model level where possible

## Multi-branch visualization

Example:

```text
View: main vs concept-electric-drive

BatterySystem
 ├─ main: 12 components
 └─ concept-electric-drive: 16 components

Changed:
 ├─ Added: ThermalRunawayMonitor
 ├─ Modified: BatteryController
 └─ Removed: LegacyFuseBox
```

The killer feature would be:

> “Show me the architecture difference between these two branches.”

Not just text diff. A systems engineer wants semantic diff:

```text
Added component
Removed interface
Changed requirement satisfaction
Changed port type
Changed allocation
```

---

# 5. Data/model architecture

I would strongly recommend this architecture:

```text
Git repo
  ↓
Textual SysML files
  ↓
Parser / language server
  ↓
Internal model graph
  ↓
View model
  ↓
Canvas / tree / table / editor UI
```

## Source of truth

The source of truth should be:

```text
.sysml / .kerml textual files in Git
```

This aligns with the current SysML v2 direction, where textual notation is a first-class representation and can be diffed and versioned like code. ([SysGit][3])

## File organization strategy

Avoid placing the whole model in one large `.sysml` file. A single-file model is easy for the first demo, but it quickly becomes painful for review, merge conflicts, ownership, supplier boundaries, and partial loading.

Use a modular repository structure from the start:

```text
model/
 ├─ root.sysml
 ├─ domains/
 │   ├─ power/
 │   │   ├─ package.sysml
 │   │   ├─ battery-system.sysml
 │   │   └─ inverter.sysml
 │   ├─ thermal/
 │   │   ├─ package.sysml
 │   │   └─ cooling-loop.sysml
 │   └─ software/
 │       ├─ package.sysml
 │       └─ allocations.sysml
 ├─ requirements/
 │   ├─ safety.sysml
 │   └─ performance.sysml
 ├─ interfaces/
 │   ├─ electrical.sysml
 │   └─ mechanical.sysml
 └─ views/
     └─ view-model-imports.sysml
```

Recommended rules:

* `root.sysml` imports the main packages and acts as the model entry point.
* Each domain or subsystem owns its own package file.
* Large subsystems can be split into one file per major component or concern.
* Requirements, interfaces, allocations, and trace relationships may live in separate concern-oriented files when that reduces merge conflicts.
* The editor should preserve user file boundaries instead of rewriting the whole model.
* New visual elements should be inserted into the file that owns the current package or selected context.
* Cross-file relationships should be written as explicit references/imports, not by duplicating elements.
* Generated formatting should be stable so Git diffs stay readable.

This gives the app a clear writing policy:

```text
User creates element inside Battery System
 → find owning package
 → resolve source file for that package
 → insert element into battery-system.sysml
 → update imports only if needed
 → update view JSON layout separately
```

For the MVP, support a simple convention first:

```text
model/root.sysml
model/<package-name>.sysml
.sysml2_editor/views/*.view.json
```

Then expand to configurable project conventions after the editor can reliably parse, edit, and save small multi-file models.

## Internal representation

Internally, maintain a graph database-like model:

```text
Node:
 ├─ id
 ├─ type
 ├─ name
 ├─ qualifiedName
 ├─ sourceFile
 ├─ sourceRange
 ├─ branch
 └─ attributes

Edge:
 ├─ source
 ├─ target
 ├─ relationshipType
 ├─ sourceFile
 ├─ sourceRange
 └─ branch
```

Do not make the canvas the model. The canvas is just one projection of the model.

---

# 6. Technology suggestion

## Best initial technical stack

The app should run on both Windows and Linux. That requirement should influence the stack early, especially around desktop packaging, file watching, Git process handling, and line-ending behavior.

For a modern desktop-style engineering application, the UI still benefits from web technologies:

### Frontend

* **React**
* **TypeScript**
* **React Flow** or **ELK.js / Cytoscape.js** for graph visualization
* **Monaco Editor** for textual SysML editing
* **Golden Layout** or a custom dockable panel system
* **Electron**, **Tauri**, or a browser-hosted shell for desktop packaging

### Backend/core

The pragmatic starting point is:

```text
React + TypeScript UI
ASP.NET Core / C# local backend
Monaco
React Flow
Git CLI
```

Why this fits the current background:

* C# experience reduces backend risk.
* .NET runs well on Windows and Linux.
* Git and filesystem operations are straightforward from C#.
* The parser/model index can be implemented as strongly typed C# domain code.
* The React UI remains flexible for graph/canvas-heavy interaction.
* The frontend and backend can be separated cleanly enough to change packaging later.

Desktop packaging options:

* **Phase 1:** run as a local web app during development.
* **Phase 2:** package with Electron if quickest cross-platform delivery matters.
* **Alternative:** use Avalonia/.NET if a more native C# desktop shell becomes more important than web-based canvas flexibility.

Backend responsibilities:

* Repository open/clone/status operations.
* SysML file discovery.
* Parse/index pipeline.
* Stable model graph API.
* Save operations that preserve file boundaries.
* Git diff/status/commit integration.

Frontend responsibilities:

* Dockable workbench.
* Canvas/tree/matrix/text views.
* Visual editing gestures.
* Inspector and validation presentation.
* Change overlays and traceability navigation.

Git integration via:

* `git` CLI wrapper initially
* later `libgit2`/LibGit2Sharp only if CLI process integration becomes a limitation

* SysML parsing through:

* existing SysML v2 pilot implementation concepts
* language server integration
* or a custom parser after the product concept stabilizes

The SysML v2 pilot implementation already provides a proof-of-concept textual notation and visualization implementation. ([GitHub][2]) There are also SysML v2 language-server efforts that support editor features like syntax highlighting, diagnostics, completion, and correction suggestions. ([GitHub][4])

## Practical recommendation

Start with:

```text
React + TypeScript + C#/.NET backend + Monaco + React Flow + Git CLI
```

Why:

* Fast to prototype
* Uses existing C# experience
* Runs on Windows and Linux
* Web UI flexibility
* Easier graph/canvas work
* Monaco gives you a strong text editor immediately
* Git CLI is reliable and avoids premature complexity
* Keeps desktop packaging as a reversible decision

Later, replace pieces with more specialized implementations if needed.

---

# 7. MVP definition

Do not start by building “all of SysML v2.” Start by building the smallest useful Git-native architecture editor.

## MVP goal

> Open a Git repo, visualize SysML v2 structure, edit simple structure graphically or textually, and commit changes.

## MVP features

### Must-have

* Open local Git repo
* Detect SysML files
* Basic textual editor
* Parse a useful subset of SysML v2
* Show package/part hierarchy as tree
* Show selected structure as graph
* Create/edit/delete basic elements
* Save back to text
* Show attributes of selected element
* Commit changes
* Store custom views as JSON

### First supported model elements

Start with:

* Package
* Part definition
* Part usage
* Port
* Connection
* Requirement
* Satisfy/trace relationship

Avoid starting with full behavioral modeling. Structure + requirements + traceability gives the strongest early value.

---

# 8. Development phases

## Phase 1: Read-only model browser

Build confidence in repo parsing and navigation.

Features:

* Open Git repo
* Parse SysML files
* Show tree hierarchy
* Show text editor
* Show graph view
* Click graph node → show source text and attributes
* Basic search

Success criterion:

> A user can open an existing SysML repo and understand the product structure visually.

---

## Phase 2: Visual editing

Add PowerPoint-like editing.

Features:

* Drag SysML type onto canvas
* Create part/package/requirement
* Create relationship by dragging connector
* Rename inline
* Auto-layout
* Save generated SysML text
* Undo/redo

Success criterion:

> A user can create a simple architecture visually and see valid textual SysML generated.

---

## Phase 3: Git-native workflow

Make it useful in real engineering teams.

Features:

* Branch switcher
* Commit panel
* Visual diff
* Branch comparison
* Changed-node highlighting
* Local changes overlay
* Basic merge conflict assistance

Success criterion:

> A user can compare two architecture alternatives on different branches without reading raw diffs.

---

## Phase 4: Custom views and traceability

Add the features that make it better than PowerPoint.

Features:

* Saved views
* Shared repo views
* Local private views
* Trace matrix
* Impact analysis
* Query/filter system
* View publishing

Success criterion:

> A team can create architecture views for different stakeholders from the same model.

---

## Phase 5: Advanced SysML v2 support

Expand language coverage.

Features:

* More SysML types
* Behavior modeling
* State/action views
* Variant/product-line views
* Validation rules
* Model libraries
* CI integration
* Report generation

Success criterion:

> The tool becomes a serious MBSE workbench, not just a diagram editor.

---

# 9. Key design principles

## 1. Text is truth, diagrams are views

Never store important model semantics only in canvas coordinates.

The repo should contain:

```text
model semantics → SysML text
view preferences → view JSON
layout coordinates → view JSON
```

## 2. Make invalid modeling difficult

The UI should guide users toward valid SysML.

For example:

* Do not allow invalid connection types.
* Suggest legal relationship types.
* Warn when a requirement has no satisfying element.
* Warn when a port is untyped.
* Warn when a diagram element is only visual and not in the model.

## 3. Separate and visualize changes compared to the committed model

Sketching should not be a separate workflow that competes with real modeling. It should be part of the same editing flow, with clear visualization of what is committed, what is modified, and what is not yet represented as valid SysML.

The editor should continuously compare the working model against the committed baseline:

```text
Committed baseline
 → HEAD or selected branch/revision

Working model
 → current files + unsaved visual/text edits
```

Visual states:

```text
Unchanged committed element
 → normal solid style

Modified model element
 → highlighted border or change badge

Added model element
 → green/new badge

Deleted model element
 → ghosted red marker in diff/impact views

Sketch/unresolved item
 → dashed neutral style until converted or discarded
```

This keeps PowerPoint-like speed while making model status explicit.

Example flow:

```text
Drag quick block onto canvas
 → name it
 → choose or infer SysML type
 → validate required properties
 → write to working model
 → show as "added compared to HEAD"
 → commit when ready
```

The important distinction is not "sketch mode vs model mode." The important distinction is:

```text
committed model
working model changes
unresolved visual placeholders
```

The commit screen should summarize semantic model changes before writing a Git commit:

```text
Added:
+ BatteryThermalMonitor : Part

Modified:
~ BatteryController
  - added port: coolantIn
  - added satisfies relationship to REQ-221

Unresolved:
? CoolingStrategy sketch block has no SysML type yet
```

Do not allow unresolved placeholders to be committed silently. The user should either convert them to SysML elements, discard them, or intentionally store them as local-only view annotations.

## 4. Make abstraction levels explicit

Large systems fail visually when everything is shown at once.

Every view should have an abstraction selector:

```text
Level 0: System
Level 1: Subsystems
Level 2: Components
Level 3: Parts
Level 4: Features/ports
```

## 5. Make traceability a first-class part of the model hierarchy

Traceability should appear in:

* Node badges
* Inspector
* Hover cards
* Impact view
* Matrix view
* Commit summary

Traceability should not be treated as a separate report generated after modeling. It should be indexed as part of the model graph and shown alongside containment.

The model browser should support two complementary hierarchies:

```text
Containment hierarchy
 → packages, parts, ports, requirements

Traceability hierarchy
 → satisfy, verify, refine, allocate, trace, dependency chains
```

Example:

```text
Battery System
 ├─ Structure
 │   ├─ BatteryPack
 │   ├─ BatteryController
 │   └─ CoolingInterface
 └─ Traceability
     ├─ Satisfies
     │   ├─ REQ-042 Thermal Safety
     │   └─ REQ-119 Voltage Accuracy
     ├─ Verified by
     │   └─ TestCase-044 Sensor Calibration
     └─ Allocated to
         └─ ECU-2
```

In the internal model graph, traceability relationships should be first-class edges with source ranges, ownership, and change status:

```text
TraceEdge:
 ├─ id
 ├─ sourceElementId
 ├─ targetElementId
 ├─ relationshipType
 ├─ sourceFile
 ├─ sourceRange
 ├─ owningPackage
 ├─ branch
 └─ gitChangeStatus
```

Stable identity matters. Do not rely only on display names, because names change and multiple elements can share local names.

Recommended identity approach:

* Create a stable id as a uuid for each node, this should never be renamed
* Persist view/layout references using stable IDs, not canvas labels.

Matrix and impact views should be built from the same graph index:

```text
Trace matrix
 → rows: requirements
 → columns: parts/tests/functions
 → cells: direct or indirect trace edges

Impact analysis
 → start from changed nodes and changed trace edges
 → traverse configured relationship types
 → show affected requirements, tests, allocations, and views
```

This also helps custom views/lenses. A view should reference stable model element IDs and query rules, not just coordinates. If an element is renamed, the view can still recover its layout and trace context.

---

# 10. Suggested feature that could make the app special

## “Architecture Lens”

A custom view could be called a **Lens**.

Examples:

```text
Safety Lens
 → show requirements, hazards, safety components, verification cases

Electrical Lens
 → show power interfaces, electrical components, ports

Software Allocation Lens
 → show functions, software components, ECUs

Supplier Lens
 → show only components owned by supplier X

Change Impact Lens
 → show everything affected by selected changes
```

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

---

# 11. Recommended initial UI screens

## Start screen

```text
sysml2_editor

[Open Git Repository]
[Clone Repository]
[Recent Projects]

Recent:
- vehicle-platform
- battery-system
- aircraft-demo
```

## Project screen

```text
Repo: vehicle-platform
Branch: main
Status: 4 modified files

Left:
- Repositories
- Branches
- Type Palette
- Model Tree of all loaded models

Center:
- Visual views
- Text views
- Diff views

Right:
- Inspector
- Traceability
- Validation
```

## Commit screen

```text
Model Changes

Added:
+ BatteryThermalMonitor : Part

Modified:
~ BatteryController
  - added port: coolantIn
  - satisfies REQ-221

Removed:
- LegacyFuseBox

[Commit Message]
[Commit]
```

This is much better than exposing only raw file diffs.

---

# 12. What I would build first

My recommended first build target:

> A Git-native SysML v2 structure editor where you can open a repo, see the hierarchy, drag parts into a canvas, connect them, edit text side-by-side, and commit the result.

That gives you the essence of the product without getting trapped in full SysML coverage too early.

The first prototype should have four panels:

```text
Model Tree | Visual Canvas | Text Editor | Inspector
```

And support only:

```text
Package
Part
Port
Connection
Requirement
Satisfy relationship
```

Once that feels excellent, expand.

---

# 13. One-sentence product definition

**sysml2_editor is a Git-native SysML v2 architecture editor that lets engineers model systems visually with PowerPoint-level ease while preserving textual precision, traceability, validation, and version control.**

[1]: https://sysml.org/sysml-v2/faq/?utm_source=chatgpt.com "SysML v2 FAQ: What is SysML v2? Who created SysML v2?"
[2]: https://github.com/Systems-Modeling/SysML-v2-Pilot-Implementation?utm_source=chatgpt.com "Systems-Modeling/SysML-v2-Pilot-Implementation"
[3]: https://resources.sysgit.io/sysgit-saturday-1-a-purpose-built-collaborative-sysml-v2-ide/?utm_source=chatgpt.com "A Purpose-Built Collaborative SysML v2 IDE"
[4]: https://github.com/MontiCore/sysmlv2?utm_source=chatgpt.com "MontiCore implementation of the SysML v2 textual notation"

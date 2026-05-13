# Implementation Plan for `sysml2_editor`

## Technology Suggestion

The app should run on both Windows and Linux. That requirement should influence the stack early, especially around desktop packaging, file watching, Git process handling, and line-ending behavior.

Recommended initial stack:

- React
- TypeScript
- C# / .NET backend
- Monaco Editor
- React Flow
- Git CLI

Why this fits:

- C# experience reduces backend risk.
- .NET runs well on Windows and Linux.
- Git and filesystem operations are straightforward from C#.
- The parser/model index can be implemented as strongly typed C# domain code.
- The React UI remains flexible for graph/canvas-heavy interaction.
- Git CLI is reliable and avoids premature complexity.

Desktop packaging options:

- Phase 1: run as a local web app during development.
- Phase 2: package with Electron if quickest cross-platform delivery matters.
- Alternative: use Avalonia/.NET if a more native C# desktop shell becomes more important than web-based canvas flexibility.

Backend responsibilities:

- Repository open/clone/status operations
- SysML file discovery
- Parse/index pipeline
- Stable model graph API
- Save operations that preserve file boundaries
- Git diff/status/commit integration

Frontend responsibilities:

- Dockable workbench
- Canvas/tree/matrix/text views
- Visual editing gestures
- Inspector and validation presentation
- Change overlays and traceability navigation

Git integration should start with the `git` CLI wrapper.

SysML parsing should start with the MVP custom subset parser described in [parser-contract.md](./parser-contract.md). External parser or language-server integrations can come later if the product needs broader language coverage.
Use [spec-reference.md](./spec-reference.md) as the external language reference and [syntax-examples.md](./syntax-examples.md) as the local implementation subset.

## Implementation Inputs Required

Before a feature slice starts, keep these decision docs current:

- Structure: [project-structure.md](./project-structure.md)
- SysML reference: [spec-reference.md](./spec-reference.md)
- Syntax examples: [syntax-examples.md](./syntax-examples.md)
- Runtime: [runtime-decision.md](./runtime-decision.md)
- API contract: [api-contract.md](./api-contract.md)
- Parser contract: [parser-contract.md](./parser-contract.md)
- Model schema: [model-schema.md](./model-schema.md)
- Write policy: [write-policy.md](./write-policy.md)
- Starter test matrix: [starter-test-matrix.md](./starter-test-matrix.md)

[starter-test-matrix.md](./starter-test-matrix.md) is the canonical readiness and gate document. If any required decision for the active slice is still open, implementation should stop at scaffolding and discovery.

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

### First Supported Model Elements

Start with:

- Package
- Part definition
- Part usage
- Port
- Connection
- Requirement
- Satisfy / trace relationship

Avoid starting with full behavioral modeling. Structure + requirements + traceability gives the strongest early value.

## Development Phases

### Phase 1: Read-only Model Browser

Build confidence in repo parsing and navigation.

Features:

- Open Git repo
- Parse SysML files
- Show tree hierarchy
- Show text editor
- Show graph view
- Click graph node -> show source text and attributes
- Basic search

Success criterion:

> A user can open an existing SysML repo and understand the product structure visually.

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
- Commit panel
- Visual diff
- Branch comparison
- Changed-node highlighting
- Local changes overlay
- Basic merge conflict assistance

Success criterion:

> A user can compare two architecture alternatives on different branches without reading raw diffs.

### Phase 4: Custom Views and Traceability

Add the features that make it better than PowerPoint.

Features:

- Saved views
- Shared repo views
- Local private views
- Trace matrix
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

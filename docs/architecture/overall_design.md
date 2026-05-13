# Overall design

## Vision Trace

All implementation decisions must trace back to [PRODUCT_VISION.md](../../PRODUCT_VISION.md). If a decision optimizes for implementation speed, parser scope, or runtime simplicity, the tradeoff must be explicit and must not weaken the core product promise: visual modeling backed by precise, traceable, reviewable SysML text in Git.


## Git and Branch Model

The app should treat Git as a visible modeling concept, not just a hidden backend.

Key Git features:

- Open local repo, clone repo, checkout branch
- Show multiple branches and repos simultaneously
- Edit multiple branches simultaneously when backed by separate safe worktrees or repository contexts
- Diff branches visually and compare model elements between branches or repos
- Commit from inside the app, pull/push
- Show file-level and model-element-level changes
- Resolve merge conflicts at the model level where possible

## Data / Model Architecture

Recommended architecture:

```text
Git repo -> Textual SysML files -> Parser service -> Internal model graph -> View model -> Canvas / tree / table / editor UI
```

The source of truth should be the textual SysML files in Git. Do not make the canvas the model — the canvas is just one projection.

For multi-context work, each model graph belongs to an explicit context:

```text
Workspace -> Repository context -> Branch/worktree context -> Model graph + Source files + Trace links
```

Views may combine contexts through a multi-context projection, but edits must always target exactly one writable context.

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

### 6. Make context explicit
Every visible model item, source file, trace link, diff, save action, and commit action should make repository and branch context clear.


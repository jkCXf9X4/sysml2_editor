# Architecture

This folder contains high-level system decisions and their rationale. Architecture documents answer *what* the system guarantees and *why*, avoiding implementation-level detail unless needed to explain a tradeoff.

## Key High-Level Decisions

1. **Local Web App Runtime**: First implementation slice uses React frontend + ASP.NET Core backend on `localhost` (no Electron wrapper yet). Details in [runtime.md](./runtime.md).
2. **Stable Model Graph**: System uses a context-aware graph schema with nodes, edges, files, and trace links. All UI projections derive from this single graph. Details in [model-graph.md](./model-graph.md).
3. **Deterministic Write Policy**: Edits target exactly one writable context, preserve file ownership, and never rewrite unrelated content. Details in [write-policy.md](./write-policy.md).

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

## Vision Trace

All implementation decisions must trace back to [PRODUCT_VISION.md](../../PRODUCT_VISION.md). If a decision optimizes for implementation speed, parser scope, or runtime simplicity, the tradeoff must be explicit and must not weaken the core product promise: visual modeling backed by precise, traceable, reviewable SysML text in Git.

## Documents

- [Runtime architecture](./runtime.md)
- [Model graph architecture](./model-graph.md)
- [Write policy](./write-policy.md)

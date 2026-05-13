# Model Graph

## Decision

Use a stable graph schema with explicit context, node, edge, file, trace-link, and source-range concepts. The UI, diff engine, inspector, and traceability features all read from the same graph.

Vision trace:

- Supports: every model element remains precise, traceable, reviewable, and tied back to source text; item, file, branch, repository, and workspace-context traceability can be projected from one shared model.
- Tradeoff: the schema carries explicit source and lifecycle metadata from the start, even when early UI slices only consume part of it.

## Core Entities

### Model Context

Each model graph belongs to one context. A context identifies the repository, branch or worktree, commit state, and whether the graph may be edited.

Rules:

- Multiple contexts may reference the same repository if they represent distinct branches, commits, or worktrees.
- Multiple contexts may reference different repositories in the same workspace.
- Save and commit operations must target exactly one writable context.
- Read-only contexts still support graph browsing, source viewing, traceability, and diff overlays.

### Node

Each node represents one semantic model element with a stable ID, kind, name, source location, and lifecycle state.

### Edge

Each edge represents a semantic relationship between two nodes, with a stable ID, kind, source and target references, source location, and lifecycle state.

Edge IDs are derived deterministically from source context for relationships that lack source-level identity.

### Source Range

Source ranges are 1-based and inclusive at the start, exclusive at the end.

### Trace Link

Trace links are derived navigation facts that make product traceability explicit for UI, tests, and future APIs. They do not replace semantic model edges; they give a stable cross-level trace view over model items, files, branches, and repositories.

Trace links are read-only derived data. Edits update the model graph or files; trace links are recomputed.

### Multi-Context View

`ModelGraphDto` always represents exactly one workspace context. Combined branch, worktree, or repository views use a separate projection so context identity cannot be lost.

A combined view must never merge nodes, files, or trace links into a context-free collection.

## Workspace Contexts

Workspace context is the unit of safe viewing and editing.

The first implementation slice may open one context. Later slices must be able to hold several contexts at once:

- Same repository, different branches, read-only comparison contexts.
- Same repository, different worktrees, independently writable contexts.
- Different repositories, independently writable contexts.
- Related repositories used as dependencies or supplier/library models.

Combined views must not collapse IDs across contexts. When a view contains data from multiple contexts, the API must return a multi-context projection and UI must include context identity so duplicate stable IDs from different branches or repositories are not ambiguous.

## Derived Views

All UI projections derive from the same graph:

- Tree view from containment edges
- Graph view from semantic edges
- Inspector from node attributes and incident edges
- Trace matrix from traceability edges
- Impact analysis from changed nodes and dependent edges
- Source ownership view from nodes, files, imports, and source ranges
- Cross-repository dependency view from imported libraries and related repositories

## Detailed Specs

Field-level requirements, C# shapes, TypeScript types, ID derivation rules, and serialization conventions are defined in [model-graph.md](../implementation/graph/model-graph.md).

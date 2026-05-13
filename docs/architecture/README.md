# Architecture

This folder contains high-level system decisions and their rationale. Architecture documents answer *what* the system guarantees and *why*, avoiding implementation-level detail unless needed to explain a tradeoff.

## Overall design 

- [Overall design](./overall_design.md)

## Key High-Level Decisions

1. **Local Web App Runtime**: First implementation slice uses React frontend + ASP.NET Core backend on `localhost` (no Electron wrapper yet). 
Details in [runtime.md](./runtime.md).

2. **Stable Model Graph**: System uses a context-aware graph schema with nodes, edges, files, and trace links. All UI projections derive from this single graph. 
Details in [model-graph.md](./model-graph.md) (architecture overview) and [model-graph.md](../implementation/graph/model-graph.md) (field-level specs, C# shapes, ID rules).

3. **Deterministic Write Policy**: Edits target exactly one writable context, preserve file ownership, and never rewrite unrelated content. 
Details in [write-policy.md](./write-policy.md).

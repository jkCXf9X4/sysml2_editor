# Test Strategy for `sysml2_editor`

## Purpose

This document defines how to test `sysml2_editor` as a Git-native SysML v2 workbench.

The plan makes three things non-negotiable:

1. Textual SysML is the source of truth.
2. Visual editing must round-trip to stable, reviewable text.
3. Git behavior must stay predictable across branches, files, and operating systems.

## Scope

The strategy covers the product areas described in [docs/README.md](../README.md):

- Repository open/clone/status/checkout flows
- SysML file discovery, parsing, indexing, and validation
- Hierarchical browsing, graph views, inspector data, and search
- Visual editing of the MVP subset
- Text editing and round-trip persistence
- Git-native workflows such as diff, branch compare, commit, and conflict handling
- Saved views, layouts, and traceability artifacts
- Cross-platform behavior on Windows and Linux

Out of scope for the first iteration:

- Full SysML v2 language coverage
- Advanced behavioral modeling
- Deep merge automation beyond safe assistance and clear conflict surfacing
- Large-scale performance claims before the parser and model graph are stable

## Test Principles

### 1. Test semantics before pixels

The highest-value assertions are about model meaning:

- Does the model graph contain the right elements and relationships?
- Does editing preserve identity, ownership, and references?
- Does the saved SysML text match the intended model change?

Visual tests matter, but they are secondary to semantic correctness.

### 2. Protect round-trips

Any supported edit must survive this loop:

`Git repo -> parse -> model graph -> UI edit -> text write -> parse again`

The second parse should reconstruct the same supported semantics.

### 3. Preserve stable identity

Views, layouts, and trace links should reference stable IDs, not display names.

Tests should verify that rename and move operations do not break:

- Node selection
- Layout recovery
- Trace overlays
- Cross-file references

### 4. Keep Git diffs intentional

The editor should not rewrite unrelated content. Tests must catch:

- Whitespace churn
- Line-ending drift
- File reordering
- Spurious import changes
- Unnecessary formatting noise

### 5. Fail safely

If parsing or editing cannot be completed safely, the app should surface an error instead of silently corrupting the model.

## Test Layers

### Unit tests

Use unit tests for fast, deterministic coverage of pure logic:

- SysML parsing for the supported subset
- Model graph construction and normalization
- Relationship validation rules
- Stable ID generation and lookup
- View JSON serialization/deserialization
- Diff computation for model elements
- Git command wrapper behavior at the boundary level

### Component tests

Use component tests for isolated UI and service behavior:

- Type palette filtering and grouping
- Inspector rendering for selected nodes and multi-select
- Tree navigation and breadcrumb updates
- Canvas item creation, connection, selection, and deletion
- View configuration editing
- Validation badges and status presentation

### Integration tests

Use integration tests to verify real interaction between modules:

- Open a local repository and build the model index
- Read and write supported SysML files
- Switch branches and refresh the model graph
- Compute semantic diffs between revisions
- Persist a visual edit and reparse the result
- Handle file watcher events and refresh stale data

### End-to-end tests

Use a small number of end-to-end tests for the full user journey:

- Open repo -> browse tree -> inspect node -> edit -> save -> commit
- Open two branches -> compare architectures -> review changed nodes
- Add a new supported element visually -> confirm the written SysML is valid
- Create a saved view -> reopen it -> confirm stable layout and filters

### Visual regression tests

Use snapshot or image-based tests only for rendering-specific behavior:

- Canvas layout stability
- Node/edge styling for status states
- Inspector and diff panel presentation
- Empty states and error states

Do not rely on visual snapshots for core model correctness.

### Performance tests

Add performance coverage for workflows that can degrade on real repositories:

- Opening large repos
- Building the model graph
- Search and trace queries
- Layout generation
- Branch diff generation
- File save and refresh latency

## Related Documents

| Document | Content |
| --- | --- |
| [MVP coverage](./mvp-coverage.md) | Supported elements, round-trip cases, negative cases |
| [Test roadmap](../roadmap/test_roadmap.md) | Five-phase rollout with test focus and exit criteria |
| [Test fixtures](./fixtures.md) | Fixture catalog and rules |
| [Automation and CI](./automation-ci.md) | Per-commit, PR, and nightly gates |
| [Quality and risk](./quality-and-risk.md) | Cross-platform, non-functional, risk priorities, exit criteria |
| [Starter test matrix](./starter-test-matrix.md) | Minimum gated test set for initial implementation slices |

## Summary

The most important test discipline for `sysml2_editor` is to keep the text model, the internal graph, and the UI synchronized without losing Git fidelity.

If a test does not protect round-trip semantics, stable identity, or reviewable diffs, it is probably not worth prioritizing early.

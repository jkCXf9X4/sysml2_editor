# Product Vision

`sysml2_editor` is a Git-native SysML v2 architecture workbench that lets engineers model systems visually with PowerPoint-level ease while preserving textual precision, traceability, validation, and version control.

## Vision Pillars

- Visual editing should make architecture modeling approachable without turning the diagram into the source of truth.
- Textual SysML files in Git are the durable source of truth for model semantics.
- Every model element should remain precise, traceable, reviewable, and tied back to source text.
- Traceability should be visible across model items, source files, branches, and related repositories so users can follow why something exists, where it lives, and what it affects.
- Git concepts such as branches, diffs, commits, and review should be first class citizens of the modeling workflow.
- Custom views should be projections of the model, not separate competing models.

## Product Promise

The product should feel like a practical engineering workbench:

- Simple enough to create and edit architecture visually.
- Strict enough to protect SysML semantics, stable identity, and source ranges.
- Reviewable enough that generated changes make sense in Git.
- Traceable enough that requirements, parts, ports, connections, files, branches, repositories, and changes can be followed through the model.

## Traceability Scope

Traceability is a product capability, not just a relationship type.

The product should help users visualize and navigate:

- Item-to-item links, such as requirements, satisfies, traces, ports, connections, and dependencies.
- Item-to-file links, showing which source files define, reference, or would be changed by an element.
- File-to-file links, including imports, modular package boundaries, generated view files, and ownership relationships.
- Branch-to-branch links, showing how model elements and files changed between alternatives.
- Repo-to-repo links, when a system model depends on libraries, supplier models, shared views, or related engineering repositories.

Early slices should prove item-to-item and item-to-file traceability first. Repo-to-repo traceability may be deferred, but decisions should preserve a path to it.

## Design Decision Trace Rule

Every material product, architecture, implementation, or UX decision must trace back to this vision.

Use this format in decision documents:

```text
Vision trace:
- Supports: <one or more vision pillars>
- Tradeoff: <what this decision intentionally optimizes or defers>
```

If a decision does not support a vision pillar, the decision should either be changed or explicitly documented as a temporary implementation constraint.

## Current Strategic Choice

The first implementation slice prioritizes a read-only Git-backed model browser before visual editing. This does not change the product vision; it reduces delivery risk by proving repository access, parsing, source mapping, and model projection before writes are exposed.

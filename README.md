# sysml2_editor

Git-native SysML v2 viewer and editor.

## Product Vision

`sysml2_editor` is a Git-native SysML v2 architecture workbench that lets engineers model systems visually with PowerPoint-level ease while preserving textual precision, traceability, validation, and version control.

Traceability includes links between model items, source files, branches, and related repositories.
The workbench is intended to support multiple branches of the same repository and multiple repositories open side by side, with explicit context on every view and edit.

Product commitments, system architecture, and technical decisions should trace back to the product vision.

## Repository Layout

```text
sysml2_editor/
  README.md
  PRODUCT_VISION.md
  LICENSE
  docs/                     -- documentation index
  src/
    backend/                -- ASP.NET Core backend
    frontend/               -- React frontend
    shared/                 -- shared contracts, generated API clients
  tests/
    unit/                   -- parser, graph, write-policy unit tests
    integration/            -- backend + filesystem + Git integration tests
    e2e/                    -- full-stack user workflow tests
  fixtures/                 -- checked-in test repos and models
  scripts/                  -- dev and CI utilities
```

# Parser Contract

## Decision

The first implementation slice uses a custom, MVP-scoped parser for a narrow textual SysML subset.

It is not a full SysML v2 grammar implementation. It is a conservative parser whose job is to load, edit, and save the subset that the app supports first.

Use [spec-reference.md](./spec-reference.md) for the external language source and [syntax-examples.md](./syntax-examples.md) for the exact MVP syntax accepted by this project.

## Supported MVP Subset

The parser must recognize the following semantic constructs:

- Package declarations
- Part definitions
- Part usages
- Port declarations and usages
- Connection declarations/usages
- Requirement declarations/usages
- Satisfy and trace relationships
- Imports and file-level references
- Editor-owned stable ID comment markers immediately above element definitions

The accepted concrete forms are defined by [syntax-examples.md](./syntax-examples.md).

## Unsupported Content

Unsupported syntax is allowed in a file only if it can be preserved as opaque text.

Rules:

- Unsupported text is loaded as an opaque span with source range metadata.
- Unsupported spans are read-only until the parser contract expands.
- If an edit would damage an opaque span, save must be blocked and the user must see a diagnostic.

## Parser Outputs

The parser must produce:

- A model graph for supported constructs
- Source ranges for every structured element
- Diagnostics for malformed or unsupported sections
- Raw text spans for content the parser does not understand structurally

## Round-Trip Invariants

Supported content must satisfy:

1. Parse to model graph.
2. Write back to text.
3. Parse again.
4. Reconstruct the same supported semantics.

Additional invariants:

- Stable IDs survive rename and move operations.
- Formatting changes must not create semantic changes.
- The parser must not reorder unrelated declarations.
- The parser must preserve file boundaries and imports.
- Stable ID markers must be preserved when a supported element is rewritten.

## Error Handling

- Parse errors must include file, line, and column.
- One broken file must not prevent other valid files from loading.
- A file with unsupported constructs may open in read-only mode if the opaque spans can be preserved safely.

## Implementation Boundary

The parser contract should be exposed as a small interface that can later be replaced by a richer grammar or language-server-backed parser without changing the UI model.

# Parser Contract

## Decision

The first implementation slice uses a custom, MVP-scoped parser for a narrow textual SysML subset.

It is not a full SysML v2 grammar implementation. It is a conservative parser whose job is to load, edit, and save the subset that the app supports first.

Use [spec-reference.md](./spec-reference.md) for the external language source and [syntax-examples.md](./syntax-examples.md) for the exact MVP syntax accepted by this project.

Vision trace:

- Supports: textual SysML in Git as the durable source of truth; model elements tied back to source ranges; safe visual editing through parseable semantics.
- Tradeoff: accepts a narrow MVP parser before full SysML v2 coverage so implementation can prove source mapping and round-trip behavior early.

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
- SysML-native `@Sysml2EditorIdentity` metadata annotations immediately above editable element definitions

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
- `OpaqueSpanDto` records for content the parser does not understand structurally

## Identity Rules

- Supported semantic elements with `@Sysml2EditorIdentity` metadata use its `id` attribute as `stableId`.
- Packages are supported semantic elements and must have identity metadata to be editable.
- In the read-only first slice, elements missing identity metadata may be loaded with deterministic derived IDs and a `MissingStableId` warning diagnostic.
- Missing-identity elements are read-only until a later explicit backfill operation adds persisted identity metadata.
- Relationship edges use deterministic derived IDs unless the source text later introduces explicit relationship identity.

## Import Ownership

Imports are file-level model items in the MVP parser.

Rules:

- Each import produces an `Import` node.
- Each import node uses the import declaration source range.
- Each import node's `owningPackageId` is `null`.
- The parser creates an `Imports` edge from the file-level import node.
- If the imported target resolves to a model node, `targetId` is that node ID.
- If the imported target is unresolved, `targetId` is `null` and `attributes["unresolvedTarget"]` contains the imported qualified name.

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
- Stable identity metadata must be preserved when a supported element is rewritten.
- In the first read-only slice, no writer exists and round-trip invariants are verified only once writer support begins.

## Error Handling

- Parse errors must include file, line, and column.
- One broken file must not prevent other valid files from loading.
- A file with unsupported constructs may open in read-only mode if the opaque spans can be preserved safely.

## Implementation Boundary

The parser contract should be exposed as a small interface that can later be replaced by a richer grammar or language-server-backed parser without changing the UI model.

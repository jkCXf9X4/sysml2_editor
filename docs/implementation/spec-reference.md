# SysML v2 Reference Source

This project treats the OMG SysML v2 specification as the authoritative language reference.

## Primary Sources

- SysML v2 overview: <https://www.omg.org/sysml/sysmlv2/>
- SysML specification page: <https://www.omg.org/spec/SysML>
- SysML 2.0 formal version: <https://www.omg.org/spec/SysML/2.0>

As checked on 2026-05-13, the OMG specification page identifies SysML 2.0 as a formal specification with publication date September 2025. The same page links normative language and transformation PDFs, machine-readable abstract syntax artifacts, JSON schema, and an informative textual notation example.

## How This Project Uses The Spec

The first implementation does not attempt full SysML v2 conformance.

Instead:

- Use the OMG SysML v2 specification as the origin for terminology and examples.
- Implement only the MVP subset documented in [parser-contract.md](./parser-contract.md).
- Keep fixture syntax close to SysML v2 textual notation where practical.
- Mark deliberate simplifications in [syntax-examples.md](./syntax-examples.md).

Vision trace:

- Supports: textual SysML files in Git as the durable source of truth; precise semantics grounded in the external SysML language source.
- Tradeoff: local MVP examples intentionally limit coverage while preserving a path back to formal SysML v2.

## Identity Metadata Decision

Stable model identity should be represented with SysML-native metadata annotations, not tool-owned comments.

The initial project metadata is:

```sysml
metadata def Sysml2EditorIdentity {
  attribute id : String;
}
```

The MVP parser recognizes `@Sysml2EditorIdentity { id = "..."; }` immediately above editable element definitions. This keeps identity in the model text using the SysML metadata mechanism while leaving room to migrate if a standard SysML identity metadata definition is adopted later.

Vision trace:

- Supports: stable identity, traceability, reviewability, and source-text ownership without hiding model identity in tool-owned side files.
- Tradeoff: uses project-owned metadata until a standard identity convention is adopted.

## Conformance Rule

If a local parser decision conflicts with the OMG specification, document the difference in [syntax-examples.md](./syntax-examples.md) before implementation continues.

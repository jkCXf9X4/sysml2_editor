# Stable IDs

Stable IDs for supported semantic elements are persisted in the source text using a SysML-native metadata annotation immediately above the element definition.

Example:

```sysml
@Sysml2EditorIdentity { id = "3c4c3f6a-5d49-4ec7-8d5f-0d792df0a8f1"; }
```

## Rules

- Every editable supported semantic element, including packages, must have `@Sysml2EditorIdentity` metadata.
- The identity value is the metadata attribute `id`.
- IDs are generated once when the element is created.
- IDs do not change on rename.
- IDs do not change when a node moves within the same owning file.
- In the read-only Phase 1 browser, elements without identity metadata may be loaded with deterministic derived IDs, but they are marked read-only with a diagnostic.
- Writer support must not silently save a file with missing identity metadata. A later explicit backfill operation may add metadata annotations and then enable editing.

## Derived Read-Only IDs

Derived read-only IDs use a deterministic UUID v5-style hash of repository-relative source file, node kind, qualified name, and source range. They are process-stable but not a substitute for persisted IDs.

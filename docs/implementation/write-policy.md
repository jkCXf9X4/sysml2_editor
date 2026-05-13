# Write Policy

## Decision

The editor should use deterministic file ownership rules so that a save changes only the intended files.

The write policy is part of the implementation contract, not an afterthought.

This policy is accepted for the initial design but is not implemented in the first read-only slice. No save endpoint or UI save action should exist until the writer gate in [starter-test-matrix.md](./starter-test-matrix.md) is active.

Vision trace:

- Supports: textual SysML files in Git as the durable source of truth; reviewable generated changes; visual editing that preserves identity and semantics.
- Tradeoff: save behavior is blocked until parser and writer gates prove that edits can be persisted without unrelated file churn.

## File Convention

The first implementation slice should use this repository layout:

```text
model/root.sysml
model/<package-name>.sysml
.sysml2_editor/views/*.view.json
.sysml2_editor/layouts/*.layout.json
```

## Ownership Rules

1. Each editable element has one owning file.
2. A child element is written to the same file as its owning package or container.
3. `model/root.sysml` acts as the import and project entry point.
4. View and layout changes are written only to `.sysml2_editor`.
5. Cross-file references are written as references or imports, not duplicate definitions.

## File Selection Rules

When the user creates or edits an element:

1. Use the selected element's owning file if one exists.
2. Otherwise use the current document's file.
3. Otherwise use `model/root.sysml`.
4. Create a new file only when no existing owner file is appropriate.

## Save Rules

- Preserve the existing line ending style of the target file.
- Preserve unrelated text and declaration order where possible.
- Never rewrite untouched files.
- Treat file moves as explicit operations, not hidden side effects.
- If the parser cannot round-trip the file safely, do not write.
- Preserve SysML-native stable identity metadata.
- Block writes for editable elements that do not have persisted identity metadata, unless the operation is an explicit identity backfill.

## Transactional Save Flow

Save should happen in this order:

1. Build the updated in-memory model.
2. Render to a temporary buffer.
3. Reparse the buffer.
4. Compare the reparsed graph with the intended change set.
5. Write the file only if the comparison passes.

## Failure Modes

- Parse mismatch: block the save, keep the original file untouched, and return diagnostics.
- Write failure: return the filesystem error and do not update in-memory file metadata.
- Concurrent file change: compare the current content hash before writing; if it differs, block the save and require reload or conflict handling.
- Dirty working tree: allow edits to already-dirty files, but show status; never silently discard uncommitted changes.
- Line-ending mismatch: preserve the existing target file line ending and report if generated content would mix line endings.

## Backup Rule

The first implementation slice should avoid backup files and rely on Git plus transactional write checks. If a file is not tracked by Git, failed writes must still leave the original content untouched.

## Naming Rules

- New package files should use lower-kebab-case by default.
- File names should be stable unless the user explicitly renames or moves the file.
- Package rename does not automatically rename the file unless the user chooses that action.

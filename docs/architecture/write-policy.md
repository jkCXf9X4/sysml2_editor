# Write Policy

## Decision

The editor should use deterministic file ownership rules so that a save changes only the intended files.

The write policy is part of the implementation contract, not an afterthought.

This policy is accepted for the initial design but is not implemented in the first read-only slice. No save endpoint or UI save action should exist until the Phase 2 writer gates in [Implementation roadmap](../roadmap/roadmap.md) are active.

Vision trace:

- Supports: textual SysML files in Git as the durable source of truth; reviewable generated changes; visual editing that preserves identity, semantics, repository context, and branch context.
- Tradeoff: save behavior is blocked until parser and writer gates prove that edits can be persisted without unrelated file churn.

## File Convention

The initial writable model layout should use this repository shape:

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

- Every save targets exactly one `workspaceId`.
- The UI must show the target repository, branch, and file before save.
- The backend must reject writes to read-only comparison contexts.
- Two branches of the same repository may both be writable only when backed by distinct safe worktrees or repository roots.
- Preserve the existing line ending style of the target file.
- Preserve unrelated text and declaration order where possible.
- Never rewrite untouched files.
- Treat file moves as explicit operations, not hidden side effects.
- If the parser cannot round-trip the file safely, do not write.
- Preserve SysML-native stable identity metadata.
- Block writes for editable elements that do not have persisted identity metadata, unless the operation is an explicit identity backfill.

## Worktree And Context Rules

The backend owns workspace-context validation. The frontend may request contexts and display their state, but it must not infer write safety from paths or branch labels.

Initial design:

1. Opening an existing repository root creates one workspace context for the currently checked-out branch.
2. Opening an existing Git worktree creates a separate workspace context, even when it belongs to a repository that is already open.
3. Creating a new worktree is an explicit backend operation requested by the user. It must never happen as a hidden side effect of opening a comparison.
4. A same-repository branch comparison may use read-only contexts from Git data, but editing both branches requires two distinct writable roots.
5. The backend marks a context read-only when the root path is not writable, the branch is detached, the branch is already open in another writable root without a distinct worktree, or the context was created only for comparison.
6. Closing a context does not delete a worktree. Worktree deletion is a separate explicit operation.

Recommended context operations for the first editable multi-branch roadmap phase:

- `POST /repositories/open`: open an existing repository or worktree path.
- `POST /workspace-contexts/worktrees`: create a new worktree for a branch and open it as a workspace context.
- `GET /workspace-contexts`: list current contexts and writable state.
- `DELETE /workspace-contexts/{workspaceId}`: close the in-memory context without deleting files.

Worktree creation request shape:

```json
{
  "repositoryId": "local-vehicle-demo",
  "branch": "experiment",
  "path": "/absolute/path/to/repo-experiment-worktree",
  "createBranch": false
}
```

Worktree creation response shape:

```json
{
  "workspaceId": "workspace-experiment",
  "repositoryId": "local-vehicle-demo",
  "rootPath": "/absolute/path/to/repo-experiment-worktree",
  "branch": "experiment",
  "isWritable": true,
  "writableReason": "Backed by a distinct writable worktree."
}
```

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

The initial writer should avoid backup files and rely on Git plus transactional write checks. If a file is not tracked by Git, failed writes must still leave the original content untouched.

## Naming Rules

- New package files should use lower-kebab-case by default.
- File names should be stable unless the user explicitly renames or moves the file.
- Package rename does not automatically rename the file unless the user chooses that action.

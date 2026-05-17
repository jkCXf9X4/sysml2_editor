# Use Cases

## Governing Product Commitments

- [PCOM-001: Git-native source of truth](../product-commitments/PCOM-001-git-native-source-of-truth.md)
- [PCOM-002: Visual editing as model projection](../product-commitments/PCOM-002-visual-editing-as-model-projection.md)
- [PCOM-003: Multi-context workspace](../product-commitments/PCOM-003-multi-context-workspace.md)
- [PCOM-004: Traceability-first modeling](../product-commitments/PCOM-004-traceability-first-modeling.md)

These use cases describe the product behavior the system architecture must support. They do not define implementation order.

## Workspace, Browsing, And Traceability

### UC-01 Open A Repository Workspace

- Actor: systems engineer or model reviewer.
- Goal: open a local Git repository and see an explicit repository, branch, and write-state context.
- System outcome: the workspace remains addressable by repository, branch, and write state even when no supported SysML files are present.
- Governing commitments: PCOM-001, PCOM-003.

### UC-02 Browse And Inspect Model Structure

- Actor: systems engineer.
- Goal: move through tree, graph, source, and inspector views without losing selection or context.
- System outcome: selecting an element updates related views and exposes source ownership, relationships, diagnostics, and context identity.
- Governing commitments: PCOM-002, PCOM-004.

### UC-03 Trace From Element To Source And Ownership

- Actor: systems engineer, reviewer, or integrator.
- Goal: understand where an element lives, what file owns it, and what it affects.
- System outcome: trace and ownership facts are visible without replacing the selected model context.
- Governing commitments: PCOM-001, PCOM-004.

### UC-04 Diagnose Invalid Or Partial Input

- Actor: systems engineer.
- Goal: understand what failed and what remains usable when parsing, validation, or write planning fails.
- System outcome: diagnostics are visible, safe parsed subsets remain browsable, and unsafe writes are blocked.
- Governing commitments: PCOM-001, PCOM-002.

## Comparison, Editing, And Git

### UC-05 Compare Branches And Repositories

- Actor: systems engineer or reviewer.
- Goal: inspect differences between contexts without collapsing them into one model.
- System outcome: side-by-side contexts preserve repository, branch, file, and write-state identity.
- Governing commitments: PCOM-003, PCOM-004.

### UC-06 Create And Modify Supported Model Elements

- Actor: systems engineer.
- Goal: add, rename, connect, or adjust supported structural elements visually without breaking model identity.
- System outcome: visual edits remain projections over source-backed model semantics.
- Governing commitments: PCOM-001, PCOM-002.

### UC-07 Save Changes Safely To Source Text

- Actor: systems engineer.
- Goal: persist a staged model change to the correct source file without collateral edits.
- System outcome: writes target one explicit writable context and preserve source ownership, stable identity, and reviewable diffs.
- Governing commitments: PCOM-001, PCOM-003.

### UC-08 Commit And Review Changes Through Git

- Actor: systems engineer, reviewer, or integrator.
- Goal: turn saved model changes into reviewable Git history.
- System outcome: Git status, semantic diffs, target repository, and target branch are visible before commit.
- Governing commitments: PCOM-001, PCOM-004.

## Multi-Context, Views, And Expansion

### UC-09 Keep Multiple Contexts Open Side By Side

- Actor: systems engineer.
- Goal: work with multiple branches, worktrees, or repositories at once.
- System outcome: every context keeps distinct identity and write state, and edits always resolve to one target context.
- Governing commitments: PCOM-003.

### UC-10 Create, Restore, And Share Saved Views

- Actor: systems engineer.
- Goal: store a useful model projection and reopen it later with stable identity.
- System outcome: saved views remain projections of the model, not separate competing models.
- Governing commitments: PCOM-002, PCOM-004.

### UC-11 Analyze Impact And Dependency Paths

- Actor: systems engineer or reviewer.
- Goal: ask what changes if a selected element changes.
- System outcome: impact views distinguish direct trace links, source ownership, branch differences, and repository dependencies.
- Governing commitments: PCOM-004.

### UC-12 Extend Supported SysML Coverage

- Actor: systems engineer or product maintainer.
- Goal: expand supported SysML constructs without weakening the source-backed modeling loop.
- System outcome: new constructs are added only when parser, graph, writer, validation, and verification coverage can preserve the same guarantees.
- Governing commitments: PCOM-001, PCOM-002, PCOM-004.

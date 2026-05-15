# Use Cases

These use cases describe the intended product purpose, not just the current implementation slice. They are the basis for later functional breakdown, product breakdown, and product requirements.

## Use Case Index

### Workspace, browsing, and traceability

- `UC-01` [Open a repository workspace](#uc-01-open-a-repository-workspace) - establish an explicit repository and branch context with visible identity. Anchors: [repository-workspace](../functionality/repository-workspace.md), [workbench-ui](../functionality/workbench-ui.md); roadmap milestone 2.
- `UC-02` [Browse and inspect model structure](#uc-02-browse-and-inspect-model-structure) - move through tree, graph, source, and inspector views with synchronized selection. Anchors: [model-browsing](../functionality/model-browsing.md), [sysml-graph-source](../functionality/sysml-graph-source.md); roadmap milestone 2.
- `UC-03` [Trace from element to source and ownership](#uc-03-trace-from-element-to-source-and-ownership) - see where an element comes from, what file owns it, and what it links to. Anchors: [traceability-analysis](../functionality/traceability-analysis.md), [model-browsing](../functionality/model-browsing.md); roadmap milestones 2 and 7.
- `UC-04` [Diagnose invalid or partial input](#uc-04-diagnose-invalid-or-partial-input) - surface parse, validation, and write failures as actionable feedback. Anchors: [model-browsing](../functionality/model-browsing.md), [editing-write-safety](../functionality/editing-write-safety.md); roadmap milestones 2 and 3.

### Comparison, editing, and Git

- `UC-05` [Compare branches and repositories](#uc-05-compare-branches-and-repositories) - show side-by-side contexts and semantic differences without losing the current workspace. Anchors: [git-workflow](../functionality/git-workflow.md), [repository-workspace](../functionality/repository-workspace.md); roadmap milestones 4 and 5.
- `UC-06` [Create and modify supported model elements](#uc-06-create-and-modify-supported-model-elements) - add, rename, connect, and stage supported structural elements visually. Anchors: [editing-write-safety](../functionality/editing-write-safety.md); roadmap milestone 3.
- `UC-07` [Save changes safely to source text](#uc-07-save-changes-safely-to-source-text) - write back to the owning file without corrupting unrelated content or identity. Anchors: [editing-write-safety](../functionality/editing-write-safety.md), [sysml-graph-source](../functionality/sysml-graph-source.md); roadmap milestone 3.
- `UC-08` [Commit and review changes through Git](#uc-08-commit-and-review-changes-through-git) - make changes visible as reviewable diffs and commit them from the app. Anchors: [git-workflow](../functionality/git-workflow.md); roadmap milestone 4.

### Multi-context, views, and expansion

- `UC-09` [Keep multiple contexts open side by side](#uc-09-keep-multiple-contexts-open-side-by-side) - work with multiple branches or repositories at once with explicit write targets. Anchors: [repository-workspace](../functionality/repository-workspace.md), [git-workflow](../functionality/git-workflow.md); roadmap milestone 5.
- `UC-10` [Create, restore, and share saved views](#uc-10-create-restore-and-share-saved-views) - save projections of the model and reopen them with stable identity. Anchors: [saved-views](../functionality/saved-views.md); roadmap milestone 6.
- `UC-11` [Analyze impact and dependency paths](#uc-11-analyze-impact-and-dependency-paths) - ask what is affected by an element across items, files, branches, and repositories. Anchors: [traceability-analysis](../functionality/traceability-analysis.md), [saved-views](../functionality/saved-views.md); roadmap milestone 7.
- `UC-12` [Extend the model with advanced SysML constructs](#uc-12-extend-the-model-with-advanced-sysml-constructs) - add broader SysML coverage only after the core browse/edit/Git loop is stable. Anchors: [advanced-sysml](../functionality/advanced-sysml.md); roadmap milestone 8.

## UC-01 Open A Repository Workspace

- Actor: systems engineer or model reviewer
- Goal: open a local Git repository and see an explicit workspace context
- Trigger: the user selects a repository or opens the app in an existing working tree
- Preconditions: the repository exists locally and may or may not contain supported SysML files
- Main flow:
  1. The system creates a repository context with visible repository and branch identity.
  2. It discovers supported SysML files and attempts to parse them.
  3. It builds the model graph and source map for the workspace.
  4. It presents the first usable browser view without hiding the context.
- Alternate flows:
  - If the repository has no supported model files, the workspace still opens in a valid empty state.
  - If parsing fails, the user sees diagnostics but the repository context remains available.
- Postconditions: the workspace is addressable by repository, branch, and write state.
- Vision trace:
  - Supports: textual SysML as the durable source of truth, Git as a first-class modeling concept, explicit context
- Derived functional blocks:
  - FB-01 Workspace and context management
  - FB-02 Model ingestion and indexing

## UC-02 Browse And Inspect Model Structure

- Actor: systems engineer
- Goal: move through the model and inspect the selected element without losing context
- Trigger: the user opens a workspace or selects a node in a tree, graph, or diff view
- Preconditions: a workspace context exists and the model graph has been built or partially built
- Main flow:
  1. The system shows the tree, graph, source, and inspector as synchronized views of the same model.
  2. Selecting an element updates the source pane, inspector, and trace context.
  3. Search and filtering narrow the visible model without changing the underlying source.
  4. Pane labels continue to show repository, branch, file, and mode.
- Alternate flows:
  - If only part of the model parses, the browsable subset still works.
  - If the selection refers to an unresolved target, the inspector reports the issue instead of hiding the item.
- Postconditions: the user can identify the selected element, its source file, and its relationships.
- Vision trace:
  - Supports: precise visual modeling, traceability, context awareness
- Derived functional blocks:
  - FB-03 Browsing and selection sync
  - FB-04 Traceability projection
  - FB-11 Diagnostics and recovery

## UC-03 Trace From Element To Source And Ownership

- Actor: systems engineer, reviewer, or integrator
- Goal: see where an element lives, what file owns it, and what it affects
- Trigger: the user opens trace or ownership details for a selected element
- Preconditions: the model graph contains source ranges and trace metadata for the element
- Main flow:
  1. The system shows the owning source file, source range, and trace links for the selected element.
  2. It exposes incoming and outgoing relationships where available.
  3. It highlights related files, branches, or derived views when the data exists.
  4. It keeps the selected element visible while trace details are expanded.
- Alternate flows:
  - If trace metadata is incomplete, the system shows the gap explicitly.
  - If a relationship target is missing, the UI reports the missing target instead of inferring one.
- Postconditions: the user can move from element to source and back without losing identity.
- Vision trace:
  - Supports: item-to-item traceability, item-to-file traceability, reviewability
- Derived functional blocks:
  - FB-04 Traceability projection
  - FB-09 Analysis and query

## UC-04 Diagnose Invalid Or Partial Input

- Actor: systems engineer
- Goal: understand what failed and what remains usable when a file or write is invalid
- Trigger: parsing, validation, or write planning fails
- Preconditions: the system attempted to parse, validate, or save supported SysML text
- Main flow:
  1. The system identifies the failure point and classifies the error.
  2. It shows diagnostics in the browser and in the relevant pane or status area.
  3. It preserves any valid parsed subset that can still be browsed safely.
  4. It prevents unsafe writes from proceeding silently.
- Alternate flows:
  - If the file is partially written, the system keeps the last safe state visible where possible.
  - If the issue is a missing identity or owner, the system explains that the write cannot proceed yet.
- Postconditions: the user can see the error, the safe subset, and the next corrective action.
- Vision trace:
  - Supports: strict semantics, reviewable text, safe writes
- Derived functional blocks:
  - FB-02 Model ingestion and indexing
  - FB-06 Write policy and persistence
  - FB-11 Diagnostics and recovery

## UC-05 Compare Branches And Repositories

- Actor: systems engineer or reviewer
- Goal: inspect differences between contexts without collapsing them into one model
- Trigger: the user opens a compare view or selects two contexts
- Preconditions: at least two repository, branch, or worktree contexts are open
- Main flow:
  1. The system renders side-by-side contexts with visible repository and branch labels.
  2. It shows structural, textual, and trace differences for the selected scope.
  3. It preserves the current working context while the comparison remains open.
  4. It makes changed items, files, and related traces easy to follow.
- Alternate flows:
  - If the comparison context is read-only, the UI still supports navigation and inspection.
  - If the compared contexts come from different repositories, the UI still labels them explicitly.
- Postconditions: the user can explain what changed and where the changes live.
- Vision trace:
  - Supports: branch-aware modeling, multi-repository context, traceability across alternatives
- Derived functional blocks:
  - FB-01 Workspace and context management
  - FB-04 Traceability projection
  - FB-07 Git workflow
  - FB-08 Saved views and projections

## UC-06 Create And Modify Supported Model Elements

- Actor: systems engineer
- Goal: add or adjust supported structural elements without breaking model identity
- Trigger: the user selects a creation tool, palette item, or inline edit action
- Preconditions: the current context allows the requested edit and the target type is supported
- Main flow:
  1. The system stages the edit in a draft model or draft canvas state.
  2. It updates the visual model and the source preview together.
  3. It keeps the selected element and owning context visible while edits are staged.
  4. It shows validation and ownership feedback before the user saves.
- Alternate flows:
  - If the requested element type is not supported yet, the system prevents the action or labels it as unavailable.
  - If a relationship target is invalid, the system flags the issue before save.
- Postconditions: the user has a staged change that can be saved or discarded.
- Vision trace:
  - Supports: PowerPoint-level ease, precise semantics, safe model edits
- Derived functional blocks:
  - FB-05 Editing and drafting
  - FB-11 Diagnostics and recovery

## UC-07 Save Changes Safely To Source Text

- Actor: systems engineer
- Goal: persist a staged model change to the correct source file without collateral edits
- Trigger: the user selects save or export from draft state
- Preconditions: a writable context exists and the draft has a valid target
- Main flow:
  1. The system resolves the owning file and target context before writing.
  2. It shows the generated source preview and intended save target.
  3. It writes the change only to the allowed file scope.
  4. It reloads or reindexes the model so the result can be verified.
- Alternate flows:
  - If identity metadata is missing, the system blocks the write and explains why.
  - If the save would touch unrelated content, the system refuses the write.
- Postconditions: the source text and model graph remain aligned after the save.
- Vision trace:
  - Supports: textual source of truth, traceable edits, reviewable Git diffs
- Derived functional blocks:
  - FB-06 Write policy and persistence
  - FB-11 Diagnostics and recovery

## UC-08 Commit And Review Changes Through Git

- Actor: systems engineer, reviewer, or integrator
- Goal: turn saved changes into reviewable Git history
- Trigger: the user opens the commit or diff workflow for the active context
- Preconditions: the working tree contains the intended changes and the target context is writable
- Main flow:
  1. The system shows Git status and a semantic diff for the current context.
  2. It presents the target repository and branch before commit.
  3. It creates the commit and returns the commit result and identifier.
  4. It keeps conflict or merge-preview diagnostics visible when relevant.
- Alternate flows:
  - If the commit would write to the wrong context, the system blocks it.
  - If a merge preview reveals a conflict, the system reports the conflict rather than hiding it.
- Postconditions: the change is represented in Git history and can be reviewed.
- Vision trace:
  - Supports: Git as a first-class modeling workflow, reviewable change history
- Derived functional blocks:
  - FB-07 Git workflow
  - FB-11 Diagnostics and recovery

## UC-09 Keep Multiple Contexts Open Side By Side

- Actor: systems engineer
- Goal: work with multiple branches, worktrees, or repositories at once
- Trigger: the user opens an additional context or compare workspace
- Preconditions: the system supports at least one existing workspace context
- Main flow:
  1. The system opens the new context with a distinct identity and write state.
  2. It keeps the current context visible instead of replacing it.
  3. It allows side-by-side navigation, comparison, and inspection.
  4. It ensures each edit target remains explicit.
- Alternate flows:
  - If the added context is read-only, the system still allows inspection and comparison.
  - If the context conflicts with write rules, the system refuses to mark it writable.
- Postconditions: the user can compare or work across contexts without losing track of origin.
- Vision trace:
  - Supports: multi-context workspace, explicit identity, compare without context loss
- Derived functional blocks:
  - FB-01 Workspace and context management
  - FB-07 Git workflow
  - FB-08 Saved views and projections

## UC-10 Create, Restore, And Share Saved Views

- Actor: systems engineer
- Goal: store a useful model projection and reopen it later with stable identity
- Trigger: the user saves a current view or opens a previously saved one
- Preconditions: the current workspace contains a selectable model context
- Main flow:
  1. The system captures the current projection, filters, and stable identity references.
  2. It persists the view as a projection, not as a separate model.
  3. It restores the view in a later session or context using stable identifiers.
  4. It shows missing or moved elements as actionable restoration feedback.
- Alternate flows:
  - If an item no longer exists, the system reports the broken reference clearly.
  - If the view is shared, the system keeps ownership and scope explicit.
- Postconditions: the saved view can be reopened as a repeatable engineering context.
- Vision trace:
  - Supports: custom views as projections, traceable context, repeatable review
- Derived functional blocks:
  - FB-08 Saved views and projections
  - FB-09 Analysis and query

## UC-11 Analyze Impact And Dependency Paths

- Actor: systems engineer or reviewer
- Goal: ask what changes if a selected element changes
- Trigger: the user requests impact, dependency, or trace analysis on a selected item
- Preconditions: the workspace has trace metadata, ownership data, or branch comparison data
- Main flow:
  1. The system gathers item, file, branch, and repository relationships for the selected scope.
  2. It presents the affected elements and the paths between them.
  3. It distinguishes direct trace links from inferred dependency paths.
  4. It keeps the analysis tied to the current selection and context.
- Alternate flows:
  - If the analysis data is incomplete, the system shows the known part and the gap.
  - If the selected element has no trace data, the system reports that explicitly.
- Postconditions: the user can explain the effect of a change before making it.
- Vision trace:
  - Supports: engineering review, traceability across model and repository boundaries
- Derived functional blocks:
  - FB-04 Traceability projection
  - FB-09 Analysis and query

## UC-12 Extend The Model With Advanced SysML Constructs

- Actor: systems engineer or product maintainer
- Goal: expand the supported language slice only after the core workflow is stable
- Trigger: the team decides to add a new language construct or modeling pattern
- Preconditions: the existing browse/edit/save/Git loop is stable for the current supported slice
- Main flow:
  1. The system adds parser, graph, writer, and validation coverage for the new construct.
  2. It exposes the construct in the appropriate views only after the backend behavior exists.
  3. It preserves the existing MVP slice while extending the supported model space.
  4. It adds regression tests before promoting the new capability.
- Alternate flows:
  - If the new construct would destabilize the MVP slice, the system defers it.
  - If the semantics are not yet precise, the UI stays conservative.
- Postconditions: the product covers more SysML without weakening the core model loop.
- Vision trace:
  - Supports: broader MBSE workbench direction, stable source-backed modeling
- Derived functional blocks:
  - FB-10 Advanced SysML support
  - FB-11 Diagnostics and recovery

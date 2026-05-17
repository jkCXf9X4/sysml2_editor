package Sysml2EditorRequirements {
  doc /*
    Source: docs/systems-engineering/product-requirements.md.
  */

  import Sysml2EditorUseCases::*;
  import Sysml2EditorFunctions::*;
  import Sysml2EditorProductBreakdown::*;

  requirement def ProductRequirement {
    doc /*
      Product-level requirement derived from docs/systems-engineering/product-requirements.md.
      Each PR_* element owns its statement and its primary trace links.
    */
  }

  part def ProductRequirementSet {
    // Context model and browsing
    requirement openLocalGitRepository : PR_01_OpenLocalGitRepository;
    requirement visibleContextIdentity : PR_02_VisibleContextIdentity;
    requirement parseToStableGraphWithDiagnostics : PR_03_ParseToStableGraphWithDiagnostics;
    requirement browseWithoutWriteAccess : PR_04_BrowseWithoutWriteAccess;
    requirement synchronizedSelectionViews : PR_05_SynchronizedSelectionViews;

    // Traceability, editing, and write safety
    requirement exposeTraceability : PR_06_ExposeTraceability;
    requirement sourceOwnershipAndImpactVisible : PR_07_SourceOwnershipAndImpactVisible;
    requirement visualStructuralEditing : PR_08_VisualStructuralEditing;
    requirement sourcePreviewBeforeSave : PR_09_SourcePreviewBeforeSave;
    requirement saveOnlyToOwningWritableContext : PR_10_SaveOnlyToOwningWritableContext;
    requirement rejectWritesWithoutIdentityOrOwnership : PR_11_RejectWritesWithoutIdentityOrOwnership;
    requirement preserveStableIdentity : PR_12_PreserveStableIdentity;

    // Git, multi-context, and views
    requirement gitStatusDiffCommitAndMergePreview : PR_13_GitStatusDiffCommitAndMergePreview;
    requirement sideBySideRepositoryAndBranchContexts : PR_14_SideBySideRepositoryAndBranchContexts;
    requirement explicitSaveAndCommitTarget : PR_15_ExplicitSaveAndCommitTarget;
    requirement restoreSavedViewsByStableIdentity : PR_16_RestoreSavedViewsByStableIdentity;
    requirement impactAndDependencyQueries : PR_17_ImpactAndDependencyQueries;

    // Validation, extensibility, and quality
    requirement visibleValidationState : PR_18_VisibleValidationState;
    requirement advancedSysMLRequiresCoverage : PR_19_AdvancedSysMLRequiresCoverage;
    requirement deterministicReviewableTextChanges : PR_20_DeterministicReviewableTextChanges;
  }

  requirement PR_01_OpenLocalGitRepository : ProductRequirement {
    doc /* The product shall open a local Git repository as an explicit workspace context. */
  }
  trace PR_01_OpenLocalGitRepository to UC_01_OpenRepositoryWorkspace;
  trace PR_01_OpenLocalGitRepository to UC_09_KeepMultipleContextsOpenSideBySide;
  satisfy PR_01_OpenLocalGitRepository by FB_01_WorkspaceAndContextManagement;
  satisfy PR_01_OpenLocalGitRepository by PB_02_WorkspaceAndRepositoryCoordinator;

  requirement PR_02_VisibleContextIdentity : ProductRequirement {
    doc /* The product shall keep repository, branch, file, and write-state identity visible in every model pane that can change context. */
  }
  trace PR_02_VisibleContextIdentity to UC_01_OpenRepositoryWorkspace;
  trace PR_02_VisibleContextIdentity to UC_02_BrowseAndInspectModelStructure;
  trace PR_02_VisibleContextIdentity to UC_05_CompareBranchesAndRepositories;
  trace PR_02_VisibleContextIdentity to UC_09_KeepMultipleContextsOpenSideBySide;
  satisfy PR_02_VisibleContextIdentity by FB_01_WorkspaceAndContextManagement;
  satisfy PR_02_VisibleContextIdentity by PB_01_WorkbenchShellAndContextChrome;

  requirement PR_03_ParseToStableGraphWithDiagnostics : ProductRequirement {
    doc /* The product shall parse supported SysML text into a stable internal graph and surface diagnostics when parsing fails. */
  }
  trace PR_03_ParseToStableGraphWithDiagnostics to UC_01_OpenRepositoryWorkspace;
  trace PR_03_ParseToStableGraphWithDiagnostics to UC_04_DiagnoseInvalidOrPartialInput;
  trace PR_03_ParseToStableGraphWithDiagnostics to UC_12_ExtendTheModelWithAdvancedSysMLConstructs;
  satisfy PR_03_ParseToStableGraphWithDiagnostics by FB_02_ModelIngestionAndIndexing;
  satisfy PR_03_ParseToStableGraphWithDiagnostics by PB_03_ModelCoreService;

  requirement PR_04_BrowseWithoutWriteAccess : ProductRequirement {
    doc /* The product shall allow browsing and search without requiring write access. */
  }
  trace PR_04_BrowseWithoutWriteAccess to UC_02_BrowseAndInspectModelStructure;
  trace PR_04_BrowseWithoutWriteAccess to UC_04_DiagnoseInvalidOrPartialInput;
  trace PR_04_BrowseWithoutWriteAccess to UC_09_KeepMultipleContextsOpenSideBySide;
  satisfy PR_04_BrowseWithoutWriteAccess by FB_03_BrowsingAndSelectionSync;
  satisfy PR_04_BrowseWithoutWriteAccess by PB_04_BrowsingAndInspectionSurfaces;

  requirement PR_05_SynchronizedSelectionViews : ProductRequirement {
    doc /* The product shall synchronize tree, graph, source, inspector, and trace views from the same selected element. */
  }
  trace PR_05_SynchronizedSelectionViews to UC_02_BrowseAndInspectModelStructure;
  trace PR_05_SynchronizedSelectionViews to UC_03_TraceFromElementToSourceAndOwnership;
  satisfy PR_05_SynchronizedSelectionViews by FB_03_BrowsingAndSelectionSync;
  satisfy PR_05_SynchronizedSelectionViews by PB_04_BrowsingAndInspectionSurfaces;

  requirement PR_06_ExposeTraceability : ProductRequirement {
    doc /* The product shall expose item-to-item, item-to-file, file-to-file, and branch-to-branch traceability where the underlying data exists. */
  }
  trace PR_06_ExposeTraceability to UC_03_TraceFromElementToSourceAndOwnership;
  trace PR_06_ExposeTraceability to UC_05_CompareBranchesAndRepositories;
  trace PR_06_ExposeTraceability to UC_11_AnalyzeImpactAndDependencyPaths;
  satisfy PR_06_ExposeTraceability by FB_04_TraceabilityProjection;
  satisfy PR_06_ExposeTraceability by PB_08_TraceabilityAndAnalysisService;

  requirement PR_07_SourceOwnershipAndImpactVisible : ProductRequirement {
    doc /* The product shall make source ownership and change impact visible from the selected element. */
  }
  trace PR_07_SourceOwnershipAndImpactVisible to UC_03_TraceFromElementToSourceAndOwnership;
  trace PR_07_SourceOwnershipAndImpactVisible to UC_11_AnalyzeImpactAndDependencyPaths;
  satisfy PR_07_SourceOwnershipAndImpactVisible by FB_09_AnalysisAndQuery;
  satisfy PR_07_SourceOwnershipAndImpactVisible by PB_08_TraceabilityAndAnalysisService;

  requirement PR_08_VisualStructuralEditing : ProductRequirement {
    doc /* The product shall support visual creation, rename, connection, and deletion of supported structural elements. */
  }
  trace PR_08_VisualStructuralEditing to UC_06_CreateAndModifySupportedModelElements;
  satisfy PR_08_VisualStructuralEditing by FB_05_EditingAndDrafting;
  satisfy PR_08_VisualStructuralEditing by PB_05_VisualEditingSurface;

  requirement PR_09_SourcePreviewBeforeSave : ProductRequirement {
    doc /* The product shall generate a source preview before any save action. */
  }
  trace PR_09_SourcePreviewBeforeSave to UC_06_CreateAndModifySupportedModelElements;
  trace PR_09_SourcePreviewBeforeSave to UC_07_SaveChangesSafelyToSourceText;
  satisfy PR_09_SourcePreviewBeforeSave by FB_06_WritePolicyAndPersistence;
  satisfy PR_09_SourcePreviewBeforeSave by PB_05_VisualEditingSurface;

  requirement PR_10_SaveOnlyToOwningWritableContext : ProductRequirement {
    doc /* The product shall save only to the owning writable context and prevent cross-context writes. */
  }
  trace PR_10_SaveOnlyToOwningWritableContext to UC_07_SaveChangesSafelyToSourceText;
  trace PR_10_SaveOnlyToOwningWritableContext to UC_09_KeepMultipleContextsOpenSideBySide;
  satisfy PR_10_SaveOnlyToOwningWritableContext by FB_06_WritePolicyAndPersistence;
  satisfy PR_10_SaveOnlyToOwningWritableContext by PB_06_PersistenceAndWritePolicyService;

  requirement PR_11_RejectWritesWithoutIdentityOrOwnership : ProductRequirement {
    doc /* The product shall reject writes when identity or ownership metadata is insufficient. */
  }
  trace PR_11_RejectWritesWithoutIdentityOrOwnership to UC_04_DiagnoseInvalidOrPartialInput;
  trace PR_11_RejectWritesWithoutIdentityOrOwnership to UC_07_SaveChangesSafelyToSourceText;
  satisfy PR_11_RejectWritesWithoutIdentityOrOwnership by FB_11_DiagnosticsAndRecovery;
  satisfy PR_11_RejectWritesWithoutIdentityOrOwnership by PB_11_SharedContractsAndIdentityPolicy;

  requirement PR_12_PreserveStableIdentity : ProductRequirement {
    doc /* The product shall preserve stable identity across supported rename and move operations. */
  }
  trace PR_12_PreserveStableIdentity to UC_06_CreateAndModifySupportedModelElements;
  trace PR_12_PreserveStableIdentity to UC_07_SaveChangesSafelyToSourceText;
  trace PR_12_PreserveStableIdentity to UC_10_CreateRestoreAndShareSavedViews;
  satisfy PR_12_PreserveStableIdentity by FB_06_WritePolicyAndPersistence;
  satisfy PR_12_PreserveStableIdentity by PB_11_SharedContractsAndIdentityPolicy;

  requirement PR_13_GitStatusDiffCommitAndMergePreview : ProductRequirement {
    doc /* The product shall expose Git status, semantic diff, commit, and merge-preview diagnostics for the active context. */
  }
  trace PR_13_GitStatusDiffCommitAndMergePreview to UC_08_CommitAndReviewChangesThroughGit;
  trace PR_13_GitStatusDiffCommitAndMergePreview to UC_09_KeepMultipleContextsOpenSideBySide;
  satisfy PR_13_GitStatusDiffCommitAndMergePreview by FB_07_GitWorkflow;
  satisfy PR_13_GitStatusDiffCommitAndMergePreview by PB_07_GitWorkflowService;

  requirement PR_14_SideBySideRepositoryAndBranchContexts : ProductRequirement {
    doc /* The product shall support side-by-side repository and branch contexts without replacing the current working context. */
  }
  trace PR_14_SideBySideRepositoryAndBranchContexts to UC_05_CompareBranchesAndRepositories;
  trace PR_14_SideBySideRepositoryAndBranchContexts to UC_09_KeepMultipleContextsOpenSideBySide;
  satisfy PR_14_SideBySideRepositoryAndBranchContexts by FB_01_WorkspaceAndContextManagement;
  satisfy PR_14_SideBySideRepositoryAndBranchContexts by PB_02_WorkspaceAndRepositoryCoordinator;

  requirement PR_15_ExplicitSaveAndCommitTarget : ProductRequirement {
    doc /* The product shall make the intended repository and branch explicit before save or commit. */
  }
  trace PR_15_ExplicitSaveAndCommitTarget to UC_07_SaveChangesSafelyToSourceText;
  trace PR_15_ExplicitSaveAndCommitTarget to UC_08_CommitAndReviewChangesThroughGit;
  trace PR_15_ExplicitSaveAndCommitTarget to UC_09_KeepMultipleContextsOpenSideBySide;
  satisfy PR_15_ExplicitSaveAndCommitTarget by FB_01_WorkspaceAndContextManagement;
  satisfy PR_15_ExplicitSaveAndCommitTarget by PB_01_WorkbenchShellAndContextChrome;

  requirement PR_16_RestoreSavedViewsByStableIdentity : ProductRequirement {
    doc /* The product shall persist and restore saved views using stable model identity rather than display names alone. */
  }
  trace PR_16_RestoreSavedViewsByStableIdentity to UC_10_CreateRestoreAndShareSavedViews;
  satisfy PR_16_RestoreSavedViewsByStableIdentity by FB_08_SavedViewsAndProjections;
  satisfy PR_16_RestoreSavedViewsByStableIdentity by PB_09_SavedViewsAndProjectionService;

  requirement PR_17_ImpactAndDependencyQueries : ProductRequirement {
    doc /* The product shall support impact analysis and dependency queries across items, files, branches, and repositories where data exists. */
  }
  trace PR_17_ImpactAndDependencyQueries to UC_11_AnalyzeImpactAndDependencyPaths;
  satisfy PR_17_ImpactAndDependencyQueries by FB_09_AnalysisAndQuery;
  satisfy PR_17_ImpactAndDependencyQueries by PB_08_TraceabilityAndAnalysisService;

  requirement PR_18_VisibleValidationState : ProductRequirement {
    doc /* The product shall keep validation state visible during browse, edit, save, and Git workflows. */
  }
  trace PR_18_VisibleValidationState to UC_04_DiagnoseInvalidOrPartialInput;
  trace PR_18_VisibleValidationState to UC_06_CreateAndModifySupportedModelElements;
  trace PR_18_VisibleValidationState to UC_07_SaveChangesSafelyToSourceText;
  trace PR_18_VisibleValidationState to UC_08_CommitAndReviewChangesThroughGit;
  satisfy PR_18_VisibleValidationState by FB_11_DiagnosticsAndRecovery;
  satisfy PR_18_VisibleValidationState by PB_01_WorkbenchShellAndContextChrome;

  requirement PR_19_AdvancedSysMLRequiresCoverage : ProductRequirement {
    doc /* The product shall add advanced SysML syntax only when parser, graph, write policy, and regression coverage are in place. */
  }
  trace PR_19_AdvancedSysMLRequiresCoverage to UC_12_ExtendTheModelWithAdvancedSysMLConstructs;
  satisfy PR_19_AdvancedSysMLRequiresCoverage by FB_10_AdvancedSysMLSupport;
  satisfy PR_19_AdvancedSysMLRequiresCoverage by PB_10_AdvancedSysMLCapabilityPack;

  requirement PR_20_DeterministicReviewableTextChanges : ProductRequirement {
    doc /* The product shall keep the supported slice deterministic and reviewable, avoiding unrelated text churn in saves. */
  }
  trace PR_20_DeterministicReviewableTextChanges to UC_07_SaveChangesSafelyToSourceText;
  trace PR_20_DeterministicReviewableTextChanges to UC_08_CommitAndReviewChangesThroughGit;
  satisfy PR_20_DeterministicReviewableTextChanges by FB_06_WritePolicyAndPersistence;
  satisfy PR_20_DeterministicReviewableTextChanges by PB_06_PersistenceAndWritePolicyService;
}

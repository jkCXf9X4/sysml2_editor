package Sysml2EditorFunctions {
  doc /*
    Source: docs/systems-engineering/functional-breakdown.md.
  */

  import Sysml2EditorUseCases::*;

  part def FunctionalBreakdown {
    part contextAndCoreModel : ContextAndCoreModelFunctions;
    part browsingAndTraceViews : BrowsingAndTraceViewFunctions;
    part editingAndPersistence : EditingAndPersistenceFunctions;
    part projectionsAndExpansion : ProjectionAndExpansionFunctions;
  }

  part def ContextAndCoreModelFunctions {
    part workspaceAndContextManagement : FB_01_WorkspaceAndContextManagement;
    part modelIngestionAndIndexing : FB_02_ModelIngestionAndIndexing;
    part diagnosticsAndRecovery : FB_11_DiagnosticsAndRecovery;
  }

  part def BrowsingAndTraceViewFunctions {
    part browsingAndSelectionSync : FB_03_BrowsingAndSelectionSync;
    part traceabilityProjection : FB_04_TraceabilityProjection;
    part analysisAndQuery : FB_09_AnalysisAndQuery;
  }

  part def EditingAndPersistenceFunctions {
    part editingAndDrafting : FB_05_EditingAndDrafting;
    part writePolicyAndPersistence : FB_06_WritePolicyAndPersistence;
    part gitWorkflow : FB_07_GitWorkflow;
  }

  part def ProjectionAndExpansionFunctions {
    part savedViewsAndProjections : FB_08_SavedViewsAndProjections;
    part advancedSysMLSupport : FB_10_AdvancedSysMLSupport;
  }

  part def FB_01_WorkspaceAndContextManagement {
    doc /* Create, close, and label repository, branch, and worktree contexts. */
  }

  part def FB_02_ModelIngestionAndIndexing {
    doc /* Discover SysML files, parse text, build the model graph, and record diagnostics. */
  }

  part def FB_03_BrowsingAndSelectionSync {
    doc /* Keep tree, graph, source, inspector, and search views aligned to one selection. */
  }

  part def FB_04_TraceabilityProjection {
    doc /* Expose item-to-item, item-to-file, file-to-file, and branch-to-branch trace links. */
  }

  part def FB_05_EditingAndDrafting {
    doc /* Create, rename, connect, and stage supported model changes in a draft state. */
  }

  part def FB_06_WritePolicyAndPersistence {
    doc /* Resolve ownership, protect identity, and save only to a valid writable context. */
  }

  part def FB_07_GitWorkflow {
    doc /* Expose status, diff, commit, and merge-preview behavior for the active context. */
  }

  part def FB_08_SavedViewsAndProjections {
    doc /* Persist, restore, and compare saved projections without making separate models. */
  }

  part def FB_09_AnalysisAndQuery {
    doc /* Answer impact, dependency, ownership, and matrix-style questions from model data. */
  }

  part def FB_10_AdvancedSysMLSupport {
    doc /* Expand supported SysML only after core workflows are stable. */
  }

  part def FB_11_DiagnosticsAndRecovery {
    doc /* Surface invalid input, unsafe writes, missing identity, and conflict conditions clearly. */
  }

  trace FB_01_WorkspaceAndContextManagement to UC_01_OpenRepositoryWorkspace;
  trace FB_01_WorkspaceAndContextManagement to UC_09_KeepMultipleContextsOpenSideBySide;
  trace FB_03_BrowsingAndSelectionSync to UC_02_BrowseAndInspectModelStructure;
  trace FB_04_TraceabilityProjection to UC_03_TraceFromElementToSourceAndOwnership;
  trace FB_05_EditingAndDrafting to UC_06_CreateAndModifySupportedModelElements;
  trace FB_06_WritePolicyAndPersistence to UC_07_SaveChangesSafelyToSourceText;
  trace FB_07_GitWorkflow to UC_08_CommitAndReviewChangesThroughGit;
  trace FB_08_SavedViewsAndProjections to UC_10_CreateRestoreAndShareSavedViews;
  trace FB_09_AnalysisAndQuery to UC_11_AnalyzeImpactAndDependencyPaths;
  trace FB_10_AdvancedSysMLSupport to UC_12_ExtendTheModelWithAdvancedSysMLConstructs;
  trace FB_11_DiagnosticsAndRecovery to UC_04_DiagnoseInvalidOrPartialInput;
}

package Sysml2EditorUseCases {
  doc /*
    Source: docs/systems-engineering/use-cases.md.
  */

  part def ProductUseCaseModel {
    part workspaceBrowsingAndTraceability : WorkspaceBrowsingAndTraceabilityUseCases;
    part editingComparisonAndGit : EditingComparisonAndGitUseCases;
    part multiContextViewsAndExpansion : MultiContextViewsAndExpansionUseCases;
  }

  part def WorkspaceBrowsingAndTraceabilityUseCases {
    part openRepositoryWorkspace : UC_01_OpenRepositoryWorkspace;
    part browseAndInspectModelStructure : UC_02_BrowseAndInspectModelStructure;
    part traceFromElementToSourceAndOwnership : UC_03_TraceFromElementToSourceAndOwnership;
    part diagnoseInvalidOrPartialInput : UC_04_DiagnoseInvalidOrPartialInput;
  }

  part def EditingComparisonAndGitUseCases {
    part compareBranchesAndRepositories : UC_05_CompareBranchesAndRepositories;
    part createAndModifySupportedModelElements : UC_06_CreateAndModifySupportedModelElements;
    part saveChangesSafelyToSourceText : UC_07_SaveChangesSafelyToSourceText;
    part commitAndReviewChangesThroughGit : UC_08_CommitAndReviewChangesThroughGit;
  }

  part def MultiContextViewsAndExpansionUseCases {
    part keepMultipleContextsOpenSideBySide : UC_09_KeepMultipleContextsOpenSideBySide;
    part createRestoreAndShareSavedViews : UC_10_CreateRestoreAndShareSavedViews;
    part analyzeImpactAndDependencyPaths : UC_11_AnalyzeImpactAndDependencyPaths;
    part extendTheModelWithAdvancedSysMLConstructs : UC_12_ExtendTheModelWithAdvancedSysMLConstructs;
  }

  part def UC_01_OpenRepositoryWorkspace {
    doc /* Establish an explicit repository and branch context with visible identity. */
  }

  part def UC_02_BrowseAndInspectModelStructure {
    doc /* Synchronized navigation across tree, graph, source, inspector, and trace views. */
  }

  part def UC_03_TraceFromElementToSourceAndOwnership {
    doc /* Show source ownership, source range, and incoming or outgoing trace links. */
  }

  part def UC_04_DiagnoseInvalidOrPartialInput {
    doc /* Surface parse, validation, and write failures without hiding usable model context. */
  }

  part def UC_05_CompareBranchesAndRepositories {
    doc /* Compare contexts side by side without collapsing them into one model. */
  }

  part def UC_06_CreateAndModifySupportedModelElements {
    doc /* Stage visual model edits while preserving semantic identity and context. */
  }

  part def UC_07_SaveChangesSafelyToSourceText {
    doc /* Persist staged changes only to the correct owning source file and context. */
  }

  part def UC_08_CommitAndReviewChangesThroughGit {
    doc /* Turn saved changes into reviewable Git status, diff, and commit history. */
  }

  part def UC_09_KeepMultipleContextsOpenSideBySide {
    doc /* Keep multiple branch, worktree, or repository contexts visible and distinct. */
  }

  part def UC_10_CreateRestoreAndShareSavedViews {
    doc /* Save projections and restore them by stable model identity. */
  }

  part def UC_11_AnalyzeImpactAndDependencyPaths {
    doc /* Analyze what is affected by a selected model item across items, files, branches, and repositories. */
  }

  part def UC_12_ExtendTheModelWithAdvancedSysMLConstructs {
    doc /* Expand SysML support only after the core browse, edit, save, and Git loop is stable. */
  }
}

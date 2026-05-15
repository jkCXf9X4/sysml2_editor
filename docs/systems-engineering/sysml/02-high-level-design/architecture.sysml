package Sysml2EditorArchitecture {
  import Sysml2EditorFunctions::*;
  import Sysml2EditorProductBreakdown::*;
  import Sysml2EditorRequirements::*;

  part def HighLevelArchitectureDefinition {
    doc /*
      Sources: docs/architecture/README.md and docs/architecture/overall_design.md.

      Architecture is the high-level design view. It defines the product-wide
      system guarantees and structural choices before implementation details.
    */

    part overallDesign : OverallDesignDecision;
    part gitAndBranchModel : GitAndBranchModel;
    part dataModelArchitecture : DataModelArchitecture;
    part contextArchitecture : ContextArchitecture;
    part highLevelDesignPrinciples : HighLevelDesignPrinciples;
  }

  part def OverallDesignDecision {
    doc /*
      Source: docs/architecture/overall_design.md.

      Textual SysML files in Git are the source of truth.
      Parser service builds an internal model graph.
      UI views are projections over that graph.
      Edits target exactly one writable context.
    */
  }

  part def GitAndBranchModel {
    doc /*
      Git is a visible modeling concept. The product scope includes opening,
      cloning, branching, comparing, committing, pulling, pushing, and resolving
      model-level conflicts where possible.
    */
  }

  part def DataModelArchitecture {
    doc /*
      Source flow:

      Git repo -> textual SysML files -> parser service -> internal model graph
      -> view model -> canvas, tree, table, and editor UI.

      The canvas is a projection, not the durable model.
    */
  }

  part def ContextArchitecture {
    doc /*
      Workspace context is explicit across repositories, branches, worktrees,
      model graphs, source files, trace links, views, save actions, and commits.
      Combined views are projections across contexts.
    */
  }

  part def HighLevelDesignPrinciples {
    part textIsTruth;
    part diagramsAreViews;
    part invalidModelingIsDifficult;
    part changesAreVisibleAgainstCommittedBaseline;
    part abstractionLevelsAreExplicit;
    part traceabilityIsFirstClass;
    part contextIsExplicit;
  }

  trace OverallDesignDecision to PR_02_VisibleContextIdentity;
  trace OverallDesignDecision to PR_06_ExposeTraceability;
  trace OverallDesignDecision to PR_14_SideBySideRepositoryAndBranchContexts;
  trace OverallDesignDecision to FB_01_WorkspaceAndContextManagement;
  trace OverallDesignDecision to FB_04_TraceabilityProjection;
  trace GitAndBranchModel to PB_02_WorkspaceAndRepositoryCoordinator;
  trace GitAndBranchModel to PB_07_GitWorkflowService;
  trace GitAndBranchModel to PR_01_OpenLocalGitRepository;
  trace GitAndBranchModel to PR_13_GitStatusDiffCommitAndMergePreview;
  trace DataModelArchitecture to PB_03_ModelCoreService;
  trace DataModelArchitecture to PB_04_BrowsingAndInspectionSurfaces;
  trace DataModelArchitecture to PB_08_TraceabilityAndAnalysisService;
  trace DataModelArchitecture to PR_03_ParseToStableGraphWithDiagnostics;
  trace DataModelArchitecture to PR_05_SynchronizedSelectionViews;
  trace ContextArchitecture to FB_01_WorkspaceAndContextManagement;
  trace ContextArchitecture to PR_10_SaveOnlyToOwningWritableContext;
  trace ContextArchitecture to PR_14_SideBySideRepositoryAndBranchContexts;
  trace HighLevelDesignPrinciples to PR_12_PreserveStableIdentity;
  trace HighLevelDesignPrinciples to PR_20_DeterministicReviewableTextChanges;
}

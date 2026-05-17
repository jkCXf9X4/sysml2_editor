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

    part overallDesign {
      doc /*
        Source: docs/architecture/overall_design.md.

        Textual SysML files in Git are the source of truth.
        Parser service builds an internal model graph.
        UI views are projections over that graph.
        Edits target exactly one writable context.
      */
    }

    part gitAndBranchModel {
      doc /*
        Git is a visible modeling concept. The product scope includes opening,
        cloning, branching, comparing, committing, pulling, pushing, and resolving
        model-level conflicts where possible.
      */
    }

    part dataModelArchitecture {
      doc /*
        Source flow:

        Git repo -> textual SysML files -> parser service -> internal model graph
        -> view model -> canvas, tree, table, and editor UI.

        The canvas is a projection, not the durable model.
      */
    }

    part contextArchitecture {
      doc /*
        Workspace context is explicit across repositories, branches, worktrees,
        model graphs, source files, trace links, views, save actions, and commits.
        Combined views are projections across contexts.
      */
    }

    part highLevelDesignPrinciples {
      part textIsTruth;
      part diagramsAreViews;
      part invalidModelingIsDifficult;
      part changesAreVisibleAgainstCommittedBaseline;
      part abstractionLevelsAreExplicit;
      part traceabilityIsFirstClass;
      part contextIsExplicit;
    }

    trace overallDesign to PR_02_VisibleContextIdentity;
    trace overallDesign to PR_06_ExposeTraceability;
    trace overallDesign to PR_14_SideBySideRepositoryAndBranchContexts;
    trace overallDesign to FB_01_WorkspaceAndContextManagement;
    trace overallDesign to FB_04_TraceabilityProjection;
    trace gitAndBranchModel to PB_02_WorkspaceAndRepositoryCoordinator;
    trace gitAndBranchModel to PB_07_GitWorkflowService;
    trace gitAndBranchModel to PR_01_OpenLocalGitRepository;
    trace gitAndBranchModel to PR_13_GitStatusDiffCommitAndMergePreview;
    trace dataModelArchitecture to PB_03_ModelCoreService;
    trace dataModelArchitecture to PB_04_BrowsingAndInspectionSurfaces;
    trace dataModelArchitecture to PB_08_TraceabilityAndAnalysisService;
    trace dataModelArchitecture to PR_03_ParseToStableGraphWithDiagnostics;
    trace dataModelArchitecture to PR_05_SynchronizedSelectionViews;
    trace contextArchitecture to FB_01_WorkspaceAndContextManagement;
    trace contextArchitecture to PR_10_SaveOnlyToOwningWritableContext;
    trace contextArchitecture to PR_14_SideBySideRepositoryAndBranchContexts;
    trace highLevelDesignPrinciples to PR_12_PreserveStableIdentity;
    trace highLevelDesignPrinciples to PR_20_DeterministicReviewableTextChanges;
  }
}

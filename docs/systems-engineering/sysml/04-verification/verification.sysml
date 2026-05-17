package Sysml2EditorVerification {
  import Sysml2EditorRequirements::*;

  part def VerificationInventory {
    doc /*
      Verification inventory derived from docs/testing.
      It connects product requirements to current or planned evidence.
    */

    part localDevelopmentGates {
      part frontendUnitAndComponentTests;
      part frontendTypecheck;
      part frontendProductionBuild;
      part backendSmoke;
      part backendDomainTests;
      part frontendSmoke;
      part frontendBrowserSmoke;
    }

    part semanticModelVerification {
      part parserGraphAndSourceTests;
      part traceabilityTests;
      part writerAndSavePolicyTests;
      part stableIdentityTests;
      part gitDomainTests;
    }

    part userWorkflowVerification {
      part openBrowseInspectWorkflow;
      part editSaveReloadWorkflow;
      part branchCompareWorkflow;
      part commitWorkflow;
      part savedViewRestoreWorkflow;
      part impactAnalysisWorkflow;
    }

    trace localDevelopmentGates to PR_18_VisibleValidationState;
    trace semanticModelVerification to PR_03_ParseToStableGraphWithDiagnostics;
    trace semanticModelVerification to PR_06_ExposeTraceability;
    trace semanticModelVerification to PR_10_SaveOnlyToOwningWritableContext;
    trace semanticModelVerification to PR_12_PreserveStableIdentity;
    trace userWorkflowVerification to PR_01_OpenLocalGitRepository;
    trace userWorkflowVerification to PR_05_SynchronizedSelectionViews;
    trace userWorkflowVerification to PR_08_VisualStructuralEditing;
    trace userWorkflowVerification to PR_13_GitStatusDiffCommitAndMergePreview;
    trace userWorkflowVerification to PR_16_RestoreSavedViewsByStableIdentity;
    trace userWorkflowVerification to PR_17_ImpactAndDependencyQueries;
  }
}

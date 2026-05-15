package Sysml2EditorVerification {
  import Sysml2EditorRequirements::*;

  part def VerificationInventory {
    doc /*
      Verification inventory derived from docs/testing.
      It connects product requirements to current or planned evidence.
    */

    part localDevelopmentGates : LocalDevelopmentGateVerification;
    part semanticModelVerification : SemanticModelVerification;
    part userWorkflowVerification : UserWorkflowVerification;
  }

  part def LocalDevelopmentGateVerification {
    part frontendUnitAndComponentTests;
    part frontendTypecheck;
    part frontendProductionBuild;
    part backendSmoke;
    part backendDomainTests;
    part frontendSmoke;
    part frontendBrowserSmoke;
  }

  part def SemanticModelVerification {
    part parserGraphAndSourceTests;
    part traceabilityTests;
    part writerAndSavePolicyTests;
    part stableIdentityTests;
    part gitDomainTests;
  }

  part def UserWorkflowVerification {
    part openBrowseInspectWorkflow;
    part editSaveReloadWorkflow;
    part branchCompareWorkflow;
    part commitWorkflow;
    part savedViewRestoreWorkflow;
    part impactAnalysisWorkflow;
  }

  trace LocalDevelopmentGateVerification to PR_18_VisibleValidationState;
  trace SemanticModelVerification to PR_03_ParseToStableGraphWithDiagnostics;
  trace SemanticModelVerification to PR_06_ExposeTraceability;
  trace SemanticModelVerification to PR_10_SaveOnlyToOwningWritableContext;
  trace SemanticModelVerification to PR_12_PreserveStableIdentity;
  trace UserWorkflowVerification to PR_01_OpenLocalGitRepository;
  trace UserWorkflowVerification to PR_05_SynchronizedSelectionViews;
  trace UserWorkflowVerification to PR_08_VisualStructuralEditing;
  trace UserWorkflowVerification to PR_13_GitStatusDiffCommitAndMergePreview;
  trace UserWorkflowVerification to PR_16_RestoreSavedViewsByStableIdentity;
  trace UserWorkflowVerification to PR_17_ImpactAndDependencyQueries;
}

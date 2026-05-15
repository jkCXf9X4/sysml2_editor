package Sysml2EditorDetailedDesign {
  import Sysml2EditorArchitecture::*;
  import Sysml2EditorProductBreakdown::*;
  import Sysml2EditorRequirements::*;

  part def DetailedDesignDefinition {
    doc /*
      Detailed design is the implementation-facing layer. It captures runtime,
      graph, write, and contract decisions that implement the high-level
      architecture.
    */

    part runtime : LocalWebRuntimeDecision;
    part modelGraph : StableModelGraphDecision;
    part writePolicy : DeterministicWritePolicyDecision;
    part implementationContracts : ImplementationContracts;
  }

  part def LocalWebRuntimeDecision {
    doc /*
      Source: docs/architecture/runtime.md.

      Phase 1 runtime is a local web app with React frontend and ASP.NET Core
      backend. The backend owns repository access, file IO, Git processes,
      workspace contexts, parsing, and save operations.
    */

    part frontendRuntime;
    part backendRuntime;
    part apiBoundary;
    part workspaceContextRuntime;
    part developmentCommandShape;
  }

  part def StableModelGraphDecision {
    doc /*
      Source: docs/architecture/model-graph.md.

      One stable graph schema carries context, nodes, edges, files, source
      ranges, trace links, and multi-context projections.
    */

    part modelContext;
    part modelNode;
    part modelEdge;
    part sourceRange;
    part traceLink;
    part multiContextView;
  }

  part def DeterministicWritePolicyDecision {
    doc /*
      Source: docs/architecture/write-policy.md.

      Save behavior must target one workspace context, preserve source
      ownership and stable identity, avoid unrelated churn, and block unsafe
      writes.
    */

    part fileConvention;
    part ownershipRules;
    part fileSelectionRules;
    part transactionalSaveFlow;
    part writeFailureModes;
    part worktreeAndContextRules;
  }

  part def ImplementationContracts {
    doc /*
      Sources: docs/implementation and docs/architecture.

      This group tracks implementation-level contracts such as API boundaries,
      parser subset behavior, stable ID behavior, DTO shapes, and source
      ownership rules. It should stay aligned with implementation specs without
      replacing the product definition.
    */

    part backendApiContracts;
    part parserContract;
    part graphDtoContract;
    part sourceOwnershipContract;
    part stableIdentityContract;
  }

  trace LocalWebRuntimeDecision to HighLevelArchitectureDefinition;
  trace LocalWebRuntimeDecision to PB_01_WorkbenchShellAndContextChrome;
  trace LocalWebRuntimeDecision to PB_02_WorkspaceAndRepositoryCoordinator;
  trace LocalWebRuntimeDecision to PR_01_OpenLocalGitRepository;
  trace LocalWebRuntimeDecision to PR_13_GitStatusDiffCommitAndMergePreview;
  trace StableModelGraphDecision to HighLevelArchitectureDefinition;
  trace StableModelGraphDecision to PB_03_ModelCoreService;
  trace StableModelGraphDecision to PB_04_BrowsingAndInspectionSurfaces;
  trace StableModelGraphDecision to PB_08_TraceabilityAndAnalysisService;
  trace StableModelGraphDecision to PR_03_ParseToStableGraphWithDiagnostics;
  trace StableModelGraphDecision to PR_05_SynchronizedSelectionViews;
  trace StableModelGraphDecision to PR_12_PreserveStableIdentity;
  trace DeterministicWritePolicyDecision to HighLevelArchitectureDefinition;
  trace DeterministicWritePolicyDecision to PB_06_PersistenceAndWritePolicyService;
  trace DeterministicWritePolicyDecision to PB_11_SharedContractsAndIdentityPolicy;
  trace DeterministicWritePolicyDecision to PR_10_SaveOnlyToOwningWritableContext;
  trace DeterministicWritePolicyDecision to PR_11_RejectWritesWithoutIdentityOrOwnership;
  trace DeterministicWritePolicyDecision to PR_20_DeterministicReviewableTextChanges;
  trace ImplementationContracts to PB_11_SharedContractsAndIdentityPolicy;
  trace ImplementationContracts to PR_03_ParseToStableGraphWithDiagnostics;
  trace ImplementationContracts to PR_12_PreserveStableIdentity;
}

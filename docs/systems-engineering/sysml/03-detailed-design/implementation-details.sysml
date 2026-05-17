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

    part runtime {
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

    part modelGraph {
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

    part writePolicy {
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

    part implementationContracts {
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

    trace runtime to HighLevelArchitectureDefinition;
    trace runtime to PB_01_WorkbenchShellAndContextChrome;
    trace runtime to PB_02_WorkspaceAndRepositoryCoordinator;
    trace runtime to PR_01_OpenLocalGitRepository;
    trace runtime to PR_13_GitStatusDiffCommitAndMergePreview;
    trace modelGraph to HighLevelArchitectureDefinition;
    trace modelGraph to PB_03_ModelCoreService;
    trace modelGraph to PB_04_BrowsingAndInspectionSurfaces;
    trace modelGraph to PB_08_TraceabilityAndAnalysisService;
    trace modelGraph to PR_03_ParseToStableGraphWithDiagnostics;
    trace modelGraph to PR_05_SynchronizedSelectionViews;
    trace modelGraph to PR_12_PreserveStableIdentity;
    trace writePolicy to HighLevelArchitectureDefinition;
    trace writePolicy to PB_06_PersistenceAndWritePolicyService;
    trace writePolicy to PB_11_SharedContractsAndIdentityPolicy;
    trace writePolicy to PR_10_SaveOnlyToOwningWritableContext;
    trace writePolicy to PR_11_RejectWritesWithoutIdentityOrOwnership;
    trace writePolicy to PR_20_DeterministicReviewableTextChanges;
    trace implementationContracts to PB_11_SharedContractsAndIdentityPolicy;
    trace implementationContracts to PR_03_ParseToStableGraphWithDiagnostics;
    trace implementationContracts to PR_12_PreserveStableIdentity;
  }
}

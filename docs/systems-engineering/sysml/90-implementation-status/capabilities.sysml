package Sysml2EditorCapabilities {
  import Sysml2EditorProductDefinition::*;
  import Sysml2EditorProductBreakdown::*;
  import Sysml2EditorFunctions::*;
  import Sysml2EditorArchitecture::*;
  import Sysml2EditorDetailedDesign::*;
  import Sysml2EditorRequirements::*;
  import Sysml2EditorVerification::*;

  part def CapabilityInventory {
    doc /*
      Capability inventory derived from docs/functionality.
      It separates full product capability scope from current implementation status.
    */

    part productCapabilityScope : ProductCapabilityScope;
    part productDefinitionAlignment : ProductDefinitionImplementationAlignment;
    part mainBreakdownReferences : MainBreakdownImplementationReferences;
    part currentImplementationStatus : CurrentImplementationStatus;
    part riskInventory : CapabilityRiskInventory;
  }

  part def ProductCapabilityScope {
    part developmentRuntimeAndTestability : CAP_01_DevelopmentRuntimeAndTestability;
    part workbenchUIFoundation : CAP_02_WorkbenchUIFoundation;
    part backendPlatformAndAPIFoundation : CAP_03_BackendPlatformAndAPIFoundation;
    part repositoryAndWorkspaceContext : CAP_04_RepositoryAndWorkspaceContext;
    part sysMLParsingGraphAndSourceModel : CAP_05_SysMLParsingGraphAndSourceModel;
    part readOnlyModelBrowsing : CAP_06_ReadOnlyModelBrowsing;
    part traceabilityAndAnalysis : CAP_07_TraceabilityAndAnalysis;
    part editingAndWriteSafety : CAP_08_EditingAndWriteSafety;
    part gitNativeWorkflow : CAP_09_GitNativeWorkflow;
    part savedViewsAndCustomProjections : CAP_10_SavedViewsAndCustomProjections;
    part advancedSysMLLibrariesCIAndReporting : CAP_11_AdvancedSysMLLibrariesCIAndReporting;
  }

  part def CurrentImplementationStatus {
    part availableNow : AvailableCapabilities;
    part partialNow : PartialCapabilities;
    part futurePlanned : FutureCapabilities;
  }

  part def ProductDefinitionImplementationAlignment {
    doc /*
      Parallel status view against the product definition. This does not
      redefine the product scope; it shows whether the current implementation
      proves, partially proves, or still lacks each product-definition promise.
    */

    part missionAlignment;
    part sourceOfTruthAlignment;
    part workbenchPromiseAlignment;
    part traceabilityScopeAlignment;
    part multiContextScopeAlignment;
    part readOnlyBrowserFirstAlignment;
  }

  part def MainBreakdownImplementationReferences {
    doc /*
      Cross references from the parallel implementation-status track back into
      the main breakdown. These references make status traceable without
      placing status inside product definition, design, or verification.
    */

    part productDefinitionReference;
    part requirementsReference;
    part highLevelFunctionReference;
    part productBreakdownReference;
    part highLevelArchitectureReference;
    part detailedDesignReference;
    part verificationReference;
  }

  part def AvailableCapabilities {
    part developmentRuntimeAndTestability : CAP_01_DevelopmentRuntimeAndTestability;
    part workbenchUIFoundation : CAP_02_WorkbenchUIFoundation;
    part backendPlatformAndAPIFoundation : CAP_03_BackendPlatformAndAPIFoundation;
    part sysMLParsingGraphAndSourceModel : CAP_05_SysMLParsingGraphAndSourceModel;
    part readOnlyModelBrowsing : CAP_06_ReadOnlyModelBrowsing;
  }

  part def PartialCapabilities {
    part repositoryAndWorkspaceContext : CAP_04_RepositoryAndWorkspaceContext;
    part traceabilityAndAnalysis : CAP_07_TraceabilityAndAnalysis;
    part editingAndWriteSafety : CAP_08_EditingAndWriteSafety;
    part gitNativeWorkflow : CAP_09_GitNativeWorkflow;
    part savedViewsAndCustomProjections : CAP_10_SavedViewsAndCustomProjections;
  }

  part def FutureCapabilities {
    part advancedSysMLLibrariesCIAndReporting : CAP_11_AdvancedSysMLLibrariesCIAndReporting;
  }

  part def CapabilityRiskInventory {
    part fixtureBackedFrontendWorkflows;
    part limitedBackendGenerality;
    part missingBackendBackedE2E;
    part backendPrimitivesWithoutUIWorkflows;
    part multiContextWriteSafety;
    part limitedResponsiveVerification;
  }

  part def CAP_01_DevelopmentRuntimeAndTestability {
    doc /* Source: docs/functionality/development-runtime.md. Status: Available. Stability: Stable slice. */
  }

  part def CAP_02_WorkbenchUIFoundation {
    doc /* Source: docs/functionality/workbench-ui.md. Status: Available. Stability: Stable slice. */
  }

  part def CAP_03_BackendPlatformAndAPIFoundation {
    doc /* Source: docs/functionality/backend-platform.md. Status: Available. Stability: Stable slice. */
  }

  part def CAP_04_RepositoryAndWorkspaceContext {
    doc /* Source: docs/functionality/repository-workspace.md. Status: Partial. Stability: Development. */
  }

  part def CAP_05_SysMLParsingGraphAndSourceModel {
    doc /* Source: docs/functionality/sysml-graph-source.md. Status: Available. Stability: Stable slice. */
  }

  part def CAP_06_ReadOnlyModelBrowsing {
    doc /* Source: docs/functionality/model-browsing.md. Status: Available. Stability: Development. */
  }

  part def CAP_07_TraceabilityAndAnalysis {
    doc /* Source: docs/functionality/traceability-analysis.md. Status: Partial. Stability: Development. */
  }

  part def CAP_08_EditingAndWriteSafety {
    doc /* Source: docs/functionality/editing-write-safety.md. Status: Partial. Stability: Development. */
  }

  part def CAP_09_GitNativeWorkflow {
    doc /* Source: docs/functionality/git-workflow.md. Status: Partial. Stability: Development. */
  }

  part def CAP_10_SavedViewsAndCustomProjections {
    doc /* Source: docs/functionality/saved-views.md. Status: Partial. Stability: Development. */
  }

  part def CAP_11_AdvancedSysMLLibrariesCIAndReporting {
    doc /* Source: docs/functionality/advanced-sysml.md. Status: Future. Stability: Planned. */
  }

  trace CAP_01_DevelopmentRuntimeAndTestability to PB_11_SharedContractsAndIdentityPolicy;
  trace CAP_02_WorkbenchUIFoundation to PB_01_WorkbenchShellAndContextChrome;
  trace CAP_03_BackendPlatformAndAPIFoundation to PB_02_WorkspaceAndRepositoryCoordinator;
  trace CAP_04_RepositoryAndWorkspaceContext to PB_02_WorkspaceAndRepositoryCoordinator;
  trace CAP_05_SysMLParsingGraphAndSourceModel to PB_03_ModelCoreService;
  trace CAP_06_ReadOnlyModelBrowsing to PB_04_BrowsingAndInspectionSurfaces;
  trace CAP_07_TraceabilityAndAnalysis to PB_08_TraceabilityAndAnalysisService;
  trace CAP_08_EditingAndWriteSafety to PB_05_VisualEditingSurface;
  trace CAP_08_EditingAndWriteSafety to PB_06_PersistenceAndWritePolicyService;
  trace CAP_09_GitNativeWorkflow to PB_07_GitWorkflowService;
  trace CAP_10_SavedViewsAndCustomProjections to PB_09_SavedViewsAndProjectionService;
  trace CAP_11_AdvancedSysMLLibrariesCIAndReporting to PB_10_AdvancedSysMLCapabilityPack;

  trace ProductDefinitionImplementationAlignment to ProductDefinition;
  trace ProductDefinitionImplementationAlignment to ProductMission;
  trace ProductDefinitionImplementationAlignment to TextualSysMLSourceOfTruth;
  trace ProductDefinitionImplementationAlignment to EngineeringWorkbenchPromise;
  trace ProductDefinitionImplementationAlignment to TraceabilityScope;
  trace ProductDefinitionImplementationAlignment to MultiContextScope;
  trace ProductDefinitionImplementationAlignment to ReadOnlyBrowserFirst;

  trace MainBreakdownImplementationReferences to ProductDefinition;
  trace MainBreakdownImplementationReferences to ProductRequirementSet;
  trace MainBreakdownImplementationReferences to FunctionalBreakdown;
  trace MainBreakdownImplementationReferences to ProductBreakdown;
  trace MainBreakdownImplementationReferences to HighLevelArchitectureDefinition;
  trace MainBreakdownImplementationReferences to DetailedDesignDefinition;
  trace MainBreakdownImplementationReferences to VerificationInventory;

  trace CAP_01_DevelopmentRuntimeAndTestability to PR_18_VisibleValidationState;
  trace CAP_02_WorkbenchUIFoundation to PR_02_VisibleContextIdentity;
  trace CAP_03_BackendPlatformAndAPIFoundation to PR_01_OpenLocalGitRepository;
  trace CAP_04_RepositoryAndWorkspaceContext to FB_01_WorkspaceAndContextManagement;
  trace CAP_05_SysMLParsingGraphAndSourceModel to FB_02_ModelIngestionAndIndexing;
  trace CAP_06_ReadOnlyModelBrowsing to FB_03_BrowsingAndSelectionSync;
  trace CAP_07_TraceabilityAndAnalysis to FB_04_TraceabilityProjection;
  trace CAP_08_EditingAndWriteSafety to FB_05_EditingAndDrafting;
  trace CAP_08_EditingAndWriteSafety to FB_06_WritePolicyAndPersistence;
  trace CAP_09_GitNativeWorkflow to FB_07_GitWorkflow;
  trace CAP_10_SavedViewsAndCustomProjections to FB_08_SavedViewsAndProjections;
  trace CAP_11_AdvancedSysMLLibrariesCIAndReporting to FB_10_AdvancedSysMLSupport;
}

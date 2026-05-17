package Sysml2EditorProductBreakdown {
  doc /*
    Source: docs/systems-engineering/product-breakdown.md.
  */

  import Sysml2EditorFunctions::*;

  part def ProductBreakdown {
    // Shell and coordination
    part workbenchShellAndContextChrome : PB_01_WorkbenchShellAndContextChrome;
    part workspaceAndRepositoryCoordinator : PB_02_WorkspaceAndRepositoryCoordinator;

    // Model and inspection
    part modelCoreService : PB_03_ModelCoreService;
    part browsingAndInspectionSurfaces : PB_04_BrowsingAndInspectionSurfaces;

    // Editing and persistence
    part visualEditingSurface : PB_05_VisualEditingSurface;
    part persistenceAndWritePolicyService : PB_06_PersistenceAndWritePolicyService;

    // Git and analysis
    part gitWorkflowService : PB_07_GitWorkflowService;
    part traceabilityAndAnalysisService : PB_08_TraceabilityAndAnalysisService;

    // Views and extensibility
    part savedViewsAndProjectionService : PB_09_SavedViewsAndProjectionService;
    part advancedSysMLCapabilityPack : PB_10_AdvancedSysMLCapabilityPack;
    part sharedContractsAndIdentityPolicy : PB_11_SharedContractsAndIdentityPolicy;
  }

  part def PB_01_WorkbenchShellAndContextChrome {
    doc /* Top app bar, rails, tiled workspace, inspector, status bar, and context labels. */
  }

  part def PB_02_WorkspaceAndRepositoryCoordinator {
    doc /* Repository open/close, branch and worktree tracking, and context write-state selection. */
  }

  part def PB_03_ModelCoreService {
    doc /* Parsing, graph building, stable IDs, source mapping, and core diagnostics. */
  }

  part def PB_04_BrowsingAndInspectionSurfaces {
    doc /* Tree, graph, source, search, inspector, and selection-sync views. */
  }

  part def PB_05_VisualEditingSurface {
    doc /* Type palette, canvas, inline rename, draft staging, and source preview. */
  }

  part def PB_06_PersistenceAndWritePolicyService {
    doc /* Save planning, owner-file resolution, safety checks, and reload after write. */
  }

  part def PB_07_GitWorkflowService {
    doc /* Git status, semantic diff, commit, and merge-preview behavior. */
  }

  part def PB_08_TraceabilityAndAnalysisService {
    doc /* Source ownership, trace matrix, impact analysis, and dependency paths. */
  }

  part def PB_09_SavedViewsAndProjectionService {
    doc /* Save, restore, share, and reopen view projections using stable identity. */
  }

  part def PB_10_AdvancedSysMLCapabilityPack {
    doc /* Later parser, graph, validation, and editor extensions beyond the MVP slice. */
  }

  part def PB_11_SharedContractsAndIdentityPolicy {
    doc /* DTOs, stable IDs, context identifiers, and cross-layer invariants. */
  }

  satisfy FB_01_WorkspaceAndContextManagement by PB_02_WorkspaceAndRepositoryCoordinator;
  satisfy FB_02_ModelIngestionAndIndexing by PB_03_ModelCoreService;
  satisfy FB_03_BrowsingAndSelectionSync by PB_04_BrowsingAndInspectionSurfaces;
  satisfy FB_04_TraceabilityProjection by PB_08_TraceabilityAndAnalysisService;
  satisfy FB_05_EditingAndDrafting by PB_05_VisualEditingSurface;
  satisfy FB_06_WritePolicyAndPersistence by PB_06_PersistenceAndWritePolicyService;
  satisfy FB_07_GitWorkflow by PB_07_GitWorkflowService;
  satisfy FB_08_SavedViewsAndProjections by PB_09_SavedViewsAndProjectionService;
  satisfy FB_09_AnalysisAndQuery by PB_08_TraceabilityAndAnalysisService;
  satisfy FB_10_AdvancedSysMLSupport by PB_10_AdvancedSysMLCapabilityPack;
  satisfy FB_11_DiagnosticsAndRecovery by PB_11_SharedContractsAndIdentityPolicy;
}

package Sysml2EditorProductDefinition {
  doc /*
    Source: PRODUCT_VISION.md and docs/systems-engineering/README.md.
  */

  part def ProductDefinition {
    doc /*
      Product definition derived from PRODUCT_VISION.md and the current
      systems-engineering basis.
    */

    part mission : ProductMission;
    part sourceOfTruth : TextualSysMLSourceOfTruth;
    part workbenchPromise : EngineeringWorkbenchPromise;
    part traceabilityScope : TraceabilityScope;
    part multiContextScope : MultiContextScope;
    part strategicChoice : ReadOnlyBrowserFirst;
  }

  part def ProductMission {
    doc /*
      A Git-native SysML v2 architecture workbench that lets engineers model
      systems visually while preserving textual precision, traceability,
      validation, and version control.
    */
  }

  part def TextualSysMLSourceOfTruth {
    doc /*
      Textual SysML files in Git are the durable source of truth.
      Diagrams, trees, matrices, and saved views are projections.
    */
  }

  part def EngineeringWorkbenchPromise {
    doc /*
      The product should be simple enough for visual architecture creation,
      strict enough to protect SysML semantics, reviewable through Git, and
      traceable across model items, files, branches, repositories, and changes.
    */
  }

  part def TraceabilityScope {
    part itemToItemLinks;
    part itemToFileLinks;
    part fileToFileLinks;
    part branchToBranchLinks;
    part repoToRepoLinks;
  }

  part def MultiContextScope {
    part repositoriesOpenTogether;
    part branchesOpenTogether;
    part worktreesAsWritableContexts;
    part sideBySideComparisons;
    part explicitContextIdentity;
  }

  part def ReadOnlyBrowserFirst {
    doc /*
      Current strategic choice: prove repository access, parsing, source
      mapping, and model projection before exposing writes.
    */
  }
}

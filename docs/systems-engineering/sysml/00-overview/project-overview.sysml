package Sysml2EditorProjectOverview {
  doc /*
    Development overview model for sysml2_editor.

    This model ports selected documentation from docs/architecture,
    docs/functionality, and docs/systems-engineering into a hierarchical
    SysML v2-oriented structure.

    It is not constrained by the current application parser.
  */

  import Sysml2EditorProductDefinition::*;
  import Sysml2EditorUseCases::*;
  import Sysml2EditorFunctions::*;
  import Sysml2EditorProductBreakdown::*;
  import Sysml2EditorArchitecture::*;
  import Sysml2EditorDetailedDesign::*;
  import Sysml2EditorRequirements::*;
  import Sysml2EditorVerification::*;

  part def Sysml2EditorDevelopmentSystem {
    doc /*
      Main breakdown for the product definition, high-level design,
      detailed design, and verification basis.
    */

    part productDefinition : ProductDefinition;
    part useCaseModel : ProductUseCaseModel;
    part productRequirements : ProductRequirementSet;

    part highLevelFunctions : FunctionalBreakdown;
    part highLevelProductStructure : ProductBreakdown;
    part highLevelArchitecture : HighLevelArchitectureDefinition;

    part detailedDesign : DetailedDesignDefinition;
    part verificationDefinition : VerificationInventory;
  }

  part mainBreakdown : Sysml2EditorDevelopmentSystem;

  trace mainBreakdown.productDefinition to mainBreakdown.useCaseModel;
  trace mainBreakdown.useCaseModel to mainBreakdown.productRequirements;
  trace mainBreakdown.productRequirements to mainBreakdown.highLevelFunctions;
  trace mainBreakdown.highLevelFunctions to mainBreakdown.highLevelProductStructure;
  trace mainBreakdown.highLevelProductStructure to mainBreakdown.highLevelArchitecture;
  trace mainBreakdown.highLevelArchitecture to mainBreakdown.detailedDesign;
  trace mainBreakdown.detailedDesign to mainBreakdown.verificationDefinition;
}

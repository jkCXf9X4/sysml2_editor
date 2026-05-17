# Syntax Examples

The examples are derived from the OMG SysML v2 textual notation reference source in [sysml-v2.md](../reference/sysml-v2.md), but they intentionally cover only the MVP subset. They are implementation fixtures, not a claim of full SysML v2 conformance.

Vision trace:

- Supports: precise source-backed model semantics and reviewable fixtures for parser, graph, and writer behavior.
- Tradeoff: uses a narrow syntax subset to prove the visual-to-text workflow before broad language support.

## Supported Identity Metadata

Stable IDs are stored as SysML-native metadata annotations on every editable model element, including packages. File-level imports and relationship edges may use deterministic derived IDs in the initial design.

The MVP parser recognizes this project metadata:

```sysml
metadata def Sysml2EditorIdentity {
  attribute id : String;
}
```

The metadata application form is:

```sysml
@Sysml2EditorIdentity { id = "11111111-1111-4111-8111-111111111111"; }
```

This uses the SysML v2 metadata mechanism instead of tool-owned comments. The metadata definition is project-owned until a standard SysML identity metadata definition exists.

## Minimal Valid Model

```sysml
@Sysml2EditorIdentity { id = "00000000-0000-4000-8000-000000000001"; }
package Vehicle {
  @Sysml2EditorIdentity { id = "11111111-1111-4111-8111-111111111111"; }
  part def BatteryPack;

  @Sysml2EditorIdentity { id = "22222222-2222-4222-8222-222222222222"; }
  part battery : BatteryPack;
}
```

Expected graph:

- Node `Package` named `Vehicle`
- Node `PartDefinition` named `BatteryPack`
- Node `PartUsage` named `battery`
- Edge `Contains` from `Vehicle` to `BatteryPack`
- Edge `Contains` from `Vehicle` to `battery`
- Edge `References` from `battery` to `BatteryPack`

## Supported Import

```sysml
import Power::*;

@Sysml2EditorIdentity { id = "00000000-0000-4000-8000-000000000001"; }
package Vehicle {
  @Sysml2EditorIdentity { id = "33333333-3333-4333-8333-333333333333"; }
  part battery : Power::BatteryPack;
}
```

Expected graph:

- Node `Import` for `Power::*`
- Node `PartUsage` named `battery`
- Edge `Imports` from the file-level import node to `Power::*`
- Edge `References` from `battery` to `Power::BatteryPack`

## Supported Requirement And Satisfy

```sysml
@Sysml2EditorIdentity { id = "00000000-0000-4000-8000-000000000001"; }
package Vehicle {
  @Sysml2EditorIdentity { id = "33333333-3333-4333-8333-333333333333"; }
  requirement def ThermalSafety;

  @Sysml2EditorIdentity { id = "44444444-4444-4444-8444-444444444444"; }
  part def BatteryController;

  satisfy ThermalSafety by BatteryController;
}
```

Expected graph:

- Node `Requirement` named `ThermalSafety`
- Node `PartDefinition` named `BatteryController`
- Edge `Satisfies` from `BatteryController` to `ThermalSafety`

## Supported Port And Connection

```sysml
@Sysml2EditorIdentity { id = "00000000-0000-4000-8000-000000000001"; }
package Vehicle {
  @Sysml2EditorIdentity { id = "11111111-1111-4111-8111-111111111111"; }
  part def BatteryPack {
    @Sysml2EditorIdentity { id = "22222222-2222-4222-8222-222222222222"; }
    port powerOut;
  }

  @Sysml2EditorIdentity { id = "33333333-3333-4333-8333-333333333333"; }
  part def Inverter {
    @Sysml2EditorIdentity { id = "44444444-4444-4444-8444-444444444444"; }
    port powerIn;
  }

  @Sysml2EditorIdentity { id = "55555555-5555-4555-8555-555555555555"; }
  connection powerPath connect BatteryPack::powerOut to Inverter::powerIn;
}
```

Expected graph:

- Node `Port` named `powerOut`
- Node `Port` named `powerIn`
- Node `Connection` named `powerPath`
- Edge `ConnectsTo` from `BatteryPack::powerOut` to `Inverter::powerIn`

## Invalid Syntax

```sysml
@Sysml2EditorIdentity { id = "00000000-0000-4000-8000-000000000001"; }
package Vehicle {
  part def ;
}
```

Expected diagnostic:

- Severity: `Error`
- Message includes missing element name
- File, line, and column are present
- No write operation is allowed for this file

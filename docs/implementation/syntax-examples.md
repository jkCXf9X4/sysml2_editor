# MVP Syntax Examples

These examples define the exact textual subset used by the first implementation slice.

The examples are derived from the OMG SysML v2 textual notation reference source in [spec-reference.md](./spec-reference.md), but they intentionally cover only the MVP subset. They are implementation fixtures, not a claim of full SysML v2 conformance.

## Supported Comment Marker

Stable IDs are stored immediately above supported elements:

```sysml
// sysml2_editor:id: 11111111-1111-4111-8111-111111111111
```

## Minimal Valid Model

```sysml
package Vehicle {
  // sysml2_editor:id: 11111111-1111-4111-8111-111111111111
  part def BatteryPack;

  // sysml2_editor:id: 22222222-2222-4222-8222-222222222222
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

package Vehicle {
  part battery : Power::BatteryPack;
}
```

Expected graph:

- Node `Import` for `Power::*`
- Node `PartUsage` named `battery`
- Edge `Imports` from file or package context to `Power::*`
- Edge `References` from `battery` to `Power::BatteryPack`

## Supported Requirement And Satisfy

```sysml
package Vehicle {
  requirement def ThermalSafety;
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
package Vehicle {
  part def BatteryPack {
    port powerOut;
  }

  part def Inverter {
    port powerIn;
  }

  connection powerPath connect BatteryPack::powerOut to Inverter::powerIn;
}
```

Expected graph:

- Node `Port` named `powerOut`
- Node `Port` named `powerIn`
- Node `Connection` named `powerPath`
- Edge `ConnectsTo` from `BatteryPack::powerOut` to `Inverter::powerIn`

## Invalid MVP Syntax

```sysml
package Vehicle {
  part def ;
}
```

Expected diagnostic:

- Severity: `Error`
- Message includes missing element name
- File, line, and column are present
- No write operation is allowed for this file

## Out-Of-Scope For First Slice

- Behavioral modeling
- State/action semantics
- Full expression parsing
- Type specialization beyond simple references
- Complete SysML v2 grammar coverage


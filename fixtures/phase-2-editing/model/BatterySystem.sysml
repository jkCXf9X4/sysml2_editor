@Sysml2EditorIdentity { id = "00000000-0000-4000-8000-000000000001"; }
package VehiclePlatform {
  @Sysml2EditorIdentity { id = "11111111-1111-4111-8111-111111111111"; }
  part Vehicle {
    @Sysml2EditorIdentity { id = "22222222-2222-4222-8222-222222222222"; }
    part Powertrain;
  }

  @Sysml2EditorIdentity { id = "33333333-3333-4333-8333-333333333333"; }
  part Powertrain {
    @Sysml2EditorIdentity { id = "44444444-4444-4444-8444-444444444444"; }
    part BatterySystem;
  }

  @Sysml2EditorIdentity { id = "55555555-5555-4555-8555-555555555555"; }
  part def BatterySystem {
    attribute capacity: Energy = 75 kWh;
    attribute nominalVoltage: Voltage = 400 V;
    port in coolant: FluidIn;
    port out hvPos: ElectricPowerOut;
    port out hvNeg: ElectricPowerOut;
    part cellArray: CellArray;
    part bms: BMS;
    part cooling: CoolingPlate;
    conn c1: cellArray.hvPos -> hvPos;
    conn c2: cellArray.hvNeg -> hvNeg;
    conn c3: cooling.coolantIn -> coolant;
  }
}
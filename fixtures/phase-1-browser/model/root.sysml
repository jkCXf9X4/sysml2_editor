@Sysml2EditorIdentity { id = "00000000-0000-4000-8000-000000000001"; }
package VehiclePlatform {
  @Sysml2EditorIdentity { id = "11111111-1111-4111-8111-111111111111"; }
  part Vehicle {
    @Sysml2EditorIdentity { id = "22222222-2222-4222-8222-222222222222"; }
    part Powertrain;

    @Sysml2EditorIdentity { id = "33333333-3333-4333-8333-333333333333"; }
    part ThermalSystem;

    @Sysml2EditorIdentity { id = "44444444-4444-4444-8444-444444444444"; }
    part Chassis;

    @Sysml2EditorIdentity { id = "55555555-5555-4555-8555-555555555555"; }
    part Body;

    @Sysml2EditorIdentity { id = "66666666-6666-4666-8666-666666666666"; }
    part Software;
  }

  @Sysml2EditorIdentity { id = "77777777-7777-4777-8777-777777777777"; }
  part Powertrain {
    @Sysml2EditorIdentity { id = "88888888-8888-4888-8888-888888888888"; }
    part BatterySystem;

    @Sysml2EditorIdentity { id = "99999999-9999-4999-8999-999999999999"; }
    part Inverter;

    @Sysml2EditorIdentity { id = "aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa"; }
    part ElectricMotor;
  }

  @Sysml2EditorIdentity { id = "bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbbb"; }
  part def BatterySystem {
    attribute capacity: Energy = 75 kWh;
    attribute nominalVoltage: Voltage = 400 V;
    port in coolant: FluidIn;
    port out hvPos: ElectricPowerOut;
    port out hvNeg: ElectricPowerOut;
  }

  @Sysml2EditorIdentity { id = "cccccccc-cccc-4ccc-8ccc-cccccccccccc"; }
  part def Inverter {
    attribute status: String = "Stable";
    port in dcIn;
    port out acOut;
  }

  @Sysml2EditorIdentity { id = "dddddddd-dddd-4ddd-8ddd-dddddddddddd"; }
  part def ElectricMotor {
    attribute status: String = "Stable";
    port out mechanicalOut;
    port in phaseA;
    port in phaseB;
    port in phaseC;
  }
}
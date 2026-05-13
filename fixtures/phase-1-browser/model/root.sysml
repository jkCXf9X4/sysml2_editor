package VehiclePlatform {
  part Vehicle {
    part Powertrain;
    part ThermalSystem;
    part Chassis;
    part Body;
    part Software;
  }

  part Powertrain {
    part BatterySystem;
    part Inverter;
    part ElectricMotor;
  }

  part def BatterySystem {
    attribute capacity: Energy = 75 kWh;
    attribute nominalVoltage: Voltage = 400 V;
    port in coolant: FluidIn;
    port out hvPos: ElectricPowerOut;
    port out hvNeg: ElectricPowerOut;
  }

  part def Inverter {
    attribute status: String = "Stable";
    port in dcIn;
    port out acOut;
  }

  part def ElectricMotor {
    attribute status: String = "Stable";
    port out mechanicalOut;
    port in phaseA;
    port in phaseB;
    port in phaseC;
  }
}

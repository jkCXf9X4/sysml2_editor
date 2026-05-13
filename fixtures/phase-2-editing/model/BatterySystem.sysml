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

import { useState, type ReactNode } from 'react';

type Tone = 'accent' | 'success' | 'warning' | 'muted' | 'danger';
type PaneMode = 'Visual' | 'Text' | 'Split';
type PaneKey = 'architecture' | 'comparison' | 'source' | 'diff';

type ContextCard = {
  id: string;
  repository: string;
  branch: string;
  path: string;
  status: string;
  tone: Tone;
};

type RepositoryCard = {
  name: string;
  scope: string;
  activeBranch: string;
  branches: string[];
  tone: Tone;
};

type TreeRow = {
  id?: string;
  label: string;
  kind: string;
  depth: number;
  selected?: boolean;
  selectable?: boolean;
};

type PaletteGroup = {
  title: string;
  items: string[];
};

type PaneSpec = {
  key: PaneKey;
  title: string;
  context: string;
  summary: string;
  tone: Tone;
  kind: 'architecture' | 'comparison' | 'source' | 'diff';
};

const topMenus = ['File', 'Edit', 'View', 'Navigate', 'Model', 'Tools', 'Window', 'Help'] as const;

const openContexts: ContextCard[] = [
  {
    id: 'vehicle-main',
    repository: 'vehicle-platform',
    branch: 'main',
    path: 'model/root.sysml',
    status: 'Writable',
    tone: 'success',
  },
  {
    id: 'vehicle-concept',
    repository: 'vehicle-platform',
    branch: 'concept-ev',
    path: 'model/root.sysml',
    status: 'Comparison',
    tone: 'accent',
  },
  {
    id: 'supplier-dev',
    repository: 'supplier-bms',
    branch: 'dev',
    path: 'model/bms.sysml',
    status: 'Writable',
    tone: 'warning',
  },
];

const repositories: RepositoryCard[] = [
  {
    name: 'vehicle-platform',
    scope: 'local',
    activeBranch: 'main',
    branches: ['main', 'concept-ev', 'feature/thermal'],
    tone: 'success',
  },
  {
    name: 'supplier-bms',
    scope: 'local',
    activeBranch: 'dev',
    branches: ['main', 'dev'],
    tone: 'warning',
  },
  {
    name: 'cloud-sensor-suite',
    scope: '@github.com',
    activeBranch: 'main',
    branches: ['main', 'release/v2.1'],
    tone: 'accent',
  },
];

const modelTree: TreeRow[] = [
  { id: 'vehicle', label: 'VehiclePlatform', kind: 'Package', depth: 0, selectable: true },
  { label: '1 Requirements', kind: 'Section', depth: 1 },
  { label: '2 System Architecture', kind: 'Section', depth: 1 },
  { id: 'vehicle', label: '2.1 Vehicle', kind: 'Part', depth: 2, selectable: true },
  { id: 'powertrain', label: '2.1.1 Powertrain', kind: 'Part Definition', depth: 3, selectable: true },
  { id: 'battery-system', label: 'BatterySystem', kind: 'Part Definition', depth: 4, selected: true, selectable: true },
  { id: 'inverter', label: 'Inverter', kind: 'Part Definition', depth: 4, selectable: true },
  { id: 'electric-motor', label: 'ElectricMotor', kind: 'Part Definition', depth: 4, selectable: true },
  { label: '2.1.2 ThermalSystem', kind: 'Part', depth: 3 },
  { label: '2.1.3 Chassis', kind: 'Part', depth: 2 },
  { label: '2.1.4 Body', kind: 'Part', depth: 2 },
  { label: '2.1.5 Software', kind: 'Part', depth: 2 },
  { label: '3 Interfaces', kind: 'Section', depth: 1 },
  { label: '4 Verification', kind: 'Section', depth: 1 },
];

const paletteGroups: PaletteGroup[] = [
  {
    title: 'Structure',
    items: ['Part', 'Part Definition', 'Package', 'View', 'Viewpoint'],
  },
  {
    title: 'Ports & Interfaces',
    items: ['Port', 'Interface', 'Flow', 'Connection'],
  },
  {
    title: 'Behavior',
    items: ['Action', 'State', 'Constraint', 'Allocation'],
  },
  {
    title: 'Traceability',
    items: ['Requirement', 'Satisfy', 'Verify', 'Refine'],
  },
];

const paneSpecs: PaneSpec[] = [
  {
    key: 'architecture',
    title: 'System Architecture',
    context: 'vehicle-platform / main',
    summary: 'Writable, source-backed architecture projection',
    tone: 'success',
    kind: 'architecture',
  },
  {
    key: 'comparison',
    title: 'Branch Comparison',
    context: 'vehicle-platform / concept-ev',
    summary: 'Read-only comparison projection',
    tone: 'accent',
    kind: 'comparison',
  },
  {
    key: 'source',
    title: 'SysML Text',
    context: 'supplier-bms / dev',
    summary: 'Textual source view with owning file',
    tone: 'warning',
    kind: 'source',
  },
  {
    key: 'diff',
    title: 'Diff',
    context: 'vehicle-platform / main ↔ concept-ev',
    summary: 'Semantic and textual change summary',
    tone: 'danger',
    kind: 'diff',
  },
];

const inspectorAttributes = [
  ['capacity', 'Energy = 75 kWh'],
  ['nominalVoltage', 'Voltage = 400 V'],
  ['status', 'Production'],
  ['allocation', 'Powertrain::BatterySystem'],
 ] as const;

const inspectorPorts = ['coolant: FluidIn', 'hvPos: ElectricPowerOut', 'hvNeg: ElectricPowerOut'] as const;
const inspectorParts = ['cellArray: CellArray', 'bms: BMS', 'cooling: CoolingPlate'] as const;
const inspectorConnections = [
  'c1: cellArray.hvPos -> hvPos',
  'c2: cellArray.hvNeg -> hvNeg',
  'c3: cooling.coolantIn -> coolant',
 ] as const;
const traceabilityRows = [
  ['Satisfies', 'REQ-0021 Battery capacity'],
  ['Verifies', 'TEST-014 HV functional test'],
  ['Allocated from', 'SYS-001 Powertrain system'],
 ] as const;
const tags = ['thermal', 'safety', 'power'] as const;
const sourceLines = [
  'package VehiclePlatform::2.1::Vehicle::2.1.1::Powertrain {',
  '  part def BatterySystem {',
  '    attribute capacity: Energy = 75 kWh;',
  '    attribute nominalVoltage: Voltage = 400 V;',
  '',
  '    port in coolant: FluidIn;',
  '    port out hvPos: ElectricPowerOut;',
  '    port out hvNeg: ElectricPowerOut;',
  '',
  '    part def CellArray;',
  '    part def BMS;',
  '    part def CoolingPlate;',
  '',
  '    part cellArray: CellArray;',
  '    part bms: BMS;',
  '    part cooling: CoolingPlate;',
  '',
  '    conn c1: cellArray.hvPos -> hvPos;',
  '    conn c2: cellArray.hvNeg -> hvNeg;',
  '    conn c3: cooling.coolantIn -> coolant;',
  '  }',
  '}',
];

const diffLines = [
  { kind: 'context', text: 'part def BatterySystem {' },
  { kind: 'removed', text: '  attribute capacity: Energy = 75 kWh;' },
  { kind: 'added', text: '  attribute capacity: Energy = 95 kWh;' },
  { kind: 'context', text: '  port in coolant: FluidIn;' },
  { kind: 'context', text: '  port out hvPos: ElectricPowerOut;' },
  { kind: 'context', text: '  port out hvNeg: ElectricPowerOut;' },
  { kind: 'context', text: '  part cellArray: CellArray;' },
  { kind: 'context', text: '  part bms: BMS;' },
  { kind: 'added', text: '  part insulation: ThermalBarrier;' },
  { kind: 'context', text: '  conn c3: cooling.coolantIn -> coolant;' },
  { kind: 'context', text: '}' },
];

const diffBefore = [
  'part def BatterySystem {',
  '  attribute capacity: Energy = 75 kWh;',
  '  attribute nominalVoltage: Voltage = 400 V;',
  '  port in coolant: FluidIn;',
  '  port out hvPos: ElectricPowerOut;',
  '  port out hvNeg: ElectricPowerOut;',
  '  part cellArray: CellArray;',
  '  part bms: BMS;',
  '  conn c3: cooling.coolantIn -> coolant;',
  '}',
];

const diffAfter = [
  'part def BatterySystem {',
  '  attribute capacity: Energy = 95 kWh;',
  '  attribute nominalVoltage: Voltage = 400 V;',
  '  port in coolant: FluidIn;',
  '  port out hvPos: ElectricPowerOut;',
  '  port out hvNeg: ElectricPowerOut;',
  '  part cellArray: CellArray;',
  '  part bms: BMS;',
  '  part insulation: ThermalBarrier;',
  '  conn c3: cooling.coolantIn -> coolant;',
  '}',
];

const statusPills = [
  ['3 modified files', 'success'],
  ['1 added', 'accent'],
  ['0 deleted', 'muted'],
  ['Model is valid', 'warning'],
] as const;

type BrowserModel = {
  id: string;
  label: string;
  kind: string;
  repository: string;
  branch: string;
  owner: string;
  file: string;
  qualifiedName: string;
  summary: string;
  attributes: readonly (readonly [string, string])[];
  ports: readonly string[];
  parts: readonly string[];
  connections: readonly string[];
  traces: readonly (readonly [string, string])[];
  sourceLines: readonly string[];
  comparisonSummary: string;
  comparisonLines: readonly string[];
  diffLines: readonly { kind: 'context' | 'added' | 'removed'; text: string }[];
};

const browserModels: Record<string, BrowserModel> = {
  vehicle: {
    id: 'vehicle',
    label: 'Vehicle',
    kind: 'Package',
    repository: 'vehicle-platform',
    branch: 'main',
    owner: 'VehiclePlatform',
    file: 'model/root.sysml',
    qualifiedName: 'VehiclePlatform::Vehicle',
    summary: 'Top-level system package for the vehicle platform.',
    attributes: [
      ['scope', 'Platform root'],
      ['status', 'Active'],
    ],
    ports: [],
    parts: ['Powertrain', 'ThermalSystem', 'Chassis', 'Body', 'Software'],
    connections: ['contains Powertrain', 'contains ThermalSystem', 'contains Chassis'],
    traces: [
      ['Defined in', 'model/root.sysml'],
      ['Referenced by', 'Powertrain package'],
    ],
    sourceLines: [
      'package VehiclePlatform {',
      '  package Vehicle {',
      '    part Powertrain;',
      '    part ThermalSystem;',
      '    part Chassis;',
      '    part Body;',
      '    part Software;',
      '  }',
      '}',
    ],
    comparisonSummary: 'The root package is unchanged between branches.',
    comparisonLines: [
      'VehiclePlatform / main',
      '  package Vehicle is retained',
      '',
      'VehiclePlatform / concept-ev',
      '  package Vehicle is retained',
    ],
    diffLines: [
      { kind: 'context', text: 'package VehiclePlatform {' },
      { kind: 'context', text: '  package Vehicle {' },
      { kind: 'context', text: '    part Powertrain;' },
      { kind: 'context', text: '    part ThermalSystem;' },
      { kind: 'context', text: '    part Chassis;' },
      { kind: 'context', text: '    part Body;' },
      { kind: 'context', text: '    part Software;' },
      { kind: 'context', text: '  }' },
      { kind: 'context', text: '}' },
    ],
  },
  powertrain: {
    id: 'powertrain',
    label: 'Powertrain',
    kind: 'Part',
    repository: 'vehicle-platform',
    branch: 'main',
    owner: 'Vehicle',
    file: 'model/2_SystemArchitecture/2.1_Vehicle/2.1.1_Powertrain/Powertrain.sysml',
    qualifiedName: 'VehiclePlatform::Vehicle::Powertrain',
    summary: 'Top-level part that groups the electric powertrain subsystems.',
    attributes: [
      ['mode', 'Electric'],
      ['status', 'Read-only in comparison'],
    ],
    ports: ['hvPos: ElectricPowerOut', 'hvNeg: ElectricPowerOut'],
    parts: ['BatterySystem', 'Inverter', 'ElectricMotor'],
    connections: ['connects BatterySystem to Inverter', 'connects Inverter to ElectricMotor'],
    traces: [
      ['Defined in', 'Powertrain.sysml'],
      ['Related to', 'vehicle/root.sysml'],
    ],
    sourceLines: [
      'part Powertrain {',
      '  part BatterySystem;',
      '  part Inverter;',
      '  part ElectricMotor;',
      '  port out hvPos: ElectricPowerOut;',
      '  port out hvNeg: ElectricPowerOut;',
      '}',
    ],
    comparisonSummary: 'Powertrain receives a thermal trace refinement in concept-ev.',
    comparisonLines: [
      'Powertrain / main',
      '  BatterySystem and Inverter are unchanged',
      '',
      'Powertrain / concept-ev',
      '  BatterySystem remains, coolant trace is explicit',
    ],
    diffLines: [
      { kind: 'context', text: 'part Powertrain {' },
      { kind: 'context', text: '  part BatterySystem;' },
      { kind: 'context', text: '  part Inverter;' },
      { kind: 'added', text: '  part ThermalMonitor;' },
      { kind: 'context', text: '  port out hvPos: ElectricPowerOut;' },
      { kind: 'context', text: '}' },
    ],
  },
  'battery-system': {
    id: 'battery-system',
    label: 'BatterySystem',
    kind: 'Part Definition',
    repository: 'vehicle-platform',
    branch: 'main',
    owner: 'Powertrain',
    file: 'model/2_SystemArchitecture/2.1_Vehicle/2.1.1_Powertrain/BatterySystem.sysml',
    qualifiedName: 'VehiclePlatform::Vehicle::Powertrain::BatterySystem',
    summary: 'Thermal and power interface for the electric powertrain.',
    attributes: [
      ['capacity', 'Energy = 75 kWh'],
      ['nominalVoltage', 'Voltage = 400 V'],
      ['status', 'Production'],
      ['allocation', 'Powertrain::BatterySystem'],
    ],
    ports: ['coolant: FluidIn', 'hvPos: ElectricPowerOut', 'hvNeg: ElectricPowerOut'],
    parts: ['cellArray: CellArray', 'bms: BMS', 'cooling: CoolingPlate'],
    connections: [
      'c1: cellArray.hvPos -> hvPos',
      'c2: cellArray.hvNeg -> hvNeg',
      'c3: cooling.coolantIn -> coolant',
    ],
    traces: [
      ['Satisfies', 'REQ-0021 Battery capacity'],
      ['Verifies', 'TEST-014 HV functional test'],
      ['Allocated from', 'SYS-001 Powertrain system'],
    ],
    sourceLines: [
      'part def BatterySystem {',
      '  attribute capacity: Energy = 75 kWh;',
      '  attribute nominalVoltage: Voltage = 400 V;',
      '  port in coolant: FluidIn;',
      '  port out hvPos: ElectricPowerOut;',
      '  port out hvNeg: ElectricPowerOut;',
      '  part cellArray: CellArray;',
      '  part bms: BMS;',
      '  part cooling: CoolingPlate;',
      '  conn c1: cellArray.hvPos -> hvPos;',
      '  conn c2: cellArray.hvNeg -> hvNeg;',
      '  conn c3: cooling.coolantIn -> coolant;',
      '}',
    ],
    comparisonSummary: 'Battery capacity increases and charging support is added.',
    comparisonLines: [
      'VehiclePlatform / main',
      '  capacity = 75 kWh',
      '  no onboard charger',
      '',
      'VehiclePlatform / concept-ev',
      '  capacity = 95 kWh',
      '  onboard charger added',
    ],
    diffLines: [
      { kind: 'context', text: 'part def BatterySystem {' },
      { kind: 'removed', text: '  attribute capacity: Energy = 75 kWh;' },
      { kind: 'added', text: '  attribute capacity: Energy = 95 kWh;' },
      { kind: 'context', text: '  port in coolant: FluidIn;' },
      { kind: 'context', text: '  port out hvPos: ElectricPowerOut;' },
      { kind: 'context', text: '  port out hvNeg: ElectricPowerOut;' },
      { kind: 'context', text: '  part cellArray: CellArray;' },
      { kind: 'context', text: '  part bms: BMS;' },
      { kind: 'added', text: '  part insulation: ThermalBarrier;' },
      { kind: 'context', text: '  conn c3: cooling.coolantIn -> coolant;' },
      { kind: 'context', text: '}' },
    ],
  },
  inverter: {
    id: 'inverter',
    label: 'Inverter',
    kind: 'Part Definition',
    repository: 'vehicle-platform',
    branch: 'main',
    owner: 'Powertrain',
    file: 'model/2_SystemArchitecture/2.1_Vehicle/2.1.1_Powertrain/Inverter.sysml',
    qualifiedName: 'VehiclePlatform::Vehicle::Powertrain::Inverter',
    summary: 'Power conversion assembly for the traction path.',
    attributes: [
      ['status', 'Stable'],
      ['voltage', '400 V class'],
    ],
    ports: ['dcIn', 'acOut'],
    parts: ['GateDriver', 'PowerStage'],
    connections: ['dcIn feeds PowerStage', 'GateDriver controls PowerStage'],
    traces: [
      ['Defined in', 'Inverter.sysml'],
      ['Referenced by', 'BatterySystem'],
    ],
    sourceLines: [
      'part def Inverter {',
      '  attribute status: String = "Stable";',
      '  port in dcIn;',
      '  port out acOut;',
      '  part GateDriver;',
      '  part PowerStage;',
      '}',
    ],
    comparisonSummary: 'Inverter remains unchanged in the comparison branch.',
    comparisonLines: [
      'VehiclePlatform / main',
      '  inverter is unchanged',
      '',
      'VehiclePlatform / concept-ev',
      '  inverter is unchanged',
    ],
    diffLines: [
      { kind: 'context', text: 'part def Inverter {' },
      { kind: 'context', text: '  attribute status: String = "Stable";' },
      { kind: 'context', text: '  port in dcIn;' },
      { kind: 'context', text: '  port out acOut;' },
      { kind: 'context', text: '}' },
    ],
  },
  'electric-motor': {
    id: 'electric-motor',
    label: 'ElectricMotor',
    kind: 'Part Definition',
    repository: 'vehicle-platform',
    branch: 'main',
    owner: 'Powertrain',
    file: 'model/2_SystemArchitecture/2.1_Vehicle/2.1.1_Powertrain/ElectricMotor.sysml',
    qualifiedName: 'VehiclePlatform::Vehicle::Powertrain::ElectricMotor',
    summary: 'Traction motor and rotor assembly.',
    attributes: [
      ['status', 'Stable'],
      ['phaseCount', '3'],
    ],
    ports: ['mechanicalOut', 'phaseA', 'phaseB', 'phaseC'],
    parts: ['Rotor', 'Stator'],
    connections: ['phaseA, phaseB, phaseC drive the stator'],
    traces: [
      ['Defined in', 'ElectricMotor.sysml'],
      ['Referenced by', 'Powertrain'],
    ],
    sourceLines: [
      'part def ElectricMotor {',
      '  attribute status: String = "Stable";',
      '  port out mechanicalOut;',
      '  port in phaseA;',
      '  port in phaseB;',
      '  port in phaseC;',
      '  part Rotor;',
      '  part Stator;',
      '}',
    ],
    comparisonSummary: 'Electric motor stays stable across both branches.',
    comparisonLines: [
      'VehiclePlatform / main',
      '  electric motor is unchanged',
      '',
      'VehiclePlatform / concept-ev',
      '  electric motor is unchanged',
    ],
    diffLines: [
      { kind: 'context', text: 'part def ElectricMotor {' },
      { kind: 'context', text: '  attribute status: String = "Stable";' },
      { kind: 'context', text: '  port out mechanicalOut;' },
      { kind: 'context', text: '  part Rotor;' },
      { kind: 'context', text: '  part Stator;' },
      { kind: 'context', text: '}' },
    ],
  },
};

type BrowserModelId = keyof typeof browserModels;

const browserModelOrder = ['vehicle', 'powertrain', 'battery-system', 'inverter', 'electric-motor'] as const;

const primaryWorkspaceStats = [
  ['Workspace', 'workspace-main'],
  ['Repository', 'vehicle-platform'],
  ['Branch', 'main'],
  ['Cursor', 'Ln 12, Col 28'],
  ['Encoding', 'UTF-8'],
  ['Dialect', 'SysML v2'],
] as const;

function App() {
  const [paneModes, setPaneModes] = useState<Record<PaneKey, PaneMode>>({
    architecture: 'Visual',
    comparison: 'Visual',
    source: 'Text',
    diff: 'Split',
  });
  const [selectedModelId, setSelectedModelId] = useState<BrowserModelId>('battery-system');
  const [treeQuery, setTreeQuery] = useState('');

  const setMode = (key: PaneKey, mode: PaneMode) => {
    setPaneModes((current) => ({ ...current, [key]: mode }));
  };
  const selectedModel = browserModels[selectedModelId] ?? browserModels['battery-system'];
  const visibleTree = modelTree.filter((node) => {
    const query = treeQuery.trim().toLowerCase();
    if (!query) {
      return true;
    }

    return node.label.toLowerCase().includes(query) || node.kind.toLowerCase().includes(query);
  });

  return (
    <div className="shell">
      <header className="topbar">
        <div className="topbar-row topbar-row-menu" aria-label="Application menu">
          <div className="brand">
            <div className="brand-mark">◇</div>
            <div>
              <div className="eyebrow">Workbench</div>
              <strong>sysml2_editor</strong>
            </div>
          </div>

          <nav className="menu-strip" aria-label="Main menu">
            {topMenus.map((item) => (
              <button key={item} type="button" className="menu-button">
                {item}
              </button>
            ))}
          </nav>
        </div>

        <div className="topbar-row topbar-row-tools">
          <div className="context-strip" aria-label="Open contexts">
            {openContexts.map((context) => (
              <ContextPill
                key={context.id}
                label={`${context.repository}/${context.branch}`}
                tone={context.tone}
                detail={context.status}
              />
            ))}
          </div>

          <div className="command-strip">
            <Selector label="Repositories" value="4 selected" />
            <Selector label="Compare" value="vehicle-platform / main ↔ vehicle-platform / concept-ev" />
            <IconCommand label="Commit" icon="↥" />
            <IconCommand label="Pull" icon="↧" />
            <IconCommand label="Push" icon="⇡" />
            <StatusIndicator />
            <IconCommand label="Search" icon="⌕" />
            <IconCommand label="Layout" icon="▦" />
            <IconCommand label="Settings" icon="⚙" />
          </div>
        </div>
      </header>

      <main className="workspace">
        <aside className="rail rail-left">
          <section className="rail-section">
            <SectionHeader title="Repositories" meta="3 open" />
            <input className="search-box" type="search" placeholder="Search repositories..." />
            <div className="repo-list">
              {repositories.map((repo) => (
                <article key={repo.name} className="repo-card">
                  <div className="repo-card__header">
                    <div>
                      <strong>{repo.name}</strong>
                      <span>{repo.scope}</span>
                    </div>
                    <ContextPill label={repo.activeBranch} tone={repo.tone} detail="active" />
                  </div>
                  <div className="branch-chips" aria-label={`${repo.name} branches`}>
                    {repo.branches.map((branch) => (
                      <span key={branch} className={`branch-chip ${branch === repo.activeBranch ? 'is-active' : ''}`}>
                        {branch}
                      </span>
                    ))}
                  </div>
                </article>
              ))}
            </div>
          </section>

          <section className="rail-section">
            <SectionHeader title="Model Tree" meta={selectedModel.owner} />
            <input
              className="search-box"
              type="search"
              placeholder="Search model..."
              value={treeQuery}
              onChange={(event) => setTreeQuery(event.target.value)}
            />
            <div className="tree-list" role="tree" aria-label="Model tree">
              {visibleTree.map((node) => {
                const isSelected = node.id ? node.id === selectedModelId : Boolean(node.selected);
                return (
                  <button
                    key={`${node.depth}-${node.label}`}
                    type="button"
                    className={`tree-row ${isSelected ? 'is-selected' : ''}`}
                    style={{ paddingLeft: `${12 + node.depth * 14}px` }}
                    onClick={() => {
                      if (node.id) {
                        setSelectedModelId(node.id);
                      }
                    }}
                  >
                    <span className="tree-row__kind">{node.kind}</span>
                    <span className="tree-row__label">{node.label}</span>
                  </button>
                );
              })}
            </div>
          </section>

          <section className="rail-section">
            <SectionHeader title="Type Palette" meta="SysML types" />
            <input className="search-box" type="search" placeholder="Search types..." />
            <div className="palette-list">
              {paletteGroups.map((group) => (
                <div key={group.title} className="palette-group">
                  <div className="palette-group__title">{group.title}</div>
                  <div className="palette-group__items">
                    {group.items.map((item) => (
                      <button key={item} type="button" className="type-chip">
                        {item}
                      </button>
                    ))}
                  </div>
                </div>
              ))}
            </div>
          </section>
        </aside>

        <section className="center-grid" aria-label="Workbench panes">
          <WorkbenchPane
            spec={paneSpecs[0]}
            mode={paneModes.architecture}
            onModeChange={(mode) => setMode('architecture', mode)}
          >
            <PaneBody
              kind="architecture"
              mode={paneModes.architecture}
              model={selectedModel}
              onSelectModel={setSelectedModelId}
            />
          </WorkbenchPane>

          <WorkbenchPane
            spec={paneSpecs[1]}
            mode={paneModes.comparison}
            onModeChange={(mode) => setMode('comparison', mode)}
          >
            <PaneBody
              kind="comparison"
              mode={paneModes.comparison}
              model={selectedModel}
              onSelectModel={setSelectedModelId}
            />
          </WorkbenchPane>

          <WorkbenchPane
            spec={paneSpecs[2]}
            mode={paneModes.source}
            onModeChange={(mode) => setMode('source', mode)}
          >
            <PaneBody
              kind="source"
              mode={paneModes.source}
              model={selectedModel}
              onSelectModel={setSelectedModelId}
            />
          </WorkbenchPane>

          <WorkbenchPane
            spec={paneSpecs[3]}
            mode={paneModes.diff}
            onModeChange={(mode) => setMode('diff', mode)}
          >
            <PaneBody
              kind="diff"
              mode={paneModes.diff}
              model={selectedModel}
              onSelectModel={setSelectedModelId}
            />
          </WorkbenchPane>
        </section>

        <aside className="rail rail-right">
          <section className="inspector">
            <SectionHeader title="Inspector" meta={selectedModel.label} />
            <div className="inspector__name">
              <div className="inspector__title">{selectedModel.label}</div>
              <div className="inspector__subtitle">{`<<${selectedModel.kind.toLowerCase()}>>`}</div>
            </div>

            <dl className="detail-grid">
              <div>
                <dt>Qualified Name</dt>
                <dd>{selectedModel.qualifiedName}</dd>
              </div>
              <div>
                <dt>Owner</dt>
                <dd>{selectedModel.owner}</dd>
              </div>
              <div>
                <dt>File</dt>
                <dd>{selectedModel.file}</dd>
              </div>
              <div>
                <dt>Documentation</dt>
                <dd>{selectedModel.summary}</dd>
              </div>
            </dl>

            <InspectorSection title="Attributes">
              <KeyValueList rows={selectedModel.attributes} />
            </InspectorSection>

            <InspectorSection title="Ports">
              <ChipList items={selectedModel.ports} tone="accent" />
            </InspectorSection>

            <InspectorSection title="Parts">
              <ChipList items={selectedModel.parts} tone="success" />
            </InspectorSection>

            <InspectorSection title="Connections">
              <TraceList rows={selectedModel.connections} />
            </InspectorSection>

            <InspectorSection title="Traceability">
              <TraceList rows={selectedModel.traces} />
            </InspectorSection>

            <InspectorSection title="Source">
              <KeyValueList rows={[['Repository', selectedModel.repository], ['Branch', selectedModel.branch]] as const} />
            </InspectorSection>

            <InspectorSection title="Lifecycle">
              <KeyValueList rows={[['Last modified', 'Today, 10:24 AM'], ['Author', 'alex']] as const} />
            </InspectorSection>
          </section>
        </aside>
      </main>

      <footer className="statusbar" aria-label="Workspace status">
        <div className="statusbar__group">
          <span className="status-dot" />
          <strong>Ready</strong>
        </div>
        <div className="statusbar__group statusbar__group--wrap">
          {primaryWorkspaceStats.map(([label, value]) => (
            <ContextPill key={label} label={`${label}: ${value}`} tone="muted" />
          ))}
        </div>
        <div className="statusbar__group statusbar__group--wrap">
          {statusPills.map(([label, tone]) => (
            <ContextPill key={label} label={label} tone={tone} />
          ))}
        </div>
      </footer>
    </div>
  );
}

function SectionHeader({ title, meta }: { title: string; meta: string }) {
  return (
    <div className="section-header">
      <div>
        <div className="eyebrow">{title}</div>
        <strong>{meta}</strong>
      </div>
      <span className="section-header__mark" aria-hidden="true">
        ⋯
      </span>
    </div>
  );
}

function ContextPill({ label, tone, detail }: { label: string; tone: Tone; detail?: string }) {
  return (
    <span className={`chip chip--${tone}`}>
      <span>{label}</span>
      {detail ? <small>{detail}</small> : null}
    </span>
  );
}

function Selector({ label, value }: { label: string; value: string }) {
  return (
    <button type="button" className="selector">
      <span className="selector__label">{label}</span>
      <span className="selector__value">{value}</span>
    </button>
  );
}

function IconCommand({ label, icon }: { label: string; icon: string }) {
  return (
    <button type="button" className="icon-command" aria-label={label} title={label}>
      <span aria-hidden="true">
        {icon}
      </span>
    </button>
  );
}

function StatusIndicator() {
  return (
    <button type="button" className="status-indicator" aria-label="Status">
      <span className="status-indicator__dot" />
      <span>Status</span>
    </button>
  );
}

function WorkbenchPane({
  spec,
  mode,
  onModeChange,
  children,
}: {
  spec: PaneSpec;
  mode: PaneMode;
  onModeChange: (mode: PaneMode) => void;
  children: ReactNode;
}) {
  return (
    <article className={`pane pane--${spec.tone}`}>
      <header className="pane__header">
        <div className="pane__header-copy">
          <div className="pane__title-row">
            <strong>{spec.title}</strong>
            <span className={`pane__status pane__status--${spec.tone}`}>{spec.summary}</span>
          </div>
          <div className="pane__context">{spec.context}</div>
        </div>
        <div className="pane__controls">
          <div className="pane__tabs" role="tablist" aria-label={`${spec.title} view mode`}>
            {(['Visual', 'Text', 'Split'] as const).map((item) => (
              <button
                key={item}
                type="button"
                className={`pane__tab ${mode === item ? 'is-active' : ''}`}
                aria-pressed={mode === item}
                onClick={() => onModeChange(item)}
              >
                {item}
              </button>
            ))}
          </div>
          <span className={`pane__tag pane__tag--${spec.tone}`}>{spec.kind}</span>
        </div>
      </header>
      <div className="pane__body">{children}</div>
    </article>
  );
}

function PaneBody({
  kind,
  mode,
  model,
  onSelectModel,
}: {
  kind: PaneSpec['kind'];
  mode: PaneMode;
  model: BrowserModel;
  onSelectModel: (id: BrowserModelId) => void;
}) {
  if (kind === 'architecture') {
    return (
      <div className={`surface surface--${mode.toLowerCase()}`}>
        {mode === 'Visual' ? <ArchitectureDiagram model={model} onSelectModel={onSelectModel} /> : null}
        {mode === 'Text' ? <CodeEditor title={model.file} lines={model.sourceLines} /> : null}
        {mode === 'Split' ? <SplitArchitecture model={model} onSelectModel={onSelectModel} /> : null}
      </div>
    );
  }

  if (kind === 'comparison') {
    return (
      <div className={`surface surface--${mode.toLowerCase()}`}>
        {mode === 'Visual' ? <ComparisonDiagram model={model} onSelectModel={onSelectModel} /> : null}
        {mode === 'Text' ? <ComparisonText model={model} /> : null}
        {mode === 'Split' ? <SplitComparison model={model} onSelectModel={onSelectModel} /> : null}
      </div>
    );
  }

  if (kind === 'source') {
    return (
      <div className={`surface surface--${mode.toLowerCase()}`}>
        {mode === 'Visual' ? <SourceTraceMap model={model} onSelectModel={onSelectModel} /> : null}
        {mode === 'Text' ? <CodeEditor title={model.file} lines={model.sourceLines} /> : null}
        {mode === 'Split' ? <SplitSource model={model} onSelectModel={onSelectModel} /> : null}
      </div>
    );
  }

  return (
    <div className={`surface surface--${mode.toLowerCase()}`}>
      {mode === 'Visual' ? <DiffSummary model={model} onSelectModel={onSelectModel} /> : null}
      {mode === 'Text' ? <DiffText model={model} /> : null}
      {mode === 'Split' ? <DiffSplit model={model} onSelectModel={onSelectModel} /> : null}
    </div>
  );
}

function ArchitectureDiagram({
  model,
  onSelectModel,
}: {
  model: BrowserModel;
  onSelectModel: (id: BrowserModelId) => void;
}) {
  return (
    <div className="diagram diagram--architecture">
      <DiagramNode
        label={browserModels.vehicle.label}
        kind={browserModels.vehicle.kind}
        tone="muted"
        wide
        selected={model.id === 'vehicle'}
        onClick={() => onSelectModel('vehicle')}
      />
      <DiagramConnector label="contains" />
      <div className="diagram__row">
        <DiagramNode
          label={browserModels.powertrain.label}
          kind={browserModels.powertrain.kind}
          tone="accent"
          selected={model.id === 'powertrain'}
          onClick={() => onSelectModel('powertrain')}
        />
        <DiagramNode label="ThermalSystem" kind="Part" tone="warning" />
        <DiagramNode label="Chassis" kind="Part" tone="muted" />
        <DiagramNode label="Body" kind="Part" tone="muted" />
        <DiagramNode label="Software" kind="Part" tone="muted" />
      </div>
      <DiagramConnector label="selected" />
      <div className="diagram__stack">
        <DiagramNode
          label={browserModels['battery-system'].label}
          kind={browserModels['battery-system'].kind}
          tone="success"
          selected={model.id === 'battery-system'}
          onClick={() => onSelectModel('battery-system')}
        />
        <DiagramNode
          label={browserModels.inverter.label}
          kind={browserModels.inverter.kind}
          tone="muted"
          selected={model.id === 'inverter'}
          onClick={() => onSelectModel('inverter')}
        />
        <DiagramNode
          label={browserModels['electric-motor'].label}
          kind={browserModels['electric-motor'].kind}
          tone="muted"
          selected={model.id === 'electric-motor'}
          onClick={() => onSelectModel('electric-motor')}
        />
      </div>
      <div className="diagram__legend">
        <span className="diagram__legend-item">unchanged</span>
        <span className="diagram__legend-item diagram__legend-item--added">added</span>
        <span className="diagram__legend-item diagram__legend-item--removed">removed</span>
        <span className="diagram__legend-item diagram__legend-item--changed">changed</span>
      </div>
    </div>
  );
}

function ComparisonDiagram({
  model,
  onSelectModel,
}: {
  model: BrowserModel;
  onSelectModel: (id: BrowserModelId) => void;
}) {
  return (
    <div className="diagram diagram--comparison">
      <DiagramNode
        label={browserModels.vehicle.label}
        kind={browserModels.vehicle.kind}
        tone="muted"
        wide
        selected={model.id === 'vehicle'}
        onClick={() => onSelectModel('vehicle')}
      />
      <DiagramConnector label="branch delta" />
      <div className="diagram__row">
        <DiagramNode
          label={browserModels.powertrain.label}
          kind={browserModels.powertrain.kind}
          tone="accent"
          selected={model.id === 'powertrain'}
          onClick={() => onSelectModel('powertrain')}
        />
        <DiagramNode
          label={browserModels['battery-system'].label}
          kind={browserModels['battery-system'].kind}
          tone="warning"
          selected={model.id === 'battery-system'}
          onClick={() => onSelectModel('battery-system')}
        />
        <DiagramNode label="Chassis" kind="Part" tone="muted" />
        <DiagramNode label="Body" kind="Part" tone="muted" />
      </div>
      <DiagramConnector label="concept-ev adds" />
      <div className="diagram__stack">
        <DiagramNode label="OnboardCharger" kind="Part Definition" tone="accent" />
        <DiagramNode label="DC-DCConverter" kind="Part Definition" tone="accent" />
        <DiagramNode label="coolant" kind="flow port" tone="warning" />
      </div>
    </div>
  );
}

function SourceTraceMap({
  model,
  onSelectModel,
}: {
  model: BrowserModel;
  onSelectModel: (id: BrowserModelId) => void;
}) {
  return (
    <div className="trace-map">
      <div className="trace-map__node trace-map__node--file">
        <span>{model.file}</span>
      </div>
      <div className="trace-map__arrow">↘</div>
      <button
        type="button"
        className="trace-map__node trace-map__node--node"
        onClick={() => onSelectModel(model.id)}
      >
        <strong>{model.label}</strong>
        <span>{model.kind.toLowerCase()}</span>
      </button>
      <div className="trace-map__arrow">↘</div>
      <div className="trace-map__node trace-map__node--trace">
        <strong>Trace links</strong>
        <span>{model.traces.map(([left, right]) => `${left}: ${right}`).join(', ')}</span>
      </div>
      <div className="trace-map__footer">
        Context: {model.repository} / {model.branch}, read-only browser, exact file ownership preserved
      </div>
    </div>
  );
}

function DiffSummary({
  model,
  onSelectModel,
}: {
  model: BrowserModel;
  onSelectModel: (id: BrowserModelId) => void;
}) {
  return (
    <div className="diff-summary">
      <div className="diff-summary__legend">
        <span className="legend-chip legend-chip--added">Added</span>
        <span className="legend-chip legend-chip--removed">Removed</span>
        <span className="legend-chip legend-chip--changed">Modified</span>
        <span className="legend-chip">Unchanged</span>
      </div>
      <div className="diff-summary__cards">
        <button type="button" className="summary-card summary-card--added" onClick={() => onSelectModel('powertrain')}>
          <strong>OnboardCharger</strong>
          <span>Added in concept-ev</span>
        </button>
        <button
          type="button"
          className="summary-card summary-card--changed"
          onClick={() => onSelectModel(model.id)}
        >
          <strong>capacity</strong>
          <span>75 kWh → 95 kWh</span>
        </button>
        <button type="button" className="summary-card summary-card--changed" onClick={() => onSelectModel('battery-system')}>
          <strong>coolant flow</strong>
          <span>Now explicitly traced</span>
        </button>
      </div>
    </div>
  );
}

function DiffText({ model }: { model: BrowserModel }) {
  return (
    <CodeEditor
      title={`${model.label} diff`}
      lines={model.diffLines.map((line) => {
        if (line.kind === 'added') return `+ ${line.text}`;
        if (line.kind === 'removed') return `- ${line.text}`;
        return `  ${line.text}`;
      })}
      annotateLines
    />
  );
}

function DiffSplit({
  model,
  onSelectModel,
}: {
  model: BrowserModel;
  onSelectModel: (id: BrowserModelId) => void;
}) {
  return (
    <div className="split-view">
      <CodeEditor title="main" lines={model.sourceLines} dense />
      <CodeEditor title="concept-ev" lines={model.comparisonLines} dense />
    </div>
  );
}

function ComparisonText({ model }: { model: BrowserModel }) {
  return (
    <CodeEditor
      title={`${model.label} comparison`}
      lines={model.comparisonLines}
    />
  );
}

function SplitArchitecture({
  model,
  onSelectModel,
}: {
  model: BrowserModel;
  onSelectModel: (id: BrowserModelId) => void;
}) {
  return (
    <div className="split-view">
      <ArchitectureDiagram model={model} onSelectModel={onSelectModel} />
      <CodeEditor title="Generated SysML" lines={model.sourceLines} dense />
    </div>
  );
}

function SplitComparison({
  model,
  onSelectModel,
}: {
  model: BrowserModel;
  onSelectModel: (id: BrowserModelId) => void;
}) {
  return (
    <div className="split-view">
      <ComparisonDiagram model={model} onSelectModel={onSelectModel} />
      <CodeEditor
        title="Semantic diff"
        lines={model.diffLines.map((line) => `${line.kind.toUpperCase()}: ${line.text}`)}
        dense
      />
    </div>
  );
}

function SplitSource({
  model,
  onSelectModel,
}: {
  model: BrowserModel;
  onSelectModel: (id: BrowserModelId) => void;
}) {
  return (
    <div className="split-view">
      <CodeEditor title="Source" lines={model.sourceLines} dense />
      <TraceMapInline model={model} onSelectModel={onSelectModel} />
    </div>
  );
}

function TraceMapInline({
  model,
  onSelectModel,
}: {
  model: BrowserModel;
  onSelectModel: (id: BrowserModelId) => void;
}) {
  return (
    <div className="split-side-panel">
      <div className="split-side-panel__title">Source ownership</div>
      <div className="split-side-panel__body">
        <div className="split-side-panel__item">
          <span>File</span>
          <strong>{model.file}</strong>
        </div>
        <div className="split-side-panel__item">
          <span>Owned by</span>
          <strong>{model.owner}</strong>
        </div>
        <div className="split-side-panel__item">
          <span>Context</span>
          <strong>
            {model.repository} / {model.branch}
          </strong>
        </div>
        <div className="split-side-panel__item">
          <span>State</span>
          <button type="button" className="split-side-panel__action" onClick={() => onSelectModel(model.id)}>
            Read-only browser
          </button>
        </div>
      </div>
    </div>
  );
}

function CodeEditor({
  title,
  lines,
  dense = false,
  annotateLines = false,
}: {
  title: string;
  lines: readonly string[];
  dense?: boolean;
  annotateLines?: boolean;
}) {
  return (
    <div className={`code-editor ${dense ? 'code-editor--dense' : ''}`}>
      <div className="code-editor__title">{title}</div>
      <div className="code-editor__body" role="textbox" aria-label={title}>
        {lines.map((line, index) => (
          <div key={`${title}-${index}`} className={`code-line ${annotateLines ? 'code-line--annotated' : ''}`}>
            <span className="code-line__number">{String(index + 1).padStart(2, '0')}</span>
            <span className="code-line__text">{line || '\u00A0'}</span>
          </div>
        ))}
      </div>
    </div>
  );
}

function DiagramNode({
  label,
  kind,
  tone,
  wide = false,
  selected = false,
  onClick,
}: {
  label: string;
  kind: string;
  tone: Tone;
  wide?: boolean;
  selected?: boolean;
  onClick?: () => void;
}) {
  return (
    <button
      type="button"
      className={`diagram-node diagram-node--${tone} ${wide ? 'is-wide' : ''} ${selected ? 'is-selected' : ''}`}
      onClick={onClick}
    >
      <span className="diagram-node__kind">{kind}</span>
      <strong className="diagram-node__label">{label}</strong>
    </button>
  );
}

function DiagramConnector({ label }: { label: string }) {
  return (
    <div className="diagram-connector">
      <span className="diagram-connector__line" />
      <span className="diagram-connector__label">{label}</span>
      <span className="diagram-connector__line" />
    </div>
  );
}

function InspectorSection({ title, children }: { title: string; children: ReactNode }) {
  return (
    <section className="inspector-section">
      <div className="inspector-section__title">{title}</div>
      {children}
    </section>
  );
}

function KeyValueList({ rows }: { rows: readonly (readonly [string, string])[] }) {
  return (
    <dl className="key-value-list">
      {rows.map(([label, value]) => (
        <div key={label} className="key-value-list__row">
          <dt>{label}</dt>
          <dd>{value}</dd>
        </div>
      ))}
    </dl>
  );
}

function TraceList({ rows }: { rows: readonly (readonly [string, string] | string)[] }) {
  return (
    <ul className="trace-list">
      {rows.map((row) => {
        if (Array.isArray(row)) {
          const [label, value] = row;
          return (
            <li key={`${label}-${value}`} className="trace-list__row">
              <span>{label}</span>
              <strong>{value}</strong>
            </li>
          );
        }

        return (
          <li key={`${row}`} className="trace-list__row">
            <strong>{row}</strong>
          </li>
        );
      })}
    </ul>
  );
}

function ChipList({ items, tone }: { items: readonly string[]; tone: Tone }) {
  return (
    <div className="chip-list">
      {items.map((item) => (
        <span key={item} className={`chip chip--${tone}`}>
          {item}
        </span>
      ))}
    </div>
  );
}

export { App };

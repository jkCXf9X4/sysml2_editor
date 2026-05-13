type StatusPillProps = {
  label: string;
  tone?: 'neutral' | 'success' | 'warning';
};

type WorkspaceContext = {
  alias: string;
  branch: string;
  mode: string;
  files: number;
  traces: number;
  tone: StatusPillProps['tone'];
};

const contexts: WorkspaceContext[] = [
  {
    alias: 'vehicle-demo',
    branch: 'main',
    mode: 'Writable',
    files: 4,
    traces: 18,
    tone: 'success',
  },
  {
    alias: 'vehicle-demo',
    branch: 'experiment',
    mode: 'Worktree',
    files: 4,
    traces: 21,
    tone: 'success',
  },
  {
    alias: 'supplier-power',
    branch: 'release',
    mode: 'Read-only',
    files: 2,
    traces: 7,
    tone: 'warning',
  },
];

function StatusPill({ label, tone = 'neutral' }: StatusPillProps) {
  return <span className={`status-pill status-pill-${tone}`}>{label}</span>;
}

export function App() {
  return (
    <div className="shell">
      <header className="topbar">
        <div>
          <div className="eyebrow">Workspace</div>
          <h1>sysml2_editor</h1>
        </div>
        <div className="topbar-actions" aria-label="Open contexts">
          {contexts.map((context) => (
            <StatusPill
              key={`${context.alias}:${context.branch}`}
              label={`${context.alias}/${context.branch}`}
              tone={context.tone}
            />
          ))}
        </div>
      </header>

      <main className="workspace">
        <aside className="panel panel-left">
          <div className="panel-header">
            <h2>Contexts</h2>
            <StatusPill label="3 open" tone="neutral" />
          </div>
          <ul className="context-list">
            {contexts.map((context) => (
              <li key={`${context.alias}:${context.branch}`}>
                <div>
                  <strong>{context.alias}</strong>
                  <span>{context.branch}</span>
                </div>
                <StatusPill label={context.mode} tone={context.tone} />
              </li>
            ))}
          </ul>
        </aside>

        <section className="panel panel-center">
          <div className="panel-header">
            <h2>Branch Comparison</h2>
            <StatusPill label="Multi-context" tone="warning" />
          </div>
          <div className="comparison-grid">
            <div className="context-surface">
              <div className="surface-header">
                <strong>vehicle-demo</strong>
                <StatusPill label="main" tone="success" />
              </div>
              <div className="canvas-card">
                <strong>Vehicle</strong>
                <span>Package</span>
                <small>model/root.sysml</small>
              </div>
              <div className="canvas-card canvas-card-muted">
                <strong>BatteryPack</strong>
                <span>Part definition</span>
                <small>workspace-main</small>
              </div>
            </div>

            <div className="context-surface">
              <div className="surface-header">
                <strong>vehicle-demo</strong>
                <StatusPill label="experiment" tone="success" />
              </div>
              <div className="canvas-card">
                <strong>Vehicle</strong>
                <span>Package</span>
                <small>model/root.sysml</small>
              </div>
              <div className="canvas-card canvas-card-added">
                <strong>ThermalMonitor</strong>
                <span>Part definition</span>
                <small>workspace-experiment</small>
              </div>
            </div>
          </div>
        </section>

        <aside className="panel panel-right">
          <div className="panel-header">
            <h2>Inspector</h2>
            <StatusPill label="Scoped" tone="success" />
          </div>
          <dl className="details">
            <div>
              <dt>Selected</dt>
              <dd>ThermalMonitor</dd>
            </div>
            <div>
              <dt>Workspace</dt>
              <dd>workspace-experiment</dd>
            </div>
            <div>
              <dt>Save target</dt>
              <dd>vehicle-demo / experiment / model/root.sysml</dd>
            </div>
          </dl>

          <section className="trace-section" aria-label="Traceability">
            <h3>Traceability</h3>
            <ul className="trace-list">
              <li>
                <span>Added from</span>
                <strong>base -> head</strong>
              </li>
              <li>
                <span>Defined in</span>
                <strong>workspace-experiment:model/root.sysml</strong>
              </li>
              <li>
                <span>Related repo</span>
                <strong>supplier-power/release</strong>
              </li>
            </ul>
          </section>
        </aside>
      </main>
    </div>
  );
}

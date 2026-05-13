type StatusPillProps = {
  label: string;
  tone?: 'neutral' | 'success' | 'warning';
};

function StatusPill({ label, tone = 'neutral' }: StatusPillProps) {
  return <span className={`status-pill status-pill-${tone}`}>{label}</span>;
}

export function App() {
  return (
    <div className="shell">
      <header className="topbar">
        <div>
          <div className="eyebrow">Project</div>
          <h1>sysml2_editor</h1>
        </div>
        <div className="topbar-actions">
          <StatusPill label="main" tone="success" />
          <StatusPill label="OpenAPI ready" tone="neutral" />
        </div>
      </header>

      <main className="workspace">
        <aside className="panel panel-left">
          <h2>Type Palette</h2>
          <ul className="list">
            <li>Package</li>
            <li>Part definition</li>
            <li>Part usage</li>
            <li>Port</li>
            <li>Connection</li>
            <li>Requirement</li>
          </ul>
        </aside>

        <section className="panel panel-center">
          <div className="panel-header">
            <h2>Model Surface</h2>
            <StatusPill label="Read-only slice" tone="warning" />
          </div>
          <div className="canvas">
            <div className="canvas-card">
              <strong>Vehicle</strong>
              <span>Package</span>
            </div>
            <div className="canvas-card canvas-card-muted">
              <strong>BatteryPack</strong>
              <span>Part definition</span>
            </div>
            <div className="canvas-card canvas-card-muted">
              <strong>battery</strong>
              <span>Part usage</span>
            </div>
          </div>
        </section>

        <aside className="panel panel-right">
          <h2>Inspector</h2>
          <dl className="details">
            <div>
              <dt>Selected</dt>
              <dd>BatteryPack</dd>
            </div>
            <div>
              <dt>Status</dt>
              <dd>Committed</dd>
            </div>
            <div>
              <dt>Source</dt>
              <dd>model/root.sysml</dd>
            </div>
          </dl>
        </aside>
      </main>
    </div>
  );
}

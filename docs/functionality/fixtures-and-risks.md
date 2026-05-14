# Fixtures And Risk Gaps

## Fixture Map

### `fixtures/phase-0-workbench/`

- Current use: Workbench shell fixture.

### `fixtures/phase-1-browser/`

- Current use: Browser UI fixture.

### `fixtures/phase-2-editing/`

- Current use: Visual editing fixture.

### `fixtures/tiny-single-file/`

- Current use: Parser/source/write smoke fixture.

### `fixtures/multi-file-modular/`

- Current use: Modular parser and import traceability.

### `fixtures/invalid-input/`

- Current use: Parser diagnostics.

### `fixtures/branch-divergence/`

- Current use: Branch diff and multi-context comparison.

### `fixtures/merge-conflict/`

- Current use: Merge conflict workflow.

### `fixtures/saved-views/`

- Current use: Saved view API fixtures.

## Planned Fixture Sets

### `fixtures/multi-context-editing/`

- Planned use: safe multi-worktree and multi-context editing.

### `fixtures/advanced-sysml/`

- Planned use: broader SysML parser and UI coverage.

### `fixtures/model-libraries/`

- Planned use: model library import resolution.

### `fixtures/variant-models/`

- Planned use: variant and product-line modeling.

### Missing identity metadata fixture

- Planned use: write-policy enforcement and user-facing diagnostics.

### Cross-repository dependency fixture

- Planned use: dependency queries and impact analysis.

### CI validation fixture

- Planned use: validation status and CI integration.

### Report generation fixture

- Planned use: report generation workflow.

## Highest-Risk Gaps

### Fixture-backed frontend workflows

- Risk: the frontend still uses fixture-backed slices for important workflows.

### Limited backend generality

- Risk: backend functionality covers the MVP subset and temporary Git flows, not full SysML v2 or generalized merge semantics.

### Missing backend-backed E2E

- Risk: editing and Git workflow UI paths need backend-backed E2E before being treated as product-ready.

### Backend primitives without UI workflows

- Risk: saved views and trace matrix have backend primitives but no full UI workflow.

### Multi-context write safety

- Risk: multi-context write safety is not complete.

### Limited responsive verification

- Risk: responsive behavior is tested through CSS contracts and smoke checks, not viewport-level assertions.

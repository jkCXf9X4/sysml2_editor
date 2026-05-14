# 5. SysML Parsing, Graph, And Source Model

## 5.1 MVP Parser

### Parse supported structural SysML subset

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `parse_minimal_graph`
- Plan: Expand only with fixtures.

### Malformed input diagnostics

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `malformed_input_reports_diagnostic`
- Plan: Surface in browser UI.

### Opaque source spans

- Status: `Available`
- Stability: `Stable slice`
- Tested by: backend graph tests
- Plan: Preserve for unsupported syntax.

## 5.2 Model Graph DTO

### Graph context IDs

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `model_graph_has_context`
- Plan: Preserve across workspaces.

### Nodes and edges

- Status: `Available`
- Stability: `Stable slice`
- Tested by: backend graph tests
- Plan: Expand with SysML support.

### Files and source ranges

- Status: `Available`
- Stability: `Stable slice`
- Tested by: source preservation tests
- Plan: Keep source-backed.

### Diagnostics

- Status: `Available`
- Stability: `Stable slice`
- Tested by: diagnostic tests
- Plan: Display in UI.

### Trace links

- Status: `Available`
- Stability: `Stable slice`
- Tested by: traceability tests
- Plan: Expand query capability.

## 5.3 Source Preservation

### Source file text endpoint

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `get_source_file_preserves_text`
- Plan: Use in backend-backed browser E2E.

### Content hash preservation

- Status: `Available`
- Stability: `Stable slice`
- Tested by: backend smoke
- Plan: Use for safe reload/write checks.

### Owner-file source policy

- Status: `Available`
- Stability: `Stable slice`
- Tested by: `save_touches_only_owner`
- Plan: Keep as write gate.

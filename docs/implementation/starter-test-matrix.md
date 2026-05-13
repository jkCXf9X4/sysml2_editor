# Starter Test Matrix

This is the minimum test set that should pass before the first implementation slice is considered healthy.

## Required Smoke Tests

| Test | Layer | Fixture | Purpose | Expected Result |
| --- | --- | --- | --- | --- |
| `parse_round_trip_minimal` | Unit/Integration | `fixtures/tiny-single-file` | Prove the parser and writer agree on the supported subset | Matches `expected/graph.json` after parse -> write -> parse |
| `save_touches_only_owner` | Integration | `fixtures/multi-file-modular` | Prevent rewrite churn | Matches `expected/changed-files.json` |
| `stable_id_survives_rename` | Integration | `fixtures/tiny-single-file` | Protect identity | Renaming an element keeps the same stable ID and preserves view bindings |
| `semantic_branch_diff` | Integration | `fixtures/branch-divergence` | Verify branch comparison | Matches `expected/diff.json` |
| `malformed_input_reports_diagnostic` | Parser/Error | `fixtures/invalid-input` | Fail safely | Matches `expected/diagnostics.json` |
| `open_browse_select_smoke` | End-to-End | `fixtures/tiny-single-file` | Prove the first user path | A repo opens, the tree renders, and a selected node shows inspector data from `expected/graph.json` |

## Gating Rule

- Any change to parsing, model mapping, save logic, or diff logic must update this matrix if it changes the minimum safe test set.
- The first implementation branch should not add editing features until the first four rows pass.

## Platform Coverage

- The parser and save tests should run on both Windows and Linux.
- The smoke E2E test may start on one platform and expand to both once the harness is stable.

# Starter Test Matrix

This is the minimum test set for the initial implementation path.

Tests are split by slice so the first read-only branch is not blocked by writer, diff, or editing behavior. Later slices inherit earlier gates.

Vision trace:

- Supports: precise source mapping, reviewable Git changes, and safe visual editing by requiring evidence before behavior is exposed.
- Tradeoff: the first slice is read-only, intentionally delaying visual writes until parser, identity, and file ownership behavior are verified.

## Slice 0: Scaffold Gate

| Test | Layer | Fixture | Purpose | Expected Result |
| --- | --- | --- | --- | --- |
| `repo_scaffold_present` | Smoke | none | Prove the implementation skeleton exists | Root docs, `src/backend/Sysml2Editor.Api`, `src/frontend`, `src/shared/contracts/generated`, `tests/unit`, `tests/integration`, `tests/e2e`, `fixtures/`, and `scripts/` are checked in and match [project-structure.md](./project-structure.md) |
| `backend_starts_with_openapi` | Smoke | none | Prove backend scaffold and contract generation are wired | `dotnet run --project src/backend/Sysml2Editor.Api` exposes OpenAPI in development |
| `frontend_starts` | Smoke | none | Prove frontend scaffold runs | `npm run dev` from `src/frontend` serves the app shell |

## Slice 1: Read-Only Browser Gate

| Test | Layer | Fixture | Purpose | Expected Result |
| --- | --- | --- | --- | --- |
| `parse_minimal_graph` | Unit/Integration | `fixtures/tiny-single-file` | Prove the parser maps the supported subset into the model graph | Matches `expected/graph.json` exactly, including schema-required fields |
| `derive_item_to_file_traceability` | Unit/Integration | `fixtures/tiny-single-file` | Prove source ownership is first-class traceability | `ModelGraphDto.traceLinks` includes item-to-file links for every structured node |
| `derive_import_traceability` | Unit/Integration | `fixtures/multi-file-modular` | Prove file-to-file traceability starts in the read-only slice | Matches `expected/trace-links.json` for source ownership and resolved imports |
| `malformed_input_reports_diagnostic` | Parser/Error | `fixtures/invalid-input` | Fail safely | Matches `expected/diagnostics.json`; valid files in the same repo still load |
| `get_source_file_preserves_text` | Integration | `fixtures/tiny-single-file` | Prove source text can be displayed without rewriting | Returned content, line ending, and hash match the fixture |
| `open_browse_select_smoke` | End-to-End | `fixtures/tiny-single-file` | Prove the first user path | A repo opens, the tree renders, and a selected node shows inspector data and source ownership from `expected/graph.json` |

## Slice 2: Writer And Identity Gate

| Test | Layer | Fixture | Purpose | Expected Result |
| --- | --- | --- | --- | --- |
| `parse_round_trip_minimal` | Unit/Integration | `fixtures/tiny-single-file` | Prove the parser and writer agree on the supported subset | Matches `expected/graph.json` after parse -> write -> parse |
| `save_touches_only_owner` | Integration | `fixtures/multi-file-modular` | Prevent rewrite churn | Matches `expected/changed-files.json` |
| `stable_id_survives_rename` | Integration | `fixtures/tiny-single-file` | Protect identity | Renaming an element keeps the same stable ID and preserves view bindings |
| `missing_id_blocks_write_until_backfill` | Integration | fixture to add with missing identity metadata | Keep identity policy explicit | Read succeeds with diagnostics or read-only derived IDs; write is blocked until the backfill command is implemented |

## Slice 3: Git Diff Gate

| Test | Layer | Fixture | Purpose | Expected Result |
| --- | --- | --- | --- | --- |
| `semantic_branch_diff` | Integration | `fixtures/branch-divergence` | Verify branch comparison | Matches `expected/diff.json` |
| `branch_trace_links` | Integration | `fixtures/branch-divergence` | Preserve branch-to-branch traceability contract | Diff output includes changed files, changed items, and branch trace links |

## Gating Rule

- Slice 0 must pass before any runtime behavior is treated as implementation-ready.
- Any change to parsing, model mapping, save logic, or diff logic must update this matrix if it changes the minimum safe test set.
- Slice 1 must pass before save or visual editing is exposed.
- Slice 2 must pass before visual editing is exposed.
- Slice 3 must pass before branch comparison or commit summaries are exposed.

## Platform Coverage

- Parser and save tests should run on both Windows and Linux.
- The smoke E2E test may start on one platform and expand to both once the harness is stable.

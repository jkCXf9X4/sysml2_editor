# Fixtures

Checked-in test data used by unit, integration, and e2e tests.

## Fixture categories

- `tiny-single-file/` — one package, a few supported elements
- `multi-file-modular/` — packages and cross-file references
- `branch-divergence/` — two branches with semantic differences
- `invalid-input/` — syntax errors and partial writes
- `merge-conflict/` — overlapping edits and a resolvable conflict
- `cross-platform-paths/` — spaces, Unicode, and line-ending sensitive paths
- `phase-0-workbench/` — visual workbench shell and multi-context UI snapshots
- `phase-1-browser/` — read-only model browser state and traceability snapshots
- `phase-2-editing/` — visual editing session snapshots and generated SysML previews

## Rules

- Keep fixtures small unless a test is explicitly performance-oriented
- Prefer checked-in fixtures over generated ones for regression coverage
- Pair each fixture with expected outputs when the test depends on exact text or graph shape

Expected JSON files are canonical assertion snapshots for the first tests. They should include every field required by the contract under test, unless the test explicitly documents that it is asserting only a partial shape.

Fixture syntax follows the MVP subset in [syntax-examples.md](../docs/implementation/syntax-examples.md).

The [implementation roadmap](../docs/roadmap/roadmap.md) defines which fixtures are needed for each development phase.

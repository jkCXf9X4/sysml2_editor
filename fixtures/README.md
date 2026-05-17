# Fixtures

Checked-in test data used by unit, integration, and e2e tests.

## Fixture categories

- `tiny-single-file/` — one package, a few supported elements
- `multi-file-modular/` — packages and cross-file references
- `branch-divergence/` — two branches with semantic differences
- `invalid-input/` — syntax errors and partial writes
- `merge-conflict/` — overlapping edits and a resolvable conflict
- `cross-platform-paths/` — spaces, Unicode, and line-ending sensitive paths

## Rules

- Keep fixtures small unless a test is explicitly performance-oriented
- Prefer checked-in fixtures over generated ones for regression coverage
- Pair each fixture with expected outputs when the test depends on exact text or graph shape

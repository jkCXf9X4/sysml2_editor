# Test Fixtures

The repo should maintain a small fixture library that grows with the product.

## Fixture categories

- `tiny-single-file`: one package, a few supported elements
- `multi-file-modular`: packages and cross-file references
- `branch-divergence`: two branches with semantic differences
- `merge-conflict`: overlapping edits and a resolvable conflict
- `invalid-input`: syntax errors and partial writes
- `large-model`: enough data to exercise search and layout performance
- `cross-platform-paths`: spaces, Unicode, and line-ending sensitive paths

Concrete fixtures live under [fixtures](../fixtures).

## Fixture rules

- Keep fixtures deterministic and small unless the test is explicitly performance-oriented
- Prefer checked-in fixtures over dynamically generated ones for regression tests
- Version any generated expected output alongside the fixture
- Use the same fixtures across unit, integration, and E2E tests when possible

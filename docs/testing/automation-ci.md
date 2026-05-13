# Automation and CI

## Per-commit gates

Run on every change:

- Formatting and linting
- Unit tests
- Fast integration tests
- Small smoke E2E test
- Snapshot checks for key UI states

## PR gates

Run on pull requests:

- Full unit and integration suite
- Core E2E journeys
- Cross-platform sanity checks
- Semantic diff regression checks

## Nightly gates

Run on a schedule:

- Larger fixture sets
- Performance benchmarks
- Cross-platform matrix
- Flakier UI and file-watcher scenarios

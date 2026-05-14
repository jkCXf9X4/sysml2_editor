# Tests

Tests are organized by scope:

- `unit/` — parser logic, graph rules, write policy, and pure helpers
- `integration/` — backend + filesystem + Git + parser interactions
- `e2e/` — user workflows across the full application stack

Test data lives in [fixtures/](../fixtures/) at the repository root.
Recommended full-stack browser workflow testing is documented in [E2E testing](../docs/testing/e2e-testing.md).

Frontend browser access is covered in two layers:

- `src/frontend/tests/browser-entry.test.tsx` imports the real React entrypoint in jsdom and verifies it mounts into `#root`.
- `tests/integration/frontend-browser-smoke.sh` starts Vite, loads the app with headless Chrome or Chromium, executes browser JavaScript, and verifies the rendered workbench shell.

# E2E Testing

End-to-end tests should validate full user workflows through the web application in a real browser.

The recommended method is web-first browser testing with Playwright, not Electron. Electron should only be introduced when the product needs desktop-specific behavior such as native menus, local packaging, auto-update, or privileged filesystem workflows.

## Goals

- Exercise the same frontend entrypoint that users open in the browser.
- Exercise the ASP.NET Core backend through HTTP, not mocked service calls.
- Fail on browser console errors and failed API responses.
- Keep tests deterministic by using checked-in fixtures and fixed local ports.
- Keep startup and shutdown explicit so stale processes do not hide failures.

## Test Layers

Use the smallest layer that proves the behavior:

- Unit/component tests: `cd src/frontend && npm test`
- Type and production bundle gates: `npm run typecheck` and `npm run build`
- Backend API smoke: `bash tests/integration/backend-smoke.sh`
- Frontend HTTP smoke: `bash tests/integration/frontend-smoke.sh`
- Browser render smoke: `bash tests/integration/frontend-browser-smoke.sh`
- Full E2E workflow tests: Playwright tests under `tests/e2e/`

The browser render smoke test is a fast guard that the page loads and JavaScript executes. Full E2E tests should cover workflows, not just startup.

## Recommended Tooling

Use Playwright for full E2E tests.

Expected future setup:

```bash
cd src/frontend
npm install -D @playwright/test
npx playwright install chromium
```

The project should use Chromium as the default browser for CI. Add Firefox or WebKit only when there is a specific cross-browser risk.

## Startup Model

E2E tests should run against the normal development stack:

- Backend: `http://127.0.0.1:5087`
- Frontend: `http://127.0.0.1:5173`

Use the repository scripts:

```bash
bash scripts/stop.sh
bash scripts/start.sh
```

For automated E2E runs, prefer a test runner config that starts both services with `webServer` commands or a wrapper script that:

1. Stops stale app instances.
2. Starts the backend.
3. Waits for `GET /api/health`.
4. Starts the frontend on strict port `5173`.
5. Runs Playwright.
6. Stops the services it started.

Do not rely on arbitrary sleeps for readiness.

## Test Requirements

Every full E2E test should:

- Navigate to `http://127.0.0.1:5173`.
- Register handlers that fail the test on `pageerror`.
- Fail on unexpected `console.error`.
- Assert that the workbench shell is visible before interacting.
- Use stable user-facing selectors such as roles, labels, and visible text.
- Avoid CSS selectors unless no accessible selector exists.
- Use deterministic fixtures from `fixtures/`.
- Leave the workspace clean or use temporary directories.

## Example Playwright Test

```ts
import { expect, test } from '@playwright/test';

test.beforeEach(async ({ page }) => {
  page.on('pageerror', (error) => {
    throw error;
  });

  page.on('console', (message) => {
    if (message.type() === 'error') {
      throw new Error(message.text());
    }
  });
});

test('loads the workbench shell', async ({ page }) => {
  await page.goto('http://127.0.0.1:5173');

  await expect(page.getByLabel('Application menu')).toContainText('sysml2_editor');
  await expect(page.getByLabel('Workbench layout')).toBeVisible();
  await expect(page.getByRole('tree', { name: 'Model tree' })).toBeVisible();
});
```

## Initial Workflow Coverage

Prioritize these E2E workflows:

- App startup: browser renders the workbench with no console errors.
- Fixture browsing: select model tree items and verify source/inspector context updates.
- Backend connectivity: verify backend status transitions to connected when API is reachable.
- Editing draft: add a model element, rename it, undo, and redo.
- Git workflow preview: view branch comparison and commit preview without mutating checked-in fixtures.

## CI Gate

A practical CI gate should run:

```bash
cd src/frontend
npm test
npm run typecheck
npm run build
cd ../..
bash tests/integration/backend-smoke.sh
bash tests/integration/frontend-smoke.sh
bash tests/integration/frontend-browser-smoke.sh
```

Once Playwright is installed, add:

```bash
cd src/frontend
npm run e2e
```

## When Not To Use E2E

Do not use E2E tests for behavior that can be proven faster and more precisely with unit, component, parser, or API integration tests. E2E tests are for cross-boundary confidence and user workflow regressions.

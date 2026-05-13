# Project Structure Definition

This document defines the canonical repository layout for `sysml2_editor`. It is the source of truth for the project's folder structure.

## Structure Goals

- Keep frontend and backend boundaries explicit
- Keep domain logic separate from transport and UI
- Keep tests close to the behavior they verify
- Keep fixtures checked in and deterministic
- Keep architecture decisions under `docs/architecture`
- Keep implementation contracts under `docs/implementation`
- Keep roadmaps and planning under `docs/roadmap`
- Keep test gates under `docs/testing`
- Keep UI design under `docs/ui`
- Keep external references under `docs/reference`
- Keep AI navigation docs under `docs/ai`

Vision trace:

- Supports: visual UI, backend Git/file ownership, parser/domain rules, and tests each stay independently understandable and reviewable.
- Tradeoff: uses a few explicit folders early instead of a minimal single-project scaffold, because parser, Git, model, and UI responsibilities need clear boundaries.

## Root Layout

The repository uses this top-level shape:

```text
sysml2_editor/
  README.md
  PRODUCT_VISION.md
  LICENSE
  docs/
  src/
    backend/
      Sysml2Editor.Api/
    frontend/
    shared/
      contracts/
        generated/
  tests/
    unit/
    integration/
    e2e/
  fixtures/
  scripts/
```

`docs/decisions` and `docs/setup` are intentionally deferred. Add them only when they contain real project material; do not create empty documentation folders.

## Proposed Source Layout

### Backend

```text
src/backend/
  Sysml2Editor.Api/
  Sysml2Editor.Application/
  Sysml2Editor.Domain/
  Sysml2Editor.Infrastructure/
```

Responsibilities:

- `Api`: HTTP endpoints, request/response mapping, startup wiring
- `Application`: use cases, orchestration, application services
- `Domain`: parser behavior, model graph rules, write policy rules, pure model logic
- `Infrastructure`: Git CLI, filesystem access, repo scanning, persistence adapters

### Frontend

```text
src/frontend/
  app/
  components/
  features/
  hooks/
  styles/
  assets/
```

Responsibilities:

- `app`: bootstrap, routing, shell layout, top-level providers
- `components`: shared UI building blocks
- `features`: screen-level workflows such as model browser, editor, diff, and inspector
- `hooks`: shared React hooks
- `styles`: tokens, theme, and global styles
- `assets`: static images and icons

### Shared Contracts

```text
src/shared/
  contracts/
    generated/
  types/
```

Use this for:

- OpenAPI-generated TypeScript API clients
- shared TypeScript helper types derived from generated contracts
- serialized view definitions
- stable frontend constants that mirror backend enums

The backend C# DTOs are canonical for the first implementation slice. TypeScript API contracts should be generated from OpenAPI as described in [api-contract.md](./implementation/api-contract.md), not manually duplicated.

## Test Layout

```text
tests/
  unit/
  integration/
  e2e/
```

Test intent:

- `unit`: parser logic, graph rules, write policy, and pure helpers
- `integration`: backend + filesystem + Git + parser interactions
- `e2e`: user workflows across the full application stack
- `fixtures/`: checked-in sample repos and models used by tests, stored at the repository root

## Fixture Layout

```text
fixtures/
  tiny-single-file/
  multi-file-modular/
  branch-divergence/
  invalid-input/
  merge-conflict/
  cross-platform-paths/
```

Fixture rules:

- Keep fixtures small unless a test is explicitly performance-oriented
- Prefer checked-in fixtures over generated ones for regression coverage
- Pair each fixture with expected outputs when the test depends on exact text or graph shape

## Reference Rule

During project setup, treat this document as the source of truth for repository shape. If implementation needs to diverge, update this file first so the setup path stays accurate.

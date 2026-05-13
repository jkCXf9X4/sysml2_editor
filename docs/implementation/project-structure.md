# Project Structure Definition

This document defines the intended repository layout for project setup and early implementation.

Use this as the reference when scaffolding the codebase. The goal is to keep responsibilities separate from the start so the editor, parser, Git logic, and tests do not collapse into one undifferentiated application folder.

## Structure Goals

- Keep frontend and backend boundaries explicit
- Keep domain logic separate from transport and UI
- Keep tests close to the behavior they verify
- Keep fixtures checked in and deterministic
- Keep implementation docs under `docs/implementation`
- Keep AI navigation docs under `docs/ai`

## Root Layout

The implementation scaffold should use this top-level shape:

```text
sysml2_editor/
  README.md
  plan.md
  design.md
  test-strategy.md
  LICENSE
  docs/
    ai/
    implementation/
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

`docs/architecture`, `docs/decisions`, and `docs/setup` are intentionally deferred. Add them only when they contain real project material; do not create empty documentation folders for the first scaffold.

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
- `Domain`: parser contract, model schema, write policy rules, pure model logic
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

The backend C# DTOs are canonical for the first implementation slice. TypeScript API contracts should be generated from OpenAPI as described in [api-contract.md](./api-contract.md), not manually duplicated.

## Test Layout

```text
tests/
  unit/
  integration/
  e2e/
  fixtures/
```

Test intent:

- `unit`: parser logic, graph rules, write policy, and pure helpers
- `integration`: backend + filesystem + Git + parser interactions
- `e2e`: user workflows across the full application stack
- `fixtures`: checked-in sample repos and models used by tests

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

## Setup Order

Project setup should follow this order:

1. Create the repository root and documentation files.
2. Create the backend solution and projects.
3. Create the frontend app shell.
4. Add backend OpenAPI generation.
5. Add test projects.
6. Add fixture repositories and sample files.
7. Wire the local dev command.
8. Add the first smoke test and parser round-trip test.

## Reference Rule

During project setup, treat this document as the source of truth for repository shape. If implementation needs to diverge, update this file first so the setup path stays accurate.

# Implementation Readiness Checklist

This checklist captures the decisions that should be locked before the first implementation slice starts.

## Ready To Scaffold

- [ ] Project structure selected
- [ ] Runtime target selected
- [ ] API contract selected
- [ ] MVP parser contract selected
- [ ] Internal model schema selected
- [ ] File write policy selected
- [ ] Starter test matrix selected
- [ ] First vertical implementation slice defined

## Decision Links

- Structure: [project-structure.md](./project-structure.md)
- Runtime: [runtime-decision.md](./runtime-decision.md)
- API: [api-contract.md](./api-contract.md)
- Parser contract: [parser-contract.md](./parser-contract.md)
- Model schema: [model-schema.md](./model-schema.md)
- Write policy: [write-policy.md](./write-policy.md)
- Starter tests: [starter-test-matrix.md](./starter-test-matrix.md)

## First Slice Complete When

The first implementation branch is ready when all of the following are true:

- A local dev command exists and launches the app.
- A tiny fixture repo can be opened without errors.
- The parser can round-trip the MVP subset without changing semantics.
- A save operation only touches the intended file set.
- A branch comparison smoke test can identify the expected semantic changes.
- The first UI path can show tree, canvas, text, and inspector views from the same model.

## First Code Slice

The first code slice should be limited to one narrow path:

1. Open a local repo.
2. Parse a tiny fixture.
3. Render the model tree and inspector.
4. Show a read-only graph view.
5. Save nothing.

That slice should prove the app can load model data correctly before editing is allowed.

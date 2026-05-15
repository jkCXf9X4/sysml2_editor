# Product Requirements

These are product-level requirements derived from the use cases. Each requirement should be traceable back to at least one use case and one verification path.

## Context, Model, And Browsing

- `PR-01` The product shall open a local Git repository as an explicit workspace context. Derived from UC-01 and UC-09. Verification: backend workspace tests; browser workflow tests.
- `PR-02` The product shall keep repository, branch, file, and write-state identity visible in every model pane that can change context. Derived from UC-01, UC-02, UC-05, and UC-09. Verification: frontend shell tests; browser E2E.
- `PR-03` The product shall parse supported SysML text into a stable internal graph and surface diagnostics when parsing fails. Derived from UC-01, UC-04, and UC-12. Verification: backend parser tests; backend domain tests.
- `PR-04` The product shall allow browsing and search without requiring write access. Derived from UC-02, UC-04, and UC-09. Verification: frontend browser tests; browser E2E.
- `PR-05` The product shall synchronize tree, graph, source, inspector, and trace views from the same selected element. Derived from UC-02 and UC-03. Verification: frontend browser tests; browser E2E.

## Traceability, Editing, And Write Safety

- `PR-06` The product shall expose item-to-item, item-to-file, file-to-file, and branch-to-branch traceability where the underlying data exists. Derived from UC-03, UC-05, and UC-11. Verification: backend trace tests; frontend browser tests.
- `PR-07` The product shall make source ownership and change impact visible from the selected element. Derived from UC-03 and UC-11. Verification: backend trace tests; frontend browser tests.
- `PR-08` The product shall support visual creation, rename, connection, and deletion of supported structural elements. Derived from UC-06. Verification: frontend editing tests; backend writer tests; E2E.
- `PR-09` The product shall generate a source preview before any save action. Derived from UC-06 and UC-07. Verification: frontend editing tests; browser E2E.
- `PR-10` The product shall save only to the owning writable context and prevent cross-context writes. Derived from UC-07 and UC-09. Verification: backend write-policy tests; save/reload E2E.
- `PR-11` The product shall reject writes when identity or ownership metadata is insufficient. Derived from UC-04 and UC-07. Verification: backend writer tests; UI error-state tests.
- `PR-12` The product shall preserve stable identity across supported rename and move operations. Derived from UC-06, UC-07, and UC-10. Verification: backend identity tests; save/reload E2E.

## Git, Multi-Context, And Views

- `PR-13` The product shall expose Git status, semantic diff, commit, and merge-preview diagnostics for the active context. Derived from UC-08 and UC-09. Verification: backend Git tests; Git workflow E2E.
- `PR-14` The product shall support side-by-side repository and branch contexts without replacing the current working context. Derived from UC-05 and UC-09. Verification: frontend Git workflow tests; backend workspace tests.
- `PR-15` The product shall make the intended repository and branch explicit before save or commit. Derived from UC-07, UC-08, and UC-09. Verification: frontend shell tests; browser E2E.
- `PR-16` The product shall persist and restore saved views using stable model identity rather than display names alone. Derived from UC-10. Verification: backend saved-view tests; save/restore E2E.
- `PR-17` The product shall support impact analysis and dependency queries across items, files, branches, and repositories where data exists. Derived from UC-11. Verification: backend query tests; traceability E2E.

## Validation, Extensibility, And Quality

- `PR-18` The product shall keep validation state visible during browse, edit, save, and Git workflows. Derived from UC-04, UC-06, UC-07, and UC-08. Verification: frontend status tests; browser E2E.
- `PR-19` The product shall add advanced SysML syntax only when parser, graph, write policy, and regression coverage are in place. Derived from UC-12. Verification: parser tests; backend domain tests; regression E2E.
- `PR-20` The product shall keep the supported slice deterministic and reviewable, avoiding unrelated text churn in saves. Derived from UC-07 and UC-08. Verification: backend save-policy tests; diff assertions.

# API Contract

The ASP.NET Core backend owns repository access, parsing, file IO, and Git execution. The frontend consumes API DTOs and does not shell out or write files directly.

## Contract Ownership

- Backend C# DTOs are canonical for the Phase 1 API path in the [implementation roadmap](../../roadmap/roadmap.md).
- The backend must expose OpenAPI during development.
- Frontend TypeScript API clients should be generated from OpenAPI into `src/shared/contracts/generated/`.
- Generated files should not be edited manually.

Vision trace:

- Supports: precise, traceable, reviewable model data exposed consistently to UI views; generated contracts prevent frontend/backend drift.
- Tradeoff: backend DTOs are canonical first, with generated TypeScript clients instead of hand-maintained shared types.

## Development Endpoints

Base URL:

```text
http://localhost:5087/api
```

Frontend dev URL:

```text
http://localhost:5173
```

## Path Rules

- All file paths in API DTOs are repository-relative paths using `/`.
- Absolute paths are accepted only in `POST /repositories/open`.
- `..`, absolute paths, drive-qualified paths, and symlink escapes outside the repository root must be rejected.
- File content endpoints must use a query parameter for paths instead of a path segment so nested model paths are unambiguous.

## HTTP Status Rules

- `200 OK`: request succeeded.
- `400 Bad Request`: malformed request or invalid path syntax.
- `404 Not Found`: repository session or file does not exist.
- `409 Conflict`: repository state prevents the operation, such as a later save hash mismatch.
- `422 Unprocessable Entity`: the repository or SysML content was readable but failed validation/parsing rules.
- `500 Internal Server Error`: unexpected backend failure.

## Related Documents

| Document | Content |
| --- | --- |
| [Core DTOs](./core-dtos.md) | All DTO definitions with JSON examples |
| [Session and context models](./session-models.md) | Repository session, workspace context, multi-context view model |
| [Endpoints](./endpoints.md) | Full endpoint contracts with behavior and errors |
| [Deferred contracts](./deferred-contracts.md) | Future DTOs and endpoints reserved for later slices |

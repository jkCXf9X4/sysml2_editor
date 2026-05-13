# Backend

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

## Commands

Prerequisite:

- .NET SDK/runtime 10

Start the API:

```bash
dotnet run --project src/backend/Sysml2Editor.Api --urls http://127.0.0.1:5087
```

From the repository root, verify the API in another terminal:

```bash
curl -fsS http://127.0.0.1:5087/api/health
curl -fsS http://127.0.0.1:5087/swagger/v1/swagger.json
```

Expected health response:

```json
{"status":"ok"}
```

OpenAPI is available in development at:

```text
http://127.0.0.1:5087/swagger/v1/swagger.json
```

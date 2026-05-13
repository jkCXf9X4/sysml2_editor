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

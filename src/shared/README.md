# Shared Contracts

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

The backend C# DTOs are canonical for the first implementation slice. TypeScript API contracts should be generated from OpenAPI as described in [api-contract.md](../../docs/implementation/api-contract.md), not manually duplicated.

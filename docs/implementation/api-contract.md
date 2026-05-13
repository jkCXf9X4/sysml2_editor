# API Contract

The ASP.NET Core backend owns repository access, parsing, file IO, and Git execution. The frontend consumes API DTOs and does not shell out or write files directly.

## Contract Ownership

- Backend C# DTOs are canonical for the first implementation slice.
- The backend must expose OpenAPI during development.
- Frontend TypeScript API clients should be generated from OpenAPI into `src/shared/contracts/generated/`.
- Generated files should not be edited manually.

## Development Endpoints

Base URL:

```text
http://localhost:5087/api
```

Frontend dev URL:

```text
http://localhost:5173
```

## Core DTOs

### ModelGraphDto

```json
{
  "nodes": [],
  "edges": [],
  "files": [],
  "diagnostics": []
}
```

### ModelNodeDto

```json
{
  "stableId": "11111111-1111-4111-8111-111111111111",
  "kind": "PartDefinition",
  "name": "BatteryPack",
  "qualifiedName": "Vehicle::BatteryPack",
  "owningPackageId": "00000000-0000-4000-8000-000000000001",
  "sourceFile": "model/root.sysml",
  "sourceRange": {
    "startLine": 3,
    "startColumn": 3,
    "endLine": 3,
    "endColumn": 24
  },
  "attributes": {},
  "modelStatus": "Committed"
}
```

### DiagnosticDto

```json
{
  "severity": "Error",
  "message": "Expected element name.",
  "sourceFile": "model/root.sysml",
  "sourceRange": {
    "startLine": 2,
    "startColumn": 12,
    "endLine": 2,
    "endColumn": 13
  }
}
```

## First-Slice Endpoints

### Open Repository

```text
POST /repositories/open
```

Request:

```json
{
  "path": "/absolute/path/to/repo"
}
```

Response:

```json
{
  "repositoryId": "local-vehicle-demo",
  "rootPath": "/absolute/path/to/repo",
  "branch": "main",
  "graph": {}
}
```

### Get Model Graph

```text
GET /repositories/{repositoryId}/model
```

Response: `ModelGraphDto`

### Get Source File

```text
GET /repositories/{repositoryId}/files/{path}
```

Response:

```json
{
  "path": "model/root.sysml",
  "content": "package Vehicle { }",
  "lineEnding": "LF",
  "contentHash": "sha256:..."
}
```

## Deferred Endpoints

- Save file
- Commit changes
- Branch comparison
- Visual diff
- Clone repository

These are not required for the read-only first slice.


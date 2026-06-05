# PulsePanel

Russian version: [README.ru.md](README.ru.md)

PulsePanel is a backend API for server inventory and monitoring.

The project stores servers, updates heartbeats, runs TCP availability checks, checks servers automatically in the background, and returns status summaries.

## Features

- server CRUD
- heartbeat endpoint
- manual TCP check by `Host` and `CheckPort`
- automatic background server checks
- `Unknown`, `Online`, `Offline` status calculation
- dashboard summary
- search and sorting
- DTO validation
- consistent error responses
- Serilog logging
- PostgreSQL via Docker Compose
- Swagger / OpenAPI
- unit tests

## Stack

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- Npgsql
- PostgreSQL 16
- Docker / Docker Compose
- Swagger / OpenAPI
- Serilog
- xUnit

## Run With Docker Compose

```bash
docker compose -f deploy/docker-compose.yml up -d --build
```

Swagger:

```text
http://localhost:8080/swagger
```

## Local Run

Start PostgreSQL:

```bash
docker compose -f deploy/docker-compose.yml up -d postgres
```

Create a migration when the database schema changes:

```bash
dotnet ef migrations add MigrationName --project src/PulsePanel.Infrastructure --startup-project src/PulsePanel.Api
```

Apply migrations:

```bash
dotnet ef database update --project src/PulsePanel.Infrastructure --startup-project src/PulsePanel.Api
```

Run the API:

```bash
dotnet run --project src/PulsePanel.Api
```

Swagger:

```text
http://localhost:5264/swagger
```

## API

| Method | Endpoint | Description |
| --- | --- | --- |
| `POST` | `/api/servers` | Create server |
| `GET` | `/api/servers` | Get server list |
| `GET` | `/api/servers/{id}` | Get server by id |
| `PUT` | `/api/servers/{id}` | Update server |
| `DELETE` | `/api/servers/{id}` | Delete server |
| `POST` | `/api/servers/{id}/heartbeat` | Update heartbeat |
| `POST` | `/api/servers/{id}/check` | Run TCP check |
| `GET` | `/api/dashboard/summary` | Get summary |

## ServerChecks

```json
{
  "ServerChecks": {
    "Enabled": true,
    "IntervalSeconds": 60,
    "TimeoutSeconds": 3
  }
}
```

## Tests

```bash
dotnet test PulsePanel.slnx
```

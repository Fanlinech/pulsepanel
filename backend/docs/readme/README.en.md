# PulsePanel Backend

Russian version: [README.ru.md](../../README.md)

PulsePanel backend is an ASP.NET Core Web API for server inventory, heartbeat updates, TCP availability checks, status calculation, and dashboard summary.

## Structure

```text
backend
├── src
│   ├── PulsePanel.Api
│   ├── PulsePanel.Core
│   └── PulsePanel.Infrastructure
│
├── tests
│   └── PulsePanel.Tests
│
└── PulsePanel.slnx
```

## Stack

- .NET 10
- ASP.NET Core Web API
- Controllers
- Entity Framework Core
- Npgsql
- PostgreSQL 16
- Serilog
- Swagger / OpenAPI
- xUnit
- Testcontainers

## Run Backend Locally

Start PostgreSQL from the repository root:

```bash
docker-compose up -d postgres
```

Then run the API from the `backend` directory:

```bash
dotnet build PulsePanel.slnx
dotnet run --project src/PulsePanel.Api
```

Swagger:

```text
http://localhost:5264/swagger
```

## Database Migrations

Run from the `backend` directory:

```bash
dotnet ef migrations add MigrationName --project src/PulsePanel.Infrastructure --startup-project src/PulsePanel.Api
dotnet ef database update --project src/PulsePanel.Infrastructure --startup-project src/PulsePanel.Api
```

Create a new migration only when entities, `AppDbContext`, or EF Core configuration change.

## Configuration

Main settings are stored in `src/PulsePanel.Api/appsettings.json`.

```json
{
  "ServerChecks": {
    "Enabled": true,
    "IntervalSeconds": 60,
    "TimeoutSeconds": 3
  }
}
```

The connection string for local development is provided by `appsettings.Development.json` or environment variables.

In Docker Compose, the API receives the connection string through:

```text
ConnectionStrings__DefaultConnection
```

## API Endpoints

| Method | Endpoint | Description |
| --- | --- | --- |
| `POST` | `/api/servers` | Create server |
| `GET` | `/api/servers` | Get server list |
| `GET` | `/api/servers/{id}` | Get server by id |
| `PUT` | `/api/servers/{id}` | Update server |
| `DELETE` | `/api/servers/{id}` | Delete server |
| `POST` | `/api/servers/{id}/heartbeat` | Update heartbeat |
| `POST` | `/api/servers/{id}/check` | Run TCP check |
| `GET` | `/api/dashboard/summary` | Get dashboard summary |

## Search And Sorting

`GET /api/servers` supports these query parameters:

| Parameter | Value |
| --- | --- |
| `search` | Search by `Name`, `Host`, `Description` |
| `sortBy` | `name`, `host`, `createdAt`, `status`, `lastHeartbeatAt` |
| `sortDirection` | `asc`, `desc` |

Examples:

```http
GET /api/servers?search=prod
GET /api/servers?sortBy=name&sortDirection=asc
GET /api/servers?search=api&sortBy=createdAt&sortDirection=desc
```

## Server Status

Server status is calculated by `ServerStatusCalculator`.

| Condition | Status |
| --- | --- |
| No heartbeat and no successful check | `Unknown` |
| Heartbeat happened less than 5 minutes ago | `Online` |
| TCP check happened less than 5 minutes ago and succeeded | `Online` |
| Last heartbeat/check is stale or check failed | `Offline` |

## Error Responses

`404` responses use a consistent shape:

```json
{
  "message": "Server not found",
  "statusCode": 404,
  "timeStamp": "2026-06-05T09:30:00Z",
  "path": "/api/servers/{id}"
}
```

Unexpected errors are handled by `ExceptionHandlingMiddleware` and returned as `500` responses.

## Logging

Serilog writes logs to:

- console
- `logs/pulsepanel-YYYYMMDD.log`

## Tests

```bash
dotnet test PulsePanel.slnx
```

Integration tests use Testcontainers and require Docker.

Test coverage includes:

- `ServerStatusCalculator`
- service unit tests
- server DTO validation
- API integration tests

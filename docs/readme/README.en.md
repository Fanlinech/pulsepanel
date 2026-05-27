# 🚀 PulsePanel

🌐 **Russian version:** [README.ru.md](README.ru.md)

**PulsePanel** is a backend API for server inventory and monitoring.

The project is educational, but it follows a practical backend structure: server CRUD, heartbeat updates, online/offline/unknown status calculation, dashboard summary, search, sorting, structured error responses, and logging.

---

## 📌 Current Features

- ✅ Create server
- ✅ Get server list
- ✅ Get server by `id`
- ✅ Update server
- ✅ Delete server
- ✅ Heartbeat endpoint
- ✅ Automatic server status calculation
- ✅ Dashboard summary
- ✅ Search by `Name`, `Host`, `Description`
- ✅ Sort by allowed fields
- ✅ Request DTO validation
- ✅ Consistent `404` error format
- ✅ Middleware for unexpected `500` errors
- ✅ HTTP request logging with Serilog
- ✅ Console and file logs
- ✅ PostgreSQL via Docker Compose
- ✅ Swagger / OpenAPI

## 🧱 Tech Stack

- **.NET 10**
- **ASP.NET Core Web API**
- **Controllers**
- **Entity Framework Core**
- **Npgsql**
- **PostgreSQL 16**
- **Docker / Docker Compose**
- **Swagger / OpenAPI**
- **Serilog**
- **xUnit**

## 📂 Project Structure

```text
PulsePanel
├── src
│   ├── PulsePanel.Api
│   ├── PulsePanel.Core
│   └── PulsePanel.Infrastructure
│
├── tests
│   └── PulsePanel.Tests
│
├── deploy
│   └── docker-compose.yml
│
├── docs
│   └── readme
│       ├── README.en.md
│       └── README.ru.md
│
├── README.md
└── PulsePanel.slnx
```

## ⚙️ Getting Started

### 1. Requirements

- .NET 10 SDK
- Docker Desktop
- EF Core CLI tool

If `dotnet ef` is not installed:

```bash
dotnet tool install --global dotnet-ef
```

### 2. Start PostgreSQL

```bash
docker compose -f deploy/docker-compose.yml up -d
```

Check that PostgreSQL is running:

```bash
docker compose -f deploy/docker-compose.yml ps
```

### 3. Create a migration when the database schema changes

Create a new migration only if you changed an entity, `AppDbContext`, or EF Core configuration.

```bash
dotnet ef migrations add MigrationName --project src/PulsePanel.Infrastructure --startup-project src/PulsePanel.Api
```

Example:

```bash
dotnet ef migrations add InitialCreate --project src/PulsePanel.Infrastructure --startup-project src/PulsePanel.Api
```

If you changed only controllers, services, request DTOs, logging, or README files, a new migration is not required.

### 4. Apply migrations to the database

```bash
dotnet ef database update --project src/PulsePanel.Infrastructure --startup-project src/PulsePanel.Api
```

### 5. Build the solution

```bash
dotnet build PulsePanel.slnx
```

### 6. Run the API

```bash
dotnet run --project src/PulsePanel.Api
```

### 7. Open Swagger

```text
http://localhost:5264/swagger
```

## 🔌 API Endpoints

| Method | Endpoint | Description |
| --- | --- | --- |
| `POST` | `/api/servers` | Create server |
| `GET` | `/api/servers` | Get server list |
| `GET` | `/api/servers/{id}` | Get server by id |
| `PUT` | `/api/servers/{id}` | Update server |
| `DELETE` | `/api/servers/{id}` | Delete server |
| `POST` | `/api/servers/{id}/heartbeat` | Update heartbeat |
| `GET` | `/api/dashboard/summary` | Get dashboard summary |

## 🔎 Search and Sorting

`GET /api/servers` supports query parameters:

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

When no query parameters are provided, the endpoint works with the default behavior and sorts by `createdAt desc`.

## 🟢 Server Status

Server status is calculated from `LastHeartbeatAt`:

| Condition | Status |
| --- | --- |
| `LastHeartbeatAt == null` | `Unknown` |
| heartbeat is recent | `Online` |
| heartbeat is stale | `Offline` |

## 📊 Dashboard Summary

```http
GET /api/dashboard/summary
```

Example response:

```json
{
  "totalServers": 12,
  "onlineServers": 2,
  "offlineServers": 3,
  "unknownServers": 7,
  "lastHeartbeatAt": "2026-05-27T07:30:00Z"
}
```

## 🧾 Error Responses

`404` responses use a consistent format:

```json
{
  "message": "Server not found",
  "statusCode": 404,
  "timeStamp": "2026-05-27T07:30:00Z",
  "path": "/api/servers/{id}"
}
```

Unexpected errors are handled by `ExceptionHandlingMiddleware` and returned as `500` responses.

## 🪵 Logging

Serilog writes logs to:

- console
- `logs/pulsepanel-YYYYMMDD.log`

Log files are ignored by git.

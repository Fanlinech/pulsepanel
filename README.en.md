# 🚀 PulsePanel

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
│   │   ├── Controllers
│   │   ├── Extensions
│   │   ├── Middleware
│   │   ├── Program.cs
│   │   └── Dockerfile
│   │
│   ├── PulsePanel.Core
│   │   ├── DTOs
│   │   ├── Entities
│   │   ├── Enums
│   │   ├── Interfaces
│   │   └── Services
│   │
│   └── PulsePanel.Infrastructure
│       ├── Persistence
│       └── Services
│
├── tests
│   └── PulsePanel.Tests
│
├── deploy
│   └── docker-compose.yml
│
├── README.md
├── README.ru.md
├── README.en.md
└── PulsePanel.slnx
```

## ⚙️ Getting Started

1. Start PostgreSQL:

```bash
docker compose -f deploy/docker-compose.yml up -d
```

2. Apply migrations:

```bash
dotnet ef database update --project src/PulsePanel.Infrastructure --startup-project src/PulsePanel.Api
```

3. Run the API:

```bash
dotnet run --project src/PulsePanel.Api
```

4. Open Swagger:

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

Endpoint:

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

## 🧪 Build

```bash
dotnet build PulsePanel.slnx
```

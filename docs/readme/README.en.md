# 🚀 PulsePanel

🌐 **Русская версия:** [README.ru.md](README.ru.md)

**PulsePanel** is a backend API for server inventory and monitoring.

It is a pet project built with ASP.NET Core. Servers can be created, updated, deleted, checked manually through TCP checks, checked automatically in the background, updated through heartbeat calls, and summarized on a dashboard.

---

## 📌 Features

- ✅ Server CRUD
- ✅ Heartbeat endpoint
- ✅ Manual TCP check by `Host:CheckPort`
- ✅ Automatic background server checks
- ✅ `Unknown`, `Online`, `Offline` status calculation
- ✅ Dashboard summary
- ✅ Search and sorting
- ✅ DTO validation
- ✅ Consistent error responses
- ✅ Serilog logging
- ✅ PostgreSQL via Docker Compose
- ✅ Swagger / OpenAPI
- ✅ Unit tests

## 🧱 Tech Stack

- **.NET 10**
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **Npgsql**
- **PostgreSQL 16**
- **Docker / Docker Compose**
- **Swagger / OpenAPI**
- **Serilog**
- **xUnit**

## ⚙️ Run

### Docker Compose

```bash
docker compose -f deploy/docker-compose.yml up -d --build
```

Swagger:

```text
http://localhost:8080/swagger
```

### Local Run With Visual Studio / dotnet run

Start PostgreSQL only:

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

## 🔌 API Endpoints

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

## ⏱️ ServerChecks

```json
{
  "ServerChecks": {
    "Enabled": true,
    "IntervalSeconds": 60,
    "TimeoutSeconds": 3
  }
}
```

## ✅ Tests

```bash
dotnet test PulsePanel.slnx
```

# 🚀 PulsePanel

🌐 **English version:** [README.en.md](README.en.md)

**PulsePanel** — backend API для учёта и мониторинга серверов.

Это pet-проект на ASP.NET Core: серверы можно добавлять, обновлять, удалять, проверять вручную через TCP-check, автоматически проверять в фоне, обновлять heartbeat и смотреть сводку по статусам.

---

## 📌 Возможности

- ✅ CRUD для серверов
- ✅ Heartbeat endpoint
- ✅ Ручная TCP-проверка сервера по `Host:CheckPort`
- ✅ Автоматическая фоновая проверка серверов
- ✅ Расчёт статусов `Unknown`, `Online`, `Offline`
- ✅ Dashboard summary
- ✅ Поиск и сортировка
- ✅ Валидация DTO
- ✅ Единый формат ошибок
- ✅ Serilog-логирование
- ✅ PostgreSQL через Docker Compose
- ✅ Swagger / OpenAPI
- ✅ Unit-тесты

## 🧱 Стек

- **.NET 10**
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **Npgsql**
- **PostgreSQL 16**
- **Docker / Docker Compose**
- **Swagger / OpenAPI**
- **Serilog**
- **xUnit**

## ⚙️ Запуск

### Docker Compose

```bash
docker compose -f deploy/docker-compose.yml up -d --build
```

Swagger:

```text
http://localhost:8080/swagger
```

### Локально через Visual Studio / dotnet run

Запустить только PostgreSQL:

```bash
docker compose -f deploy/docker-compose.yml up -d postgres
```

Создать миграцию, если изменилась схема БД:

```bash
dotnet ef migrations add MigrationName --project src/PulsePanel.Infrastructure --startup-project src/PulsePanel.Api
```

Применить миграции:

```bash
dotnet ef database update --project src/PulsePanel.Infrastructure --startup-project src/PulsePanel.Api
```

Запустить API:

```bash
dotnet run --project src/PulsePanel.Api
```

Swagger:

```text
http://localhost:5264/swagger
```

## 🔌 API endpoints

| Method | Endpoint | Описание |
| --- | --- | --- |
| `POST` | `/api/servers` | Создать сервер |
| `GET` | `/api/servers` | Получить список серверов |
| `GET` | `/api/servers/{id}` | Получить сервер по id |
| `PUT` | `/api/servers/{id}` | Обновить сервер |
| `DELETE` | `/api/servers/{id}` | Удалить сервер |
| `POST` | `/api/servers/{id}/heartbeat` | Обновить heartbeat |
| `POST` | `/api/servers/{id}/check` | Выполнить TCP-проверку |
| `GET` | `/api/dashboard/summary` | Получить dashboard summary |

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

## ✅ Тесты

```bash
dotnet test PulsePanel.slnx
```

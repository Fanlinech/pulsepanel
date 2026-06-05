# PulsePanel

English version: [README.en.md](README.en.md)

PulsePanel — backend API для учёта и мониторинга серверов.

Проект позволяет хранить список серверов, обновлять heartbeat, выполнять TCP-проверку доступности, автоматически проверять серверы в фоне и получать сводку по статусам.

## Возможности

- CRUD для серверов
- heartbeat endpoint
- ручная TCP-проверка сервера по `Host` и `CheckPort`
- автоматическая фоновая проверка серверов
- расчёт статусов `Unknown`, `Online`, `Offline`
- dashboard summary
- поиск и сортировка
- валидация DTO
- единый формат ошибок
- логирование через Serilog
- PostgreSQL через Docker Compose
- Swagger / OpenAPI
- unit-тесты

## Стек

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- Npgsql
- PostgreSQL 16
- Docker / Docker Compose
- Swagger / OpenAPI
- Serilog
- xUnit

## Запуск через Docker Compose

```bash
docker compose -f deploy/docker-compose.yml up -d --build
```

Swagger:

```text
http://localhost:8080/swagger
```

## Локальный запуск

Запустить PostgreSQL:

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

## API

| Method | Endpoint | Описание |
| --- | --- | --- |
| `POST` | `/api/servers` | Создать сервер |
| `GET` | `/api/servers` | Получить список серверов |
| `GET` | `/api/servers/{id}` | Получить сервер по id |
| `PUT` | `/api/servers/{id}` | Обновить сервер |
| `DELETE` | `/api/servers/{id}` | Удалить сервер |
| `POST` | `/api/servers/{id}/heartbeat` | Обновить heartbeat |
| `POST` | `/api/servers/{id}/check` | Выполнить TCP-проверку |
| `GET` | `/api/dashboard/summary` | Получить сводку |

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

## Тесты

```bash
dotnet test PulsePanel.slnx
```

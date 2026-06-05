# PulsePanel Backend

English version: [docs/readme/README.en.md](docs/readme/README.en.md)

Backend проекта PulsePanel — ASP.NET Core Web API для хранения серверов, heartbeat, TCP-проверок, статусов и dashboard summary.

## Структура

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

## Запуск backend отдельно

PostgreSQL можно поднять из корня проекта:

```bash
docker-compose up -d postgres
```

Затем из папки `backend`:

```bash
dotnet build PulsePanel.slnx
dotnet run --project src/PulsePanel.Api
```

Swagger при локальном запуске:

```text
http://localhost:5264/swagger
```

## Миграции

Из папки `backend`:

```bash
dotnet ef migrations add MigrationName --project src/PulsePanel.Infrastructure --startup-project src/PulsePanel.Api
dotnet ef database update --project src/PulsePanel.Infrastructure --startup-project src/PulsePanel.Api
```

## Тесты

```bash
dotnet test PulsePanel.slnx
```

Интеграционные тесты используют Testcontainers и требуют запущенный Docker.

## Основные endpoint'ы

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

# PulsePanel Backend

English version: [docs/readme/README.en.md](docs/readme/README.en.md)

Backend проекта PulsePanel — ASP.NET Core Web API для хранения серверов, обновления heartbeat, TCP-проверок, расчёта статусов и dashboard summary.

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

## Стек

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

## Запуск backend отдельно

PostgreSQL можно поднять из корня репозитория:

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

## Настройки

Основные настройки лежат в `src/PulsePanel.Api/appsettings.json`.

```json
{
  "ServerChecks": {
    "Enabled": true,
    "IntervalSeconds": 60,
    "TimeoutSeconds": 3
  }
}
```

Connection string для локального запуска берётся из `appsettings.Development.json` или переменных окружения.

В Docker Compose API получает строку подключения через:

```text
ConnectionStrings__DefaultConnection
```

## Endpoint'ы

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

## Поиск и сортировка

`GET /api/servers` поддерживает query-параметры:

| Параметр | Значение |
| --- | --- |
| `search` | Поиск по `Name`, `Host`, `Description` |
| `sortBy` | `name`, `host`, `createdAt`, `status`, `lastHeartbeatAt` |
| `sortDirection` | `asc`, `desc` |

## Статусы

Статус рассчитывается через `ServerStatusCalculator`.

| Условие | Статус |
| --- | --- |
| Нет heartbeat и нет успешной проверки | `Unknown` |
| Heartbeat был меньше 5 минут назад | `Online` |
| TCP-проверка была меньше 5 минут назад и успешна | `Online` |
| Последний heartbeat/check устарел или check неуспешен | `Offline` |

## Логирование

Serilog пишет логи:

- в консоль
- в файл `logs/pulsepanel-YYYYMMDD.log`

## Тесты

```bash
dotnet test PulsePanel.slnx
```

Интеграционные тесты используют Testcontainers и требуют запущенный Docker.

Покрыты:

- unit-тесты калькулятора статусов
- unit-тесты сервисов
- валидация DTO
- интеграционные тесты API

# PulsePanel

English version: [docs/readme/README.en.md](docs/readme/README.en.md)

PulsePanel — backend API для учёта и мониторинга серверов.

Проект позволяет хранить список серверов, обновлять heartbeat, выполнять TCP-проверку доступности, автоматически проверять серверы в фоне и получать сводку по статусам.

## Возможности

- CRUD для серверов
- получение списка серверов и сервера по `id`
- обновление heartbeat
- ручная TCP-проверка сервера по `Host` и `CheckPort`
- автоматическая фоновая проверка серверов
- расчёт статусов `Unknown`, `Online`, `Offline`
- сводка `/api/dashboard/summary`
- поиск по `Name`, `Host`, `Description`
- сортировка по разрешённым полям
- валидация входных DTO
- единый формат `404` ошибок
- middleware для необработанных ошибок
- логирование через Serilog
- запуск PostgreSQL и API через Docker Compose
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

## Структура

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

## Требования

- .NET 10 SDK
- Docker Desktop
- EF Core CLI tool

Если `dotnet ef` не установлен:

```bash
dotnet tool install --global dotnet-ef
```

## Запуск через Docker Compose

Этот вариант запускает API и PostgreSQL.

```bash
docker compose -f deploy/docker-compose.yml up -d --build
```

Swagger будет доступен по адресу:

```text
http://localhost:8080/swagger
```

Проверить контейнеры:

```bash
docker compose -f deploy/docker-compose.yml ps
```

Посмотреть логи API:

```bash
docker compose -f deploy/docker-compose.yml logs -f api
```

Остановить контейнеры:

```bash
docker compose -f deploy/docker-compose.yml down
```

В Docker API подключается к PostgreSQL через имя сервиса `postgres`:

```text
Host=postgres;Port=5432;Database=pulsepanel;Username=pulsepanel;Password=***
```

## Локальный запуск

Этот вариант подходит для запуска API через Visual Studio или `dotnet run`, когда PostgreSQL работает в Docker.

Запустить только PostgreSQL:

```bash
docker compose -f deploy/docker-compose.yml up -d postgres
```

PostgreSQL будет доступен на порту `5433`.

Если изменилась схема базы данных, создать миграцию:

```bash
dotnet ef migrations add MigrationName --project src/PulsePanel.Infrastructure --startup-project src/PulsePanel.Api
```

Применить миграции:

```bash
dotnet ef database update --project src/PulsePanel.Infrastructure --startup-project src/PulsePanel.Api
```

Собрать проект:

```bash
dotnet build PulsePanel.slnx
```

Запустить API:

```bash
dotnet run --project src/PulsePanel.Api
```

Swagger для локального запуска:

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

## Поиск и сортировка

`GET /api/servers` поддерживает query-параметры:

| Параметр | Значение |
| --- | --- |
| `search` | Поиск по `Name`, `Host`, `Description` |
| `sortBy` | `name`, `host`, `createdAt`, `status`, `lastHeartbeatAt` |
| `sortDirection` | `asc`, `desc` |

Примеры:

```http
GET /api/servers?search=prod
GET /api/servers?sortBy=name&sortDirection=asc
GET /api/servers?search=api&sortBy=createdAt&sortDirection=desc
```

Если параметры не переданы, список сортируется по `createdAt desc`.

## Статусы серверов

Статус рассчитывается в `ServerStatusCalculator`.

| Условие | Статус |
| --- | --- |
| Нет heartbeat и нет check | `Unknown` |
| Heartbeat был меньше 5 минут назад | `Online` |
| TCP-check был меньше 5 минут назад и успешен | `Online` |
| Последний heartbeat/check устарел или check неуспешен | `Offline` |

## TCP-проверки

У сервера есть поле `CheckPort`. Ручная проверка делает TCP-подключение к `Host:CheckPort`.

Пример ответа:

```json
{
  "serverId": "00000000-0000-0000-0000-000000000000",
  "host": "example.com",
  "port": 443,
  "isAvailable": true,
  "status": "Online",
  "checkedAt": "2026-06-05T09:30:00Z",
  "responseTimeMs": 42,
  "message": "Connection successful"
}
```

## Автопроверка серверов

Фоновая проверка настраивается в `src/PulsePanel.Api/appsettings.json`.

```json
{
  "ServerChecks": {
    "Enabled": true,
    "IntervalSeconds": 60,
    "TimeoutSeconds": 3
  }
}
```

| Поле | Описание |
| --- | --- |
| `Enabled` | Включает или выключает автопроверку |
| `IntervalSeconds` | Интервал между циклами проверки |
| `TimeoutSeconds` | Timeout TCP-подключения |

## Dashboard summary

```http
GET /api/dashboard/summary
```

Пример ответа:

```json
{
  "totalServers": 12,
  "onlineServers": 2,
  "offlineServers": 3,
  "unknownServers": 7,
  "lastHeartbeatAt": "2026-06-05T09:30:00Z"
}
```

## Ошибки

Для `404` используется единый формат:

```json
{
  "message": "Server not found",
  "statusCode": 404,
  "timeStamp": "2026-06-05T09:30:00Z",
  "path": "/api/servers/{id}"
}
```

Необработанные ошибки проходят через `ExceptionHandlingMiddleware` и возвращают `500`.

## Логирование

Serilog пишет логи в консоль и в файл:

```text
logs/pulsepanel-YYYYMMDD.log
```

Папка `logs/` игнорируется Git.

## Тесты

```bash
dotnet test PulsePanel.slnx
```

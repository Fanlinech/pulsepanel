# 🚀 PulsePanel

🌐 **English version:** [docs/readme/README.en.md](docs/readme/README.en.md)

**PulsePanel** — backend API для учёта и мониторинга серверов.

Это pet-проект на ASP.NET Core: серверы можно добавлять, обновлять, удалять, проверять вручную через TCP-check, автоматически проверять в фоне, обновлять heartbeat и смотреть сводку по статусам.

---

## 📌 Возможности

- ✅ CRUD для серверов
- ✅ Получение списка серверов
- ✅ Получение сервера по `id`
- ✅ Heartbeat endpoint
- ✅ Ручная TCP-проверка сервера по `Host:CheckPort`
- ✅ Автоматическая фоновая проверка серверов
- ✅ Расчёт статусов `Unknown`, `Online`, `Offline`
- ✅ Dashboard summary
- ✅ Поиск по `Name`, `Host`, `Description`
- ✅ Сортировка по разрешённым полям
- ✅ Валидация request DTO
- ✅ Единый формат `404` ошибок
- ✅ Middleware для неожиданных `500` ошибок
- ✅ Логирование HTTP-запросов через Serilog
- ✅ Логи в консоль и файл
- ✅ PostgreSQL через Docker Compose
- ✅ Swagger / OpenAPI
- ✅ Unit-тесты для калькулятора статусов, сервисов и DTO

## 🧱 Стек

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

## 📂 Структура проекта

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

## ⚙️ Требования

- .NET 10 SDK
- Docker Desktop
- EF Core CLI tool

Если `dotnet ef` не установлен:

```bash
dotnet tool install --global dotnet-ef
```

## 🐳 Запуск через Docker Compose

Этот вариант запускает весь стек: API + PostgreSQL.

```bash
docker compose -f deploy/docker-compose.yml up -d --build
```

Проверить контейнеры:

```bash
docker compose -f deploy/docker-compose.yml ps
```

Swagger:

```text
http://localhost:8080/swagger
```

Логи API:

```bash
docker compose -f deploy/docker-compose.yml logs -f api
```

Остановить контейнеры:

```bash
docker compose -f deploy/docker-compose.yml down
```

В Docker API подключается к PostgreSQL через внутреннее имя сервиса:

```text
Host=postgres;Port=5432;Database=pulsepanel;Username=pulsepanel;Password=***
```

## 💻 Локальный запуск

Этот вариант удобен для Visual Studio: API запускается локально, а PostgreSQL работает в Docker.

### 1. Запустить только PostgreSQL

```bash
docker compose -f deploy/docker-compose.yml up -d postgres
```

PostgreSQL будет доступен с компьютера на порту `5433`.

### 2. Создать миграцию, если изменилась схема БД

Новая миграция нужна, если менялись entity, `AppDbContext` или EF Core configuration.

```bash
dotnet ef migrations add MigrationName --project src/PulsePanel.Infrastructure --startup-project src/PulsePanel.Api
```

Пример:

```bash
dotnet ef migrations add InitialCreate --project src/PulsePanel.Infrastructure --startup-project src/PulsePanel.Api
```

Если менялись только контроллеры, сервисы, DTO, логирование, тесты или README, миграция не нужна.

### 3. Применить миграции

```bash
dotnet ef database update --project src/PulsePanel.Infrastructure --startup-project src/PulsePanel.Api
```

### 4. Собрать проект

```bash
dotnet build PulsePanel.slnx
```

### 5. Запустить API

```bash
dotnet run --project src/PulsePanel.Api
```

Swagger для локального запуска:

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
| `POST` | `/api/servers/{id}/check` | Выполнить TCP-проверку сервера |
| `GET` | `/api/dashboard/summary` | Получить dashboard summary |

## 🔎 Поиск и сортировка

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

## 🟢 Статусы серверов

Статус рассчитывается в `ServerStatusCalculator`.

| Условие | Статус |
| --- | --- |
| Нет heartbeat и нет check | `Unknown` |
| Heartbeat был меньше 5 минут назад | `Online` |
| TCP-check был меньше 5 минут назад и успешен | `Online` |
| Последний heartbeat/check устарел или check неуспешен | `Offline` |

## 🧪 TCP-проверки

У сервера есть поле `CheckPort`. Ручная проверка делает TCP-подключение к:

```text
Host:CheckPort
```

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

## ⏱️ Автопроверка серверов

Фоновая проверка настраивается в `src/PulsePanel.Api/appsettings.json`:

```json
{
  "ServerChecks": {
    "Enabled": true,
    "IntervalSeconds": 60,
    "TimeoutSeconds": 3
  }
}
```

Поля:

| Поле | Описание |
| --- | --- |
| `Enabled` | Включает или выключает автопроверку |
| `IntervalSeconds` | Интервал между циклами проверки |
| `TimeoutSeconds` | Timeout TCP-подключения |

## 📊 Dashboard summary

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

## 🧾 Ошибки

Для `404` используется единый формат:

```json
{
  "message": "Server not found",
  "statusCode": 404,
  "timeStamp": "2026-06-05T09:30:00Z",
  "path": "/api/servers/{id}"
}
```

Неожиданные ошибки проходят через `ExceptionHandlingMiddleware` и возвращают `500`.

## 📑 Логирование

Serilog пишет логи:

- в консоль
- в файл `logs/pulsepanel-YYYYMMDD.log`

Папка `logs/` игнорируется Git.

## ✅ Тесты

Запуск тестов:

```bash
dotnet test PulsePanel.slnx
```

Покрыты:

- `ServerStatusCalculator`
- `ServerCheckService`
- `DashboardService`
- валидация `CreateServerRequest`
- валидация `UpdateServerRequest`

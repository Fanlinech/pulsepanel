# 🚀 PulsePanel

🌐 **English version:** [README.en.md](README.en.md)

**PulsePanel** — backend API для учёта и мониторинга серверов.

Проект построен как учебный, но приближенный к реальному backend-сервису: есть CRUD для серверов, heartbeat, расчёт online/offline/unknown статусов, dashboard summary, поиск, сортировка, единый формат ошибок и логирование.

---

## 📌 Что умеет проект

- ✅ Создание сервера
- ✅ Получение списка серверов
- ✅ Получение сервера по `id`
- ✅ Обновление сервера
- ✅ Удаление сервера
- ✅ Heartbeat endpoint
- ✅ Автоматический расчёт статуса сервера
- ✅ Dashboard summary
- ✅ Поиск по `Name`, `Host`, `Description`
- ✅ Сортировка по разрешённым полям
- ✅ Валидация входных DTO
- ✅ Единый формат `404` ошибок
- ✅ Middleware для неожиданных `500` ошибок
- ✅ Логирование HTTP-запросов через Serilog
- ✅ Логи в консоль и файл
- ✅ PostgreSQL через Docker Compose
- ✅ Swagger / OpenAPI

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

## ⚙️ Запуск проекта

### 1. Требования

- .NET 10 SDK
- Docker Desktop
- EF Core CLI tool

Если `dotnet ef` не установлен:

```bash
dotnet tool install --global dotnet-ef
```

### 2. Запустить PostgreSQL

```bash
docker compose -f deploy/docker-compose.yml up -d
```

Проверить, что PostgreSQL запущен:

```bash
docker compose -f deploy/docker-compose.yml ps
```

### 3. Создать миграцию, если изменилась схема базы данных

Создавай новую миграцию только если менялись entity, `AppDbContext` или EF Core configuration.

```bash
dotnet ef migrations add MigrationName --project src/PulsePanel.Infrastructure --startup-project src/PulsePanel.Api
```

Пример:

```bash
dotnet ef migrations add InitialCreate --project src/PulsePanel.Infrastructure --startup-project src/PulsePanel.Api
```

Если менялись только контроллеры, сервисы, DTO для запросов, логирование или README-файлы, новая миграция не нужна.

### 4. Применить миграции к базе данных

```bash
dotnet ef database update --project src/PulsePanel.Infrastructure --startup-project src/PulsePanel.Api
```

### 5. Собрать решение

```bash
dotnet build PulsePanel.slnx
```

### 6. Запустить API

```bash
dotnet run --project src/PulsePanel.Api
```

### 7. Открыть Swagger

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

Если параметры не переданы, список работает как раньше и сортируется по `createdAt desc`.

## 🟢 Статусы серверов

Статус считается на основе `LastHeartbeatAt`:

| Условие | Статус |
| --- | --- |
| `LastHeartbeatAt == null` | `Unknown` |
| heartbeat был недавно | `Online` |
| heartbeat устарел | `Offline` |

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
  "lastHeartbeatAt": "2026-05-27T07:30:00Z"
}
```

## 🧾 Ошибки

Для `404` используется единый формат:

```json
{
  "message": "Server not found",
  "statusCode": 404,
  "timeStamp": "2026-05-27T07:30:00Z",
  "path": "/api/servers/{id}"
}
```

Неожиданные ошибки проходят через `ExceptionHandlingMiddleware` и возвращают `500`.

## 🪵 Логирование

Serilog пишет логи:

- в консоль
- в файл `logs/pulsepanel-YYYYMMDD.log`

Логи не попадают в git благодаря `.gitignore`.

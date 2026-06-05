# PulsePanel

English version: [docs/readme/README.en.md](docs/readme/README.en.md)

PulsePanel — веб-приложение для учёта и мониторинга серверов.

Проект состоит из backend API на ASP.NET Core и frontend панели на Vue.js. Через интерфейс можно добавлять серверы, обновлять heartbeat, запускать TCP-проверку доступности, смотреть статусы и работать со списком серверов.

## Структура

```text
PulsePanel
├── backend
│   ├── src
│   ├── tests
│   ├── PulsePanel.slnx
│   └── README.md
│
├── frontend
│   ├── src
│   ├── package.json
│   └── README.md
│
├── docker-compose.yml
├── README.md
└── LICENSE.txt
```

## Стек

Backend:

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL 16
- Serilog
- xUnit

Frontend:

- Vue.js
- TypeScript
- PrimeVue
- Vite
- Nginx для Docker-сборки

## Запуск всего приложения

Из корня проекта:

```bash
docker-compose up -d --build
```

После запуска:

```text
Frontend: http://localhost:3000
Backend API: http://localhost:8080
Swagger: http://localhost:8080/swagger
PostgreSQL: localhost:5433
```

Остановить:

```bash
docker-compose down
```

Посмотреть логи:

```bash
docker-compose logs -f
```

## Документация

- Backend: [backend/README.md](backend/README.md)
- Frontend: [frontend/README.md](frontend/README.md)
- English root README: [docs/readme/README.en.md](docs/readme/README.en.md)

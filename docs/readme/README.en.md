# PulsePanel

Russian version: [../../README.md](../../README.md)

PulsePanel is a web application for server inventory and monitoring.

The project contains an ASP.NET Core backend API and a Vue.js frontend panel. The UI can create servers, update heartbeat state, run TCP availability checks, display statuses, and manage the server list.

## Structure

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

## Stack

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
- Nginx for Docker builds

## Run The Full Application

From the repository root:

```bash
docker-compose up -d --build
```

Available endpoints:

```text
Frontend: http://localhost:3000
Backend API: http://localhost:8080
Swagger: http://localhost:8080/swagger
PostgreSQL: localhost:5433
```

Stop:

```bash
docker-compose down
```

Logs:

```bash
docker-compose logs -f
```

## Documentation

- Backend: [../../backend/README.md](../../backend/README.md)
- Frontend: [../../frontend/README.md](../../frontend/README.md)

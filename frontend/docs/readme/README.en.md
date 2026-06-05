# PulsePanel Frontend

Russian version: [README.ru.md](../../README.md))

PulsePanel frontend is a Vue.js panel for working with the backend API.

The UI is used to manage servers: list records, create and edit servers, update heartbeat state, run manual TCP checks, view dashboard summary, search, and sort.

## Stack

- Vue.js 3
- TypeScript
- PrimeVue
- PrimeIcons
- Vite
- Nginx for Docker builds

## Structure

```text
frontend
├── src
│   ├── App.vue
│   ├── api.ts
│   ├── main.ts
│   ├── styles.css
│   └── types.ts
│
├── nginx
│   └── default.conf
│
├── Dockerfile
├── package.json
└── vite.config.ts
```

## UI Features

- `Servers` view with the server table
- create server
- edit server
- delete server
- search by `Name`, `Host`, `Description`
- sort by backend-supported fields
- update heartbeat
- run manual TCP checks
- `Dashboard` view with status summary
- `Checks` view for manual check operations

## Local Development

Install dependencies:

```bash
npm install
```

Run the dev server:

```bash
npm run dev
```

Frontend URL:

```text
http://localhost:5173
```

In development mode, Vite proxies `/api` to the backend:

```text
http://localhost:5264
```

The backend must be running separately.

## Build

```bash
npm run build
```

This command runs `vue-tsc` and `vite build`.

## Docker

The full application is started from the repository root:

```bash
docker-compose up -d --build
```

Frontend URL:

```text
http://localhost:3000
```

In Docker, nginx proxies `/api` to the backend service:

```text
http://api:8080
```

Because of this, the backend does not need CORS for the frontend container.

## API Client

Backend requests are implemented in `src/api.ts`.

Main methods:

- `getServers`
- `createServer`
- `updateServer`
- `deleteServer`
- `heartbeat`
- `checkServer`
- `getDashboardSummary`

Request and response types are defined in `src/types.ts`.

## Important Files

| File | Purpose |
| --- | --- |
| `src/App.vue` | Main UI and view switching |
| `src/api.ts` | Backend API client |
| `src/types.ts` | TypeScript API models |
| `src/styles.css` | Application layout and custom styles |
| `vite.config.ts` | Vite dev server and `/api` proxy |
| `nginx/default.conf` | Production nginx config and `/api` proxy |

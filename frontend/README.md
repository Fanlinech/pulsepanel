# PulsePanel Frontend

English version: [docs/readme/README.en.md](docs/readme/README.en.md)

Frontend проекта PulsePanel — Vue.js панель для работы с backend API.

Интерфейс предназначен для управления серверами: просмотр списка, создание и редактирование записей, heartbeat, ручные TCP-проверки, dashboard summary, поиск и сортировка.

## Стек

- Vue.js 3
- TypeScript
- PrimeVue
- PrimeIcons
- Vite
- Nginx для Docker-сборки

## Структура

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

## Возможности интерфейса

- экран `Servers` со списком серверов
- создание сервера
- редактирование сервера
- удаление сервера
- поиск по `Name`, `Host`, `Description`
- сортировка по разрешённым backend-полям
- обновление heartbeat
- ручная TCP-проверка сервера
- экран `Dashboard` со сводкой по статусам
- экран `Checks` для ручных проверок

## Локальный запуск

Установить зависимости:

```bash
npm install
```

Запустить dev server:

```bash
npm run dev
```

Frontend будет доступен по адресу:

```text
http://localhost:5173
```

В dev-режиме Vite проксирует `/api` на backend:

```text
http://localhost:5264
```

Backend должен быть запущен отдельно.

## Сборка

```bash
npm run build
```

Команда запускает `vue-tsc` и `vite build`.

## Docker

Для запуска всего приложения используется root compose из корня репозитория:

```bash
docker-compose up -d --build
```

После запуска frontend доступен по адресу:

```text
http://localhost:3000
```

В Docker nginx проксирует `/api` на backend service:

```text
http://api:8080
```

Поэтому CORS в backend не нужен.

## API client

Запросы к backend собраны в `src/api.ts`.

Основные методы:

- `getServers`
- `createServer`
- `updateServer`
- `deleteServer`
- `heartbeat`
- `checkServer`
- `getDashboardSummary`

Типы ответов и запросов описаны в `src/types.ts`.

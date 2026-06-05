# PulsePanel Frontend

English version: [docs/readme/README.en.md](docs/readme/README.en.md)

Frontend проекта PulsePanel — Vue.js панель для работы с backend API.

## Стек

- Vue.js
- TypeScript
- PrimeVue
- Vite
- Nginx для Docker-сборки

## Возможности интерфейса

- просмотр списка серверов
- создание, обновление и удаление сервера
- поиск и сортировка
- запуск heartbeat
- ручная TCP-проверка
- отображение статусов и основных счётчиков

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

Vite проксирует `/api` на backend:

```text
http://localhost:5264
```

## Docker

Для обычного запуска всего приложения используется root compose:

```bash
docker-compose up -d --build
```

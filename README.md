# AI-Powered Habit Tracker

Приложение для отслеживания привычек с авторизацией, дневной статистикой, персональными и городскими инсайтами, а также AI-подсказками для поддержки привычек. Репозиторий объединяет backend на ASP.NET Core и frontend на React с отдельным Docker-стеком для локального запуска.

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET Version](https://img.shields.io/badge/.NET-10.0-blue)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)](https://www.docker.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-4169E1?logo=postgresql)](https://www.postgresql.org/)

## Возможности

- Регистрация, вход и выход пользователя через JWT и cookie-based auth.
- Управление привычками: создание, редактирование, удаление и отметка выполнения.
- Страница привычки с историей и контекстными действиями.
- Персональные инсайты по привычкам.
- Городская аналитика и погодный контекст.
- Профиль пользователя с настройками.
- AI-поддержка и подсказки на основе данных приложения.
- Адаптивный интерфейс, рассчитанный на мобильный сценарий.

## Технологический стек

### Backend

- .NET 10 / C# 12
- ASP.NET Core Web API
- Entity Framework Core 10
- PostgreSQL 15
- ASP.NET Core Identity
- JWT Authentication
- Swagger / OpenAPI
- Redis
- Polly
- FluentValidation
- xUnit, Moq, Testcontainers

### Frontend

- React 19
- Vite
- React Router DOM 7
- Zustand
- Axios
- SCSS / Sass
- Vitest
- Playwright
- ESLint

## Структура проекта

- `backend/HabitApi` - API, EF Core migrations, Docker Compose и основная серверная логика.
- `backend/HabitApi.Tests` - unit- и integration-тесты backend-части.
- `backend/scripts` - PowerShell-скрипты для запуска integration-тестов.
- `frontend` - React-приложение, стили, store, сервисы и UI-компоненты.
- `frontend/end-to-end-tests` - Playwright smoke- и сценарные тесты.
- `postman` - коллекции для ручной проверки API.
- `README.md` - общий ориентир по проекту и запуску.

## Требования

- [Node.js](https://nodejs.org/)
- [npm](https://www.npmjs.com/)
- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Git](https://git-scm.com/)
- `dotnet-ef` для работы с миграциями Entity Framework

## Установка зависимостей

### Frontend

```bash
cd frontend
npm install
```

### Entity Framework CLI

Если `dotnet-ef` еще не установлен:

```bash
dotnet tool install --global dotnet-ef
```

## Общий запуск

Основной сценарий запуска такой:

1. Установите зависимости frontend и `dotnet-ef`.
2. Поднимите backend-стек в Docker из папки `backend/HabitApi`.
3. Примените миграции базы данных.
4. Запустите frontend из корня репозитория командой `npm run dev:frontend`.

### Шаг 1. Поднять backend в Docker

```bash
cd backend/HabitApi
docker compose up -d
```

Этот compose-файл поднимает:

- `habit_api` на `http://localhost:5093`
- PostgreSQL
- Redis
- MailHog
- Ollama
- mock weather service

### Шаг 2. Применить миграции базы данных

В папке `backend/HabitApi` выполните:

```bash
dotnet ef database update
```

Если база уже создана в Docker, команда применит схему к текущему подключению из переменных окружения `DB_*`.

### Шаг 3. Запустить frontend

Из корня репозитория:

```bash
npm run dev:frontend
```

После запуска frontend будет использовать API по адресу `http://localhost:5093`.

### Swagger

После старта backend документация API доступна по адресу:

```text
http://localhost:5093/swagger
```

## Тестирование

### Backend unit tests

```bash
dotnet test backend/HabitApi.Tests/HabitApi.Tests.csproj
```

### Backend integration tests

Для интеграционных тестов используются Testcontainers и отдельные PowerShell-скрипты:

```powershell
.\backend\scripts\Run-BackendIntegrationTests.ps1
```

Если нужен полный прогон LLM-интеграции:

```powershell
.\backend\scripts\Run-LlmIntegrationTests.ps1
```

### Frontend unit tests

```bash
cd frontend
npx vitest run
```

### Frontend e2e tests

Из корня репозитория:

```bash
npm run e2e-tests:ordered
```

Этот сценарий последовательно запускает Playwright-тесты для smoke- и пользовательских потоков.

## Контейнеризация

Локальная контейнеризация сосредоточена в `backend/HabitApi/docker-compose.yml`.

### Состав окружения

- `api` - backend-приложение
- `db` - PostgreSQL 15
- `redis` - кэш
- `mailhog` - локальная почта для подтверждения email и тестов
- `ollama` - локальный AI-движок
- `weather_mock` - мок погодного API

### Полезные порты

- `5093` - API
- `5431` - PostgreSQL
- `6379` - Redis
- `8025` - MailHog UI
- `11434` - Ollama
- `8080` - weather mock

### Остановка стенда

```bash
cd backend/HabitApi
docker compose down
```

## Roadmap

- Улучшить AI-подсказки и сценарии рекомендаций.
- Расширить аналитику по привычкам и городской статистике.
- Добавить уведомления и напоминания.
- Улучшить офлайн-устойчивость и UX для мобильных сценариев.
- Расширить покрытие e2e и интеграционных тестов.

## Как внести вклад

1. Сделайте fork или создайте отдельную ветку.
2. Внесите изменения в код или документацию.
3. Добавьте или обновите тесты, если это требуется.
4. Проверьте локальный запуск и тесты.
5. Откройте Pull Request с кратким описанием изменений.

## Авторы

- `@SV0L04ch`
- `@jakepz23`
- `@Mungums`
- `@arcteryx00`
- `@Chomachok`
  
## Лицензия

Проект распространяется под лицензией MIT. Подробности см. в файле `LICENSE`.

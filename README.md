# AI-Powered Habit Tracker Backend

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET Version](https://img.shields.io/badge/.NET-10.0-blue)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)](https://www.docker.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-4169E1?logo=postgresql)](https://www.postgresql.org/)
[![Redis](https://img.shields.io/badge/Redis-7-DC382D?logo=redis)](https://redis.io/)

Backend для AI-Powered Habit Tracker. API отвечает за регистрацию и подтверждение email, вход через JWT-cookie, управление привычками, отметки выполнения, профиль пользователя, погоду, статистику и AI-подсказки. README описывает только backend-часть проекта.

## Возможности

* Регистрация, подтверждение email, вход и выход пользователя.
* JWT-аутентификация через `HttpOnly` cookie `access_token`.
* Управление профилем: имя, город, настройки напоминаний и тема.
* Управление привычками: положительные и отрицательные привычки, триггеры по времени или количеству раз, цель по дням, штрафные дни и напоминания.
* Отслеживание прогресса: `Completed`, `Partial`, `Skipped` для положительных привычек и `RelapseCount` для отрицательных.
* Защита от дублирующей отметки привычки на одну дату.
* Погодные данные по городу через OpenWeatherMap-compatible API.
* Redis-кэш погоды.
* Ежедневная персональная сводка по привычкам, погоде и AI-комментарию.
* Анонимная городская статистика популярных привычек.
* AI-подсказки для сценариев `lazy`, `skip`, `relapse`.
* Graceful degradation: если Redis, weather API или AI endpoint недоступны, API возвращает fallback-данные и продолжает отвечать.
* Глобальная обработка ошибок через `ProblemDetails`.
* Swagger UI в Development-режиме.
* Unit и integration тесты.

## Технологический стек

- Платформа и язык: `.NET 10`, `C# 12`
- Фреймворк: `ASP.NET Core Web API`
- База данных: `PostgreSQL 15`
- ORM: `Entity Framework Core`
- Аутентификация: `ASP.NET Core Identity`, `JWT Bearer`, `HttpOnly` cookie
- Валидация: `FluentValidation`
- Кэширование: `Redis`
- Email для локальной разработки: `MailHog`
- Weather integration: `OpenWeatherMap-compatible API`
- AI integration: `Ollama`
- Документация API: `Swagger`
- Resilience: `HttpClientFactory`, `Polly`
- Тестирование: `xUnit`, `Moq`, `MockHttp`
- Контейнеризация: `Docker`, `Docker Compose`

## Быстрый старт

Эти инструкции помогут запустить backend локально для разработки и тестирования.

### Предварительные требования

* [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
* [Docker Desktop](https://www.docker.com/products/docker-desktop/)
* [Git](https://git-scm.com/)

## Установка и запуск

1. Клонируйте репозиторий:

        git clone https://github.com/SV0L04ch/AI-Powered-Habit-Tracker-mobile-app.git
        cd AI-Powered-Habit-Tracker-mobile-app

2. Создайте локальный `.env` по примеру:

        Copy-Item backend/HabitApi/.env.example .env

   Заполните значения в `.env`. Список нужных переменных уже есть в `backend/HabitApi/.env.example`.

3. Запустите backend и инфраструктуру через Docker Compose:

        docker compose --env-file .env -f backend/HabitApi/docker-compose.yml up -d --build

4. Проверьте API:

        Invoke-RestMethod http://localhost:5093/health

5. Откройте Swagger:

        http://localhost:5093/swagger

После запуска также доступны:

- API: `http://localhost:5093`
- MailHog UI: `http://localhost:8025`
- PostgreSQL: `localhost:5431`
- Redis: `localhost:6379`
- Ollama: `http://localhost:11434`

### Локальный запуск API без контейнера

Если инфраструктура уже поднята в Docker, API можно запустить напрямую:

        docker compose --env-file .env -f backend/HabitApi/docker-compose.yml up -d db redis mailhog ollama
        dotnet ef database update
        dotnet run --project backend/HabitApi/HabitApi.csproj

API будет доступно по адресу `http://localhost:5093`.

### Структура проекта

    backend/
    |-- HabitApi/
    |   |-- Controllers/          # HTTP endpoints
    |   |-- Data/                 # DbContext и EF Core mapping
    |   |-- Exceptions/           # Доменные исключения
    |   |-- Migrations/           # EF Core migrations
    |   |-- Models/
    |   |   |-- Domain/           # EF/Identity domain models
    |   |   `-- DTO/              # Request/response DTO
    |   |-- Services/             # Бизнес-логика и внешние интеграции
    |   |   `-- Interfaces/       # Контракты сервисов
    |   |-- Validators/           # FluentValidation validators
    |   |-- Program.cs            # DI, middleware, auth, CORS, Swagger
    |   |-- Dockerfile
    |   |-- docker-compose.yml
    |   `-- docker-compose.ci.yml
    |-- HabitApi.Tests/
    |   |-- Controllers/
    |   |-- Services/
    |   |-- Validators/
    |   `-- Integration/
    `-- HabitTracker.slnx

## Использование API

### Аутентификация

1. Зарегистрируйте пользователя:

        POST /api/auth/register

2. Подтвердите email по ссылке из письма в MailHog:

        GET /api/auth/confirm-email

3. Выполните вход:

        POST /api/auth/login

После успешного входа API устанавливает JWT в `HttpOnly` cookie `access_token`. Защищенные эндпоинты читают токен из этой cookie.

### Основные эндпоинты

- `GET /api/profile` - получить профиль пользователя
- `PUT /api/profile` - обновить профиль пользователя
- `GET /api/habits` - получить привычки пользователя
- `POST /api/habits` - создать привычку
- `GET /api/habits/{habitId}` - получить привычку
- `PUT /api/habits/{habitId}` - обновить привычку
- `DELETE /api/habits/{habitId}` - мягко удалить привычку
- `GET /api/habits/{habitId}/entries` - получить отметки за период
- `POST /api/habits/{habitId}/entries` - добавить отметку выполнения
- `PUT /api/habits/{habitId}/entries/{entryId}` - обновить отметку
- `DELETE /api/habits/{habitId}/entries/{entryId}` - удалить отметку
- `GET /api/weather?city=Samara&date=2026-05-28` - получить погоду
- `GET /api/stats/daily-summary?date=2026-05-28` - получить ежедневную сводку
- `GET /api/stats/city-summary?city=Samara` - получить городскую статистику
- `POST /api/habits/{habitId}/insights/support` - получить AI-подсказку
- `GET /health` - health check API

Полный список эндпоинтов и DTO доступен в Swagger после запуска приложения.

### Enum-значения в JSON

- `triggerType`: `1` = `TimeOfDay`, `2` = `CountPerDay`
- `status`: `1` = `Completed`, `2` = `Partial`, `3` = `Skipped`

## Контейнеризация

Backend готов к запуску через Docker Compose.

Конфигурация включает:

- `api` - ASP.NET Core backend
- `db` - PostgreSQL 15
- `redis` - Redis 7
- `mailhog` - SMTP и web UI для писем подтверждения
- `ollama` - локальный AI runtime

Запуск:

        docker compose --env-file .env -f backend/HabitApi/docker-compose.yml up -d --build

Остановка:

        docker compose --env-file .env -f backend/HabitApi/docker-compose.yml down

Для CI есть отдельный compose-файл:

        backend/HabitApi/docker-compose.ci.yml

## Тестирование

Запуск backend-тестов:

        dotnet test backend/HabitApi.Tests/HabitApi.Tests.csproj --no-restore

Если зависимости еще не восстановлены:

        dotnet test backend/HabitApi.Tests/HabitApi.Tests.csproj

Что покрыто тестами:

- контроллеры: Auth, Habits, HabitEntries, Profile, Stats, Weather, Insights
- сервисы: auth, habits, entries, profile, stats, weather, email, AI insights
- validators DTO
- integration workflow для backend и PostgreSQL

## Roadmap backend

- Вынести production secrets из tracked config в безопасное хранилище.
- Применить rate limiting к auth-эндпоинтам.
- Усилить password policy и lockout.
- Вынести HTML confirmation page из `AuthController`.
- Разделить AI provider logic через Strategy/Adapter.
- Уточнить production Docker profile без публикации внутренних сервисов наружу.
- Добавить security headers для production.

## Как внести вклад

1. Форкните репозиторий.
2. Создайте ветку для изменения backend.
3. Внесите изменения и добавьте тесты.
4. Убедитесь, что backend-тесты проходят.
5. Откройте Pull Request в ветку `develop`.

## Лицензия

Проект распространяется под лицензией MIT. Подробности смотрите в файле [LICENSE](LICENSE).

## Авторы

`@SV0L04ch`, `@jakepz23` - backend разработка и архитектура.

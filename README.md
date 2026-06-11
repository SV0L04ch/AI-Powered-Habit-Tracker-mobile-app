# AI-Powered Habit Tracker

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET Version](https://img.shields.io/badge/.NET-10.0-blue)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-19-61DAFB?logo=react)](https://react.dev/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)](https://www.docker.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-4169E1?logo=postgresql)](https://www.postgresql.org/)
[![Redis](https://img.shields.io/badge/Redis-7-DC382D?logo=redis)](https://redis.io/)

AI-Powered Habit Tracker — full-stack приложение для отслеживания привычек с AI-подсказками, gamification, социальными функциями и premium дизайном.

## Возможности

### Backend (15 контроллеров, 16 сервисов)
* Регистрация, подтверждение email (Gmail для production, MailHog для dev), JWT-cookie аутентификация.
* Управление привычками: положительные/отрицательные, триггеры по времени/количеству, штрафные дни, цвета.
* Стрики и Gamification: XP, уровни (1-15), 8 достижений, виртуальная валюта (HabitCoins).
* Шаблоны привычек: 10 seeded шаблонов (Fitness, Mindfulness, Productivity, Health, Learning).
* Планировщик: daily/weekdays/custom расписание с исключениями.
* AI-подсказки и ежедневная сводка с погодой.
* Социальные функции: лента города, друзья, вызовы.
* Журнал: заметки, настроение, сон, питание, цели (OKR).
* Сервис цитат: 10 мотивационных цитат.
* Оптимизации: Serilog, сжатие Brotli/Gzip, rate limiting (auth/ai/default), health checks, output cache, Mapster, MediatR CQRS.

### Frontend (React 19 + Vite 8, PWA)
* **Landing Page**: 9 секций (Hero, Features, InteractiveDemo, VideoShowcase, AppPreview3D, HowItWorks, SocialProof, CTA, Footer) с premium анимациями.
* **Premium дизайн**: Claude.ai-inspired тёплая палитра (#faf8f5, #d97706, #059669), spring physics анимации, skeleton loading.
* **i18n**: English + Russian переводы.
* **Компоненты**: VoiceButton, MoodPicker, Soundscapes, MeditationTimer, StreakShareVideo, QRCode, ErrorBoundary, OfflineIndicator, PushNotifications.
* **Оптимизация**: Code splitting (React.lazy), bundle analysis, service worker, prefetching.

## Технологический стек

### Backend
- Платформа: `.NET 10`, `C# 12`
- Фреймворк: `ASP.NET Core Web API`
- База данных: `PostgreSQL 15` + `Entity Framework Core`
- Аутентификация: `ASP.NET Core Identity`, `JWT Bearer`, `HttpOnly` cookie
- Валидация: `FluentValidation`
- Кэширование: `Redis`
- Маппинг: `Mapster`
- CQRS: `MediatR`
- Логирование: `Serilog`
- Resilience: `Polly`, `HttpClientFactory`
- Observability: `OpenTelemetry`
- Тестирование: `xUnit`, `Moq`, `Testcontainers`
- Контейнеризация: `Docker`, `Docker Compose`

### Frontend
- Фреймворк: `React 19` + `Vite 8`
- Состояние: `Zustand`
- Анимации: `Framer Motion`
- Стили: `SCSS` + CSS Custom Properties
- Графики: `Recharts`
- i18n: `react-i18next`
- PWA: `vite-plugin-pwa`
- Тестирование: `Vitest`, `Playwright`

## Быстрый старт

### Предварительные требования

* [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
* [Node.js 20+](https://nodejs.org/)
* [Docker Desktop](https://www.docker.com/products/docker-desktop/)
* [Git](https://git-scm.com/)

### 1. Клонируйте репозиторий

```bash
git clone https://github.com/SV0L04ch/AI-Powered-Habit-Tracker-mobile-app.git
cd AI-Powered-Habit-Tracker-mobile-app
```

### 2. Настройте окружение

```bash
Copy-Item backend/HabitApi/.env.example .env
```

Отредактируйте `.env` — заполните `DB_PASSWORD`, `JWT_SECRET` (минимум 32 символа), `WEATHER_API_KEY`, `AI_API_KEY`.

Для Gmail-рассылки добавьте:
```
GMAIL_SENDER_EMAIL=your-app@gmail.com
GMAIL_APP_PASSWORD=your-app-specific-password
```

### 3. Установите зависимости

```bash
# Backend
dotnet restore

# Frontend
cd frontend
npm install
cd ..
```

### 4. Запустите инфраструктуру

```bash
docker compose --env-file .env -f backend/HabitApi/docker-compose.yml up -d db redis mailhog ollama
```

### 5. Примените миграции

```bash
dotnet ef database update --project backend/HabitApi/HabitApi.csproj
```

### 6. Запустите backend

```bash
dotnet run --project backend/HabitApi/HabitApi.csproj
```

API: `http://localhost:5093` | Swagger: `http://localhost:5093/swagger`

### 7. Запустите frontend

```bash
cd frontend
npm run dev
```

Frontend: `http://localhost:5173` | Landing Page: `http://localhost:5173/`

### Или всё сразу через Docker

```bash
docker compose --env-file .env -f backend/HabitApi/docker-compose.yml up -d --build
```

### Доступные сервисы

| Сервис | URL |
|--------|-----|
| Frontend (Landing Page) | http://localhost:5173/ |
| Frontend (Dashboard) | http://localhost:5173/habits |
| Backend API | http://localhost:5093 |
| Swagger UI | http://localhost:5093/swagger |
| Health Check | http://localhost:5093/health |
| MailHog UI | http://localhost:8025 |
| PostgreSQL | localhost:5431 |
| Redis | localhost:6379 |
| Ollama (AI) | http://localhost:11434 |

### Структура проекта

```
backend/
|-- HabitApi/
|   |-- Controllers/          # 15 контроллеров (Auth, Habits, Profile, Social, Journal...)
|   |-- Data/                 # AppDbContext, EF Core mapping
|   |-- Exceptions/           # Доменные исключения
|   |-- Extensions/           # Расширения (ClaimsPrincipal и др.)
|   |-- Features/             # MediatR CQRS queries (Streaks, Gamification)
|   |-- Mappings/             # Mapster mapping configurations
|   |-- Migrations/           # EF Core migrations
|   |-- Models/
|   |   |-- Domain/           # 20+ моделей (Habit, Streak, Achievement, Challenge...)
|   |   `-- DTO/              # Request/response DTO
|   |-- Services/             # 16 сервисов (Auth, Habit, Streak, Gamification, Social...)
|   |   `-- Interfaces/       # Контракты сервисов
|   |-- Validators/           # FluentValidation validators
|   |-- Program.cs            # DI, middleware, auth, compression, rate limiting
|   |-- Dockerfile
|   |-- docker-compose.yml
|   `-- docker-compose.ci.yml
|-- HabitApi.Tests/
|   |-- Controllers/
|   |-- Services/
|   |-- Validators/
|   `-- Integration/
`-- HabitTracker.slnx

frontend/
|-- src/
|   |-- components/           # 20+ компонентов (Button, Modal, VoiceButton, MoodPicker...)
|   |-- pages/                # 8 страниц (Landing, Login, Register, Habits, Insights...)
|   |   |-- LandingPage/      # Landing с 9 секциями
|   |   |-- HabitsPage/       # Dashboard привычек
|   |   |-- PersonalInsightsPage/
|   |   |-- CityInsightsPage/
|   |   |-- ProfilePage/
|   |   |-- LoginPage/
|   |   |-- RegisterPage/
|   |   `-- CreateHabitPage/
|   |-- store/                # Zustand stores
|   |-- services/             # API клиенты
|   |-- styles/               # SCSS (_animations.scss, main.scss)
|   |-- i18n/                 # EN/RU переводы
|   `-- lib/                  # Утилиты
|-- playwright.config.cjs
`-- vite.config.js
```

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

**Auth & Profile:**
- `POST /api/auth/register` - регистрация (Gmail валидация)
- `POST /api/auth/login` - вход
- `GET /api/profile` - профиль
- `PUT /api/profile` - обновить профиль

**Habits & Entries:**
- `GET/POST /api/habits` - список/создание привычек
- `GET/PUT/DELETE /api/habits/{id}` - CRUD привычки
- `GET/POST /api/habits/{id}/entries` - отметки выполнения
- `GET /api/schedule/today` - сегодняшние привычки по расписанию

**Gamification:**
- `GET /api/streaks` - все стрики
- `GET /api/streaks/{habitId}` - стрик привычки
- `GET /api/gamification` - XP, уровень, достижения
- `GET /api/economics/wallet` - баланс HabitCoins

**Social:**
- `GET /api/social/feed?city=Samara` - лента города
- `POST /api/social/friends/{friendId}` - запрос дружбы
- `GET /api/social/challenges` - список вызовов

**Journal & Wellness:**
- `POST /api/journal/notes/{habitId}` - заметка
- `POST /api/journal/mood` - лог настроения
- `POST /api/journal/sleep` - лог сна
- `POST /api/journal/meals` - лог еды
- `GET/POST /api/journal/goals` - цели (OKR)

**AI & Weather:**
- `GET /api/weather?city=Samara&date=2026-06-11` - погода
- `GET /api/stats/daily-summary` - ежедневная сводка
- `POST /api/habits/{id}/insights/support` - AI-подсказка
- `GET /api/quotes/daily` - цитата дня

**Templates & Schedule:**
- `GET /api/templates?category=Fitness` - шаблоны привычек
- `PUT /api/schedule/{habitId}` - настроить расписание

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

### Backend тесты

```bash
# Unit тесты (без Docker)
dotnet test backend/HabitApi.Tests/HabitApi.Tests.csproj

# Integration тесты (требует Docker + Testcontainers)
backend/scripts/Run-BackendIntegrationTests.ps1
```

### Frontend тесты

```bash
# Unit тесты (vitest)
cd frontend && npx vitest run

# E2E тесты (Playwright, требует запущенный backend + frontend)
npm run e2e-tests:ordered
```

### Что покрыто тестами

- контроллеры: Auth, Habits, HabitEntries, Profile, Stats, Weather, Insights, Streaks, Gamification, Templates, Quotes, Schedule, Economics, Social, Journal
- сервисы: auth, habits, entries, profile, stats, weather, email, AI, streak, gamification, templates, quotes, schedule, economics, social, journal
- validators DTO
- integration workflow для backend и PostgreSQL

## Roadmap

### Реализовано
- ✅ Landing Page с 9 секциями и premium анимациями
- ✅ Тёплая Claude.ai-inspired цветовая палитра
- ✅ Gmail SMTP routing + валидация
- ✅ Streak & Gamification (XP, уровни, 8 достижений)
- ✅ 10 шаблонов привычек
- ✅ Планировщик (daily/weekdays/custom)
- ✅ Цветовая кастомизация привычек
- ✅ Виртуальная валюта (HabitCoins)
- ✅ Социальные функции (лента, друзья, вызовы)
- ✅ Журнал (заметки, настроение, сон, питание, цели)
- ✅ Голосовые команды (Web Speech API)
- ✅ Meditation Timer с дыхательным гайдом
- ✅ Soundscapes (5 звуков)
- ✅ Offline-индикатор + Push-уведомления
- ✅ QR code sharing
- ✅ i18n (EN/RU)
- ✅ Error Boundaries, Bundle Analysis, Skeleton Loading
- ✅ Serilog, Compression, Rate Limiting, Health Checks, Output Cache
- ✅ Mapster + MediatR CQRS

### Будущее
- Видео-рекап недели с canvas-рендерингом
- 3D визуализация привычек (Three.js)
- AR-режим для привычек
- A/B тестирование
- Multi-device синхронизация
- Интеграция с Apple Health / Google Fit

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

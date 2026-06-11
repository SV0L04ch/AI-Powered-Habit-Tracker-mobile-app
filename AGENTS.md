# AGENTS.md

## Repo overview

AI-Powered Habit Tracker — monorepo with a .NET 10 backend API and a React 19 + Vite 8 frontend (PWA). Backend communicates with PostgreSQL, Redis, Ollama (AI), and OpenWeatherMap. Frontend is a mobile-first SPA.

## Project layout

```
backend/HabitApi/          # ASP.NET Core Web API (entrypoint: Program.cs)
backend/HabitApi.Tests/    # xUnit unit + integration tests
backend/scripts/           # PowerShell scripts for integration tests
frontend/                  # React + Vite + Zustand + SCSS
frontend/end-to-end-tests/ # Playwright E2E (scenarious/ + smoke/)
end-to-end-tests/          # Legacy Playwright specs (root-level, IGNORE)
postman/                   # Newman test collections
artefacts/                 # Design docs (tech spec, ER diagram)
```

Backend solution: `backend/HabitTracker.slnx` (XML format, not traditional `.sln`).

## Backend commands

```bash
# Run backend tests (unit only — no Docker required)
dotnet test backend/HabitApi.Tests/HabitApi.Tests.csproj

# Run backend integration tests (requires Docker, spins up Testcontainers)
# PowerShell only — sets RUN_BACKEND_INTEGRATION_TESTS=1 and starts Postgres container
backend/scripts/Run-BackendIntegrationTests.ps1

# Run LLM integration tests (requires Docker, spins up Ollama + Postgres)
backend/scripts/Run-LlmIntegrationTests.ps1

# Start infra only (Postgres, Redis, MailHog, Ollama) — no API container
docker compose --env-file .env -f backend/HabitApi/docker-compose.yml up -d db redis mailhog ollama

# Run API outside container (needs infra from above)
dotnet ef database update --project backend/HabitApi/HabitApi.csproj
dotnet run --project backend/HabitApi/HabitApi.csproj

# Full stack via Docker Compose
docker compose --env-file .env -f backend/HabitApi/docker-compose.yml up -d --build
```

## Frontend commands

```bash
# Dev server (proxies /api to localhost:5093 via Vite)
cd frontend && npm run dev

# Lint
cd frontend && npm run lint

# Format
cd frontend && npm run format

# Build
cd frontend && npm run build

# Unit tests (vitest, configured in vite.config.js — jsdom environment)
cd frontend && npx vitest run
```

## E2E tests (Playwright)

E2E tests must run in strict sequential order. Use the ordered runner:

```bash
npm run e2e-tests:ordered   # from repo root
```

This runs `frontend/end-to-end-tests/scenarious/*.spec.js` (register → login → habits → profile) then `smoke/*.spec.js`. Workers=1, retries=2 in CI.

Playwright config: `frontend/playwright.config.cjs`. Scenario tests run on Chromium; smoke tests run on Firefox, WebKit, and Mobile Chrome (Pixel 5).

Base URL: `http://localhost:5173`. Both frontend dev server and backend API must be running.

## CI workflows

Two GitHub Actions workflows on push/PR to `main`:

- **Playwright Tests** (`playwright.yml`): Starts DB + Redis + MailHog via `docker-compose.ci.yml`, applies EF migrations, starts API, installs frontend, runs `npm run e2e-tests:ordered`.
- **Postman API Tests** (`postman.yml`): Same infra setup, runs Newman collections (setup first, then main tests).

Both use the CI compose file: `backend/HabitApi/docker-compose.ci.yml` (includes a `weather_mock` service instead of real OpenWeatherMap).

## Environment setup

1. Copy `backend/HabitApi/.env.example` to `.env` at repo root
2. Required vars: `DB_PASSWORD`, `JWT_SECRET` (min 32 chars), `WEATHER_API_KEY`, `AI_API_KEY`
3. Backend loads `.env` from CWD via DotNetEnv (`Program.cs:25`)
4. Frontend does not need a `.env` — the API client uses `/api` as baseURL and Vite's dev-server proxy forwards it to `localhost:5093` (see `vite.config.js`).

**Do not commit `.env` files.** They are gitignored.

## Key architecture facts

- **Auth**: JWT in HttpOnly cookie `access_token`. No `Authorization` header — the backend reads the cookie directly (`Program.cs:112`). Protected endpoints require this cookie.
- **Backend API port**: 5093 (external) → 8080 (container). Frontend dev server: 5173.
- **Infra ports**: PostgreSQL 5431, Redis 6379, MailHog 8025, Ollama 11434.
- **Docker Compose files**: `docker-compose.yml` (full stack with Ollama), `docker-compose.ci.yml` (CI without Ollama, uses weather mock).
- **Integration tests are gated**: Unit tests run by default. Integration tests require `RUN_BACKEND_INTEGRATION_TESTS=1` (backend) or `RUN_LLM_INTEGRATION_TESTS=1` (LLM) and use Testcontainers to spin up ephemeral Postgres/Ollama.
- **PWA**: Frontend is a PWA with auto-update service worker (vite-plugin-pwa). Splash screen shown once per session.
- **State management**: Zustand stores in `frontend/src/store/`.
- **Styling**: SCSS in `frontend/src/styles/`, no Tailwind.
- **Frontend tests**: vitest for unit tests (`frontend/src/tests/`), Playwright for E2E.
- **Backend validation**: FluentValidation — validators in `backend/HabitApi/Validators/`.

## Gotchas

- The solution file is `HabitTracker.slnx` (XML format). Do not look for `.sln`.
- Backend `appsettings.json` contains placeholder/dummy credentials. Real values come from `.env` via `DotNetEnv`.
- E2E tests are order-dependent (register creates the user that login/habits/profile specs use). Do not reorder or skip files.
- `docker-compose.ci.yml` uses a `weather_mock` service — do not use it for local dev where you want real weather data.
- Backend integration tests use `Testcontainers` — they require Docker Desktop running.
- The root `end-to-end-tests/` directory contains legacy Playwright specs; the active ones are in `frontend/end-to-end-tests/`. Do not run tests from the root `end-to-end-tests/`.
- The root `playwright.config.js` is a legacy config pointing at `./end-to-end-tests` — the active config is `frontend/playwright.config.cjs`.
- Root-level `HabitApi/` and `HabitApi.Tests/` directories contain only `bin/obj` build artifacts. Actual source is in `backend/HabitApi/` and `backend/HabitApi.Tests/`.
- Frontend `pnpm-lock.yaml` exists alongside `package-lock.json` — CI uses npm, stick with npm.
- Root `package.json` has a broken `test:e2e` script referencing `QA/e2e` which doesn't exist. Use `npm run e2e-tests:ordered` instead.
- Root `package.json` `dev:backend` runs `cd backend && dotnet run` which will fail (no project in `backend/`). Use `dotnet run --project backend/HabitApi/HabitApi.csproj` instead.

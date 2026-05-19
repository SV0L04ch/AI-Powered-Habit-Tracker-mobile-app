# Проектный практикум — Трекер привычек с ИИ-поддержкой

## 📌 Роль QA: артефакты и автоматизация

В этом документе собраны все результаты работы по тестированию проекта. Здесь вы найдёте как артефакты ручного тестирования (тест-план, тест-кейсы, чек-листы, баг-репорты), так и описание автоматизированных тестов (API, E2E, нагрузочных) с инструкциями по запуску.

---

## 🗂️ Структура QA-документации

Все артефакты находятся в папке [`qa-docs/`](./qa-docs) (или в корне репозитория, если иное не указано).

| Файл | Описание |
|------|----------|
| [test-plan.md](./qa-docs/test-plan.md) | Тест-план (scope, виды, приоритеты, severity) |
| [test-cases.md](./qa-docs/test-cases.md) | 20 тест-кейсов с результатами Passed |
| [smoke-checklist.md](./qa-docs/smoke-checklist.md) | Smoke-чек-лист (10 проверок) |
| [regression-checklist.md](./qa-docs/regression-checklist.md) | Регрессионный чек-лист (24 проверки) |
| [api-endpoints.md](./qa-docs/api-endpoints.md) | Список API-эндпоинтов (18 эндпоинтов) |
| [bug-report-template.md](./qa-docs/bug-report-template.md) | Шаблон баг-репорта |
| [regression-api-report.md](./qa-docs/regression-api-report.md) | Результаты регрессии API (таблицы) |
| [retest-report.md](./qa-docs/retest-report.md) | Отчёт по ретесту дефектов (все исправлены) |
| [test-report.md](./qa-docs/test-report.md) | Итоговый отчёт о тестировании |

---

## 🤖 Автоматизированное тестирование

### 1. API-тесты (Postman + Newman)

**Коллекции и окружение** находятся в папке [`postman/`](../postman).

- [`AI-Powered-Habit-Tracker-setup-tests.json`](../postman/AI-Powered-Habit-Tracker-setup-tests.json) – подготовительные тесты (регистрация, логин, создание привычек).
- [`AI-Powered-Habit-Tracker-main-tests.json`](../postman/AI-Powered-Habit-Tracker-main-tests.json) – основные тесты (CRUD привычек, статистика, погода).
- [`AI-Powered-Habit-Tracker-Environment.json`](../postman/AI-Powered-Habit-Tracker-Environment.json) – окружение (baseUrl, переменные).

**Локальный запуск:**
```bash
newman run postman/AI-Powered-Habit-Tracker-setup-tests.json -e postman/AI-Powered-Habit-Tracker-Environment.json --export-environment postman/AI-Powered-Habit-Tracker-Environment.json
newman run postman/AI-Powered-Habit-Tracker-main-tests.json -e postman/AI-Powered-Habit-Tracker-Environment.json
```

### 2. E2E-тесты (Playwright)

Тесты находятся в папке `end-to-end-tests/`.

- [`register.spec.js`](../end-to-end-tests/register.spec.js) - регистраиця, подтверждение email при помощи MailHog.
- [`login.spec.js`](../end-to-end-tests/login.spec.js) - логин.
- [`habits.spec.js`](../end-to-end-tests/habits.spec.js) - создание, редактирование, удаление привычек.

**Локальный запуск**
```bash
npm ci
npx playwright install chromium --with-deps
npx playwright tset
```

### 3. Нагрузочные тесты

Сценарии находятся в папке `load-tests/'.

- [`scenarious/register.js`](../load-tests/scenarious/register.js) - регистрация + подтверждение email через MailHog.
- [`scenarious/login.js`](../load-tests/scenarious/login.js) - логин (с предварительно созданными пользователями).

**Локальный запуск**

```bash
k6 run load-tests/scenarios/register.js
k6 run load-tests/scenarios/login.js
```

**Примечание:** нагрузочные тесты требуют запущенного окружения (API, БД, MailHog, Redis, LLM). Рекомендуется запускать через `docker-compose up -d`.

## 🐞 Баг-репорты

На данный момент заведено 7 баг-репортов.

Примеры критических багов:
1. **Регистрация нового пользователя возвращает 500, хотя запись в БД создаётся** (клиент не может распознать успех, сценарий регистрации блокируется)
2. **Регистрация с email длиннее 256 символов возвращает 500, хотя должна 400** (ошибка валидации, приводит к исключению в БД)
3. **Погодный API: 429/404 → API возвращает 200 с заглушкой вместо соответствующего статуса**  
   (не проксируются ошибки внешнего сервиса)

Все баги оформлены по единому шаблону (см. [`bug-report-template.md`](./qa-docs/bug-report-template.md)) и содержат шаги воспроизведения, ожидаемый/фактический результат, severity/priority, окружение и вложения.

## 🔄 Интеграция с CI/CD (GitHub Actions)
В репозитории настроены следующие workflows:
| Workflow | Событие | Что делает |
| :---: | :---: | :---: |
| `playwright.yml` | push / PR в `main` | Запускает E2E-тесты Playwright (с поднятием API через Docker Compose) |
| `postman-tests.yml` | push / PR в `main` | Запускает API-тесты Postman через Newman

## 📊 Итоговый отчёт

Финальный отчёт о тестировании будет представлен в файле [`test-report.md`](../qa-docs/test-report.md). Он включит:

- Результаты ручного и автоматизированного тестирования

- Статистику по багам (найдено, исправлено, осталось)

- Оценку покрытия автотестами

- Риски и рекомендации к релизу

---
_Документация поддерживается в актуальном состоянии в рамках проектного практикума (последнее обновление: 16-05-2026)_
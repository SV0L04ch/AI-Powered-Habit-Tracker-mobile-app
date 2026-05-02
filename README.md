# AI-Powered Habit Tracker

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET Version](https://img.shields.io/badge/.NET-10.0-blue)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)](https://www.docker.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-4169E1?logo=postgresql)](https://www.postgresql.org/)

Backend для мобильного трекера привычек с поддержкой искусственного интеллекта. Пользователи могут формировать полезные привычки, отслеживать их выполнение и получать персонализированные советы от ИИ. Проект включает интеграцию с погодным API для анализа связей между привычками и погодой, а также анонимную городскую статистику.

## Возможности

*   Управление привычками: добавление положительных и отрицательных привычек с категориями (со штрафами / развлекательные).
*   Гибкая настройка: поддержка триггеров (время дня / количество раз) и напоминаний.
*   Система тегов: категоризация привычек для удобной фильтрации.
*   Отслеживание прогресса: отметки выполнения с поддержкой частичного выполнения и количества срывов.
*   Система штрафов: автоматическое начисление штрафных дней за пропуски для привычек категории "со штрафом".
*   ИИ-поддержка: генерация мотивирующих сообщений и советов при лени, срывах или пропусках.
*   Аналитика: ежедневная персональная сводка с анализом связи привычек с погодой через ИИ.
*   Городская статистика: анонимная сводка популярных привычек в вашем городе.
*   Аутентификация: JWT-токены с хешированием паролей.
*   Контейнеризация: Docker и Docker Compose для простого развертывания.

##  Технологический стек

- Платформа и Язык программирования: .NET 10 / C# 12

- Фреймворк:    NET Core Web API
  
- База данных:  PostgreSQL 15 с Entity Framework Core
  
- Аутентификация:  NET Core Identity + JWT
  
- Документация: API  Swagger
  
- Контейнеризация: Docker, Docker Compose
  
- Кэширование:  Redis
  
- Тестирование:  xUnit, Moq

- Интеграционное тестирование: Testcontainers

- Миграция: EF Core Migrations

- Валидация: FluentValidation / Data Annotations

## Быстрый старт

Эти инструкции помогут вам запустить локальную копию проекта для разработки и тестирования.

### Предварительные требования

*   [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
*   [Docker Desktop](https://www.docker.com/products/docker-desktop/)
*   [Git](https://git-scm.com/)
*   [PostgreSQL](https://www.postgresql.org/)

## Установка и запуск

1. Клонируйте репозиторий:
   
        git clone https://github.com/SV0L04ch/AI-Powered-Habit-Tracker-mobile-app.git
        cd AI-Powered-Habit-Tracker-mobile-app

2. Запустите базу данных PostgreSQL в Docker:

        docker-compose up -d db

3. Настройте переменные окружения:

   - используйте User Secrets для локальной разработки:
      ```
      dotnet user-secrets init <br>
      dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=habit_tracker;Username=your_user;Password=your_password"<br>
      dotnet user-secrets set "Jwt:Secret" "your_super_secret_key_32_chars_min"<br>
      dotnet user-secrets set "WeatherApi:ApiKey" "your_openweathermap_api_key"<br>
      dotnet user-secrets set "AiApi:ApiKey" "your_llm_api_key"<br>```


4. Примените миграции базы данных:<br>

        dotnet ef database update

6. Запустите приложение:<br>

        dotnet run

    API будет доступно по адресу: https://localhost:5093.

8. Просмотр документации API:

    Swagger UI: https://localhost:5093/swagger

## Запуск с помощью Docker Compose:
### Для запуска и API, и базы данных в контейнерах:

    docker-compose up -d

После этого API будет доступно по адресу http://localhost:5093.

### Структура проекта:

    HabitApi/
    ├──  Controllers/        # Обработка HTTP-запросов
    ├── Data/               # DbContext и конфигурация базы данных
    ├── Models/<br>
    │   ├── Domain/         # Сущности базы данных
    │   └── DTO/            # Объекты передачи данных
    ├── Services/           # Бизнес-логика и внешние интеграции
    │   └── Interfaces/     # Контракты для DI
    ├── Middleware/         # Кастомные middleware
    ├── Helpers/            # Вспомогательные классы
    ├── Extensions/         # Методы расширения для ServiceCollection
    ├── Program.cs          # Точка входа и конфигурация приложения
    ├── appsettings.json    # Базовая конфигурация
    ├── Dockerfile          # Инструкция для сборки Docker-образа
    └── docker-compose.yml  # Оркестрация контейнеров


## Использование API:

### Аутентификация:

1. Зарегистрируйте нового пользователя:<br>

   POST /api/auth/register

3. Получите JWT-токен:<br>

   POST /api/auth/login

4. Вставьте полученный токен в Swagger UI (кнопка "Authorize") или используйте заголовок:<br>

   Authorization: Bearer <your_jwt_token>


### Основные эндпоинты:


- GET  /api/habits  (Получить все привычки пользователя)<br>

- POST  /api/habits  (Создать новую привычку)<br>
 
- POST  /api/habits/{habitId}/entries  (Добавить отметку выполнения)<br>

- GET  /api/stats/daily-summary  (Получить ежедневную сводку)<br>

- GET  /api/stats/city-summary?city=Moscow  (Получить сводку по городу)<br>

- POST  /api/habits/{habitId}/insights/support  (Получить ИИ-совет)<br>

Полный список эндпоинтов и примеры тел запросов доступны в документации Swagger после запуска приложения. <br>



## Контейнеризация:

Проект полностью готов к запуску в Docker-контейнерах. <br>

Конфигурация включает:<br>


  - PostgreSQL 15: база данных с постоянным хранилищем.


  - Habit API: приложение, собранное на основе многоступенчатого Dockerfile.


  - Healthcheck: проверка готовности базы данных перед запуском API.


  - Переменные окружения: безопасная передача конфиденциальных данных. <br> <br>


### docker-compose.yml:

    yaml
    services:
      db:
        image: postgres:15-alpine
        container_name: habit_tracker_db
        environment:
          POSTGRES_USER: habit_user
          POSTGRES_PASSWORD: your_password
          POSTGRES_DB: habit_tracker
        ports:
          - "5432:5432"
        volumes:
          - postgres_data:/var/lib/postgresql/data
        healthcheck:
          test: ["CMD-SHELL", "pg_isready -U habit_user -d habit_tracker"]
          interval: 5s
          timeout: 5s
          retries: 5
    
      api:
        build: .
        container_name: habit_api
        ports:
          - "5093:8080"
        depends_on:
          db:
            condition: service_healthy
        environment:
          - ASPNETCORE_ENVIRONMENT=Development
          - ConnectionStrings__DefaultConnection=Host=db;Port=5432;Database=habit_tracker;Username=habit_user;Password=your_password


## Тестирование:

### Для запуска юнит-тестов:

    dotnet test

### Что покрыто тестами:

- Контроллеры (Auth, Habits, HabitEntries)

- Сервисы (Business logic, репозитории)

- Валидация DTO

- Middleware (глобальная обработка ошибок)


## Roadmap:

- Интеграция с реальным AI API (GroqCloud)

- Push-уведомления для напоминаний

## Как внести вклад:

Мы приветствуем вклад в проект! Пожалуйста, следуйте этим шагам:

  - Форкните репозиторий.

  - Создайте ветку для вашей функции: git checkout -b feature/amazing-feature.

  - Внесите изменения и добавьте тесты.

  - Убедитесь, что все тесты проходят: dotnet test.

  - Запушьте ветку: git push origin feature/amazing-feature.

  - Откройте Pull Request в ветку develop.

## Лицензия:

  Проект распространяется под лицензией MIT. Подробности смотрите в файле LICENSE.

## Авторы:

@SV0L04ch, @jakepz23 - Backend разработка и архитектура.
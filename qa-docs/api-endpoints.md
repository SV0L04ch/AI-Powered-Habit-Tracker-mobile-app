# Список API-эндпоинтов для тестирования

Полный перечень эндпоинтов согласно OpenAPI (Swagger) спецификации.  
Базовый URL: `http://localhost:5093/api`

| Метод | Эндпоинт | Описание | Параметры / Тело | Ожидаемые статусы |
|-------|----------|----------|------------------|-------------------|
| POST | `/auth/register` | Регистрация пользователя | `RegisterRequestDto` (email, password, city) | 201 – Created<br>400 – Bad Request<br>409 – Conflict |
| GET | `/auth/confirm-email` | Подтверждение email | query: `userId` (uuid), `token` (string) | 200 – OK |
| POST | `/auth/login` | Вход в систему | `LoginRequestDto` (email, password) | 200 – OK (возвращает `AuthResponseDto`)<br>401 – Unauthorized |
| POST | `/auth/logout` | Выход из системы | – (требуется токен) | 204 – No Content |
| GET | `/habits` | Получить список привычек | – (требуется токен) | 200 – OK (массив `HabitDto`)<br>401 – Unauthorized |
| POST | `/habits` | Создать привычку | `CreateHabitDto` (name, isPositive, hasPenalty, triggerType, triggerValue, targetDays, penaltyDaysPerMiss, reminders) | 201 – Created (`HabitDto`)<br>400 – Bad Request<br>401 – Unauthorized |
| GET | `/habits/{habitId}` | Получить привычку по ID | path: `habitId` (uuid) | 200 – OK (`HabitDto`)<br>401 – Unauthorized<br>404 – Not Found |
| PUT | `/habits/{habitId}` | Обновить привычку | path: `habitId` (uuid)<br>body: `UpdateHabitDto` | 200 – OK (`HabitDto`)<br>400 – Bad Request<br>401 – Unauthorized<br>404 – Not Found |
| DELETE | `/habits/{habitId}` | Удалить привычку | path: `habitId` (uuid) | 204 – No Content<br>401 – Unauthorized<br>404 – Not Found |
| GET | `/habits/{habitId}/entries` | Получить записи выполнения привычки | path: `habitId` (uuid)<br>query: `fromDate` (date, optional), `toDate` (date, optional) | 200 – OK (массив `HabitEntryDto`)<br>401 – Unauthorized<br>404 – Not Found |
| POST | `/habits/{habitId}/entries` | Добавить запись выполнения | path: `habitId` (uuid)<br>body: `CreateHabitEntryDto` (date, status, partialValue, relapseCount, note) | 201 – Created (`HabitEntryDto`)<br>400 – Bad Request<br>401 – Unauthorized<br>404 – Not Found |
| PUT | `/habits/{habitId}/entries/{entryId}` | Обновить запись выполнения | path: `habitId` (uuid), `entryId` (uuid)<br>body: `UpdateHabitEntryDto` | 200 – OK (`HabitEntryDto`)<br>400 – Bad Request<br>401 – Unauthorized<br>404 – Not Found<br>409 – Conflict |
| DELETE | `/habits/{habitId}/entries/{entryId}` | Удалить запись выполнения | path: `habitId` (uuid), `entryId` (uuid) | 204 – No Content<br>401 – Unauthorized<br>404 – Not Found |
| POST | `/habits/{habitId}/insights/support` | Запрос поддержки ИИ для привычки | path: `habitId` (uuid)<br>body: `HabitSupportRequestDto` (scenario) | 200 – OK (`HabitSupportResponseDto`)<br>400 – Bad Request<br>401 – Unauthorized<br>404 – Not Found |
| GET | `/profile` | Получить профиль пользователя | – (требуется токен) | 200 – OK (`UserProfileDto`)<br>401 – Unauthorized<br>404 – Not Found |
| PUT | `/profile` | Обновить профиль пользователя | body: `UpdateUserProfileDto` (name, city, habitReminderEnabled, habitReminderTime, themePreference) | 200 – OK (`UserProfileDto`)<br>400 – Bad Request<br>401 – Unauthorized<br>404 – Not Found |
| GET | `/stats/daily-summary` | Получить дневную сводку | query: `date` (date, optional) | 200 – OK (`DailySummaryDto`)<br>400 – Bad Request<br>401 – Unauthorized<br>404 – Not Found |
| GET | `/stats/city-summary` | Получить сводку по городу | query: `city` (string) | 200 – OK (`CitySummaryDto`)<br>400 – Bad Request |
| GET | `/weather` | Получить погоду | query: `city` (string), `date` (date, optional) | 200 – OK (`WeatherSnapshotDto`)<br>400 – Bad Request<br>401 – Unauthorized<br>404 – Not Found<br>429 – Too Many Requests |

> **Примечание:** Все эндпоинты, кроме `/auth/register`, `/auth/login`, `/auth/confirm-email`, `/stats/city-summary`, требуют авторизации (JWT токен в заголовке `Authorization: Bearer <token>`).  
> Полная документация доступна в Swagger: `http://localhost:5093/swagger`
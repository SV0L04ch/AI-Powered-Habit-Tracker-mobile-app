# Отчёт по регрессионному тестированию API

**Тестовые данные:**  
baseUrl: `http://localhost:5093/api`  
Пользователь: `tester@example.com` / `password1`  
Переменные окружения: `accessToken`, `positiveHabitId`, `negativeHabitId`

## Сводка
- **Всего протестировано эндпоинтов:** 18
- **Успешно:** 18
- **Неудачно:** 0  
- **Статус:** все эндпоинты работают корректно, багов не обнаружено.

## Детализация по группам

### Auth (5 эндпоинтов)
| ID | Запрос | Ожидаемый результат | Фактический результат | Статус |
|----|--------|---------------------|----------------------|--------|
| AUTH-01 | POST /auth/register | 201 Created | 201 Created | Passed |
| AUTH-02 | GET /auth/confirm-email | 200 OK | 200 OK | Passed |
| AUTH-03 | POST /auth/login | 200 OK, accessToken | 200 OK | Passed |
| AUTH-04 | POST /auth/logout | 204 No Content | 204 No Content | Passed |
| AUTH-05 | POST /auth/login (неверный пароль) | 401 Unauthorized | 401 Unauthorized | Passed |

### Habits (5 эндпоинтов)
| ID | Запрос | Ожидаемый результат | Фактический результат | Статус |
|----|--------|---------------------|----------------------|--------|
| HAB-01 | GET /habits | 200 OK, массив | 200 OK | Passed |
| HAB-02 | POST /habits | 201 Created | 201 Created | Passed |
| HAB-03 | GET /habits/{id} | 200 OK | 200 OK | Passed |
| HAB-04 | PUT /habits/{id} | 200 OK | 200 OK | Passed |
| HAB-05 | DELETE /habits/{id} | 204 No Content | 204 No Content | Passed |

### HabitEntries (3 эндпоинта)
| ID | Запрос | Ожидаемый результат | Фактический результат | Статус |
|----|--------|---------------------|----------------------|--------|
| ENT-01 | POST /habits/{id}/entries | 201 Created | 201 Created | Passed |
| ENT-02 | PUT /habits/{id}/entries/{entryId} | 200 OK | 200 OK | Passed |
| ENT-03 | DELETE /habits/{id}/entries/{entryId} | 204 No Content | 204 No Content | Passed |

### Profile (1 эндпоинт)
| ID | Запрос | Ожидаемый результат | Фактический результат | Статус |
|----|--------|---------------------|----------------------|--------|
| PRO-01 | GET /profile | 200 OK | 200 OK | Passed |

### Stats (2 эндпоинта)
| ID | Запрос | Ожидаемый результат | Фактический результат | Статус |
|----|--------|---------------------|----------------------|--------|
| STA-01 | GET /stats/city-summary?city=Moscow | 200 OK | 200 OK | Passed |
| STA-02 | GET /stats/daily-summary (без даты) | 200 OK (сегодняшняя статистика) | 200 OK | Passed |

### Weather (1 эндпоинт)
| ID | Запрос | Ожидаемый результат | Фактический результат | Статус |
|----|--------|---------------------|----------------------|--------|
| WTH-01 | GET /weather?city=Moscow | 200 OK, температура | 200 OK | Passed |

### Insights (1 эндпоинт)
| ID | Запрос | Ожидаемый результат | Фактический результат | Статус |
|----|--------|---------------------|----------------------|--------|
| INS-01 | POST /habits/{habitId}/insights/support | 200 OK, совет ИИ | 200 OK | Passed |

> **Итог:** API полностью работоспособно, все ранее зарегистрированные дефекты исправлены. Автотесты проходят успешно.
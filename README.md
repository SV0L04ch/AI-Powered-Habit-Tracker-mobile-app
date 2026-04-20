# 🌤️ AI-Powered Habit Tracker (Frontend)

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![React](https://img.shields.io/badge/React-18.2-61DAFB?logo=react)](https://reactjs.org/)
[![Vite](https://img.shields.io/badge/Vite-5.0-646CFF?logo=vite)](https://vitejs.dev/)
[![SCSS](https://img.shields.io/badge/SCSS-1.69-CC6699?logo=sass)](https://sass-lang.com/)
[![Zustand](https://img.shields.io/badge/Zustand-4.4-764ABC)](https://github.com/pmndrs/zustand)
[![Vitest](https://img.shields.io/badge/Vitest-1.0-6E9F18?logo=vitest)](https://vitest.dev/)

Клиентская часть мобильного трекера привычек с поддержкой искусственного интеллекта. Пользователи могут управлять привычками, отмечать их выполнение, получать персонализированные ИИ-советы и анализировать связь привычек с погодой. Проект включает светлую/тёмную темы, анонимную городскую статистику и «ленивый коучинг».

## Возможности

- **Управление привычками**: добавление полезных и вредных привычек с указанием времени или количества раз в день.
- **Гибкая отметка выполнения**: поддержка бинарных (сделано/не сделано) и количественных привычек (сколько раз).
- **Ленивый коучинг**: при пропуске полезной привычки ИИ предлагает микро-шаг; при срыве вредной – мотивацию.
- **Персональная сводка с погодой**: автоматическая связь выполненных привычек с погодой (Open‑Meteo) через нейросеть.
- **Городская статистика**: анонимный рейтинг самых популярных привычек в вашем городе.
- **Аутентификация**: JWT‑токены с безопасным хранением (httpOnly или SecureStore).
- **Тёмная и светлая темы**: автоматически под системную или ручное переключение.
- **Адаптивный дизайн**: оптимизирован для мобильных устройств (iOS/Android) и веба.

## 🧰 Технологический стек

| Категория                 | Технологии                     |
| ------------------------- | ------------------------------ |
| **Фреймворк**             | React 18                       |
| **Сборка**                | Vite 5                         |
| **Маршрутизация**         | React Router 6                 |
| **HTTP-клиент**           | Axios                          |
| **Управление состоянием** | Zustand                        |
| **Стилизация**            | SCSS, CSS-модули               |
| **Тестирование**          | Vitest + React Testing Library |
| **Качество кода**         | ESLint + Prettier              |
| **Навигация**             | React Router (в веб‑версии)    |
| **Иконки**                | React Icons / Feather Icons    |

## 🚀 Быстрый старт

Эти инструкции помогут вам запустить локальную копию проекта для разработки и тестирования.

### Предварительные требования

- [Node.js](https://nodejs.org/) 18+
- [npm](https://www.npmjs.com/) или [pnpm](https://pnpm.io/)
- [Git](https://git-scm.com/)

## 📦 Установка и запуск

1. **Клонируйте репозиторий и перейдите в папку фронтенда:**

   ```bash
   git clone https://github.com/SV0L04ch/AI-Powered-Habit-Tracker-mobile-app.git
   cd AI-Powered-Habit-Tracker-mobile-app/frontend
   ```

2. **Установите зависимости:**

   ```bash
   npm install
   # или
   pnpm install
   ```

3. **Запустите сервер для разработки:**

   ```bash
   npm run dev
   ```

   Приложение будет доступно по адресу `http://localhost:5173`.

4. **Сборка для продакшена:**
   ```bash
   npm run build
   ```
   Готовые файлы появятся в папке `dist`.

## 🔧 Переменные окружения

Создайте файл `.env` в папке `frontend` со следующим содержимым:

```ini
VITE_API_BASE_URL=http://localhost:5000/api
VITE_WEATHER_API_KEY=your_openweather_api_key  # если требуется
```

**Важно:** Не коммитьте файл `.env` в Git (добавьте его в `.gitignore`).

## 🗂️ Структура проекта

```
frontend/
├── public/                 # статические файлы (favicon, robots.txt)
├── src/
│   ├── assets/             # изображения, шрифты, иконки
│   ├── components/         # переиспользуемые UI-компоненты (Button, Input, Card)
│   │   └── common/         # общие компоненты (Header, Footer, Loader)
│   ├── pages/              # компоненты страниц (LoginPage, HabitsPage, etc.)
│   ├── services/           # API-запросы (axios instance, эндпоинты)
│   ├── store/              # Zustand store (тема, пользователь, привычки)
│   ├── styles/             # глобальные стили, SCSS-переменные, темы
│   ├── utils/              # вспомогательные функции (форматирование дат, валидация)
│   ├── App.jsx             # главный компонент с роутингом
│   ├── main.jsx            # точка входа
│   └── router.jsx          # маршруты (если вынесены)
├── .eslint.config.js       # конфигурация ESLint
├── index.html
├── package.json
├── vite.config.js
├── README.md
└── pnpm-lock.yaml
```

## 🧪 Тестирование

Запуск unit-тестов (Vitest + React Testing Library):

```bash
npm run test
```

Написание тестов для компонентов и утилит:

```javascript
// пример теста для утилиты
import { formatDate } from "../utils/date";
import { describe, it, expect } from "vitest";

describe("formatDate", () => {
  it("should format date correctly", () => {
    expect(formatDate("2025-04-20")).toBe("20 апреля 2025");
  });
});
```

## 🧹 Линтинг и форматирование

Проверка кода:

```bash
npm run lint
```

Автоматическое исправление форматирования:

```bash
npm run format
```

## 🔌 Интеграция с API

Все запросы к бэкенду вынесены в сервисы (папка `services`). Основные эндпоинты:

| Метод | Эндпоинт                        | Назначение                                |
| ----- | ------------------------------- | ----------------------------------------- |
| POST  | `/auth/login`                   | Вход пользователя                         |
| POST  | `/auth/register`                | Регистрация                               |
| GET   | `/habits`                       | Получить все привычки пользователя        |
| POST  | `/habits`                       | Создать привычку                          |
| POST  | `/habit-entries`                | Добавить отметку выполнения               |
| GET   | `/stats/daily?date=...`         | Получить ежедневную сводку (персональную) |
| GET   | `/stats/city?city=...`          | Получить городскую сводку                 |
| POST  | `/habits/{id}/insights/support` | Получить ИИ-совет (ленивый коуч)          |
| GET   | `/weather?city=...&date=...`    | Получить погоду                           |

Пример вызова из компонента:

```javascript
import { getHabits } from "../services/habitService";

useEffect(() => {
  getHabits().then((data) => setHabits(data));
}, []);
```

## 🎨 Темизация

Приложение поддерживает светлую и тёмную темы. Переключение происходит:

- Автоматически по системной теме ОС.
- Вручную через переключатель в профиле.

Темы реализованы через CSS-переменные и класс `dark` на корневом элементе.

## 📄 Лицензия

Проект распространяется под лицензией MIT. Подробности смотрите в файле [LICENSE](LICENSE).

## ✍️ Авторы

- **Ваше имя** (@Arcteryx, @Mungums) – фронтенд-разработка, дизайн, UI/UX
- **Команда бэкенда** (@SV0L04ch, @jakepz23) – API и архитектура

---

**🌐 [Репозиторий на GitHub](https://github.com/SV0L04ch/AI-Powered-Habit-Tracker-mobile-app)**

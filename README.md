```markdown
# 🌤️ AI-Powered Habit Tracker (Frontend)

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![React](https://img.shields.io/badge/React-18.2-61DAFB?logo=react)](https://reactjs.org/)
[![Vite](https://img.shields.io/badge/Vite-5.0-646CFF?logo=vite)](https://vitejs.dev/)
[![SCSS](https://img.shields.io/badge/SCSS-1.69-CC6699?logo=sass)](https://sass-lang.com/)
[![Zustand](https://img.shields.io/badge/Zustand-4.4-764ABC)](https://github.com/pmndrs/zustand)
[![Vitest](https://img.shields.io/badge/Vitest-1.0-6E9F18?logo=vitest)](https://vitest.dev/)

Клиентская часть мобильного трекера привычек с поддержкой искусственного интеллекта.  
В проекте используется собственный UI-кит на SCSS-модулях, обеспечивающий консистентность стилей, светлую/тёмную темы и переиспользуемость компонентов.

## 📋 Содержание

- [Ключевые возможности](#-ключевые-возможности)
- [Технологический стек](#-технологический-стек)
- [Быстрый старт](#-быстрый-старт)
- [Структура проекта](#-структура-проекта)
- [🎨 UI-кит и стилизация](#-ui-кит-и-стилизация)
  - [SCSS-переменные и миксины](#scss-переменные-и-миксины)
  - [Типографика](#типографика)
  - [Компонент Button](#button)
  - [Компонент Input](#input)
  - [Компонент Substrate](#substrate)
  - [Компонент BottomNav](#bottomnav)
  - [Иконки и изображения](#иконки-и-изображения)
- [📡 Интеграция с API](#-интеграция-с-api)
- [🧪 Тестирование](#-тестирование)
- [🧹 Линтинг и форматирование](#-линтинг-и-форматирование)
- [📄 Лицензия](#-лицензия)
- [Авторы](#-авторы)

## 🚀 Ключевые возможности

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
| **Платформа**             | Tauri (Мобильное приложение)   |
| **Фреймворк**             | React 18                       |
| **Сборка**                | Vite 5                         |
| **Маршрутизация**         | React Router 6                 |
| **HTTP-клиент**           | Axios                          |
| **Управление состоянием** | Zustand                        |
| **Стилизация**            | SCSS, CSS-модули               |
| **Тестирование**          | Vitest                         |
| **Качество кода**         | ESLint + Prettier              |
| **Иконки**                | Кастомные SVG-компоненты       |

## 🚀 Быстрый старт

### Предварительные требования

- [Node.js](https://nodejs.org/) 18+
- [pnpm](https://pnpm.io/) (рекомендуется) или npm
- [Git](https://git-scm.com/)

### Установка и запуск

1. **Клонируйте репозиторий и перейдите в папку фронтенда:**

   ```bash
   git clone https://github.com/SV0L04ch/AI-Powered-Habit-Tracker-mobile-app.git
   cd AI-Powered-Habit-Tracker-mobile-app/frontend
   ```

2. **Установите зависимости:**

   ```bash
   pnpm install
   ```

3. **Запустите сервер для разработки:**

   ```bash
   pnpm run dev
   ```

   Приложение будет доступно по адресу `http://localhost:5173`.

4. **Сборка для продакшена:**
   ```bash
   pnpm run build
   ```
   Готовые файлы в папке `dist`.

### Переменные окружения

Создайте файл `.env` в папке `frontend`:

```ini
VITE_API_BASE_URL=http://localhost:5000/api
VITE_WEATHER_API_KEY=your_openweather_api_key  # если требуется
```

Не коммитьте `.env` в Git (добавьте в `.gitignore`).

## 🗂️ Структура проекта

```
frontend/
├── public/                 # статические файлы (favicon, robots.txt)
├── src/
│   ├── assets/             # исходные изображения, шрифты
│   │   ├── images/
│   │   │   ├── illustrations/   # Растровые картинки, фотографии
│   │   │   └── icons/           # Векторные картинки
│   ├── components/         # UI-компоненты
│   ├── lib/                # словари ресурсов
│   ├── pages/              # компоненты страниц (LoginPage, HabitsPage …)
│   ├── services/           # API-запросы (axios instance, эндпоинты)
│   ├── store/              # Zustand store (тема, пользователь, привычки)
│   ├── styles/             # глобальные стили и SCSS-основа
│   │   ├── abstracts/
│   │   │   ├── _variables.scss   (цвета, шрифты, отступы, радиусы)
│   │   │   └── _mixins.scss      (типографика, кнопки и т.д.)
│   │   ├── _fonts.scss           (подключение шрифтов)
│   │   └── main.scss             (импорт всех стилей, подключается в index.jsx)
│   ├── App.jsx             # главный компонент с роутингом
│   ├── main.jsx            # точка входа
├── .eslint.config.js       # конфигурация ESLint
├── index.html
├── package.json
├── vite.config.js
├── README.md
└── pnpm-lock.yaml
```

## 🎨 UI-кит и стилизация

В проекте используется **собственный UI-кит** на основе SCSS-модулей. Все компоненты независимы, стили изолированы, а темы управляются через переменные.

### SCSS-переменные и миксины

Глобальные настройки находятся в `src/styles/abstracts/`.  
Основные переменные (файл `_variables.scss`):

```scss
// Цвета (светлая / тёмная тема)
$light-background: #CCE6D9;
$dark-background: #133348;
$dark-buttons: #A0522D;
$dark-placeholder: #769DB7;
…

// Типографика
$font-family: 'Inter', sans-serif;
$font-weight-regular: 400;
$font-weight-700: 700;
$h1-font-size: 48px;
$h2-font-size: 36px;
…

// Отступы
$spacing-1: 5px;
$spacing-2: 10px;
$spacing-3: 15px;
$spacing-4: 20px;

// Радиусы, тени, переходы
$base-radius: 8px;
$transition-base: 0.25s ease;
```

Миксины типографики (`_mixins.scss`) позволяют применять готовые стили текста:

```scss
@mixin headline1 {
  font-family: $font-family;
  font-weight: $font-weight-700;
  font-size: $h1-font-size;
  line-height: 1.2;
}
@mixin body1 { … }
@mixin buttons { … }  // сбросы + текст для всех кнопок
```

### Типографика

Для любого текста используйте соответствующий миксин или компонент-обёртку (если он будет создан). Пока в проекте тексты стилизуются через миксины внутри компонентов.

**Пример внутри SCSS-компонента:**
```scss
.card-title {
  @include mix.headline1;
}
```

### Компоненты

#### Button

**Назначение:** Основная кнопка UI-кита. Поддерживает варианты оформления и состояния.

**Импорт:**
```jsx
import Button from '../components/Button/Button';
```

**Пропсы:**

| Проп      | Тип                   | По умолчанию  | Описание                                                                 |
|-----------|-----------------------|---------------|--------------------------------------------------------------------------|
| children  | ReactNode (обяз.)     | —             | Содержимое кнопки (текст / иконка)                                       |
| variant   | `'primary'` \| `'form'` \| `'secondary'` | `'primary'` | Вариант стиля. `form` – широкая для форм, `secondary` – на всю ширину блока |
| disabled  | boolean               | `false`       | Делает кнопку неактивной                                                 |
| onClick   | function              | —             | Обработчик клика                                                         |
| className | string                | `''`          | Дополнительный внешний класс                                             |

**Примеры:**
```jsx
<Button>Нажми меня</Button>
<Button variant="form">Отправить</Button>
<Button variant="secondary">Блочная кнопка</Button>
<Button disabled>Недоступно</Button>
<Button onClick={() => console.log('клик')}>Кликабельная</Button>
```

**Состояния:**
- **`:active`** — фон меняется на `$dark-buttons`, лёгкое уменьшение.
- **`:disabled`** — прозрачность 0.4, курсор `not-allowed`.

---

#### Input

**Назначение:** Поле ввода с поддержкой иконки (слева). Контейнер является `<label>`, поэтому клик по иконке или отступу переводит фокус на поле.

**Импорт:**
```jsx
import Input from '../components/Input/Input';
```

**Пропсы:**

| Проп        | Тип                   | По умолчанию         | Описание                                                                 |
|-------------|-----------------------|----------------------|--------------------------------------------------------------------------|
| placeholder | string                | `"Введите текст"`    | Текст-подсказка                                                          |
| type        | string                | `"text"`             | Тип поля (text, password, email …)                                       |
| icon        | string (ключ)         | —                    | Имя иконки из библиотеки `icons.js` (например `"Search"`)                |
| disabled    | boolean               | `false`              | Делает поле неактивным                                                   |
| className   | string                | —                    | Дополнительный класс для контейнера                                      |
| …rest       | любые атрибуты input  | —                    | Пробрасываются на `<input>` (value, onChange, name, id и т.д.)           |

**Примеры:**
```jsx
<Input placeholder="Введите имя" />
<Input placeholder="Поиск" icon="Search" />
<Input type="password" placeholder="Пароль" />
<Input disabled placeholder="Недоступно" />

// Контролируемый компонент
const [email, setEmail] = useState('');
<Input value={email} onChange={(e) => setEmail(e.target.value)} />
```

---

#### Substrate

**Назначение:** Универсальная подложка-карточка. Может содержать иконку, изображение (оба опционально) и любое содержимое через `children`.

**Импорт:**
```jsx
import Substrate from '../components/Substrate/Substrate';
```

**Пропсы:**

| Проп      | Тип                   | По умолчанию   | Описание                                                                   |
|-----------|-----------------------|----------------|----------------------------------------------------------------------------|
| children  | ReactNode             | —              | Основной контент (заголовки, текст)                                        |
| variant   | `'main'` \| `'secondary'` \| `'form'` | `'main'` | Стиль карточки. `form` – на всю ширину с большими отступами               |
| image     | string (ключ)         | —              | Имя растрового изображения из `images.js` (например `"clouds"`)            |
| icon      | string (ключ)         | —              | Имя иконки из `icons.js` (например `"Moon"`)                               |
| alt       | string                | `"Картинка:)"` | Альтернативный текст для изображения                                       |
| className | string                | —              | Дополнительный класс для контейнера                                        |

**Примеры:**
```jsx
<Substrate variant="main" image="clouds" icon="Search">
  <h2>Погода</h2>
  <p>Облачно с прояснениями</p>
</Substrate>

<Substrate variant="secondary" icon="Moon">
  <span>Ночной режим</span>
</Substrate>

<Substrate variant="form">
  <p>Форма занимает 100% ширины</p>
</Substrate>
```

---

#### BottomNav

В разработке...

### Иконки и изображения

Иконки (SVG) и растровые изображения централизованы в `src/lib/icons.js` и `src/lib/images.js`.  
Там лежат объекты-словари:

**icons.js**
```js
import SearchIcon from '../assets/images/icons/SearchIcon';
import MoonIcon from '../assets/images/icons/MoonIcon';

const icons = {
  Search: SearchIcon,
  Moon: MoonIcon,
  // … другие
};
export default icons;
```

**images.js**
```js
import Clouds from '../assets/images/illustrations/Clouds.png';
import Rain from '../assets/images/illustrations/Rain.png';

const images = {
  clouds: Clouds,
  rain: Rain,
};
export default images;
```

**Как добавить новую иконку:**
1. Поместите SVG-файл в `assets/images/icons/`.
2. Импортируйте его как компонент в `lib/icons.js` и добавьте в объект `icons`.
3. В компоненте используйте ключ (например `"Search"`). Всё, остальное сделает библиотека.

**Как добавить новое изображение:**
1. Положите PNG/JPG в `assets/images/illustrations/`.
2. Импортируйте в `lib/images.js` и добавьте в объект `images`.
3. Передайте ключ в проп `image` компонента `Substrate` или используйте `<img src={images.key} />`.

**Почему так:**
- Строки-пути не работают динамически в React (сборщик требует статический импорт).
- Словари решают проблему и делают код чистым.

## 📡 Интеграция с API

Ниже перечислены основные эндпоинты. Подробности в `src/services/`.

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

Пример вызова:
```js
import { getHabits } from "../services/habitService";

useEffect(() => {
  getHabits().then((data) => setHabits(data));
}, []);
```

## 🧪 Тестирование

Запуск unit-тестов (Vitest):

```bash
pnpm run test
```

Пример теста:
```js
import { describe, it, expect } from "vitest";
import { formatDate } from "../utils/date";

describe("formatDate", () => {
  it("should format date correctly", () => {
    expect(formatDate("2025-04-20")).toBe("20 апреля 2025");
  });
});
```

## 🧹 Линтинг и форматирование

- **Проверка:** `pnpm run lint`
- **Автоформатирование:** `pnpm run format`

## 📄 Лицензия

MIT – см. [LICENSE](LICENSE).

## Авторы

- **Фронтенд:** @Arcteryx, @Mungums
- **Бэкенд:** @SV0L04ch, @jakepz23

---

**Репозиторий:** [https://github.com/SV0L04ch/AI-Powered-Habit-Tracker-mobile-app](https://github.com/SV0L04ch/AI-Powered-Habit-Tracker-mobile-app)
```
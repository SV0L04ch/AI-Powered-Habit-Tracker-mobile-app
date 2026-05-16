// src/utils/errorTranslations.js

// Справочник английских сообщений и их русских переводов
const errorTranslations = {
  'Email is required.': 'Требуется email.',
  'Password is required.': 'Требуется пароль.',
  'Invalid email or password.': 'Неверный email или пароль.',
  'User already exists.': 'Пользователь уже существует.',
  'Habit name is required.': 'Требуется название привычки.',
  'TriggerValue is required.': 'Требуется значение (время/количество).',
  'TriggerValue format is invalid for selected TriggerType.': 'Неверный формат значения. Для времени используйте ЧЧ:ММ (08:00), для счётчика — целое число.',
  'The Email field is required.': 'Поле Email обязательно.',
  'The Password field is required.': 'Поле Пароль обязательно.',
  'The Name field is required.': 'Поле Название обязательно.',
  'Password must contain at least one letter.': 'Пароль должен содержать символ.',
  'Password must contain at least one digit.': 'Пароль должен содержать число.',
  'Conflict': 'Пользователь уже существует.'
};

export default errorTranslations;
// src/utils/handleServerError.js

export const getErrorMessage = (error) => {
  // 1. Ошибка валидации (400)
  if (error.response?.data?.errors) {
    const messages = Object.values(error.response.data.errors)
      .flat()
      .join('. ');
    return messages || 'Проверьте правильность заполнения полей.';
  }

  // 2. Другие поля с описанием ошибки
  if (error.response?.data?.title) return error.response.data.title;
  if (error.response?.data?.detail) return error.response.data.detail;

  // 3. Популярные статусы
  if (error.response?.status === 401) return 'Неверный email или пароль.';
  if (error.response?.status === 403) return 'Доступ запрещён.';
  if (error.response?.status === 404) return 'Ресурс не найден.';
  if (error.response?.status === 500) return 'Внутренняя ошибка сервера. Попробуйте позже.';

  // 4. Проблемы с сетью
  if (error.code === 'ERR_NETWORK' || error.message === 'Network Error') {
    return 'Сервер недоступен. Проверьте подключение к интернету.';
  }

  // 5. Стандартное сообщение axios
  if (error.message) return error.message;

  return 'Произошла неизвестная ошибка.';
};
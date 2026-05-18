// src/utils/handleServerError.js
import errorTranslations from './errorTranslation'

const translateError = (message) => {
  if (!message) return '';
  const trimmed = message.trim();
  return errorTranslations[trimmed] || trimmed;
};

export const getErrorMessage = (error) => {
  // 1. Ошибка валидации (400)
  if (error.response?.data?.errors) {
    const errorObj = error.response.data.errors;
    let messages = [];
    
    if (typeof errorObj === 'object') {
      for (const key in errorObj) {
        const fieldErrors = errorObj[key];
        if (Array.isArray(fieldErrors)) {
          messages = messages.concat(fieldErrors);
        }
      }
    } else if (Array.isArray(errorObj)) {
      messages = errorObj;
    }
    
    const translated = messages.map(msg => translateError(msg)).join('. ');
    return translated || 'Проверьте правильность заполнения полей.';
  }

  // 2. Поле title
  if (error.response?.data?.title) {
    return translateError(error.response.data.title);
  }

  // 3. Поле detail
  if (error.response?.data?.detail) {
    return translateError(error.response.data.detail);
  }

  // 4. Частые HTTP-статусы
  if (error.response?.status === 401) return 'Неверный email или пароль.';
  if (error.response?.status === 403) return 'Доступ запрещён.';
  if (error.response?.status === 404) return 'Ресурс не найден.';
  if (error.response?.status === 500) return 'Внутренняя ошибка сервера. Попробуйте позже.';
  if ([502, 503, 504].includes(error.response?.status)) {return 'Сервер временно недоступен. Попробуйте позже.';}
  


  // 5. Проблемы с сетью
  if (error.code === 'ERR_NETWORK' || error.message === 'Network Error') {
    return 'Сервер недоступен. Проверьте подключение к интернету.';
  }

  // 6. Стандартное сообщение axios
  if (error.message) {
    return translateError(error.message);
  }

  return 'Произошла неизвестная ошибка.';
};
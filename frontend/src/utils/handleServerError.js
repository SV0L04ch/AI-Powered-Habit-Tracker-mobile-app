import errorTranslations from './errorTranslation';

const translateError = (message) => {
  if (!message) return '';
  const trimmed = String(message).trim();
  const exact = errorTranslations[trimmed];
  if (exact) return exact;

  const partial = Object.entries(errorTranslations).find(([key]) => trimmed.includes(key));
  return partial ? partial[1] : trimmed;
};

export const getErrorMessage = (error) => {
  const data = error.response?.data;

  if (data?.errors) {
    const messages = Object.values(data.errors).flat().filter(Boolean);
    const translated = messages.map((message) => translateError(message)).join(' ');
    if (translated) return translated;
  }

  if (data?.error) return translateError(data.error);
  if (data?.detail) return translateError(data.detail);
  if (data?.title) return translateError(data.title);

  if (error.response?.status === 401) {
    return 'Сессия истекла или email не подтвержден. Войдите снова.';
  }
  if (error.response?.status === 403) return 'Недостаточно прав для этого действия.';
  if (error.response?.status === 404) return 'Данные не найдены.';
  if (error.response?.status === 409) return 'Такая запись уже существует.';
  if (error.response?.status === 429) return 'Слишком много запросов. Попробуйте чуть позже.';
  if (error.response?.status >= 500) {
    return 'Сервер временно недоступен. Попробуйте позже.';
  }

  if (error.code === 'ERR_NETWORK' || error.message === 'Network Error') {
    return 'Не удалось подключиться к HabitApi. Проверьте, что бэкенд запущен.';
  }

  if (error.message) return translateError(error.message);
  return 'Произошла неизвестная ошибка.';
};

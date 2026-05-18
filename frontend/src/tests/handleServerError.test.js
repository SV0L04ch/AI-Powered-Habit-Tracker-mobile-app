import { describe, it, expect } from 'vitest';
import { getErrorMessage } from '../utils/handleServerError';

describe('getErrorMessage', () => {
  it('возвращает понятное сообщение для статуса 401', () => {
    const error = { response: { status: 401 } };
    expect(getErrorMessage(error)).toBe('Неверный email или пароль.');
  });

  it('возвращает сообщение об ошибке сети', () => {
    const error = { code: 'ERR_NETWORK', message: 'Network Error' };
    expect(getErrorMessage(error)).toBe('Сервер недоступен. Проверьте подключение к интернету.');
  });

  it('возвращает переведённое сообщение валидации', () => {
    const error = {
      response: {
        data: {
          errors: { Email: ['The Email field is required.'] }
        }
      }
    };
    expect(getErrorMessage(error)).toContain('Поле Email обязательно');
  });

  it('возвращает стандартное сообщение, если ничего не подошло', () => {
    const error = new Error('Что-то пошло не так');
    expect(getErrorMessage(error)).toBe('Что-то пошло не так');
  });
});
import { describe, it, expect, beforeEach } from 'vitest';
import useAuthStore from '../store/useAuthStore';
import { vi } from 'vitest';

// Мокаем модуль AuthService, чтобы не делать реальные запросы
vi.mock('../services/AuthService', () => ({
  loginUser: vi.fn().mockResolvedValue({
    email: 'test@mail.com',
    city: 'Москва',
    token: 'fake-token',
  }),
  registerUser: vi.fn(),
}));

// Сбрасываем состояние перед каждым тестом
beforeEach(() => {
  useAuthStore.setState({
    email: null,
    isAuthenticated: false,
    isLoading: false,
    error: null,
    city: null,
  });
});

describe('useAuthStore', () => {
  it('устанавливает isAuthenticated в true при успешном входе', async () => {
    // Вызываем login (заглушка всё ещё работает в сторе)
    await useAuthStore.getState().login('test@mail.com', 'password123');
    const state = useAuthStore.getState();
    expect(state.isAuthenticated).toBe(true);
    expect(state.email).toBe('test@mail.com');
  });

  it('сбрасывает состояние при выходе', async () => {
    // Сначала "войдём"
    await useAuthStore.getState().login('test@mail.com', 'password123');
    // Теперь выйдем
    await useAuthStore.getState().logout();
    const state = useAuthStore.getState();
    expect(state.isAuthenticated).toBe(false);
    expect(state.email).toBeNull();
  });
});
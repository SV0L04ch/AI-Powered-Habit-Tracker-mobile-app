import { describe, it, expect, beforeEach } from 'vitest';
import useHabits from '../store/useHabitsStore';
import { vi } from 'vitest';

vi.mock('../services/habitService', () => ({
  createHabit: vi.fn().mockResolvedValue({
    id: 1,
    name: 'Тестовая',
    type: true,
    category: false,
  }),
  fetchHabits: vi.fn(),
  updHabit: vi.fn(),
  deleteHabit: vi.fn(),
}));

beforeEach(() => {
  useHabits.setState({ habits: [], isLoading: false, error: null });
});

describe('useHabitsStore', () => {
  it('добавляет новую привычку', async () => {
    const newHabit = { name: 'Тестовая', type: true, category: false };
    await useHabits.getState().addHabit(newHabit);
    const habits = useHabits.getState().habits;
    expect(habits).toHaveLength(1);
    expect(habits[0].name).toBe('Тестовая');
  });
});
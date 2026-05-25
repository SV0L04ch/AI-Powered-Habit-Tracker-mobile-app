import { create } from 'zustand';
import {
  createHabit,
  createHabitEntry,
  delHabit,
  fetchHabitEntries,
  fetchHabits,
  updateHabitEntry,
  updHabit,
} from '../services/habitService';
import { getErrorMessage } from '../utils/handleServerError';

const todayIso = () => new Date().toISOString().slice(0, 10);
const COMPLETED = 1;

const indexTodayEntries = async (habits) => {
  const today = todayIso();
  const pairs = await Promise.all(
    habits.map(async (habit) => {
      const entries = await fetchHabitEntries(habit.id, {
        fromDate: today,
        toDate: today,
      });
      return [habit.id, entries?.[0] || null];
    }),
  );
  return Object.fromEntries(pairs);
};

const useHabits = create((set, get) => ({
  habits: [],
  entriesByHabitId: {},
  isLoading: false,
  actionLoadingId: null,
  error: null,
  isLoaded: false,

  addHabit: async (habitData) => {
    set({ isLoading: true, error: null });
    try {
      const newHabit = await createHabit(habitData);
      set((state) => ({
        habits: [newHabit, ...state.habits],
        entriesByHabitId: { ...state.entriesByHabitId, [newHabit.id]: null },
        isLoading: false,
        isLoaded: true,
      }));
      return newHabit;
    } catch (err) {
      set({ error: getErrorMessage(err), isLoading: false });
      return null;
    }
  },

  clearError: () => set({ error: null }),

  updateHabit: async (id, updates) => {
    set({ actionLoadingId: id, error: null });
    try {
      const updated = await updHabit(id, updates);
      set((state) => ({
        habits: state.habits.map((habit) => (habit.id === id ? updated : habit)),
        actionLoadingId: null,
      }));
      return updated;
    } catch (err) {
      set({ error: getErrorMessage(err), actionLoadingId: null });
      return null;
    }
  },

  clearHabits: () =>
    set({
      habits: [],
      entriesByHabitId: {},
      isLoaded: false,
      error: null,
      actionLoadingId: null,
    }),

  deleteHabit: async (id) => {
    set({ actionLoadingId: id, error: null });
    try {
      await delHabit(id);
      set((state) => {
        const nextEntries = { ...state.entriesByHabitId };
        delete nextEntries[id];
        return {
          habits: state.habits.filter((habit) => habit.id !== id),
          entriesByHabitId: nextEntries,
          actionLoadingId: null,
        };
      });
      return true;
    } catch (err) {
      set({ error: getErrorMessage(err), actionLoadingId: null });
      return false;
    }
  },

  getHabits: async ({ force = false } = {}) => {
    if (get().isLoaded && !force) return get().habits;
    set({ isLoading: true, error: null });
    try {
      const habits = await fetchHabits();
      const entriesByHabitId = await indexTodayEntries(habits);
      set({ habits, entriesByHabitId, isLoading: false, isLoaded: true });
      return habits;
    } catch (err) {
      set({ error: getErrorMessage(err), isLoading: false });
      return [];
    }
  },

  refreshTodayEntries: async () => {
    const habits = get().habits;
    if (!habits.length) return;
    try {
      const entriesByHabitId = await indexTodayEntries(habits);
      set({ entriesByHabitId });
    } catch (err) {
      set({ error: getErrorMessage(err) });
    }
  },

  markHabitCompleted: async (habit) => {
    const currentEntry = get().entriesByHabitId[habit.id];
    const isCompleted = currentEntry?.status === COMPLETED;
    const today = todayIso();
    set({ actionLoadingId: habit.id, error: null });

    try {
      const payload = habit.isPositive
        ? {
            date: today,
            status: isCompleted ? 3 : COMPLETED,
            partialValue: null,
            note: null,
          }
        : {
            date: today,
            relapseCount: 1,
            note: null,
          };

      const entry = currentEntry
        ? await updateHabitEntry(habit.id, currentEntry.id, payload)
        : await createHabitEntry(habit.id, payload);

      set((state) => ({
        entriesByHabitId: {
          ...state.entriesByHabitId,
          [habit.id]: entry,
        },
        actionLoadingId: null,
      }));
      return entry;
    } catch (err) {
      set({ error: getErrorMessage(err), actionLoadingId: null });
      return null;
    }
  },
}));

export default useHabits;

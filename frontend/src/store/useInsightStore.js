import { create } from 'zustand';
import { getHabitInsight } from '../services/insightService.js';
import { getErrorMessage } from '../utils/handleServerError.js';

const useInsight = create((set) => ({
  message: null,
  scenario: null,
  error: null,
  isLoading: false,

  fetchSupport: async (habitId, scenario = 'daily') => {
    set({ isLoading: true, error: null, message: null, scenario });
    try {
      const data = await getHabitInsight(habitId, scenario);
      set({ message: data.message, isLoading: false });
      return data;
    } catch (err) {
      set({ error: getErrorMessage(err), isLoading: false });
      return null;
    }
  },

  clearInsight: () => set({ message: null, scenario: null, error: null, isLoading: false }),
}));

export default useInsight;

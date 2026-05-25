import { create } from 'zustand';
import { getCitySummary, getDailySummary } from '../services/dailySummaryService.js';
import { getErrorMessage } from '../utils/handleServerError.js';

const useDailySummaryStore = create((set, get) => ({
  summary: null,
  citySummary: null,
  error: null,
  cityError: null,
  isLoading: false,
  cityLoading: false,

  fetchStats: async (date) => {
    set({ isLoading: true, error: null });
    try {
      const data = await getDailySummary(date);
      set({ summary: data, isLoading: false });
      return data;
    } catch (err) {
      set({ error: getErrorMessage(err), isLoading: false });
      return null;
    }
  },

  fetchCitySummary: async (city) => {
    const targetCity = city?.trim();
    if (!targetCity) {
      set({ citySummary: null, cityError: 'Укажите город для городской сводки.' });
      return null;
    }

    set({ cityLoading: true, cityError: null });
    try {
      const data = await getCitySummary(targetCity);
      set({ citySummary: data, cityLoading: false });
      return data;
    } catch (err) {
      set({ cityError: getErrorMessage(err), cityLoading: false });
      return null;
    }
  },

  clearSummary: () =>
    set({
      summary: null,
      citySummary: null,
      error: null,
      cityError: null,
      isLoading: false,
      cityLoading: false,
    }),
}));

export default useDailySummaryStore;

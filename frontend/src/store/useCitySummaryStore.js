import { create } from 'zustand';
import { getCitySummary } from '../services/citySummaryService';
import { getErrorMessage } from '../utils/handleServerError';

const useCitySummaryStore = create((set) => ({
  data: null,
  isLoading: false,
  error: null,

  fetchCitySummary: async (city) => {
    set({ isLoading: true, error: null });
    try {
      const data = await getCitySummary(city);
      set({ data, isLoading: false });
    } catch (err) {
      set({ error: getErrorMessage(err), isLoading: false });
    }
  },

  clearData: () => set({ data: null, error: null }),
}));

export default useCitySummaryStore;
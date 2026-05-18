import { create } from "zustand";
import { getDailySummary } from "../services/dailySummaryService.js";
import { getErrorMessage } from "../utils/handleServerError.js";

// const mockGetDailySummary = async (date) => {
//   await new Promise((resolve) => setTimeout(resolve, 800));
//   return {
//     date: date || new Date().toISOString().slice(0, 10),
//     habitsCompleted: 3,
//     habitsPartiallyCompleted: 1,
//     habitsSkipped: 1,
//     weather: {
//       city: 'Москва',
//       date: date || new Date().toISOString().slice(0, 10),
//       condition: 'Солнечно',
//       temperatureCelsius: 23,
//       humidityPercent: 65,
//       precipitation: 'none',
//     },
//     aiInsight:
//       'Отличный день! Солнечная погода сопоставляется с вашим пиком активности в 9',
//   };
// };

const useDailySummaryStore = create((set,get) => ({
    summary: null,
    error: null,
    isLoading: false,
    historyInsights: [],

    fetchStats: async (date) => {
        const current = get().summary;
        if (!current) {
            set({ isLoading: true, error: null });
        }
        try {
            const data = await getDailySummary(date);
            set({ summary: data, isLoading: false });
        } catch (err) {
            set({ error: getErrorMessage(err), isLoading: false });
        }
    },

    clearSummary: () => set({summary: null, error: null})
}))

export default useDailySummaryStore
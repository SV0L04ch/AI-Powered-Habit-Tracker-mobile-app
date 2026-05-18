import { create } from "zustand";
import { getHabitInsight } from "../services/insightService.js"
import { getErrorMessage } from "../utils/handleServerError.js";

// const mockGetHabitInsight = async (habitId, habitName) => {
//   await new Promise((resolve) => setTimeout(resolve, 1500)); // имитация задержки
//   const displayName = habitName
//   return {
//     message: `Совет для привычки "${displayName}": Сегодня отличный день, чтобы начать!`,
//   };
// };

const useInsight = create((set) => ({
  message: null,        // теперь массив
  currentIndex: 0,     // индекс текущего отображаемого сообщения
  error: null,
  isLoading: false,

  fetchSupport: async (habitId, habitName) => {
    set({ isLoading: true, error: null });
    try {
      const data = await getHabitInsight(habitId, habitName);
      set((state) => ({
        message: data.message, // добавляем в конец
        isLoading: false,
      }));
    } catch (err) {
      set({ error: getErrorMessage(err), isLoading: false });
    }
  },

  clearInsight: () => set({ message: null, currentIndex: 0, error: null }),
}));

export default useInsight
import { create } from "zustand";
import { persist } from "zustand/middleware";

// const createHabit = async (habitData) => {
//     await new Promise((resolve) => setTimeout(resolve, 1000))
//     return { id: Date.now(), ...habitData, is_active: true }
// }

// const fetchHabits = async () => {
//   await new Promise((resolve) => setTimeout(resolve, 800));
//   // Возвращаем фиктивный список (например, две стартовые привычки)
//   return [
//     { id: 1, name: 'Медитация', type: true, category: true, trigger_type: 1, trigger_value: '08:00', target_days: 12, is_active: true},
//     { id: 2, name: 'Не есть сладкое', type: false, category: false, trigger_type: 2, target_days: 30, trigger_value: '3', is_active: false},
//   ];
// };

// const updHabits = async (id, updates) => {
//     await new Promise((resolve) => setTimeout(resolve, 600))
//     return {id, ...updates}
// }

// const delHabit = async (id) => {
//     await new Promise((resolve) => setTimeout(resolve, 500))
//     return { success: true, id}
// }


const useHabits = create(
    persist((set) => ({
                habits: [],
                isLoading: false,
                error: false,
                isLoaded: false,

                addHabit: async (habitData) => {
                    set({ isLoading: true, error: null })
                    try {

                        const newHabit = await createHabit(habitData)
                        set((state) => ({
                            habits: [...state.habits, newHabit],
                            isLoading: false
                        }))
                    } catch(err) {
                        set({error: err.message, isLoading: false})
                    }
                },

                updateHabit: async (id, updates) => {
                    set({isLoading: true, error: null})
                    try{
                        const upd = await updHabits(id, updates)
                        set((state) =>({
                            habits: state.habits.map((h) => (h.id === id ? {... h, ...updates} : h)),
                            isLoading: false
                        }))
                    } catch(err) {
                        set({error: err.message, isLoading: false})
                    }
                },

                deleteHabit: async (id) => {
                    set({isLoading: true, error: null})
                    try {
                        await delHabit(id);
                        set((state) => ({
                        habits: state.habits.filter((h) => h.id !== id),
                        isLoading: false
                        }))
                    } catch (err) {
                        set({error: err.message, isLoading: false})
                    }
                },

                getHabits: async () => {
                    set({ isLoading: true, error: null})
                    try {
                        const data = await fetchHabits()
                        set({habits: data, isLoading: false})
                    } catch (err) {
                        set({error: err.message, isLoading: false})
                    }
                }
            })))

export default useHabits
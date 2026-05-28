import { create } from "zustand";
import { persist } from "zustand/middleware";
import { createHabit, fetchHabits, updHabit, delHabit } from "../services/habitService";
import { getErrorMessage } from "../utils/handleServerError";


const useHabits = create(
    persist((set, get) => ({
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
                        set({error: getErrorMessage(err), isLoading: false})
                    }
                },

                clearError: () => set({ error: null }),

                updateHabit: async (id, updates) => {
                    set({isLoading: true, error: null})
                    try{
                        const upd = await updHabit(id, updates)
                        set((state) =>({
                            habits: state.habits.map((h) => (h.id === id ? {... h, ...updates} : h)),
                            isLoading: false
                        }))
                    } catch(err) {
                        set({error: getErrorMessage(err), isLoading: false})
                    }
                },

                clearHabits: () => set({
                    habits: [],
                    isLoaded: false,
                    error: null
                }),

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
                  if (get().isLoaded) return
                set({ isLoading: true, error: null})
                try {
                    const data = await fetchHabits()
                    set({habits: data, isLoading: false, isLoaded: true})
                } catch (err) {
                    set({error: getErrorMessage(err), isLoading: false})
                }
                }
            }),
            { 
                name: 'habits-storage',
                partialize: (state) => ({
                    habits: state.habits,
                    isLoaded: state.isLoaded
                })
            }
        ))

export default useHabits




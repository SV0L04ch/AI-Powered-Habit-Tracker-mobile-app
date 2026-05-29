import { create } from "zustand";
import { persist } from "zustand/middleware";
import { loginUser, registerUser } from "../services/AuthService";
import { getErrorMessage } from "../utils/handleServerError";
import axios from "axios";
import useHabits from "./useHabitsStore";
import useInsight from "./useInsightStore"

const useAuthUser = create(
    persist((set, get) => ({
        email: null,
        city: null,
        reportTime: null,  // Добавляем поле для времени отчёта
        isAuthenticated: false,
        isLoading: false,
        error: null,

        register: async (email, city, password) => {
            set({ isLoading: true, error: null });
            try {
                const data = await registerUser(email, city, password);
                set({
                    email: data.email,
                    city: data.city,
                    isLoading: false,
                });
            } catch (err) {
                set({ error: getErrorMessage(err), isLoading: false });
            }
        },
        
        clearError: () => set({ error: null }),

        login: async (email, password) => {
            set({ isLoading: true, error: null });
            try {
                const data = await loginUser(email, password);
                set({
                    email: data.email,
                    isAuthenticated: true,
                    isLoading: false,
                    city: data.city,
                });
            } catch (err) {
                set({ error: getErrorMessage(err), isLoading: false });
            }
        },

        // ✅ НОВЫЙ МЕТОД: Получение профиля
        getProfile: async () => {
            set({ isLoading: true, error: null });
            try {
                const response = await axios.get('/api/profile');
                set({ 
                    email: response.data.email,
                    city: response.data.city,
                    reportTime: response.data.reportTime,
                    isLoading: false 
                });
                return response.data;
            } catch (err) {
                set({ error: getErrorMessage(err), isLoading: false });
                throw err;
            }
        },

        // ✅ НОВЫЙ МЕТОД: Обновление профиля
        updateProfile: async (profileData) => {
            set({ isLoading: true, error: null });
            try {
                const response = await axios.put('/api/profile', profileData);
                // Обновляем локальные данные
                if (profileData.city !== undefined) {
                    set({ city: profileData.city });
                }
                if (profileData.reportTime !== undefined) {
                    set({ reportTime: profileData.reportTime });
                }
                set({ isLoading: false });
                return response.data;
            } catch (err) {
                set({ error: getErrorMessage(err), isLoading: false });
                throw err;
            }
        },

        logout: async () => {
            try { await axios.post('/api/auth/logout'); } catch(e){ }
            useHabits.getState().clearHabits();
            useInsight.getState().clearInsight()
            set({ email: null, city: null, reportTime: null, isAuthenticated: false });
        },
    }),
    {
        name: 'auth-storage',
        partialize: (state) => ({
            email: state.email,
            city: state.city,
            reportTime: state.reportTime,
            isAuthenticated: state.isAuthenticated
        })
    })
);

export default useAuthUser;
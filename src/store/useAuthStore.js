import { create } from "zustand";
import { persist } from "zustand/middleware";
import { loginUser, registerUser } from "../services/AuthService";
import { getErrorMessage } from "../utils/handleServerError";
import axios from "axios";
import useHabits from "./useHabitsStore";

const useAuthUser = create(
    persist((set) => ({
        email: null,
        city: null,
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
                    isAuthenticated: true,
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

        logout: async () => {
            try { await axios.post('/api/auth/logout'); } catch(e){ }
            useHabits.getState().clearHabits();
            set({ email: null, city: null, isAuthenticated: false });
        },
    }),
    {
        name: 'auth-storage',
        partialize: (state) => ({
            email: state.email,
            city: state.city,
            isAuthenticated: state.isAuthenticated
        })
    })
);
export default useAuthUser;

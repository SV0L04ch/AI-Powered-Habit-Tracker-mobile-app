import { create } from "zustand";
import { persist } from "zustand/middleware";
import { loginUser, registerUser } from "../services/AuthService";


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
                set({ error: err.response?.data?.message || err.message, isLoading: false });
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
                set({ error: err.message, isLoading: false });
            }
        },

        logout: async () => {
            try { await axios.post('/api/auth/logout'); } catch(e){}
            set({ email: null, city: null, isAuthenticated: false });
        },
    })),
);
export default useAuthUser;

import { create } from "zustand";
import { persist } from "zustand/middleware";

const registerUser = async (email, city, password) => {
  await new Promise((resolve) => setTimeout(resolve, 1000));
  return {
    token: "fake-jwt-token-123",
    email: email,
    city: city,
  };
};

const loginUser = async (email, password) => {
  await new Promise((resolve) => setTimeout(resolve, 1000));
  return {
    token: "fake-jwt-token-12",
    email: email,
  };
};

const useAuthUser = create(
  persist((set) => ({
    email: null,
    city: null,
    token: localStorage.getItem("token") || null,
    isLoading: false,
    error: null,

    register: async (email, city, password) => {
      set({ isLoading: true, error: null });
      try {
        const data = await registerUser(email, city, password);
        set({
          email: data.email,
          city: data.city,
          token: data.token,
          isLoading: false,
        });
      } catch (err) {
        set({ error: err.message, isLoading: false });
      }
    },

    login: async (email, password) => {
      set({ isLoading: true, error: null });
      try {
        const data = await loginUser(email, password);
        set({
          email: data.email,
          token: data.token,
          isLoading: false,
          city: data.city,
        });
      } catch (err) {
        set({ error: err.message, isLoading: false });
      }
    },

    logout: () => {
      set({ email: null, token: null, city: null });
    },
  })),
);
export default useAuthUser;

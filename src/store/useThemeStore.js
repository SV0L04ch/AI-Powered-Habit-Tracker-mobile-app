import { create } from "zustand";

const useThemeStore = create((set) => ({
  theme: 'dark',                     // данные (тема)
  toggleTheme: () => set((state) => ({   // действие (переключить тему)
    theme: state.theme === 'dark' ? 'light' : 'dark'
  })),
}));

export default useThemeStore;

import { create } from "zustand"
import { getProfile, updateProfile } from '../services/profileService'
import { getErrorMessage } from "../utils/handleServerError";

const applyThemeToDOM = (theme) => {
  if (typeof document === 'undefined') return;
  document.documentElement.dataset.theme = theme;
  document.documentElement.style.colorScheme = theme;
};

const useProfileStore = create((set, get) => ({
    email: null,
    theme: 'dark',
    remindTime: '08:00',
    city: null,
    error: null,
    isLoading: false,
    isLoaded: false,

    fetchProfile: async () => {
        if(get().isLoaded) return
        set({isLoading: true, error: null})
        try {
            const data = await getProfile()
            const theme = data.theme || 'dark'
            applyThemeToDOM(theme)
            set({
                email: data.email,
                theme: data.theme || 'dark',
                remindTime: data.remindTime || '08:00',
                city: data.city,
                isLoading: false,
                isLoaded: true
            })
        } catch (err) {
            set({error: getErrorMessage(err), isLoading: false})
        }
    },

    toggleTheme: async () => {
        const currentTheme = get().theme
        const newTheme = currentTheme === 'dark' ? 'light' : 'dark'
        applyThemeToDOM(newTheme)
        set ({ theme: newTheme, isLoading: true})
        try {
            await updateProfile({ theme: newTheme })
            set({isLoading: false})
        } catch (err) {
            set({theme: currentTheme, error: getErrorMessage(err), isLoading: false})
        }
    },
    
    updProfile: async (updates) => {
        set({isLoading: true, error: null})
        try {
            const data = await updateProfile(updates)
                  if (data.theme) applyThemeToDOM(data.theme);
            set({
                email: data.email,
                theme: data.theme || get().theme,
                remindTime: data.remindTime,
                city: data.city,
                isLoading: false,
            })
        } catch (err) {
            set({error: getErrorMessage(err), isLoading: false})
        }
    },

    clearError: () => set({ error: null })
}))

export default useProfileStore
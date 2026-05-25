import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import { loginUser, registerUser } from '../services/AuthService';
import { getProfile, updateProfile } from '../services/profileService';
import { getErrorMessage } from '../utils/handleServerError';
import apiClient from '../services/apiClient';
import useHabits from './useHabitsStore';
import useInsight from './useInsightStore';
import useDailySummaryStore from './useDailySummaryStore';
import useThemeStore from './useThemeStore';

const normalizeTheme = (theme) => (theme === 'dark' ? 'dark' : 'light');

const useAuthUser = create(
  persist(
    (set, get) => ({
      email: null,
      city: '',
      profile: null,
      isAuthenticated: false,
      isLoading: false,
      profileLoading: false,
      error: null,
      profileError: null,
      registrationMessage: null,

      register: async (email, city, password) => {
        set({ isLoading: true, error: null, registrationMessage: null });
        try {
          const data = await registerUser(email, city, password);
          set({
            email: data.email,
            city,
            registrationMessage: data.message,
            isAuthenticated: false,
            isLoading: false,
          });
          return data;
        } catch (err) {
          set({ error: getErrorMessage(err), isLoading: false });
          return null;
        }
      },

      login: async (email, password) => {
        set({ isLoading: true, error: null });
        try {
          const data = await loginUser(email, password);
          set({
            email: data.email,
            isAuthenticated: true,
            isLoading: false,
          });
          await get().loadProfile();
          return data;
        } catch (err) {
          set({
            error: getErrorMessage(err),
            isAuthenticated: false,
            isLoading: false,
          });
          return null;
        }
      },

      loadProfile: async () => {
        if (!get().isAuthenticated) return null;
        set({ profileLoading: true, profileError: null });
        try {
          const profile = await getProfile();
          const theme = normalizeTheme(profile.themePreference);
          useThemeStore.getState().setTheme(theme);
          set({
            profile,
            email: profile.email,
            city: profile.city || '',
            profileLoading: false,
          });
          return profile;
        } catch (err) {
          const message = getErrorMessage(err);
          set({ profileError: message, profileLoading: false });
          if (err.response?.status === 401) {
            set({ isAuthenticated: false, profile: null });
          }
          return null;
        }
      },

      saveProfile: async (updates) => {
        set({ profileLoading: true, profileError: null });
        try {
          const current = get().profile || {};
          const payload = {
            name: current.name || null,
            city: updates.city ?? current.city ?? '',
            habitReminderEnabled:
              updates.habitReminderEnabled ?? current.habitReminderEnabled ?? false,
            habitReminderTime:
              updates.habitReminderTime ?? current.habitReminderTime ?? null,
            themePreference: normalizeTheme(
              updates.themePreference ?? current.themePreference ?? useThemeStore.getState().theme,
            ),
          };
          const profile = await updateProfile(payload);
          const savedTheme = normalizeTheme(
            profile.themePreference || payload.themePreference || useThemeStore.getState().theme,
          );
          useThemeStore.getState().setTheme(savedTheme);
          set({
            profile: { ...profile, themePreference: savedTheme },
            email: profile.email,
            city: profile.city || '',
            profileLoading: false,
          });
          return profile;
        } catch (err) {
          set({ profileError: getErrorMessage(err), profileLoading: false });
          return null;
        }
      },

      clearError: () => set({ error: null, profileError: null }),

      logout: async () => {
        set({ isLoading: true });
        try {
          await apiClient.post('/auth/logout');
        } catch {
          // Local state still needs to be cleared if the session already expired.
        }
        useHabits.getState().clearHabits();
        useInsight.getState().clearInsight();
        useDailySummaryStore.getState().clearSummary();
        set({
          email: null,
          city: '',
          profile: null,
          isAuthenticated: false,
          isLoading: false,
          error: null,
          profileError: null,
        });
      },
    }),
    {
      name: 'auth-storage',
      partialize: (state) => ({
        email: state.email,
        city: state.city,
        profile: state.profile,
        isAuthenticated: state.isAuthenticated,
      }),
    },
  ),
);

export default useAuthUser;

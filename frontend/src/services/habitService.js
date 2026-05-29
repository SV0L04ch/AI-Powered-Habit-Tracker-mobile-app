// src/services/habitService.js
import axios from "axios";

const api = axios.create({
  baseURL: '/api',
});

// Функция для получения токена из auth-storage
const getToken = () => {
  try {
    const authStorage = localStorage.getItem('auth-storage');
    if (authStorage) {
      const parsed = JSON.parse(authStorage);
      return parsed.state?.token || null;
    }
  } catch (e) {
    console.error('Ошибка при чтении токена:', e);
  }
  return null;
};

// Перехватчик — добавляет токен в каждый запрос
api.interceptors.request.use(
  (config) => {
    const token = getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

export const createHabit = async (habitData) => {
    const response = await api.post('/habits', habitData);
    return response.data;
};

export const fetchHabits = async () => {
    const response = await api.get('/habits');
    return response.data;
};

export const updHabit = async (id, updates) => {
    const response = await api.put(`/habits/${id}`, updates);
    return response.data;
};

export const delHabit = async (id) => {
    const response = await api.delete(`/habits/${id}`);
    return response.data;
};
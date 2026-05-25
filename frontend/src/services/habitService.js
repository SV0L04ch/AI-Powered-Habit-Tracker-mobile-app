import apiClient from './apiClient';

export const createHabit = async (habitData) => {
  const response = await apiClient.post('/habits', habitData);
  return response.data;
};

export const fetchHabits = async () => {
  const response = await apiClient.get('/habits');
  return response.data;
};

export const fetchHabitEntries = async (habitId, params = {}) => {
  const response = await apiClient.get(`/habits/${habitId}/entries`, { params });
  return response.data;
};

export const createHabitEntry = async (habitId, entryData) => {
  const response = await apiClient.post(`/habits/${habitId}/entries`, entryData);
  return response.data;
};

export const updateHabitEntry = async (habitId, entryId, entryData) => {
  const response = await apiClient.put(`/habits/${habitId}/entries/${entryId}`, entryData);
  return response.data;
};

export const updHabit = async (id, updates) => {
  const response = await apiClient.put(`/habits/${id}`, updates);
  return response.data;
};

export const delHabit = async (id) => {
  const response = await apiClient.delete(`/habits/${id}`);
  return response.data;
};

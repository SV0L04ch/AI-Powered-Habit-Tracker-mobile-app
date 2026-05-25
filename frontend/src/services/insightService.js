import apiClient from './apiClient';

export const getHabitInsight = async (habitId, scenario = 'daily') => {
  const response = await apiClient.post(`/habits/${habitId}/insights/support`, {
    scenario,
  });
  return response.data;
};

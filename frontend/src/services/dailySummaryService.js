import apiClient from './apiClient';

export const getDailySummary = async (date) => {
  const params = {};
  if (date) params.date = date;
  const response = await apiClient.get('/stats/daily-summary', { params });
  return response.data;
};

export const getCitySummary = async (city) => {
  const response = await apiClient.get('/stats/city-summary', {
    params: { city },
  });
  return response.data;
};

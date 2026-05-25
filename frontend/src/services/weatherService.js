import apiClient from './apiClient';

export const getWeather = async (city, date) => {
  const params = { city };
  if (date) params.date = date;
  const response = await apiClient.get('/weather', { params });
  return response.data;
};

import apiClient from './apiClient';

export const getProfile = async () => {
  const response = await apiClient.get('/profile');
  return response.data;
};

export const updateProfile = async (profileData) => {
  const response = await apiClient.put('/profile', profileData);
  return response.data;
};

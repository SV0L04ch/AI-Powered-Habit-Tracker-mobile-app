import apiClient from './apiClient';

export const loginUser = async (email, password) => {
  const response = await apiClient.post('/auth/login', {
    email,
    password,
  });
  return response.data;
};

export const registerUser = async (email, city, password) => {
  const response = await apiClient.post('/auth/register', {
    email,
    city,
    password,
  });
  return response.data;
};

import axios from 'axios';

export const getCitySummary = async (city) => {
  const response = await axios.get('/api/stats/city-summary', {
    params: { city },
  });
  return response.data;
};
import axios from 'axios';

export const getHabitInsight = async (habitId, habitName) => {
  const response = await axios.post(
    `/api/habits/${habitId}/insights/support`,
    {},
  );
  return response.data;
};
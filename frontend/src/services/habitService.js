import axios from "axios";

export const createHabit = async (habitData) => {
    const response = await axios.post('/api/habits', habitData)
    return response.data
}

export const fetchHabits = async () => {
    const response = await axios.get('/api/habits')
    return response.data
}

export const updHabit = async (id, updates) => {
    const response = await axios.put(`/api/habits/${id}`, updates)
    return response.data
}

export const delHabit = async (id) => {
    const response = await axios.delete(`/api/habits/${id}`)
    return response.data
}
 
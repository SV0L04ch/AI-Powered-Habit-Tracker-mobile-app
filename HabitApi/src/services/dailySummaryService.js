import axios from "axios";

export const getDailySummary = async (date) => {
    const params = {}
    if (date) params.date = date
    const response = await axios.get('/api/stats/daily-summary', {
        params
    })
    return response.data
} 
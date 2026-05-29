import axios from 'axios'

export const getProfile = async () => {
    const response = await axios.get('/profile')
    return response.data
}

export const updateProfile = async (profileData) => {
    const response = await axios.put('/profile', profileData)
    return response.data
}
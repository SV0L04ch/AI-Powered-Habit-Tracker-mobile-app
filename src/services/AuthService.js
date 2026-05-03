import axios from 'axios';


export const loginUser = async (email, password) => {
    const response = await axios.post(`/api/auth/login`, { 
        Email: email,
        Password: password });
    return response.data; // ожидаем { token, email, city }
};

export const registerUser = async (email, city, password) => {
    const response = await axios.post(`/api/auth/register`, { 
        Email: email,
        City: city, 
        Password: password });
    return response.data; // ожидаем { token, email, city }
};
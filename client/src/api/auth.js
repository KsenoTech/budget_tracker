import axios from "axios";

const API_URL = "https://localhost:7007/api/auth"; // Ваш API URL

export const register = async (username, password, email) => {
  const response = await axios.post(`${API_URL}/register`, {
    username,
    password,
    email,
  });
  return response.data.token;
};

export const login = async (username, password) => {
  const response = await axios.post(`${API_URL}/login`, { username, password });
  return response.data.token;
};

export const checkToken = async (token) => {
    console.log('Checking token:', token); // Отладка токена
    try {
      const response = await axios.get(`${API_URL}/check`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      console.log('Check response:', response.data); // Отладка ответа
      return response.data;
    } catch (error) {
      console.error('Check token error:', error.response ? error.response.data : error.message);
      throw error;
    }
  };

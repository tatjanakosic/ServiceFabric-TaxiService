import axios from 'axios';

const API_URL = 'http://localhost:8975/Communication'; // Replace with your API endpoint

export const register = async (userData) => {
  try {
    const response = await axios.post(`${process.env.REACT_APP_COMM}/register`, userData);
    return response.data;
  } catch (error) {
    throw error.response.data; // Handle specific error response
  }
};

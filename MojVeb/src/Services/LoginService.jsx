import axios from 'axios';

export const login = async (email, password) => {
  try {
    const response = await axios.post(`http://localhost:8975/Communication/login`, { email, password });
    if (response.data && response.data.token) {
      return response.data.token; // Return the token upon successful login
    } else {
      throw new Error('Token not found in response');
    }
  } catch (error) {
    console.error('Error during login:', error);
    if (error.response && error.response.data) {
      throw new Error(error.response.data.message || 'Login failed');
    } else {
      throw new Error('Login failed. Please check your network connection.');
    }
  }
};

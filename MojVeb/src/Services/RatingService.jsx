import axios from 'axios';

const API_URL = 'http://localhost:8975/Ride'; // Adjust according to your actual API URL

const RatingService = {
  submitRating: async ({ rating, isAccepted }) => {
    try {
      const token = localStorage.getItem('token');
      if (!token) {
        throw new Error('No token found');
      }
      const response = await axios.post(`${process.env.REACT_APP_RIDE}/submit`, { rating, isAccepted }, {
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
      });
      return response.data;
    } catch (error) {
      console.error('Error submitting rating:', error);
      throw error;
    }
  }
};

export default RatingService;

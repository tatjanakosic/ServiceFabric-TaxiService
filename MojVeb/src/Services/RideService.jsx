import axios from 'axios';

const RideService = {
  orderRide: async (rideData) => {
    try {
      const token = localStorage.getItem('token');
      if (!token) {
        throw new Error('No token found');
      }
      const response = await axios.post(`${process.env.REACT_APP_RIDE}/orderRide`, rideData, {
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
      });
      return response;
    } catch (error) {
      console.error('Error ordering ride:', error);
      throw error;
    }
  },

  confirmRide: async (rideDetails) => {
    try {
      const token = localStorage.getItem('token');
      if (!token) {
        throw new Error('No token found');
      }
      const response = await axios.post(`${process.env.REACT_APP_RIDE}/confirmRide`, rideDetails, {
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
      });
      return response;
    } catch (error) {
      console.error('Error confirming ride:', error);
      throw error;
    }
  },

    getAdminRideHistory: async () => {
    try {
      const token = localStorage.getItem('token');
      if (!token) {
        throw new Error('No token found');
      }
      const response = await axios.put(`${process.env.REACT_APP_RIDE}/adminRideHistory`, {}, {
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching ride history:', error);
      throw error;
    }
  },

  getDriverRideHistory: async () => {
    try {
      const token = localStorage.getItem('token');
      if (!token) {
        throw new Error('No token found');
      }
      const response = await axios.put(`${process.env.REACT_APP_RIDE}/driverRideHistory`, {}, {
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching ride history:', error);
      throw error;
    }
  },

    getRideHistory: async () => {
    try {
      const token = localStorage.getItem('token');
      if (!token) {
        throw new Error('No token found');
      }
      const response = await axios.put(`${process.env.REACT_APP_RIDE}/rideHistory`, {}, {
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching ride history:', error);
      throw error;
    }
  },

  getAvailableRides: async () => {
    try {
      const token = localStorage.getItem('token');
      if (!token) {
        throw new Error('No token found');
      }
      const response = await axios.put(`${process.env.REACT_APP_RIDE}/availableRides`, {}, {
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching available rides:', error);
      throw error;
    }
  },

  acceptRide: async (rideId) => {
    try {
      const token = localStorage.getItem('token');
      if (!token) {
        throw new Error('No token found');
      }
      const response = await axios.get(`${process.env.REACT_APP_RIDE}/acceptRide/${rideId}`, {
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
      });
      return response.data;
    } catch (error) {
      console.error('Error accepting ride:', error);
      throw error;
    }
  },
};

export default RideService;

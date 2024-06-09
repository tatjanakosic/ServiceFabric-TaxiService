import axios from 'axios';

const CountdownService = {
    getCountdown: async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) {
                throw new Error('No token found');
            }
            const response = await axios.get(`${process.env.REACT_APP_RIDE}/countdown`, {
                headers: {
                    Authorization: `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
            });
            console.log('API response:', response.data); // Debug: Log the API response
            return response.data;
        } catch (error) {
            console.error('Error fetching countdown:', error);
            throw error;
        }
    }
};

export default CountdownService;

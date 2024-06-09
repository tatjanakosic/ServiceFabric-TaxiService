import React, { useState, useEffect } from 'react';
import RideService from '../Services/RideService';
import '../Style/HistoryComponent.css';

const HistoryComponent = () => {
  const [rideHistory, setRideHistory] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchRideHistory = async () => {
    setLoading(true);
    try {
      const history = await RideService.getRideHistory();
      console.log('Ride History:', history); // Log the response for debugging
      setRideHistory(history);
    } catch (error) {
      console.error('Error fetching ride history:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchRideHistory();
  }, []);

  return (
    <div className="history-component">
      <h2>History Page</h2>
      <button onClick={fetchRideHistory}>Refresh</button>
      {loading ? (
        <p>Loading...</p>
      ) : rideHistory.length > 0 ? (
        <table>
          <thead>
            <tr>
              <th>Id</th>
              <th>Start Address</th>
              <th>End Address</th>
              <th>Wait Duration</th>
              <th>Ride Duration</th>
              <th>Price</th>
            </tr>
          </thead>
          <tbody>
            {rideHistory.map((ride) => (
              <tr key={ride.id}>
                <td>{ride.id}</td>
                <td>{ride.startAdress}</td>
                <td>{ride.endAdress}</td>
                <td>{ride.waitDuration}</td>
                <td>{ride.rideDuration}</td>
                <td>{ride.price}</td>
              </tr>
            ))}
          </tbody>
        </table>
      ) : (
        <p>No ride history available.</p>
      )}
    </div>
  );
};

export default HistoryComponent;

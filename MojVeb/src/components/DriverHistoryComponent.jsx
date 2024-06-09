import React, { useState, useEffect } from 'react';
import RideService from '../Services/RideService';
import '../Style/HistoryComponent.css';

const DriverHistoryComponent = () => {
  const [driverRideHistory, setDriverRideHistory] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchDriverRideHistory = async () => {
    setLoading(true);
    try {
      const history = await RideService.getDriverRideHistory();
      console.log('Driver Ride History:', history); // Log the response for debugging
      setDriverRideHistory(history);
    } catch (error) {
      console.error('Error fetching driver ride history:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchDriverRideHistory();
  }, []);

  return (
    <div className="history-component">
      <h2>Driver Ride History</h2>
      <button onClick={fetchDriverRideHistory}>Refresh</button>
      {loading ? (
        <p>Loading...</p>
      ) : driverRideHistory.length > 0 ? (
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
            {driverRideHistory.map((ride) => (
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

export default DriverHistoryComponent;

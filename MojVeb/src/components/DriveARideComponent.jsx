import React, { useState, useEffect } from 'react';
import RideService from '../Services/RideService';
import '../Style/HistoryComponent.css';

const DriveARideComponent = () => {
  const [availableRides, setAvailableRides] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchAvailableRides = async () => {
    setLoading(true);
    try {
      const rides = await RideService.getAvailableRides();
      console.log('Available Rides:', rides); // Log the response for debugging
      setAvailableRides(rides);
    } catch (error) {
      console.error('Error fetching available rides:', error);
      setAvailableRides([]); // Ensure the state is set to an empty array on error
    } finally {
      setLoading(false);
    }
  };

  const acceptRide = async (rideId) => {
    try {
      await RideService.acceptRide(rideId);
      fetchAvailableRides(); // Refresh the available rides after accepting a ride
    } catch (error) {
      console.error('Error accepting ride:', error);
    }
  };

  useEffect(() => {
    fetchAvailableRides();
  }, []);

  return (
    <div className="history-component">
      <h2>Drive A Ride</h2>
      <button onClick={fetchAvailableRides}>Refresh</button>
      {loading ? (
        <p>Loading...</p>
      ) : availableRides.length > 0 ? (
        <table>
          <thead>
            <tr>
              <th>Id</th>
              <th>Start Address</th>
              <th>End Address</th>
              <th>Wait Duration</th>
              <th>Ride Duration</th>
              <th>Price</th>
              <th>Action</th>
            </tr>
          </thead>
          <tbody>
            {availableRides.map((ride) => (
              <tr key={ride.id}>
                <td>{ride.id}</td>
                <td>{ride.startAdress}</td>
                <td>{ride.endAdress}</td>
                <td>{ride.waitDuration}</td>
                <td>{ride.rideDuration}</td>
                <td>{ride.price}</td>
                <td>
                  <button onClick={() => acceptRide(ride.id)}>Accept Ride</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      ) : (
        <p>No available rides.</p>
      )}
    </div>
  );
};

export default DriveARideComponent;

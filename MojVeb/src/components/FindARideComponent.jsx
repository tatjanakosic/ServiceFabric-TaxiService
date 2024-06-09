import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import RideService from '../Services/RideService';
import '../Style/FindARideComponent.css';

const FindARideComponent = () => {
  const [startAddress, setStartAddress] = useState('');
  const [endAddress, setEndAddress] = useState('');
  const [rideDetails, setRideDetails] = useState(null);
  const navigate = useNavigate();

  const handleOrderRide = async () => {
    try {
      const response = await RideService.orderRide({ StartAdress: startAddress, EndAdress: endAddress });
      setRideDetails({
        StartAdress: startAddress,
        EndAdress: endAddress,
        Duration: response.data.duration,
        Price: response.data.price,
      });
    } catch (error) {
      console.error('Error ordering ride:', error);
      alert('Failed to order ride');
    }
  };

  const handleConfirmRide = async () => {
    try {
      await RideService.confirmRide(rideDetails);
      alert('Ride confirmed successfully');
      navigate(`/countdown`); // Navigate to UserCountdownComponent without email
    } catch (error) {
      console.error('Error confirming ride:', error);
      alert('Failed to confirm ride');
    }
  };

  return (
    <div className="find-ride-component">
      <h2>Find A Ride</h2>
      <div className="ride-form">
        <div>
          <label>Start Address:</label>
          <input
            type="text"
            value={startAddress}
            onChange={(e) => setStartAddress(e.target.value)}
          />
        </div>
        <div>
          <label>End Address:</label>
          <input
            type="text"
            value={endAddress}
            onChange={(e) => setEndAddress(e.target.value)}
          />
        </div>
        <button onClick={handleOrderRide}>Order</button>
      </div>
      {rideDetails && (
        <div className="ride-details">
          <div>
            <label>Duration:</label>
            <input
              type="text"
              value={rideDetails.Duration}
              readOnly
            />
          </div>
          <div>
            <label>Price:</label>
            <input
              type="text"
              value={rideDetails.Price}
              readOnly
            />
          </div>
          <button onClick={handleConfirmRide}>Confirm</button>
        </div>
      )}
    </div>
  );
};

export default FindARideComponent;

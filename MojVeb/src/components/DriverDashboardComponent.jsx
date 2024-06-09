// src/components/UserDashboardComponent.js
import React from 'react';
import { useNavigate } from 'react-router-dom';
import '../Style/UserDashboardComponent.css'; // Ensure you have some styles for the dashboard

const DriverDashboardComponent = () => {
  const navigate = useNavigate();

  const handleNavigation = (path) => {
    navigate(path);
  };

  return (
    <div className="user-dashboard-component">
      <h2>Driver Dashboard</h2>
      <div className="dashboard-buttons">
        <button onClick={() => handleNavigation('/profile')}>Profile</button>
        <button onClick={() => handleNavigation('/drive-a-ride')}>Find A Ride</button>
        <button onClick={() => handleNavigation('/driver-history')}>History</button>
        <button onClick={() => handleNavigation('/')}>Logout</button>
      </div>
    </div>
  );
};

export default DriverDashboardComponent;

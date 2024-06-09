// src/components/UserDashboardComponent.js
import React from 'react';
import { useNavigate } from 'react-router-dom';
import '../Style/UserDashboardComponent.css'; // Ensure you have some styles for the dashboard

const UserDashboardComponent = () => {
  const navigate = useNavigate();

  const handleNavigation = (path) => {
    navigate(path);
  };

  return (
    <div className="user-dashboard-component">
      <h2>User Dashboard</h2>
      <div className="dashboard-buttons">
        <button onClick={() => handleNavigation('/profile')}>Profile</button>
        <button onClick={() => handleNavigation('/find-a-ride')}>Find A Ride</button>
        <button onClick={() => handleNavigation('/history')}>History</button>
        <button onClick={() => handleNavigation('/')}>Logout</button>
      </div>
    </div>
  );
};

export default UserDashboardComponent;

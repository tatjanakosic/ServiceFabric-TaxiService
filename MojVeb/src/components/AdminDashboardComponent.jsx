// src/components/UserDashboardComponent.js
import React from 'react';
import { useNavigate } from 'react-router-dom';
import '../Style/UserDashboardComponent.css'; // Ensure you have some styles for the dashboard

const AdminDashboardComponent = () => {
  const navigate = useNavigate();

  const handleNavigation = (path) => {
    navigate(path);
  };

  return (
    <div className="user-dashboard-component">
      <h2>Admin Dashboard</h2>
      <div className="dashboard-buttons">
        <button onClick={() => handleNavigation('/profile')}>Profile</button>
        <button onClick={() => handleNavigation('/verification')}>Verification</button>
        <button onClick={() => handleNavigation('/blocking')}>Blocking</button>
        <button onClick={() => handleNavigation('/admin-history')}>History</button>
        <button onClick={() => handleNavigation('/')}>Logout</button>
      </div>
    </div>
  );
};

export default AdminDashboardComponent;

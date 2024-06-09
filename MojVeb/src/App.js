import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import StartComponent from './components/StartComponent';
import RegisterComponent from './components/RegisterComponent';
import UserDashboardComponent from './components/UserDashboardComponent';
import ProfileComponent from './components/ProfileComponent';
import FindARideComponent from './components/FindARideComponent';
import DriveARideComponent from './components/DriveARideComponent';
import HistoryComponent from './components/HistoryComponent';
import DriverHistoryComponent from './components/DriverHistoryComponent';
import DriverDashboardComponent from './components/DriverDashboardComponent';
import AdminDashboardComponent from './components/AdminDashboardComponent';
import AdminHistoryComponent from "./components/AdminHistoryComponent";
import VerificationComponent from "./components/VerificationComponent";
import BlockingComponent from "./components/BlockingComponent";
import UserCountdownComponent from './components/UserCountdownComponent';
import UserRatingComponent from './components/UserRatingComponent'; // Import UserRatingComponent

const App = () => {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<StartComponent />} />
        <Route path="/register" element={<RegisterComponent />} />
        <Route path="/user-dashboard" element={<UserDashboardComponent />} />
        <Route path="/driver-dashboard" element={<DriverDashboardComponent />} />
        <Route path="/admin-dashboard" element={<AdminDashboardComponent />} />
        <Route path="/profile" element={<ProfileComponent />} />
        <Route path="/find-a-ride" element={<FindARideComponent />} />
        <Route path="/drive-a-ride" element={<DriveARideComponent />} />
        <Route path="/history" element={<HistoryComponent />} />
        <Route path="/driver-history" element={<DriverHistoryComponent />} />
        <Route path="/admin-history" element={<AdminHistoryComponent />} />
        <Route path="/verification" element={<VerificationComponent />} />
        <Route path="/blocking" element={<BlockingComponent />} />
        <Route path="/countdown" element={<UserCountdownComponent />} />
        <Route path="/user-rating" element={<UserRatingComponent />} /> {/* Add route */}
      </Routes>
    </Router>
  );
};

export default App;

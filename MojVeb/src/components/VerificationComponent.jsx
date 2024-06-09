import React, { useState, useEffect } from 'react';
import VerificationService from '../Services/VerificationService';
import '../Style/HistoryComponent.css';

const getVerificationStatusText = (status) => {
  switch (status) {
    case 0:
      return 'Pending';
    case 1:
      return 'Accepted';
    case 2:
      return 'Declined';
    default:
      return 'Unknown';
  }
};

const VerificationComponent = () => {
  const [usersForVerification, setUsersForVerification] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchUsersForVerification = async () => {
    setLoading(true);
    try {
      const users = await VerificationService.getUsersForVerification();
      console.log('Users for Verification:', users); // Log the response for debugging
      setUsersForVerification(users);
    } catch (error) {
      console.error('Error fetching users for verification:', error);
      setUsersForVerification([]); // Ensure the state is set to an empty array on error
    } finally {
      setLoading(false);
    }
  };

  const verifyUser = async (userId) => {
    try {
      await VerificationService.verifyUser(userId);
      fetchUsersForVerification(); // Refresh the users list after verification
    } catch (error) {
      console.error('Error verifying user:', error);
    }
  };

  const denyUser = async (userId) => {
    try {
      await VerificationService.denyUser(userId);
      fetchUsersForVerification(); // Refresh the users list after denying
    } catch (error) {
      console.error('Error denying user:', error);
    }
  };

  useEffect(() => {
    fetchUsersForVerification();
  }, []);

  return (
    <div className="history-component">
      <h2>User Verification</h2>
      <button onClick={fetchUsersForVerification}>Refresh</button>
      {loading ? (
        <p>Loading...</p>
      ) : usersForVerification.length > 0 ? (
        <table>
          <thead>
            <tr>
              <th>Username</th>
              <th>Name</th>
              <th>Last Name</th>
              <th>Email</th>
              <th>Date of Birth</th>
              <th>Address</th>
              <th>Verification Status</th>
              <th>Action</th>
            </tr>
          </thead>
          <tbody>
            {usersForVerification.map((user) => (
              <tr key={user.email}>
                <td>{user.username}</td>
                <td>{user.name}</td>
                <td>{user.lastName}</td>
                <td>{user.email}</td>
                <td>{new Date(user.dateOfBirth).toLocaleDateString()}</td>
                <td>{user.address}</td>
                <td>{getVerificationStatusText(user.verificationStatus)}</td>
                <td>
                  <button onClick={() => verifyUser(user.email)}>Verify</button>
                  <button onClick={() => denyUser(user.email)}>Deny</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      ) : (
        <p>No users awaiting verification.</p>
      )}
    </div>
  );
};

export default VerificationComponent;

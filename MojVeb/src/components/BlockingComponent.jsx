import React, { useState, useEffect } from 'react';
import BlockingService from '../Services/BlockingService';
import '../Style/HistoryComponent.css';

const getBlockingStatusText = (status) => {
  switch (status) {
    case 0:
      return 'Not Blocked';
    case 1:
      return 'Blocked';
    default:
      return 'Unknown';
  }
};

const BlockingComponent = () => {
  const [usersForBlocking, setUsersForBlocking] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchUsersForBlocking = async () => {
    setLoading(true);
    try {
      const users = await BlockingService.getUsersForBlocking();
      console.log('Users for Blocking:', users); // Log the response for debugging
      setUsersForBlocking(users);
    } catch (error) {
      console.error('Error fetching users for blocking:', error);
      setUsersForBlocking([]); // Ensure the state is set to an empty array on error
    } finally {
      setLoading(false);
    }
  };

  const blockUser = async (userId) => {
    try {
      await BlockingService.blockUser(userId);
      fetchUsersForBlocking(); // Refresh the users list after blocking
    } catch (error) {
      console.error('Error blocking user:', error);
    }
  };

  const unblockUser = async (userId) => {
    try {
      await BlockingService.unblockUser(userId);
      fetchUsersForBlocking(); // Refresh the users list after unblocking
    } catch (error) {
      console.error('Error unblocking user:', error);
    }
  };

  useEffect(() => {
    fetchUsersForBlocking();
  }, []);

  return (
    <div className="history-component">
      <h2>User Blocking</h2>
      <button onClick={fetchUsersForBlocking}>Refresh</button>
      {loading ? (
        <p>Loading...</p>
      ) : usersForBlocking.length > 0 ? (
        <table>
          <thead>
            <tr>
              <th>Username</th>
              <th>Name</th>
              <th>Last Name</th>
              <th>Email</th>
              <th>Date of Birth</th>
              <th>Address</th>
              <th>Rating</th>
              <th>Block Status</th>
              <th>Action</th>
            </tr>
          </thead>
          <tbody>
            {usersForBlocking.map((user) => (
              <tr key={user.email}>
                <td>{user.username}</td>
                <td>{user.name}</td>
                <td>{user.lastName}</td>
                <td>{user.email}</td>
                <td>{new Date(user.dateOfBirth).toLocaleDateString()}</td>
                <td>{user.address}</td>
                <td>{user.rating.toFixed(2)}</td>
                <td>{getBlockingStatusText(user.blockingStatus)}</td>
                <td>
                  {user.blockingStatus === 0 ? (
                    <button onClick={() => blockUser(user.email)}>Block</button>
                  ) : (
                    <button onClick={() => unblockUser(user.email)}>Unblock</button>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      ) : (
        <p>No users awaiting blocking.</p>
      )}
    </div>
  );
};

export default BlockingComponent;

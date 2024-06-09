import React, { useEffect, useState } from 'react';
import ProfileService from '../Services/ProfileService';
import { Navigate } from 'react-router-dom';
import '../Style/ProfileComponent.css';

const ProfileComponent = () => {
  const [profileData, setProfileData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [formData, setFormData] = useState({
    email: '',
    username: '',
    name: '',
    lastName: '',
    dateOfBirth: '',
    address: '',
    verificationStatus: 0
  });

  useEffect(() => {
    const fetchProfileData = async () => {
      try {
        const data = await ProfileService.getProfile();
        setProfileData(data);
        setFormData({
          email: data.email, // Add email field
          username: data.username,
          name: data.name,
          lastName: data.lastName,
          dateOfBirth: new Date(data.dateOfBirth).toISOString().split('T')[0],
          address: data.address,
          verificationStatus: data.verificationStatus
        });
      } catch (error) {
        if (error.response && error.response.status === 401) {
          setError('Unauthorized. Please log in again.');
        } else {
          setError('Failed to fetch profile data.');
        }
      } finally {
        setLoading(false);
      }
    };

    fetchProfileData();
  }, []);

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

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData((prevData) => ({
      ...prevData,
      [name]: value
    }));
  };

  const handleUpdateProfile = async () => {
    try {
      await ProfileService.updateProfile(formData);
      setProfileData(formData);
      alert('Profile updated successfully');
    } catch (error) {
      console.error('Error updating profile data:', error);
      alert('Failed to update profile data');
    }
  };

  if (loading) {
    return <div>Loading...</div>;
  }

  if (error) {
    if (error === 'Unauthorized. Please log in again.') {
      return <Navigate to="/login" />;
    }
    return <div>{error}</div>;
  }

  return (
    <div className="profile-component">
      <h2>Profile Page</h2>
      <div className="profile-info">
        <div>
          <strong>Email:</strong>
          <input
            type="text"
            name="email"
            value={formData.email}
            readOnly
          />
        </div>
        <div>
          <strong>Username:</strong>
          <input
            type="text"
            name="username"
            value={formData.username}
            onChange={handleInputChange}
          />
        </div>
        <div>
          <strong>Name:</strong>
          <input
            type="text"
            name="name"
            value={formData.name}
            onChange={handleInputChange}
          />
        </div>
        <div>
          <strong>Last Name:</strong>
          <input
            type="text"
            name="lastName"
            value={formData.lastName}
            onChange={handleInputChange}
          />
        </div>
        <div>
          <strong>Date of Birth:</strong>
          <input
            type="date"
            name="dateOfBirth"
            value={formData.dateOfBirth}
            onChange={handleInputChange}
          />
        </div>
        <div>
          <strong>Address:</strong>
          <input
            type="text"
            name="address"
            value={formData.address}
            onChange={handleInputChange}
          />
        </div>
        <div>
          <strong>Verification Status:</strong>
          <input
            type="text"
            name="verificationStatus"
            value={getVerificationStatusText(formData.verificationStatus)}
            readOnly
          />
        </div>
        <button onClick={handleUpdateProfile}>Update</button>
      </div>
    </div>
  );
};

export default ProfileComponent;

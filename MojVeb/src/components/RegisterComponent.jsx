import React, { useState } from 'react';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import { register } from '../Services/RegisterService'; // Ensure this path is correct
import { GoogleLogin } from '@react-oauth/google';
import { jwtDecode } from 'jwt-decode';
import '../Style/RegisterComponent.css';
import {Navigate} from "react-router-dom";
import { useNavigate } from 'react-router-dom'; // Import useNavigate

const RegisterComponent = () => {
  const [formData, setFormData] = useState({
    Email: '',
    Password: '',
    ConfirmPassword: '',
    Username: '',
    Name: '',
    LastName: '',
    DateOfBirth: null,
    Address: '',
    UserType: '', // Now a string for URL or any text input
    Image: '', // Now a string for URL or any text input
  });

  const navigate = useNavigate(); // Initialize useNavigate
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  const handleDateChange = (date) => {
    setFormData({ ...formData, DateOfBirth: date });
  };

  const handleGoogleLoginSuccess = (response) => {
    const token = response.credential;
    const userObject = jwtDecode(token);
    console.log(userObject); // Log the decoded user object for debugging
    setFormData({
      ...formData,
      Email: userObject.email,
      Name: userObject.given_name,
      LastName: userObject.family_name,
      // Add additional fields if necessary, e.g., DateOfBirth or Image if available
    });
  };

  const handleGoogleLoginFailure = (error) => {
    console.error('Google login error:', error);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (formData.Password !== formData.ConfirmPassword) {
      alert('Passwords do not match');
      return;
    }

    const userPayload = {
      email: formData.Email,
      password: formData.Password,
      username: formData.Username,
      name: formData.Name,
      lastName: formData.LastName,
      dateOfBirth: formData.DateOfBirth,
      address: formData.Address,
      userType: parseInt(formData.UserType), // Convert to integer
      image: formData.Image,
    };

    try {
      const response = await register(userPayload);
      console.log(response);
        navigate('/'); // Use navigate function to redirect
    } catch (error) {
      console.error('Error submitting form:', error);
      // Handle error
    }
  };

  return (
    <div className="register-component">
      <h2>Register</h2>
      <form onSubmit={handleSubmit}>
        <div>
          <label>Email:</label>
          <br/>
          <input type="email" name="Email" value={formData.Email} onChange={handleChange} required />
        </div>
        <div>
          <label>Password:</label>
          <br/>
          <input type="password" name="Password" value={formData.Password} onChange={handleChange} required />
        </div>
        <div>
          <label>Confirm Password:</label>
          <br/>
          <input type="password" name="ConfirmPassword" value={formData.ConfirmPassword} onChange={handleChange} required />
        </div>
        <div>
          <label>Username:</label>
          <input type="text" name="Username" value={formData.Username} onChange={handleChange} required />
        </div>
        <div>
          <label>Name:</label>
          <input type="text" name="Name" value={formData.Name} onChange={handleChange} required />
        </div>
        <div>
          <label>Last Name:</label>
          <input type="text" name="LastName" value={formData.LastName} onChange={handleChange} required />
        </div>
        <div>
          <label>Date of Birth:</label>
          <br/>
          <DatePicker
            selected={formData.DateOfBirth}
            onChange={handleDateChange}
            dateFormat="dd/MM/yyyy"
            required
          />
        </div>
        <div>
          <label>Address:</label>
          <br/>
          <input type="text" name="Address" value={formData.Address} onChange={handleChange} required />
        </div>
        <div>
          <label>User Type:</label>
          <br/>
          <select name="UserType" value={formData.UserType} onChange={handleChange} required>
            <option value="">Select User Type</option>
            <option value="0">Driver</option>
            <option value="1">Admin</option>
            <option value="2">User</option>
          </select>
        </div>
        <div>
          <label>Image URL:</label>
          <br/>
          <input type="text" name="Image" value={formData.Image} onChange={handleChange} />
        </div>
        <button type="submit">Register</button>
      </form>
      <br/>
      <GoogleLogin
        onSuccess={handleGoogleLoginSuccess}
        onFailure={handleGoogleLoginFailure}
        cookiePolicy={'single_host_origin'}
      />
    </div>
  );
};

export default RegisterComponent;

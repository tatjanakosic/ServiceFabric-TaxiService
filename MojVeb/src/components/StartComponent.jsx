import React, { useState } from 'react';
import { Navigate } from 'react-router-dom';
import '../Style/StartComponent.css';
import { login } from '../Services/LoginService';
const StartComponent = () => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [redirectToRegister, setRedirectToRegister] = useState(false);
  const [loginError, setLoginError] = useState(null);

  const handleRegisterRedirect = () => {
    setRedirectToRegister(true);
  };

  const handleLogin = async (email, password) => {
    try {
      const token = await login(email, password);
      localStorage.setItem('token', token);
      setIsAuthenticated(true);
      setLoginError(null);
    } catch (error) {
      console.error('Login error:', error);
      setLoginError(error.message);
    }
  };

  if (redirectToRegister) {
    return <Navigate to="/register" />;
  }

  if (isAuthenticated) {
    return <RedirectToDashboard />;
  }

  return (
    <div className="start-component">
      <h1>Login</h1>
      <LoginForm handleLogin={handleLogin} loginError={loginError} />
      <button onClick={handleRegisterRedirect}>
        Don’t have an account? Register
      </button>
    </div>
  );
};

const LoginForm = ({ handleLogin, loginError }) => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  const handleSubmit = (event) => {
    event.preventDefault();
    handleLogin(email, password);
  };

  return (
    <form onSubmit={handleSubmit}>
      <div>
        <label>Email:</label>
        <br/>
        <input
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />
      </div>
      <div>
        <label>Password:</label>
        <br/>
        <input
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
      </div>
      {loginError && <p className="error">{loginError}</p>}
      <button type="submit">Login</button>
    </form>
  );
};

const RedirectToDashboard = () => {
  const token = localStorage.getItem('token');
  let jwtRole = '';

  if (token) {
    console.log('Token:', token); // Log the token for debugging
    try {
      const base64Url = token.split('.')[1];
      if (!base64Url) {
        throw new Error('Invalid token format');
      }
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split('')
          .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
          .join('')
      );
      const payload = JSON.parse(jsonPayload);
      jwtRole = payload.role; // Adjust according to your token structure
    } catch (error) {
      console.error('Error parsing JWT:', error);
      return <p>Error: Invalid token format</p>;
    }
  } else {
    console.error('Token not found in local storage');
    return <p>Error: Token not found</p>;
  }

  switch (jwtRole) {
    case 'User':
      return <Navigate to="/user-dashboard" />;
    case 'Driver':
      return <Navigate to="/driver-dashboard" />;
    case 'Admin':
      return <Navigate to="/admin-dashboard" />;
    default:
      console.error('Invalid JWT role:', jwtRole);
      return <p>Error: Invalid JWT role</p>;
  }
};

export default StartComponent;

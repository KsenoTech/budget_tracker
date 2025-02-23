import React, { useState, useContext } from 'react';
import { AuthContext } from '../context/AuthContext';
import { login } from '../api/auth';
import { useNavigate, Link } from 'react-router-dom';

const Login = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState(null);
  const { setToken } = useContext(AuthContext);
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const token = await login(username, password);
      setToken(token);
      localStorage.setItem('token', token);
      navigate('/dashboard');
    } catch (error) {
      setError(error.response?.data?.message || 'Network error, please try again');
      console.error('Login failed:', error);
    }
  };

  return (
    <div>
<form onSubmit={handleSubmit}>
      <input
        type="text"
        value={username}
        onChange={(e) => setUsername(e.target.value)}
        placeholder="Username"
        required
      />
      <input
        type="password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        placeholder="Password"
        required
      />
      {error && <p style={{ color: 'red' }}>{error}</p>}
      <button type="submit">Login</button>

    </form>

<p>
Don’t have an account? <Link to="/register">Register here</Link>
</p>
    </div>
    

  );
};

export default Login;
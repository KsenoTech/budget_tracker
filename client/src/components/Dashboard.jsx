import React, { useContext } from 'react';
import { AuthContext } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';

const Dashboard = () => {
  const { logout } = useContext(AuthContext);
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div>
      <h1>Добро пожаловать в Budget Tracker</h1>
      <p>Здесь будет ваш дашборд с расходами, доходами и лимитами.</p>
      <button onClick={handleLogout}>Выйти</button>
    </div>
  );
};

export default Dashboard;
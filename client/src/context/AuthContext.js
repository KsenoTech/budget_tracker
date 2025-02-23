import React, { createContext, useState, useEffect } from 'react';
import { checkToken } from '../api/auth';

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [token, setToken] = useState(localStorage.getItem('token') || null);
  const [user, setUser] = useState(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const validateToken = async () => {
      if (token) {
        try {
          const userData = await checkToken(token);
          setUser({ id: userData.userId, username: userData.username });
          localStorage.setItem('token', token);
        } catch (error) {
          console.error('Token invalid or expired:', error);
          setToken(null);
          setUser(null);
          localStorage.removeItem('token');
        }
      }
      setIsLoading(false);
    };
    validateToken();
  }, [token]);

  const logout = () => {
    setToken(null);
    setUser(null);
    localStorage.removeItem('token');
  };

  return (
    <AuthContext.Provider value={{ token, setToken, user, logout, isLoading }}>
      {children}
    </AuthContext.Provider>
  );
};
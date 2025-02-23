import React from 'react';
import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import Auth from './components/Auth';
import Dashboard from './components/Dashboard';
import Incomes from './components/Incomes';
import Expenses from './components/Expenses';
import Limits from './components/Limits';
import Categories from './components/Categories';
import PrivateRoute from './components/PrivateRoute';

function App() {
    return (
        <AuthProvider>
            <Router>
                <Routes>
                    <Route path="/auth" element={<Auth />} />
                    <Route path="/dashboard" element={<PrivateRoute><Dashboard /></PrivateRoute>}>
                        <Route path="incomes" element={<Incomes />} />
                        <Route path="expenses" element={<Expenses />} />
                        <Route path="limits" element={<Limits />} />
                        <Route path="categories" element={<Categories />} />
                        <Route path="" element={<Navigate to="/dashboard/incomes" />} />
                    </Route>
                    <Route path="/" element={<Navigate to="/auth" />} />
                </Routes>
            </Router>
        </AuthProvider>
    );
}

export default App;
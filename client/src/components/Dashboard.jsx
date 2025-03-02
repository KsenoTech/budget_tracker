import React, { useContext } from 'react';
import { AppBar, Toolbar, Typography, Drawer, List, ListItem, ListItemIcon, ListItemText, Box, Button, Divider } from '@mui/material';
import { AttachMoney, MoneyOff, Speed, Category, AccountBalanceWallet } from '@mui/icons-material'; // Добавили иконку для приложения
import { AuthContext } from '../context/AuthContext';
import { useNavigate, Outlet } from 'react-router-dom';

const Dashboard = () => {
    const { user, setToken } = useContext(AuthContext); // Получаем user из AuthContext
    const navigate = useNavigate();

    const handleLogout = () => {
        setToken(null);
        localStorage.removeItem('token');
        navigate('/auth');
    };

    const menuItems = [
        { text: 'Доходы', icon: <AttachMoney />, path: '/dashboard/incomes' },
        { text: 'Расходы', icon: <MoneyOff />, path: '/dashboard/expenses' },
        { text: 'Лимиты', icon: <Speed />, path: '/dashboard/limits' },
        { text: 'Статистика', icon: <Category />, path: '/dashboard/generalstatistics' },
    ];

    return (
        <Box sx={{ display: 'flex' }}>
            <AppBar position="fixed">
                <Toolbar>
                    <Typography variant="h6" sx={{ flexGrow: 1, textAlign: 'center' }}>
                        {user?.email || 'Пользователь'} {/* Отображаем email пользователя */}
                    </Typography>
                    <Button color="inherit" onClick={handleLogout}>
                        Выйти
                    </Button>
                </Toolbar>
            </AppBar>
            <Drawer variant="permanent" sx={{ width: 240, flexShrink: 0 }}>
                <Toolbar sx={{ display: 'flex', alignItems: 'center', px: 2 }}>
                    <AccountBalanceWallet sx={{ mr: 1 }} />
                    <Typography variant="h6">
                        Мои финансы
                    </Typography>
                </Toolbar>
                <Divider />
                <List>
                    {menuItems.map((item) => (
                        <ListItem button key={item.text} onClick={() => navigate(item.path)}>
                            <ListItemIcon>{item.icon}</ListItemIcon>
                            <ListItemText primary={item.text} />
                        </ListItem>
                    ))}
                </List>
            </Drawer>
            <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
                <Toolbar />
                <Outlet />
            </Box>
        </Box>
    );
};

export default Dashboard;
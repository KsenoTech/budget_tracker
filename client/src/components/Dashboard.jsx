import React, { useContext } from 'react';
import { AppBar, Toolbar, Typography, Drawer, List, ListItem, ListItemIcon, ListItemText, Box, Button } from '@mui/material';
import { AttachMoney, MoneyOff, Speed, Category } from '@mui/icons-material'; // Иконки для красоты
import { AuthContext } from '../context/AuthContext';
import { useNavigate, Outlet } from 'react-router-dom';

const Dashboard = () => {
    const { setToken } = useContext(AuthContext);
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
        { text: 'Категории трат', icon: <Category />, path: '/dashboard/categories' },
    ];

    return (
        <Box sx={{ display: 'flex' }}>
            <AppBar position="fixed">
                <Toolbar>
                    <Typography variant="h6" sx={{ flexGrow: 1 }}>
                        Мои финансы
                    </Typography>
                    <Button color="inherit" onClick={handleLogout}>
                        Выйти
                    </Button>
                </Toolbar>
            </AppBar>
            <Drawer variant="permanent" sx={{ width: 240, flexShrink: 0 }}>
                <Toolbar /> {/* Пустое пространство под AppBar */}
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
                <Toolbar /> {/* Пустое пространство под AppBar */}
                <Typography variant="h5">Эта страница пока в разработке</Typography>
                <Outlet /> {/* Здесь будут рендериться подмаршруты */}
            </Box>
        </Box>
    );
};

export default Dashboard;
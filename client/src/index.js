import React from 'react';
import { createRoot } from 'react-dom/client'; // Импортируем createRoot
import App from './App';
import { ThemeProvider, createTheme } from '@mui/material/styles';

// Создаем тему
const theme = createTheme({
    palette: {
        primary: { main: '#1976d2' }, // Синий
        secondary: { main: '#dc004e' }, // Красный
    },
});

// Получаем элемент корня и создаем root
const container = document.getElementById('root');
const root = createRoot(container);

// Рендерим приложение с темой
root.render(
    <ThemeProvider theme={theme}>
        <App />
    </ThemeProvider>
);
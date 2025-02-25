import React, { createContext, useState } from 'react';

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
    const [token, setToken] = useState(localStorage.getItem('token') || null);

    return (
        <AuthContext.Provider value={{ token, setToken }}>
            {children}
        </AuthContext.Provider>
    );
};

// import React, { createContext, useState, useEffect } from 'react';
// import axios from 'axios';

// export const AuthContext = createContext();

// export const AuthProvider = ({ children }) => {
//     const [token, setToken] = useState(localStorage.getItem('token') || null);
//     const [user, setUser] = useState(null);

//     // Проверяем токен и загружаем данные пользователя при монтировании или изменении токена
//     useEffect(() => {
//         const validateToken = async () => {
//             if (token) {
//                 try {
//                     const response = await axios.get('https://localhost:7007/api/auth/checkAuth', {
//                         headers: { Authorization: `Bearer ${token}` },
//                     });
//                     // Предполагаем, что сервер возвращает { userId, username }, где username — это email
//                     setUser({ id: response.data.userId, email: response.data.username });
//                 } catch (error) {
//                     console.error('Ошибка проверки токена:', error);
//                     setToken(null);
//                     setUser(null);
//                     localStorage.removeItem('token');
//                 }
//             } else {
//                 setUser(null);
//             }
//         };
//         validateToken();
//     }, [token]);

//     // Обновляем setToken, чтобы сохранять токен в localStorage
//     const updateToken = (newToken) => {
//         setToken(newToken);
//         if (newToken) {
//             localStorage.setItem('token', newToken);
//         } else {
//             localStorage.removeItem('token');
//         }
//     };

//     return (
//         <AuthContext.Provider value={{ token, setToken: updateToken, user }}>
//             {children}
//         </AuthContext.Provider>
//     );
// };
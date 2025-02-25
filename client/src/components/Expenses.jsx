import React, { useState, useEffect, useContext } from "react";
import {
  Typography,
  Box,
  List,
  ListItem,
  ListItemText,
  Divider,
  CircularProgress,
} from "@mui/material";
import axios from "axios";
import { AuthContext } from "../context/AuthContext";

const Expenses = () => {
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const { user } = useContext(AuthContext); // Предполагаем, что email пользователя доступен в контексте

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        setLoading(true);
        const email = user?.email; // Получаем email пользователя из контекста
        if (!email) throw new Error("Email пользователя не найден");

        const token = localStorage.getItem("token");
        
        if (!token) {
          setError("Токен авторизации не найден");
          setLoading(false);
          return;
        }

        const response = await axios.get(
          "https://localhost:7007/api/Expense/getAllForOneUserByEmail",
          {
            params: { email },
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );

        if (!Array.isArray(response.data)) {
          setError("Некорректный формат данных");
          return;
        }
        
        // Очистите данные, удалив лишние поля
        const cleanedCategories = response.data.map((category) => ({
          id: category.id,
          name: category.name,
          createdAt: category.createdAt,
          expenseItems: category.expenseItems || [], // Убедитесь, что expenseItems всегда массив
        }));

        setCategories(cleanedCategories);
      } catch (err) {
        setError(err.response?.data?.message );
      } finally {
        setLoading(false);
      }
    };

    fetchCategories();
  }, [user]);

  if (loading) {
    return (
      <Box sx={{ display: "flex", justifyContent: "center", mt: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Typography variant="h6" color="error" sx={{ mt: 2 }}>
        {error}
      </Typography>
    );
  }

  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h5" gutterBottom>
        Расходы
      </Typography>
      <List>
        {categories.map((category) => (
          <React.Fragment key={category.id}>
            <ListItem>
              <ListItemText
                primary={category.name}
                secondary={`Создано: ${new Date(
                  category.createdAt
                ).toLocaleDateString()}`}
              />
            </ListItem>
            {category.expenseItems.length > 0 && (
              <Box sx={{ pl: 4 }}>
                {/* <Typography variant="subtitle1" color="textSecondary">
                  Элементы расходов:
                </Typography> */}
                <List dense>
                  {category.expenseItems.map((item) => (
                    <ListItem key={item.id}>
                      <ListItemText
                        primary={`${item.name} - ${item.amount} руб.`}
                        secondary={`Дата: ${new Date(
                          item.transactionDate
                        ).toLocaleDateString()}`}
                      />
                    </ListItem>
                  ))}
                </List>
              </Box>
            )}
            <Divider />
          </React.Fragment>
        ))}
      </List>
      {categories.length === 0 && (
        <Typography variant="body1" sx={{ mt: 2 }}>
          Нет категорий расходов для этого пользователя.
        </Typography>
      )}
    </Box>
  );
};

export default Expenses;

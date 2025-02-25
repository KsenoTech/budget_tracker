import React, { useState, useEffect, useContext, useCallback } from "react";
import {
  Typography,
  Box,
  List,
  ListItem,
  ListItemText,
  Divider,
  CircularProgress,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  IconButton,
  Collapse,
} from "@mui/material";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import ExpandLessIcon from "@mui/icons-material/ExpandLess";
import axios from "axios";
import { AuthContext } from "../context/AuthContext";

const Expenses = () => {
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [openAddDialog, setOpenAddDialog] = useState(false);
  const [openEditDialog, setOpenEditDialog] = useState(false);
  const [categoryName, setCategoryName] = useState("");
  const [categoryAmount, setCategoryAmount] = useState(""); // Новое состояние для стоимости
  const [editingCategory, setEditingCategory] = useState(null);
  const [parentId, setParentId] = useState(null); // Для создания подкатегорий
  const [expandedCategories, setExpandedCategories] = useState([]); // Состояние для разворачивания категорий
  const { user } = useContext(AuthContext);

  // Функция для получения категорий
  const fetchCategories = useCallback(async () => {
    try {
      setLoading(true);
      const email = user?.email;
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
          headers: { Authorization: `Bearer ${token}` },
        }
      );

      if (!Array.isArray(response.data)) {
        setError("Некорректный формат данных");
        return;
      }

      const cleanedCategories = response.data.map((category) => ({
        id: category.id,
        name: category.name,
        createdAt: category.createdAt,
        expenseItems: category.expenseItems || [],
        totalAmount: category.expenseItems.reduce(
          (sum, item) => sum + item.amount,
          0
        ), // Общая сумма трат
      }));
      setCategories(cleanedCategories);
    } catch (err) {
      setError(err.response?.data?.message);
    } finally {
      setLoading(false);
    }
  }, [user]);

  useEffect(() => {
    fetchCategories();
  }, [fetchCategories]);

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

  // Создание новой категории или подкатегории
  const handleCreateCategory = async () => {
    if (!categoryName.trim()) return;

    try {
      const token = localStorage.getItem("token");
      const payload = parentId
        ? { Name: categoryName, ParentId: parentId }
        : { Name: categoryName };

      await axios.post(
        "https://localhost:7007/api/Expense/createCategory",
        payload,
        {
          headers: { Authorization: `Bearer ${token}` },
        }
      );

      setOpenAddDialog(false);
      setCategoryName("");
      setParentId(null);
      fetchCategories();
    } catch (error) {
      console.error("Ошибка при создании категории:", error);
    }
  };

  // Обновление категории или подкатегории
  const handleUpdateCategory = async () => {
    if (!categoryName.trim() || !editingCategory) return;

    try {
      const token = localStorage.getItem("token");
      let url = `https://localhost:7007/api/Expense/updateCategory/${editingCategory.id}`;
      let payload = { Name: categoryName };

      // Если это подкатегория, добавляем поле Amount
      if (editingCategory.amount !== undefined) {
        url = `https://localhost:7007/api/Expense/updateExpenseItem/${editingCategory.id}`;
        payload = { Name: categoryName, Amount: parseFloat(categoryAmount) };
      }

      await axios.put(url, payload, {
        headers: { Authorization: `Bearer ${token}` },
      });

      setOpenEditDialog(false);
      setCategoryName("");
      setCategoryAmount(""); // Очищаем поле стоимости
      setEditingCategory(null);
      fetchCategories();
    } catch (error) {
      console.error("Ошибка при обновлении категории:", error);
    }
  };

  // Удаление категории или подкатегории
  const handleDeleteCategory = async (categoryId) => {
    try {
      const token = localStorage.getItem("token");
      await axios.delete(
        `https://localhost:7007/api/Expense/deleteCategory/${categoryId}`,
        {
          headers: { Authorization: `Bearer ${token}` },
        }
      );
      fetchCategories();
    } catch (error) {
      console.error("Ошибка при удалении категории:", error);
    }
  };

  // Обработка разворачивания/сворачивания категории
  const handleToggleExpand = (categoryId) => {
    setExpandedCategories((prevExpanded) =>
      prevExpanded.includes(categoryId)
        ? prevExpanded.filter((id) => id !== categoryId)
        : [...prevExpanded, categoryId]
    );
  };

  return (
    <Box sx={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 2, p: 3 }}>
      <Box
        sx={{
          flex: 1, // Занимает половину пространства
          overflowY: "auto", // Добавляем прокрутку
          maxHeight: "calc(100vh - 80px)", // Ограничиваем высоту для прокрутки
          borderRight: "1px solid #e0e0e0", // Разделительная линия между панелями
          pr: 2, // Отступ справа
        }}
      >
        <Typography variant="h5" gutterBottom>
          Расходы
        </Typography>
        
        {/* Кнопка добавления категории */}
        <Button
          variant="contained"
          color="primary"
          onClick={() => {
            setOpenAddDialog(true);
            setCategoryName("");
            setParentId(null);
          }}
          sx={{ marginTop: 2 }}
        >
          Добавить категорию
        </Button>

        {/* Список категорий */}
        <List>
          {categories.map((category) => (
            <React.Fragment key={category.id}>
              <ListItem
                secondaryAction={
                  <>
                    {/* Кнопка разворачивания/сворачивания */}
                    <IconButton
                      edge="end"
                      aria-label="expand"
                      onClick={() => handleToggleExpand(category.id)}
                    >
                      {expandedCategories.includes(category.id) ? (
                        <ExpandLessIcon />
                      ) : (
                        <ExpandMoreIcon />
                      )}
                    </IconButton>
                    {/* Кнопка редактирования категории */}
                    <IconButton
                      edge="end"
                      aria-label="edit"
                      onClick={() => {
                        setEditingCategory(category);
                        setCategoryName(category.name);
                        setCategoryAmount(""); // Очищаем поле стоимости для категории
                        setOpenEditDialog(true);
                      }}
                    >
                      <EditIcon />
                    </IconButton>
                    {/* Кнопка удаления категории */}
                    <IconButton
                      edge="end"
                      aria-label="delete"
                      onClick={() => handleDeleteCategory(category.id)}
                    >
                      <DeleteIcon color="error" />
                    </IconButton>
                  </>
                }
              >
                <ListItemText
                  primary={`${category.name} - ${category.totalAmount} руб.`}
                />
              </ListItem>

              {/* Подкатегории */}
              <Collapse
                in={expandedCategories.includes(category.id)}
                timeout="auto"
                unmountOnExit
              >
                <Box sx={{ pl: 4 }}>
                  <List dense>
                    {category.expenseItems.map((item) => (
                      <ListItem
                        key={item.id}
                        secondaryAction={
                          <>
                            {/* Кнопка редактирования подкатегории */}
                            <IconButton
                              edge="end"
                              aria-label="edit"
                              onClick={() => {
                                setEditingCategory(item);
                                setCategoryName(item.name);
                                setCategoryAmount(item.amount.toString()); // Заполняем поле стоимости
                                setOpenEditDialog(true);
                              }}
                            >
                              <EditIcon />
                            </IconButton>
                            {/* Кнопка удаления подкатегории */}
                            <IconButton
                              edge="end"
                              aria-label="delete"
                              onClick={() => handleDeleteCategory(item.id)}
                            >
                              <DeleteIcon color="error" />
                            </IconButton>
                          </>
                        }
                      >
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
              </Collapse>
              <Divider />
            </React.Fragment>
          ))}
        </List>

        {/* Диалог добавления категории */}
        <Dialog open={openAddDialog} onClose={() => setOpenAddDialog(false)}>
          <DialogTitle>
            {parentId ? "Добавить подкатегорию" : "Добавить категорию"}
          </DialogTitle>
          <DialogContent>
            <TextField
              autoFocus
              margin="dense"
              id="name"
              label={parentId ? "Название подкатегории" : "Название категории"}
              fullWidth
              value={categoryName}
              onChange={(e) => setCategoryName(e.target.value)}
            />
            {parentId && (
              <TextField
                margin="dense"
                id="amount"
                label="Стоимость"
                type="number"
                fullWidth
                value={categoryAmount}
                onChange={(e) => setCategoryAmount(e.target.value)}
              />
            )}
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setOpenAddDialog(false)}>Отмена</Button>
            <Button onClick={handleCreateCategory}>Создать</Button>
          </DialogActions>
        </Dialog>

        {/* Диалог редактирования категории или подкатегории */}
        <Dialog open={openEditDialog} onClose={() => setOpenEditDialog(false)}>
          <DialogTitle>
            {editingCategory?.amount !== undefined
              ? "Редактировать подкатегорию"
              : "Редактировать категорию"}
          </DialogTitle>
          <DialogContent>
            <TextField
              autoFocus
              margin="dense"
              id="name"
              label="Новое название"
              fullWidth
              value={categoryName}
              onChange={(e) => setCategoryName(e.target.value)}
            />
            {editingCategory?.amount !== undefined && ( // Показываем поле стоимости только для подкатегорий
              <TextField
                margin="dense"
                id="amount"
                label="Стоимость"
                type="number"
                fullWidth
                value={categoryAmount}
                onChange={(e) => setCategoryAmount(e.target.value)}
              />
            )}
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setOpenEditDialog(false)}>Отмена</Button>
            <Button onClick={handleUpdateCategory}>Сохранить</Button>
          </DialogActions>
        </Dialog>

        {categories.length === 0 && (
          <Typography variant="body1" sx={{ mt: 2 }}>
            Нет категорий расходов для этого пользователя.
          </Typography>
        )}
      </Box>
      {/* Правая панель: График трат */}
      <Box
        sx={{
          flex: 1, // Занимает половину пространства
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          height: "100%",
        }}
      >
        <Typography variant="h6">График трат</Typography>
        {/* Здесь можно добавить компонент графика, например, Chart.js или Recharts */}
      </Box>
    </Box>
  );
};

export default Expenses;

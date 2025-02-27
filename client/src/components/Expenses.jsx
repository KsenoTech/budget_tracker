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
  Grid,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  RadioGroup,
  FormControlLabel,
  Radio,
} from "@mui/material";

import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  PieChart,
  Pie,
  Cell,
  Legend,
} from "recharts";

import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import ExpandLessIcon from "@mui/icons-material/ExpandLess";
import AddIcon from "@mui/icons-material/Add";
import axios from "axios";
import { jwtDecode } from "jwt-decode";
import { AuthContext } from "../context/AuthContext";

const Expenses = () => {
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [openAddDialog, setOpenAddDialog] = useState(false);
  const [openEditDialog, setOpenEditDialog] = useState(false);
  const [categoryName, setCategoryName] = useState("");
  const [categoryAmount, setCategoryAmount] = useState("");
  const [editingCategory, setEditingCategory] = useState(null);
  const [parentId, setParentId] = useState(null); // Для создания подкатегорий
  const [expandedCategories, setExpandedCategories] = useState([]);
  const { user } = useContext(AuthContext);

  // Состояние для графиков
  const [selectedCategory, setSelectedCategory] = useState("");
  const [startDate, setStartDate] = useState(
    new Date(new Date().setDate(new Date().getDate() - 7))
      .toISOString()
      .split("T")[0]
  ); // Начало недели назад
  const [endDate, setEndDate] = useState(
    new Date().toISOString().split("T")[0]
  ); // Сегодня
  const [chartData, setChartData] = useState([]);
  const [pieData, setPieData] = useState([]);
  const [totalBudget, setTotalBudget] = useState(0); // Динамический бюджет
  const [chartType, setChartType] = useState("line"); // Тип графика: line или pie

  const COLORS = ["#0088FE", "#00C49F", "#FFBB28", "#FF8042", "#8884D8"];

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
        ),
      }));
      setCategories(cleanedCategories);
    } catch (err) {
      setError(err.response?.data?.message);
    } finally {
      setLoading(false);
    }
  }, [user]);

  // Функция для получения данных для линейного графика
  const fetchChartData = useCallback(async () => {
    if (!selectedCategory) return;

    try {
      const token = localStorage.getItem("token");
      const email = user?.email;
      const response = await axios.get(
        "https://localhost:7007/api/Expense/getAllForOneUserByEmail",
        {
          params: { email },
          headers: { Authorization: `Bearer ${token}` },
        }
      );

      const selectedCat = response.data.find(
        (cat) => cat.name === selectedCategory
      );
      if (selectedCat) {
        const filteredItems = selectedCat.expenseItems.filter((item) => {
          const itemDate = new Date(item.transactionDate);
          return (
            itemDate >= new Date(startDate) && itemDate <= new Date(endDate)
          );
        });
        const data = filteredItems.map((item) => ({
          name: new Date(item.transactionDate).toLocaleDateString(),
          amount: item.amount,
        }));
        setChartData(data);

        // Рассчитываем общий бюджет как сумму трат за период
        const total = filteredItems.reduce((sum, item) => sum + item.amount, 0);
        setTotalBudget(total);
      }
    } catch (error) {
      console.error("Ошибка при загрузке данных для графика:", error);
    }
  }, [selectedCategory, startDate, endDate, user]);

  // Функция для получения данных для круговой диаграммы
  const fetchPieData = useCallback(async () => {
    try {
      const token = localStorage.getItem("token");
      const email = user?.email;
      const response = await axios.get(
        "https://localhost:7007/api/Expense/getAllForOneUserByEmail",
        {
          params: { email },
          headers: { Authorization: `Bearer ${token}` },
        }
      );

      const data = response.data
        .map((cat) => ({
          name: cat.name,
          value: cat.expenseItems
            .filter((item) => {
              const itemDate = new Date(item.transactionDate);
              return (
                itemDate >= new Date(startDate) && itemDate <= new Date(endDate)
              );
            })
            .reduce((sum, item) => sum + item.amount, 0),
        }))
        .filter((d) => d.value > 0); // Убираем категории без трат
      setPieData(data);

      // Общий бюджет для круговой диаграммы
      const total = data.reduce((sum, cat) => sum + cat.value, 0);
      setTotalBudget(total);
    } catch (error) {
      console.error(
        "Ошибка при загрузке данных для круговой диаграммы:",
        error
      );
    }
  }, [startDate, endDate, user]);

  useEffect(() => {
    fetchCategories();
    fetchPieData();
  }, [fetchCategories, fetchPieData]);

  useEffect(() => {
    if (chartType === "line") {
      fetchChartData(); // Обновляем линейный график
    } else {
      fetchPieData(); // Обновляем круговую диаграмму
    }
  }, [fetchChartData, fetchPieData, chartType]);

  // Создание новой категории
  const handleCreateCategory = async () => {
    if (!categoryName.trim()) return;

    try {
      const token = localStorage.getItem("token");
      if (!token) throw new Error("Токен не найден");

      const decodedToken = jwtDecode(token);
      const userId =
        decodedToken[
          "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
        ];

      const payload = {
        Name: categoryName,
        UserId: userId,
        CreatedAt: new Date().toISOString(),
        CategoryLimits: [],
        ExpenseItems: parentId
          ? [
              {
                Name: categoryName,
                Amount: parseFloat(categoryAmount) || 0,
                TransactionDate: new Date().toISOString(),
              },
            ]
          : [],
        ExpenseCategoryId: parentId || null, // Указываем родительскую категорию, если это подкатегория
      };

      const url = parentId
        ? "https://localhost:7007/api/Expense/createExpenseItem"
        : "https://localhost:7007/api/Expense/createCategory";

      await axios.post(url, payload, {
        headers: { Authorization: `Bearer ${token}` },
      });

      setOpenAddDialog(false);
      setCategoryName("");
      setCategoryAmount("");
      setParentId(null);
      fetchCategories();
    } catch (error) {
      console.error("Ошибка при создании категории:", error);
    }
  };

  // Создание нового элемента расхода (подкатегории)
  const handleCreateExpenseItem = async () => {
    if (!categoryName.trim() || !parentId) return;

    try {
      const token = localStorage.getItem("token");
      if (!token) throw new Error("Токен не найден");

      const decodedToken = jwtDecode(token);
      const userId =
        decodedToken[
          "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
        ];

      const payload = {
        name: categoryName,
        amount: parseFloat(categoryAmount) || 0,
        transactionDate: new Date().toISOString(),
        categoryName: categories.find((c) => c.id === parentId)?.name || "",
      };

      await axios.post(
        `https://localhost:7007/api/Expense/createExpenseItem?userId=${userId}`,
        payload,
        {
          headers: { Authorization: `Bearer ${token}` },
        }
      );

      setOpenAddDialog(false);
      setCategoryName("");
      setCategoryAmount("");
      setParentId(null);
      fetchCategories();
    } catch (error) {
      console.error("Ошибка при создании элемента расхода:", error);
    }
  };

  // Обновление категории или подкатегории
  const handleUpdateCategory = async (isSubcategory = false) => {
    if (!categoryName.trim() || !editingCategory) return;

    try {
      const token = localStorage.getItem("token");
      if (!token) throw new Error("Токен не найден");

      const url = isSubcategory
        ? `https://localhost:7007/api/Expense/updateItem/${editingCategory.id}`
        : `https://localhost:7007/api/Expense/updateCategory/${editingCategory.id}`;

      const payload = isSubcategory
        ? { name: categoryName, amount: parseFloat(categoryAmount) || 0 }
        : { name: categoryName }; // Только имя для категории

      await axios.put(url, payload, {
        headers: { Authorization: `Bearer ${token}` },
      });

      setOpenEditDialog(false);
      setCategoryName("");
      setCategoryAmount("");
      setEditingCategory(null);
      fetchCategories();
    } catch (error) {
      console.error("Ошибка при обновлении:", error);
    }
  };

  // Удаление категории или подкатегории
  const handleDeleteCategory = async (id, isSubcategory = false) => {
    try {
      const token = localStorage.getItem("token");
      if (!token) throw new Error("Токен не найден");

      const url = isSubcategory
        ? `https://localhost:7007/api/Expense/deleteItem/${id}`
        : `https://localhost:7007/api/Expense/deleteCategory/${id}`;

      await axios.delete(url, {
        headers: { Authorization: `Bearer ${token}` },
      });
      fetchCategories();
    } catch (error) {
      console.error("Ошибка при удалении:", error);
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
    <Box sx={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 2, p: 3 }}>
      {/* calc(100vh - 80px) */}
      <Box
        sx={{
          flex: 1,
          overflowY: "auto",
          maxHeight: "75vh",
          borderRight: "1px solid #e0e0e0",
          pr: 2,
        }}
      >
        <Typography variant="h5" gutterBottom>
          Расходы
        </Typography>

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

        <List>
          {categories.map((category) => (
            <React.Fragment key={category.id}>
              <ListItem
                secondaryAction={
                  <>
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
                    <IconButton
                      edge="end"
                      aria-label="edit"
                      onClick={() => {
                        setEditingCategory(category);
                        setCategoryName(category.name);
                        setCategoryAmount("");
                        setOpenEditDialog(true);
                      }}
                    >
                      <EditIcon />
                    </IconButton>
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
                  secondary={`Создано: ${new Date(
                    category.createdAt
                  ).toLocaleDateString()}`}
                />
              </ListItem>

              <Collapse
                in={expandedCategories.includes(category.id)}
                timeout="auto"
                unmountOnExit
              >
                <Box sx={{ pl: 4 }}>
                  {/* Кнопка добавления подкатегории */}
                  <Button
                    variant="outlined"
                    size="small"
                    startIcon={<AddIcon />}
                    onClick={() => {
                      setOpenAddDialog(true);
                      setCategoryName("");
                      setCategoryAmount("");
                      setParentId(category.id);
                    }}
                    sx={{ mb: 1 }}
                  >
                    Добавить подкатегорию
                  </Button>

                  <List dense>
                    {category.expenseItems.map((item) => (
                      <ListItem
                        key={item.id}
                        secondaryAction={
                          <>
                            <IconButton
                              edge="end"
                              aria-label="edit"
                              onClick={() => {
                                setEditingCategory(item);
                                setCategoryName(item.name);
                                setCategoryAmount(item.amount.toString());
                                setOpenEditDialog(true);
                              }}
                            >
                              <EditIcon />
                            </IconButton>
                            <IconButton
                              edge="end"
                              aria-label="delete"
                              onClick={() =>
                                handleDeleteCategory(item.id, true)
                              }
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
            <Button
              onClick={
                parentId ? handleCreateExpenseItem : handleCreateCategory
              }
            >
              Создать
            </Button>
          </DialogActions>
        </Dialog>

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
            {editingCategory?.amount !== undefined && (
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
            <Button
              onClick={() =>
                handleUpdateCategory(editingCategory?.amount !== undefined)
              }
            >
              Сохранить
            </Button>
          </DialogActions>
        </Dialog>

        {categories.length === 0 && (
          <Typography variant="body1" sx={{ mt: 2 }}>
            Нет категорий расходов для этого пользователя.
          </Typography>
        )}
      </Box>
      <Box sx={{ flex: 1, height: "100%", p: 2 }}>
        <Typography variant="h6" gutterBottom align="center">
          Графики трат
        </Typography>
        <Grid container spacing={2}>
          <Grid item xs={12}>
            <Box>
              <RadioGroup
                row
                value={chartType}
                onChange={(e) => setChartType(e.target.value)}
                sx={{ mb: 2, justifyContent: "center" }}
              >
                <FormControlLabel
                  value="line"
                  control={<Radio />}
                  label="Линейный график"
                />
                <FormControlLabel
                  value="pie"
                  control={<Radio />}
                  label="Круговая диаграмма"
                />
              </RadioGroup>

              {chartType === "line" && (
                <>
                  <FormControl fullWidth sx={{ mb: 2 }}>
                    <InputLabel>Категория</InputLabel>
                    <Select
                      value={selectedCategory}
                      onChange={(e) => setSelectedCategory(e.target.value)}
                      label="Категория"
                    >
                      <MenuItem value="">Выберите категорию</MenuItem>
                      {categories.map((cat) => (
                        <MenuItem key={cat.id} value={cat.name}>
                          {cat.name}
                        </MenuItem>
                      ))}
                    </Select>
                  </FormControl>
                  <Box display="flex" gap={2} mb={2}>
                    <TextField
                      type="date"
                      label="Начало периода"
                      value={startDate}
                      onChange={(e) => setStartDate(e.target.value)}
                      InputLabelProps={{ shrink: true }}
                      fullWidth
                    />
                    <TextField
                      type="date"
                      label="Конец периода"
                      value={endDate}
                      onChange={(e) => setEndDate(e.target.value)}
                      InputLabelProps={{ shrink: true }}
                      fullWidth
                    />
                  </Box>
                  <LineChart width={500} height={300} data={chartData}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="name" />
                    <YAxis />
                    <Tooltip formatter={(value) => `${value} руб.`} />
                    <Line type="monotone" dataKey="amount" stroke="#8884d8" />
                  </LineChart>
                </>
              )}

              {chartType === "pie" && (
                <PieChart width={500} height={300}>
                  <Pie
                    data={pieData}
                    cx="50%"
                    cy="50%"
                    outerRadius={80}
                    fill="#8884d8"
                    dataKey="value"
                    label={({ name, percent }) =>
                      `${name} (${(percent * 100).toFixed(0)}%)`
                    }
                  >
                    {pieData.map((entry, index) => (
                      <Cell
                        key={`cell-${index}`}
                        fill={COLORS[index % COLORS.length]}
                      />
                    ))}
                  </Pie>
                  <Tooltip formatter={(value) => `${value} руб.`} />
                  <Legend />
                </PieChart>
              )}
              <Typography variant="body2" align="center" sx={{ mt: 2 }}>
                Общий бюджет за период: {totalBudget} руб.
              </Typography>
            </Box>
          </Grid>
        </Grid>
      </Box>
    </Box>
  );
};

export default Expenses;

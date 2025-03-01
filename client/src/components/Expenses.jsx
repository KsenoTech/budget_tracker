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
} from "@mui/material";
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend } from "recharts";
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
  const [parentId, setParentId] = useState(null);
  const [expandedCategories, setExpandedCategories] = useState([]);
  const { user } = useContext(AuthContext);

  // Состояние для графика
  const [selectedCategory, setSelectedCategory] = useState("");

  const [startDate, setStartDate] = useState(
    new Date(new Date().getFullYear(), new Date().getMonth()+1, -29).toISOString().split("T")[0]
  );

  const [endDate, setEndDate] = useState(
    new Date(new Date().getFullYear(), new Date().getMonth() + 1, 2).toISOString().split("T")[0]
  ); // Сегодня
  const [chartData, setChartData] = useState([]);
  const [totalBudget, setTotalBudget] = useState(0);

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
        totalAmount: category.expenseItems.reduce((sum, item) => sum + item.amount, 0),
      }));
      setCategories(cleanedCategories);
    } catch (err) {
      setError(err.response?.data?.message);
    } finally {
      setLoading(false);
    }
  }, [user]);

  const fetchChartData = useCallback(async () => {
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

      if (!Array.isArray(response.data)) return;

      const filteredCategories = selectedCategory
        ? response.data.filter((cat) => cat.name === selectedCategory)
        : response.data;

      // Создаём объект для объединения данных по дням
      const dataByDate = {};

      filteredCategories.forEach((cat, index) => {
        const filteredItems = cat.expenseItems.filter((item) => {
          const itemDate = new Date(item.transactionDate);
          return itemDate >= new Date(startDate) && itemDate <= new Date(endDate);
        });

        filteredItems.forEach((item) => {
          const date = new Date(item.transactionDate).toLocaleDateString();
          if (!dataByDate[date]) {
            dataByDate[date] = { name: date };
            filteredCategories.forEach((c) => {
              dataByDate[date][c.name] = 0; // Инициализируем все категории нулем
            });
          }
          dataByDate[date][cat.name] += item.amount;
        });
      });

      // Сортируем данные по датам в хронологическом порядке
      const chartDataFormatted = Object.values(dataByDate).sort((a, b) => {
        const dateA = new Date(a.name.split(".").reverse().join("-"));
        const dateB = new Date(b.name.split(".").reverse().join("-"));
        return dateA - dateB;
      });
      setChartData(chartDataFormatted);
      
      const total = filteredCategories.reduce(
        (sum, cat) =>
          sum +
          cat.expenseItems
            .filter((item) => {
              const itemDate = new Date(item.transactionDate);
              return itemDate >= new Date(startDate) && itemDate <= new Date(endDate);
            })
            .reduce((sum, item) => sum + item.amount, 0),
        0
      );
      setTotalBudget(total);
    } catch (error) {
      console.error("Ошибка при загрузке данных для графика:", error);
    }
  }, [selectedCategory, startDate, endDate, user]);

  useEffect(() => {
    fetchCategories();
  }, [fetchCategories]);

  useEffect(() => {
    fetchChartData();
  }, [fetchChartData]);

  const handleCreateCategory = async () => {
    if (!categoryName.trim()) return;

    try {
      const token = localStorage.getItem("token");
      if (!token) throw new Error("Токен не найден");

      const decodedToken = jwtDecode(token);
      const userId = decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];

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
        ExpenseCategoryId: parentId || null,
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

  const handleCreateExpenseItem = async () => {
    if (!categoryName.trim() || !parentId) return;

    try {
      const token = localStorage.getItem("token");
      if (!token) throw new Error("Токен не найден");

      const decodedToken = jwtDecode(token);
      const userId = decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];

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
        : { name: categoryName };

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
                  secondary={`Создано: ${new Date(category.createdAt).toLocaleDateString()}`}
                />
              </ListItem>

              <Collapse
                in={expandedCategories.includes(category.id)}
                timeout="auto"
                unmountOnExit
              >
                <Box sx={{ pl: 4 }}>
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
                              onClick={() => handleDeleteCategory(item.id, true)}
                            >
                              <DeleteIcon color="error" />
                            </IconButton>
                          </>
                        }
                      >
                        <ListItemText
                          primary={`${item.name} - ${item.amount} руб.`}
                          secondary={`Дата: ${new Date(item.transactionDate).toLocaleDateString()}`}
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
          <DialogTitle>{parentId ? "Добавить подкатегорию" : "Добавить категорию"}</DialogTitle>
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
            <Button onClick={parentId ? handleCreateExpenseItem : handleCreateCategory}>
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
              onClick={() => handleUpdateCategory(editingCategory?.amount !== undefined)}
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
          График трат
        </Typography>
        <Grid container spacing={2}>
          <Grid item xs={12}>
            <Box>
              <FormControl fullWidth sx={{ mb: 2 }}>
                <InputLabel>Категория</InputLabel>
                <Select
                  value={selectedCategory}
                  onChange={(e) => setSelectedCategory(e.target.value)}
                  label="Категория"
                >
                  <MenuItem value="">Все категории</MenuItem>
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
                <Legend />
                {selectedCategory
                  ? <Line type="monotone" dataKey={selectedCategory} stroke="#8884d8" name={selectedCategory} />
                  : categories.map((cat, index) => (
                      <Line
                        key={cat.name}
                        type="monotone"
                        dataKey={cat.name}
                        stroke={COLORS[index % COLORS.length]}
                        name={cat.name}
                      />
                    ))}
              </LineChart>
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
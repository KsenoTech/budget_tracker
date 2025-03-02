import React, { useState, useEffect, useContext } from "react";
import {
  Box,
  Typography,
  CircularProgress,
  List,
  ListItem,
  ListItemText,
  Divider,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  IconButton,
} from "@mui/material";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import axios from "axios";
import { AuthContext } from "../context/AuthContext";

const Limits = () => {
  const [categories, setCategories] = useState([]);
  const [limits, setLimits] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [openAddDialog, setOpenAddDialog] = useState(false);
  const [openEditDialog, setOpenEditDialog] = useState(false);
  const [selectedCategoryId, setSelectedCategoryId] = useState("");
  const [limitAmount, setLimitAmount] = useState("");
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [editingLimit, setEditingLimit] = useState(null);
  const { user } = useContext(AuthContext);
  

  const fetchData = async () => {
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

      const categoriesResponse = await axios.get(
        "https://localhost:7007/api/Expense/getAllForOneUserByEmail",
        {
          params: { email },
          headers: { Authorization: `Bearer ${token}` },
        }
      );

      if (!Array.isArray(categoriesResponse.data)) {
        setError("Некорректный формат данных категорий");
        setLoading(false);
        return;
      }

      setCategories(categoriesResponse.data);

      const limitsResponse = await axios.get(
        "https://localhost:7007/api/Expense/getCategoryLimits",
        {
          headers: { Authorization: `Bearer ${token}` },
        }
      );

      if (!Array.isArray(limitsResponse.data)) {
        setError("Некорректный формат данных лимитов");
        setLoading(false);
        return;
      }

      setLimits(limitsResponse.data);
    } catch (err) {
      setError(err.response?.data?.message || "Ошибка загрузки данных");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const handleCreateLimit = async () => {
    if (!selectedCategoryId || !limitAmount || !startDate || !endDate) {
      setError("Все поля должны быть заполнены");
      return;
    }

    try {
      const token = localStorage.getItem("token");
      if (!token) throw new Error("Токен не найден");

      const payload = {
        categoryId: selectedCategoryId,
        limitAmount: parseFloat(limitAmount),
        startDate: new Date(startDate).toISOString(),
        endDate: new Date(endDate).toISOString(),
      };

      await axios.post(
        "https://localhost:7007/api/Expense/createCategoryLimit",
        payload,
        {
          headers: { Authorization: `Bearer ${token}` },
        }
      );

      setOpenAddDialog(false);
      setSelectedCategoryId("");
      setLimitAmount("");
      setStartDate("");
      setEndDate("");
      fetchData();
    } catch (error) {
      setError(error.response?.data?.message || "Ошибка при создании лимита");
      console.error("Ошибка при создании лимита:", error);
    }
  };

  const handleUpdateLimit = async () => {
    if (!selectedCategoryId || !limitAmount || !startDate || !endDate || !editingLimit) {
      setError("Все поля должны быть заполнены");
      return;
    }

    try {
      const token = localStorage.getItem("token");
      if (!token) throw new Error("Токен не найден");

      const payload = {
        categoryId: selectedCategoryId,
        limitAmount: parseFloat(limitAmount),
        startDate: new Date(startDate).toISOString(),
        endDate: new Date(endDate).toISOString(),
      };

      await axios.put(
        `https://localhost:7007/api/Expense/updateCategoryLimit/${editingLimit.id}`,
        payload,
        {
          headers: { Authorization: `Bearer ${token}` },
        }
      );

      setOpenEditDialog(false);
      setEditingLimit(null);
      setSelectedCategoryId("");
      setLimitAmount("");
      setStartDate("");
      setEndDate("");
      fetchData();
    } catch (error) {
      setError(error.response?.data?.message || "Ошибка при обновлении лимита");
      console.error("Ошибка при обновлении лимита:", error);
    }
  };

  const handleDeleteLimit = async (limitId) => {
    try {
      const token = localStorage.getItem("token");
      if (!token) throw new Error("Токен не найден");

      await axios.delete(
        `https://localhost:7007/api/Expense/deleteCategoryLimit/${limitId}`,
        {
          headers: { Authorization: `Bearer ${token}` },
        }
      );

      fetchData();
    } catch (error) {
      setError(error.response?.data?.message || "Ошибка при удалении лимита");
      console.error("Ошибка при удалении лимита:", error);
    }
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
    <Box sx={{ p: 3 }}>
      <Typography variant="h5" gutterBottom>
        Лимиты на категории расходов
      </Typography>

      <Button
        variant="contained"
        color="primary"
        onClick={() => {
          setOpenAddDialog(true);
          setSelectedCategoryId("");
          setLimitAmount("");
          setStartDate("");
          setEndDate("");
        }}
        sx={{ mb: 2 }}
      >
        Добавить лимит
      </Button>

      <List>
        {limits.length === 0 ? (
          <Typography variant="body1">
            Лимиты на категории отсутствуют.
          </Typography>
        ) : (
          limits.map((limit) => {
            const category = categories.find(
              (cat) => cat.id === limit.expenseCategoryId
            );
            return (
              <React.Fragment key={limit.id}>
                <ListItem
                  secondaryAction={
                    <>
                      <IconButton
                        edge="end"
                        aria-label="edit"
                        onClick={() => {
                          setEditingLimit(limit);
                          setSelectedCategoryId(limit.expenseCategoryId);
                          setLimitAmount(limit.limitAmount);
                          setStartDate(new Date(limit.startDate).toISOString().split("T")[0]);
                          setEndDate(new Date(limit.endDate).toISOString().split("T")[0]);
                          setOpenEditDialog(true);
                        }}
                      >
                        <EditIcon />
                      </IconButton>
                      <IconButton
                        edge="end"
                        aria-label="delete"
                        onClick={() => handleDeleteLimit(limit.id)}
                      >
                        <DeleteIcon color="error" />
                      </IconButton>
                    </>
                  }
                >
                  <ListItemText
                    primary={`${category?.name || "Неизвестная категория"}: ${limit.limitAmount} руб.`}
                    secondary={`Период: ${new Date(limit.startDate).toLocaleDateString()} - ${new Date(limit.endDate).toLocaleDateString()}`}
                  />
                </ListItem>
                <Divider />
              </React.Fragment>
            );
          })
        )}
      </List>

      {/* Диалог добавления лимита */}
      <Dialog open={openAddDialog} onClose={() => setOpenAddDialog(false)}>
        <DialogTitle>Добавить лимит на категорию</DialogTitle>
        <DialogContent>
          <FormControl fullWidth sx={{ mt: 2 }}>
            <InputLabel>Категория</InputLabel>
            <Select
              value={selectedCategoryId}
              onChange={(e) => setSelectedCategoryId(e.target.value)}
              label="Категория"
            >
              <MenuItem value="">Выберите категорию</MenuItem>
              {categories.map((cat) => (
                <MenuItem key={cat.id} value={cat.id}>
                  {cat.name}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
          <TextField
            margin="dense"
            label="Сумма лимита"
            type="number"
            fullWidth
            value={limitAmount}
            onChange={(e) => setLimitAmount(e.target.value)}
          />
          <TextField
            margin="dense"
            label="Начало периода"
            type="date"
            fullWidth
            value={startDate}
            onChange={(e) => setStartDate(e.target.value)}
            InputLabelProps={{ shrink: true }}
          />
          <TextField
            margin="dense"
            label="Конец периода"
            type="date"
            fullWidth
            value={endDate}
            onChange={(e) => setEndDate(e.target.value)}
            InputLabelProps={{ shrink: true }}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenAddDialog(false)}>Отмена</Button>
          <Button onClick={handleCreateLimit}>Создать</Button>
        </DialogActions>
      </Dialog>

      {/* Диалог редактирования лимита */}
      <Dialog open={openEditDialog} onClose={() => setOpenEditDialog(false)}>
        <DialogTitle>Редактировать лимит</DialogTitle>
        <DialogContent>
          <FormControl fullWidth sx={{ mt: 2 }}>
            <InputLabel>Категория</InputLabel>
            <Select
              value={selectedCategoryId}
              onChange={(e) => setSelectedCategoryId(e.target.value)}
              label="Категория"
            >
              <MenuItem value="">Выберите категорию</MenuItem>
              {categories.map((cat) => (
                <MenuItem key={cat.id} value={cat.id}>
                  {cat.name}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
          <TextField
            margin="dense"
            label="Сумма лимита"
            type="number"
            fullWidth
            value={limitAmount}
            onChange={(e) => setLimitAmount(e.target.value)}
          />
          <TextField
            margin="dense"
            label="Начало периода"
            type="date"
            fullWidth
            value={startDate}
            onChange={(e) => setStartDate(e.target.value)}
            InputLabelProps={{ shrink: true }}
          />
          <TextField
            margin="dense"
            label="Конец периода"
            type="date"
            fullWidth
            value={endDate}
            onChange={(e) => setEndDate(e.target.value)}
            InputLabelProps={{ shrink: true }}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenEditDialog(false)}>Отмена</Button>
          <Button onClick={handleUpdateLimit}>Сохранить</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default Limits;
import React, { useState, useEffect, useCallback } from "react";
import {
  Box,
  Typography,
  CircularProgress,
  Grid,
  TextField,
  Paper,
  List,
  ListItem,
  ListItemText,
} from "@mui/material";
import { PieChart, Pie, Cell, Tooltip } from "recharts"; // Убрали Legend
import axios from "axios";

const GeneralStatistics = () => {
  const [incomeData, setIncomeData] = useState([]);
  const [expenseData, setExpenseData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  
  const [startDate, setStartDate] = useState(
    new Date(new Date().getFullYear(), new Date().getMonth()+1, -29).toISOString().split("T")[0]
  );
  const [endDate, setEndDate] = useState(
    new Date(new Date().getFullYear(), new Date().getMonth() + 1, 2).toISOString().split("T")[0]
  );

  const COLORS = ["#0088FE", "#00C49F", "#FFBB28", "#FF8042", "#8884D8", "#82ca9d"];

  const fetchStatistics = useCallback(async () => {
    try {
      setLoading(true);
      const token = localStorage.getItem("token");
      if (!token) throw new Error("Токен не найден");

      const response = await axios.get(
        "https://localhost:7007/api/Statistics/getMonthlyStatistics",
        {
          params: { startDate, endDate },
          headers: { Authorization: `Bearer ${token}` },
        }
      );

      const { incomes, expenses } = response.data;
      setIncomeData(
        incomes.map((cat) => ({
          name: cat.name,
          value: cat.totalAmount,
        }))
      );
      setExpenseData(
        expenses.map((cat) => ({
          name: cat.name,
          value: cat.totalAmount,
        }))
      );
    } catch (err) {
      setError(err.response?.data?.message || "Ошибка загрузки статистики");
    } finally {
      setLoading(false);
    }
  }, [startDate, endDate]);

  useEffect(() => {
    fetchStatistics();
  }, [fetchStatistics]); // Добавили fetchStatistics в зависимости

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
      <Typography variant="h5" align="center" gutterBottom>
        Общая статистика за месяц
      </Typography>
      <Box display="flex" justifyContent="center" gap={2} mb={4}>
        <TextField
          type="date"
          label="Начало периода"
          value={startDate}
          onChange={(e) => setStartDate(e.target.value)}
          InputLabelProps={{ shrink: true }}
        />
        <TextField
          type="date"
          label="Конец периода"
          value={endDate}
          onChange={(e) => setEndDate(e.target.value)}
          InputLabelProps={{ shrink: true }}
        />
      </Box>
      <Grid container spacing={4}>
        {/* Доходы (слева) */}
        <Grid item xs={6}>
          <Paper elevation={3} sx={{ p: 2 }}>
            <Typography variant="h6" align="center" gutterBottom>
              Доходы
            </Typography>
            <PieChart width={600} height={200}>
              <Pie
                data={incomeData}
                cx="45%"
                cy="50%"
                outerRadius={80}
                fill="#8884d8"
                dataKey="value"
                label={({ name, percent }) => `${name} (${(percent * 100).toFixed(0)}%)`}
              >
                {incomeData.map((entry, index) => (
                  <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                ))}
              </Pie>
              <Tooltip formatter={(value) => `${value} руб.`} />
            </PieChart>
            <List dense>
              {incomeData.map((cat) => (
                <ListItem key={cat.name}>
                  <ListItemText primary={`${cat.name}: ${cat.value} руб.`} />
                </ListItem>
              ))}
            </List>
          </Paper>
        </Grid>

        {/* Расходы (справа) */}
        <Grid item xs={6}>
          <Paper elevation={3} sx={{ p: 2 }}>
            <Typography variant="h6" align="center" gutterBottom>
              Расходы
            </Typography>
            <PieChart width={600} height={200}>
              <Pie
                data={expenseData}
                cx="45%"
                cy="50%"
                outerRadius={80}
                fill="#8884d8"
                dataKey="value"
                label={({ name, percent }) => `${name} (${(percent * 100).toFixed(0)}%)`}
              >
                {expenseData.map((entry, index) => (
                  <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                ))}
              </Pie>
              <Tooltip formatter={(value) => `${value} руб.`} />
            </PieChart>
            <List dense>
              {expenseData.map((cat) => (
                <ListItem key={cat.name}>
                  <ListItemText primary={`${cat.name}: ${cat.value} руб.`} />
                </ListItem>
              ))}
            </List>
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
};

export default GeneralStatistics;
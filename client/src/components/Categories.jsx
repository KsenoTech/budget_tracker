import React, { useState, useEffect } from 'react';
import {
  Box,
  Typography,
  Button,
  TextField,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  List,
  ListItem,
  ListItemText,
  IconButton,
} from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import axios from 'axios';

const Categories = () => {
  const [categories, setCategories] = useState([]);
  const [openAddDialog, setOpenAddDialog] = useState(false);
  const [openEditDialog, setOpenEditDialog] = useState(false);
  const [categoryName, setCategoryName] = useState('');
  const [editingCategory, setEditingCategory] = useState(null);

  // Загрузка категорий при монтировании компонента
  useEffect(() => {
    fetchCategories();
  }, []);

  // Функция для получения категорий с сервера
  const fetchCategories = async () => {
    try {
      const response = await axios.get('http://localhost:7007/api/IncomeCategory'); // Измените URL, если необходимо
      setCategories(response.data);
    } catch (error) {
      console.error('Error fetching categories:', error);
    }
  };

  // Функция для создания новой категории
  const handleCreateCategory = async () => {
    if (!categoryName.trim()) return;

    try {
      await axios.post('http://localhost:7007/api/IncomeCategory/getAll', {
        Name: categoryName,
      });
      setOpenAddDialog(false);
      setCategoryName('');
      fetchCategories(); // Обновляем список категорий
    } catch (error) {
      console.error('Error creating category:', error);
    }
  };

  // Функция для обновления категории
  const handleUpdateCategory = async () => {
    if (!categoryName.trim() || !editingCategory) return;

    try {
      await axios.put(`http://localhost:7007/api/IncomeCategory/update`, {
        Id: editingCategory.Id,
        Name: categoryName,
      });
      setOpenEditDialog(false);
      setCategoryName('');
      setEditingCategory(null);
      fetchCategories(); // Обновляем список категорий
    } catch (error) {
      console.error('Error updating category:', error);
    }
  };

  // Функция для удаления категории
  const handleDeleteCategory = async (categoryId) => {
    try {
      await axios.delete(`http://localhost:7007/api/IncomeCategory/delete${categoryId}`);
      fetchCategories(); // Обновляем список категорий
    } catch (error) {
      console.error('Error deleting category:', error);
    }
  };

  return (
    <Box sx={{ padding: 2 }}>
      <Typography variant="h5" gutterBottom>
        Категории расходов
      </Typography>

      {/* Список категорий */}
      <List>
        {categories.map((category) => (
          <ListItem key={category.Id} secondaryAction={
            <>
              {/* Кнопка редактирования */}
              <IconButton
                edge="end"
                aria-label="edit"
                onClick={() => {
                  setEditingCategory(category);
                  setCategoryName(category.Name);
                  setOpenEditDialog(true);
                }}
              >
                <EditIcon />
              </IconButton>

              {/* Кнопка удаления */}
              <IconButton
                edge="end"
                aria-label="delete"
                onClick={() => handleDeleteCategory(category.Id)}
              >
                <DeleteIcon color="error" />
              </IconButton>
            </>
          }>
            <ListItemText primary={category.Name} />
          </ListItem>
        ))}
      </List>

      {/* Диалог добавления категории */}
      <Dialog open={openAddDialog} onClose={() => setOpenAddDialog(false)}>
        <DialogTitle>Добавить категорию</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            id="name"
            label="Название категории"
            fullWidth
            value={categoryName}
            onChange={(e) => setCategoryName(e.target.value)}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenAddDialog(false)}>Отмена</Button>
          <Button onClick={handleCreateCategory}>Создать</Button>
        </DialogActions>
      </Dialog>

      {/* Диалог редактирования категории */}
      <Dialog open={openEditDialog} onClose={() => setOpenEditDialog(false)}>
        <DialogTitle>Редактировать категорию</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            id="name"
            label="Новое название категории"
            fullWidth
            value={categoryName}
            onChange={(e) => setCategoryName(e.target.value)}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenEditDialog(false)}>Отмена</Button>
          <Button onClick={handleUpdateCategory}>Сохранить</Button>
        </DialogActions>
      </Dialog>

      {/* Кнопка добавления категории */}
      <Button
        variant="contained"
        color="primary"
        onClick={() => {
          setOpenAddDialog(true);
          setCategoryName('');
        }}
        sx={{ marginTop: 2 }}
      >
        Добавить категорию
      </Button>
    </Box>
  );
};

export default Categories;
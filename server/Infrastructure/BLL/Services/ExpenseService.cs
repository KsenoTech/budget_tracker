using server.ApplicationCore.DomModels;
using server.ApplicationCore.Interfaces.Repositories;
using server.ApplicationCore.Interfaces.Services;

namespace server.Infrastructure.BLL.Services
{
    public class ExpenseService : IExpenseService
    {
        private IDbRepository _dbcontext;
        private readonly ILogger<ExpenseService> _logger;

        public ExpenseService(IDbRepository dbcontext, ILogger<ExpenseService> logger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
        }

        public async Task<bool> CreateCategoryAsync(ExpenseCategory category)
        {
            // Используем методы из IDbRepository для работы с IncomeCategory
            var result = await _dbcontext.ExpenseCategories.CreateAsync(category);
            if (result)
            {
                await _dbcontext.SaveAsync(); // Сохраняем изменения в базе данных
            }
            return result;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            // Удаляем категорию по ID через репозиторий
            var category = await _dbcontext.ExpenseCategories.GetByIdAsync(id);
            if (category == null) return false;

            var result = await _dbcontext.ExpenseCategories.DeleteAsync(category.Id);
            if (result)
            {
                await _dbcontext.SaveAsync(); // Сохраняем изменения в базе данных
            }
            return result;
        }
        public async Task<bool> DeleteItemAsync(int id)
        {
            // Удаляем подкатегорию по ID через репозиторий
            var item = await _dbcontext.ExpenseItems.GetByIdAsync(id);
            if (item == null) return false;

            var result = await _dbcontext.ExpenseItems.DeleteAsync(item.Id);
            if (result)
            {
                await _dbcontext.SaveAsync(); // Сохраняем изменения в базе данных
            }
            return result;
        }

        public async Task<List<ExpenseCategory>> GetCategoriesByEmailAsync(string email)
        {
            // Получаем категории по UserId
            return await _dbcontext.ExpenseCategories.GetByUserEmailAsync(email);
        }

        public async Task<bool> UpdateCategoryAsync(int id, string name)
        {
            try
            {
                var existingCategory = await _dbcontext.ExpenseCategories.GetByIdAsync(id);
                if (existingCategory == null)
                {
                    _logger.LogWarning("Category with ID {Id} not found", id);
                    return false;
                }

                existingCategory.Name = name;

                var result = await _dbcontext.ExpenseCategories.UpdateAsync(existingCategory);
                if (result)
                {
                    await _dbcontext.SaveAsync();
                    _logger.LogInformation("Category with ID {Id} successfully updated", id);
                    return true;
                }

                _logger.LogWarning("Failed to update category with ID {Id}", id);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category with ID {Id}", id);
                return false;
            }
        }

        public async Task<bool> UpdateItemAsync(ExpenseItem category)
        {
            // Обновляем существующую категорию
            var existingItem = await _dbcontext.ExpenseItems.GetByIdAsync(category.Id);
            if (existingItem == null) return false;

            existingItem.Name = category.Name; // Обновляем необходимые поля
            existingItem.Amount = category.Amount;

            var result = await _dbcontext.ExpenseItems.UpdateAsync(existingItem);
            if (result)
            {
                await _dbcontext.SaveAsync(); // Сохраняем изменения в базе данных
            }
            return result;
        }

        public async Task<bool> CreateExpenseItemAsync(ExpenseItem expenseItem, string userId, string categoryName)
        {
            try
            {
                // Получаем все категории пользователя по UserId
                var userCategories = await _dbcontext.ExpenseCategories.GetByUserIdAsync(userId);
                if (!userCategories.Any())
                {
                    _logger.LogWarning("Категории для пользователя с ID {UserId} не найдены", userId);
                    return false;
                }

                // Ищем категорию по имени
                var category = userCategories.FirstOrDefault(c => c.Name == categoryName);
                if (category == null)
                {
                    _logger.LogWarning("Категория с именем {CategoryName} не найдена для пользователя {UserId}", categoryName, userId);
                    return false;
                }

                // Устанавливаем ExpenseCategoryId
                expenseItem.ExpenseCategoryId = category.Id;

                // Создаем элемент расхода
                var result = await _dbcontext.ExpenseItems.CreateAsync(expenseItem);
                if (result)
                {
                    await _dbcontext.SaveAsync();
                    _logger.LogInformation("Элемент расхода {Name} успешно создан в категории {CategoryId}", expenseItem.Name, expenseItem.ExpenseCategoryId);
                    return true;
                }

                _logger.LogWarning("Не удалось создать элемент расхода {Name}", expenseItem.Name);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании элемента расхода {Name}", expenseItem.Name);
                return false;
            }
        }

    }
}

using server.ApplicationCore.DomModels;
using server.ApplicationCore.Interfaces.Repositories;
using server.ApplicationCore.Interfaces.Services;

namespace server.Infrastructure.BLL.Services
{
    public class IncomeService : IIncomeService
    {
        private IDbRepository _dbcontext;
        private readonly ILogger<IncomeService> _logger;

        public IncomeService(IDbRepository dbcontext, ILogger<IncomeService> logger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
        }

        public async Task<bool> CreateCategoryAsync(IncomeCategory category)
        {
            var result = await _dbcontext.IncomeCategories.CreateAsync(category);
            if (result)
            {
                await _dbcontext.SaveAsync(); // Сохраняем изменения в базе данных
            }
            return result;
        }

        public async Task<bool> CreateItemAsync(IncomeItem expenseItem, string userId, string categoryName)
        {
            try
            {
                // Получаем все категории пользователя по UserId
                var userCategories = await _dbcontext.IncomeCategories.GetByUserIdAsync(userId);
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
                expenseItem.IncomeCategoryId = category.Id;

                // Создаем элемент расхода
                var result = await _dbcontext.IncomeItems.CreateAsync(expenseItem);
                if (result)
                {
                    await _dbcontext.SaveAsync();
                    _logger.LogInformation("Элемент расхода {Name} успешно создан в категории {CategoryId}", expenseItem.Name, expenseItem.IncomeCategoryId);
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

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            // Удаляем категорию по ID через репозиторий
            var category = await _dbcontext.IncomeCategories.GetByIdAsync(id);
            if (category == null) return false;

            var result = await _dbcontext.IncomeCategories.DeleteAsync(category.Id);
            if (result)
            {
                await _dbcontext.SaveAsync(); // Сохраняем изменения в базе данных
            }
            return result;
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            // Удаляем подкатегорию по ID через репозиторий
            var item = await _dbcontext.IncomeItems.GetByIdAsync(id);
            if (item == null) return false;

            var result = await _dbcontext.IncomeItems.DeleteAsync(item.Id);
            if (result)
            {
                await _dbcontext.SaveAsync(); // Сохраняем изменения в базе данных
            }
            return result;
        }

        public async Task<List<IncomeCategory>> GetCategoriesByEmailAsync(string email)
        {
            // Получаем категории по UserId
            return await _dbcontext.IncomeCategories.GetByUserEmailAsync(email);
        }

        public async Task<bool> UpdateCategoryAsync(int id, string name)
        {
            try
            {
                var existingCategory = await _dbcontext.IncomeCategories.GetByIdAsync(id);
                if (existingCategory == null)
                {
                    _logger.LogWarning("IncomeCategory with ID {Id} not found", id);
                    return false;
                }

                existingCategory.Name = name;

                var result = await _dbcontext.IncomeCategories.UpdateAsync(existingCategory);
                if (result)
                {
                    await _dbcontext.SaveAsync();
                    _logger.LogInformation("IncomeCategory with ID {Id} successfully updated", id);
                    return true;
                }

                _logger.LogWarning("Failed to update IncomeCategory with ID {Id}", id);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating IncomeCategory with ID {Id}", id);
                return false;
            }
        }

        public async Task<bool> UpdateItemAsync(IncomeItem category)
        {
            // Обновляем существующую категорию
            var existingItem = await _dbcontext.IncomeItems.GetByIdAsync(category.Id);
            if (existingItem == null) return false;

            existingItem.Name = category.Name; // Обновляем необходимые поля
            existingItem.Amount = category.Amount;

            var result = await _dbcontext.IncomeItems.UpdateAsync(existingItem);
            if (result)
            {
                await _dbcontext.SaveAsync(); // Сохраняем изменения в базе данных
            }
            return result;
        }
    }
}

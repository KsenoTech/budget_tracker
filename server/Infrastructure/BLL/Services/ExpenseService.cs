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

        public async Task<(bool Success, string ErrorMessage)> CreateExpenseItemAsync(ExpenseItem expenseItem, string userId, string categoryName)
        {
            try
            {
                // Находим категорию по имени и userId
                var category = await _dbcontext.ExpenseCategories.GetListAsync()
                    .ContinueWith(task => task.Result
                        .FirstOrDefault(c => c.Name == categoryName && c.UserId == userId));

                if (category == null)
                {
                    _logger.LogWarning("Категория с именем {CategoryName} не найдена для пользователя {UserId}", categoryName, userId);
                    return (false, "Категория не найдена");
                }

                // Проверяем лимит
                var activeLimit = await GetActiveCategoryLimitAsync(category.Id, expenseItem.TransactionDate);
                if (activeLimit != null)
                {
                    // Получаем все траты за период действия лимита
                    var expensesInPeriod = await _dbcontext.ExpenseItems.GetListAsync()
                        .ContinueWith(task => task.Result
                            .Where(item =>
                                item.ExpenseCategoryId == category.Id &&
                                item.TransactionDate >= activeLimit.StartDate &&
                                item.TransactionDate <= activeLimit.EndDate)
                            .ToList());

                    decimal totalAmountInPeriod = expensesInPeriod.Sum(item => item.Amount) + expenseItem.Amount;

                    if (totalAmountInPeriod > activeLimit.LimitAmount)
                    {
                        _logger.LogWarning(
                            "Превышен лимит для категории {CategoryId}: {TotalAmount} превышает {LimitAmount}",
                            category.Id, totalAmountInPeriod, activeLimit.LimitAmount);
                        return (false, $"Превышен лимит на категорию {categoryName}. Лимит: {activeLimit.LimitAmount}, Текущие траты: {totalAmountInPeriod}");
                    }
                }

                // Связываем элемент с категорией
                expenseItem.ExpenseCategoryId = category.Id;

                var result = await _dbcontext.ExpenseItems.CreateAsync(expenseItem);
                if (result)
                {
                    await _dbcontext.SaveAsync();
                    _logger.LogInformation("Элемент расхода {Name} успешно создан в категории {CategoryId}", expenseItem.Name, expenseItem.ExpenseCategoryId);
                    return (true, null);
                }

                _logger.LogWarning("Не удалось создать элемент расхода {Name}", expenseItem.Name);
                return (false, "Не удалось создать элемент расхода");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании элемента расхода {Name}", expenseItem.Name);
                return (false, "Внутренняя ошибка сервера");
            }
        }

        public async Task<CategoryLimit> GetActiveCategoryLimitAsync(int expenseCategoryId, DateTime transactionDate)
        {
            try
            {
                var limit = await _dbcontext.CategoryLimits.GetListAsync()
                    .ContinueWith(task => task.Result
                        .FirstOrDefault(l =>
                            l.ExpenseCategoryId == expenseCategoryId &&
                            transactionDate >= l.StartDate &&
                            transactionDate <= l.EndDate));

                return limit;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении лимита для категории {CategoryId}", expenseCategoryId);
                return null;
            }
        }

        public async Task<bool> CreateCategoryLimitAsync(CategoryLimit categoryLimit)
        {
            try
            {
                var result = await _dbcontext.CategoryLimits.CreateAsync(categoryLimit);
                if (result)
                {
                    await _dbcontext.SaveAsync();
                    _logger.LogInformation("Лимит для категории {CategoryId} успешно создан", categoryLimit.ExpenseCategoryId);
                    return true;
                }

                _logger.LogWarning("Не удалось создать лимит для категории {CategoryId}", categoryLimit.ExpenseCategoryId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании лимита для категории {CategoryId}", categoryLimit.ExpenseCategoryId);
                return false;
            }
        }

        public async Task<bool> UpdateCategoryLimitAsync(CategoryLimit categoryLimit)
        {
            try
            {
                var existingLimit = await _dbcontext.CategoryLimits.GetByIdAsync(categoryLimit.Id);
                if (existingLimit == null)
                {
                    _logger.LogWarning("Лимит с ID {Id} не найден", categoryLimit.Id);
                    return false;
                }

                existingLimit.LimitAmount = categoryLimit.LimitAmount;
                existingLimit.StartDate = categoryLimit.StartDate;
                existingLimit.EndDate = categoryLimit.EndDate;

                var result = await _dbcontext.CategoryLimits.UpdateAsync(existingLimit);
                if (result)
                {
                    await _dbcontext.SaveAsync();
                    _logger.LogInformation("Лимит с ID {Id} успешно обновлён", categoryLimit.Id);
                    return true;
                }

                _logger.LogWarning("Не удалось обновить лимит с ID {Id}", categoryLimit.Id);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении лимита с ID {Id}", categoryLimit.Id);
                return false;
            }
        }

        public async Task<bool> DeleteCategoryLimitAsync(int id)
        {
            try
            {
                var result = await _dbcontext.CategoryLimits.DeleteAsync(id);
                if (result)
                {
                    await _dbcontext.SaveAsync();
                    _logger.LogInformation("Лимит с ID {Id} успешно удалён", id);
                    return true;
                }

                _logger.LogWarning("Не удалось удалить лимит с ID {Id}", id);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении лимита с ID {Id}", id);
                return false;
            }
        }

        public async Task<List<CategoryLimit>> GetAllCategoryLimitsAsync(string userId)
        {
            try
            {
                // Получаем все категории пользователя
                var userCategories = await _dbcontext.ExpenseCategories.GetListAsync()
                    .ContinueWith(task => task.Result.Where(c => c.UserId == userId).ToList());

                var userCategoryIds = userCategories.Select(c => c.Id).ToList();

                // Получаем лимиты для категорий пользователя
                var limits = await _dbcontext.CategoryLimits.GetListAsync()
                    .ContinueWith(task => task.Result
                        .Where(l => userCategoryIds.Contains(l.ExpenseCategoryId))
                        .ToList());

                return limits;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении лимитов для пользователя {UserId}", userId);
                return new List<CategoryLimit>();
            }
        }

        


        //public async Task<bool> CreateExpenseItemAsync(ExpenseItem expenseItem, string userId, string categoryName)
        //{
        //    try
        //    {
        //        // Получаем все категории пользователя по UserId
        //        var userCategories = await _dbcontext.ExpenseCategories.GetByUserIdAsync(userId);
        //        if (!userCategories.Any())
        //        {
        //            _logger.LogWarning("Категории для пользователя с ID {UserId} не найдены", userId);
        //            return false;
        //        }

        //        // Ищем категорию по имени
        //        var category = userCategories.FirstOrDefault(c => c.Name == categoryName);
        //        if (category == null)
        //        {
        //            _logger.LogWarning("Категория с именем {CategoryName} не найдена для пользователя {UserId}", categoryName, userId);
        //            return false;
        //        }

        //        // Устанавливаем ExpenseCategoryId
        //        expenseItem.ExpenseCategoryId = category.Id;

        //        // Создаем элемент расхода
        //        var result = await _dbcontext.ExpenseItems.CreateAsync(expenseItem);
        //        if (result)
        //        {
        //            await _dbcontext.SaveAsync();
        //            _logger.LogInformation("Элемент расхода {Name} успешно создан в категории {CategoryId}", expenseItem.Name, expenseItem.ExpenseCategoryId);
        //            return true;
        //        }

        //        _logger.LogWarning("Не удалось создать элемент расхода {Name}", expenseItem.Name);
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Ошибка при создании элемента расхода {Name}", expenseItem.Name);
        //        return false;
        //    }
        //}

    }
}

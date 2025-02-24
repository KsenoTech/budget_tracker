using Microsoft.EntityFrameworkCore.Internal;
using server.ApplicationCore.DomModels;
using server.ApplicationCore.Interfaces.Repositories;
using server.ApplicationCore.Interfaces.Services;
using server.Infrastructure.DAL.Repositories;

namespace server.Infrastructure.BLL.Services
{
    public class ExpenseCategoryService : IExpenseCategoryService
    {
        private IDbRepository _dbcontext;

        public ExpenseCategoryService(IDbRepository dbcontext)
        {
            _dbcontext = dbcontext;
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

        public async Task<List<ExpenseCategory>> GetCategoriesByEmailAsync(string email)
        {
            // Получаем категории по UserId
            return await _dbcontext.ExpenseCategories.GetByUserEmailAsync(email);
        }

        public async Task<bool> UpdateCategoryAsync(ExpenseCategory category)
        {
            // Обновляем существующую категорию
            var existingCategory = await _dbcontext.ExpenseCategories.GetByIdAsync(category.Id);
            if (existingCategory == null) return false;

            existingCategory.Name = category.Name; // Обновляем необходимые поля
            //existingCategory.UpdatedAt = DateTime.UtcNow; // Можно добавить поле UpdatedAt

            var result = await _dbcontext.ExpenseCategories.UpdateAsync(existingCategory);
            if (result)
            {
                await _dbcontext.SaveAsync(); // Сохраняем изменения в базе данных
            }
            return result;
        }
    }
}

using Microsoft.EntityFrameworkCore.Internal;
using server.ApplicationCore.DomModels;
using server.ApplicationCore.Interfaces.Repositories;
using server.ApplicationCore.Interfaces.Services;
using server.Infrastructure.DAL.Repositories;

namespace server.Infrastructure.BLL.Services
{
    public class IncomeCategoryService : IIncomeCategoryService
    {
        private IDbRepository _dbcontext;

        public IncomeCategoryService(IDbRepository dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<bool> CreateCategoryAsync(IncomeCategory category)
        {
            // Используем методы из IDbRepository для работы с IncomeCategory
            var result = await _dbcontext.IncomeCategories.CreateAsync(category);
            if (result)
            {
                await _dbcontext.SaveAsync(); // Сохраняем изменения в базе данных
            }
            return result;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            // Удаляем категорию по ID через репозиторий
            var category = await _dbcontext.IncomeCategories.GetByIdAsync(id);
            if (category == null) return false;

            var result = await _dbcontext.IncomeCategories.DeleteAsync(category);
            if (result)
            {
                await _dbcontext.SaveAsync(); // Сохраняем изменения в базе данных
            }
            return result;
        }

        public async Task<List<IncomeCategory>> GetCategoriesByUserAsync(string userId)
        {
            // Получаем категории по UserId
            return await _dbcontext.IncomeCategories.GetByUserIdAsync(userId);
        }

        public async Task<bool> UpdateCategoryAsync(IncomeCategory category)
        {
            // Обновляем существующую категорию
            var existingCategory = await _dbcontext.IncomeCategories.GetByIdAsync(category.Id);
            if (existingCategory == null) return false;

            existingCategory.Name = category.Name; // Обновляем необходимые поля
            //existingCategory.UpdatedAt = DateTime.UtcNow; // Можно добавить поле UpdatedAt

            var result = await _dbcontext.IncomeCategories.UpdateAsync(existingCategory);
            if (result)
            {
                await _dbcontext.SaveAsync(); // Сохраняем изменения в базе данных
            }
            return result;
        }
    }
}

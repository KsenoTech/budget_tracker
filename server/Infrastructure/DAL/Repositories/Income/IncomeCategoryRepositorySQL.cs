using Microsoft.EntityFrameworkCore;
using server.ApplicationCore.DomModels;
using server.ApplicationCore.Interfaces.Repositories;

namespace server.Infrastructure.DAL.Repositories.Income
{
    public class IncomeCategoryRepositorySQL : IRepository<IncomeCategory>
    {
        private readonly AccountingForIncomeAndExpensesContext _dbcontext;
        private readonly ILogger _logger;

        public IncomeCategoryRepositorySQL(AccountingForIncomeAndExpensesContext context, ILogger logger)
        {
            _dbcontext = context;
            _logger = logger;
        }

        public async Task<bool> CreateAsync(IncomeCategory entity)
        {
            try
            {
                _logger.LogInformation("Creating income category with Name: {Name} and UserId: {UserId}", entity.Name, entity.UserId);
                await _dbcontext.IncomeCategories.AddAsync(entity);
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating income category");
                return false;
            }
        }

        public async Task<bool> DeleteAsync<TId>(TId id)
        {
            var category = await GetByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Attempted to delete non-existent income category with Id: {Id}", id);
                return false;
            }

            try
            {
                _logger.LogInformation("Deleting income category with Id: {Id}", category.Id);
                _dbcontext.IncomeCategories.Remove(category);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting income category with Id: {Id}", category.Id);
                return false;
            }
        }

        public async Task<IncomeCategory> GetByIdAsync<TId>(TId id)
        {
            try
            {
                _logger.LogInformation("Fetching income category by Id: {Id}", id);

                if (id is int intId)
                {
                    return await _dbcontext.IncomeCategories
                                .FirstOrDefaultAsync(c => c.Id == intId);
                }
                else
                {
                    return await _dbcontext.IncomeCategories
                                .Include(ic => ic.IncomeItems) // Включаем связанные IncomeItems
                                .FirstOrDefaultAsync(ic => ic.Id == int.Parse(id.ToString()));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching income category by Id: {Id}", id);
                return null;
            }
        }

        public async Task<List<IncomeCategory>> GetByUserEmailAsync(string email)
        {
            var userId = await _dbcontext.Clients
                 .Where(c => c.Email == email)
                 .Select(c => c.Id)
                 .FirstOrDefaultAsync();

            if (userId == default)
            {
                return new List<IncomeCategory>();
            }

            var incomeCategories = await _dbcontext.IncomeCategories
                    .Join(_dbcontext.Clients,
                          category => category.UserId,
                          client => client.Id,
                          (category, client) => new { Category = category, Client = client })
                    .Where(x => x.Client.Email == email)
                    .Select(x => x.Category)
                    .Include(c => c.IncomeItems)
                    .ToListAsync();

            // Логируем результат
            _logger.LogInformation("Найдено {Count} категорий зачислений для пользователя {UserId}.", incomeCategories.Count, userId);
            foreach (var category in incomeCategories)
            {
                _logger.LogInformation("Категория зачислений: {CategoryName}, Количество IncomeItems: {ItemCount}", category.Name, category.IncomeItems.Count);
            }

            return incomeCategories;
        }

        public async Task<List<IncomeCategory>> GetByUserIdAsync(string userId)
        {
            try
            {
                _logger.LogInformation("Fetching income categories for user with UserId: {UserId}", userId);
                return await _dbcontext.IncomeCategories
                    .Where(c => c.UserId == userId)
                    .Include(c => c.IncomeItems)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching income categories for user with UserId: {UserId}", userId);
                return new List<IncomeCategory>();
            }
        }

        public async Task<List<IncomeCategory>> GetListAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all income categories");
                return await _dbcontext.IncomeCategories
                .Include(ic => ic.IncomeItems) // Включаем связанные IncomeItems
                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all income categories");
                return new List<IncomeCategory>();
            }
        }

        public async Task<bool> UpdateAsync(IncomeCategory request)
        {
            try
            {
                _logger.LogInformation("Updating income category with Id: {Id}", request.Id);
                _dbcontext.Entry(request).State = EntityState.Modified;
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating income category with Id: {Id}", request.Id);
                return false;
            }
        }
    }
}

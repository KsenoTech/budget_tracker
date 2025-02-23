using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using server.ApplicationCore.DomModels;
using server.ApplicationCore.Interfaces.Repositories;

namespace server.Infrastructure.DAL.Repositories
{
    public class IncomeCategoryRepositorySQL : IRepository<IncomeCategory>
    {
        private readonly AccountingForIncomeAndExpensesContext _dbcontext;
        private readonly ILogger<IncomeCategoryRepositorySQL> _logger;

        public IncomeCategoryRepositorySQL(AccountingForIncomeAndExpensesContext context, 
                                           ILogger<IncomeCategoryRepositorySQL> logger)
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

        public async Task<List<IncomeCategory>> GetListAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all income categories");
                return await _dbcontext.IncomeCategories.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all income categories");
                return new List<IncomeCategory>();
            }
        }

        public async Task<IncomeCategory> GetByIdAsync<TId>(TId id)
        {
            try
            {
                _logger.LogInformation("Fetching income category by Id: {Id}", id);
                return await _dbcontext.IncomeCategories.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching income category by Id: {Id}", id);
                return null;
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
    }
}

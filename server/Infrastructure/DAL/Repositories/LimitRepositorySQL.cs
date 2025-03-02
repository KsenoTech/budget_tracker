using Microsoft.EntityFrameworkCore;
using server.ApplicationCore.DomModels;
using server.ApplicationCore.Interfaces.Repositories;

namespace server.Infrastructure.DAL.Repositories
{
    public class LimitRepositorySQL : IRepository<CategoryLimit>
    {
        private readonly AccountingForIncomeAndExpensesContext _dbcontext;
        private readonly ILogger _logger;

        public LimitRepositorySQL(AccountingForIncomeAndExpensesContext context, ILogger logger)
        {
            _dbcontext = context;
            _logger = logger;
        }

        public async Task<bool> CreateAsync(CategoryLimit entity)
        {
            try
            {
                _logger.LogInformation("Creating CategoryLimits with Name: {Name} and UserId: {UserId}", entity.Id, entity.ExpenseCategoryId);
                await _dbcontext.CategoryLimits.AddAsync(entity);
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
                _dbcontext.CategoryLimits.Remove(category);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting income category with Id: {Id}", category.Id);
                return false;
            }
        }

        public async Task<CategoryLimit> GetByIdAsync<TId>(TId id)
        {
            try
            {
                _logger.LogInformation("Fetching income category by Id: {Id}", id);

                if (id is int intId)
                {
                    return await _dbcontext.CategoryLimits
                                .FirstOrDefaultAsync(c => c.Id == intId);
                }
                else
                {
                    //return new CategoryLimit();
                    return await _dbcontext.CategoryLimits
                                .Include(ic => ic.ExpenseCategory) // Включаем связанные IncomeItems
                                .FirstOrDefaultAsync(ic => ic.Id == int.Parse(id.ToString()));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching income category by Id: {Id}", id);
                return null;
            }
        }

        public Task<List<CategoryLimit>> GetByUserEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<List<CategoryLimit>> GetByUserIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<CategoryLimit>> GetListAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all income categories");
                return await _dbcontext.CategoryLimits
                .Include(ic => ic.ExpenseCategory) // Включаем связанные IncomeItems
                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all income categories");
                return new List<CategoryLimit>();
            }
        }

        public async Task<bool> UpdateAsync(CategoryLimit request)
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

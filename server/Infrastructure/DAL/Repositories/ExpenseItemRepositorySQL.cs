using Microsoft.EntityFrameworkCore;
using server.ApplicationCore.DomModels;
using server.ApplicationCore.Interfaces.Repositories;

namespace server.Infrastructure.DAL.Repositories
{
    public class ExpenseItemRepositorySQL : IRepository<ExpenseItem>
    {
        private readonly AccountingForIncomeAndExpensesContext _dbcontext;
        private readonly ILogger _logger;

        public ExpenseItemRepositorySQL(AccountingForIncomeAndExpensesContext context, ILogger logger)
        {
            _dbcontext = context;
            _logger = logger;
        }

        
        public async Task<bool> CreateAsync(ExpenseItem entity)
        {
            try
            {
                _logger.LogInformation("Creating expense category with Name: {Name}", entity.Name);
                await _dbcontext.ExpenseItems.AddAsync(entity);
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
                _dbcontext.ExpenseItems.Remove(category);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting income category with Id: {Id}", category.Id);
                return false;
            }
        }

        public async Task<ExpenseItem> GetByIdAsync<TId>(TId id)
        {
            try
            {
                _logger.LogInformation("Fetching income category by Id: {Id}", id);

                if (id is int intId)
                {
                    return await _dbcontext.ExpenseItems
                                .FirstOrDefaultAsync(c => c.Id == intId);
                }
                else
                {
                    return new ExpenseItem();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching income category by Id: {Id}", id);
                return null;
            }
        }

        public Task<List<ExpenseCategory>> GetByUserEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<List<ExpenseCategory>> GetByUserIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<ExpenseItem>> GetListAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(ExpenseItem request)
        {
            throw new NotImplementedException();
        }
    }
}

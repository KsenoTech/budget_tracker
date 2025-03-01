using Microsoft.EntityFrameworkCore;
using server.ApplicationCore.DomModels;
using server.ApplicationCore.Interfaces.Repositories;

namespace server.Infrastructure.DAL.Repositories.Income
{
    public class IncomeItemRepositorySQL : IRepository<IncomeItem>
    {
        private readonly AccountingForIncomeAndExpensesContext _dbcontext;
        private readonly ILogger _logger;

        public IncomeItemRepositorySQL(AccountingForIncomeAndExpensesContext context, ILogger logger)
        {
            _dbcontext = context;
            _logger = logger;
        }

        public async Task<bool> CreateAsync(IncomeItem entity)
        {
            try
            {
                _logger.LogInformation("Creating IncomeItem with Name: {Name}", entity.Name);
                await _dbcontext.IncomeItems.AddAsync(entity);
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating IncomeItem");
                return false;
            }
        }

        public async Task<bool> DeleteAsync<TId>(TId id)
        {
            var category = await GetByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Attempted to delete non-existent income item with Id: {Id}", id);
                return false;
            }

            try
            {
                _logger.LogInformation("Deleting income item with Id: {Id}", category.Id);
                _dbcontext.IncomeItems.Remove(category);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting income item with Id: {Id}", category.Id);
                return false;
            }
        }

        public async Task<IncomeItem> GetByIdAsync<TId>(TId id)
        {
            try
            {
                _logger.LogInformation("Fetching income item by Id: {Id}", id);

                if (id is int intId)
                {
                    return await _dbcontext.IncomeItems
                                .FirstOrDefaultAsync(c => c.Id == intId);
                }
                else
                {
                    return new IncomeItem();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching income item by Id: {Id}", id);
                return null;
            }
        }

        public Task<List<IncomeItem>> GetByUserEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<List<IncomeItem>> GetByUserIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<IncomeItem>> GetListAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateAsync(IncomeItem request)
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

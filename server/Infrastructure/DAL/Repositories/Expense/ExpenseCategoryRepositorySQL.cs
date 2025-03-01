using Microsoft.EntityFrameworkCore;
using server.ApplicationCore.DomModels;
using server.ApplicationCore.Interfaces.Repositories;

namespace server.Infrastructure.DAL.Repositories.Expense
{
    public class ExpenseCategoryRepositorySQL : IRepository<ExpenseCategory>
    {
        private readonly AccountingForIncomeAndExpensesContext _dbcontext;
        private readonly ILogger _logger;

        public ExpenseCategoryRepositorySQL(AccountingForIncomeAndExpensesContext context, ILogger logger)
        {
            _dbcontext = context;
            _logger = logger;
        }


        public async Task<List<ExpenseCategory>> GetByUserEmailAsync(string email)
        {
            var userId = await _dbcontext.Clients
                .Where(c => c.Email == email)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            if (userId == default)
            {
                return new List<ExpenseCategory>();
            }

            var incomeCategories = await _dbcontext.ExpenseCategories
                    .Join(_dbcontext.Clients,
                          category => category.UserId,
                          client => client.Id,
                          (category, client) => new { Category = category, Client = client })
                    .Where(x => x.Client.Email == email)
                    .Select(x => x.Category)
                    .Include(c => c.ExpenseItems)
                    .ToListAsync();

            // Логируем результат
            _logger.LogInformation("Найдено {Count} категорий трат для пользователя {UserId}.", incomeCategories.Count, userId);
            foreach (var category in incomeCategories)
            {
                _logger.LogInformation("Категория трат: {CategoryName}, Количество IncomeItems: {ItemCount}", category.Name, category.ExpenseItems.Count);
            }

            return incomeCategories;
        }

        public async Task<bool> CreateAsync(ExpenseCategory entity)
        {
            try
            {
                _logger.LogInformation("Creating expense category with Name: {Name} and UserId: {UserId}", entity.Name, entity.UserId);
                await _dbcontext.ExpenseCategories.AddAsync(entity);
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating expense category");
                return false;
            }
        }

        public async Task<bool> DeleteAsync<TId>(TId id)
        {
            var category = await GetByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Attempted to delete non-existent expense category with Id: {Id}", id);
                return false;
            }

            try
            {
                _logger.LogInformation("Deleting expense category with Id: {Id}", category.Id);
                _dbcontext.ExpenseCategories.Remove(category);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting expense category with Id: {Id}", category.Id);
                return false;
            }
        }
        

        public async Task<List<ExpenseCategory>> GetListAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all expense categories");
                return await _dbcontext.ExpenseCategories
                .Include(ic => ic.ExpenseItems) // Включаем связанные IncomeItems
                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all income categories");
                return new List<ExpenseCategory>();
            }
        }

        public async Task<ExpenseCategory> GetByIdAsync<TId>(TId id)
        {
            try
            {
                _logger.LogInformation("Fetching expense category by Id: {Id}", id);

                if (id is int intId)
                {
                    return await _dbcontext.ExpenseCategories
                                .FirstOrDefaultAsync(c => c.Id == intId);
                }
                else
                {
                    return await _dbcontext.ExpenseCategories
                                .Include(ic => ic.ExpenseItems) // Включаем связанные IncomeItems
                                .FirstOrDefaultAsync(ic => ic.Id == int.Parse(id.ToString()));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching expense category by Id: {Id}", id);
                return null;
            }
        }

        public async Task<bool> UpdateAsync(ExpenseCategory request)
        {
            try
            {
                _logger.LogInformation("Updating expense category with Id: {Id}", request.Id);
                _dbcontext.Entry(request).State = EntityState.Modified;
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating expense category with Id: {Id}", request.Id);
                return false;
            }
        }

        public async Task<List<ExpenseCategory>> GetByUserIdAsync(string userId)
        {
            try
            {
                _logger.LogInformation("Fetching expense categories for user with UserId: {UserId}", userId);
                return await _dbcontext.ExpenseCategories
                    .Where(c => c.UserId == userId)
                    .Include(c => c.ExpenseItems)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching expense categories for user with UserId: {UserId}", userId);
                return new List<ExpenseCategory>();
            }
        }
    }
}

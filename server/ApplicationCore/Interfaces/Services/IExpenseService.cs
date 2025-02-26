using server.ApplicationCore.DomModels;

namespace server.ApplicationCore.Interfaces.Services
{
    public interface IExpenseService
    {
        Task<bool> CreateCategoryAsync(ExpenseCategory category);
        Task<bool> CreateExpenseItemAsync(ExpenseItem expenseItem, string userId, string categoryName);
        Task<bool> DeleteCategoryAsync(int id);
        Task<bool> DeleteItemAsync(int id);
        Task<List<ExpenseCategory>> GetCategoriesByEmailAsync(string email);
        Task<bool> UpdateCategoryAsync(ExpenseCategory category);
    }
}

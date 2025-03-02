using server.ApplicationCore.DomModels;

namespace server.ApplicationCore.Interfaces.Services
{
    public interface IExpenseService
    {
        #region Categories
            Task<bool> CreateCategoryAsync(ExpenseCategory category);
            Task<bool> DeleteCategoryAsync(int id);
            Task<List<ExpenseCategory>> GetCategoriesByEmailAsync(string email);
            Task<bool> UpdateCategoryAsync(int id, string name);
        #endregion


        #region Items
        Task<(bool Success, string ErrorMessage)> CreateExpenseItemAsync(ExpenseItem expenseItem, string userId, string categoryName);
        Task<bool> DeleteItemAsync(int id);
            Task<bool> UpdateItemAsync(ExpenseItem category);
        #endregion


        #region Limits
            Task<CategoryLimit> GetActiveCategoryLimitAsync(int expenseCategoryId, DateTime transactionDate);
            Task<bool> CreateCategoryLimitAsync(CategoryLimit categoryLimit);
            Task<List<CategoryLimit>> GetAllCategoryLimitsAsync(string userId);
            Task<bool> UpdateCategoryLimitAsync(CategoryLimit categoryLimit);
            Task<bool> DeleteCategoryLimitAsync(int id);
        #endregion
    }
}

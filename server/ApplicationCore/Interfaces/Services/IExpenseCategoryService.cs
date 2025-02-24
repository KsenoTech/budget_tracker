using server.ApplicationCore.DomModels;

namespace server.ApplicationCore.Interfaces.Services
{
    public interface IExpenseCategoryService
    {
        Task<bool> CreateCategoryAsync(ExpenseCategory category);
        Task<bool> DeleteCategoryAsync(int id);
        Task<List<ExpenseCategory>> GetCategoriesByEmailAsync(string email);
        Task<bool> UpdateCategoryAsync(ExpenseCategory category);
    }
}

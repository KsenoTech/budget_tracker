using server.ApplicationCore.DomModels;

namespace server.ApplicationCore.Interfaces.Services
{
    public interface IIncomeService
    {
        Task<bool> CreateCategoryAsync(IncomeCategory category);
        Task<bool> CreateItemAsync(IncomeItem expenseItem, string userId, string categoryName);

        Task<bool> DeleteCategoryAsync(int id);
        Task<bool> DeleteItemAsync(int id);

        Task<List<IncomeCategory>> GetCategoriesByEmailAsync(string email);

        Task<bool> UpdateCategoryAsync(int id, string name);
        Task<bool> UpdateItemAsync(IncomeItem category);
    }
}

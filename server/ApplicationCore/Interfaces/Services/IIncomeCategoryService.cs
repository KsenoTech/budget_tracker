using server.ApplicationCore.DomModels;

namespace server.ApplicationCore.Interfaces.Services
{
    public interface IIncomeCategoryService
    {
        Task<bool> CreateCategoryAsync(IncomeCategory category);
        Task<bool> DeleteCategoryAsync(int id);
        Task<List<IncomeCategory>> GetCategoriesByUserAsync(string userId);
        Task<bool> UpdateCategoryAsync(IncomeCategory category);
    }
}

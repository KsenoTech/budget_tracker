using server.ApplicationCore.DomModels;

namespace server.ApplicationCore.Interfaces.Repositories
{
    public interface IDbRepository
    {
        IRepository<Client> Clients { get; }
        IRepository<CategoryLimit> CategoryLimits { get; }
        IRepository<ExpenseCategory> ExpenseCategorys { get; }
        IRepository<ExpenseItem> ExpenseItems { get; }
        IRepository<IncomeCategory> IncomeCategories { get; }
        IRepository<IncomeItem> IncomeItems { get; }
        Task SaveAsync();
    }
}

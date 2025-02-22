using server.ApplicationCore.DomModels;

namespace server.ApplicationCore.Interfaces.Repositories
{
    public interface IDbRepository
    {
        IRepository<User> Clients { get; }
        IRepository<CategoryLimit> CategoryLimits { get; }
        IRepository<ExpenseCategory> ExpenseCategorys { get; }
        IRepository<ExpenseItem> ExpenseItems { get; }
        IRepository<IncomeCategory> IncomeCategorys { get; }
        IRepository<IncomeItem> IncomeItems { get; }
        int Save();
    }
}

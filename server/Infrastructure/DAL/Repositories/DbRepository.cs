using server.ApplicationCore.DomModels;
using server.ApplicationCore.Interfaces.Repositories;

namespace server.Infrastructure.DAL.Repositories
{
    public class DbRepository : IDbRepository
    {
        private AccountingForIncomeAndExpensesContext _dbcontext;
        private readonly ILogger<AccountingForIncomeAndExpensesContext> _logger;


        public DbRepository(AccountingForIncomeAndExpensesContext dbcontext, ILogger<AccountingForIncomeAndExpensesContext> logger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
        }


        public IRepository<User> Clients
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IRepository<CategoryLimit> CategoryLimits
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IRepository<ExpenseCategory> ExpenseCategorys
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IRepository<ExpenseItem> ExpenseItems
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IRepository<IncomeCategory> IncomeCategorys
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IRepository<IncomeItem> IncomeItems
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Save()
        {
            throw new NotImplementedException();
        }
    }
}

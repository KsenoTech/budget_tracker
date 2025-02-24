using server.ApplicationCore.DomModels;
using server.ApplicationCore.Interfaces.Repositories;

namespace server.Infrastructure.DAL.Repositories
{
    public class DbRepository : IDbRepository
    {
        private AccountingForIncomeAndExpensesContext _dbcontext;
        private readonly ILogger<AccountingForIncomeAndExpensesContext> _logger;

        private ClientRepositorySQL _clientReposSQL;
        private ExpenseCategoryRepositorySQL _expenseCategorySQL;


        public DbRepository(AccountingForIncomeAndExpensesContext dbcontext, ILogger<AccountingForIncomeAndExpensesContext> logger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
        }


        public IRepository<Client> Clients
        {
            get
            {
                if (_clientReposSQL == null)
                    _clientReposSQL = new ClientRepositorySQL(_dbcontext);
                return _clientReposSQL;
            }
        }

        public IRepository<CategoryLimit> CategoryLimits
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IRepository<ExpenseCategory> ExpenseCategories
        {
            get
            {
                if (_expenseCategorySQL == null)
                    _expenseCategorySQL = new ExpenseCategoryRepositorySQL(_dbcontext, _logger);
                return _expenseCategorySQL;
            }
        }

        public IRepository<ExpenseItem> ExpenseItems
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IRepository<IncomeCategory> IncomeCategories
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

        public async Task SaveAsync()
        {
            await _dbcontext.SaveChangesAsync();
        }
    }
}

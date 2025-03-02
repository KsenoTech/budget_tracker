using server.ApplicationCore.DomModels;
using server.ApplicationCore.Interfaces.Repositories;
using server.Infrastructure.DAL.Repositories.Expense;
using server.Infrastructure.DAL.Repositories.Income;

namespace server.Infrastructure.DAL.Repositories
{
    public class DbRepository : IDbRepository
    {
        private AccountingForIncomeAndExpensesContext _dbcontext;
        private readonly ILogger<AccountingForIncomeAndExpensesContext> _logger;

        private ClientRepositorySQL _clientReposSQL;

        private ExpenseCategoryRepositorySQL _expenseCategoryReposSQL;
        private ExpenseItemRepositorySQL _expenseItemReposSQL;

        private IncomeCategoryRepositorySQL _incomeCategoryReposSQL;
        private IncomeItemRepositorySQL _incomeItemReposSQL;

        private LimitRepositorySQL _limitReposSQL;

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
                if (_limitReposSQL == null)
                    _limitReposSQL = new LimitRepositorySQL(_dbcontext, _logger);
                return _limitReposSQL;
            }
        }

        public IRepository<ExpenseCategory> ExpenseCategories
        {
            get
            {
                if (_expenseCategoryReposSQL == null)
                    _expenseCategoryReposSQL = new ExpenseCategoryRepositorySQL(_dbcontext, _logger);
                return _expenseCategoryReposSQL;
            }
        }

        public IRepository<ExpenseItem> ExpenseItems
        {
            get
            {
                if (_expenseItemReposSQL == null)
                    _expenseItemReposSQL = new ExpenseItemRepositorySQL(_dbcontext, _logger);
                return _expenseItemReposSQL;
            }
        }

        public IRepository<IncomeCategory> IncomeCategories
        {
            get
            {
                if (_incomeCategoryReposSQL == null)
                    _incomeCategoryReposSQL = new IncomeCategoryRepositorySQL(_dbcontext, _logger);
                return _incomeCategoryReposSQL;
            }
        }

        public IRepository<IncomeItem> IncomeItems
        {
            get
            {
                if (_incomeItemReposSQL == null)
                    _incomeItemReposSQL = new IncomeItemRepositorySQL(_dbcontext, _logger);
                return _incomeItemReposSQL;
            }
        }

        public async Task SaveAsync()
        {
            await _dbcontext.SaveChangesAsync();
        }
    }
}

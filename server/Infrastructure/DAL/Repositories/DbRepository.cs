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

        private ExpenseCategoryRepositorySQL _expenseCategorySQL;
        private ExpenseItemRepositorySQL _expenseItemSQL;

        private IncomeCategoryRepositorySQL _incomeCategorySQL;
        private IncomeItemRepositorySQL _incomeItemSQL;


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
                if (_expenseItemSQL == null)
                    _expenseItemSQL = new ExpenseItemRepositorySQL(_dbcontext, _logger);
                return _expenseItemSQL;
            }
        }

        public IRepository<IncomeCategory> IncomeCategories
        {
            get
            {
                if (_incomeCategorySQL == null)
                    _incomeCategorySQL = new IncomeCategoryRepositorySQL(_dbcontext, _logger);
                return _incomeCategorySQL;
            }
        }

        public IRepository<IncomeItem> IncomeItems
        {
            get
            {
                if (_incomeItemSQL == null)
                    _incomeItemSQL = new IncomeItemRepositorySQL(_dbcontext, _logger);
                return _incomeItemSQL;
            }
        }

        public async Task SaveAsync()
        {
            await _dbcontext.SaveChangesAsync();
        }
    }
}

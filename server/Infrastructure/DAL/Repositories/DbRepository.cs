using server.ApplicationCore.DomModels;
using server.ApplicationCore.Interfaces.Repositories;

namespace server.Infrastructure.DAL.Repositories
{
    public class DbRepository : IDbRepository
    {
        private AccountingForIncomeAndExpensesContext _dbcontext;

        private ClientRepositorySQL _clientReposSQL;
        private IncomeCategoryRepositorySQL _incomeCategorySQL;


        public DbRepository(AccountingForIncomeAndExpensesContext dbcontext)
        {
            _dbcontext = dbcontext;
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

        public IRepository<IncomeCategory> IncomeCategories
        {
            get
            {
                if (_incomeCategorySQL == null)
                    _incomeCategorySQL = new IncomeCategoryRepositorySQL(_dbcontext);
                return _incomeCategorySQL;
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

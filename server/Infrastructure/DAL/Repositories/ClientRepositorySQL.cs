using Microsoft.EntityFrameworkCore;
using server.ApplicationCore.DomModels;
using server.ApplicationCore.Interfaces.Repositories;

namespace server.Infrastructure.DAL.Repositories
{
    public class ClientRepositorySQL : IRepository<Client>
    {
        private readonly AccountingForIncomeAndExpensesContext _dbcontext;

        public ClientRepositorySQL(AccountingForIncomeAndExpensesContext context)
        {
            _dbcontext = context;
        }

        public Task<List<Client>> GetListAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CreateAsync(Client entity)
        {
            try
            {
                await _dbcontext.Clients.AddAsync(entity);                
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Client> GetByIdAsync<TId>(TId email)
        {
            try
            {
                var user = await _dbcontext.Clients.FirstOrDefaultAsync(u => u.Email == email.ToString());

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении User: {ex.Message}");
                throw; // Повторно выбрасываем исключение
            }
        }

        public Task<bool> UpdateAsync(Client request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync<TId>(TId id)
        {
            throw new NotImplementedException();
        }

        public Task<List<IncomeCategory>> GetByUserIdAsync(string userId)
        {
            throw new NotImplementedException();
        }
    }
}

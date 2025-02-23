using Microsoft.EntityFrameworkCore;
using server.ApplicationCore.DomModels;
using server.ApplicationCore.Interfaces.Repositories;
using server.Infrastructure.BLL.Services;

namespace server.Infrastructure.DAL.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AccountingForIncomeAndExpensesContext _context;

        public AuthRepository(AccountingForIncomeAndExpensesContext context)
        {
            _context = context;
        }

        public async Task<Client> RegisterUser(Client user)
        {
            _context.Clients.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<Client> GetUserByUsername(string username)
        {
            return await _context.Clients.FirstOrDefaultAsync(u => u.UserName == username);
        }
    }
}

using server.ApplicationCore.DomModels;

namespace server.ApplicationCore.Interfaces.Repositories
{
    public interface IAuthRepository
    {
        Task<Client> RegisterUser(Client user);
        Task<Client> GetUserByUsername(string username);
    }
}

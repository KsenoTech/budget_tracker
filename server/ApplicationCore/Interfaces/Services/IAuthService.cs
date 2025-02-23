namespace server.ApplicationCore.Interfaces.Services
{
    public interface IAuthService
    {
        Task<string> Register(string username, string password, string email);
        Task<string> Login(string username, string password);
    }
}

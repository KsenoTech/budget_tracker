namespace server.ApplicationCore.Interfaces.Services
{
    public interface IClientService
    {
        Task<string> AuthenticateClient(string username, string email, string password);
    }
}

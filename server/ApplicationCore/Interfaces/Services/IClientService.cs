namespace server.ApplicationCore.Interfaces.Services
{
    public interface IClientService
    {
        Task<string> AuthenticateClient(string password, string email);
    }
}

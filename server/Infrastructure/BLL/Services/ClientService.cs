using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using server.ApplicationCore.DomModels;
using server.ApplicationCore.Interfaces.Repositories;
using server.ApplicationCore.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace server.Infrastructure.BLL.Services
{
    public class ClientService : IClientService
    {
        private IDbRepository _dbcontext;
        private readonly IConfiguration _configuration;      

        public ClientService(IDbRepository dbcontext, IConfiguration configuration)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
        }

        public async Task<string> AuthenticateClient(string password, string email)
        {
            var existingUser = await _dbcontext.Clients.GetByIdAsync(email);
            if (existingUser != null)
            {
                // Если пользователь уже существует, проверяем пароль
                if (!BCrypt.Net.BCrypt.Verify(password, existingUser.PasswordHash))
                    throw new Exception("Пользователь с таким Email существует. Проверьте пароль");

                // Выдаем новый токен существующему пользователю
                return GenerateJwtToken(existingUser);
            }

            // Регистрация нового пользователя
            var user = new Client
            {
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Email = email,
                CreatedAt = DateTime.UtcNow
            };

            var clientCreated = await _dbcontext.Clients.CreateAsync(user);
            if (clientCreated)
            {
                await _dbcontext.SaveAsync();
            }
            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(Client user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7), // Токен действует 7 дней
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

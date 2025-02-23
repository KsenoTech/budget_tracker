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
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;      

        public AuthService(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
        }

        public async Task<string> Register(string username, string password, string email)
        {
            var existingUser = await _authRepository.GetUserByUsername(username);
            if (existingUser != null)
            {
                // Если пользователь уже существует, проверяем пароль
                if (!BCrypt.Net.BCrypt.Verify(password, existingUser.PasswordHash))
                    throw new Exception("Invalid password for existing user");

                // Выдаем новый токен существующему пользователю
                return GenerateJwtToken(existingUser);
            }

            // Регистрация нового пользователя
            var user = new Client
            {
                UserName = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Email = email,
                CreatedAt = DateTime.UtcNow
            };

            await _authRepository.RegisterUser(user);
            return GenerateJwtToken(user);
        }

        public async Task<string> Login(string username, string password)
        {
            var user = await _authRepository.GetUserByUsername(username);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                throw new Exception("Invalid credentials");

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(Client user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
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

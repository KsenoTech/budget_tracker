using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using server.ApplicationCore.Interfaces.Services;
using System.Security.Claims;
using static server.ApplicationCore.Models.ResponseModels;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class AuthController : Controller
    {
        private readonly IClientService _clientService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IClientService authService, ILogger<AuthController> logger)
        {
            _clientService = authService;
            _logger = logger;
        }

        [HttpPost("auth")]
        public async Task<IActionResult> Authenticate([FromBody] RegisterDto dto)
        {
            try
            {
                _logger.LogInformation("Получение dto.Email в методе Register in Controller: {Email}", dto.Email);

                var token = await _clientService.AuthenticateClient( dto.UserName, dto.Email, dto.Password);
                _logger.LogInformation(dto.Email, token);
                _logger.LogInformation("Login successful for Email: {Email} with token {token}", dto.Email, token);

                return Ok(new { Token = token, Message = "Successfully logged in" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for username: {Username}", dto.Email);
                return BadRequest(new { ex.Message });
            }
        }


        // Новый метод для проверки токена
        [HttpGet("checkAuth")]
        public IActionResult CheckAuth()
        {
            try
            {
                if (!HttpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    _logger.LogWarning("Заголовок Authorization отсутствует");
                    return Unauthorized(new { Message = "Missing authorization header" });
                }

                // Извлекаем данные из токена через ClaimsPrincipal
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var username = User.FindFirst(ClaimTypes.Name)?.Value;
                var email = User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Токен не содержит userId о пользователе");
                    return Unauthorized(new { Message = "Недостаточно данных в токене" });
                }

                if (string.IsNullOrEmpty(username))
                {
                    _logger.LogWarning("Токен не содержит username о пользователе");
                    return Unauthorized(new { Message = "Недостаточно данных в токене" });
                }

                _logger.LogInformation("Токен валиден для пользователя: {Username}", username);
                return Ok(new
                {
                    UserId = userId,
                    Username = username,
                    Email = email,
                    Message = "Token is valid"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при проверке токена");
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }
    }
}

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

                var token = await _clientService.AuthenticateClient( dto.Password, dto.Email);
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


        //[HttpGet("checkAuth")]
        //[Authorize] // Требует валидный токен проверять при обновлении страницы на реакте
        //public IActionResult CheckToken()
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    var email = User.FindFirst(ClaimTypes.Email)?.Value;
        //    return Ok(new { UserId = userId, Email = email, Message = "Token is valid" });
        //}
    }
}

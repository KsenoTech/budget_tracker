using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.ApplicationCore.Interfaces.Services;
using System.Security.Claims;
using static server.ApplicationCore.Models.ResponseModels;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                _logger.LogInformation("Login attempt for username: {Username}", dto.Username);
                var token = await _authService.Register(dto.Username, dto.Password, dto.Email);
                _logger.LogInformation("Login successful for username: {Username}", dto.Username);
                return Ok(new { Token = token, Message = "Successfully logged in" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for username: {Username}", dto.Username);
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                _logger.LogInformation("Login attempt for username: {Username}", dto.Username);
                var token = await _authService.Login(dto.Username, dto.Password);
                return Ok(new { Token = token, Message = "Successfully logged in" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("check")]
        [Authorize] // Требует валидный токен
        public IActionResult CheckToken()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            return Ok(new { UserId = userId, Username = username, Message = "Token is valid" });
        }
    }
}

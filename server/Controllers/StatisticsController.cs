using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.ApplicationCore.Interfaces.Services;
using System.Security.Claims;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : Controller
    {
        private readonly IIncomeService _incomeService;
        private readonly IExpenseService _expenseService;

        private readonly ILogger<StatisticsController> _logger;

        public StatisticsController(
            IIncomeService incomeService,
            IExpenseService expenseService,
            ILogger<StatisticsController> logger)
            {
                _incomeService = incomeService;
                _expenseService = expenseService;
                _logger = logger;
            }

        [HttpGet("getMonthlyStatistics")]
        [Authorize]
        public async Task<IActionResult> GetMonthlyStatistics(DateTime startDate, DateTime endDate)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Не удалось извлечь UserId из токена");
                    return Unauthorized(new { Message = "Недействительный токен" });
                }

                // Получаем email пользователя (предполагаем, что оно связано с UserId)
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(userEmail))
                {
                    _logger.LogWarning("Email пользователя с ID {UserId} не найден", userId);
                    return BadRequest(new { Message = "Email пользователя не найден" });
                }

                // Получаем категории доходов
                var incomeCategories = await _incomeService.GetCategoriesByEmailAsync(userEmail);
                var incomeStats = incomeCategories
                    .Select(cat => new
                    {
                        Name = cat.Name,
                        TotalAmount = cat.IncomeItems
                            .Where(item => item.TransactionDate >= startDate && item.TransactionDate <= endDate)
                            .Sum(item => item.Amount)
                    })
                    .Where(stat => stat.TotalAmount > 0)
                    .ToList();

                // Получаем категории расходов
                var expenseCategories = await _expenseService.GetCategoriesByEmailAsync(userEmail);
                var expenseStats = expenseCategories
                    .Select(cat => new
                    {
                        Name = cat.Name,
                        TotalAmount = cat.ExpenseItems
                            .Where(item => item.TransactionDate >= startDate && item.TransactionDate <= endDate)
                            .Sum(item => item.Amount)
                    })
                    .Where(stat => stat.TotalAmount > 0)
                    .ToList();

                _logger.LogInformation("Успешно получена статистика за период с {StartDate} по {EndDate}", startDate, endDate);

                return Ok(new
                {
                    Incomes = incomeStats,
                    Expenses = expenseStats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении статистики за период с {StartDate} по {EndDate}", startDate, endDate);
                return StatusCode(500, new { Message = "Внутренняя ошибка сервера" });
            }
        }
    }
}

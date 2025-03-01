using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using server.ApplicationCore.DomModels;
using server.ApplicationCore.Interfaces.Services;
using System.Security.Claims;
using static server.ApplicationCore.Models.ResponseModels;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    public class IncomeController : Controller
    {
        private readonly IIncomeService _incomeService;
        private readonly ILogger<IncomeController> _logger;

        public IncomeController(IIncomeService incomeService, ILogger<IncomeController> logger)
        {
            _incomeService = incomeService;
            _logger = logger;
        }

        //[Authorize]
        [HttpGet("getAllForOneUserByEmail")]
        public async Task<ActionResult<List<IncomeCategory>>> GetIncomeCategories(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("Email не указан");
                return BadRequest(new { Message = "Email не указан" });
            }

            var incomeCategories = await _incomeService.GetCategoriesByEmailAsync(email);
            return Ok(incomeCategories);
        }

        //[Authorize]
        [HttpPost("createCategory")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategory dto)
        {
            try
            {
                if (dto == null || string.IsNullOrEmpty(dto.Name))
                {
                    _logger.LogWarning("Invalid request body.");
                    return BadRequest(new { Message = "Invalid request body." });
                }

                if (dto.CreatedAt == default(DateTime))
                {
                    dto.CreatedAt = DateTime.UtcNow;
                }

                _logger.LogInformation("Received category data: {@Category}", dto);

                var category = new IncomeCategory
                {
                    Name = dto.Name,
                    UserId = dto.UserId,
                    CreatedAt = dto.CreatedAt,
                };

                _logger.LogInformation("Creating category with Name: {Name} for user ID: {UserId}", category.Name, dto.UserId);

                var result = await _incomeService.CreateCategoryAsync(category);

                if (result)
                {
                    _logger.LogInformation("Successfully created category with Name: {Name} for user ID: {UserId}", category.Name, dto.UserId);
                    return Ok(new { Message = "Category created" });
                }
                else
                {
                    _logger.LogWarning("Failed to create category with Name: {Name} for user ID: {UserId}", category.Name, dto.UserId);
                    return BadRequest(new { Message = "Failed to create category" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating category.");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        [HttpPost("createIncomeItem")]
        //[Authorize]
        public async Task<IActionResult> CreateIncomeItem([FromBody] CreateItem dto)
        {
            try
            {
                _logger.LogInformation("Создание элемента дохода: {Name} для категории {CategoryName}", dto.Name, dto.CategoryName);

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Не удалось извлечь UserId из токена");
                    return Unauthorized(new { Message = "Недействительный токен" });
                }

                var incomeItem = new IncomeItem
                {
                    Name = dto.Name,
                    Amount = dto.Amount,
                    TransactionDate = dto.TransactionDate != default ? dto.TransactionDate : DateTime.UtcNow
                };

                var result = await _incomeService.CreateItemAsync(incomeItem, userId, dto.CategoryName);

                if (result)
                {
                    _logger.LogInformation("Элемент дохода {Name} успешно создан для категории {CategoryName}", dto.Name, dto.CategoryName);
                    return Ok(new { Message = "Элемент дохода успешно создан" });
                }
                else
                {
                    _logger.LogWarning("Не удалось создать элемент дохода {Name} для категории {CategoryName}", dto.Name, dto.CategoryName);
                    return BadRequest(new { Message = "Не удалось создать элемент дохода" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании элемента дохода: {Name}", dto.Name);
                return StatusCode(500, new { Message = "Внутренняя ошибка сервера" });
            }
        }


        [HttpPut("updateCategory/{id}")]
        //[Authorize]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategory dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Name))
                {
                    _logger.LogWarning("Missing name for category update with ID: {Id}", id);
                    return BadRequest("Missing name for category update.");
                }

                _logger.LogInformation("Updating category with ID: {Id} and Name: {Name}", id, dto.Name);

                var result = await _incomeService.UpdateCategoryAsync(id, dto.Name);

                if (result)
                {
                    _logger.LogInformation("Successfully updated category with ID: {Id}", id);
                    return Ok(new { Message = "Category updated" });
                }
                else
                {
                    _logger.LogWarning("Failed to update category with ID: {Id}", id);
                    return BadRequest(new { Message = "Failed to update category" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating category with ID: {Id}", id);
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPut("updateItem/{id}")]
        //[Authorize]
        public async Task<IActionResult> UpdateIncomeItem(int id, [FromBody] UpdateItem dto)
        {
            try
            {
                _logger.LogInformation("Updating income item with ID: {Id}, Name: {Name}", id, dto.Name);

                var incomeItem = new IncomeItem
                {
                    Id = id,
                    Name = dto.Name,
                    Amount = dto.Amount
                };

                var result = await _incomeService.UpdateItemAsync(incomeItem);

                if (result)
                {
                    _logger.LogInformation("Successfully updated income item with ID: {Id}", id);
                    return Ok(new { Message = "Income item updated" });
                }
                else
                {
                    _logger.LogWarning("Failed to update income item with ID: {Id}", id);
                    return BadRequest(new { Message = "Failed to update income item" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating income item with ID: {Id}", id);
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpDelete("deleteCategory/{id}")]
        //[Authorize]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                _logger.LogInformation("Deleting category with ID: {Id}", id);

                var result = await _incomeService.DeleteCategoryAsync(id);

                if (result)
                {
                    _logger.LogInformation("Successfully deleted category with ID: {Id}", id);
                    return Ok(new { Message = "Category deleted" });
                }
                else
                {
                    _logger.LogWarning("Failed to delete category with ID: {Id}", id);
                    return BadRequest(new { Message = "Failed to delete category" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting category with ID: {Id}", id);
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpDelete("deleteItem/{id}")]
        //[Authorize]
        public async Task<IActionResult> DeleteItem(int id)
        {
            try
            {
                _logger.LogInformation("Deleting income item with ID: {Id}", id);

                var result = await _incomeService.DeleteItemAsync(id);

                if (result)
                {
                    _logger.LogInformation("Successfully deleted income item with ID: {Id}", id);
                    return Ok(new { Message = "Income item deleted" });
                }
                else
                {
                    _logger.LogWarning("Failed to delete income item with ID: {Id}", id);
                    return BadRequest(new { Message = "Failed to delete income item" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting income item with ID: {Id}", id);
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}

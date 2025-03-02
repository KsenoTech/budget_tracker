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
    
    public class ExpenseController : Controller
    {
        private readonly IExpenseService _expenseCategoryService;
        private readonly ILogger<ExpenseController> _logger;

        public ExpenseController(IExpenseService iexpenseCategoryService, ILogger<ExpenseController> logger)
        {
            _expenseCategoryService = iexpenseCategoryService;
            _logger = logger;
        }


        #region Category

            [Authorize]
            [HttpGet("getAllForOneUserByEmail")]
            public async Task<ActionResult<List<ExpenseCategory>>> GetExpenseCategories(string email)
            {
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("Email не указан");
                    return BadRequest(new { Message = "Email не указан" });
                }

                var incomeCategories = await _expenseCategoryService.GetCategoriesByEmailAsync(email);
                return Ok(incomeCategories);
            }


            [Authorize]
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

                    // Преобразуем DTO в полноценную модель
                    var category = new ExpenseCategory
                    {
                        Name = dto.Name,
                        UserId = dto.UserId,
                        CreatedAt = dto.CreatedAt,
                    };

                    _logger.LogInformation("Creating category with Name: {Name} for user ID: {UserId}", category.Name, dto.UserId);

                    var result = await _expenseCategoryService.CreateCategoryAsync(category);

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


            [HttpPut("updateCategory/{id}")]
            [Authorize]
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

                    var result = await _expenseCategoryService.UpdateCategoryAsync(id, dto.Name);

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


            [HttpDelete("deleteCategory/{id}")]
            [Authorize]
            public async Task<IActionResult> DeleteCategory(int id)
            {
                try
                {
                    _logger.LogInformation("Deleting category with ID: {Id}", id);

                    var result = await _expenseCategoryService.DeleteCategoryAsync(id);

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

        #endregion


        #region LIMITS
            [HttpGet("getCategoryLimits")]
            [Authorize]
            public async Task<ActionResult<List<CategoryLimit>>> GetCategoryLimits()
            {
                try
                {
                    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userId))
                    {
                        _logger.LogWarning("Не удалось извлечь UserId из токена");
                        return Unauthorized(new { Message = "Недействительный токен" });
                    }

                    var limits = await _expenseCategoryService.GetAllCategoryLimitsAsync(userId);
                    return Ok(limits);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при получении лимитов категорий");
                    return StatusCode(500, new { Message = "Внутренняя ошибка сервера" });
                }
            }

            [HttpPost("createCategoryLimit")]
            [Authorize]
            public async Task<IActionResult> CreateCategoryLimit([FromBody] CategoryLimitDTO dto)
            {
                try
                {
                    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userId))
                    {
                        _logger.LogWarning("Не удалось извлечь UserId из токена");
                        return Unauthorized(new { Message = "Недействительный токен" });
                    }

                    var categoryLimit = new CategoryLimit
                    {
                        ExpenseCategoryId = dto.CategoryId,
                        LimitAmount = dto.LimitAmount,
                        StartDate = dto.StartDate,
                        EndDate = dto.EndDate
                    };

                    var result = await _expenseCategoryService.CreateCategoryLimitAsync(categoryLimit);
                    if (result)
                    {
                        _logger.LogInformation("Лимит для категории {CategoryId} успешно создан", categoryLimit.ExpenseCategoryId);
                        return Ok(new { Message = "Лимит успешно создан" });
                    }

                    return BadRequest(new { Message = "Не удалось создать лимит" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при создании лимита для категории {CategoryId}", dto.CategoryId);
                    return StatusCode(500, new { Message = "Внутренняя ошибка сервера" });
                }
            }


            [HttpPut("updateCategoryLimit/{id}")]
            [Authorize]
            public async Task<IActionResult> UpdateCategoryLimit(int id, [FromBody] CategoryLimitDTO dto)
            {
                try
                {
                    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userId))
                    {
                        _logger.LogWarning("Не удалось извлечь UserId из токена");
                        return Unauthorized(new { Message = "Недействительный токен" });
                    }

                    var categoryLimit = new CategoryLimit
                    {
                        Id = id,
                        ExpenseCategoryId = dto.CategoryId,
                        LimitAmount = dto.LimitAmount,
                        StartDate = dto.StartDate,
                        EndDate = dto.EndDate
                    };

                    var result = await _expenseCategoryService.UpdateCategoryLimitAsync(categoryLimit);
                    if (result)
                    {
                        _logger.LogInformation("Лимит с ID {Id} успешно обновлён", id);
                        return Ok(new { Message = "Лимит успешно обновлён" });
                    }

                    return BadRequest(new { Message = "Не удалось обновить лимит" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при обновлении лимита с ID {Id}", id);
                    return StatusCode(500, new { Message = "Внутренняя ошибка сервера" });
                }
            }


            [HttpDelete("deleteCategoryLimit/{id}")]
            [Authorize]
            public async Task<IActionResult> DeleteCategoryLimit(int id)
            {
                try
                {
                    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userId))
                    {
                        _logger.LogWarning("Не удалось извлечь UserId из токена");
                        return Unauthorized(new { Message = "Недействительный токен" });
                    }

                    var result = await _expenseCategoryService.DeleteCategoryLimitAsync(id);
                    if (result)
                    {
                        _logger.LogInformation("Лимит с ID {Id} успешно удалён", id);
                        return Ok(new { Message = "Лимит успешно удалён" });
                    }

                    return BadRequest(new { Message = "Не удалось удалить лимит" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при удалении лимита с ID {Id}", id);
                    return StatusCode(500, new { Message = "Внутренняя ошибка сервера" });
                }
            }

        #endregion





        #region Item
        [HttpPost("createExpenseItem")]
        [Authorize]
        public async Task<IActionResult> CreateExpenseItem([FromBody] CreateItem dto)
        {
            try
            {
                _logger.LogInformation("Создание элемента расхода: {Name} для категории {CategoryName}", dto.Name, dto.CategoryName);

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Не удалось извлечь UserId из токена");
                    return Unauthorized(new { Message = "Недействительный токен" });
                }

                var expenseItem = new ExpenseItem
                {
                    Name = dto.Name,
                    Amount = dto.Amount,
                    TransactionDate = dto.TransactionDate != default ? dto.TransactionDate : DateTime.UtcNow
                };

                var (success, errorMessage) = await _expenseCategoryService.CreateExpenseItemAsync(expenseItem, userId, dto.CategoryName);

                if (success)
                {
                    _logger.LogInformation("Элемент расхода {Name} успешно создан для категории {CategoryName}", dto.Name, dto.CategoryName);
                    return Ok(new { Message = "Элемент расхода успешно создан" });
                }
                else
                {
                    _logger.LogWarning("Не удалось создать элемент расхода {Name} для категории {CategoryName}: {Error}", dto.Name, dto.CategoryName, errorMessage);
                    return BadRequest(new { Message = errorMessage });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Неожиданная ошибка при создании элемента расхода: {Name}", dto.Name);
                return StatusCode(500, new { Message = "Внутренняя ошибка сервера" });
            }
        }


        [HttpPut("updateItem/{id}")]
            [Authorize]
            public async Task<IActionResult> UpdateExpenseItem(int id, [FromBody] UpdateItem dto)
            {
                try
                {
                    _logger.LogInformation("Обновление элемента расхода с ID: {Id}, Name: {Name}", id, dto.Name);

                    var expenseItem = new ExpenseItem
                    {
                        Id = id,
                        Name = dto.Name,
                        Amount = dto.Amount
                    };

                    var result = await _expenseCategoryService.UpdateItemAsync(expenseItem);

                    if (result)
                    {
                        _logger.LogInformation("Элемент расхода с ID: {Id} успешно обновлен", id);
                        return Ok(new { Message = "Элемент расхода обновлен" });
                    }
                    else
                    {
                        _logger.LogWarning("Не удалось обновить элемент расхода с ID: {Id}", id);
                        return BadRequest(new { Message = "Не удалось обновить элемент расхода" });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при обновлении элемента расхода с ID: {Id}", id);
                    return StatusCode(500, new { Message = "Внутренняя ошибка сервера" });
                }
            }

        
            [HttpDelete("deleteItem/{id}")]
            [Authorize]
            public async Task<IActionResult> DeleteItem(int id)
            {
                try
                {
                    _logger.LogInformation("Deleting category with ID: {Id}", id);

                    var result = await _expenseCategoryService.DeleteItemAsync(id);

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

        #endregion
    }
}

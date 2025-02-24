using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using server.ApplicationCore.DomModels;
using server.ApplicationCore.Interfaces.Services;
using System.Security.Claims;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]
    
    public class ExpenseController : Controller
    {
        private readonly IExpenseCategoryService _expenseCategoryService;
        private readonly ILogger<ExpenseController> _logger;

        public ExpenseController(IExpenseCategoryService iexpenseCategoryService, ILogger<ExpenseController> logger)
        {
            _expenseCategoryService = iexpenseCategoryService;
            _logger = logger;
        }

        [HttpGet("getAllForOneUserByEmail")]
        public async Task<ActionResult<List<ExpenseCategory>>> GetIncomeCategories(string email)
        {
            var incomeCategories = await _expenseCategoryService.GetCategoriesByEmailAsync(email);
            return Ok(incomeCategories);
        }



        [HttpGet("debugClaims")]
        public IActionResult DebugClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);
        }

        //[Authorize]
        [HttpGet("getAll1")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(userEmail))
                {
                    _logger.LogWarning("User ID not found in claims.");
                    return BadRequest("User ID not found in claims.");
                }

                _logger.LogInformation("Fetching categories for user ID: {UserId}", userEmail);

                var categories = await _expenseCategoryService.GetCategoriesByEmailAsync(userEmail);

                if (categories == null || !categories.Any())
                {
                    _logger.LogWarning("No categories found for user with ID: {UserId}", userEmail);
                    return NotFound("No categories found for the user.");
                }

                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching categories for user.");
                return StatusCode(500, "Internal server error.");
            }
        }

        //[Authorize]
        [HttpPost("сreate")]
        public async Task<IActionResult> CreateCategory([FromBody] ExpenseCategory category)
        {

            //category.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //var result = await _incomeCategoryService.CreateCategoryAsync(category);
            //return result ? Ok(new { Message = "Category created" }) : BadRequest(new { Message = "Failed to create category" });
            try
            {
                var userId = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User ID not found in claims.");
                    return BadRequest("User ID not found in claims.");
                }

                category.UserId = userId;
                _logger.LogInformation("Creating category with Name: {Name} for user ID: {UserId}", category.Name, userId);

                var result = await _expenseCategoryService.CreateCategoryAsync(category);

                if (result)
                {
                    _logger.LogInformation("Successfully created category with Name: {Name} for user ID: {UserId}", category.Name, userId);
                    return Ok(new { Message = "Category created" });
                }
                else
                {
                    _logger.LogWarning("Failed to create category with Name: {Name} for user ID: {UserId}", category.Name, userId);
                    return BadRequest(new { Message = "Failed to create category" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating category.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPut("update")]
        //[Authorize]
        public async Task<IActionResult> UpdateCategory([FromBody] ExpenseCategory category)
        {
            //var result = await _incomeCategoryService.UpdateCategoryAsync(category);
            //return result ? Ok(new { Message = "Category updated" }) : BadRequest(new { Message = "Failed to update category" });
            try
            {
                if (category == null || string.IsNullOrEmpty(category.UserId))
                {
                    _logger.LogWarning("Invalid or missing data for category update.");
                    return BadRequest("Invalid or missing data for category update.");
                }

                _logger.LogInformation("Updating category with ID: {Id} and Name: {Name}", category.Id, category.Name);

                var result = await _expenseCategoryService.UpdateCategoryAsync(category);

                if (result)
                {
                    _logger.LogInformation("Successfully updated category with ID: {Id}", category.Id);
                    return Ok(new { Message = "Category updated" });
                }
                else
                {
                    _logger.LogWarning("Failed to update category with ID: {Id}", category.Id);
                    return BadRequest(new { Message = "Failed to update category" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating category.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpDelete("delete/{id}")]
        //[Authorize]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            //var result = await _incomeCategoryService.DeleteCategoryAsync(id);
            //return result ? Ok(new { Message = "Category deleted" }) : BadRequest(new { Message = "Failed to delete category" });
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
    }
}

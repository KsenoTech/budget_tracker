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
    public class IncomeCategoryController : Controller
    {
        private readonly IIncomeCategoryService _incomeCategoryService;

        public IncomeCategoryController(IIncomeCategoryService incomeCategoryService)
        {
            _incomeCategoryService = incomeCategoryService;
        }

        //[Authorize]
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllCategories()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID not found in claims.");
            }

            Console.WriteLine($"Fetching categories for user ID: {userId}");

            var categories = await _incomeCategoryService.GetCategoriesByUserAsync(userId);

            if (categories == null || !categories.Any())
            {
                return NotFound("No categories found for the user.");
            }

            return Ok(categories);
        }

        //[Authorize]
        [HttpPost("сreate")]
        public async Task<IActionResult> CreateCategory([FromBody] IncomeCategory category)
        {
            category.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _incomeCategoryService.CreateCategoryAsync(category);
            return result ? Ok(new { Message = "Category created" }) : BadRequest(new { Message = "Failed to create category" });
        }

        [HttpGet("update")]
        //[Authorize]
        public async Task<IActionResult> UpdateCategory([FromBody] IncomeCategory category)
        {
            var result = await _incomeCategoryService.UpdateCategoryAsync(category);
            return result ? Ok(new { Message = "Category updated" }) : BadRequest(new { Message = "Failed to update category" });
        }

        [HttpDelete("delete{id}")]
        //[Authorize]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _incomeCategoryService.DeleteCategoryAsync(id);
            return result ? Ok(new { Message = "Category deleted" }) : BadRequest(new { Message = "Failed to delete category" });
        }
    }
}

using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.UniFarm.API.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : BaseController
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("api/v1/categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var response = await _categoryService.GetAllCategories();
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [HttpGet("api/v1/category/{id}")]
        public async Task<IActionResult> GetCategoryById(Guid categoryId)
        {
            var response = await _categoryService.GetCategoryById(categoryId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [HttpPost("api/v1/category")]
        public async Task<IActionResult> CreateCategory(CategoryRequest categoryRequest)
        {
            if (ModelState.IsValid)
            {
                var response = await _categoryService.CreateCategory(categoryRequest);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [HttpPut("api/v1/category/{id}")]
        public async Task<IActionResult> UpdateCategory(Guid categoryId, CategoryRequestUpdate categoryRequestUpdate)
        {
            if (ModelState.IsValid)
            {
                var response = await _categoryService.UpdateCategory(categoryId, categoryRequestUpdate);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [HttpDelete("api/v1/category/{id}")]
        public async Task<IActionResult> DeleteCategory(Guid categoryId)
        {
            var response = await _categoryService.DeleteCategory(categoryId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}

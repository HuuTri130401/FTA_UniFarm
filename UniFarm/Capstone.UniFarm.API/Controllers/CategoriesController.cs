using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.UniFarm.API.Controllers
{
    [Route("api/[controller]")]
    public class CategoriesController : BaseController
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var response = await _categoryService.GetAllCategories();
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [HttpGet("categoryId")]
        public async Task<IActionResult> GetCategoryById(Guid categoryId)
        {
            var response = await _categoryService.GetCategoryById(categoryId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryRequest categoryRequest)
        {
            if (ModelState.IsValid)
            {
                var response = await _categoryService.CreateCategory(categoryRequest);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [HttpPut("{categoryId}")]
        public async Task<IActionResult> UpdateCategory(Guid categoryId, CategoryRequestUpdate categoryRequestUpdate)
        {
            if (ModelState.IsValid)
            {
                var response = await _categoryService.UpdateCategory(categoryId, categoryRequestUpdate);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [HttpDelete("categoryId")]
        public async Task<IActionResult> DeleteCategory(Guid categoryId)
        {
            var response = await _categoryService.DeleteCategory(categoryId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}

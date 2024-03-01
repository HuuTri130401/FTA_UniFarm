using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers
{
    [ApiController]
    public class CategoriesController : BaseController
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [SwaggerOperation(Summary = "Get All Categories - Admin Role & FarmHub Role - {Huu Tri}")]
        [HttpGet("categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var response = await _categoryService.GetAllCategories();
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "Get Categories Recommends - Customer Role - {Huu Tri}")]
        [HttpGet("categories-recommends")]
        public async Task<IActionResult> GetAllCategorieForCustomers()
        {
            var response = await _categoryService.GetAllCategoriesForCustomer();
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "Get Category By Id - Admin Role & FarmHub Role - {Huu Tri}")]
        [HttpGet("category/{id}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var response = await _categoryService.GetCategoryById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "Create Category - Admin Role - {Huu Tri}")]
        [HttpPost("category")]
        public async Task<IActionResult> CreateCategory(CategoryRequest categoryRequest)
        {
            if (ModelState.IsValid)
            {
                var response = await _categoryService.CreateCategory(categoryRequest);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [SwaggerOperation(Summary = "Update Category - Admin Role - {Huu Tri}")]
        [HttpPut("category/{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, CategoryRequestUpdate categoryRequestUpdate)
        {
            if (ModelState.IsValid)
            {
                var response = await _categoryService.UpdateCategory(id, categoryRequestUpdate);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [SwaggerOperation(Summary = "InActive Category - Admin Role - {Huu Tri}")]
        [HttpDelete("category/{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var response = await _categoryService.DeleteCategory(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}

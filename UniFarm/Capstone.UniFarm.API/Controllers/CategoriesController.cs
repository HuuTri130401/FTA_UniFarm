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
        public async Task<IActionResult> GetAllCategory()
        {
            var response = await _categoryService.GetAllCategories();
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }
        
        /*[HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var response = await _categoryService.GetCategory(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);

            if (categoryId == 0)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                return BadRequest(_apiResponse);
            }
            var category = await _categoryService.GetCategoryById(categoryId);
            if (category == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Can not found Category!");
                return NotFound(_apiResponse);
            }
            var categoryResponse = _mapper.Map<CategoryResponse>(category);
            _apiResponse.Result = categoryResponse;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }*/

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryRequest requestModel)
        {
            if (ModelState.IsValid)
            {
                var response = await _categoryService.CreateCategory(requestModel);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
            }
            return BadRequest("Model is invalid");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, CategoryRequest requestModel)
        {
            if (ModelState.IsValid)
            {
                var response = await _categoryService.UpdateCategory(id, requestModel);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
            }
            return BadRequest("Model is invalid");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveCategory(Guid id)
        {
            var response = await _categoryService.DeleteCategory(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
        }
    }
}

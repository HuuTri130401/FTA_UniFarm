using AutoMapper;
using Capstone.UniFarm.API.Helpers;
using Capstone.UniFarm.API.ViewModels.ModelRequests;
using Capstone.UniFarm.API.ViewModels.ModelResponses;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Domain.Specifications;
using Capstone.UniFarm.Services.ICustomServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Capstone.UniFarm.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoriesController> _logger;
        private ApiResponse _apiResponse;

        public CategoriesController(ICategoryService categoryService, IMapper mapper, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _mapper = mapper;
            _logger = logger;
            _apiResponse = new ApiResponse();
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoryList()
        {
            _logger.LogInformation("CategoriesController: Get method called");
            var categoryList = await _categoryService.GetAllCategories();
            var resonsecategoryList = _mapper.Map<List<CategoryResponse>>(categoryList);

            if (resonsecategoryList == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                _apiResponse.IsSuccess = false;
                return NotFound(_apiResponse);
            }
            _apiResponse.Result = resonsecategoryList;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }

        [HttpGet("{categoryId}")]
        public async Task<IActionResult> GetCategoryById(int categoryId)
        {
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
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromForm] CategoryRequest categoryRequest)
        {
            var category = _mapper.Map<Category>(categoryRequest);
            var isCategoryCreated = await _categoryService.CreateCategory(category);
            _apiResponse.Result = isCategoryCreated;
            _apiResponse.StatusCode = HttpStatusCode.Created;
            return Ok(_apiResponse);
        }

        [HttpPut("{categoryId}")]
        public async Task<IActionResult> UpdateCategory(int categoryId, [FromForm] CategoryRequestUpdate categoryRequestUpdate)
        {
            var existingCategory = await _categoryService.GetCategoryById(categoryId);
            if (existingCategory == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                return NotFound(_apiResponse);
            }
            existingCategory = _mapper.Map<Category>(categoryRequestUpdate);
            existingCategory.Id = categoryId;

            var isCategoryUpdated = await _categoryService.UpdateCategory(existingCategory);
            _apiResponse.Result = isCategoryUpdated;
            _apiResponse.StatusCode= HttpStatusCode.NoContent;
            return Ok(_apiResponse);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            if(categoryId == 0)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                return BadRequest();
            }

            var isCategoryDeleted = await _categoryService.DeleteCategory(categoryId);
            if (isCategoryDeleted == false)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                return BadRequest();
            }
            _apiResponse.Result = isCategoryDeleted;
            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.NoContent;
            return Ok(_apiResponse);
        }
    }
}

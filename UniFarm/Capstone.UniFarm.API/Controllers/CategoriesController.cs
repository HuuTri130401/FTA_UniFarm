using AutoMapper;
using Capstone.UniFarm.API.Helpers;
using Capstone.UniFarm.API.ViewModels.ModelResponses;
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
        public async Task<IActionResult> GetProductList()
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
    }
}

using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.CustomServices;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.UniFarm.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImagesController : BaseController
    {
        private readonly IProductImageService _productImageService;

        public ProductImagesController(IProductImageService productImageService)
        {
            _productImageService = productImageService;
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetAllProductImagesByProductId(Guid productId)
        {
            var response = await _productImageService.GetAllProductImagesByProductId(productId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductImage(Guid productId, ProductImageRequest productImageRequest)
        {
            if (ModelState.IsValid)
            {
                var response = await _productImageService.CreateProductImage(productId, productImageRequest);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [HttpPut("{productImageId}")]
        public async Task<IActionResult> UpdateProductImage(Guid productImageId, ProductImageRequestUpdate productImageRequestUpdate)
        {
            if (ModelState.IsValid)
            {
                var response = await _productImageService.UpdateProductImage(productImageId, productImageRequestUpdate);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }
    }
}

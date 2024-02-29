using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.CustomServices;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.UniFarm.API.Controllers
{
    [ApiController]
    public class ProductImagesController : BaseController
    {
        private readonly IProductImageService _productImageService;

        public ProductImagesController(IProductImageService productImageService)
        {
            _productImageService = productImageService;
        }

        [HttpGet("product/{id}/product-images")]
        public async Task<IActionResult> GetAllProductImagesByProductId(Guid id)
        {
            var response = await _productImageService.GetAllProductImagesByProductId(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [HttpGet("product-image/{id}")]
        public async Task<IActionResult> GetProductImageById(Guid id)
        {
            var response = await _productImageService.GetProductImageById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [HttpPost("product/{id}/product-image")]
        public async Task<IActionResult> CreateProductImageForProduct(Guid id, ProductImageRequest productImageRequest)
        {
            if (ModelState.IsValid)
            {
                var response = await _productImageService.CreateProductImage(id, productImageRequest);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [HttpPut("product-image/{id}")]
        public async Task<IActionResult> UpdateProductImage(Guid id, ProductImageRequestUpdate productImageRequestUpdate)
        {
            if (ModelState.IsValid)
            {
                var response = await _productImageService.UpdateProductImage(id, productImageRequestUpdate);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [HttpDelete("product-image/{id}")]
        public async Task<IActionResult> DeleteProductImage(Guid id)
        {
            var response = await _productImageService.DeleteProductImage(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}

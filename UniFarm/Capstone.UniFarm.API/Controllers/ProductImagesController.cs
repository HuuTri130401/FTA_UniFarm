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

        [HttpGet("/api/v1/products/{productId}/productImages")]
        public async Task<IActionResult> GetAllProductImagesByProductId(Guid productId)
        {
            var response = await _productImageService.GetAllProductImagesByProductId(productId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [HttpGet("/api/v1/productImage/{productImageId}")]
        public async Task<IActionResult> GetProductImageById(Guid productImageId)
        {
            var response = await _productImageService.GetProductImageById(productImageId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [HttpPost("/api/v1/products/{productId}/productImage")]
        public async Task<IActionResult> CreateProductImageForProduct(Guid productId, ProductImageRequest productImageRequest)
        {
            if (ModelState.IsValid)
            {
                var response = await _productImageService.CreateProductImage(productId, productImageRequest);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [HttpPut("/api/v1/productImage/{productImageId}")]
        public async Task<IActionResult> UpdateProductImage(Guid productImageId, ProductImageRequestUpdate productImageRequestUpdate)
        {
            if (ModelState.IsValid)
            {
                var response = await _productImageService.UpdateProductImage(productImageId, productImageRequestUpdate);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [HttpDelete("/api/v1/productImage/{productImageId}")]
        public async Task<IActionResult> DeleteProductImage(Guid productImageId)
        {
            var response = await _productImageService.DeleteProductImage(productImageId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}

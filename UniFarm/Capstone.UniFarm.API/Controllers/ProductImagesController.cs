using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.CustomServices;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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

        [SwaggerOperation(Summary = "Get All ProductItem Images By Product Item Id - ADMIN, FARMHUB, CUSTOMER - {Huu Tri}")]
        [HttpGet("product-item/{id}/product-images")]
        public async Task<IActionResult> GetAllProductItemImagesByProductItemId(Guid id)
        {
            var response = await _productImageService.GetAllProductImagesByProductItemId(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "Get ProductItem Images By Id - ADMIN, FARMHUB - {Huu Tri}")]
        [HttpGet("product-image/{id}")]
        public async Task<IActionResult> GetProductItemImagesById(Guid id)
        {
            var response = await _productImageService.GetProductImageById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "Create Product Item Image For Product Item - FARMHUB - {Huu Tri}")]
        [HttpPost("product-item/{id}/product-image")]
        public async Task<IActionResult> CreateProductItemImageForProductItem(Guid id, [FromForm] ProductImageRequest productImageRequest)
        {
            if (ModelState.IsValid)
            {
                var response = await _productImageService.CreateProductImage(id, productImageRequest);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [SwaggerOperation(Summary = "Update Product Item Image - FARMHUB - {Huu Tri}")]
        [HttpPut("product-image/{id}")]
        public async Task<IActionResult> UpdateProductItemImage(Guid id, [FromForm] ProductImageRequestUpdate productImageRequestUpdate)
        {
            if (ModelState.IsValid)
            {
                var response = await _productImageService.UpdateProductImage(id, productImageRequestUpdate);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [SwaggerOperation(Summary = "Inactive Product Item Image - ADMIN, FARMHUB - {Huu Tri}")]
        [HttpDelete("product-image/{id}")]
        public async Task<IActionResult> DeleteProductItemImage(Guid id)
        {
            var response = await _productImageService.DeleteProductImage(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}

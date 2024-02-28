using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.UniFarm.API.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class ProductsController : BaseController
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("api/v1/products")]
        public async Task<IActionResult> GetAllProducts()
        {
            var response = await _productService.GetAllProducts();
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [HttpGet("api/v1/product/{id}")]
        public async Task<IActionResult> GetProductById(Guid productId)
        {
            var response = await _productService.GetProductById(productId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [HttpPost("api/v1/product/{id}")]
        public async Task<IActionResult> CreateProduct(ProductRequest productRequest)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.CreateProduct(productRequest);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [HttpPut("api/v1/product/{id}")]
        public async Task<IActionResult> UpdateProduct(Guid productId, ProductRequest productRequest)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.UpdateProduct(productId, productRequest);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [HttpDelete("api/v1/product/{id}")]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            var response = await _productService.DeleteProduct(productId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}

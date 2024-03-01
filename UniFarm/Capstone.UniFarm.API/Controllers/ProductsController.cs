using Capstone.UniFarm.Services.CustomServices;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers
{
    [ApiController]
    public class ProductsController : BaseController
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [SwaggerOperation(Summary = "Get All Products - Admin, FarmHub, Customer Role - {Huu Tri}")]
        [HttpGet("products")]
        public async Task<IActionResult> GetAllProducts()
        {
            var response = await _productService.GetAllProducts();
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "Get Products By CategoryId - Customer Role - {Huu Tri}")]
        [HttpGet("category/{id}/products")]
        public async Task<IActionResult> GetAllProductsByCategoryId(Guid id)
        {
            var response = await _productService.GetAllProductsByCategoryId(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "Get Product By Id - Admin Role - {Huu Tri}")]
        [HttpGet("product/{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var response = await _productService.GetProductById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "Create Product - Admin Role - {Huu Tri}")]
        [HttpPost("product")]
        public async Task<IActionResult> CreateProduct(ProductRequest productRequest)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.CreateProduct(productRequest);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [SwaggerOperation(Summary = "Update Product - Admin Role - {Huu Tri}")]
        [HttpPut("product/{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, ProductRequestUpdate productRequestUpdate)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.UpdateProduct(id, productRequestUpdate);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [SwaggerOperation(Summary = "Delete Product - Admin Role - {Huu Tri}")]
        [HttpDelete("product/{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var response = await _productService.DeleteProduct(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}

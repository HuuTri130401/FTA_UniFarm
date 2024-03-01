using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers
{
    [ApiController]
    public class ProductItemsController : BaseController
    {
        private readonly IProductItemService _productItemService;

        public ProductItemsController(IProductItemService productItemService)
        {
            _productItemService = productItemService;
        }

        [SwaggerOperation(Summary = "Get All ProductItems By Product Id - Customer Role - {Huu Tri}")]
        [HttpGet("product/{id}/product-items")]
        public async Task<IActionResult> GetAllProductItemsByProductId(Guid id)
        {
            var response = await _productItemService.GetAllProductItemsByProductId(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "Get ProductItem ById - Customer Role - {Huu Tri}")]
        [HttpGet("product-item/{id}")]
        public async Task<IActionResult> GetProductItemById(Guid id)
        {
            var response = await _productItemService.GetProductItemById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "Create Product Item For Product - FarmHub Role - {Huu Tri}")]
        [HttpPost("product/{id}/product-item")]
        public async Task<IActionResult> CreateProductItemForProduct(Guid id, ProductItemRequest productItemRequest)
        {
            if (ModelState.IsValid)
            {
                var response = await _productItemService.CreateProductItemForProduct(id, productItemRequest);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [SwaggerOperation(Summary = "Update Product Item - FarmHub Role - {Huu Tri}")]
        [HttpPut("product-item/{id}")]
        public async Task<IActionResult> UpdateProductItem(Guid id, ProductItemRequestUpdate productItemRequestUpdate)
        {
            if (ModelState.IsValid)
            {
                var response = await _productItemService.UpdateProductItem(id, productItemRequestUpdate);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [SwaggerOperation(Summary = "Update Product Item - Admin Role, FarmHub Role - {Huu Tri}")]
        [HttpDelete("product-item/{id}")]
        public async Task<IActionResult> DeleteProductItem(Guid id)
        {
            var response = await _productItemService.DeleteProductItem(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}

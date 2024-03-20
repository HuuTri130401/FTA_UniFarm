using Capstone.UniFarm.Repositories.RequestFeatures;
using Capstone.UniFarm.Services.CustomServices;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers
{
    [ApiController]
    public class ProductItemsController : BaseController
    {
        private readonly IProductItemService _productItemService;
        private readonly IAccountService _accountService;

        public ProductItemsController(IProductItemService productItemService, IAccountService accountService)
        {
            _productItemService = productItemService;
            _accountService = accountService;
        }

        [SwaggerOperation(Summary = "Get All ProductItems By Product Id - Admin, Customer Role - {Huu Tri}")]
        [HttpGet("product/{id}/product-items")]
        public async Task<IActionResult> GetAllProductItemsByProductId(Guid id)
        {
            var response = await _productItemService.GetAllProductItemsByProductId(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "Search ProductItems - {Huu Tri}")]
        [HttpGet("product-items/search")]
        public async Task<IActionResult> SearchProductItems([FromQuery] ProductItemParameters productItemParameters)
        {
            var response = await _productItemService.SearchProductItems(productItemParameters);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "Get All Product Items In FarmHub - FarmHub Role - {Huu Tri}")]
        [HttpGet("product-items")]
        [Authorize(Roles = "FarmHub")]
        public async Task<IActionResult> GetAllProductItemsInFarmHub()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader))
            {
                return Unauthorized();
            }

            // The token is prefixed with "Bearer ", so we need to remove that prefix
            string token = authHeader.Replace("Bearer ", "");

            var defineUser = _accountService.GetIdAndRoleFromToken(token);
            if (defineUser.Payload == null)
            {
                return HandleErrorResponse(defineUser!.Errors);
            }
            var farmHubAccountId = defineUser.Payload.Id;
            var response = await _productItemService.GetAllProductItemsByFarmHubAccountId(farmHubAccountId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "Get Product Item By Id - FarmHub, Customer Role - {Huu Tri}")]
        [HttpGet("product-item/{id}")]
        public async Task<IActionResult> GetProductItemById(Guid id)
        {
            var response = await _productItemService.GetProductItemById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "Create Product Item For Product - FarmHub Role - {Huu Tri}")]
        [HttpPost("product/{id}/product-item")]
        [Authorize(Roles = "FarmHub")]
        public async Task<IActionResult> CreateProductItemForProduct(Guid id, ProductItemRequest productItemRequest)
        {
            if (ModelState.IsValid)
            {
                string authHeader = HttpContext.Request.Headers["Authorization"];
                if (string.IsNullOrEmpty(authHeader))
                {
                    return Unauthorized();
                }

                // The token is prefixed with "Bearer ", so we need to remove that prefix
                string token = authHeader.Replace("Bearer ", "");

                var defineUser = _accountService.GetIdAndRoleFromToken(token);
                if (defineUser.Payload == null)
                {
                    return HandleErrorResponse(defineUser!.Errors);
                }
                var farmHubAccountId = defineUser.Payload.Id;
                var response = await _productItemService.CreateProductItemForProduct(id, farmHubAccountId, productItemRequest);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [SwaggerOperation(Summary = "Update Product Item - FarmHub Role - {Huu Tri}")]
        [HttpPut("product-item/{id}")]
        [Authorize(Roles = "FarmHub")]
        public async Task<IActionResult> UpdateProductItem(Guid id, ProductItemRequestUpdate productItemRequestUpdate)
        {
            if (ModelState.IsValid)
            {
                var response = await _productItemService.UpdateProductItem(id, productItemRequestUpdate);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [SwaggerOperation(Summary = "Delete Product Item - Admin Role, FarmHub Role - {Huu Tri}")]
        [HttpDelete("product-item/{id}")]
        [Authorize(Roles = "FarmHub, Admin")]
        public async Task<IActionResult> DeleteProductItem(Guid id)
        {
            var response = await _productItemService.DeleteProductItem(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}

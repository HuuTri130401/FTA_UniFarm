﻿using Capstone.UniFarm.Services.CustomServices;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers
{
    [ApiController]
    public class ProductItemInMenusController : BaseController
    {
        private readonly IProductItemInMenuService _productItemInMenuService;

        public ProductItemInMenusController(IProductItemInMenuService productItemInMenuService)
        {
            _productItemInMenuService = productItemInMenuService;
        }

        [SwaggerOperation(Summary = "Create Product Item For Menu - FARMHUB - {Huu Tri}")]
        [HttpPost("menu/{id}/product-item")]
        public async Task<IActionResult> CreateProductItemForProduct(Guid id, ProductItemInMenuRequest productItemInMenuRequest)
        {
            if (ModelState.IsValid)
            {
                var response = await _productItemInMenuService.AddProductItemToMenu(id, productItemInMenuRequest);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [SwaggerOperation(Summary = "Get All ProductItems By Menu Id - FARMHUB - {Huu Tri}")]
        [HttpGet("menu/{id}/product-items")]
        public async Task<IActionResult> GetAllProductItemsInMenuByMenuId(Guid id)
        {
            var response = await _productItemInMenuService.GetProductItemsInMenuByMenuId(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "Inactive Product Item from Menu - FARMHUB - {Huu Tri}")]
        [HttpDelete("product-item-in-menu/{id}")]
        public async Task<IActionResult> DeleteProductItemInMenu(Guid id)
        {
            var response = await _productItemInMenuService.RemoveProductItemFromMenu(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}

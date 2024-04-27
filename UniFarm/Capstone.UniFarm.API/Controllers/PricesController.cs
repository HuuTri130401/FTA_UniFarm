using Capstone.UniFarm.Services.CustomServices;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers
{
    [ApiController]
    public class PricesController : BaseController
    {
        private readonly IPriceService _priceService;
        private readonly IPriceItemService _priceItemService;

        public PricesController(IPriceService priceService, IPriceItemService priceItemService)
        {
            _priceService = priceService;
            _priceItemService = priceItemService;
        }

        [SwaggerOperation(Summary = "Get All Price - ADMIN - {Huu Tri}")]
        [HttpGet("prices")]
        public async Task<IActionResult> GetAllPrices()
        {
            var response = await _priceService.GetAllPrice();
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "Update Price - ADMIN - {Huu Tri}")]
        [HttpPut("price/{id}")]
        public async Task<IActionResult> UpdatePrice(Guid id, PriceTableRequestUpdate priceTableRequestUpdate)
        {
            if (ModelState.IsValid)
            {
                var response = await _priceService.UpdatePrice(id, priceTableRequestUpdate);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }        
        
        [SwaggerOperation(Summary = "Update Price Item - ADMIN - {Huu Tri}")]
        [HttpPut("price-item/{priceItemId}")]
        public async Task<IActionResult> UpdatePriceItem(Guid priceItemId, PriceTableItemRequestUpdate priceTableItemRequestUpdate)
        {
            if (ModelState.IsValid)
            {
                var response = await _priceItemService.UpdatePriceItem(priceItemId, priceTableItemRequestUpdate);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }
    }
}

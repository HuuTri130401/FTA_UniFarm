using Capstone.UniFarm.Services.CustomServices;
using Capstone.UniFarm.Services.ICustomServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers
{
    [ApiController]
    public class PricesController : BaseController
    {
        private readonly IPriceService _priceService;

        public PricesController(IPriceService priceService)
        {
            _priceService = priceService;
        }

        [SwaggerOperation(Summary = "Get All Price - ADMIN - {Huu Tri}")]
        [HttpGet("prices")]
        public async Task<IActionResult> GetAllPrices()
        {
            var response = await _priceService.GetAllPrice();
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}

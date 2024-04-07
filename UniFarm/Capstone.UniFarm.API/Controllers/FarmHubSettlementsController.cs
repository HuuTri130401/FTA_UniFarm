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
    public class FarmHubSettlementsController : BaseController
    {
        private readonly ISettlementService _settlementService;

        public FarmHubSettlementsController(ISettlementService settlementService)
        {
            _settlementService = settlementService;
        }

        //[SwaggerOperation(Summary = "TEST SETLEMENT - ADMIN - {Huu Tri}")]
        //[HttpPost("settlement")]
        //public async Task<IActionResult> CreateSettlement(Guid businessDayId)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        await _settlementService.CreateSettlementForBusinessDay(businessDayId);
        //        return Ok();
        //    }
        //    return BadRequest("Model is invalid");
        //}

        //[SwaggerOperation(Summary = "FarmHub Shop Create Settlement - FARMHUB - {Huu Tri}")]
        //[HttpPost("settlement/create-or-update")]
        //public async Task<IActionResult> CreateFarmHub(Guid businessDayId, Guid farmHubId)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var response = await _settlementService.CreateSettlementForFarmHub(businessDayId, farmHubId);
        //        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        //    }
        //    return BadRequest("Model is invalid");
        //}

        [SwaggerOperation(Summary = "FarmHub Shop Get Settlement By FarmHubId and BusinessDayId - FARMHUB - {Huu Tri}")]
        [HttpGet("settlement/businessday/{businessdayId}/farmhub/{farmHubId}")]
        public async Task<IActionResult> GetMenuById(Guid businessdayId, Guid farmHubId)
        {
            var response = await _settlementService.GetSettlementForFarmHub(businessdayId, farmHubId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}

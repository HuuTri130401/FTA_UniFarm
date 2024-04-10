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
        private readonly IAccountService _accountService;

        public FarmHubSettlementsController(ISettlementService settlementService, 
            IAccountService accountService)
        {
            _settlementService = settlementService;
            _accountService = accountService;
        }

        [SwaggerOperation(Summary = "Payment Profit For All FarmHub In BusinessDay - ADMIN - {Huu Tri}")]
        [HttpPost("settlement/payout-for-all-farmhub/businessday/{businessDayId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PaymentProfitForFarmHubInBusinessDay(Guid businessDayId)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader))
            {
                return Unauthorized();
            }
            string token = authHeader.Replace("Bearer ", "");

            var defineUser = _accountService.GetIdAndRoleFromToken(token);
            if (defineUser.Payload == null)
            {
                return HandleErrorResponse(defineUser!.Errors);
            }
            var systemAccountId = defineUser.Payload.Id;
            var response = await _settlementService.PaymentProfitForFarmHubInBusinessDay(businessDayId, systemAccountId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [Authorize(Roles = "Admin, FarmHub")]
        [SwaggerOperation(Summary = "FarmHub Shop Get Settlement By FarmHubId and BusinessDayId - FARMHUB - {Huu Tri}")]
        [HttpGet("settlement/businessday/{businessdayId}/farmhub/{farmHubId}")]
        public async Task<IActionResult> GetMenuById(Guid businessdayId, Guid farmHubId)
        {
            var response = await _settlementService.GetSettlementForFarmHub(businessdayId, farmHubId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}

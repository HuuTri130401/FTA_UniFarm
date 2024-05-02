using Capstone.UniFarm.Repositories.IRepository;
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
    public class BusinessDaysController : BaseController
    {
        private readonly IBusinessDayService _businessDayService;
        private readonly IAccountService _accountService;

        public BusinessDaysController(IBusinessDayService businessDayService, IAccountService accountService)
        {
            _businessDayService = businessDayService;
            _accountService = accountService;
        }

        [SwaggerOperation(Summary = "Get All Business Days - ADMIN, FARMHUB - {Huu Tri}")]
        [HttpGet("business-days")]
        public async Task<IActionResult> GetAllBusinessDays()
        {
            var response = await _businessDayService.GetAllBusinessDays();
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
        
        [SwaggerOperation(Summary = "Get All Business Days - CollectedStaff - Tien")]
        [HttpGet("business-days-for-collected-staff")]
        [Authorize(Roles = "CollectedStaff")]
        public async Task<IActionResult> GetAllBusinessDaysForCollectedStaff()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader))
            {
                return Unauthorized();
            }
            string token = authHeader.Replace("Bearer ", "");
            var defineUser = _accountService.GetIdAndRoleFromToken(token);
            if (defineUser.Payload == null) return HandleErrorResponse(defineUser!.Errors);
            var response = await _businessDayService.GetAllBusinessDaysContainBatchQuantity(Guid.Parse(defineUser.Payload.AuthorizationDecision));
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "Get Business Day By Id - ADMIN - {Huu Tri}")]
        [HttpGet("business-day/{id}")]
        public async Task<IActionResult> GetBusinessDayById(Guid id)
        {
            var response = await _businessDayService.GetBusinessDayById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "FarmHub Get Business Day By Id - FARMHUB - {Huu Tri}")]
        [HttpGet("farmhub/business-day/{id}")]
        [Authorize(Roles = "FarmHub")]
        public async Task<IActionResult> FarmHubGetBusinessDayById(Guid id)
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
            var farmHubAccountId = defineUser.Payload.Id;
            var response = await _businessDayService.FarmHubGetBusinessDayById(farmHubAccountId, id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "Create Business Day - ADMIN - {Huu Tri}")]
        [HttpPost("business-day")]
        public async Task<IActionResult> CreateBusinessDay(BusinessDayRequest businessDayRequest)
        {
            if (ModelState.IsValid)
            {
                var response = await _businessDayService.CreateBusinessDay(businessDayRequest);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [SwaggerOperation(Summary = "Stop Selling in Business Day - ADMIN - {Huu Tri}")]
        [HttpPut("business-day/{businessDayId}")]
        public async Task<IActionResult> StopSellingDay(Guid businessDayId)
        {
            if (ModelState.IsValid)
            {
                var response = await _businessDayService.StopSellingDay(businessDayId);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [SwaggerOperation(Summary = "InActive BusinessDay - ADMIN - {Huu Tri}")]
        [HttpDelete("business-day/{id}")]
        public async Task<IActionResult> DeleteBusinessDay(Guid id)
        {
            var response = await _businessDayService.DeleteBusinessDay(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "Update BusinessDay - ADMIN - {Huu Tri}")]
        [HttpPut("business-day-update/{id}")]
        public async Task<IActionResult> UpdateBusinessDay(Guid id, BusinessDayRequestUpdate businessDayRequestUpdate)
        {
            var response = await _businessDayService.UpdateBusinessDay(id, businessDayRequestUpdate);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}

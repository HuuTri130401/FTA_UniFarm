using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers
{
    [Authorize]
    public class AccountsController : BaseController
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        
        [HttpGet("aboutMe")]
        [SwaggerOperation(Summary = "About Me - Done {Tien}")]
        public async Task<IActionResult> AboutMe()
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader))
            {
                return Unauthorized();
            }

            string token = authHeader.Replace("Bearer ", "");

            var defineUser = _accountService.GetIdAndRoleFromToken(token);
            if (defineUser.Payload == null) return HandleErrorResponse(defineUser!.Errors);
            if (defineUser.Payload.Role == EnumConstants.RoleEnumString.FARMHUB)
            {
                var responseFarmHub = await _accountService.GetAboutFarmHub(defineUser.Payload.Id);
                return responseFarmHub.IsError
                    ? HandleErrorResponse(responseFarmHub!.Errors)
                    : Ok(responseFarmHub.Payload);
            }
            else if (defineUser.Payload.Role == EnumConstants.RoleEnumString.COLLECTEDSTAFF)
            {
                var responseCollected = await _accountService.GetAboutCollectedStaff(defineUser.Payload.Id);
                return responseCollected.IsError
                    ? HandleErrorResponse(responseCollected!.Errors)
                    : Ok(responseCollected.Payload);
            }
            else if (defineUser.Payload.Role == EnumConstants.RoleEnumString.STATIONSTAFF)
            {
                var responseStationStaff = await _accountService.GetAboutStationStaff(defineUser.Payload.Id);
                return responseStationStaff.IsError
                    ? HandleErrorResponse(responseStationStaff!.Errors)
                    : Ok(responseStationStaff.Payload);
            }
            else if (defineUser.Payload.Role == EnumConstants.RoleEnumString.CUSTOMER)
            {
                var responseCustomer = await _accountService.GetAboutCustomer(defineUser.Payload.Id);
                return responseCustomer.IsError
                    ? HandleErrorResponse(responseCustomer!.Errors)
                    : Ok(responseCustomer.Payload);
            }

            var response = await _accountService.GetAboutAdmin(defineUser.Payload.Id);
            return response.IsError ? HandleErrorResponse(response!.Errors) : Ok(response.Payload);
        }
        
        [HttpPut("update-profile")]
        [SwaggerOperation(Summary = "Update Profile - Done {Tien}")]
        public async Task<IActionResult> UpdateProfile([FromBody] AccountRequestUpdate request)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader))
            {
                return Unauthorized();
            }

            // The token is prefixed with "Bearer ", so we need to remove that prefix
            string token = authHeader.Replace("Bearer ", "");

            var defineUser = _accountService.GetIdAndRoleFromToken(token);
            if (defineUser.Payload == null) return HandleErrorResponse(defineUser!.Errors);
            request.Role = defineUser.Payload.Role;
            var response = await _accountService.UpdateAccount(defineUser.Payload.Id, request);
            return response.IsError ? HandleErrorResponse(response!.Errors) : Ok(response.Payload);
        }
        
    }
}
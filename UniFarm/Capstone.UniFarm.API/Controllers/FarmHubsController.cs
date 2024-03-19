using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Services.CustomServices;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Data;
using System.Security.Claims;

namespace Capstone.UniFarm.API.Controllers
{
    [ApiController]
    public class FarmHubsController : BaseController
    {
        private readonly IFarmHubService _farmHubService;
        private readonly IAccountService _accountService;

        public FarmHubsController(IFarmHubService farmHubService, IAccountService accountService)
        {
            _farmHubService = farmHubService;
            _accountService = accountService;
        }

        [SwaggerOperation(Summary = "Get All FarmHubs - Admin Role - {Huu Tri}")]
        [HttpGet("farm-hubs")]
        public async Task<IActionResult> GetAllFarmHubs()
        {
            var response = await _farmHubService.GetAllFarmHubs();
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "Get FarmHub By Id - Admin Role, FarmHub Role - {Huu Tri}")]
        [HttpGet("farm-hub/{id}")]
        public async Task<IActionResult> GetFarmHubById(Guid id)
        {
            var response = await _farmHubService.GetFarmHubById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        //[SwaggerOperation(Summary = "Get Profile of FarmHub - FarmHub Role - {Huu Tri}")]
        //[HttpGet("farm-hub/profile")]
        //[Authorize(Roles = "FarmHub")]
        //public async Task<IActionResult> GetProfileFarmHub()
        //{
        //    string authHeader = HttpContext.Request.Headers["Authorization"];
        //    if (string.IsNullOrEmpty(authHeader))
        //    {
        //        return Unauthorized();
        //    }
        //    string token = authHeader.Replace("Bearer ", "");

        //    var defineUser = _accountService.GetIdAndRoleFromToken(token);
        //    if (defineUser.Payload == null)
        //    {
        //        return HandleErrorResponse(defineUser!.Errors);
        //    }
        //    var farmHubAccountId = defineUser.Payload.Id;
        //    var response = await _farmHubService.GetFarmHubInforByFarmHubAccountId(farmHubAccountId);
        //    return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        //}

        [SwaggerOperation(Summary = "Create FarmHub Shop - Admin Role - {Huu Tri}")]
        [HttpPost("farm-hub/create-shop")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateFarmHub(AccountAndFarmHubRequest farmHubRequest)
        {
            if (ModelState.IsValid)
            {
                var response = await _farmHubService.CreateFarmHubShop(farmHubRequest);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [SwaggerOperation(Summary = "Update FarmHub Shop - Admin Role, FarmHub Role - {Huu Tri}")]
        [HttpPut("farm-hub/{id}")]
        public async Task<IActionResult> UpdateFarmHub(Guid id, FarmHubRequestUpdate FarmHubRequestUpdate)
        {
            if (ModelState.IsValid)
            {
                var response = await _farmHubService.UpdateFarmHub(id, FarmHubRequestUpdate);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [SwaggerOperation(Summary = "Delete FarmHub - Admin Role - {Huu Tri}")]
        [HttpDelete("farm-hub/{id}")]
        public async Task<IActionResult> DeleteFarmHub(Guid id)
        {
            var response = await _farmHubService.DeleteFarmHub(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}

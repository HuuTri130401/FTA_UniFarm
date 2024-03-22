using Capstone.UniFarm.Services.CustomServices;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Data;

namespace Capstone.UniFarm.API.Controllers
{
    [ApiController]
    public class MenusController : BaseController
    {
        private readonly IMenuService _menuService;
        private readonly IAccountService _accountService;

        public MenusController(IMenuService menuService, IAccountService accountService)
        {
            _menuService = menuService;
            _accountService = accountService;
        }

        [SwaggerOperation(Summary = "Get All Menus In FarmHub - FARMHUB - {Huu Tri}")]
        [HttpGet("menus")]
        [Authorize(Roles = "FarmHub")]
        public async Task<IActionResult> GetAllMenusInFarmHubId()
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

            var response = await _menuService.GetAllMenusByFarmHubAccountId(farmHubAccountId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "Get Menu By Id - FARMHUB - {Huu Tri}")]
        [HttpGet("menu/{id}")]
        public async Task<IActionResult> GetMenuById(Guid id)
        {
            var response = await _menuService.GetMenuById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "Create Menu For FarmHub - FARMHUB - {Huu Tri}")]
        [HttpPost("menu")]
        [Authorize(Roles = "FarmHub")]
        public async Task<IActionResult> CreateMenuForFarmHub(MenuRequest menuRequest)
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
                var response = await _menuService.CreateMenuForFarmHub(farmHubAccountId, menuRequest);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [SwaggerOperation(Summary = "Assign Menu to BusinessDay - FARMHUB - {Huu Tri}")]
        [HttpPost("business-day/{businessDayId}/menu/{menuId}")]
        public async Task<IActionResult> AssignMenuToBusinessDay(Guid businessDayId, Guid menuId)
        {
            if (ModelState.IsValid)
            {
                var response = await _menuService.AssignMenuToBusinessDay(businessDayId, menuId);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [SwaggerOperation(Summary = "Update Menu - FARMHUB - {Huu Tri}")]
        [HttpPut("menu/{id}")]
        public async Task<IActionResult> UpdateMenu(Guid id, MenuRequestUpdate menuRequestUpdate)
        {
            if (ModelState.IsValid)
            {
                var response = await _menuService.UpdateMenu(id, menuRequestUpdate);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [SwaggerOperation(Summary = "InActive Menu - FARMHUB - {Huu Tri}")]
        [HttpDelete("menu/{id}")]
        public async Task<IActionResult> DeleteMenu(Guid id)
        {
            var response = await _menuService.DeleteMenu(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}

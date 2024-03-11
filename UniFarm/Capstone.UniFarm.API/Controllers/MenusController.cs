using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers
{
    [ApiController]
    public class MenusController : BaseController
    {
        private readonly IMenuService _menuService;

        public MenusController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [SwaggerOperation(Summary = "Get All Menus By FarmHub Id - FarmHub Role - {Huu Tri}")]
        [HttpGet("farm-hub/{id}/menus")]
        public async Task<IActionResult> GetAllMenusByFarmHubId(Guid id)
        {
            var response = await _menuService.GetAllMenusByFarmHubId(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "Get Menu By Id - FarmHub Role - {Huu Tri}")]
        [HttpGet("menu/{id}")]
        public async Task<IActionResult> GetMenuById(Guid id)
        {
            var response = await _menuService.GetMenuById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [SwaggerOperation(Summary = "Create Menu For FarmHub - FarmHub Role - {Huu Tri}")]
        [HttpPost("farm-hub/{id}/menu")]
        public async Task<IActionResult> CreateMenuForFarmHub(Guid id, MenuRequest menuRequest)
        {
            if (ModelState.IsValid)
            {
                var response = await _menuService.CreateMenuForFarmHub(id, menuRequest);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [SwaggerOperation(Summary = "Assign Menu to BusinessDay - FarmHub Role - {Huu Tri}")]
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

        [SwaggerOperation(Summary = "Update Menu - FarmHub Role - {Huu Tri}")]
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

        [SwaggerOperation(Summary = "Update Menu - FarmHub Role - {Huu Tri}")]
        [HttpDelete("menu/{id}")]
        public async Task<IActionResult> DeleteMenu(Guid id)
        {
            var response = await _menuService.DeleteMenu(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}

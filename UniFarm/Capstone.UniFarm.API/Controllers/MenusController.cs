using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.UniFarm.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenusController : BaseController
    {
        private readonly IMenuService _menuService;

        public MenusController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet("/api/v1/farmHub/{farmHubId}/menus")]
        public async Task<IActionResult> GetAllMenusByFarmHubId(Guid farmHubId)
        {
            var response = await _menuService.GetAllMenusByFarmHubId(farmHubId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [HttpGet("/api/v1/menu/{menuId}")]
        public async Task<IActionResult> GetMenuById(Guid menuId)
        {
            var response = await _menuService.GetMenuById(menuId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [HttpPost("/api/v1/farmHub/{farmHubId}/menu")]
        public async Task<IActionResult> CreateMenuForFarmHub(Guid farmHubId, MenuRequest menuRequest)
        {
            if (ModelState.IsValid)
            {
                var response = await _menuService.CreateMenuForFarmHub(farmHubId, menuRequest);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [HttpPut("/api/v1/menu/{menuId}")]
        public async Task<IActionResult> UpdateMenu(Guid menuId, MenuRequestUpdate menuRequestUpdate)
        {
            if (ModelState.IsValid)
            {
                var response = await _menuService.UpdateMenu(menuId, menuRequestUpdate);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [HttpDelete("/api/v1/menu/{menuId}")]
        public async Task<IActionResult> DeleteMenu(Guid menuId)
        {
            var response = await _menuService.DeleteMenu(menuId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}

using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers
{
    [ApiController]
    public class FarmHubsController : BaseController
    {
        private readonly IFarmHubService _farmHubService;

        public FarmHubsController(IFarmHubService farmHubService)
        {
            _farmHubService = farmHubService;
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

        [SwaggerOperation(Summary = "Create FarmHub - Admin Role - {Huu Tri}")]
        [HttpPost("farm-hub")]
        public async Task<IActionResult> CreateFarmHub(FarmHubRequest farmHubRequest)
        {
            if (ModelState.IsValid)
            {
                var response = await _farmHubService.CreateFarmHub(farmHubRequest);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [SwaggerOperation(Summary = "Update FarmHub - Admin Role, FarmHub Role - {Huu Tri}")]
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

using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.UniFarm.API.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class FarmHubsController : BaseController
    {
        private readonly IFarmHubService _farmHubService;

        public FarmHubsController(IFarmHubService farmHubService)
        {
            _farmHubService = farmHubService;
        }

        [HttpGet("api/v1/farm-hubs")]
        public async Task<IActionResult> GetAllFarmHubs()
        {
            var response = await _farmHubService.GetAllFarmHubs();
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [HttpGet("api/v1/farm-hub/{id}")]
        public async Task<IActionResult> GetFarmHubById(Guid farmHubId)
        {
            var response = await _farmHubService.GetFarmHubById(farmHubId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [HttpPost("api/v1/farm-hub")]
        public async Task<IActionResult> CreateFarmHub(FarmHubRequest farmHubRequest)
        {
            if (ModelState.IsValid)
            {
                var response = await _farmHubService.CreateFarmHub(farmHubRequest);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [HttpPut("api/v1/farm-hub/{id}")]
        public async Task<IActionResult> UpdateFarmHub(Guid farmHubId, FarmHubRequestUpdate FarmHubRequestUpdate)
        {
            if (ModelState.IsValid)
            {
                var response = await _farmHubService.UpdateFarmHub(farmHubId, FarmHubRequestUpdate);
                return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            }
            return BadRequest("Model is invalid");
        }

        [HttpDelete("api/v1/farm-hub/{id}")]
        public async Task<IActionResult> DeleteFarmHub(Guid farmHubId)
        {
            var response = await _farmHubService.DeleteFarmHub(farmHubId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}

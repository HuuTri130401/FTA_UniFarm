using Capstone.UniFarm.Services.ICustomServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.UniFarm.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapsController : ControllerBase
    {
        private readonly IGoongMapsService _goongMapsService;

        public MapsController(IGoongMapsService goongMapsService)
        {
            _goongMapsService = goongMapsService;
        }

        [HttpGet("address")]
        public async Task<IActionResult> GetAddress(double latitude, double longitude)
        {
            var addressJson = await _goongMapsService.GetAddressFromLatLong(latitude, longitude);
            return Ok(addressJson);
        }

        [HttpGet("distance")]
        public async Task<IActionResult> GetDistance(string origins, string destinations)
        {
            var addressJson = await _goongMapsService.GetDistanceAndDuration(origins, destinations);
            return Ok(addressJson);
        }
    }
}

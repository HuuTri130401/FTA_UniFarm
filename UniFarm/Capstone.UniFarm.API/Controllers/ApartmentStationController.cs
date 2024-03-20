using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers;

public class ApartmentStationController : BaseController
{
    private readonly IApartmentStationService _apartmentStationService;
    private readonly IAccountService _accountService;

    public ApartmentStationController(
        IApartmentStationService apartmentStationService,
        IAccountService accountService
    )
    {
        _apartmentStationService = apartmentStationService;
        _accountService = accountService;
    }

    [HttpGet("apartment-station")]
    [SwaggerOperation(Summary = "Get all apartment station of a customer - Done {Tien}")]
    [Authorize]
    public async Task<IActionResult> GetAllApartmentStation(
        [FromQuery] string? orderBy,
        [FromQuery] bool? isAscending
    )
    {
        string authHeader = HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized();
        }
        string token = authHeader.Replace("Bearer ", "");
        var defineUser = _accountService.GetIdAndRoleFromToken(token);
        if (defineUser.Payload == null) return HandleErrorResponse(defineUser!.Errors);

        var response =
            await _apartmentStationService.GetAllByAccountId(defineUser.Payload.Id, isAscending ?? false, orderBy);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    [HttpPost("apartment-station/upsert")]
    [SwaggerOperation(Summary = "Create or update apartment station - Customer - Done {Tien}")]
    [Authorize]
    public async Task<IActionResult> Upsert(ApartmentStationRequestCreate objectRequestCreate)
    {
        string authHeader = HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized();
        }

        string token = authHeader.Replace("Bearer ", "");
        var defineUser = _accountService.GetIdAndRoleFromToken(token);
        if (defineUser.Payload == null) return HandleErrorResponse(defineUser!.Errors);

        var result = await _apartmentStationService.Upsert(defineUser.Payload.Id, objectRequestCreate);
        return result.IsError ? BadRequest(result.Errors) : Ok(result.Payload);
    }
}
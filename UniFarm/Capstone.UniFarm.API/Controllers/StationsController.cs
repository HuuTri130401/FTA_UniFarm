using System.ComponentModel.DataAnnotations;
using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers;

public class StationsController : BaseController
{
    private readonly IStationService _stationService;
    private readonly IAccountService _accountService;

    public StationsController(IStationService stationService, IAccountService accountService)
    {
        _stationService = stationService;
        _accountService = accountService;
    }

    [HttpGet("admin/stations")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Get all stations - Admin - Done {Tien}")]
    public async Task<IActionResult> GetAllStationsForAdmin(
        [FromQuery] string? keyword,
        [FromQuery] Guid? id,
        [FromQuery] Guid? areaId,
        [FromQuery] string? code,
        [FromQuery] string? name,
        [FromQuery] string? description,
        [FromQuery] string? address,
        [FromQuery] string? status,
        [FromQuery] string? orderBy,
        [FromQuery] bool? isAscending,
        [FromQuery] string[]? includeProperties,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10)
    {
        var response = await _stationService.GetAll(
            isAscending: isAscending,
            filter: x => (!id.HasValue || x.Id == id) &&
                         (!areaId.HasValue || x.AreaId == areaId) &&
                         (string.IsNullOrEmpty(keyword) || x.Code!.Contains(keyword) || x.Name!.Contains(keyword) ||
                          x.Address!.Contains(keyword) || x.Status!.Contains(keyword)) &&
                         (string.IsNullOrEmpty(code) || x.Code!.Contains(code)) &&
                         (string.IsNullOrEmpty(name) || x.Name!.Contains(name)) &&
                         (string.IsNullOrEmpty(description) || x.Description!.Contains(description)) &&
                         (string.IsNullOrEmpty(address) || x.Address!.Contains(address)) &&
                         (string.IsNullOrEmpty(status) || x.Status!.Contains(status)),
            orderBy: orderBy, // Pass the string representation of the property name
            includeProperties: includeProperties,
            pageIndex: pageIndex,
            pageSize: pageSize
        );
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }


    [HttpGet("admin/station/{id}")]
    [SwaggerOperation(Summary = "Get status By Id - Admin Role- Done {Tien}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetStationByIdForAdmin(Guid id)
    {
        var response = await _stationService.GetById(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }


    [HttpPost("admin/station")]
    [SwaggerOperation(Summary = "Create station - Admin - Done {Tien}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateStation([FromBody] StationRequestCreate request)
    {
        var response = await _stationService.Create(request);
        return response.IsError
            ? HandleErrorResponse(response.Errors)
            : Created($"station/{response.Payload!.Id}", response);
    }

    [HttpPut("admin/station/{id}")]
    [SwaggerOperation(Summary = "Update station - Admin - Done {Tien}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStation(Guid id, [FromBody] StationRequestUpdate request)
    {
        var response = await _stationService.Update(id, request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    [HttpDelete("admin/station/{id}")]
    [SwaggerOperation(Summary = "Delete station - Admin - Done {Tien}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteStation(Guid id)
    {
        var response = await _stationService.Delete(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    [HttpGet("admin/station/{id}/staffs-filter")]
    [SwaggerOperation(Summary = "Get all staffs in a station - Admin - Done {Tien}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllStaffsInStation([Required] Guid id, [FromQuery] string? keyword,
        [FromQuery] string? orderBy, [FromQuery] bool? isAscending, [FromQuery] string[]? includeProperties,
        [FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10)
    {
        var response = await _stationService.GetStationStaffsData(
            id,
            isAscending,
            orderBy,
            x => string.IsNullOrEmpty(keyword) || x.UserName.Contains(keyword) || x.Email.Contains(keyword) ||
                 x.PhoneNumber.Contains(keyword) || x.Code!.Contains(keyword) || x.Address!.Contains(keyword),
            includeProperties,
            pageIndex,
            pageSize
        );
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    [HttpGet("admin/station/staffs-not-working")]
    [SwaggerOperation(Summary = "Get all collected staff data not working - Admin Role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetCollectedStaffsDataNotWorking([FromQuery] string? keyword,
        [FromQuery] string? orderBy, [FromQuery] bool? isAscending, [FromQuery] string[]? includeProperties,
        [FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10)
    {
        var response = await _stationService.GetStationStaffsNotWorking(
            isAscending,
            orderBy,
            x => string.IsNullOrEmpty(keyword) || x.UserName.Contains(keyword) || x.Email.Contains(keyword) ||
                 x.PhoneNumber.Contains(keyword) || x.Code!.Contains(keyword) || x.Address!.Contains(keyword),
            includeProperties,
            pageIndex,
            pageSize
        );
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    [HttpGet("stations")]
    [SwaggerOperation(Summary = "Get all active stations for user - Done {Tien}")]
    public async Task<IActionResult> GetAllStations(
        [FromQuery] string? keyword,
        [FromQuery] Guid? id,
        [FromQuery] Guid? areaId,
        [FromQuery] string? code,
        [FromQuery] string? name,
        [FromQuery] string? description,
        [FromQuery] string? address,
        [FromQuery] string? orderBy,
        [FromQuery] bool? isAscending,
        [FromQuery] string[]? includeProperties,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10)
    {
        var response = await _stationService.GetAll(
            isAscending: isAscending,
            filter: x => (!id.HasValue || x.Id == id) &&
                         (!areaId.HasValue || x.AreaId == areaId) &&
                         (string.IsNullOrEmpty(keyword) || x.Code!.Contains(keyword) || x.Name!.Contains(keyword) ||
                          x.Address!.Contains(keyword)) &&
                         (string.IsNullOrEmpty(code) || x.Code!.Contains(code)) &&
                         (string.IsNullOrEmpty(name) || x.Name!.Contains(name)) &&
                         (string.IsNullOrEmpty(description) || x.Description!.Contains(description)) &&
                         (string.IsNullOrEmpty(address) || x.Address!.Contains(address)) &&
                         x.Status!.Equals(EnumConstants.ActiveInactiveEnum.ACTIVE),
            orderBy: orderBy,
            includeProperties: includeProperties,
            pageIndex: pageIndex,
            pageSize: pageSize
        );
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    [HttpGet("station/{id}")]
    [SwaggerOperation(Summary = "Get status By Id which is active - Done {Tien}")]
    public async Task<IActionResult> GetStationActiveByIdForUser(Guid id, string[]? includeProperties = null)
    {
        var response = await _stationService.GetFilterByExpression(
            x => x.Id == id && x.Status!.Equals(EnumConstants.ActiveInactiveEnum.ACTIVE), includeProperties);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }


    [HttpGet("station/dashboards")]
    [SwaggerOperation(Summary = "Show dashboard - StationStaff - Done {Tien}")]
    [Authorize(Roles = "StationStaff")]
    public async Task<IActionResult> ShowDashboard(
        [FromQuery] int dayBack = 7
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
        
        
        var response = await _stationService.ShowDashboard(DateTime.Now.AddDays(-dayBack), DateTime.Now, defineUser.Payload);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
}
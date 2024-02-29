using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers;

public class AreasController : BaseController
{
    private readonly IAreaService _areaService;
    
    public AreasController(IAreaService areaService)
    {
        _areaService = areaService;
    }
    
    [HttpGet("areas")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Get all areas - Done {Tien}")]
    public async Task<IActionResult> GetAllAreas(
        [FromQuery] string? keyword,
        [FromQuery] Guid? id,
        [FromQuery] string? province,
        [FromQuery] string? district,
        [FromQuery] string? commune,
        [FromQuery] string? address,
        [FromQuery] string? status,
        [FromQuery] string? code,
        [FromQuery] string? orderBy,
        [FromQuery] bool? isAscending,
        [FromQuery] string[]? includeProperties,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10)
    {
        var response = await _areaService.GetAll(
            isAscending: isAscending,
            filter: x => (!id.HasValue || x.Id == id) &&
                         (string.IsNullOrEmpty(keyword) || x.Province.Contains(keyword) || x.District.Contains(keyword) || x.Commune.Contains(keyword) || x.Address.Contains(keyword)) &&
                         (string.IsNullOrEmpty(province) || x.Province.Contains(province)) &&
                         (string.IsNullOrEmpty(district) || x.District.Contains(district)) &&
                         (string.IsNullOrEmpty(commune) || x.Commune.Contains(commune)) &&
                         (string.IsNullOrEmpty(address) || x.Address.Contains(address)) &&
                         (string.IsNullOrEmpty(status) || x.Status.Contains(status)) &&
                         (string.IsNullOrEmpty(code) || x.Code.Contains(code)),
            orderBy: orderBy, // Pass the string representation of the property name
            includeProperties: includeProperties,
            pageIndex: pageIndex,
            pageSize: pageSize
        );
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    
    [HttpGet("area/{id}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Get area by id - Done {Tien}")]
    public async Task<IActionResult> GetArea([FromQuery] Guid id)
    {
        var response = await _areaService.GetById(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
    [HttpPost("area/create")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Create area - Done {Tien}", Description = "Create new area")]
    public async Task<IActionResult> CreateArea([FromBody] AreaRequestCreate requestCreateModel)
    {
        var response = await _areaService.Create(requestCreateModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
    [HttpPut("area/update/{id}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Update area - Done {Tien}", Description = "Update area by id")]
    public async Task<IActionResult> UpdateArea(Guid id, [FromBody] AreaRequestUpdate requestModel)
    {
        var response = await _areaService.Update(id, requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
    [HttpDelete("area/delete/{id}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Soft remove area - Done {Tien}", Description = "Delete area by id")]
    public async Task<IActionResult> RemoveArea(Guid id)
    {
        var response = await _areaService.Delete(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
    [HttpGet("odata/areas")]
    [EnableQuery]
    public async Task<IEnumerable<AreaResponse>> GetAll()
    {
        return (await _areaService.GetAll(null)).Payload!.AsQueryable();
    }
}
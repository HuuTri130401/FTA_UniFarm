using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Capstone.UniFarm.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AreasController : BaseController
{
    private readonly IAreaService _areaService;
    
    public AreasController(IAreaService areaService)
    {
        _areaService = areaService;
    }
    
    [HttpGet]
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

    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetArea([FromQuery] Guid id)
    {
        var response = await _areaService.GetById(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateArea([FromBody] AreaRequestCreate requestCreateModel)
    {
        var response = await _areaService.Create(requestCreateModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateArea(Guid id, [FromBody] AreaRequestUpdate requestModel)
    {
        var response = await _areaService.Update(id, requestModel);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveArea(Guid id)
    {
        var response = await _areaService.Delete(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
    
    [HttpGet("odata")]
    [EnableQuery]
    public async Task<IEnumerable<AreaResponse>> GetAll()
    {
        return (await _areaService.GetAll(null)).Payload!.AsQueryable();
    }
}
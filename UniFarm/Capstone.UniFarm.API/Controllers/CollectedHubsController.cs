using System.Linq.Expressions;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers;

public class CollectedHubsController : BaseController
{
    private readonly ICollectedHubService _collectedHubService;

    public CollectedHubsController(ICollectedHubService collectedHubService)
    {
        _collectedHubService = collectedHubService;
    }

    [HttpGet("admin/collected-hubs/")]
    [SwaggerOperation(Summary = "Get all collected hubs - Admin ")]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? keyword,
        [FromQuery] Guid? id,
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
        var response = await _collectedHubService.GetAll(
            isAscending: isAscending,
            filter: x => (!id.HasValue || x.Id == id) &&
                         (string.IsNullOrEmpty(keyword) || x.Code.Contains(keyword) || x.Name.Contains(keyword) || x.Description.Contains(keyword) || x.Address.Contains(keyword) || x.Status.Contains(keyword)) &&
                         (string.IsNullOrEmpty(code) || x.Code.Contains(code)) &&
                         (string.IsNullOrEmpty(name) || x.Name.Contains(name)) &&
                         (string.IsNullOrEmpty(description) || x.Description.Contains(description)) &&
                         (string.IsNullOrEmpty(address) || x.Address.Contains(address)) &&
                         (string.IsNullOrEmpty(status) || x.Status.Contains(status)),
            orderBy: orderBy,
            includeProperties: includeProperties,
            pageIndex: pageIndex,
            pageSize: pageSize
        );
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
    [HttpGet("admin/collected-hub/{id}")]
    [SwaggerOperation(Summary = "Get collected hub By Id - Admin Role")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _collectedHubService.GetById(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
    [HttpPost("admin/collected-hub/")]
    [SwaggerOperation(Summary = "Create collected hub - Admin Role")]
    public async Task<IActionResult> Create([FromBody] CollectedHubRequestCreate request)
    {
        var response = await _collectedHubService.Create(request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Created($"api/v1/admin/collected-hub/{response.Payload.Id}", response);
    }
    
    [HttpPut("admin/collected-hub/{id}")]
    [SwaggerOperation(Summary = "Update collected hub - Admin Role")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CollectedHubRequestUpdate request)
    {
        var response = await _collectedHubService.Update(id, request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
    [HttpDelete("admin/delete/{id}")]
    [SwaggerOperation(Summary = "Delete collected hub - Admin Role")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _collectedHubService.Delete(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
}
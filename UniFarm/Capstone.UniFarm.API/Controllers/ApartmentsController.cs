using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers;

public class ApartmentsController : BaseController
{
    private readonly IApartmentService _apartmentService;

    public ApartmentsController(IApartmentService apartmentService)
    {
        _apartmentService = apartmentService;
    }

    [HttpGet("apartments")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Get all apartments - Done {Tien}")]
    public async Task<IActionResult> GetAllApartments(
        [FromQuery] string? keyword,
        [FromQuery] Guid? id,
        [FromQuery] string? name,
        [FromQuery] string? address,
        [FromQuery] string? status,
        [FromQuery] string? orderBy,
        [FromQuery] bool? isAscending,
        [FromQuery] string[]? includeProperties,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10)
    {
        var response = await _apartmentService.GetAll(
            isAscending: isAscending,
            filter: x => (!id.HasValue || x.Id == id) &&
                         (string.IsNullOrEmpty(keyword) || x.Name.Contains(keyword) || x.Address.Contains(keyword)) &&
                         (string.IsNullOrEmpty(name) || x.Name.Contains(name)) &&
                         (string.IsNullOrEmpty(address) || x.Address.Contains(address)) &&
                         (string.IsNullOrEmpty(status) || x.Status.Contains(status)),
            orderBy: orderBy,
            includeProperties: includeProperties,
            pageIndex: pageIndex,
            pageSize: pageSize
        );
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    [HttpGet("apartment/{id}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Get apartment by id - Done {Tien}")]
    public async Task<IActionResult> GetApartment([FromQuery] Guid id)
    {
        var response = await _apartmentService.GetById(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    [HttpPost("apartment/create")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Create apartment - Done {Tien}", Description = "Create new apartment")]
    public async Task<IActionResult> CreateApartment([FromBody] ApartmentRequestCreate model)
    {
        var response = await _apartmentService.Create(model);
        return response.IsError ? HandleErrorResponse(response.Errors) : Created("/api/apartments", response.Payload);
    }

    [HttpPut("apartment/update/{id}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Update apartment - Done {Tien}", Description = "Update apartment by id")]
    public async Task<IActionResult> UpdateApartment(Guid id, [FromBody] ApartmentRequestUpdate model)
    {
        var response = await _apartmentService.Update(id, model);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    [HttpDelete("apartment/delete/{id}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Soft remove apartment - Done {Tien}", Description = "Delete apartment by id")]
    public async Task<IActionResult> DeleteApartment([FromQuery] Guid id)
    {
        var response = await _apartmentService.Delete(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
}
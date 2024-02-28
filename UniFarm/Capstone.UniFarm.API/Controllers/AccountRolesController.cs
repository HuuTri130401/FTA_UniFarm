using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers;

public class AccountRolesController : BaseController
{
    private readonly IAccountRoleService _accountRoleService;

    public AccountRolesController(IAccountRoleService accountRoleService)
    {
        _accountRoleService = accountRoleService;
    }

    [HttpGet("account-roles")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Get all account roles - Done {Tien}")]
    public async Task<IActionResult> GetAllAccountRoles(
        [FromQuery] Guid? id,
        [FromQuery] Guid? accountId,
        [FromQuery] string? status,
        [FromQuery] bool? isAscending,
        string? orderBy = null,
        string[]? includeProperties = null,
        int pageIndex = 0,
        int pageSize = 10)
    {
        var response = await _accountRoleService
            .GetAll(isAscending,
                orderBy,
                filter: x => (!id.HasValue || x.Id == id) &&
                             (!accountId.HasValue || x.AccountId == accountId) &&
                             (string.IsNullOrEmpty(status) || x.Status.Contains(status)),
                includeProperties,
                pageIndex,
                pageSize);

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    [HttpGet("account-role/{id}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Get account role by id - Done {Tien}")]
    public async Task<IActionResult> GetAccountRole([FromQuery] Guid id)
    {
        var response = await _accountRoleService.GetById(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    [HttpPost("account-role/create")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Create account role - Done {Tien}", Description = "Create account role")]
    public async Task<IActionResult> CreateAccountRole([FromBody] AccountRoleRequest model)
    {
        var response = await _accountRoleService.Create(model);
        return response.IsError ? HandleErrorResponse(response.Errors) : Created("/api/AccountRoles", response.Payload);
    }

    [HttpPut("account-role/update/{id}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Update account role - Done {Tien}")]
    public async Task<IActionResult> UpdateAccountRole(Guid id, [FromBody] AccountRoleRequestUpdate model)
    {
        var response = await _accountRoleService.Update(id, model);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    [HttpDelete("account-role/delete/{id}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Soft remove account role - Done {Tien}")]
    public async Task<IActionResult> DeleteAccountRole([FromQuery] Guid id)
    {
        var response = await _accountRoleService.Delete(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
}
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers;

[Authorize(Roles = "Admin")]
public class AccountRolesController : BaseController
{
    private readonly IAccountRoleService _accountRoleService;

    public AccountRolesController(IAccountRoleService accountRoleService)
    {
        _accountRoleService = accountRoleService;
    }

    [HttpGet("admin/account-roles")]
    [SwaggerOperation(Summary = "Get all account roles - Done {Tien}")]
    public async Task<IActionResult> GetAllAccountRoles(
        [FromQuery] Guid? id,
        [FromQuery] Guid? accountId,
        [FromQuery] string? status,
        [FromQuery] bool? isAscending,
        [FromQuery] string? orderBy = null,
        [FromQuery] string[]? includeProperties = null,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10)
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

    [HttpGet("admin/account-role/{id}")]
    [SwaggerOperation(Summary = "Get account role by id - Done {Tien}")]
    public async Task<IActionResult> GetAccountRole(Guid id)
    {
        var response = await _accountRoleService.GetById(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    

    [HttpPut("admin/account-role/update/{id}")]
    [SwaggerOperation(Summary = "Update account role - Done {Tien}")]
    public async Task<IActionResult> UpdateAccountRole(Guid id, [FromBody] AccountRoleRequestUpdate model)
    {
        var response = await _accountRoleService.Update(id, model);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    [HttpDelete("admin/account-role/delete/{id}")]
    [SwaggerOperation(Summary = "Soft remove account role by updating status inactive - Done {Tien}")]
    public async Task<IActionResult> DeleteAccountRole([FromQuery] Guid id)
    {
        var response = await _accountRoleService.Delete(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
    [HttpDelete("admin/account-role/delete/{accountId}")]
    [SwaggerOperation(Summary = "Soft remove account role by updating status inactive - Done {Tien}")]
    public async Task<IActionResult> DeleteAccountRoleByAccountId([FromQuery] Guid accountId)
    {
        var response = await _accountRoleService.Delete(accountId);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
}
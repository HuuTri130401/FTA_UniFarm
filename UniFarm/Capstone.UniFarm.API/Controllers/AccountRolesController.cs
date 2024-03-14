using System.ComponentModel.DataAnnotations;
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
                filter: x => (!accountId.HasValue || x.AccountId == accountId) &&
                             (string.IsNullOrEmpty(status) || x.Status.Contains(status)),
                includeProperties,
                pageIndex,
                pageSize);

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    [HttpGet("admin/account-role/{accountId}")]
    [SwaggerOperation(Summary = "Get account role by accountId - Done {Tien}")]
    public async Task<IActionResult> GetAccountRole(Guid accountId)
    {
        var response = await _accountRoleService.GetByAccountId(accountId);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }


    [HttpPut("admin/account-role/update/{accountId}")]
    [SwaggerOperation(Summary = "Update account role by account Id - Done {Tien}")]
    public async Task<IActionResult> UpdateAccountRole([Required] Guid accountId,
        [FromBody] AccountRoleRequestUpdate model)
    {
        model.AccountId = accountId;
        var response = await _accountRoleService.Update(accountId, model);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    /*[HttpDelete("admin/account-role/delete/{id}")]
    [SwaggerOperation(Summary = "Soft remove account role by Id - Done {Tien}")]
    public async Task<IActionResult> DeleteAccountRole([FromQuery] Guid id)
    {
        var response = await _accountRoleService.Delete(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }*/

    [HttpDelete("admin/account-role/delete/{accountId}")]
    [SwaggerOperation(Summary = "Soft remove account role by account Id - Done {Tien}")]
    public async Task<IActionResult> DeleteAccountRoleByAccountId([FromQuery] Guid accountId)
    {
        var response = await _accountRoleService.DeleteByAccountId(accountId);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
}
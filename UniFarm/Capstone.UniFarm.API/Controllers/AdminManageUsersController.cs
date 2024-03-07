using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers;

[Authorize(Roles = "Admin")]
public class AdminManageUsersController : BaseController
{
    private readonly IManageUsersService _manageUsersService;

    public AdminManageUsersController(IManageUsersService manageUsersService)
    {
        _manageUsersService = manageUsersService;
    }

    [HttpPost("admin/account/create")]
    [SwaggerOperation(Summary = "Create account FarmHub, CollectedStaff, StationStaff  - Admin Role - Done {Tien}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAccount([FromBody] AccountRequestCreate model)
    {
        var response = await _manageUsersService.CreateAccountForAdmin(model);
        return response.IsError
            ? HandleErrorResponse(response!.Errors)
            : Created("/api/v1/admin/account/create", response.Payload);
    }

    [HttpGet("admin/accounts")]
    [SwaggerOperation(Summary = "Get all accounts  - Admin Role - Done {Tien}")]
    public async Task<IActionResult> GetAllAccounts(
        [FromQuery] string? keyword,
        [FromQuery] Guid? id,
        [FromQuery] string? roleName,
        [FromQuery] string? firstName,
        [FromQuery] string? lastName,
        [FromQuery] string? phone,
        [FromQuery] string? email,
        [FromQuery] string? status,
        [FromQuery] string? orderBy,
        [FromQuery] bool? isAscending,
        [FromQuery] string[]? includeProperties,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10)
    {
        var response = await _manageUsersService.GetAllAccountsForAdmin(
            isAscending: isAscending,
            filter: x => (!id.HasValue || x.Id == id) &&
                         (string.IsNullOrEmpty(keyword) || x.UserName.Contains(keyword) ||
                          x.FirstName!.Contains(keyword) || x.LastName!.Contains(keyword) ||
                          x.Phone!.Contains(keyword) || x.Email.Contains(keyword) || x.Status!.Contains(keyword)) &&
                         (string.IsNullOrEmpty(roleName) || x.RoleName!.Contains(roleName)) &&
                         (string.IsNullOrEmpty(firstName) || x.FirstName!.Contains(firstName)) &&
                         (string.IsNullOrEmpty(lastName) || x.LastName!.Contains(lastName)) &&
                         (string.IsNullOrEmpty(phone) || x.Phone!.Contains(phone)) &&
                         (string.IsNullOrEmpty(email) || x.Email.Contains(email)) &&
                         (string.IsNullOrEmpty(status) || x.Status!.Contains(status)),
            orderBy: orderBy,
            includeProperties: includeProperties,
            pageIndex: pageIndex,
            pageSize: pageSize
        );
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    [HttpGet("admin/manage/customers")]
    [SwaggerOperation(Summary = "Get all customers  - Admin Role - Done {Tien}")]
    public async Task<IActionResult> GetAllCustomers(
        [FromQuery] string? keyword,
        [FromQuery] Guid? id,
        [FromQuery] string? firstName,
        [FromQuery] string? lastName,
        [FromQuery] string? phone,
        [FromQuery] string? email,
        [FromQuery] string? status,
        [FromQuery] string? orderBy,
        [FromQuery] bool? isAscending,
        [FromQuery] string[]? includeProperties,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10)
    {
        var response = await _manageUsersService.GetAllCustomersForAdmin(
            isAscending: isAscending,
            filter: x => (!id.HasValue || x.Id == id) &&
                         (string.IsNullOrEmpty(keyword) || x.UserName.Contains(keyword) ||
                          x.FirstName!.Contains(keyword) || x.LastName!.Contains(keyword) ||
                          x.Phone!.Contains(keyword) || x.Email.Contains(keyword) || x.Status!.Contains(keyword)) &&
                         (string.IsNullOrEmpty(firstName) || x.FirstName!.Contains(firstName)) &&
                         (string.IsNullOrEmpty(lastName) || x.LastName!.Contains(lastName)) &&
                         (string.IsNullOrEmpty(phone) || x.Phone!.Contains(phone)) &&
                         (string.IsNullOrEmpty(email) || x.Email.Contains(email)) &&
                         (string.IsNullOrEmpty(status) || x.Status!.Contains(status)),
            orderBy: orderBy,
            includeProperties: includeProperties,
            pageIndex: pageIndex,
            pageSize: pageSize
        );
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    [HttpGet("admin/manage/farm-hubs")]
    [SwaggerOperation(Summary = "Get all farm hubs  - Admin Role - Done {Tien}")]
    public async Task<IActionResult> GetAllFarmHubs(
        [FromQuery] string? keyword,
        [FromQuery] Guid? id,
        [FromQuery] string? firstName,
        [FromQuery] string? lastName,
        [FromQuery] string? phone,
        [FromQuery] string? email,
        [FromQuery] string? status,
        [FromQuery] string? orderBy,
        [FromQuery] bool? isAscending,
        [FromQuery] string[]? includeProperties,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10)
    {
        var response = await _manageUsersService.GetAllFarmHubsForAdmin(
            isAscending: isAscending,
            filter: x => (!id.HasValue || x.Id == id) &&
                         (string.IsNullOrEmpty(keyword) || x.UserName.Contains(keyword) ||
                          x.FirstName!.Contains(keyword) || x.LastName!.Contains(keyword) ||
                          x.Phone!.Contains(keyword) || x.Email.Contains(keyword) || x.Status!.Contains(keyword)) &&
                         (string.IsNullOrEmpty(firstName) || x.FirstName!.Contains(firstName)) &&
                         (string.IsNullOrEmpty(lastName) || x.LastName!.Contains(lastName)) &&
                         (string.IsNullOrEmpty(phone) || x.Phone!.Contains(phone)) &&
                         (string.IsNullOrEmpty(email) || x.Email.Contains(email)) &&
                         (string.IsNullOrEmpty(status) || x.Status!.Contains(status)),
            orderBy: orderBy,
            includeProperties: includeProperties,
            pageIndex: pageIndex,
            pageSize: pageSize
        );
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    [HttpGet("admin/manage/collected-staffs")]
    [SwaggerOperation(Summary = "Get all collected staffs  - Admin Role - Done {Tien}")]
    public async Task<IActionResult> GetAllCollectedStaffs(
        [FromQuery] string? keyword,
        [FromQuery] Guid? id,
        [FromQuery] string? firstName,
        [FromQuery] string? lastName,
        [FromQuery] string? phone,
        [FromQuery] string? email,
        [FromQuery] string? status,
        [FromQuery] string? orderBy,
        [FromQuery] bool? isAscending,
        [FromQuery] string[]? includeProperties,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10)
    {
        var response = await _manageUsersService.GetAllCollectedStaffsForAdmin(
            isAscending: isAscending,
            filter: x => (!id.HasValue || x.Id == id) &&
                         (string.IsNullOrEmpty(keyword) || x.UserName.Contains(keyword) ||
                          x.FirstName!.Contains(keyword) || x.LastName!.Contains(keyword) ||
                          x.Phone!.Contains(keyword) || x.Email.Contains(keyword) || x.Status!.Contains(keyword)) &&
                         (string.IsNullOrEmpty(firstName) || x.FirstName!.Contains(firstName)) &&
                         (string.IsNullOrEmpty(lastName) || x.LastName!.Contains(lastName)) &&
                         (string.IsNullOrEmpty(phone) || x.Phone!.Contains(phone)) &&
                         (string.IsNullOrEmpty(email) || x.Email.Contains(email)) &&
                         (string.IsNullOrEmpty(status) || x.Status!.Contains(status)),
            orderBy: orderBy,
            includeProperties: includeProperties,
            pageIndex: pageIndex,
            pageSize: pageSize
        );
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    [HttpGet("admin/manage/station-staffs")]
    [SwaggerOperation(Summary = "Get all station staffs  - Admin Role - Done {Tien}")]
    public async Task<IActionResult> GetAllStationStaffs(
        [FromQuery] string? keyword,
        [FromQuery] Guid? id,
        [FromQuery] string? firstName,
        [FromQuery] string? lastName,
        [FromQuery] string? phone,
        [FromQuery] string? email,
        [FromQuery] string? status,
        [FromQuery] string? orderBy,
        [FromQuery] bool? isAscending,
        [FromQuery] string[]? includeProperties,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10)
    {
        var response = await _manageUsersService.GetAllStationStaffsForAdmin(
            isAscending: isAscending,
            filter: x => (!id.HasValue || x.Id == id) &&
                         (string.IsNullOrEmpty(keyword) || x.UserName.Contains(keyword) ||
                          x.FirstName!.Contains(keyword) || x.LastName!.Contains(keyword) ||
                          x.Phone!.Contains(keyword) || x.Email.Contains(keyword) || x.Status!.Contains(keyword)) &&
                         (string.IsNullOrEmpty(firstName) || x.FirstName!.Contains(firstName)) &&
                         (string.IsNullOrEmpty(lastName) || x.LastName!.Contains(lastName)) &&
                         (string.IsNullOrEmpty(phone) || x.Phone!.Contains(phone)) &&
                         (string.IsNullOrEmpty(email) || x.Email.Contains(email)) &&
                         (string.IsNullOrEmpty(status) || x.Status!.Contains(status)),
            orderBy: orderBy,
            includeProperties: includeProperties,
            pageIndex: pageIndex,
            pageSize: pageSize
        );
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    [HttpGet("admin/account/{id}")]
    [SwaggerOperation(Summary = "Get account by id  - Admin Role - Done {Tien}")]
    public async Task<IActionResult> GetAccountById(Guid id)
    {
        var response = await _manageUsersService.GetAccountByExpression(x => x.Id == id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
}
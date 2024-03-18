﻿using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers;

public class TransferController : BaseController
{
    private readonly ITransferService _transferService;
    private readonly IAccountService _accountService;

    public TransferController(ITransferService transferService, IAccountService accountService)
    {
        _transferService = transferService;
        _accountService = accountService;
    }

    [HttpPost("transfer/create")]
    /*
    [Authorize(Roles = "CollectedStaff")]
    */
    [SwaggerOperation(Summary = "Create transfer request - CollectedStaff - Done {Tien}")]
    public async Task<IActionResult> Create([FromBody] TransferRequestCreate request)
    {
        string authHeader = HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized();
        }

        string token = authHeader.Replace("Bearer ", "");

        var defineUser = _accountService.GetIdAndRoleFromToken(token);
        if (defineUser.Payload == null) return HandleErrorResponse(defineUser!.Errors);
        if (defineUser.Payload.Role != EnumConstants.RoleEnumString.COLLECTEDSTAFF)
        {
            return Unauthorized("You are not allowed to access this resource");
        }

        var createdBy = defineUser.Payload.Id;
        var result = await _transferService.Create(createdBy, request);
        return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result.Payload);
    }

    [HttpGet("transfers/getall")]
    [SwaggerOperation(Summary = "Get all transfer request - Admin, CollectedStaff, StationStaff - Done {Tien}")]
    /*
    [Authorize(Roles = "Admin,CollectedStaff,StationStaff")]
    */
    public async Task<IActionResult> GetAll(
        [FromQuery] string? keyword,
        [FromQuery] Guid? collectedId,
        [FromQuery] Guid? stationId,
        [FromQuery] string? status,
        [FromQuery] string? code,
        [FromQuery] Guid? createdBy,
        [FromQuery] Guid? updatedBy,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] bool? isAscending,
        [FromQuery] string? orderBy,
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 10
    )
    {
        var result = await _transferService.GetAll(
            isAscending: isAscending,
            filter: x => (!collectedId.HasValue || x.CollectedId == collectedId) &&
                         (!stationId.HasValue || x.StationId == stationId) &&
                         (!createdBy.HasValue || x.CreatedBy == createdBy) &&
                         (!updatedBy.HasValue || x.UpdatedBy == updatedBy) &&
                         (!fromDate.HasValue || x.CreatedAt >= fromDate) &&
                         (!toDate.HasValue || x.CreatedAt <= toDate) &&
                         (string.IsNullOrEmpty(keyword) || x.Code!.Contains(keyword) || x.Status!.Contains(keyword)) &&
                         (string.IsNullOrEmpty(status) || x.Status!.Contains(status)) &&
                         (string.IsNullOrEmpty(code) || x.Code!.Contains(code)),
            orderBy: orderBy,
            includeProperties: null,
            pageIndex: page,
            pageSize: pageSize);
        return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result.Payload);
    }
}
using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Services.CustomServices;
using Capstone.UniFarm.Services.ICustomServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers;

public class TransactionController : BaseController
{
    private readonly ITransactionService _transactionService;
    private readonly IAccountService _accountService;
    private readonly IWalletService _walletService;
    public TransactionController(ITransactionService transactionService, 
        IAccountService accountService, 
        IWalletService walletService)
    {
        _transactionService = transactionService;
        _accountService = accountService;
        _walletService = walletService;
    }

    [HttpGet("transactions")]
    [Authorize(Roles = "Admin,Customer")]
    [SwaggerOperation(Summary = "Get all transactions - Done {Tien}")]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status,
        [FromQuery] DateTime? paymentDate,
        [FromQuery] bool? isAscending = false,
        [FromQuery] string? orderBy = "paymentDate",
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10)
    {
        string authHeader = HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized();
        }
        string token = authHeader.Replace("Bearer ", "");
        var defineUser = _accountService.GetIdAndRoleFromToken(token);
        if (defineUser.Payload == null) return HandleErrorResponse(defineUser!.Errors);
        
        var walletResponse = await _walletService.GetByAccountId(defineUser.Payload.Id);

        if (walletResponse.Payload == null)
        {
            return HandleErrorResponse(walletResponse.Errors);
        }
        
        var walletId = walletResponse.Payload.Id;
        
        if(defineUser.Payload.Role == EnumConstants.RoleEnumString.CUSTOMER)
        {
            var result = await _transactionService.GetAll(
                isAscending,
                orderBy,
                x => x.PayerWalletId == walletId &&
                     (string.IsNullOrEmpty(status) || x.Status!.Contains(status)) &&
                     (!paymentDate.HasValue || x.PaymentDate == paymentDate),
                null,
                pageIndex,
                pageSize);
            return Ok(result);
        }

        if (defineUser.Payload.Role == EnumConstants.RoleEnumString.ADMIN)
        {
            var result = await _transactionService.GetAll(
                isAscending,
                orderBy,
                x => (string.IsNullOrEmpty(status) || x.Status!.Contains(status)) &&
                     (!paymentDate.HasValue || x.PaymentDate == paymentDate),
                null,
                pageIndex,
                pageSize);

            return Ok(result);
        }
        
        return Unauthorized();
    }

    [SwaggerOperation(Summary = "Get All Transaction - ADMIN, FARMHUB - {Huu Tri}")]
    [HttpGet("transactions/all")]
    [Authorize(Roles = "Admin, FarmHub")]
    public async Task<IActionResult> GetAllTransaction()
    {
        string authHeader = HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized();
        }
        string token = authHeader.Replace("Bearer ", "");

        var defineUser = _accountService.GetIdAndRoleFromToken(token);
        if (defineUser.Payload == null)
        {
            return HandleErrorResponse(defineUser!.Errors);
        }
        var accountId = defineUser.Payload.Id;
        var response = await _transactionService.GetAllTransaction(accountId);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
}
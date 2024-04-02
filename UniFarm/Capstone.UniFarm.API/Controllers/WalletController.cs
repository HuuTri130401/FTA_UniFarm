using Capstone.UniFarm.Services.ICustomServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.UniFarm.API.Controllers;

public class WalletController : BaseController
{
    private readonly IWalletService _walletService;
    private readonly IAccountService _accountService;

    public WalletController(IWalletService walletService, IAccountService accountService)
    {
        _walletService = walletService;
        _accountService = accountService;
    }

    [HttpGet("wallet")]
    [Authorize]
    public async Task<IActionResult> GetWallet()
    {
        string authHeader = HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized();
        }
        string token = authHeader.Replace("Bearer ", "");
        var defineUser = _accountService.GetIdAndRoleFromToken(token);
        if (defineUser.Payload == null) return HandleErrorResponse(defineUser!.Errors);
        var result = await _walletService.GetByAccountId(defineUser.Payload.Id);
        return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result.Payload);
    }
}
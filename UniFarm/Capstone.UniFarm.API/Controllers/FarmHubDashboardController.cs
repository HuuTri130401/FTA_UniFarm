using Capstone.UniFarm.Services.ICustomServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.UniFarm.API.Controllers;

[Authorize(Roles = "FarmHub")]
public class FarmHubDashboardController : BaseController
{
    private readonly IProductItemInMenuService _productItemInMenuService;
    private readonly IAccountService _accountService;
    
    public FarmHubDashboardController(IProductItemInMenuService productItemInMenuService, IAccountService accountService)
    {
        _productItemInMenuService = productItemInMenuService;
        _accountService = accountService;
    }
    
    [HttpGet("farm-hub/product-item-selling-percent-ratio")]
    [Authorize(Roles = "FarmHub")]
    public async Task<IActionResult> GetProductItemSellingPercentRatio(Guid businessDayId)
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
        var response = await _productItemInMenuService.GetProductItemSellingPercentRatio(Guid.Parse(defineUser.Payload.AuthorizationDecision), businessDayId);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
}
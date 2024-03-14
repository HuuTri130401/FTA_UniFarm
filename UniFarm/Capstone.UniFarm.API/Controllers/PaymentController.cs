using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers;

[Authorize]
public class PaymentController : BaseController
{
    private readonly IPaymentService _paymentService;
    private readonly IAccountService _accountService;

    public PaymentController(IPaymentService paymentService, IAccountService accountService)
    {
        _paymentService = paymentService;
        _accountService = accountService;
    }

    [HttpPost("payment")]
    public async Task<IActionResult> CreatePayment(PaymentRequestCreate paymentRequest)
    {
        if (ModelState.IsValid)
        {
            var response = await _paymentService.CreatePayment(paymentRequest);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        return BadRequest("Model is invalid");
    }


    [HttpGet("payments")]
    [SwaggerOperation(Summary = "Get payments by accountId - Customer - Done {Tien}")]
    public async Task<IActionResult> GetPayments(
        [FromQuery] string? status,
        [FromQuery] bool? isAscending,
        [FromQuery] string? orderBy = null,
        [FromQuery] string[]? includeProperties = null,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10)
    {
        string authHeader = HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized();
        }

        // The token is prefixed with "Bearer ", so we need to remove that prefix
        string token = authHeader.Replace("Bearer ", "");

        var defineUser = _accountService.GetIdAndRoleFromToken(token);
        if (defineUser.Payload == null) return HandleErrorResponse(defineUser!.Errors);

        var response = await _paymentService.GetAllForCustomer(
            isAscending, 
            orderBy, 
            filterWallet: null,
            filterAccount: x => x.Id == defineUser.Payload!.Id,
            filterPayment: x => string.IsNullOrEmpty(status) || x.Status == status,
            includeProperties, 
            pageIndex, pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
    
    [HttpGet("admin/payments")]
    [SwaggerOperation(Summary = "Get payments by accountId - Admin - Done {Tien}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllPayments(
        [FromQuery] Guid? accountId,
        [FromQuery] string? status,
        [FromQuery] bool? isAscending,
        [FromQuery] string? orderBy = null,
        [FromQuery] string[]? includeProperties = null,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10)
    {
        var response = await _paymentService.GetAll(
            isAscending, 
            orderBy, 
            filterWallet: x => Guid.Empty == accountId && x.AccountId == accountId,
            filterAccount: null,
            filterPayment: x => string.IsNullOrEmpty(status) || x.Status == status,
            includeProperties, 
            pageIndex, pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
}
using Capstone.UniFarm.API.Configurations;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers;

public class PaymentController : BaseController
{
    private readonly IPaymentService _paymentService;
    private readonly IAccountService _accountService;
    private readonly VNPayConfig _vnPayConfig;

    public PaymentController(IPaymentService paymentService, IAccountService accountService, VNPayConfig vnPayConfig)
    {
        _paymentService = paymentService;
        _accountService = accountService;
        _vnPayConfig = vnPayConfig;
    }

    [HttpPost]
    [Route("/payment")]
    public IActionResult ProcessPayment()
    {
        if (!_vnPayConfig.IsConfigured())
        {
            return StatusCode(500, "VNPay is not properly configured.");
        }
        string paymentUrl = GetPaymentUrl();
        return Redirect(paymentUrl);
    }

    private string GetPaymentUrl()
    {
        var version = "2.0.0";
        var command = "pay";
        var amount = "10000000";
        var tmnCode = _vnPayConfig.vnp_TmnCode;
        var createDate = DateTime.Now.ToString("yyyyMMddHHmmss");
        var returnUrl = _vnPayConfig.vnp_Returnurl;

        var paymentUrl = $"{_vnPayConfig.vnp_Url}?" +
                         $"vnp_Version={version}&" +
                         $"vnp_Command={command}&" +
                         $"vnp_TmnCode={tmnCode}&" +
                         $"vnp_Amount={amount}&" +
                         $"vnp_CreateDate={createDate}&" +
                         $"vnp_ReturnUrl={returnUrl}";
        return paymentUrl;
    }
    
    [HttpPost]
    [Route("/api/v1/vnpay/return")]
    public IActionResult VNPayReturn([FromQuery] string vnp_ResponseCode, [FromQuery] string vnp_TransactionNo)
    {
        // Process VNPay return data
        // Example: Log the response code and transaction number
        Console.WriteLine($"VNPay Response Code: {vnp_ResponseCode}");
        Console.WriteLine($"VNPay Transaction No: {vnp_TransactionNo}");

        // You can return any response here, such as an OkResult or custom response.
        return Ok();
    }



    /*[HttpPost("payment")]
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
    }*/
}